using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Deck : GuiElement, IClickable, IHasText
    {
        public string Text { get; set; }
        private Vector2 textPosition;
        public Vector2 TextPosition
        {
            get { return textPosition; }
            set { textPosition = value; }
        }
        public SpriteFont Font { get; set; }
        public bool TextWrappable { get; set; }
        public bool Active { get; set; }
        public bool ActiveChangeable { get; set; }
        public bool RecentlyAdded { get; set; }
        private bool focused;
        private bool lastStateOfFocus;
        public object Parent { get; set; }
        public Tooltip Tooltip { get; set; }
        public bool HeightDerivatingFromText { get; set; }
        private Fleet fleet;
        public delegate void ElementClicked(object sender);
        public event ElementClicked clickEvent;
        public bool Chosen { get; set; }
        private Texture2D inactive, over, focusedTexture;
        public Rectangle GetBoundary()
        {
            return Boundary;
        }
        public bool IsOver { get; set; }
        public void OnClick()
        {
            clickEvent(this);
            focused = true;
        }

        public void SetFleet(Fleet fleet)
        {
            this.fleet = fleet;
        }
        public List<Ship> GetShips()
        {
            return fleet.Ships;
        }

        public Fleet GetFleet()
        {
            return fleet;
        }

        public override void Update()
        {
            Update(Text, ref textPosition, Font);
            focused = Game1.self.FocusedElement == this;
            bool changeOfFocus = focused != lastStateOfFocus;
            if (NeedNewTexture)
            {
                Texture = Util.CreateTexture(Device, Width, Height, Util.OutsideColorOriginal, Util.InsideColorOriginal);
                inactive = Util.CreateTexture(Device, Width, Height, new Color(200, 200, 200), new Color(160, 160, 160));
                over = Util.CreateTexture(Device, Width, Height, new Color(20, 150, 70), new Color(100, 100, 100));
                focusedTexture = Util.CreateTexture(Device, Width, Height, new Color(140, 140, 140),
                    new Color(60, 60, 60));
            }
            /*
            if (focused)
            {
                if(changeOfFocus || NeedNewTexture)
                Texture = Util.CreateTexture(Device, Width, Height, new Color(140, 140, 140),
                    new Color(60, 60, 60));
            }
            else
            {
                if(changeOfFocus || NeedNewTexture)
                Texture = Util.CreateTexture(Device, Width, Height, new Color(180, 180, 180),
                    new Color(100, 100, 100));
            }
            */
            lastStateOfFocus = focused;
        }

        public void AddShip(Ship s)
        {
            fleet.Ships.Add(s);
        }

        public Deck(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable,string name) 
            : base(origin, width, height, device, gui)
        {
            Font = font;
            TextWrappable = wrapable;
            Text = name;
            fleet = new Fleet();
            fleet.Ships = new List<Ship>();
            focused = true;
        }

        public override void Draw(SpriteBatch sp)
        {
            //sp.Begin();
            



            if (Chosen)
            {
                sp.Draw(focusedTexture, Boundary, Color.White);
            }
            else
            {
                if (IsOver)
                {
                    sp.Draw(over,Boundary,Color.White);
                }
                else
                {
                    sp.Draw(Texture, Boundary, Color.White);
                    
                }
            }
            if (!String.IsNullOrEmpty(Text))
                sp.DrawString(Font, Text, TextPosition, Color.Black);
            //sp.End();
        }
    }
}
