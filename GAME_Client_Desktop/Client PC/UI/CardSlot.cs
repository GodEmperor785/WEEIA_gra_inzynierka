using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class CardSlot : GuiElement, IClickable
    {
        public bool Active { get; set; }
        public bool ActiveChangeable { get; set; }
        public Tooltip Tooltip { get; set; }
        public Card Card { get; set; }
        private bool hasCard;
        public bool CardClicked;
        public delegate void ElementClicked(object sender);
        public event ElementClicked clickEvent;
        public Point Position { get; set; }



        public Rectangle GetBoundary()
        {
            return Boundary;
        }

        public CardSlot(int width, int height, GraphicsDevice device, GUI gui) : base(width, height, device, gui)
        {
            hasCard = false;
            CardClicked = false;
            Card = null;
        }

        public override void Update()
        {
            if (NeedNewTexture)
                Texture = Util.CreateTextureHollow(Device, Width, Height, new Color(10, 10, 10), new Color(10, 10, 10), 5, 1);
        }
        public void OnClick()
        {
            if (Active)
                clickEvent(this);
        }

        public void SetCard(Card card)
        {
            Card = card;
            Card.Position = Position;
            Card.Origin = Origin;
            Card.Update();
            hasCard = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(hasCard)
                Card.Draw(spriteBatch);
            else
            {
                if(CardClicked)
                    spriteBatch.Draw(Texture, Boundary, Color.White);
            }
        }
    }
}
