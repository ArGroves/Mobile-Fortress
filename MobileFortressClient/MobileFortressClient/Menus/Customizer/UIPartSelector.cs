using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Data;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.Menus.Customizer
{
    class UIPartSelector : UIElement
    {
        static int nameLength = (int)Resources.Menus.ButtonFont.MeasureString("123456789012345").X;
        public UIStandardButton NameBar;
        public UIStandardButton TypeBar;
        //UIElement weaponDescriptionBox;
        public UIElement UpArrow;
        public UIElement DownArrow; 
        PartData part;
        public PartData Part
        {
            get { return part; }
            set
            {
                part = value;
                if(part.Name != null)
                    NameBar.Text = part.Name;
                fitDesc = part.Description;
            }
        }
        public string fitDesc;
        public string PartType
        {
            get { return TypeBar.Text; }
            set { TypeBar.Text = value; }
        }

        public UIPartSelector(BaseMenu menu, Point position)
            : base(menu, null, new Rectangle(position.X, position.Y, 0, 0))
        {
            TypeBar = new UIStandardButton(menu, new Point(position.X,position.Y), "Nose");
            TypeBar.ForcedLength = (int)Resources.Menus.ButtonFont.MeasureString("012345").X;
            TypeBar.dimensions = new Rectangle(position.X, position.Y, TypeBar.ForcedLength + 36 * 2, 48);
            int x = TypeBar.dimensions.X + TypeBar.dimensions.Width - 24;
            NameBar = new UIStandardButton(menu, new Point(x, position.Y), "No Part");
            NameBar.ForcedLength = nameLength;
            NameBar.isDisplay = true;
            NameBar.dimensions = new Rectangle(x, position.Y, NameBar.ForcedLength + 36 * 2, 48);
            menu.Manager.Elements.Add(NameBar);

            /*tex = Resources.Menus.Customization.DescriptionScreen;
            dim = new Rectangle((weaponNameBar.dimensions.X+weaponNameBar.dimensions.Width)-tex.Width, position.Y + weaponGraphicBox.dimensions.Height + 8, tex.Width, tex.Height);
            weaponDescriptionBox = new UIElement(menu, tex, dim);*/

            Texture2D tex = Resources.Menus.Customization.UpArrow;
            Rectangle dim = new Rectangle(NameBar.dimensions.X + (36 * 2 + NameBar.ForcedLength) - tex.Width
                , position.Y-6, tex.Width, tex.Height);
            UpArrow = new UIElement(menu, tex, dim);
            tex = Resources.Menus.Customization.DownArrow;
            dim = new Rectangle(NameBar.dimensions.X + (36 * 2 + NameBar.ForcedLength) - tex.Width
                , position.Y+dim.Height+8, tex.Width, tex.Height);
            DownArrow = new UIElement(menu, tex, dim);
            menu.Manager.Elements.Add(UpArrow);
            menu.Manager.Elements.Add(DownArrow);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            NameBar.Draw(spriteBatch);
            TypeBar.Draw(spriteBatch);
            UpArrow.Draw(spriteBatch);
            DownArrow.Draw(spriteBatch);
        }
    
    }
}
