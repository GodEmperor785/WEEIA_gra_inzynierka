using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class DeckMenu : Menu
    {
        private RelativeLayout layout;
        private Grid gridTopLeft;
        private Grid gridRight;
        private Grid gridRightBottom;
        private Grid gridCenter;
        private List<Deck> Decks;
        private Deck ChosenDeck;
        private double CardGridHeightMulti = 0.15;
        private double CardGridWidthMulti = 0.75;
        private double RightGridHeightMulti = 0.6;
        private int cardWidth = 100;
        private int cardHeight = 150;
        private InputBox DeckInputBox;
        private List<Card> ShipsInTop;
        private List<Card> ShipsInBot;
        private List<Ship> ships;
        private Popup popup;
        private Label lbl1;
        public override void Initialize(ContentManager Content)
        {
            ShipsInTop = new List<Card>();
            ShipsInBot = new List<Card>();
            layout = new RelativeLayout();
            Gui = new GUI(Content);
            Decks = new List<Deck>();
            int gridRightColumnWidth =
                (int) ((int) Game1.self.graphics.PreferredBackBufferWidth *(1- CardGridWidthMulti)) - 30;
            int gridRightRowHeight = 60;

            int ColumnWidth = (int) ((int) Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti * 0.2);
            int rowHeight = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * CardGridHeightMulti);
            gridTopLeft = new Grid(5, 3, ColumnWidth, rowHeight);
            gridRight = new Grid(1, 8,gridRightColumnWidth,gridRightRowHeight);
            gridRightBottom = new Grid();
            gridCenter = new Grid(5, 6, ColumnWidth, rowHeight);
            ;
            gridTopLeft.DrawBorder = true;
            gridRight.DrawBorder = true;
            gridRightBottom.DrawBorder = false;
            gridCenter.DrawBorder = true;

            gridTopLeft.BorderSize = 3;
            gridRight.BorderSize = 3;
            gridRightBottom.BorderSize = 3;
            gridCenter.BorderSize = 3;

            gridTopLeft.WitdhAndHeightColumnDependant = false;
            gridRight.WitdhAndHeightColumnDependant = false;
            gridRightBottom.WitdhAndHeightColumnDependant = false;
            gridCenter.WitdhAndHeightColumnDependant = false;

            gridTopLeft.Width =(int) ((int)Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti);
            gridRight.Width = (int) ((int) Game1.self.graphics.PreferredBackBufferWidth * (1 - CardGridWidthMulti) - 30);
            gridRightBottom.Width = gridRight.Width;
            gridCenter.Width = gridTopLeft.Width;

            gridTopLeft.Height = (int) ((int)Game1.self.graphics.PreferredBackBufferHeight * CardGridHeightMulti);
            gridRight.Height = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * RightGridHeightMulti);
            gridRightBottom.Height = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * (1 - RightGridHeightMulti) - 30);
            gridCenter.Height = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * (1 - CardGridHeightMulti) - 30);


            gridTopLeft.Origin = new Point(10, 10);
            gridRight.Origin = new Point((int) (Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti + 20),10);
            gridRightBottom.Origin = new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti + 20),
                                    (int)(Game1.self.graphics.PreferredBackBufferHeight * RightGridHeightMulti + 20));
            gridCenter.Origin = new Point(10, (int)(Game1.self.graphics.PreferredBackBufferHeight - gridCenter.Height - 10));

            Button b = new Button(new Point(0, 0), (int) (gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.25) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Add"
            };
            Button b2 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.25) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Save"
            };
            Button b3 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.25) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Remove"
            };
            Button b4 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.25) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
            };
            Button b5 = new Button(new Point(0, 0), (int)(gridRightBottom.Width), (int)((gridRightBottom.Height * 0.25) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Organize"
            };

            Button up = new Button(new Point(gridTopLeft.Origin.X + (int)(gridTopLeft.Width) - 60, gridTopLeft.Origin.Y), 60, 30, Game1.self.GraphicsDevice,
            Gui, Gui.mediumFont, true)
            {
                Text = "up"
            };
            Button down = new Button(new Point(gridTopLeft.Origin.X + (int)(gridTopLeft.Width) - 60, gridTopLeft.Origin.Y + 30), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "down"
            };



            Button upCenter = new Button(new Point(gridCenter.Origin.X + (int)(gridCenter.Width) - 60, gridCenter.Origin.Y), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "up"
            };
            Button downCenter = new Button(new Point(gridCenter.Origin.X + (int)(gridCenter.Width) - 60, gridCenter.Origin.Y + 30), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "down"
            };

            DeckInputBox = new InputBox(new Point(), gridRightBottom.Width,(int) (gridRightBottom.Height * 0.25), Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false );
            DeckInputBox.TextLimit = 30;
            up.Update();
            down.Update();
            upCenter.Update();
            downCenter.Update();

            up.clickEvent += UpClick;
            down.clickEvent += DownClick;
            upCenter.clickEvent += UpClickCenter;
            downCenter.clickEvent += DownClickCenter;

            Clickable.Add(up);
            Clickable.Add(down);
            Clickable.Add(upCenter);
            Clickable.Add(downCenter);

            RelativeLayout rl = new RelativeLayout();
            rl.AddChild(up);
            rl.AddChild(down);
            rl.AddChild(upCenter);
            rl.AddChild(downCenter);

            Clickable.Add(b4);
            Clickable.Add(DeckInputBox);
            b4.clickEvent += OnExit;
            b.clickEvent += OnAdd;
            b2.clickEvent += OnSave;
            b3.clickEvent += OnRemove;
            b5.clickEvent += onOrganize;
            Clickable.Add(b5);
            Clickable.Add(b2);
            Clickable.Add(b);
            Clickable.Add(b3);
            gridRightBottom.AddChild(DeckInputBox,0,0,3);
            gridRightBottom.AddChild(b5,1,0,3);
            gridRightBottom.AddChild(b,2,1);
            gridRightBottom.AddChild(b2, 2, 2);
            gridRightBottom.AddChild(b3, 3, 1);
            gridRightBottom.AddChild(b4, 3, 2);
            gridRightBottom.ResizeChildren();
            DeckInputBox.Update();



            layout.AddChild(gridTopLeft);
            layout.AddChild(gridRight);
            layout.AddChild(gridRightBottom);
            layout.AddChild(gridCenter);
            layout.AddChild(rl);

            gridTopLeft.AllVisible = false;
            gridTopLeft.VisibleRows = 1;

            gridCenter.AllVisible = false;
            gridCenter.VisibleRows = 5;


            gridTopLeft.ConstantRowsAndColumns = true;
            gridTopLeft.MaxChildren = true;
            gridTopLeft.ChildMaxAmount = 15;

            gridCenter.ConstantRowsAndColumns = true;
            gridCenter.MaxChildren = true;
            gridCenter.ChildMaxAmount = 150;

            gridRight.ConstantRowsAndColumns = true;
            gridRight.MaxChildren = true;
            gridRight.ChildMaxAmount = 8;
            
            cardHeight = gridTopLeft.Height;


            /*     //basic test

            ships = new List<Ship>();
            Random rndRandom = new Random();
            for (int i = 0; i < 30; i++)
            {
                Ship ship = new Ship();
                ship.Armor = rndRandom.Next(1, 30);
                ship.Hp = rndRandom.Next(1, 30);
                ships.Add(ship);
            }

            List<Ship> Deck1Ships = ships.GetRange(1, 10);
            Deck BasicDeck = new Deck(new Point(), 20, (int)(gridRight.Height * 0.1),
                Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, "Basic" );
            Deck1Ships.ForEach(p=> BasicDeck.AddShip(p));
            BasicDeck.clickEvent += DeckClick;
            gridRight.AddChild(BasicDeck);
            Clickable.Add(BasicDeck);
            gridRight.ResizeChildren();


            */

            popup = new Popup(new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * 0.5), (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.5)), 100, 400, Game1.self.GraphicsDevice, Gui);
            Grid popupGrid = new Grid();
            lbl1 = new Label(200, 200, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b1 = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
            };
            lbl1.DrawBackground = false;
            b1.DrawBackground = false;
            popup.grid = popupGrid;
            popupGrid.AddChild(lbl1, 0, 0);
            popupGrid.AddChild(b1, 1, 0);
            b1.clickEvent += onPopupExit;
            Clickable.Add(b1);
            popup.SetToGrid();







            FillBotShips(ships);
            FillBotGrid();

            gridTopLeft.AllVisible = false;
            gridTopLeft.VisibleRows = 1;

            gridCenter.AllVisible = false;
            gridCenter.VisibleRows = 5;

            gridCenter.UpdateP();
            gridTopLeft.UpdateP();

        }

        public void onOrganize()
        {
            if (ChosenDeck != null)
            {
                Game1.self.SetFleetMenu(ChosenDeck.GetFleet());
                Game1.self.state = Game1.State.FleetMenu;
            }
        }
        public void onPopupExit()
        {
            popup.SetActive(false);
            foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
            {
                clickable.Active = true;
            }

            Game1.self.popupToDraw = null;

        }
        public void LoadDecksAndShips(List<Fleet> fleets, List<Ship> ships)
        {
            Decks.ForEach(p=> Clickable.Remove(p));
            Decks.Clear();
            fleets.ForEach(p=>
            {
                Deck z = FleetToDeck(p);
                z.clickEvent += DeckClick;
                Clickable.Add(z);
                Decks.Add(z);
                RefreshRightGrid();

            });
            this.ships = ships;
            Decks.ForEach(p => p.RecentlyAdded = false);
        }

        public override void Clean()
        {
            ClearTopGrid();
            ShipsInTop.Clear();
            //Console.WriteLine(ShipsInBot.Count);
            ChosenDeck = null;
            ClearBotGrid();
            ShipsInBot.Clear();
            FillBotShips(ships);
            FillBotGrid();
            DeckInputBox.Text = "";
        }
        private void CardClick(object sender)
        {
            Card c = (Card) sender;
            Grid g = (Grid)c.Parent;
            if (g == gridTopLeft)
            {
                c.ChangeParent((Grid)c.Parent,gridCenter);
                ShipsInBot.Add(c);
                ShipsInTop.Remove(c);
                gridTopLeft.MoveChildren();
            }
            else if (g == gridCenter)
            {
                c.ChangeParent((Grid)c.Parent,gridTopLeft);
                ShipsInTop.Add(c);
                ShipsInBot.Remove(c);
                gridCenter.MoveChildren();
            }
            
            
            //Console.WriteLine("-----------------GridTopLeft-----------------");
           // gridTopLeft.PrintChildren();

           // Console.WriteLine(Clickable.Count());
        }

        private void RefreshClickables()
        {

        }
        private void UpClick()
        {
           // Console.WriteLine(-1);
            gridTopLeft.ChangeRow(-1);
        }

        private void DownClick()
        {
            //Console.WriteLine(1);
            gridTopLeft.ChangeRow(1);
        }

        
        private void UpClickCenter()
        {
            gridCenter.ChangeRow(-1);
        }

        private void DownClickCenter()
        {
            gridCenter.ChangeRow(1);
        }

        private void DeckClick(object sender)
        {
            Deck d = (Deck) sender;
            ChosenDeck = d;
            ClearTopGrid();
            ShipsInTop.Clear();
            FillTopShips(d.GetShips());
            FillTopGrid();
            ClearBotGrid();
            ShipsInBot.Clear();
            List<Ship> shipsToFill = new List<Ship>();
            ships.ForEach(p =>
            {
                bool add = true;
                ChosenDeck.GetShips().ForEach(a =>
                {
                    if (p.Id == a.Id)
                        add = false;
                });
                if (add)
                {
                    shipsToFill.Add(p);
                }
            });
            FillBotShips(shipsToFill);
            FillBotGrid();
        }
        

        private void ClearTopGrid()
        {
            foreach (var card in ShipsInTop)
            {
                Clickable.Remove(card);
                gridTopLeft.RemoveChild(card);
            }
        }

        private void ClearBotGrid()
        {
            foreach (var card in ShipsInBot)
            {
                Clickable.Remove(card);
                gridCenter.RemoveChild(card);
            }
        }
        private void FillTopGrid()
        {
           
            foreach (var card in ShipsInTop)
            {
                gridTopLeft.AddChild(card);
            }

        }
        private void FillBotGrid()
        {
            
            foreach (var card in ShipsInBot)
            {
                gridCenter.AddChild(card, true);
            }
        }
        private void FillTopShips(List<Ship> ships)
        {
            foreach (var ship in ships)
            {
                Card dc = new Card( cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ship);
                Clickable.Add(dc);
                dc.clickEvent += CardClick;
                ShipsInTop.Add(dc);

            }
        }
        private void FillBotShips(List<Ship> ships)
        {
            if(ships != null)
            foreach (var ship in ships)
            {
                Card dc = new Card( cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ship);
                Clickable.Add(dc);
                dc.clickEvent += CardClick;
                ShipsInBot.Add(dc);

            }
        }
        private void OnAdd()
        {
            if (DeckInputBox.Text.Length > 0 && gridRight.CanHaveMoreChildren())
            {
                Deck newDeck = new Deck(new Point(), 20, (int)(gridRight.Height * 0.1),
                    Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, DeckInputBox.Text);
                Decks.Add(newDeck);
                newDeck.RecentlyAdded = true;
                newDeck.clickEvent += DeckClick;
                gridRight.AddChild(newDeck);
                DeckInputBox.Text = "";
                Clickable.Add(newDeck);
                gridRight.ResizeChildren();
            }

        }

        private void RefreshRightGrid()
        {
            gridRight.RemoveChildren();
            Decks.ForEach(p=> gridRight.AddChild(p));
            gridRight.ResizeChildren();
        }

        private void RefreshTopGrid()
        {

        }

        private void RefreshBotGrid()
        {

        }
        private void OnSave()
        {
            if (ChosenDeck != null)
            {

                if (DeckInputBox.Text.Length > 0)
                {
                    ChosenDeck.Text = DeckInputBox.Text;
                }

                List<Ship> newShips = new List<Ship>();
                ShipsInTop.ForEach(p => newShips.Add(p.GetShip()));
                Fleet fleet = new Fleet(ChosenDeck.Text,Game1.self.player,newShips);
                ChosenDeck.SetFleet(fleet);
                GamePacket packet;
                if (ChosenDeck.RecentlyAdded)
                {
                    packet = new GamePacket(OperationType.ADD_FLEET,fleet);
                }
                else
                {
                    packet = new GamePacket(OperationType.UPDATE_FLEET, fleet);
                }
                Game1.self.Connection.Send(packet);
                GamePacket packetReceived = Game1.self.Connection.GetReceivedPacket();
                if (packetReceived.OperationType != OperationType.SUCCESS)
                {
                    lbl1.Text = "Problem with adding fleet";
                    popup.SetActive(true);
                    Clean();
                    Game1.self.popupToDraw = popup;
                    SetClickables(false);
                }
                packet = new GamePacket(OperationType.VIEW_FLEETS, Game1.self.player);
                Game1.self.Connection.Send(packet);
                packetReceived = Game1.self.Connection.GetReceivedPacket();
                if (packetReceived.OperationType == OperationType.VIEW_FLEETS)
                {
                    Game1.self.Decks = (List<Fleet>)packetReceived.Packet;
                }
                Game1.self.SetDecks(Game1.self.Decks);
                
                ChosenDeck = null;
                
            }
        }

        private void OnRemove()
        {
            GamePacket packet = new GamePacket(OperationType.DELETE_FLEET, ChosenDeck.GetFleet());
            Game1.self.Connection.Send(packet);
            GamePacket packetReceived = Game1.self.Connection.GetReceivedPacket();
            if (packetReceived.OperationType == OperationType.SUCCESS)
            {
                Decks.Remove(ChosenDeck);
                Clickable.Remove(ChosenDeck);
                RefreshRightGrid();
                ClearTopGrid();
                ShipsInTop.Clear();
                ClearBotGrid();
                ShipsInBot.Clear();
                FillBotShips(ships);
                FillBotGrid();

            }
        }

        private Deck FleetToDeck(Fleet fleet)
        {
            Deck deck = new Deck(new Point(), 100,100,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true,fleet.Name );
            deck.SetFleet(fleet);


            return deck;
        }
        private void OnExit()
        {
            Game1.self.state = Game1.State.MainMenu;
        }

        public override void UpdateGrid()
        {
            gridRightBottom.UpdateP();
            gridRight.UpdateP();
            SetClickables(true);
        }
        public void Draw(GameTime gameTime)
        {
            if (false)
            {
                Clickable.ForEach(p =>
                {
                    var z = (GuiElement) p;
                    z.Draw(Game1.self.spriteBatch);
                });
            }
            else
            {
                layout.Draw(Game1.self.spriteBatch);
            }
        }


    }
}