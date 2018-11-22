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
        private bool focused;
        private bool lastStateOfFocus;
        public object Parent { get; set; }
        public Tooltip Tooltip { get; set; }
        private List<Ship> ships;
        public delegate void ElementClicked(object sender);
        public event ElementClicked clickEvent;
        public Rectangle GetBoundary()
        {
            return Boundary;
        }

        public void OnClick()
        {
            clickEvent(this);
            focused = true;
        }

        public void SetShips(List<Ship> newShips)
        {
            ships = newShips;
        }
        public List<Ship> GetShips()
        {
            return ships;
        }

        public override void Update()
        {
            Update(Text, ref textPosition, Font);
            focused = Game1.self.FocusedElement == this;
            bool changeOfFocus = focused != lastStateOfFocus;
            if (focused)
            {
                if(changeOfFocus || NeedNewTexture)
                Texture = Util.CreateTexture(Device, Width, Height, pixel => Color.Black, new Color(140, 140, 140),
                    new Color(60, 60, 60));
            }
            else
            {
                if(changeOfFocus || NeedNewTexture)
                Texture = Util.CreateTexture(Device, Width, Height, pixel => Color.Black, new Color(180, 180, 180),
                    new Color(100, 100, 100));
            }

            lastStateOfFocus = focused;
        }

        public void AddShip(Ship s)
        {
            ships.Add(s);
        }

        public Deck(Point origin, int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable,string name) 
            : base(origin, width, height, device, gui)
        {
            Font = font;
            TextWrappable = wrapable;
            Text = name;
            ships = new List<Ship>();
        }

        public override void Draw(SpriteBatch sp)
        {
            //sp.Begin();
            if (Game1.self.FocusedElement == this)
            {
                sp.Draw(Texture, Boundary, Color.White);
            }
            else
            {
                sp.Draw(Texture, Boundary, Color.White);
            }
            if (!String.IsNullOrEmpty(Text))
                sp.DrawString(Font, Text, TextPosition, Color.Black);
            //sp.End();
        }
    }
}
