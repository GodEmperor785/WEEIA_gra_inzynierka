using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Label : GuiElement, IHasText
    {
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;

            }
        }

        private Vector2 textPosition;
        public Vector2 TextPosition
        {
            get { return textPosition;}
            set { textPosition = value; }
        }

        public SpriteFont Font { get ; set ; }
        public bool TextWrappable { get; set; }
        public bool HeightDerivatingFromText { get; set; }
        public bool WidthDerivatingFromText { get; set; }

        public Label(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(origin, width, height, device, gui)
        {
            Font = font;
            TextWrappable = wrapable;
            HeightDerivatingFromText = false;
            WidthDerivatingFromText = false;
        }
        public Label(int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(width, height, device, gui)
        {
            Font = font;
            TextWrappable = wrapable;
        }
        /*
        public override void Update()
        {
            if (text != null)
            {
                Vector2 z = Font.MeasureString(text);
                if (z.X < Width)
                {
                    TextPosition = new Vector2(((TextBox.X + Width / 2.0f)) - z.X / 2.0f,
                        (TextBox.Y + Height / 2.0f) - z.Y / 2.0f);
                }
                else
                {
                    TextPosition = new Vector2(((TextBox.X )),(TextBox.Y));
                }
            }
        }
        */
        /*
        protected override String parseText(String text, SpriteFont Font)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = text.Split(' ');
            int usedHeight = 0;
            foreach (String word in wordArray)
            {
                
                if (Font.MeasureString(line + word).Length() > TextBox.Width - 10)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }
                int z = 0;
                if (line == String.Empty)
                {
                    z = (int)Font.MeasureString(line + word).Y;
                }
                
                if (((usedHeight + z) > TextBox.Height))
                {
                    line = line + word + ' ';
                    usedHeight += z;
                }
                else if (!((usedHeight += z) > TextBox.Height))
                {
                    line = line + word + ' ';
                }

                
            }

            if (HeightDerivatingFromText)
                Height = usedHeight + 20;
            return returnString + line;
        }

        */

        private void AdaptToText(String text, SpriteFont Font)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = text.Split(' ');
            int usedHeight = 0;
            foreach (String word in wordArray)
            {

                if (Font.MeasureString(line + word).Length() > TextBox.Width - 10)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }
                int z = 0;
                if (line == String.Empty)
                {
                    z = (int)Font.MeasureString(line + word).Y;
                }

                if (((usedHeight + z) > TextBox.Height))
                {
                    line = line + word + ' ';
                    usedHeight += z;
                }
                else if (!((usedHeight += z) > TextBox.Height))
                {
                    line = line + word + ' ';
                }


            }

            Height = usedHeight + 20;
        }

        private void AdaptToTextWidth(String text, SpriteFont Font)
        {
            Width =(int) Font.MeasureString(text).Length() + 20;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            this.Update();
           // spriteBatch.Begin();
            if(DrawBackground)
                spriteBatch.Draw(Texture, Boundary, Color.White);
            if(text != null)
                if(!WidthDerivatingFromText)
                    spriteBatch.DrawString(Font, parseText(Text,Font), TextPosition, Color.Black);
                else
                    spriteBatch.DrawString(Font, Text, TextPosition, Color.Black);
            //spriteBatch.End();
        }

        public override void Update()
        {
            if(HeightDerivatingFromText)
                AdaptToText(Text, Font);
            if (WidthDerivatingFromText)
                AdaptToTextWidth(Text, Font);
            Update(text,ref textPosition,Font);
            if (NeedNewTexture)
                Texture = Util.CreateTexture(Device, Width, Height, new Color(180, 180, 180),new Color(100, 100, 100), alpha : 50);
        }
    }
}
