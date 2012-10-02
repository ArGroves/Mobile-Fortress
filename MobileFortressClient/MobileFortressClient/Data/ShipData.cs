using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Entities;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace MobileFortressClient.Data
{
    class ShipData
    {
        public Dictionary<Vector3, WeaponData> Weapons = new Dictionary<Vector3, WeaponData>();
        public PartData Nose { get; private set; }
        public PartData Core { get; private set; }
        public PartData Engine { get; private set; }

        public int NoseID { get; private set; }
        public int CoreID { get; private set; }
        public int EngineID { get; private set; }

        public Entity Hitbox { get; private set; }

        public Color NoseColor, CoreColor, TailColor, WeaponColor;

        float weaponWeight = 0;

        public int Thrust { get { return Nose.Thrust + Core.Thrust + Engine.Thrust; } }
        public int StrafeVelocity { get { return Nose.Strafe + Core.Strafe + Engine.Strafe; } }
        public int Turn { get { return Nose.Turn + Core.Turn + Engine.Turn; } }

        public ShipData(int noseIndex, int coreIndex, int engineIndex)
        {
            SetNose(noseIndex);
            SetCore(coreIndex);
            SetEngine(engineIndex);
            NoseColor = Color.Red;
            CoreColor = Color.Red;
            TailColor = Color.Red;

            WeaponColor = Color.White;
        }

        public float TotalWeight
        {
            get
            {
                return Nose.Weight + Engine.Weight + weaponWeight;
            }
        }
        public float CapacityWeight
        {
            get { return Core.Weight; }
        }

        public bool IsOverweight
        {
            get { return TotalWeight > CapacityWeight; }
        }

        public float TotalArmor
        {
            get { return Nose.Armor + Core.Armor + Engine.Armor; }
        }

        public void SetNose(int partID)
        {
            NoseID = partID;
            PartData part = PartData.Noses[partID].Copy();
            if (Nose != null)
            {
                if (Nose.WeaponSlots != null)
                {
                    foreach (Vector3 pos in Nose.WeaponSlots)
                    {
                        if(Weapons[pos] != null) weaponWeight -= Weapons[pos].Weight;
                        Weapons.Remove(pos);
                    }
                }

            }
            Nose = part;
            if (part.WeaponSlots != null)
            {
                foreach (Vector3 pos in part.WeaponSlots)
                {
                    Weapons.Add(pos, null);
                }
            }
        }

        public void SetCore(int partID)
        {
            CoreID = partID;
            PartData part = PartData.Cores[partID].Copy();
            if (Core != null)
            {
                if (Core.WeaponSlots != null)
                {
                    foreach (Vector3 pos in Core.WeaponSlots)
                    {
                        if (Weapons[pos] != null) weaponWeight -= Weapons[pos].Weight;
                        Weapons.Remove(pos);
                    }
                }

            }
            Core = part;
            if (part.WeaponSlots != null)
            {
                foreach (Vector3 pos in part.WeaponSlots)
                {
                    Weapons.Add(pos, null);
                }
            }
        }

        public void SetEngine(int partID)
        {
            EngineID = partID;
            PartData part = PartData.Engines[partID].Copy();
            if (Engine != null)
            {
                if (Engine.WeaponSlots != null)
                {
                    foreach (Vector3 pos in Engine.WeaponSlots)
                    {
                        if (Weapons[pos] != null) weaponWeight -= Weapons[pos].Weight;
                        Weapons.Remove(pos);
                    }
                }

            }
            Engine = part;
            if (part.WeaponSlots != null)
            {
                foreach (Vector3 pos in part.WeaponSlots)
                {
                    Weapons.Add(pos, null);
                }
            }
        }

        public void SetWeapon(Vector3 slot, WeaponData weapon)
        {
            WeaponData currentWeapon = Weapons[slot];
            if (currentWeapon != null)
            {
                weaponWeight -= currentWeapon.Weight;
            }
            if(weapon != null) weaponWeight += weapon.Weight;
            Weapons[slot] = weapon;
        }

        public void SetDrawingResources(out GraphicsResource nose, out GraphicsResource core, out GraphicsResource engine)
        {
            nose = Nose.Resource;
            core = Core.Resource;
            engine = Engine.Resource;
        }
        public void ComposeHitbox()
        {
            Hitbox = new CompoundBody(
                new List<CompoundShapeEntry>{
                    new CompoundShapeEntry(new BoxShape(2f, 2f, 6f),Vector3.Zero),
                    new CompoundShapeEntry(new BoxShape(Core.WingVector.X, 0.1f, Core.WingVector.Z),
                        new Vector3(0,Core.WingVector.Y,0))
                }
                , TotalWeight);
        }

    }
}
