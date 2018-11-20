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

            Button b = new Button(new Point(0, 0), (int) (gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.75) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Add"
            };
            Button b2 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.75) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Save"
            };
            Button b3 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.75) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Remove"
            };
            Button b4 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset), (int)((gridRightBottom.Height * 0.75) * 0.5 - gridCenter.rowOffset), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
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
            Clickable.Add(b2);
            Clickable.Add(b);
            gridRightBottom.AddChild(DeckInputBox,0,0,3);
            gridRightBottom.AddChild(b,1,1);
            gridRightBottom.AddChild(b2, 1, 2);
            gridRightBottom.AddChild(b3, 2, 1);
            gridRightBottom.AddChild(b4, 2, 2);
            gridRightBottom.ResizeChildren();
            DeckInputBox.Update();



            layout.AddChild(gridTopLeft);
            layout.AddChild(gridRight);
            layout.AddChild(gridRightBottom);
            layout.AddChild(gridCenter);
            layout.AddChild(rl);
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

            FillBotShips(ships);
            FillBotGrid();

            gridTopLeft.AllVisible = false;
            gridTopLeft.VisibleRows = 1;

            gridCenter.AllVisible = false;
            gridCenter.VisibleRows = 5;

            gridCenter.UpdateP();
            gridTopLeft.UpdateP();

        }

        public void Clear()
        {
            ClearTopGrid();
            ShipsInTop.Clear();
            Console.WriteLine(ShipsInBot.Count);
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

            Console.WriteLine(Clickable.Count());
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
            FillBotShips(ships.Except(d.GetShips()).ToList());
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
                Card dc = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ship);
                Clickable.Add(dc);
                dc.clickEvent += CardClick;
                ShipsInTop.Add(dc);

            }
        }
        private void FillBotShips(List<Ship> ships)
        {
            foreach (var ship in ships)
            {
                Card dc = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ship);
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
                gridRight.AddChild(newDeck);
                DeckInputBox.Text = "";
                Clickable.Add(newDeck);
                gridRight.ResizeChildren();
            }

        }
        private void OnSave()
        {
            if (DeckInputBox.Text.Length > 0)
            {
                ChosenDeck.Text = DeckInputBox.Text;
            }
            List<Ship> newShips = new List<Ship>();
            ShipsInTop.ForEach(p=> newShips.Add(p.GetShip()));
            ChosenDeck.SetShips(newShips);
        }

        private void OnRemove()
        {

        }
        private void OnExit()
        {
            Game1.self.state = Game1.State.MainMenu;
        }

        public override void UpdateGrid()
        {
            gridRightBottom.UpdateP();
            gridRight.UpdateP();
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