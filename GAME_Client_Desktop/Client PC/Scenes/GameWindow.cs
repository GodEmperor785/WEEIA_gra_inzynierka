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
        private int GetColumn(Line en, bool allied)
        {
            int row = 1;
            if (allied)
            {
                if (en == Line.SHORT)
                    row = 2;
                if (en == Line.MEDIUM)
                    row = 1;
                if (en == Line.LONG)
                    row = 0;
            }
            else
            {
                if (en == Line.SHORT)
                    row = 0;
                if (en == Line.MEDIUM)
                    row = 1;
                if (en == Line.LONG)
                    row = 2;
            }

            return row;
        }
      
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
        private bool readyToSend = false;
        private bool readyToSet = false;
        private Thread th;
        private List<Card> AllyCards;
        private List<Card> EnemyCards;
        private Move move;
        private Button turnButton;
        private GameState currentGameState;
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
            move = new Move();




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
            turnButton = new Button(new Point(1000,400), 200, 200, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text="End turn"
            };

            turnButton.clickEvent += EndTurnClick;
            Clickable.Add(turnButton);
            yourGrid.Origin = new Point(10,10);
            enemyGrid.Origin = new Point(10 + yourGrid.Width + (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.4), 10 );

            yourCards.Origin = new Point(10,yourGrid.Origin.Y + yourGrid.Height + (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.02));

            cardWidth = (int) (yourGrid.Width * cardWidthPercentage);
            cardHeight = (int) (yourGrid.Height * cardHeightPercentage);


            layout.AddChild(enemyGrid);
            layout.AddChild(yourGrid);
            layout.AddChild(yourCards);
            layout.AddChild(turnButton);
            th = new Thread(ContactLoop);
            layout.Update();

        }

        public void EndTurnClick()
        {
            readyToSend = true;
            turnButton.Active = false;
        }
        public void CardClick(object sender)
        {
            CurrentCard = (Card)sender;
            if (LastCard == null) //checking if it's the first card clicked in action 
            {
                if (CurrentCard.Parent == yourGrid) // checking if it's your card so you can use it later
                {
                    if (CurrentCard.CanMove)
                    {
                        CurrentCard.Status = Card.status.clicked;
                        CardsInRow = 1;
                        SetSlots(true);
                    }
                }
            }
            else
            {
                CardsInRow++;
                if (CardsInRow == 2)
                {
                    if (LastCard.Parent == CurrentCard.Parent) // checking if previous card in action was from the same parent
                    {
                        if (CurrentCard.CanMove)
                        {
                            LastCard.Status = Card.status.clear;
                            CurrentCard.Status = Card.status.clicked;
                            CardsInRow = 1;
                        }
                    }
                    else
                    {
                        CurrentCard.Status = Card.status.target;
                        LastCard.CanMove = false;
                        LastCard.Status = Card.status.hasMove;
                        ShipPosition origin = getShipPosition(LastCard.GetShip());
                        ShipPosition target = getShipPosition(CurrentCard.GetShip());
                        move.AttackList.Add(new Tuple<ShipPosition, ShipPosition>(origin,target));
                        HideSlots();
                        CurrentCard = null;
                    }
                }
            }

            LastCard = CurrentCard;
        }

        public void HideSlots()
        {
            foreach (var clickable in Clickable)
            {
                if (clickable is CardSlot)
                {
                    CardSlot c = (CardSlot) clickable;
                    c.CardClicked = false;
                }
            }
        }
        public void SetSlots(bool clicked)
        {
            Clickable.ForEach(p =>
            {
                if (p is CardSlot)
                {
                    CardSlot c = (CardSlot) p;
                    if(CurrentCard != null && LineDifference(c.line,CurrentCard.line) == 1)
                        if(c.Active)
                            c.CardClicked = clicked;
                }
            });
        }
        private ShipPosition getShipPosition(Ship ship)
        {
            ShipPosition result = null;
            if (currentGameState.YourGameBoard.Board[Line.SHORT].Contains(ship))
            {
                result = new ShipPosition(Line.SHORT,getIndexOfShip(ship, currentGameState.YourGameBoard.Board[Line.SHORT]));
            }
            else if(currentGameState.YourGameBoard.Board[Line.MEDIUM].Contains(ship))
            {
                result = new ShipPosition(Line.MEDIUM, getIndexOfShip(ship, currentGameState.YourGameBoard.Board[Line.MEDIUM]));
            }
            else if (currentGameState.YourGameBoard.Board[Line.LONG].Contains(ship))
            {
                result = new ShipPosition(Line.LONG, getIndexOfShip(ship, currentGameState.YourGameBoard.Board[Line.LONG]));
            }
            else if(currentGameState.EnemyGameBoard.Board[Line.SHORT].Contains(ship))
            {
                result = new ShipPosition(Line.SHORT, getIndexOfShip(ship, currentGameState.EnemyGameBoard.Board[Line.SHORT]));
            }
            else if (currentGameState.EnemyGameBoard.Board[Line.MEDIUM].Contains(ship))
            {
                result = new ShipPosition(Line.MEDIUM, getIndexOfShip(ship, currentGameState.EnemyGameBoard.Board[Line.MEDIUM]));
            }
            else if (currentGameState.EnemyGameBoard.Board[Line.LONG].Contains(ship))
            {
                result = new ShipPosition(Line.LONG, getIndexOfShip(ship, currentGameState.EnemyGameBoard.Board[Line.LONG]));
            }
            else
            {
                result = null;
            }

            return result;
        }

        public int getIndexOfShip(Ship ship, List<Ship> ships)
        {
            int result = 0;
            int i = 0;
            foreach (var s in ships)
            {
                if (s.Equals(ship))
                {
                    result = i;
                }

                i++;
            }

            return result;
        }
        public void CardSlotClick(object sender)
        {
            CardSlot c = (CardSlot) sender;
            if (CurrentCard != null && LineDifference(CurrentCard.line, c.line) == 1 && CurrentCard.CanMove )
            {
                CurrentCard.CanMove = false;
                ShipPosition origin = getShipPosition(CurrentCard.GetShip());
                move.MoveList.Add(new Tuple<ShipPosition, Line>(origin,c.line));
                c.Active = false;
                CurrentCard.Status = Card.status.hasMove;
                SetSlots(false);
                if(CurrentCard.Parent == yourGrid)
                LastCard = null;
            }
        }

        private int LineDifference(Line l1, Line l2)
        {
            return Math.Abs(l1 - l2);
        }
        public void Start()
        {
            gameloop = true;
            th.Start();
        }
        public override void UpdateClickables()
        {
            /*if (Game1.self.FocusedElement == null)
            {
                Clickable.ForEach(p =>
                {
                    if (p is Card)
                    {
                        Card c = (Card)p;
                        c.Status = Card.status.clear;
                    }
                });
            }*/
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
                packet = Game1.self.Connection.GetReceivedPacket(10);
                if (packet != null)
                {
                    if (packet.OperationType == OperationType.GAME_STATE)
                    {
                        GameState state = (GameState)packet.Packet;
                        currentGameState = state;
                        readyToSet = true;
                        turnButton.Active = true;
                    }
                }
                if (readyToSend)
                {
                    readyToSend = false;
                    packet = new GamePacket(OperationType.MAKE_MOVE,move);
                    Game1.self.Connection.Send(packet);
                    packet = Game1.self.Connection.GetReceivedPacket();
                    if (packet.OperationType == OperationType.FAILURE)
                    {
                        //TODO add text to game
                    }





                }
            }
        }

        public override void UpdateLast()
        {
            if (readyToSet)
            {

                setState(currentGameState);
                readyToSet = false;
            }
        }

        public void setState(GameState state)
        {
            
            move = new Move();
            var shortAlly = state.YourGameBoard.Board[Line.SHORT].ToList();
            var medAlly = state.YourGameBoard.Board[Line.MEDIUM].ToList();
            var longAlly = state.YourGameBoard.Board[Line.LONG].ToList();

            var shortEnemy = state.EnemyGameBoard.Board[Line.SHORT].ToList();
            var medEnemy = state.EnemyGameBoard.Board[Line.MEDIUM].ToList();
            var longEnemy = state.EnemyGameBoard.Board[Line.LONG].ToList();

            yourGrid.RemoveChildren();
            enemyGrid.RemoveChildren();

            List<IClickable> toRemove = new List<IClickable>();
            Clickable.ForEach(p =>
            {
                if (p is Card || p is CardSlot)
                    toRemove.Add(p);
            });
            foreach (var clickable in toRemove)
            {
                Clickable.Remove(clickable);
            }

            CardsToRow(shortAlly, Line.SHORT, true);
            CardsToRow(medAlly, Line.MEDIUM, true);
            CardsToRow(longAlly, Line.LONG, true);

            CardsToRow(shortEnemy, Line.SHORT, false);
            CardsToRow(medEnemy, Line.MEDIUM, false);
            CardsToRow(longEnemy, Line.LONG, false);
            SetClickables(true);
        
        }

        protected override void SetClickables(bool active)
        {
            Clickable.ForEach(p => { p.Active = active; });
        }

        public void sendState()
        {

        }

        public void prepareState()
        {

        }
        
        public void CardsToRow(List<Ship> ships, Line line, bool allied)
        {
            if (allied)
            {
                int i;
                for (i = 0; i < ships.Count; i++)
                {
                    Card c = ShipToCard(ships[i]);
                    Clickable.Add(c);
                    c.CanMove = true;
                    c.line = line;
                    yourGrid.AddChild(c,i, GetColumn(line, allied));
                }

                while(i < 5)
                {
                    
                    CardSlot c = new CardSlot(cardWidth,cardHeight,Game1.self.GraphicsDevice,Gui);
                    yourGrid.AddChild(c,i,GetColumn(line, allied));
                    c.line = line;
                    c.clickEvent += CardSlotClick;
                    Clickable.Add(c);
                    i++;
                }
                yourGrid.UpdateP();
            }
            else
            {
                for (int i = 0; i < ships.Count; i++)
                {

                    Card c = ShipToCard(ships[i]);
                    c.line = line;
                    Clickable.Add(c);
                    enemyGrid.AddChild(c, i, GetColumn(line, allied));
                }
                enemyGrid.UpdateP();
            }
        }
        public Card ShipToCard(Ship ship)
        {
            Card result = new Card(cardWidth,cardHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true,ship);
            result.clickEvent += CardClick;
            
            return result;
        }
        public override void UpdateButtonNull()
        {
            CardsInRow = 0;
            SetSlots(false);
            if (LastCard != null && LastCard.Parent == yourGrid)
                LastCard.Status = Card.status.clear;
            LastCard = null;
            if (CurrentCard != null && CurrentCard.Parent == yourGrid)
                CurrentCard.Status = Card.status.clear;
            CurrentCard = null;
        }
        public void Draw(GameTime gameTime)
        {
            layout.Draw(Game1.self.spriteBatch);
        }

        

    }
}
