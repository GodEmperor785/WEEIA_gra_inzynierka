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
using Label = Client_PC.UI.Label;

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
        private PlayerGameBoard gmBoard;
        private bool readyToSend;
        private List<IClickable> ClickableToRemove;
        private Label tipLabel;
        private Label lbl;
        private bool isOver;
        private bool isInitialized = false;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            layout = new RelativeLayout();
            grid = new Grid();
            buttonsGrid = new Grid();
            cardsGrid = new Grid(5, 3, CardWidth, CardHeight);
            slots = new List<CardSlot>();
            ClickableToRemove = new List<IClickable>();
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
            buttonsGrid.Origin = new Point(Game1.self.graphics.PreferredBackBufferWidth / 2 - 100, Game1.self.graphics.PreferredBackBufferHeight - 200);
            cardsGrid.Origin = new Point(50, buttonsGrid.Origin.Y - 200);
            Button up = new Button(new Point(cardsGrid.Origin.X + (int)(cardsGrid.Width/2) , cardsGrid.Origin.Y), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "up"
            };
            up.clickEvent += upClick;
            Button down = new Button(new Point(cardsGrid.Origin.X + (int)(cardsGrid.Width/2) , cardsGrid.Origin.Y + 30), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "down"
            };
            down.clickEvent += downClick;

            Button save = new Button(200,100,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                text = "Ready"
            };
            save.clickEvent += onSave;
            tipLabel = new Label(new Point(grid.Origin.X + grid.Width + 50,grid.Origin.Y), Game1.self.graphics.PreferredBackBufferWidth - 150 - grid.Width, grid.RealHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text="You have 2 minutes to choose your fleet's shape. There's no difference between positions in rows, all the ships in the end will be put to the left border."
            };
            tipLabel.HeightDerivatingFromText = true;
           // tipLabel.Update();
            layout.AddChild(tipLabel);


            Grid popupGrid = new Grid();
            Button popupExitButton = new Button(200,100,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                text = "Exit to menu"
            };
            popupExitButton.clickEvent += onExit;
            Clickable.Add(popupExitButton);
            lbl = new Label(200,200,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "You lost the game due to not choosing shape of fleet for longer than 2 minutes"
            };
            popupGrid.AddChild(lbl,0,0);
            popupGrid.AddChild(popupExitButton,1,0);
            Point popupOrigin = new Point(Game1.self.graphics.PreferredBackBufferWidth / 2 - 100, Game1.self.graphics.PreferredBackBufferHeight / 2 - 150);
            popup = new Popup(popupOrigin,200,300,Game1.self.GraphicsDevice,Gui);
            popup.grid = popupGrid;
            popup.SetToGrid();




            buttonsGrid.AddChild(save,0,0);
            Clickable.Add(save);
            layout.AddChild(up);
            layout.AddChild(down);
            Clickable.Add(up);
            Clickable.Add(down);
            cardsGrid.AllVisible = false;
            cardsGrid.VisibleRows = 1;
            cardsGrid.ConstantRowsAndColumns = true;
            cardsGrid.MaxChildren = true;
            cardsGrid.ChildMaxAmount = 15;
            cardsGrid.UpdateP();
            buttonsGrid.UpdateP();
            SetClickables(true);
            layout.Update();
            isInitialized = true;
        }

        public void ReDo()
        {
            if (isInitialized)
            {
                ClickableToRemove = new List<IClickable>();
                Clickable.ForEach(p =>
                {
                    if(p is Card || p is CardSlot)
                        ClickableToRemove.Add(p);
                });
                ClickableToRemove.ForEach(p=> Clickable.Remove(p));
                grid.RemoveChildren();
                slots = new List<CardSlot>();
                for (int row = 0; row < 3; row++)
                {
                    for (int column = 0; column < 5; column++)
                    {
                        CardSlot c = new CardSlot(CardWidth, CardHeight, Game1.self.GraphicsDevice, Gui);
                        grid.AddChild(c, row, column);
                        c.clickEvent += CardSlotClick;
                        slots.Add(c);
                        Clickable.Add(c);
                    }
                }

                grid.UpdateP();
            }
        }
        public void onSave()
        {
            //TODO not tested
            if (cardsGrid.ChildrenCount == 0)
            {
                grid.UpdateActive(false);
                List<Ship> closestShips = new List<Ship>();
                List<Ship> midShips = new List<Ship>();
                List<Ship> furthestShips = new List<Ship>();
                for (int i = 0; i < 5; i++)
                {
                    CardSlot slot = (CardSlot) grid.GetChild(0, i);
                    if (slot.HasCard)
                        closestShips.Add(slot.Card.GetShip());
                }

                for (int i = 0; i < 5; i++)
                {
                    CardSlot slot = (CardSlot) grid.GetChild(1, i);
                    if (slot.HasCard)
                        midShips.Add(slot.Card.GetShip());
                }

                for (int i = 0; i < 5; i++)
                {
                    CardSlot slot = (CardSlot) grid.GetChild(2, i);
                    if (slot.HasCard)
                        furthestShips.Add(slot.Card.GetShip());
                }

                gmBoard = new PlayerGameBoard(closestShips, midShips, furthestShips);

                readyToSend = true;
            }
            else
            {
                lbl.Text = "You have to place all your cards.";
                isOver = false;
                popup.SetActive(true);
                Game1.self.popupToDraw = popup;
                SetClickables(false);
            }
        }
        public void setFleet(Fleet fleet)
        {
            this.fleet = fleet;
        }

        public void onExit()
        {
            if (isOver)
            {
                Game1.self.state = Game1.State.MainMenu;
                Game1.self.UpdateHistory();
                
            }

            popup.SetActive(false);
            foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
            {
                clickable.Active = true;
            }

            Game1.self.popupToDraw = null;

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
                    c.Status = Card.status.clear;
                    c.clickEvent += CardClick;
                });
            }
            SetClickables(true);
            Task task = new Task(ReadyFunction);
            task.Start();
        }

        public void CardClick(object sender)
        {
            if (CurrentCard != null)
            {
                CurrentCard.Status = Card.status.clear;
                CurrentCard.Update();
            }
            Card c = (Card)sender;
            CurrentCard = c;
            CurrentCard.Status = Card.status.clicked;
            CurrentCard.Update();
            slots.Where(p=> p.Card == null).ToList().ForEach(p => p.CardClicked = true);
        }

        public override void UpdateLast()
        {
            
            
        }

        public void ReadyFunction()
        {
            GamePacket packet;
            while (true)
            {
                packet = Game1.self.Connection.GetReceivedPacket(100);
                if (packet != null)
                {
                    if (packet.OperationType == OperationType.SUCCESS) // Fleet accepted and can start playing
                    {
                        Game1.self.state = Game1.State.GameWindow;
                        grid.RemoveChildren();
                        Game1.self.StartGame();
                        break;
                    }
                    else if (packet.OperationType == OperationType.FAILURE) // Failed to set fleet therefore loses game 
                    {
                        isOver = true;
                        popup.SetActive(true);
                        Game1.self.popupToDraw = popup;
                        SetClickables(false);
                        break;
                    }
                }

                if (readyToSend)
                {
                    readyToSend = false;
                    packet = new GamePacket(OperationType.SETUP_FLEET, gmBoard);
                    Game1.self.Connection.Send(packet);
                }
            }
        }

        public override void UpdateButtonNull()
        {
            slots.ForEach(p=> p.CardClicked = false);
            if (CurrentCard != null)
            {
                CurrentCard.Status = Card.status.clear;
                CurrentCard.Update();
            }

            CurrentCard = null;
        }

        public void CardSlotClick(object sender)
        {
            CardSlot c = (CardSlot)sender;
            if (CurrentCard != null)
            {
                if (c.Card == null)
                {
                    cardsGrid.RemoveChild(CurrentCard);
                    CurrentCard.Status = Card.status.clear;
                    var list = slots.Where(p => p.Card == CurrentCard).ToList();
                    if (list.Count > 0)
                    {
                        slots.Where(p => p.Card == CurrentCard).ToList().ForEach(p => { p.RemoveCard(); });
                    }

                    c.SetCard(CurrentCard);
                    slots.ForEach(p => p.CardClicked = false);
                    CurrentCard = null;
                }
            }
            else
            {
                if (c.HasCard)
                {
                    c.Card.OnClick();
                }
            }
        }
        public void Draw(GameTime gameTime)
        {
            grid.Draw(Game1.self.spriteBatch);
            cardsGrid.Draw(Game1.self.spriteBatch);
            buttonsGrid.Draw(Game1.self.spriteBatch);
            layout.Draw(Game1.self.spriteBatch);
        }
    }
}
