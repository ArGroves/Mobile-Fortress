using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileFortressClient.Menus
{
    class UIStandardButton : UIElement
    {
        public string Text = "";
        public bool isDisplay = false;
        public bool hasButton = false;
        public int ForcedLength = 0;
        public SpriteFont customFont;
        public UIStandardButton(BaseMenu menu, Point position, string text) : base(menu,null,new Rectangle(position.X,position.Y,0,0))
        {
            Text = text;
            dimensions.Width = 36 * 2 + (int)Resources.Menus.ButtonFont.MeasureString(text).X;
            dimensions.Height = 48;
        }
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (hasButton)
            {
                Texture2D tex = Resources.Menus.ButtonSquare;
                Rectangle dim = new Rectangle(dimensions.X, dimensions.Y, tex.Width, tex.Height);
                spriteBatch.Draw(tex, dim, color);
                if (isDisplay)
                    DrawStandardDisplay(spriteBatch, new Point(dimensions.X + tex.Width, dimensions.Y), Text, color, ForcedLength);
                else
                    DrawStandardButton(spriteBatch, new Point(dimensions.X + tex.Width, dimensions.Y), Text, color, ForcedLength);
            }
            else
            {
                if (isDisplay)
                    DrawStandardDisplay(spriteBatch, new Point(dimensions.X, dimensions.Y), Text, color, ForcedLength);
                else
                    DrawStandardButton(spriteBatch, new Point(dimensions.X, dimensions.Y), Text, color, ForcedLength);
            }
        }

        static Color engravingColor1 = new Color(157, 124, 43);
        static Color engravingColor2 = new Color(75, 59, 21);

        public void DrawStandardButton(SpriteBatch spriteBatch, Point position, string text)
        {
            DrawStandardButton(spriteBatch, position, text, Color.White);
        }

        public void DrawStandardButton(SpriteBatch spriteBatch, Point position, string text, Color color, int forcedLength = 0)
        {
            SpriteFont font = Resources.Menus.ButtonFont;
            if (customFont != null) font = customFont;
            int buttonHeight = 48;
            int endWidth = 36;
            int centerWidth = 16;
            Rectangle dimensions = new Rectangle(position.X, position.Y, endWidth, buttonHeight);
            Texture2D texture = Resources.Menus.ButtonLeft;
            spriteBatch.Draw(texture, dimensions, color);

            texture = Resources.Menus.ButtonCenter;
            Vector2 stringDimensions = font.MeasureString(text);
            int i = endWidth;
            for (; i < endWidth + Math.Max(stringDimensions.X, forcedLength); i += centerWidth)
            {
                dimensions = new Rectangle(position.X + i, position.Y, centerWidth, buttonHeight);
                spriteBatch.Draw(texture, dimensions, color);
            }
            texture = Resources.Menus.ButtonRight;
            dimensions = new Rectangle(position.X + i, position.Y, endWidth, buttonHeight);
            spriteBatch.Draw(texture, dimensions, color);
            spriteBatch.DrawString(font, text, new Vector2(position.X + endWidth, position.Y), ColorMul(engravingColor2, color));
            spriteBatch.DrawString(font, text, new Vector2(position.X + endWidth + 1, position.Y + 1), ColorMul(engravingColor1, color));
        }

        public void DrawStandardDisplay(SpriteBatch spriteBatch, Point position, string text, Color color, int forcedLength = 0)
        {
            SpriteFont font = Resources.Menus.ButtonFont;
            if (customFont != null) font = customFont;
            int buttonHeight = 48;
            int endWidth = 36;
            int centerWidth = 16;
            Rectangle dimensions = new Rectangle(position.X, position.Y, endWidth, buttonHeight);
            Texture2D texture = Resources.Menus.DisplayLeft;
            spriteBatch.Draw(texture, dimensions, Color.White);

            texture = Resources.Menus.DisplayCenter;
            Vector2 stringDimensions = font.MeasureString(text);
            int i = endWidth;
            for (; i < endWidth + Math.Max(stringDimensions.X, forcedLength); i += centerWidth)
            {
                dimensions = new Rectangle(position.X + i, position.Y, centerWidth, buttonHeight);
                spriteBatch.Draw(texture, dimensions, Color.White);
            }
            texture = Resources.Menus.DisplayRight;
            dimensions = new Rectangle(position.X + i, position.Y, endWidth, buttonHeight);
            spriteBatch.Draw(texture, dimensions, Color.White);
            spriteBatch.DrawString(font, text, new Vector2(position.X + endWidth, position.Y), ColorMul(Color.Gray, color));
            spriteBatch.DrawString(font, text, new Vector2(position.X + endWidth + 1, position.Y + 1), ColorMul(Color.White, color));
        }

        Color ColorMul(Color A, Color B)
        {
            return new Color(A.R * (B.R / 255), A.G * (B.G / 255), A.B * (B.B / 255));
        }
    }
}
