using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client_PC;
using Client_PC.UI;
using Client_PC.Utilities;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class MainMenu : Menu
    {
        private Grid grid;
        private Grid lastGamesGrid;
        private Deck chosenDeck;
        private Grid g;
        private Popup popup;
        private Label labelWaiting;
        private Timer timer;
        private int time;
        private string initialText = "Select deck and press search button to play";
        private string foundgame = "Found game";
        private bool searching = false;
        private bool stopSearching = false;
        private bool startedSearching = false;
        private int historyLabelHeight = 80;
        #region popup buttons
        List<IClickable> ClickableToRemove = new List<IClickable>();
        private Button up;
        private Button down;
        private Button exit;
        private Button search;
        #endregion

        #region popupCustomPlay
        private InputBox nameInputBox, creatornameInputBox;
        private Label labelError;
        private Button Join;
        private Button Create;
        #endregion



        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Button z = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Deck"
            };
            Button z2 = new Button(new Point(100, 200), 70, 125, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Settings"
            };
            Button z3 = new Button(new Point(100, 200), 200, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Exit"
            };
            Button p1 = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont, true)
            {
                Text = "Play ranked"
            };
            Button p2 = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont, true)
            {
                Text = "Play custom"
            };
            Button z4 = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont, true)
            {
                Text = "Shop"
            };
            Button z5 = new Button(120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont, true)
            {
                Text = "Cards"
            };
            z5.clickEvent += goToCards;
            Clickable.Add(z5);
            Clickable.Add(p1);
            Clickable.Add(p2);
            Clickable.Add(z);
            Clickable.Add(z2);
            Clickable.Add(z3);
            Clickable.Add(z4);
            grid = new Grid();
            lastGamesGrid = new Grid();
            lastGamesGrid.Width = 200;
            lastGamesGrid.Origin = new Point(50, 100);
            lastGamesGrid.DrawBackground = true;
            grid.AddChild(p1,0,0);
            grid.AddChild(p2,1,0);
            grid.AddChild(z, 2, 0);
            grid.AddChild(z4, 3, 0);
            grid.AddChild(z5, 4, 0);
            grid.AddChild(z2, 5, 0);
            grid.AddChild(z3,6,0);
            p1.clickEvent += Play;
            p2.clickEvent += PlayCustom;
            z3.clickEvent += ExitClick;
            z4.clickEvent += GoToShop;
            z2.clickEvent += GoToSettings;
            z.clickEvent += GoToDeck;
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f),(int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            grid.ResizeChildren();
            SetClickables(true);
        }

        public void goToCards()
        {
            Game1.self.CleanCards();
            Game1.self.state = Game1.State.CardsMenu;

        }
        public void Play()
        {
            ClickableToRemove = new List<IClickable>();
            int leftOffset = 10;
            int topOffset = 10;
            int buttonWidth = 200;
            int buttonHeight = 50;
            int topOrigin = (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.3);
            int leftOrigin = (int) (Game1.self.graphics.PreferredBackBufferWidth * 0.5 - (buttonWidth + 10));
            Point popupOrigin = new Point(leftOrigin,topOrigin);
            RelativeLayout layout = new RelativeLayout();
            up = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "up"
            };
            up.Origin = new Point(leftOrigin + leftOffset, topOrigin + topOffset);
            up.clickEvent += upClick;

            Clickable.Add(up);
            ClickableToRemove.Add(up);
            down = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "down"
            };
            exit = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "exit"
            };
            search = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "search game"
            };
            search.ActiveChangeable = false;
            search.clickEvent += searchClick;
            Clickable.Add(search);
            ClickableToRemove.Add(search);
            exit.clickEvent += exitClick;
            Clickable.Add(exit);
            ClickableToRemove.Add(exit);
            down.clickEvent += downClick;
            Clickable.Add(down);
            ClickableToRemove.Add(down);
            g = new Grid(1, 8, buttonWidth, buttonHeight);
            g.Origin = new Point(leftOrigin + leftOffset, up.Origin.Y + up.Height + 10);
            g.AllVisible = false;
            g.MaxChildren = true;
            g.ChildMaxAmount = 8;
            g.VisibleRows = 5;
            Game1.self.Decks.Where(a=> a.Ships.Count > 0).ToList().ForEach(p =>
            {
                Deck d = new Deck(new Point(), buttonWidth, buttonHeight, Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true,p.Name );
                d.SetFleet(p);
                d.clickEvent += DeckClick;
                d.ActiveChangeable = true;
                Clickable.Add(d);
                ClickableToRemove.Add(d);
                g.AddChild(d);
            });

            g.WitdhAndHeightColumnDependant = false;
            g.ConstantRowsAndColumns = true;
            g.UpdateP();
            Point downPoint = new Point(leftOrigin + leftOffset, g.Origin.Y + (int)g.RowOffset(5) + 10);
            down.Origin = downPoint;
            exit.Origin = new Point(down.Origin.X+10+buttonWidth, down.Origin.Y);
            search.Origin = new Point(exit.Origin.X,exit.Origin.Y - buttonHeight - 10);
            labelWaiting = new Label(buttonWidth, buttonHeight * 2 , Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            labelWaiting.Text = initialText;
            time = 0;
            labelWaiting.Origin = new Point(up.Origin.X + buttonWidth + 10, up.Origin.Y);
            layout.AddChild(up);
            layout.AddChild(down);
            layout.AddChild(g);
            layout.AddChild(exit);
            layout.AddChild(search);
            layout.AddChild(labelWaiting);
            popup = new Popup(popupOrigin,2 * buttonWidth + 30, topOffset + up.Height + (int)g.RowOffset(5) + 10 + down.Height + 10 + 10,Game1.self.GraphicsDevice,Gui);
            popup.layout = layout;
            up.Update();
            down.Update();
            exit.Update();
            search.Update();
            g.UpdateP();
            labelWaiting.Update();
            popup.SetBackground();
            Game1.self.popupToDraw = popup;
            SetClickables(false);
            popup.SetActive(true);
            popup.layout.UpdateActive(true);
            search.Active = false;
        }

        public void PlayCustom()
        {
            int topOrigin = (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.2);
            int buttonWidth = 200;
            int leftOffset = 10;
            int leftOrigin = (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.5 - (2*buttonWidth + leftOffset * 1.5));
            int topOffset = 10;
            int buttonHeight = 50;
            ClickableToRemove = new List<IClickable>();
            RelativeLayout layout = new RelativeLayout();
            nameInputBox = new InputBox(new Point(leftOrigin + leftOffset, topOrigin + topOffset),2* buttonWidth, 2* buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                TextLimit = 30,
                BasicText = "Room name"
            };
            creatornameInputBox = new InputBox(new Point(leftOrigin + leftOffset, nameInputBox.Origin.Y+nameInputBox.Height + topOffset), 2 * buttonWidth, 2 * buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                TextLimit = 30,
                BasicText = "Creator name"
            };
            Clickable.Add(nameInputBox);
            ClickableToRemove.Add(nameInputBox);
            Clickable.Add(creatornameInputBox);
            ClickableToRemove.Add(creatornameInputBox);
            Join = new Button(new Point(nameInputBox.Origin.X, creatornameInputBox.Origin.Y+ creatornameInputBox.Height + topOffset),buttonWidth ,buttonHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                Text = "Join"
            };
            Join.clickEvent += onJoin;
            Clickable.Add(Join);
            ClickableToRemove.Add(Join);
            Create = new Button(new Point(Join.Origin.X+buttonWidth,Join.Origin.Y),buttonWidth,buttonHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                Text = "Create"
            };
            Create.clickEvent += onCreate;
            Clickable.Add(Create);
            ClickableToRemove.Add(Create);
            labelError = new Label(new Point(nameInputBox.Origin.X + nameInputBox.Width + leftOffset, nameInputBox.Origin.Y),buttonWidth * 2,buttonHeight * 3, Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                Text = "To create room write the name and click Create button, to join room write it's name and click Join button." +
                       " To join you have also to write creator's name."
            };
            Button Exit = new Button(new Point(labelError.Origin.X, labelError.Origin.Y+labelError.Height + topOffset),buttonWidth,buttonHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                Text = "Exit"
            };
            Exit.clickEvent += onExitCustom;
            Clickable.Add(Exit);
            ClickableToRemove.Add(Exit);

            up = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "up"
            };
            up.Origin = new Point(nameInputBox.Origin.X, Join.Origin.Y + Join.Height + topOffset);
            up.clickEvent += upClick;

            Clickable.Add(up);
            ClickableToRemove.Add(up);

            down = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "down"
            };
            down.clickEvent += downClick;
            Clickable.Add(down);
            ClickableToRemove.Add(down);


            g = new Grid(1, 8, buttonWidth, buttonHeight);
            g.Origin = new Point(Join.Origin.X, up.Origin.Y + up.Height + topOffset);
            g.AllVisible = false;
            g.MaxChildren = true;
            g.ChildMaxAmount = 8;
            g.VisibleRows = 5;
            Game1.self.Decks.ForEach(p =>
            {
                Deck d = new Deck(new Point(0, 0), buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, p.Name);
                d.SetFleet(p);
                d.clickEvent += DeckClick;
                d.ActiveChangeable = true;
                Clickable.Add(d);
                ClickableToRemove.Add(d);
                g.AddChild(d);
            });

            g.WitdhAndHeightColumnDependant = false;
            g.ConstantRowsAndColumns = true;
            g.UpdateP();
            ;
            Point downPoint = new Point(leftOrigin + leftOffset, g.Origin.Y + (int)g.RowOffset(5) + 10);
            down.Origin = downPoint;
            Exit.Origin = new Point(Exit.Origin.X, down.Origin.Y);


            layout.AddChild(creatornameInputBox);
            layout.AddChild(nameInputBox);
            layout.AddChild(Join);
            layout.AddChild(Create);
            layout.AddChild(labelError);
            layout.AddChild(Exit);
            layout.AddChild(g);
            layout.AddChild(up);
            layout.AddChild(down);
            popup = new Popup(new Point(leftOrigin, topOrigin), 4 * buttonWidth + leftOffset * 3, 7 * buttonHeight + topOffset * 7 + (int)g.RowOffset(5), Game1.self.GraphicsDevice, Gui);
            popup.layout = layout;

            nameInputBox.Update();
            creatornameInputBox.Update();
            g.UpdateP();
            Join.Update();
            Create.Update();
            labelError.Update();
            Exit.Update();
            up.Update();
            down.Update();
            ;
            popup.SetBackground();
            Game1.self.popupToDraw = popup;
            SetClickables(false);
            popup.SetActive(true);
            popup.layout.UpdateActive(true);
            Join.Active = false;
            Create.Active = false;
        }

        public void onExitCustom()
        {
            if (searching)
            {
                stopSearching = true;
            }
            else
            {
                popup.SetActive(false);
                foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
                {
                    clickable.Active = true;
                }

                chosenDeck = null;
                Game1.self.popupToDraw = null;
                ClickableToRemove.ForEach(p=> Clickable.Remove(p));
            }
        }

        public void onCreate()
        {
            if (chosenDeck != null && nameInputBox.Text.Length > 0)
            {
                Task t = new Task(CreateFunction);
                t.Start();
            }
        }

        public void onJoin()
        {
            if (chosenDeck != null && nameInputBox.Text.Length > 0 && creatornameInputBox.Text.Length > 0)
            {
                GamePacket packet = new GamePacket(OperationType.SELECT_FLEET, chosenDeck.GetFleet());
                Game1.self.Connection.Send(packet);
                packet = Game1.self.Connection.GetReceivedPacket();
                if (packet.OperationType == OperationType.SUCCESS)
                {
                    CustomGameRoom room = new CustomGameRoom(nameInputBox.Text);
                    room.CreatorsUsername = creatornameInputBox.Text;
                    packet = new GamePacket(OperationType.PLAY_CUSTOM_JOIN, room);
                    Game1.self.Connection.Send(packet);
                    packet = Game1.self.Connection.GetReceivedPacket();
                    if (packet.OperationType == OperationType.SUCCESS)
                    {
                        packet = Game1.self.Connection.GetReceivedPacket();
                        if (packet.OperationType == OperationType.SUCCESS)
                        {
                            Game1.self.SetFleetMenu(chosenDeck.GetFleet());
                            Game1.self.ReadyToPlay = true;
                            Game1.self.popupToDraw = null;
                            popup.SetActive(false);
                            SetClickables(true);
                            Game1.self.popupToDraw = null;
                            popup.layout = null;
                            ClickableToRemove.ForEach(p => Clickable.Remove(p));
                        }
                    }
                    else
                    {
                        labelError.Text = "There is no free room with that name";
                    }
                }
            }
        }
        public void CreateFunction()
        {
            if (chosenDeck != null)
            {
                searching = false;
                GamePacket packet = new GamePacket(OperationType.SELECT_FLEET, chosenDeck.GetFleet());
                Game1.self.Connection.Send(packet);
                packet = Game1.self.Connection.GetReceivedPacket();
                if (packet.OperationType == OperationType.SUCCESS)
                {
                    CustomGameRoom room = new CustomGameRoom(nameInputBox.Text,"",true,Game1.self.player.Username);
                    packet = new GamePacket(OperationType.PLAY_CUSTOM_CREATE, room);
                    Game1.self.Connection.Send(packet);
                    packet = Game1.self.Connection.GetReceivedPacket();
                    if (packet.OperationType == OperationType.SUCCESS)
                    {
                        labelError.Text = "Created room with name: " + nameInputBox.Text;
                        searching = true;
                        while (searching)
                        {
                            packet = Game1.self.Connection.GetReceivedPacket(10);
                            if (packet != null)
                            {
                                if (packet.OperationType == OperationType.SUCCESS)
                                {
                                    if (timer != null)
                                        timer.Dispose();
                                    Game1.self.SetFleetMenu(chosenDeck.GetFleet());
                                    Game1.self.ReadyToPlay = true;
                                    searching = false;
                                    popup.SetActive(false);
                                    SetClickables(true);
                                    Game1.self.popupToDraw = null;
                                    ClickableToRemove.ForEach(p => Clickable.Remove(p));
                                    searching = false;
                                    startedSearching = false;
                                    stopSearching = false;
                                    popup.layout = null;
                                    break;
                                }
                                else
                                    continue;
                            }

                            if (stopSearching)
                            {
                                stopSearching = false;
                                packet = new GamePacket(OperationType.ABANDON_GAME, new object());
                                Game1.self.Connection.Send(packet);
                                searching = false;
                                startedSearching = false;
                                break;
                            }
                        }
                    }

                }
            }
        }

        protected override void SetClickables(bool active)
        {
            foreach (var clickable in Clickable)
            {
                clickable.Active = active;
                if (popup != null)
                {
                    if (clickable.Parent == popup.layout)
                        clickable.Active = !active;
                }
                
            }
        }

        public void FillHistory(List<GameHistory> games)
        {
            int count = (Game1.self.graphics.PreferredBackBufferHeight - (200 + historyLabelHeight)) / historyLabelHeight;
            var list = games.OrderByDescending(p => p.GameDate).Take(count).ToList();
            lastGamesGrid.RemoveChildren();
            foreach (var gameHistory in list)
            {
                lastGamesGrid.AddChild(gameToLabel(gameHistory));
            }
        }
        private Label gameToLabel(GameHistory game)
        {
            Label lbl = new Label(500, historyLabelHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            string part1, part0;
            string enemy;
            Fleet myFleet;
            part0 = game.GameDate.ToString()+" \n";
            if (game.Winner.Id == Game1.self.player.Id)
            {
                part1 = "You won against ";
                enemy = game.Loser.Username;
                myFleet = game.WinnerFleet;
            }
            else if (game.WasDraw)
            {
                if (game.Loser.Id == Game1.self.player.Id)
                {
                    enemy = game.Winner.Username;
                    myFleet = game.LoserFleet;
                }
                else
                {
                    enemy = game.Loser.Username;
                    myFleet = game.WinnerFleet;
                }
                part1 = "You drew with ";
            }
            else
            {
                part1 = "You lost against ";
                enemy = game.Winner.Username;
                myFleet = game.LoserFleet;
            }

            lbl.Text = part0;
            lbl.Text += part1;
            lbl.Text += enemy + " with fleet: " + myFleet.Name;
            return lbl;
        }

        public void searchClick()
        {
            if (chosenDeck != null)
            {
                Task task = new Task(searchFunction);
                task.Start();
                TurnPop(false);
                exit.Active = true;
                startedSearching = true;
            }
        }

        public void TurnPop(bool active)
        {
            foreach (var clickable in Clickable)
            {
                if (popup != null)
                {
                    if (clickable.Parent == popup.layout)
                        clickable.Active = active;
                    if(!active)
                        exit.Active = !active;
                }
            }
        }
        public void searchFunction()
        {
            if (chosenDeck != null)
            {
                searching = false;
                GamePacket packet = new GamePacket(OperationType.SELECT_FLEET, chosenDeck.GetFleet());
                Game1.self.Connection.Send(packet);
                packet = Game1.self.Connection.GetReceivedPacket();
                if (packet.OperationType == OperationType.SUCCESS)
                {
                    var autoEvent = new AutoResetEvent(false);
                    time = 0;
                    timer = new Timer(timerStart, autoEvent, 0, 1000);
                    //TODO not tested
                    packet = new GamePacket(OperationType.PLAY_RANKED, null);
                    Game1.self.Connection.Send(packet);
                    packet = Game1.self.Connection.GetReceivedPacket();
                    if (packet.OperationType == OperationType.SUCCESS)
                    {
                        searching = true;
                        while (searching)
                        {
                            packet = Game1.self.Connection.GetReceivedPacket(10);
                            if (packet != null)
                            {
                                if (packet.OperationType == OperationType.SUCCESS)
                                {
                                    if (timer != null)
                                        timer.Dispose();
                                    Game1.self.SetFleetMenu(chosenDeck.GetFleet());
                                    Game1.self.ReadyToPlay = true;
                                    searching = false;
                                    popup.SetActive(false);
                                    SetClickables(true);
                                    Game1.self.popupToDraw = null;
                                    ClickableToRemove.ForEach(p =>  Clickable.Remove(p));
                                    searching = false;
                                    startedSearching = false;
                                    stopSearching = false;
                                    popup.layout = null;
                                    break;
                                }
                                else
                                    continue;
                            }

                            if (stopSearching)
                            {
                                stopSearching = false;
                                packet = new GamePacket(OperationType.ABANDON_GAME,new object());
                                Game1.self.Connection.Send(packet);
                                searching = false;
                                startedSearching = false;
                                break;
                            }
                        }
                    }

                }
            }
        }
        public void timerStart(object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            time++;
            labelWaiting.Text = "Searching game for " + time + " seconds.";
        }
        public void exitClick()
        {
            if (searching)
            {
                stopSearching = true;
                
                if (timer != null)
                    timer.Dispose();
                TurnPop(true);
                popup.SetActive(true);
            }
            else
            {
                popup.SetActive(false);
                foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
                {
                    clickable.Active = true;
                }

                chosenDeck = null;
                Game1.self.popupToDraw = null;
                ClickableToRemove.ForEach(p => Clickable.Remove(p));
            }
        }
        public void DeckClick(object sender)
        {
            foreach (var clickable  in Clickable.Where(p=> p is Deck))
            {
                (clickable as Deck).Chosen = false;
            }
            Deck d = (Deck)sender;
            d.Chosen = true;
            chosenDeck = d;
            if (search != null)
            {
                search.ActiveChangeable = true;
                search.Active = true;
            }

            if (Join != null)
            {
                Join.Active = true;
                Create.Active = true;
            }
        }

        public void downClick()
        {
            g.ChangeRow(1);
        }

        public void upClick()
        {
            g.ChangeRow(-1);
        }
        public void GoToDeck()
        {
            Game1.self.state = Game1.State.DeckMenu;
            Game1.self.SetDecks(Game1.self.Decks);
            Game1.self.CleanDeck();
        }

        public void GoToShop()
        {
            GamePacket packet = new GamePacket(OperationType.GET_LOOTBOXES, null);
            Game1.self.Connection.Send(packet);
            packet = Game1.self.Connection.GetReceivedPacket();
            List<LootBox> loots = (List<LootBox>) packet.Packet;
            Game1.self.SetMoney(Game1.self.player.Money);
            Game1.self.SetShop(loots);
            Game1.self.state = Game1.State.ShopMenu;
        }
        public void ExitClick()
        {
            Game1.self.Quit();
        }

        public void GoToSettings()
        {
            Game1.self.SetSettings();
            Game1.self.state = Game1.State.OptionsMenu;
        }

        public void UpdatePlayer()
        {
            GamePacket packet = new GamePacket(OperationType.PLAYER_DATA,Game1.self.player);
            Game1.self.Connection.Send(packet);
            packet = Game1.self.Connection.GetReceivedPacket();
            Game1.self.player = (Player) packet.Packet;
            packet = new GamePacket(OperationType.VIEW_ALL_PLAYER_SHIPS,null);
            Game1.self.Connection.Send(packet);
            packet = Game1.self.Connection.GetReceivedPacket();
            Game1.self.OwnedShips = (List<Ship>) packet.Packet;
        }

        public override void UpdateGrid()
        {
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            if (popup != null)
            {
                
                    SetClickables(!popup.Active);
                    popup.SetActive(popup.Active);
                if (startedSearching)
                    TurnPop(!startedSearching);
            }
            else
            {
                SetClickables(true);
            }
        }

        public override void UpdateLast()
        {
            if (nameInputBox != null)
            {
                nameInputBox.Update();
                creatornameInputBox.Update();
            }
        }

        public void Draw(GameTime gameTime)
        {

            grid.Draw(Game1.self.spriteBatch);
            lastGamesGrid.Draw(Game1.self.spriteBatch);

        }
    }
}
