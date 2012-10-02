using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.Menus
{
    class UIAmmoIndicator : UIElement
    {
        int ammo;
        bool reloaded = true;
        public int Ammo
        {
            get { return ammo; }
            set
            {
                if (value - ammo > 0 && !reloaded)
                {
                    Resources.Sounds.Reload.Play();
                    reloaded = true;
                }
                if (value == 0 && reloaded)
                {
                    reloaded = false;
                    Resources.Sounds.Latch.Play();
                }
                ammo = value;
            }
        }

        public UIAmmoIndicator(ShipHUD menu, Texture2D tex, Rectangle dim)
            : base(menu, tex, dim)
        {
            color = menu.HUDColor;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var HUD = (ShipHUD)menu;
            spriteBatch.DrawString(Resources.defaultFont, Ammo.ToString(), new Vector2(dimensions.X + dimensions.Width + 10, dimensions.Y), HUD.HUDColor);
        }
    }
}
