using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

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
        private Card CurrentCard;
        private Card LastCard;
        private int CardsInRow = 0;
        private int cardWidth;
        private int cardHeight;
        private bool gameloop = false;
        private Thread th;
        private List<Card> AllyCards;
        private List<Card> EnemyCards;
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

            enemyGrid.columnOffset = (int) (enemyGrid.Width * 0.125);
            yourGrid.columnOffset = (int) (yourGrid.Width * 0.125);


            yourCards.Width = (int)(enemyGrid.Width + yourGrid.Width + (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.4));



            yourGrid.Origin = new Point(10,10);
            enemyGrid.Origin = new Point(10 + yourGrid.Width + (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.4), 10 );

            yourCards.Origin = new Point(10,yourGrid.Origin.Y + yourGrid.Height + (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.02));

            cardWidth = (int) (yourGrid.Width * cardWidthPercentage);
            cardHeight = (int) (yourGrid.Height * cardHeightPercentage);


            layout.AddChild(enemyGrid);
            layout.AddChild(yourGrid);
            layout.AddChild(yourCards);
            th = new Thread(ContactLoop);
            

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

        public void Start()
        {
            gameloop = true;
            th.Start();
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

        public void ContactLoop()
        {
            GamePacket packet;
            while (gameloop)
            {
                packet = Game1.self.Connection.GetReceivedPacket(100);
                if (packet != null)
                {
                    if (packet.OperationType == OperationType.GAME_STATE)
                    {
                        GameState state = (GameState)packet.Packet;
                    }
                }
            }
        }

        public void setState(GameState state)
        {
            var shortAlly = state.YourGameBoard.Board[Line.SHORT].ToArray();
            var medAlly = state.YourGameBoard.Board[Line.MEDIUM].ToArray();
            var longAlly = state.YourGameBoard.Board[Line.LONG].ToArray();
            for (int i = 0; i < 5; i++)
            {
                Card c = new Card(cardWidth,cardHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true,shortAlly[i]);

            }

        }

        public void initialState(GameState state)
        {
            var shortAlly = state.YourGameBoard.Board[Line.SHORT].ToArray();
            var medAlly = state.YourGameBoard.Board[Line.MEDIUM].ToArray();
            var longAlly = state.YourGameBoard.Board[Line.LONG].ToArray();

            var allied = shortAlly.Concat(medAlly).Concat(longAlly);

            var shortEnemy = state.YourGameBoard.Board[Line.SHORT].ToArray();
            var medEnemy = state.YourGameBoard.Board[Line.MEDIUM].ToArray();
            var longEnemy = state.YourGameBoard.Board[Line.LONG].ToArray();

            var enemy = shortEnemy.Concat(medEnemy).Concat(longEnemy);

            foreach (var ship in allied)
            {
                Card c = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ship);
                c.clickEvent += CardClick;
                Clickable.Add(c);
                AllyCards.Add(c);
            }
            foreach (var ship in enemy)
            {
                Card c = new Card(cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ship);
                c.clickEvent += CardClick;
                Clickable.Add(c);
                EnemyCards.Add(c);
            }

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
