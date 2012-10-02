using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.Menus
{
    class UIHealthIndicator  : UIElement
    {
        float ratio = 1f;
        public float HealthRatio
        {
            get { return ratio; }
            set
            {
                ratio = value;
                var n = (byte)(ratio*255);
                color.G = (byte)(n);
                color.R = (byte)(255 - n);
                color.B = 0;
            }
        }

        public UIHealthIndicator(BaseMenu menu, Texture2D sprite, Rectangle dimensions)
            : base(menu, sprite, dimensions)
        {
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
