using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressClient.Physics;
using Microsoft.Xna.Framework.Audio;
using MobileFortressClient.ClientObjects;

namespace MobileFortressClient
{
    class Camera
    {
        public static Matrix View;
        public static Matrix Projection;
        public static Vector3 Offset;
        public static PhysicsObj Target;

        public static AudioListener Audio = new AudioListener();

        public static bool isLoaded { get { return Target != null; } }

        public static Vector3 Position
        {
            get
            {
                if (isLoaded)
                    return Target.Position + Vector3.Transform(Offset, Target.Orientation);
                else
                    return Vector3.Zero;
            }
        }

        public static Seaplane Seaplane;

        public static void Setup(MobileFortressClient game, PhysicsObj target, Vector3 offset)
        {
            Target = target;
            Offset = offset;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), game.GraphicsDevice.Viewport.AspectRatio, 0.1f, 700f);
            Seaplane = new Seaplane();
        }
        public static void CustomizationSetup(MobileFortressClient game, Vector3 offset)
        {
            Offset = offset;
            float aspectRatio = 4f / 3f;
            float height = .09f;
            float vertical = -.035f;
            Projection = Matrix.CreatePerspectiveOffCenter(-height * aspectRatio, height * aspectRatio, -height+vertical, height+vertical, 0.1f, 700f);
            View = Matrix.CreateLookAt(Offset, Vector3.Zero, Vector3.Up);
        }

        public static void Update()
        {
            Vector3 upVector = Vector3.Up;
            upVector += Target.Entity.WorldTransform.Forward;
            View = Matrix.CreateLookAt(Position, Target.Position, upVector);
            Audio.Position = Target.Position/Resources.AudioPositionQuotient;
            Audio.Forward = Target.worldTransform.Forward;
            Audio.Up = Target.worldTransform.Up;
            Audio.Velocity = Target.Entity.LinearVelocity;
        }

    }
}
