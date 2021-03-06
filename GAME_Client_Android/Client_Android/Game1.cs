using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Serialization;
using Client_PC.Scenes;
using Client_PC.UI;
using Client_PC.Utilities;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client_Android
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public List<Ship> CardTypes;
        public class ShipAndSkin
        {
            public string ship;
            public Texture2D skin;
        }
        public class SkinAndPath
        {
            public Texture2D skin;
            public string path;
        }
        public enum State
        {
            LoginMenu, MainMenu, OptionsMenu, GameWindow, DeckMenu, RegisterMenu, ShopMenu, FleetMenu, CardsMenu
        }
        public static Game1 self;
        public State state = State.LoginMenu;
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        GraphicsDevice gd;
        private MainMenu mainMenu;
        private SettingsMenu settingsMenu;
        private LoginMenu loginMenu;
        private RegisterMenu registerMenu;
        private DeckMenu deckMenu;
        private Client_PC.Scenes.GameWindow gameWindow;
        private ShopMenu shopMenu;
        private FleetMenu fleetMenu;
        private CardsMenu cardsMenu;
        public float DeltaSeconds;
        public bool AbleToClick;
        internal Tooltip tooltipToDraw;
        internal Popup popupToDraw;
        public Config conf;
        internal object graphicsDevice;
        internal IClickable FocusedElement;
        internal Texture2D Wallpaper;
        public Player player;
        public List<Fleet> Decks { get; set; }
        public List<Ship> OwnedShips { get; set; }
        public Effect Darker;
        public RasterizerState RasterizerState = new RasterizerState() { ScissorTestEnable = true };
        List<Menu> menus = new List<Menu>();
        public BaseModifiers Modifiers;
        public GAME_connection.TcpConnection Connection;
        public List<ShipAndSkin> EnemyShipsSkins = new List<ShipAndSkin>();
        public List<ShipAndSkin> ShipsSkins = new List<ShipAndSkin>();
        public List<SkinAndPath> SkinsPaths = new List<SkinAndPath>();
        private bool test = true; // false if dont connect with server
        public bool ReadyToPlay;
        public Activity1 activitySelf;
        public string ServerIp = "212.191.92.88";
        public Cards config;
        public Game1(Activity1 activity)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            activitySelf = activity;
            setUpConnection();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            self = this;
            //LoadConfig();
            Wallpaper = Utils.CreateTexture(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //  gd = GraphicsDevice;

            loginMenu = new LoginMenu();
            loginMenu.Initialize(Content);
            registerMenu = new RegisterMenu();
            registerMenu.Initialize(Content);
            menus.Add(loginMenu);
            menus.Add(registerMenu);
            base.Initialize();
            state = State.LoginMenu;
        }

        public void LoginInitialize()
        {
            
            mainMenu = new MainMenu();
            settingsMenu = new SettingsMenu();
            settingsMenu.Initialize(Content);
            mainMenu.Initialize(Content);
            deckMenu = new DeckMenu();
            deckMenu.Initialize(Content);
            gameWindow = new Client_PC.Scenes.GameWindow();
            gameWindow.Initialize(Content);
            shopMenu = new ShopMenu();
            shopMenu.Initialize(Content);
            fleetMenu = new FleetMenu();
            fleetMenu.Initialize(Content);
            cardsMenu = new CardsMenu();
            cardsMenu.Initialize(Content);
            menus.Add(mainMenu);
            menus.Add(settingsMenu);
            menus.Add(deckMenu);
            menus.Add(gameWindow);
            menus.Add(shopMenu);
            menus.Add(fleetMenu);
            menus.Add(cardsMenu);

            settingsMenu.SetMenus(menus);
            
        }


        public void setUpConnection()
        {
            if (test)
            {
                try
                {
                    int port = TcpConnection.DEFAULT_PORT_CLIENT;
                    TcpClient client = new TcpClient(ServerIp, port);
                    Connection = new TcpConnection(client, true, Nothing, false, true);
                }
                catch (Exception e)
                {

                }
            }
        }

        public void Nothing(string c)
        {

        }
        private void LoadAllCards()
        {
            CardTypes = new List<Ship>();
            GamePacket packet = new GamePacket(OperationType.GET_SHIP_TEMPLATES, null);
            Connection.Send(packet);
            packet = Connection.GetReceivedPacket();
            if (packet.OperationType == OperationType.GET_SHIP_TEMPLATES)
            { //TODO make it when server is working with it
                CardTypes = (List<Ship>)packet.Packet;
            }

        }
        public void SetSettings()
        {
            settingsMenu.FillGridWithCardTypes(CardTypes);
        }
        public void LoadCardTextures()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            LoadAllCards();
            XmlSerializer serializer = new XmlSerializer(typeof(Cards));
            if (!store.FileExists("Config_Cards"))
            {
                var fs = store.CreateFile("Config_Cards");
                TextWriter writer = new StreamWriter(fs);
                config = new Cards();
                config.listOfCards = new List<CardConfig>();
                CardTypes.ForEach(p =>
                {
                    CardConfig c = new CardConfig();
                    c.Name = p.Name;
                    c.SkinPath = String.Empty;
                    config.listOfCards.Add(c);
                });
                XmlSerializer xml = new XmlSerializer(typeof(Cards));
                xml.Serialize(writer, config);
                writer.Close();
                fs.Close();
            }

            var file = store.OpenFile("Config_Cards",FileMode.Open);
            
            config = (Cards)serializer.Deserialize(file);
            file.Close();
            ShipsSkins.Clear();
            config.listOfCards.ForEach(p =>
            {
                CardTypes.Where(a => a.Name == p.Name).ToList().ForEach(z =>
                {
                    
                        Texture2D skin = store.FileExists(p.SkinPath) ? loadTexture2D(p.SkinPath) : null;
                        ShipsSkins.Add(new ShipAndSkin()
                        {
                            ship = z.Name,
                            skin = skin

                        });
                        SkinsPaths.Add(new SkinAndPath()
                        {
                            skin = skin,
                            path = p.SkinPath
                        });
                    
                    
                });
            });
            file.Close();
            store.Close();
        }
        public Texture2D loadTexture2D(string path)
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            Texture2D text = null;
            var file = store.OpenFile(path, FileMode.Open);

            text = Texture2D.FromStream(Game1.self.GraphicsDevice, file);
            store.Close();
            return text;
        }
        private void LoadConfig()
        {
            XmlSerializer serializer =
                new XmlSerializer(typeof(Config));
            try
            {
                using (FileStream fs = new FileStream("Config", FileMode.Open))
                {

                }

            }
            catch
            {
                TextWriter writer = new StreamWriter("Config");
                XmlSerializer xml = new XmlSerializer(typeof(Config));
                xml.Serialize(writer, Config.Default());
                writer.Close();
            }



            using (Stream reader = new FileStream("Config", FileMode.Open))
            {
                conf = (Config)serializer.Deserialize(reader);
            }

            UseConfig();
        }
        private void UseConfig()
        {
            #region Resolution

            int height = 0;
            int width = 0;
            if (conf.Resolution.Equals(Constants.hd))
            {
                height = 720;
                width = 1080;
            }
            if (conf.Resolution.Equals(Constants.fullhd))
            {
                height = 1080;
                width = 1920;
            }

            Game1.self.graphics.PreferredBackBufferHeight = height;
            Game1.self.graphics.PreferredBackBufferWidth = width;
            Game1.self.graphics.ApplyChanges();
            #endregion
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Game.Activity.Assets.Open()
            //var filePath = Path.Combine(Content.RootDirectory, "Shaders/GrayScaleShader");
            
            Darker = Content.Load<Effect>("GrayScaleShader");
        }

        public void StartGame()
        {
            gameWindow.Start();
        }

        public void SetMoney(int amount)
        {
            shopMenu.SetMoney(amount);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            if (ReadyToPlay)
            {
                ReadyToPlay = false;
                state = State.FleetMenu;
            }
            switch (state)
            {
                case State.MainMenu:
                    mainMenu.Update(gameTime);
                    break;
                case State.OptionsMenu:
                    settingsMenu.Update(gameTime);
                    break;
                case State.LoginMenu:
                    loginMenu.Update(gameTime);
                    break;
                case State.RegisterMenu:
                    registerMenu.Update(gameTime);
                    break;
                case State.DeckMenu:
                    deckMenu.Update(gameTime);
                    break;
                case State.GameWindow:
                    gameWindow.Update(gameTime);
                    break;
                case State.ShopMenu:
                    shopMenu.Update(gameTime);
                    break;
                case State.FleetMenu:
                    fleetMenu.Update(gameTime);
                    break;
                case State.CardsMenu:
                    cardsMenu.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        public void CleanDeck()
        {
            deckMenu.Clean();
        }

        public void CleanCards()
        {
            cardsMenu.Clean();
        }

        public void SetDecks(List<Fleet> fleets)
        {
            deckMenu.LoadDecksAndShips(fleets, OwnedShips);
        }
        public void CleanLogin()
        {
            loginMenu.Clean();
        }

        public void UpdatePlayer()
        {
            mainMenu.UpdatePlayer();
        }
        public void SetShop(List<LootBox> loots)
        {
            shopMenu.Reinitialize(loots);
        }

        public void SetFleetMenu(Fleet fleet)
        {
            fleetMenu.ReDo();
            fleetMenu.setFleet(fleet);
            fleetMenu.Fill();
        }
        public void CleanRegister()
        {
            registerMenu.Clean();
        }

        public void UpdateHistory()
        {
            GamePacket packet = new GamePacket(OperationType.GET_PLAYER_STATS, Game1.self.player);
            Connection.Send(packet);
            packet = Connection.GetReceivedPacket();
            if (packet.OperationType == OperationType.GET_PLAYER_STATS)
            {
                mainMenu.FillHistory((List<GameHistory>)packet.Packet);
            }

        }

        public void Quit()
        {
            Connection.SendDisconnect();
            activitySelf.HideKeyboard();
            this.Exit();
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Game1.self.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (popupToDraw != null)
            {
                Darker.CurrentTechnique.Passes[0].Apply();
                popupToDraw.Draw(spriteBatch);
            }
            Game1.self.spriteBatch.Draw(Game1.self.Wallpaper, new Vector2(0, 0), Color.White);

            switch (state)
            {
                case State.MainMenu:
                    mainMenu.Draw(gameTime);
                    break;
                case State.OptionsMenu:
                    settingsMenu.Draw(gameTime);
                    break;
                case State.LoginMenu:
                    loginMenu.Draw(gameTime);
                    break;
                case State.RegisterMenu:
                    registerMenu.Draw(gameTime);
                    break;
                case State.DeckMenu:
                    deckMenu.Draw(gameTime);
                    break;
                case State.GameWindow:
                    gameWindow.Draw(gameTime);
                    break;
                case State.ShopMenu:
                    shopMenu.Draw(gameTime);
                    break;
                case State.FleetMenu:
                    fleetMenu.Draw(gameTime);
                    break;
                case State.CardsMenu:
                    cardsMenu.Draw(gameTime);
                    break;
            }

            if (tooltipToDraw != null)
            {
                tooltipToDraw.Draw(spriteBatch);
            }
            base.Draw(gameTime);
            Game1.self.spriteBatch.End();
            if (popupToDraw != null)
            {
                Game1.self.spriteBatch.Begin();
                popupToDraw.Draw(spriteBatch);
                Game1.self.spriteBatch.End();
            }
        }
    }
}
