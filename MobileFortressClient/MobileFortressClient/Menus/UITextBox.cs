using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MobileFortressClient.Menus
{
    class UITextBox : UIElement
    {
        string contents = "";
        public string Contents
        {
            get
            {
                if (isObfuscated) return obfuscatedContents;
                else return contents;
            }
            set
            {
                contents = value;
                if (isObfuscated)
                {
                    obfuscatedContents = "";
                    for (int i = 0; i < contents.Length; i++)
                    {
                        obfuscatedContents += "*";
                    }
                }
                    
            }
        }
        string obfuscatedContents = "";
        bool isActive
        {
            get
            {
                return Controls.Instance.activeText == this && Controls.Instance.acceptTextInput;
            }
        }
        bool isObfuscated = false;
        public int maxLength = 21;
        public string TrueContents
        {
            get { return contents; }
        }

        bool barBlink = false;
        float blinkInterval = 0;

        public UITextBox(BaseMenu menu, Texture2D sprite, Rectangle dimensions, bool isobfuscated) : base(menu, sprite,dimensions)
        {
            this.Clicked += new ClickEvent(UITextBox_Clicked);
            isObfuscated = isobfuscated;
        }

        void UITextBox_Clicked(UIElement element)
        {
            Controls.Instance.SetActiveTextBox(this);
        }

        public void ToNextBox()
        {
            int nextIndex = this.menu.Manager.Elements.LastIndexOf(this)+1;
            if (nextIndex > menu.Manager.Elements.Count - 1)
                nextIndex = 0;
            UIElement nextElement;
            while(!((nextElement = this.menu.Manager.Elements[nextIndex]) is UITextBox))
            {
                if (nextIndex >= menu.Manager.Elements.Count - 1)
                    nextIndex = 0;
                else
                    nextIndex++;
            }
            Controls.Instance.SetActiveTextBox((UITextBox)nextElement);
        }

        public void UpdateBlink(GameTime gameTime)
        {
            blinkInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (blinkInterval <= 0)
            {
                barBlink = !barBlink;
                blinkInterval = .25f;
            }
            if (!isActive) barBlink = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            SpriteFont Font = Resources.defaultFont;
            int endOfStringOffset = (int)Font.MeasureString("*").X*Controls.Instance.textEditPosition - 6;
            spriteBatch.DrawString(Font, Contents, new Vector2(dimensions.X+6, dimensions.Y), color);
            if(isActive && barBlink) spriteBatch.DrawString(Font, "|", new Vector2(dimensions.X + endOfStringOffset+6, dimensions.Y), color);
        }
    }
}
