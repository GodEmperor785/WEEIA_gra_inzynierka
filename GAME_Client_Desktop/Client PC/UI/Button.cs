using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Button : GuiElement, IClickable, IHasText
    { 
        public Vector2 TextPosition
        {
            get;
            set;
        }

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

        public int Id { get; set; }

        public delegate void ElementClicked();
        public delegate void ElementClickedInt(int n);
        public event ElementClicked clickEvent;
        public SpriteFont Font { get; set; }
        public event ElementClickedInt clickEventInt;
        public Button(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font) : base(origin,width,height,device,gui)
        {
            Font = font;
        }
        public override void Update()
        {
            Vector2 z = Font.MeasureString(text);
            TextPosition = new Vector2(((Origin.X + Width / 2.0f)) - z.X / 2.0f, (Origin.Y + Height / 2.0f) - z.Y / 2.0f);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Util.CreateTexture(Device,Width,Height, pixel => Color.Black),Boundary,Color.White);
            spriteBatch.DrawString(Font, Text, TextPosition, Color.Black);
        }

        public void OnClick()
        {
            if(clickEvent != null)
                clickEvent();
            else if (clickEventInt != null)
                clickEventInt(Id);
        }

        public Rectangle GetBoundary()
        {
            return Boundary;
        }
    }
}
