using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_Android;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class CardsMenu : Menu
    {
        private RelativeLayout layout;
        private Grid grid;
        private int cardWidth, cardHeight;
        private List<Card> ShipsInBot;
        private Card ChosenCard;
        private Label lbl1;
        private List<IClickable> ClickableToRemove = new List<IClickable>();
        public override void Initialize(ContentManager Content)
        {
            cardWidth = 100;
            cardHeight = (int)(cardWidth * (200f/133f));
            int columns = (Game1.self.graphics.PreferredBackBufferWidth - 200) / (cardWidth+5);
            grid = new Grid(columns, Game1.self.Modifiers.MaxShipsPerPlayer / columns + 1,cardWidth, cardHeight);
            grid.DrawBorder = true;
            grid.BorderSize = 3;
            grid.WitdhAndHeightColumnDependant = false;
            grid.AllVisible = false;
            grid.Width = columns * cardWidth + (columns + 1) * grid.columnOffset;
            grid.Height = 5 * cardHeight + 6 * grid.rowOffset;
            grid.VisibleRows = 5;
            grid.ConstantRowsAndColumns = true;
            grid.MaxChildren = true;
            grid.ChildMaxAmount = Game1.self.Modifiers.MaxShipsPerPlayer;
            grid.Origin = new Point(100,100);
            Gui = new GUI(Content);
            layout = new RelativeLayout();
            int buttonWidth = 100;
            int buttonHeight = 50;
            Button exitButton = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                text = "Exit"
            };
            exitButton.Origin =new Point(grid.Origin.X + grid.Width/2 - buttonWidth/2,grid.Origin.Y + grid.Height + 10);
            exitButton.clickEvent += onExit;
            Clickable.Add(exitButton);
            exitButton.Active = true;
            Button up = new Button(new Point(grid.Origin.X - 60, grid.Origin.Y + grid.Height/2 - 30), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "up"
            };
            Button down = new Button(new Point(up.Origin.X, up.Origin.Y + 30), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "down"
            };
            layout.AddChild(exitButton);
            layout.AddChild(up);
            layout.AddChild(down);
            up.clickEvent += UpClick;
            down.clickEvent += DownClick;
            up.Active = true;
            down.Active = true;
            Clickable.Add(up);
            Clickable.Add(down);

            #region popup

            popup = new Popup(new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * 0.5), (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.5)), 100, 400, Game1.self.GraphicsDevice, Gui);
            Grid popupGrid = new Grid();
            lbl1 = new Label(200, 200, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Are you sure you want to dissolve this card for "
            };
            Button b1 = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Cancel"
            };
            Button a1 = new Button(100,100,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "Dissolve"
            };
            a1.clickEvent += onDissolve;
            Clickable.Add(a1);
            lbl1.DrawBackground = false;
            a1.DrawBackground = false;
            b1.DrawBackground = false;
            popup.grid = popupGrid;
            popupGrid.AddChild(lbl1, 0, 0);
            popupGrid.AddChild(a1, 1, 0);
            popupGrid.AddChild(b1, 1, 1);
            b1.clickEvent += onPopupExit;
            Clickable.Add(b1);
            popup.SetToGrid();

            #endregion


            exitButton.Update();
            down.Update();
            up.Update();
            grid.UpdateP();
        }
        private void UpClick()
        {
            grid.ChangeRow(-1);
        }

        private void DownClick()
        {
            grid.ChangeRow(1);
        }
        private void FillBotShips()
        {
            var ships = Game1.self.OwnedShips;
            if (ships != null)
                foreach (var ship in ships)
                {
                    Card dc = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ship);
                    Clickable.Add(dc);
                    dc.clickEvent += CardClick;
                    ShipsInBot.Add(dc);
                    ClickableToRemove.Add(dc);
                }
        }

        public override void Clean()
        {
            ShipsInBot = new List<Card>();
            ClickableToRemove.ForEach(p => { Clickable.Remove(p); });
            ClickableToRemove.Clear();
            FillBotGrid();
        }

        private void FillBotGrid()
        {
            grid.RemoveChildren();
            ClickableToRemove.ForEach(p => { Clickable.Remove(p); });
            ClickableToRemove.Clear();
            FillBotShips();
            ShipsInBot.ForEach(p =>
            {
                grid.AddChild(p);
            });
            grid.UpdateP();
        }
        private void CardClick(Object sender)
        {
            ChosenCard = (Card) sender;
            popup.SetActive(true);
            lbl1.Text = "Are you sure you want to dissolve this card for "+ ChosenCard.GetShip().Cost + " credits?";
            Game1.self.popupToDraw = popup;
            SetClickables(false);
        }
        private void onPopupExit()
        {
            popup.SetActive(false);
            foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
            {
                clickable.Active = true;
            }

            Game1.self.popupToDraw = null;
        }

        private void onDissolve()
        {
            GamePacket packet = new GamePacket(OperationType.SELL_SHIP,ChosenCard.GetShip());
            Game1.self.Connection.Send(packet);
            packet = Game1.self.Connection.GetReceivedPacket();
           
            ChosenCard = null;
            Game1.self.UpdatePlayer();
            Clean();
            onPopupExit();
        }
        private void onExit()
        {
            Game1.self.state = Game1.State.MainMenu;
            Game1.self.UpdatePlayer();
        }

        public void Draw(GameTime gameTime)
        {
            grid.Draw(Game1.self.spriteBatch);
            layout.Draw(Game1.self.spriteBatch);
        }
    }
}
