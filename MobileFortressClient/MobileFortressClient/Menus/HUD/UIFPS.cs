using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileFortressClient.Menus
{
    class UIFPS
    {
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public float ping = 0;

        public UIFPS()
        {
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            frameCounter++;

            string fps = string.Format("fps: {0} ping: {1}ms", frameRate, ping);
            spriteBatch.DrawString(Resources.defaultFont, fps, new Vector2(32, 32), Color.White);
        }
    }
}
