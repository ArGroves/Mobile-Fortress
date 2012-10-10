using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Messages;

namespace MobileFortressServer.Ships
{
    class ControlMechanism
    {
        public bool leftMouse { get; private set; }
        public bool rightMouse { get; private set; }
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Left { get; private set; }
        public bool Right { get; private set; }
        public bool ThrottleUp { get; private set; }
        public bool ThrottleDown { get; private set; }
        public bool Eject { get; private set; }

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float Roll { get; private set; }

        public void ReceiveMessage(ControlKey key, bool edge)
        {
            switch (key)
            {
                case ControlKey.Down:
                    Down = edge;
                    break;
                case ControlKey.Left:
                    Left = edge;
                    break;
                case ControlKey.LeftMouse:
                    leftMouse = edge;
                    break;
                case ControlKey.Right:
                    Right = edge;
                    break;
                case ControlKey.RightMouse:
                    rightMouse = edge;
                    break;
                case ControlKey.Up:
                    Up = edge;
                    break;
                case ControlKey.ThrottleDown:
                    ThrottleDown = edge;
                    break;
                case ControlKey.ThrottleUp:
                    ThrottleUp = edge;
                    break;
                case ControlKey.Eject:
                    Eject = edge;
                    break;
            }
        }
        public void ReceiveMessage(float Pitch, float Yaw, float Roll)
        {
            this.Yaw = Yaw;
            this.Pitch = Pitch;
            this.Roll = Roll;
        }
    }
}
