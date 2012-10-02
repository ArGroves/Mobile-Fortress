using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Data;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.Menus.Customizer
{
    class UIWeaponSelector : UIElement
    {
        static int nameLength = (int)Resources.Menus.ButtonFont.MeasureString("123456789012345").X;
        static char[] fireGroupChars = new char[] { 'L', 'R' };
        public UIStandardButton NameBar;
        public UIElement graphicBox;
        UIElement graphic;
        //UIElement weaponDescriptionBox;
        public UIElement UpArrow;
        public UIElement DownArrow; 
        WeaponData weapon;
        public bool Available
        {
            set
            {
                if (value)
                {
                    NameBar.Text = "No Weapon";
                    fitDesc = "";
                    graphicBox.color = Color.White;
                }
                else
                {
                    NameBar.Text = "";
                    fitDesc = "";
                    graphicBox.color = Color.Gray;
                }
            }
        }
        public WeaponData Weapon
        {
            get { return weapon; }
            set
            {
                weapon = value;
                Available = true;
                if (weapon != null)
                {
                    graphic.sprite = Resources.Menus.HUD.WeaponIcon[weapon.Index];
                    graphic.dimensions = new Rectangle(graphicBox.dimensions.X + 
                        (graphicBox.dimensions.Width / 2 - graphic.sprite.Width / 2),
                        graphicBox.dimensions.Y + (graphicBox.dimensions.Height / 2 - graphic.sprite.Height / 2),
                        graphic.sprite.Width, graphic.sprite.Height);
                    NameBar.Text = weapon.Name;
                    fitDesc = weapon.Description;
                }
                else
                {
                    NameBar.Text = "No Weapon";
                    fitDesc = "";
                }
            }
        }
        string fitDesc = "";

        public UIWeaponSelector(BaseMenu menu, Point position)
            : base(menu, null, new Rectangle(position.X, position.Y, 0, 0))
        {
            Texture2D tex = Resources.Menus.Customization.WeaponSelector;
            Rectangle dim = new Rectangle(position.X, position.Y, tex.Width, tex.Height);
            graphicBox = new UIElement(menu, tex, dim);
            tex = Resources.Menus.HUD.WeaponIcon[0];
            dim = new Rectangle(dim.X + (dim.Width / 2 - tex.Width / 2), dim.Y + (dim.Height / 2 - tex.Height / 2), tex.Width, tex.Height);
            graphic = new UIElement(menu, tex, dim);
            int x = graphicBox.dimensions.X + graphicBox.dimensions.Width - 24;
            int y = 8;
            NameBar = new UIStandardButton(menu, new Point(x, position.Y+y), "No Weapon");
            NameBar.ForcedLength = nameLength;
            NameBar.dimensions = new Rectangle(x, position.Y, NameBar.ForcedLength + 36 * 2, 48);
            NameBar.isDisplay = true;
            menu.Manager.Elements.Add(NameBar);

            /*tex = Resources.Menus.Customization.DescriptionScreen;
            dim = new Rectangle((weaponNameBar.dimensions.X+weaponNameBar.dimensions.Width)-tex.Width, position.Y + weaponGraphicBox.dimensions.Height + 8, tex.Width, tex.Height);
            weaponDescriptionBox = new UIElement(menu, tex, dim);*/

            tex = Resources.Menus.Customization.UpArrow;
            dim = new Rectangle(NameBar.dimensions.X + (36 * 2 + NameBar.ForcedLength) - tex.Width
                , position.Y, tex.Width, tex.Height);
            UpArrow = new UIElement(menu, tex, dim);
            tex = Resources.Menus.Customization.DownArrow;
            dim = new Rectangle(NameBar.dimensions.X + (36 * 2 + NameBar.ForcedLength) - tex.Width
                , position.Y + dim.Height + 12, tex.Width, tex.Height);
            DownArrow = new UIElement(menu, tex, dim);
            menu.Manager.Elements.Add(UpArrow);
            menu.Manager.Elements.Add(DownArrow);
            menu.Manager.Elements.Add(graphicBox);
            Available = false;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            NameBar.Draw(spriteBatch);
            graphicBox.Draw(spriteBatch);
            if (weapon != null)
            {
                graphic.Draw(spriteBatch);
                string fireGroupString = fireGroupChars[weapon.fireGroup].ToString();
                spriteBatch.DrawString(Resources.Menus.SmallDisplayFont, "Fire: "+fireGroupString,
                    new Vector2(graphicBox.dimensions.X + graphicBox.dimensions.Width - 64,
                        graphicBox.dimensions.Y + graphicBox.dimensions.Height - 32), Color.Lime);
            }
            UpArrow.Draw(spriteBatch);
            DownArrow.Draw(spriteBatch);
        }
    
    }
}
