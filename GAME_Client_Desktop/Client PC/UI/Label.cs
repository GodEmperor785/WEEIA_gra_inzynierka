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

        public Label(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(origin, width, height, device, gui)
        {
            Font = font;
            TextWrappable = wrapable;
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
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            this.Update();
           // spriteBatch.Begin();
            if(DrawBackground)
                spriteBatch.Draw(Texture, Boundary, Color.White);
            if(text != null)
                spriteBatch.DrawString(Font, parseText(Text,Font), TextPosition, Color.Black);
            //spriteBatch.End();
        }

        public override void Update()
        {
            Update(text,ref textPosition,Font);
            if (NeedNewTexture)
                Texture = Util.CreateTexture(Device, Width, Height);
        }
    }
}
