using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Tooltip : GuiElement, IHasText
    {
        private string text;
        private string textToShow;
        public string Text {
            get { return text; }
            set
            {
                text = value;
                if (Font.MeasureString(text).X < TextBox.Width)
                    textToShow = text;
            }
        }
        private Vector2 textPosition;
        public Vector2 TextPosition { get; set; }
        public SpriteFont Font { get; set; }
        public bool TextWrappable { get; set; }
        public bool ActiveChangeable { get; set; }
        public bool ToDraw;
        public Tooltip( int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(width, height, device, gui)
        {
            Font = font;
            ActiveChangeable = true;
            text = "";
            textToShow = "";
            TextWrappable = wrapable;
            ToDraw = false;
        }
        public override void Update()
        {
            Update(text, ref textPosition, Font);
        }

        public void Update(Point origin)
        {
            Origin = origin;
            Update(text, ref textPosition, Font);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Util.CreateTextureHollow(Device, Width, Height, pixel => Color.Black), Boundary, Color.White);
            if (!String.IsNullOrEmpty(text))
                spriteBatch.DrawString(Font, parseText(Text, Font), textPosition, Color.Black);
            spriteBatch.End();
        }
    }
}
