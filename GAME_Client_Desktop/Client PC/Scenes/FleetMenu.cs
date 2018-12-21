using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Button = Client_PC.UI.Button;

namespace Client_PC.Scenes
{
    class FleetMenu : Menu
    {
        private Grid grid;
        private Grid buttonsGrid;
        private Grid cardsGrid;
        private Card CurrentCard;
        private RelativeLayout layout;
        private List<CardSlot> slots;
        private Fleet fleet;
        private double CardGridWidthMulti = 0.75;
        private double CardGridHeightMulti = 0.15;
        private int CardHeight = 200;
        private int CardWidth = 133;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            layout = new RelativeLayout();
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    CardSlot c = new CardSlot(CardWidth, CardHeight, Game1.self.GraphicsDevice,Gui);
                    grid.AddChild(c,row,column);
                    c.clickEvent += CardSlotClick;
                    slots.Add(c);
                    Clickable.Add(c);
                }
            }
            grid.Origin = new Point(50,50);
            grid.UpdateP();
            cardsGrid.WitdhAndHeightColumnDependant = false;
            cardsGrid.Width = (int)((int)Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti);
            cardsGrid.Height = (int)((int)Game1.self.graphics.PreferredBackBufferHeight * CardGridHeightMulti);
            Button up = new Button(new Point(cardsGrid.Origin.X + (int)(cardsGrid.Width) - 60, cardsGrid.Origin.Y), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "up"
            };
            up.clickEvent += upClick;
            Button down = new Button(new Point(cardsGrid.Origin.X + (int)(cardsGrid.Width) - 60, cardsGrid.Origin.Y + 30), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "down"
            };
            down.clickEvent += downClick;
            Button exit = new Button(200, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "Back"
            };
            exit.clickEvent += onExit;
            Button save = new Button(200,100,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                text = "Save"
            };
            save.clickEvent += onSave;
            layout.AddChild(up);
            layout.AddChild(down);
            Clickable.Add(up);
            Clickable.Add(down);
            cardsGrid.AllVisible = false;
            cardsGrid.VisibleRows = 1;
            cardsGrid.ConstantRowsAndColumns = true;
            cardsGrid.UpdateP();
        }

        public void onExit()
        {
            Game1.self.state = Game1.State.DeckMenu;
        }

        public void onSave()
        {
            fleet.
        }
        public void setFleet(Fleet fleet)
        {
            this.fleet = fleet;
        }

        private void upClick()
        {
            cardsGrid.ChangeRow(-1);
        }

        private void downClick()
        {
            cardsGrid.ChangeRow(1);
        }
        public void Fill()
        {
            if (fleet != null)
            {
                fleet.Ships.ForEach(p =>
                {
                    Card c = new Card(CardWidth,CardHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true,p);
                    cardsGrid.AddChild(c);
                    Clickable.Add(c);
                    c.clickEvent += CardClick;
                });
            }
            SetClickables(true);
        }

        public void CardClick(object sender)
        {
            CurrentCard = (Card)sender;
            slots.ForEach(p => p.CardClicked = true);
        }

        public void CardSlotClick(object sender)
        {
            if (CurrentCard != null)
            {
                CardSlot c = (CardSlot) sender;
                c.SetCard(CurrentCard);
                slots.ForEach(p => p.CardClicked = false);
            }
        }
        public void Draw(GameTime gameTime)
        {
            grid.Draw(Game1.self.spriteBatch);
            cardsGrid.Draw(Game1.self.spriteBatch);
            buttonsGrid.Draw(Game1.self.spriteBatch);
        }
    }
}
