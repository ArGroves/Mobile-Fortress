using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MobileFortressClient.Physics;
using MobileFortressClient.Menus;
using MobileFortressClient.Ships;
using Microsoft.Xna.Framework.Graphics;

namespace MobileFortressClient
{
    class Controls
    {
        public static Controls Instance;

        public bool leftMouse = false;
        public bool rightMouse = false;
        public bool Up = false;
        public bool Down = false;
        public bool Left = false;
        public bool Right = false;
        public bool Shift = false;
        public float deltaX = 0;
        public float deltaY = 0;

        public float Pitch = 0;
        public float Yaw = 0;
        public float Roll = 0;

        public float mouseSensitivityX = 0.075f;
        public float mouseSensitivityY = 0.075f;

        public UITextBox activeText;
        public bool acceptTextInput = false;
        public int textEditPosition { get; private set; }

        bool leftArrow = false;
        bool rightArrow = false;

        public Controls()
        {
            EventInput.CharEntered += new CharEnteredHandler(EventInput_CharEntered);
        }

        void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            char character = e.Character;
            if (character == 'n' && Network.JoinedGame)
            {
                Weather.Toggle();
            }
            if (acceptTextInput && activeText != null)
            {
                if (character == 8)
                {
                    if (activeText.TrueContents.Length != 0)
                    {
                        activeText.Contents = activeText.TrueContents.Remove(this.textEditPosition - 1, 1);
                        textEditPosition--;
                    }
                }
                else if (((character > 36 && character < 126) || character == 32) && activeText.TrueContents.Length < activeText.maxLength)
                {
                    if (this.textEditPosition < activeText.TrueContents.Length)
                        activeText.Contents = activeText.TrueContents.Substring(0, this.textEditPosition) +
                            character + activeText.TrueContents.Substring(this.textEditPosition);
                    else
                        activeText.Contents = activeText.TrueContents.Substring(0, this.textEditPosition) + character;
                    textEditPosition++;
                }
                else
                {
                    activeText.ToNextBox();
                }
            }
        }

        public void SetActiveTextBox(UITextBox box)
        {
            acceptTextInput = true;
            activeText = box;
            textEditPosition = box.TrueContents.Length;
        }

        public void Check(KeyboardState keyState, MouseState mouseState)
        {
            CheckMouse(mouseState);
            CheckKeyboard(keyState);
        }

        void CheckMouse(MouseState mouse)
        {
            if (!leftMouse && mouse.LeftButton == ButtonState.Pressed)
            {
                leftMouse = true;
                Network.SendControlMsg(true, ControlKey.LeftMouse);
            }
            if (leftMouse && mouse.LeftButton == ButtonState.Released)
            {
                leftMouse = false;
                Network.SendControlMsg(false, ControlKey.LeftMouse);
            }

            if (!rightMouse && mouse.RightButton == ButtonState.Pressed)
            {
                rightMouse = true;
                Network.SendControlMsg(true, ControlKey.RightMouse);
            }
            if (rightMouse && mouse.RightButton == ButtonState.Released)
            {
                rightMouse = false;
                Network.SendControlMsg(false, ControlKey.RightMouse);
            }

            deltaX = (200 - mouse.X) * mouseSensitivityX;
            deltaY = (mouse.Y - 200) * mouseSensitivityY;
            Pitch = MathHelper.Clamp(Pitch*0.9f + (Pitch + deltaY)*0.1f, -MathHelper.PiOver2 * .95f, MathHelper.PiOver2 * .95f);
            Yaw = Yaw * 0.95f + (Yaw + deltaX) * 0.05f;
            Roll = MathHelper.Clamp(Roll*0.95f + (deltaX)*0.10f, -MathHelper.PiOver4, MathHelper.PiOver4);
        }
        void CheckKeyboard(KeyboardState keyboard)
        {
            if (!Up && keyboard.IsKeyDown(Keys.W))
            {
                Up = true;
                Network.SendControlMsg(Up, ControlKey.Up);
            }
            if (Up && keyboard.IsKeyUp(Keys.W))
            {
                Up = false;
                Network.SendControlMsg(Up, ControlKey.Up);
            }
            if (!Down && keyboard.IsKeyDown(Keys.S))
            {
                Down = true;
                Network.SendControlMsg(Down, ControlKey.Down);
            }
            if (Down && keyboard.IsKeyUp(Keys.S))
            {
                Down = false;
                Network.SendControlMsg(Down, ControlKey.Down);
            }
            if (!Left && keyboard.IsKeyDown(Keys.A))
            {
                Left = true;
                Network.SendControlMsg(Left, ControlKey.Left);
            }
            if (Left && keyboard.IsKeyUp(Keys.A))
            {
                Left = false;
                Network.SendControlMsg(Left, ControlKey.Left);
            }
            if (!Right && keyboard.IsKeyDown(Keys.D))
            {
                Right = true;
                Network.SendControlMsg(Right, ControlKey.Right);
            }
            if (Right && keyboard.IsKeyUp(Keys.D))
            {
                Right = false;
                Network.SendControlMsg(Right, ControlKey.Right);
            }

            if (!Shift && keyboard.IsKeyDown(Keys.LeftShift))
            {
                Shift = true;
            }
            if (Shift && keyboard.IsKeyUp(Keys.LeftShift))
            {
                Shift = false;
            }
        }

        public void MenuCheck(MouseState mouse, KeyboardState keyboard)
        {
            if (!leftMouse && mouse.LeftButton == ButtonState.Pressed)
            {
                leftMouse = true;
            }
            if (leftMouse && mouse.LeftButton == ButtonState.Released)
            {
                leftMouse = false;
            }

            if (!rightMouse && mouse.RightButton == ButtonState.Pressed)
            {
                rightMouse = true;
            }
            if (rightMouse && mouse.RightButton == ButtonState.Released)
            {
                rightMouse = false;
            }

            if (acceptTextInput)
            {
                if (!leftArrow && keyboard.IsKeyDown(Keys.Left))
                {
                    leftArrow = true;
                    if (textEditPosition > 0)
                        textEditPosition--;
                }
                if(!rightArrow && keyboard.IsKeyDown(Keys.Right))
                {
                    rightArrow = true;
                    if(textEditPosition < activeText.Contents.Length)
                        textEditPosition++;
                }
                if (leftArrow && keyboard.IsKeyUp(Keys.Left))
                {
                    leftArrow = false;
                }
                if (rightArrow && keyboard.IsKeyUp(Keys.Right))
                {
                    rightArrow = false;
                }
            }
        }

        
    }
}
