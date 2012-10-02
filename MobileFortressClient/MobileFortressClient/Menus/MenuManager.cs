using System.Collections.Generic;
using MobileFortressClient.Menus;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;

namespace MobileFortressClient.Managers
{
    class MenuManager
    {
        public static BaseMenu Menu { get; set; }

        public delegate void DClickAway();

        public event DClickAway ClickAway;

        void DoClickAway()
        {
            if (ClickAway != null)
                ClickAway();
        }

        public List<UIElement> Elements = new List<UIElement>();
        public void Update(GameTime gameTime, MouseState mouse)
        {
            if(!Controls.Instance.leftMouse && mouse.LeftButton == ButtonState.Pressed)
                Controls.Instance.acceptTextInput = false;
            bool isOverButton = false;
            foreach(UIElement element in Elements)
            {
                if (element.canClick)
                {
                    if (element.dimensions.Contains(new Point(mouse.X, mouse.Y)))
                    {
                        if (!element.mouseOver)
                            element.DoMouseOver();
                        element.mouseOver = true;
                        if (!Controls.Instance.leftMouse && mouse.LeftButton == ButtonState.Pressed)
                        {
                            element.DoClick();
                        }
                        if (!Controls.Instance.rightMouse && mouse.RightButton == ButtonState.Pressed)
                        {
                            element.DoRightClick();
                        }
                        isOverButton = true;
                    }
                    else
                    {
                        if (element.mouseOver)
                            element.DoMouseOff();
                        element.mouseOver = false;
                    }
                }
                if (element is UITextBox)
                {
                    UITextBox textBox = (UITextBox)element;
                    textBox.UpdateBlink(gameTime);
                }
            }
            if (!isOverButton && !Controls.Instance.leftMouse && mouse.LeftButton == ButtonState.Pressed)
            {
                DoClickAway();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(UIElement element in Elements)
            {
                element.Draw(spriteBatch);
            }
        }
    }
}
