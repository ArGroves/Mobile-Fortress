using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MobileFortressClient.Physics;
using Lidgren.Network;
using MobileFortressClient.Menus;
using MobileFortressClient.Managers;
using Microsoft.Xna.Framework.Input;
using MobileFortressClient.Particles;
using MobileFortressClient.ClientObjects;

namespace MobileFortressClient
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MobileFortressClient : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static MobileFortressClient Game;
        public static string statusLine = "";

        public MobileFortressClient()
        {
            graphics = new GraphicsDeviceManager(this);
            Controls.Instance = new Controls();
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
            Game = this;
            EventInput.Initialize(this.Window);
            this.Exiting += new EventHandler<EventArgs>(Shutdown);
        }

        void Shutdown(object sender, EventArgs e)
        {
            Network.Shutdown();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Console.WriteLine("Mobile Fortress Client Alpha Build 1");

            Network.Initialize(this);
            Sector.Redria.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Resources.Load(Content);
            MenuManager.Menu = new TitleScreen();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            if (MenuManager.Menu is ShipHUD)
            {

                // TODO: Add your update logic here

                if (keyboard.IsKeyDown(Keys.Escape))
                {
                    if (Network.IsConnected) Network.Client.Disconnect("Requested by user.");
                    this.Exit();
                }
                else

                    if (Camera.isLoaded) Camera.Update();

                var HUD = (ShipHUD)MenuManager.Menu;

                Network.Process(NetTime.Now);

                Sector.Redria.Update(1f / 60f);


                if (IsActive && !IsMouseVisible)
                {
                    Controls.Instance.Check(keyboard, mouse);
                    Mouse.SetPosition(200, 200);
                }

                MenuManager.Menu.Update(gameTime, keyboard, mouse);

                base.Update(gameTime);
            }
            else
            {
                if (IsActive && !IsMouseVisible)
                {
                    MenuManager.Menu.Update(gameTime, keyboard, mouse);
                    Controls.Instance.MenuCheck(mouse, keyboard);
                }
                Network.Process(NetTime.Now);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Weather.SkyColor);

            // TODO: Add your drawing code here
            if (MenuManager.Menu is ShipHUD)
            {
                base.Draw(gameTime);
                GraphicsDevice.BlendState = BlendState.Additive;
                GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

                foreach (ClientObject obj in Sector.Redria.ClientObjects.table)
                {
                    obj.Draw();
                }
                Camera.Seaplane.Draw();

                spriteBatch.Begin();
                if (Camera.Target != null) spriteBatch.DrawString(Resources.defaultFont, statusLine, new Vector2(5, 5), Color.Red);
                MenuManager.Menu.Draw(spriteBatch);
                spriteBatch.End();
            }
            else if(Resources.isLoaded)
            {
                spriteBatch.Begin();
                MenuManager.Menu.Draw(spriteBatch);
                spriteBatch.End();
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
