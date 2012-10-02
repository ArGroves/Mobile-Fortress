using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MobileFortressClient.Managers;

namespace MobileFortressClient.Menus
{
    abstract class BaseMenu
    {
        public MenuManager Manager = new MenuManager();

        public BaseMenu()
        {
            Initialize();
        }

        public abstract void Update(GameTime gameTime, KeyboardState keyboard, MouseState mouse);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Initialize();
    }
}
