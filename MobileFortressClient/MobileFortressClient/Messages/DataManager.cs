using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MobileFortressClient.Data;
using MobileFortressClient.Ships;
using Microsoft.Xna.Framework;
using MobileFortressClient.Menus;
using MobileFortressClient.Managers;
using MobileFortressClient.Physics;
using MobileFortressClient.MobileObjects;
using Microsoft.Xna.Framework.Audio;
using MobileFortressClient.ClientObjects;

namespace MobileFortressClient.Messages
{
    abstract class DataManager
    {
        public abstract void HandleData(NetIncomingMessage msg);
    }

    class ShipDataManager : DataManager
    {
        public override void HandleData(NetIncomingMessage msg)
        {
            NetEntityType Type;
            ushort ID;
            Vector3 Position;
            Vector3 Velocity;
            Quaternion Orientation;
            NetMsgType datatype = (NetMsgType)msg.ReadByte();
            switch (datatype)
            {
                case NetMsgType.Chat:
                    Console.WriteLine(msg.ReadString());
                    break;
                case NetMsgType.CreateOnClient:
                    #region CreateOnClient
                    Type = (NetEntityType)msg.ReadByte();
                    ID = msg.ReadUInt16();
                    Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    Orientation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    ushort Resource;
                    long OwnerID;

                    //DEBUG
                    if (Type == NetEntityType.Ship)
                    {
                        int[] shipWeapons = null;
                        var shipNose = msg.ReadInt32();
                        var shipCore = msg.ReadInt32();
                        var shipTail = msg.ReadInt32();
                        int weaponLength = msg.ReadInt32();
                        if (weaponLength > 0)
                        {
                            shipWeapons = new int[weaponLength];
                            for (int i = 0; i < weaponLength; i++)
                                shipWeapons[i] = msg.ReadInt32();
                        }
                        Resource = 0;
                        OwnerID = msg.ReadInt64();
                        var shipData = new ShipData(shipNose, shipCore, shipTail);
                        WeaponsFromData(shipData, shipWeapons);
                        var ship = new ShipObj(MobileFortressClient.Game, Position, Orientation, shipData);
                        ship.ID = ID;
                        if (OwnerID == Network.Client.UniqueIdentifier)
                        {
                            Network.JoinedGame = true;
                            Camera.Setup(MobileFortressClient.Game, ship, new Vector3(0, 2f, 6f));
                            var HUD = new ShipHUD();
                            if (shipWeapons != null)
                            {
                                for (int i = 0; i < shipWeapons.Length; i++)
                                {
                                    HUD.MakeAmmoIndicator(shipWeapons[i]);
                                }
                            }
                            MenuManager.Menu = HUD;
                        }
                    }
                    else
                    {
                        Resource = msg.ReadUInt16();
                        if (Type == NetEntityType.Building)
                        {
                            new Building(Position, Resource);
                        }
                        else if (Type == NetEntityType.Missile)
                        {
                            var obj = new Rocket(MobileFortressClient.Game, Resource, Position, Orientation);
                            obj.ID = ID;
                        }
                        else
                        {
                            var obj = new PhysicsObj(MobileFortressClient.Game, Position, Orientation, Resource);
                            obj.ID = ID;
                        }
                    }
                    #endregion
                    break;
                case NetMsgType.MapLoading:
                    #region MapLoading
                    if (msg.ReadByte() == 0)
                    { 
                        var Seed = msg.ReadInt32();
                        var Power = msg.ReadInt32();
                        var Roughness = msg.ReadFloat();
                        Sector.Redria.Terrain.Terrain = new Heightmap(Seed, Power, Roughness);
                    }
                    else
                    {
                        var terrain = Sector.Redria.Terrain.Terrain;
                        int alterations = msg.ReadInt32();
                        Console.WriteLine(alterations + " Recieved Alterations.");
                        for (int i = 0; i < alterations; i++)
                        {
                            Rectangle flattenedArea = new Rectangle(msg.ReadInt32(),
                                msg.ReadInt32(), msg.ReadInt32(), msg.ReadInt32());
                            terrain.FlattenArea(flattenedArea);
                            
                        }
                        terrain.Graphics.ResetVertexBuffer();

                    }
                    #endregion
                    break;
                case NetMsgType.EntityUpdate:
                    #region EntityUpdate
                    Type = (NetEntityType)msg.ReadByte();
                    ID = msg.ReadUInt16();
                    Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    Velocity = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    Orientation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    if (Type == NetEntityType.Ship)
                    {
                        var ship = Sector.Redria.Ships.Retrieve(ID);
                        if (ship == null)
                        {
                            Console.WriteLine("No such ship ID: " + ID);
                            break;
                        }
                        ship.UpdateFromNet(Position, Orientation, Velocity);
                    }
                    else if (Type == NetEntityType.Missile)
                    {
                        var missile = Sector.Redria.Objects.Retrieve(ID);
                        if (missile == null) break;
                        missile.UpdateFromNet(Position, Orientation, Velocity);
                    }
                    #endregion
                    break;
                case NetMsgType.Bullet:
                    #region Bullet
                    ID = msg.ReadUInt16();
                    Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    Velocity = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    Orientation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
                    var Projectile = ProjectileData.ProjectileTable[ID];
                    var ping = Network.Client.ServerConnection.AverageRoundtripTime;
                    Projectile.Create(Position + Velocity * (ping / 2), Orientation, Velocity);
                    var Audio = new AudioEmitter();
                    Audio.Position = Position / Resources.AudioPositionQuotient;
                    Audio.Forward = Vector3.Transform(Vector3.Forward, Orientation);
                    Audio.Up = Vector3.Up;
                    Audio.Velocity = Velocity;

                    var sound = Resources.Sounds.WeaponSounds[ID].CreateInstance();
                    sound.Apply3D(Camera.Audio, Audio);
                    sound.Play();
                    #endregion
                    break;
                case NetMsgType.ShipExplode:
                    #region ShipExplode
                    ID = msg.ReadUInt16();
                    var deadship = Sector.Redria.Ships.Retrieve(ID);
                    deadship.Explode();
                    #endregion
                    break;
                case NetMsgType.Explosion:
                    #region Explosion
                    ID = msg.ReadUInt16();
                    var Radius = msg.ReadFloat();
                    var exploding = Sector.Redria.Objects.Retrieve(ID);
                    if (exploding != null)
                    {
                        new Explosion(exploding.Position, Radius);
                        exploding.Destroy();
                    }
                    #endregion
                    break;
                case NetMsgType.Lockon:
                    #region Lockon
                    if (!(MenuManager.Menu is ShipHUD))
                        break;
                    ID = msg.ReadUInt16();
                    var LockonStatus = (LockonStatus)msg.ReadByte();
                    ShipHUD Menu = (ShipHUD)MenuManager.Menu;
                    if (LockonStatus > LockonStatus.Locked)
                    {
                        if (LockonStatus == LockonStatus.EnemyLock)
                        {
                            Menu.enemyLock = true;
                        }
                        else if (LockonStatus == LockonStatus.EnemyMissile)
                        {
                            Menu.enemyLock = false;
                            Menu.enemyMissile = true;
                        }
                        else
                        {
                            Menu.enemyMissile = false;
                            Menu.enemyLock = false;
                        }
                    }
                    else
                    {
                        if (ID == ushort.MaxValue)
                        {
                            Menu.Target = null;
                            Menu.LockStatus = LockonStatus.NotLocked;
                        }
                        else
                        {
                            Menu.Target = Sector.Redria.Ships.Retrieve(ID);
                            Menu.LockStatus = LockonStatus;
                            if (Menu.LockStatus == LockonStatus.Locking)
                                Resources.Sounds.LockingOn.Play();
                            else if (Menu.LockStatus == LockonStatus.Locked)
                                Resources.Sounds.LockedOn.Play();
                        }
                    }
                    #endregion
                    break;

                case NetMsgType.ShipUpdate:
                    #region ShipUpdate
                    var Health = msg.ReadFloat();
                    var AmmoCount = msg.ReadByte();
                    var Ammo = new ushort[AmmoCount];
                    for (byte i = 0; i < AmmoCount; i++)
                    {
                        Ammo[i] = msg.ReadUInt16();
                    }
                    if (MenuManager.Menu is ShipHUD)
                    {
                        var HUD = (ShipHUD)MenuManager.Menu;
                        HUD.SetHealth((int)Health);

                        for (int i = 0; i < Ammo.Length; i++)
                            HUD.SetAmmo(i, Ammo[i]);
                        ping = msg.SenderConnection.AverageRoundtripTime * 1000;
                        HUD.SetPing(ping);
                    }
                    #endregion
                    break;
                default:
                    Console.WriteLine("Unhandled NetMsgType: " + datatype);
                    break;
            }
        }
        static void WeaponsFromData(ShipData ship, int[] weapondata)
        {
            if (weapondata != null)
            {
                for (int i = 0; i < weapondata.Length; i++)
                {
                    Vector3? freeSlot = FreeSlot(ship, ship.Nose);
                    if (freeSlot != null)
                    {
                        ship.SetWeapon((Vector3)freeSlot, WeaponData.WeaponTypes[weapondata[i]].Copy());
                        continue;
                    }
                    freeSlot = FreeSlot(ship, ship.Core);
                    if (freeSlot != null)
                    {
                        ship.SetWeapon((Vector3)freeSlot, WeaponData.WeaponTypes[weapondata[i]].Copy());
                        continue;
                    }
                    freeSlot = FreeSlot(ship, ship.Engine);
                    if (freeSlot != null)
                    {
                        ship.SetWeapon((Vector3)freeSlot, WeaponData.WeaponTypes[weapondata[i]].Copy());
                        continue;
                    }
                }
            }
        }
        static Vector3? FreeSlot(ShipData ship, PartData part)
        {
            if (part.WeaponSlots == null) return null;
            foreach (Vector3 offset in part.WeaponSlots)
            {
                if (ship.Weapons[offset] == null) return offset;
            }
            return null;
        }
    }
}
