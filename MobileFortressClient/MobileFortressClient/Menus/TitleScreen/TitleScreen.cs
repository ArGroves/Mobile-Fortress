using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MobileFortressClient.Managers;
using MobileFortressClient.Menus.Options;

namespace MobileFortressClient.Menus
{
    class TitleScreen : BaseMenu
    {
        public float introTime = 0;
        UIElement loginButton;
        UIElement optionsButton;
        UIElement exitButton;
        UITextBox usernameBar;
        UITextBox passwordBar;
        UIElement loginConfirmButton;
        UIStandardButton loginBackButton;

        Texture2D pointerTex;
        Rectangle pointerDimensions;
        Texture2D loginMenu;
        Rectangle loginDimensions;

        Color tileColor = Color.Black;
        Color titleColor = Color.Black;

        bool loginMenuScroll = false;
        int scrolledAmt = 0;
        bool findingNetwork = false;
        float networkTime = 10;
        int lastNetTime = 10;
        string netFailString = "No Network Found.";

        public void LoginClick(UIElement element)
        {
            Resources.Menus.Title.MenuConfirm.Play();
            loginMenuScroll = true;
        }

        public void LoginCancel(UIElement element)
        {
            Resources.Menus.Title.MenuCancel.Play();
            loginMenuScroll = false;
        }

        public void OptionsClick(UIElement element)
        {
            Resources.Menus.Title.MenuConfirm.Play();
            MenuManager.Menu = new OptionsMenu();
        }

        public void ExitClick(UIElement element)
        {
            Resources.Menus.Title.MenuCancel.Play();
            introTime = -1 - (float)Resources.Menus.Title.MenuCancel.Duration.TotalSeconds;
        }

        public void ConfirmClick(UIElement element)
        {
            Resources.Menus.Title.MenuSelect.Play();
            introTime = 12;
            findingNetwork = true;
            Network.Username = usernameBar.TrueContents;
            Network.Password = passwordBar.TrueContents;
        }

        public void MouseOver(UIElement element)
        {
            Resources.Menus.Title.MenuSelect.Play();
            element.color = Color.White;
        }

        public void MouseOff(UIElement element)
        {
            element.color = Color.LightGray;
        }

        public void Login()
        {
            Resources.Menus.Title.MenuConfirm.Play();
            findingNetwork = false;
        }

        public void WrongPassword()
        {
            Resources.Menus.Title.MenuDeny.Play();
            findingNetwork = false;
        }

        public override void Initialize()
        {
            Viewport viewport = MobileFortressClient.Game.GraphicsDevice.Viewport;
            Texture2D tex = Resources.Menus.Title.LoginButton;
            Rectangle rect = new Rectangle(0, 290, tex.Width, tex.Height);
            loginButton = new UIElement(this, tex, rect);
            loginButton.Clicked += LoginClick;
            loginButton.MouseOver += MouseOver;
            loginButton.MouseOff += MouseOff;
            loginButton.color = Color.Black;
            Manager.Elements.Add(loginButton);

            tex = Resources.Menus.Title.OptionsButton;
            rect = new Rectangle(0, 370, tex.Width, tex.Height);
            optionsButton = new UIElement(this, tex, rect);
            optionsButton.Clicked += OptionsClick;
            optionsButton.MouseOver += MouseOver;
            optionsButton.MouseOff += MouseOff;
            optionsButton.color = Color.Black;
            Manager.Elements.Add(optionsButton);

            tex = Resources.Menus.Title.ExitButton;
            rect = new Rectangle(0, 450, tex.Width, tex.Height);
            exitButton = new UIElement(this, tex, rect);
            exitButton.Clicked += ExitClick;
            exitButton.MouseOver += MouseOver;
            exitButton.MouseOff += MouseOff;
            exitButton.color = Color.Black;
            Manager.Elements.Add(exitButton);



            pointerTex = Resources.Menus.Title.Pointer;
            pointerDimensions = new Rectangle(0, 0, pointerTex.Width, pointerTex.Height);

            loginMenu = Resources.Menus.Title.LoginMenu;
            loginDimensions = new Rectangle(viewport.Width / 2 - loginMenu.Width / 2, viewport.Height, loginMenu.Width, loginMenu.Height);

            tex = Resources.Menus.Title.LoginConfirmButton;
            rect = new Rectangle(25, 9, tex.Width, tex.Height);
            rect.Offset(loginDimensions.Location);

            loginConfirmButton = new UIElement(this, tex, rect);
            loginConfirmButton.Clicked += ConfirmClick;

            Manager.Elements.Add(loginConfirmButton);

            tex = Resources.Menus.Title.TextBox;
            rect = new Rectangle(186, 6, tex.Width,tex.Height);
            rect.Offset(loginDimensions.Location);
            usernameBar = new UITextBox(this, tex, rect, false);
            rect = new Rectangle(186, 42, tex.Width, tex.Height);
            rect.Offset(loginDimensions.Location);
            passwordBar = new UITextBox(this, tex, rect, true);
            Manager.Elements.Add(usernameBar);
            Manager.Elements.Add(passwordBar);

            loginBackButton = new UIStandardButton(this, new Point(128, 128), "Back");
            loginBackButton.dimensions.Offset(loginDimensions.Location);
            loginBackButton.hasButton = true;
            loginBackButton.customFont = Resources.Menus.BaroqueScript16;
            loginBackButton.Clicked += LoginCancel;
            Manager.Elements.Add(loginBackButton);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyboard, Microsoft.Xna.Framework.Input.MouseState mouse)
        {
            if (findingNetwork)
            {
                if (introTime > 8.1f) introTime -= (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                networkTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (networkTime < lastNetTime)
                {
                    lastNetTime--;
                    Network.FindServer();
                    if (Network.IsConnected)
                        Login();
                }
                if (networkTime <= 0)
                {
                    findingNetwork = false;
                    lastNetTime = 10;
                    netFailString = "No Network Found.";
                }
            }
            else
            {
                introTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (networkTime < 10)
                    networkTime += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
            }
            if (introTime < 0 && introTime > -1)
                MobileFortressClient.Game.Exit();
            if (introTime > 8)
            {
                Manager.Update(gameTime,mouse);
                if (keyboard.IsKeyDown(Keys.Enter))
                {
                    if (!loginMenuScroll)
                        loginButton.DoClick();
                    else
                        loginConfirmButton.DoClick();
                }
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Escape) || keyboard.IsKeyDown(Keys.Enter))
                    introTime = 8;
            }
            Viewport viewport = MobileFortressClient.Game.GraphicsDevice.Viewport;
            if (loginMenuScroll)
            {
                Color disabledColor = new Color(96, 64, 64);
                loginButton.canClick = false;
                loginButton.color = Color.Lerp(loginButton.color, disabledColor, 0.3f);
                optionsButton.canClick = false;
                optionsButton.color = Color.Lerp(optionsButton.color, disabledColor, 0.3f);
                exitButton.canClick = false;
                exitButton.color = Color.Lerp(exitButton.color, disabledColor, 0.3f);
                if (scrolledAmt < loginMenu.Height)
                {
                    foreach (UIElement element in Manager.Elements)
                    {
                        element.dimensions.Y -= 4;
                    }
                    scrolledAmt += 4;
                }
                loginDimensions.Y = viewport.Height - scrolledAmt;
            }
            else
            {
                loginButton.canClick = true;
                loginButton.color = Color.Lerp(loginButton.color, Color.LightGray, 0.3f);
                optionsButton.canClick = true;
                optionsButton.color = Color.Lerp(optionsButton.color, Color.LightGray, 0.3f);
                exitButton.canClick = true;
                exitButton.color = Color.Lerp(exitButton.color, Color.LightGray, 0.3f);
                if (scrolledAmt > 0)
                {
                    foreach (UIElement element in Manager.Elements)
                    {
                        element.dimensions.Y += 4;
                    }
                    scrolledAmt -= 4;
                }
                loginDimensions.Y = viewport.Height - scrolledAmt;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Viewport viewport = spriteBatch.GraphicsDevice.Viewport;
            if (introTime <= 8)
            {
                spriteBatch.GraphicsDevice.Clear(Color.Black);
                Texture2D halcyonLogo = Resources.Menus.Title.HalcyonLogo;
                int x = viewport.Width / 2 - halcyonLogo.Width / 2;
                int y = viewport.Height / 2 - halcyonLogo.Height / 2;
                Color color;
                float n = Math.Min(introTime / 3, 1f);
                if (introTime >= 4)
                    n = Math.Max(1f - (introTime-4) / 3, 0f);
                color = new Color(n, n, n);
                spriteBatch.Draw(halcyonLogo, new Rectangle(x, y, halcyonLogo.Width, halcyonLogo.Height), color);
            }
            else
            {
                //float n = Math.Min((introTime - 8) / 2, 1);
                //Color color = new Color(n*1.5f, n*1.5f, n*1.5f);
                //Color buttonColor = new Color(n * 1.3f, n * 1.3f, n * 1.3f);
                if (introTime <= 10)
                {
                    loginButton.color = Color.Lerp(loginButton.color,Color.LightGray, (introTime-8)/3);
                    optionsButton.color = Color.Lerp(optionsButton.color, Color.LightGray, (introTime - 8)/3);
                    exitButton.color = Color.Lerp(exitButton.color, Color.LightGray, (introTime - 8)/3);
                    tileColor = Color.Lerp(tileColor, Color.LightGray, (introTime - 8)/3);
                    titleColor = Color.Lerp(titleColor, Color.White, (introTime - 8)/2.25f);
                }

                Texture2D tile = Resources.Menus.Title.Tile;
                int x;
                int y;
                for (x = 0; x < viewport.Width; x += 100)
                {
                    for (y = 0; y < viewport.Height+200; y += 100)
                        spriteBatch.Draw(tile, new Rectangle(x, y-scrolledAmt, 100, 100), tileColor);
                }
                Texture2D titlebar = Resources.Menus.Title.TitleBar;
                float scale = viewport.Width / titlebar.Width;
                x = viewport.Width / 2 - titlebar.Width / 2;
                y = 0-scrolledAmt;
                spriteBatch.Draw(titlebar, new Rectangle(x, y, titlebar.Width, titlebar.Height), titleColor);
                spriteBatch.Draw(loginMenu, loginDimensions, titleColor);
                Manager.Draw(spriteBatch);
                MouseState mouse = Mouse.GetState();
                pointerDimensions.Location = new Point(mouse.X, mouse.Y);
                spriteBatch.Draw(pointerTex, pointerDimensions, titleColor);
                if (findingNetwork)
                    spriteBatch.DrawString(Resources.defaultFont, "Finding Network...", new Vector2(5, 5), Color.White);
                else if (networkTime < 10)
                    spriteBatch.DrawString(Resources.defaultFont, netFailString, new Vector2(5, 5), Color.White);
            }
        }
    }
}
