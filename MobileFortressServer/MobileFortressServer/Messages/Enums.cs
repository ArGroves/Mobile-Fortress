using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace MobileFortressServer.Messages
{
    enum NetMsgType {
        Chat = 0, Control = 1, ControlUpdate = 7, Login = 8,
        EntityUpdate = 2, ShipUpdate = 3,
        CreateOnClient = 4, ShipDataOutput = 9, Bullet = 5, ShipExplode = 6,
        Explosion = 10, Lockon = 11, MapLoading = 12
    }
    enum NetEntityType
    {
        Ground = 0, Ship = 1, Bullet = 2, Missile = 3, Building = 4
    }
    enum MapMsgType
    {
        Create = 0, Flatten = 1
    }
    enum LockonStatus { NotLocked = 0, Locking = 1, Locked = 2, EnemyLock = 3, EnemyMissile = 4, MissileDestroyed = 5 }
    enum ControlKey
    {
        None = 0,
        LeftMouse = 1, RightMouse = 2,
        Up = 3, Down = 4, Left = 5, Right = 6
    }
}
