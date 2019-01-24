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
        public delegate void ElementClickedObject(object sender);
        public event ElementClicked clickEvent;
        public SpriteFont Font { get; set; }
        public bool Active { get; set; }
        
        public bool ActiveChangeable { get; set; }
        public bool TextWrappable { get; set; }
        public Tooltip Tooltip { get; set; }
        public bool IsOver { get; set; }
        public bool HeightDerivatingFromText { get; set; }

        public event ElementClickedInt clickEventInt;
        public event ElementClickedObject clickEventObject;
        private Texture2D inactive, over;
        public Button(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(origin,width,height,device,gui)
        {
            Font = font;
            ActiveChangeable = true;
            TextWrappable = wrapable;
        }
        public Button(int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable) : base(width, height, device, gui)
        {
            Font = font;
            ActiveChangeable = true;
            TextWrappable = wrapable;
        }
        public override void Update()
        {
            Update(text, ref textPosition, Font);
            if (NeedNewTexture)
            {
                Texture = Util.CreateTexture(Device, Width, Height, Util.OutsideColorOriginal, Util.InsideColorOriginal);
                inactive = Util.CreateTexture(Device, Width, Height, new Color(200, 200, 200), new Color(160, 160, 160));
                over = Util.CreateTexture(Device, Width, Height, new Color(20, 150, 70), new Color(100, 100, 100));
            }
            
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            if (DrawBackground)
            {
                if (IsOver)
                {
                    if (Active)
                    {
                        spriteBatch.Draw(over, Boundary, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(inactive, Boundary, Color.White);
                    }
                }
                else if (Active)
                {
                    spriteBatch.Draw(Texture, Boundary, Color.White);

                }
                else
                {
                    spriteBatch.Draw(inactive, Boundary, Color.White);
                }
                
            }

            if (!String.IsNullOrEmpty(text))
                spriteBatch.DrawString(Font, Text, TextPosition, Color.Black);
            //spriteBatch.End();
        }

        public void OnClick()
        {
            if (clickEvent != null)
                clickEvent();
            else if (clickEventInt != null)
                clickEventInt(Id);
            else if (clickEventObject != null)
                clickEventObject(this);
            
        }

        public Rectangle GetBoundary()
        {
            return Boundary;
        }
    }
}
