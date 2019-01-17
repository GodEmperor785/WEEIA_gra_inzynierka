using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class InputBox : GuiElement, IClickable, IHasText
    {
        private string text;
        public string Text
        {
            get { return text;}
            set
            {
                text = value;
                if (Font.MeasureString(text).X < TextBox.Width)
                    textToShow = text;
            }
        }
        public bool IsOver { get; set; }
        private string textToShow;
        public Vector2 TextPosition { get; set; }
        private Vector2 BasicTextPosition { get; set; }
        public SpriteFont Font { get; set; }
        public bool Active { get; set; }
        public bool ActiveChangeable { get; set; }
        public object Parent { get; set; }
        public int TextLimit { get; set; }
        public bool TextWrappable { get; set; }
        public Tooltip Tooltip { get; set; }
        public bool IsPassword { get; set; }
        public bool HeightDerivatingFromText { get; set; }
        public string BasicText;
        private Texture2D focused;
        public Rectangle GetBoundary()
        {
            return Boundary;
        }
        public delegate void ElementClicked();
        public event ElementClicked clickEvent;

        public InputBox(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(origin, width, height, device, gui)
        {
            Font = font;
            ActiveChangeable = true;
            text = "";
            textToShow = "";
            TextWrappable = wrapable;
            IsPassword = false;
        }

        public override void Update()
        {
            if (IsPassword)
            {
                int length = textToShow.Length;
                string str = new string('*', length);
                textToShow = str;
            }

            Vector2 z = Font.MeasureString(textToShow);
            //  if (!String.IsNullOrEmpty(BasicText))
            {
                var z2 = Font.MeasureString(BasicText);
                BasicTextPosition = new Vector2(((Origin.X + Width / 2.0f)) - z2.X / 2.0f,
                    (Origin.Y + Height / 2.0f) - z2.Y / 2.0f);
            }
            TextPosition = new Vector2(((Origin.X + Width / 2.0f)) - z.X / 2.0f,
                (Origin.Y + Height / 2.0f) - z.Y / 2.0f);
            if (NeedNewTexture)
            {
                Texture = Util.CreateTextureHollow(Device, Width, Height);
                focused = Util.CreateTexture(Device, Width, Height, new Color(20, 150, 70), new Color(100, 100, 100));
            }
        }

        public void OnClick()
        {
            Game1.self.FocusedElement = this;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            if (Game1.self.FocusedElement == this)
                spriteBatch.Draw(focused, Boundary, Color.White);
            else
                spriteBatch.Draw(Texture, Boundary, Color.White);
            if(!String.IsNullOrEmpty(text))
                spriteBatch.DrawString(Font, textToShow, TextPosition, Color.Black);
            else if (!String.IsNullOrEmpty(BasicText))
                spriteBatch.DrawString(Font, BasicText, BasicTextPosition, Color.Gray);
            //spriteBatch.End();
        }
    }
}
