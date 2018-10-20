using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Button : GuiElement, IClickable, IHasText
    {
        private Vector2 textPosition;
        public Vector2 TextPosition
        {
            get { return textPosition; }
            set { textPosition = value; }
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

        

        public delegate void ElementClicked();
        public delegate void ElementClickedInt(int n);
        public event ElementClicked clickEvent;
        public SpriteFont Font { get; set; }
        public bool Active { get; set; }
        public object Parent { get; set; }
        public bool ActiveChangeable { get; set; }
        public bool TextWrappable { get; set; }
        public Tooltip Tooltip { get; set; }
        public event ElementClickedInt clickEventInt;
        public Button(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(origin,width,height,device,gui)
        {
            Font = font;
            ActiveChangeable = true;
            TextWrappable = wrapable;
        }
        public override void Update()
        {
            Update(text, ref textPosition, Font);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Util.CreateTexture(Device,Width,Height, pixel => Color.Black),Boundary,Color.White);
            if (!String.IsNullOrEmpty(text))
                spriteBatch.DrawString(Font, Text, TextPosition, Color.Black);
            spriteBatch.End();
        }

        public void OnClick()
        {
            if (clickEvent != null)
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
