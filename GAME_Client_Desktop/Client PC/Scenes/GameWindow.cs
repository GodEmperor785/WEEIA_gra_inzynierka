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
    /// <summary>
    /// In this menu grid's columns are supposed to act as row since grid's are supposed to be rotated 
    /// so when you're supposed to work on normal rows (cards in 1 row, like closest row etc) you have to work on columns
    /// because grid doesn't have the option to be rotated
    ///
    /// 
    /// </summary>


    class GameWindow : Menu
    {
        private RelativeLayout layout;
        private Grid enemyGrid;
        private Grid yourGrid;
        private Grid yourCards;
        private List<Ship> ships;
        private Card CurrentCard;
        private Card LastCard;
        private int CardsInRow = 0;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            layout = new RelativeLayout();
            double cardWidthPercentage = 0.25;
            double cardHeightPercentage = 0.18;
            double widthPercentage = 0.2;
            double heightPercentage = 0.75;
            int ColumnWidth = (int) (Game1.self.graphics.PreferredBackBufferWidth * widthPercentage * cardWidthPercentage);
            int RowHeight = (int) (Game1.self.graphics.PreferredBackBufferHeight * heightPercentage * cardHeightPercentage);
            enemyGrid = new Grid(3,5, ColumnWidth, RowHeight);
            yourGrid = new Grid(3,5, ColumnWidth, RowHeight);
            yourCards = new Grid();
            Random rndRandom = new Random();
            ships = new List<Ship>(20);
            for (int i = 0; i < 20; i++)
            {
                Ship ship = new Ship();
                ship.Armor = rndRandom.Next(1, 30);
                ship.Hp = rndRandom.Next(1, 30);
                ships.Add(ship);
            }




            enemyGrid.WitdhAndHeightColumnDependant = false;
            yourGrid.WitdhAndHeightColumnDependant = false;
            yourCards.WitdhAndHeightColumnDependant = false;

            enemyGrid.Width = (int) (Game1.self.graphics.PreferredBackBufferWidth * widthPercentage);
            yourGrid.Width = (int)(Game1.self.graphics.PreferredBackBufferWidth * widthPercentage);

            enemyGrid.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * heightPercentage);
            yourGrid.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * heightPercentage);
            yourCards.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.15);

            enemyGrid.DrawBorder = true;
            yourGrid.DrawBorder = true;
            yourCards.DrawBorder = true;

            enemyGrid.BorderSize = 3;
            yourGrid.BorderSize = 3;
            yourCards.BorderSize = 3;

            enemyGrid.rowOffset = (int) (enemyGrid.Height * 0.025);
            yourGrid.rowOffset = (int)(enemyGrid.Height * 0.025);

            enemyGrid.columnOffset =(int) (enemyGrid.Width * 0.125);
            yourGrid.columnOffset = (int) (yourGrid.Width * 0.125);


            yourCards.Width = (int)(enemyGrid.Width + yourGrid.Width + (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.4));



            yourGrid.Origin = new Point(10,10);
            enemyGrid.Origin = new Point(10 + yourGrid.Width + (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.4), 10 );

            yourCards.Origin = new Point(10,yourGrid.Origin.Y + yourGrid.Height + (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.02));

            int cardWidth = (int) (yourGrid.Width * cardWidthPercentage);
            int cardHeight = (int) (yourGrid.Height * cardHeightPercentage);

            Card c1 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[0]);
            Card c2 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[1]);
            Card c3 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[2]);
            Card c4 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[3]);
            Card c5 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[4]);
            Card c6 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[5]);
            Card c7 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[6]);
            Card c8 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[7]);
            Card c9 = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false, ships[8]);

            c1.clickEvent += CardClick;
            c2.clickEvent += CardClick;
            c3.clickEvent += CardClick;
            c4.clickEvent += CardClick;
            c5.clickEvent += CardClick;
            c6.clickEvent += CardClick;
            c7.clickEvent += CardClick;
            c8.clickEvent += CardClick;
            c9.clickEvent += CardClick;

            Clickable.Add(c1);
            Clickable.Add(c2);
            Clickable.Add(c3);
            Clickable.Add(c4);
            Clickable.Add(c5);
            Clickable.Add(c6);

            c1.Active = true;
            c2.Active = true;
            c3.Active = true;
            c4.Active = true;
            c5.Active = true;
            c6.Active = true;

            yourGrid.AddChild(c1, 0, 0);
            yourGrid.AddChild(c2, 0, 1);
            yourGrid.AddChild(c3, 0, 2);

            enemyGrid.AddChild(c4, 0, 0);
            enemyGrid.AddChild(c5, 0, 1);
            enemyGrid.AddChild(c6, 0, 2);


            enemyGrid.ResizeChildren();
            yourGrid.ResizeChildren();
            layout.AddChild(enemyGrid);
            layout.AddChild(yourGrid);
            layout.AddChild(yourCards);

            

        }

        public void CardClick(object sender)
        {
            CurrentCard = (Card)sender;
            if (LastCard == null) //checking if it's the first card clicked in action 
            {
                if (CurrentCard.Parent == yourGrid) // checking if it's your card so you can use it later
                {
                    CurrentCard.Status = Card.status.clicked;
                    CardsInRow = 1;
                }
            }
            else
            {
                CardsInRow++;
                if (CardsInRow == 2)
                {
                    if (LastCard.Parent == CurrentCard.Parent) // checking if previous card in action was from the same parent (
                    {
                        LastCard.Status = Card.status.clear;
                        CurrentCard.Status = Card.status.clicked;
                        CardsInRow = 1;
                    }
                    else
                    {
                        CurrentCard.Status = Card.status.target;
                    }
                }
            }

            LastCard = CurrentCard;
        }
        
        public override void UpdateClickables()
        {
            if (Game1.self.FocusedElement == null)
            {
                Clickable.ForEach(p =>
                {
                    if (p is Card)
                    {
                        Card c = (Card)p;
                        c.Status = Card.status.clear;
                    }
                });
            }
            Clickable.ForEach(p =>
            {
                if (p is Card)
                {
                    Card c = (Card) p;
                    c.Update();
                }
            });
        }
        public override void UpdateButtonNull()
        {
            CardsInRow = 0;
            LastCard = null;
        }
        public void Draw(GameTime gameTime)
        {
            layout.Draw(Game1.self.spriteBatch);
        }

    }
}
