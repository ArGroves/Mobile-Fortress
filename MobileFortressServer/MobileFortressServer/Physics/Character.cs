using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressServer.Character;

namespace MobileFortressServer.Physics
{
    class Character : PhysicsObj
    {
        public CharacterController Controller;
        public Character(Vector3 position, Quaternion orientation)
            : base()
        {
            Controller = new CharacterController(position, 1.15f, 0.25f, 0.1f, 15);
            Sector.Redria.Space.Add(Controller);
            SetEntity(Controller.Body, 5);
            Controller.Activate();
            Sector.Redria.Add(this);
        }
    }
}
