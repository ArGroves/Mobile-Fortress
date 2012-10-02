using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileFortressClient.Menus
{
    class UIElement
    {
        public Rectangle dimensions;
        Rectangle source;
        public Color color = Color.White;
        public Texture2D sprite;
        public bool mouseOver = false;
        public bool canClick = true;

        protected BaseMenu menu;

        public delegate void ClickEvent(UIElement element);
        public delegate void RightClickEvent(UIElement element);
        public delegate void MouseOverEvent(UIElement element);
        public delegate void MouseOffEvent(UIElement element);

        public event ClickEvent Clicked;
        public event RightClickEvent RightClicked;
        public event MouseOverEvent MouseOver;
        public event MouseOffEvent MouseOff;

        public UIElement(BaseMenu menu, Texture2D sprite, Rectangle dimensions)
        {
            this.menu = menu;
            this.sprite = sprite;
            this.dimensions = dimensions;
        }

        public UIElement(BaseMenu menu, Texture2D sprite, Rectangle dimensions, Rectangle source)
        {
            this.menu = menu;
            this.sprite = sprite;
            this.dimensions = dimensions;
            this.source = source;
        }

        public void DoClick()
        {
            if(Clicked != null)
                Clicked(this);
        }
        public void DoRightClick()
        {
            if(RightClicked != null)
                RightClicked(this);
        }
        public void DoMouseOver()
        {
            if (MouseOver != null)
                MouseOver(this);
        }
        public void DoMouseOff()
        {
            if (MouseOff != null)
                MouseOff(this);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(source != Rectangle.Empty)
                spriteBatch.Draw(sprite,dimensions,source,color);
            else
                spriteBatch.Draw(sprite, dimensions, color);
        }
    }
}
