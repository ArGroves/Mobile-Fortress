using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using BEPUphysics.EntityStateManagement;

namespace MobileFortressServer.Physics
{
    class PhysicsObj : WorldObj
    {
        enum PhysicsObjType { Terrain = 0, Ship = 1 }
        public Entity Entity { get; set; }

        public override Vector3 Position
        {
            get
            {
                return Entity.Position;
            }
        }

        public override Quaternion Orientation
        {
            get
            {
                return Entity.Orientation;
            }
        }
        public override Vector3 Velocity
        {
            get
            {
                return Entity.LinearVelocity;
            }
        }

        public PhysicsObj(Entity entity, UInt16 resource)
        {
            Entity = entity;
            Entity.CollisionInformation.Tag = this;
            resource_index = resource;
            Sector.Redria.Add(this);
        }
        public PhysicsObj()
        {
        }

        public void SetEntity(Entity entity, UInt16 resource)
        {
            Entity = entity;
            Entity.CollisionInformation.Tag = this;
            resource_index = resource;
            Sector.Redria.Add(this);
        }

        public virtual void Update(float dt)
        {
        }

        public static MotionState CreateMotionState(Vector3 position, Quaternion orientation, Vector3 velocity)
        {
            MotionState newState = new MotionState();
            newState.Position = position;
            newState.Orientation = orientation;
            newState.LinearVelocity = velocity;
            return newState;
        }
        public static MotionState CreateMotionState(Vector3 position, Quaternion orientation)
        {
            MotionState newState = new MotionState();
            newState.Position = position;
            newState.Orientation = orientation;
            return newState;
        }
    }
}
