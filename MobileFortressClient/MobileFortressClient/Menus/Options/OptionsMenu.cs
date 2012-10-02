using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MobileFortressClient.Managers;

namespace MobileFortressClient.Menus.Options
{
    class OptionsMenu : BaseMenu
    {
        UIStandardButton OptionsTitle;

        UIStandardButton Server;
        UITextBox ServerAddress;

        UIStandardButton MouseSettings;
        UITextBox Sensitivity;


        UIStandardButton Exit;

        Viewport viewport;

        Texture2D pointerTex;
        Rectangle pointerDim;

        public override void Initialize()
        {
            viewport = MobileFortressClient.Game.GraphicsDevice.Viewport;

            Point loc = new Point(viewport.Width / 2 - 128, 32);
            OptionsTitle = new UIStandardButton(this, loc, "Options");
            OptionsTitle.customFont = Resources.Menus.BaroqueScript16;

            Texture2D tex = Resources.Menus.Title.TextBox;
            loc = new Point(32, 128);
            Server = new UIStandardButton(this, loc, "Server Address");
            Rectangle dim = new Rectangle(loc.X+Server.dimensions.Width+12,loc.Y+8,tex.Width,tex.Height);
            ServerAddress = new UITextBox(this, tex, dim, false);
            ServerAddress.Contents = Network.NetworkAddress;
            Manager.Elements.Add(ServerAddress);

            loc = new Point(32, 196);
            MouseSettings = new UIStandardButton(this, loc, "Mouse Sensitivity");

            dim = new Rectangle(loc.X + MouseSettings.dimensions.Width + 12, loc.Y + 8, tex.Width/6, tex.Height);
            Sensitivity = new UITextBox(this, tex, dim, false);
            Sensitivity.Contents = (Controls.Instance.mouseSensitivityX / 0.015f).ToString();
            Sensitivity.maxLength = 3;
            Manager.Elements.Add(Sensitivity);


            loc = new Point(32, viewport.Height - 64);
            Exit = new UIStandardButton(this, loc, "Exit");
            Exit.hasButton = true;
            Exit.customFont = Resources.Menus.BaroqueScript16;
            Exit.Clicked += new UIElement.ClickEvent(Exit_Clicked);
            Manager.Elements.Add(Exit);

            pointerTex = Resources.Menus.Title.Pointer;
            pointerDim = new Rectangle(0, 0, pointerTex.Width, pointerTex.Height);
        }

        void Exit_Clicked(UIElement element)
        {
            Network.NetworkAddress = ServerAddress.TrueContents;
            try
            {
                Controls.Instance.mouseSensitivityX = Controls.Instance.mouseSensitivityY
                    = (float)Convert.ToDouble(Sensitivity.TrueContents) * 0.015f;
            }
            catch
            {
                Controls.Instance.mouseSensitivityX = Controls.Instance.mouseSensitivityY
                    = 0.075f;
            }
            TitleScreen titleScreen = new TitleScreen();
            titleScreen.introTime = 8;
            MenuManager.Menu = titleScreen;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboard, MouseState mouse)
        {
            Manager.Update(gameTime, mouse);
            pointerDim.Location = new Point(mouse.X, mouse.Y);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D tile = Resources.Menus.Title.Tile;
            int x;
            int y;
            for (x = 0; x < viewport.Width; x += 100)
            {
                for (y = 0; y < viewport.Height + 200; y += 100)
                    spriteBatch.Draw(tile, new Rectangle(x, y, 100, 100), Color.Gray);
            }
            OptionsTitle.Draw(spriteBatch);
            ServerAddress.Draw(spriteBatch);
            Server.Draw(spriteBatch);
            Sensitivity.Draw(spriteBatch);
            MouseSettings.Draw(spriteBatch);
            Exit.Draw(spriteBatch);

            spriteBatch.Draw(pointerTex, pointerDim, Color.White);
        }
    }
}
