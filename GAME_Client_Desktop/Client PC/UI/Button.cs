using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Button : GuiElement, IClickable
    {
        private GraphicsDevice Device { get; set; }
        private GUI Gui { get; set; }
        private Vector2 TextPosition;
        public string text;
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
        public delegate void ElementClicked();

        public event ElementClicked clickEvent;
        public Button(Point origin, int width, int height, GraphicsDevice device, GUI gui)
        {
            Origin = origin;
            Width = width;
            Height = height;
            Device = device;
            Gui = gui;
        }
        public override void Update()
        {
            Vector2 z = Gui.bigFont.MeasureString(text);
            TextPosition = new Vector2(((Origin.X + Width / 2.0f)) - z.X / 2.0f, (Origin.Y + Height / 2.0f) - z.Y / 2.0f);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Util.CreateTexture(Device,Width,Height, pixel => Color.Black),Boundary,Color.White);
            spriteBatch.DrawString(Gui.bigFont, Text, TextPosition, Color.Black);
        }

        public void OnClick()
        {
            if(clickEvent != null)
                clickEvent();
        }

        public Rectangle GetBoundary()
        {
            return Boundary;
        }
    }
}
