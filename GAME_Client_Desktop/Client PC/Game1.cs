using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Serialization;
using Client_PC.Scenes;
using Client_PC.UI;
using Client_PC.Utilities;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Button = Client_PC.UI.Button;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using GameWindow = Client_PC.Scenes.GameWindow;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Label = Client_PC.UI.Label;
using MainMenu = Client_PC.Scenes.MainMenu;
using Menu = Client_PC.Scenes.Menu;

namespace Client_PC
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public enum State
        {
            LoginMenu,MainMenu,OptionsMenu,GameWindow,DeckMenu,RegisterMenu,ShopMenu, FleetMenu
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
        private GameWindow gameWindow;
        private ShopMenu shopMenu;
        private FleetMenu fleetMenu;
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
        private bool test = true; // false if dont connect with server
        public bool ReadyToPlay;





        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            LoadConfig();
            Wallpaper = Utils.CreateTexture(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //  gd = GraphicsDevice;
            mainMenu = new MainMenu();
            settingsMenu = new SettingsMenu();
            settingsMenu.Initialize(Content);
            mainMenu.Initialize(Content);
            loginMenu = new LoginMenu();
            loginMenu.Initialize(Content);
            registerMenu = new RegisterMenu();
            registerMenu.Initialize(Content);
            deckMenu = new DeckMenu();
            deckMenu.Initialize(Content);
            gameWindow = new GameWindow();
            gameWindow.Initialize(Content);
            shopMenu = new ShopMenu();
            shopMenu.Initialize(Content);
            fleetMenu = new FleetMenu();
            fleetMenu.Initialize(Content);
            menus.Add(mainMenu);
            menus.Add(settingsMenu);
            menus.Add(loginMenu);
            menus.Add(registerMenu);
            menus.Add(deckMenu);
            menus.Add(gameWindow);
            menus.Add(shopMenu);
            menus.Add(fleetMenu);

            settingsMenu.SetMenus(menus);
            base.Initialize();
            
        }

        private void setUpConnection()
        {
            if (test)
            {
                try
                {
                    string server = "212.191.92.88";
                    int port = GAME_connection.TcpConnection.DEFAULT_PORT_CLIENT;
                    TcpClient client = new TcpClient(server, port);
                    Console.WriteLine("tcpClient created");
                    Connection = new TcpConnection(client, true, null, false, false, null);
                    Console.WriteLine("Connection established");
                }
                catch (Exception e)
                {
                    
                }
            }
        }

        public void Quit()
        {
            Connection.SendDisconnect();
            this.Exit();
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
            Darker = Content.Load<Effect>("Shaders/GrayScaleShader");

            Form MyGameForm = (Form)Form.FromHandle(Window.Handle);
            MyGameForm.Closing += ClosingFunction;
            // TODO: use this.Content to load your game content here
        }
        public void ClosingFunction(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Connection?.Disconnect();

        }

        public void StartGame()
        {
            gameWindow.Start();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
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
            }
            base.Update(gameTime);
        }

        public void CleanDeck()
        {
            deckMenu.Clean();
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
            fleetMenu.setFleet(fleet);
            fleetMenu.Fill();
        }
        public void CleanRegister()
        {
            registerMenu.Clean();
        }
        private void WallpaperChange()
        {
            Random rndRandom = new Random();
            int width = Wallpaper.Width;
            double minAddition = 1;
            double maxAddition = 1;
            int colorRange = 5;
            int startMin = 0;
            int startMax = 255;
            Color[] data = new Color[Wallpaper.Width * Wallpaper.Height];
            Wallpaper.GetData(data);
            for (int i = 0; i < 10000; i++)
            {
                int z = rndRandom.Next(0, data.Length - 1);
                ChangePixel(z, width, data, rndRandom, minAddition, colorRange, maxAddition, startMin, startMax);




            }
            Wallpaper.SetData(data);
            
        }

        private void ChangePixel(int z, int width, Color[] data, Random rndRandom, double minAddition, int colorRange,
            double maxAddition, int startMin, int startMax)
        {
            Color empty = new Color();
            if (z % width > 0 && data[z - 1] != empty)
            {
                if (z - width > 0 && data[z - width] != empty) // inside
                {
                    if (z - 2 * width > 0 && data[z - 2 * width] != empty && (z - 2) % width > 0) // further rows inside
                    {
                        Color cl = new Color(
                                rndRandom.Next(
                                    (int) Math.Round((data[z - 1].R + data[z - width].R + data[z - 2].R +
                                                      data[z - 2 * width].R + minAddition) / 4.0f - colorRange),
                                    (int) Math.Round((data[z - 1].R + data[z - width].R + data[z - 2].R +
                                                      data[z - 2 * width].R + maxAddition) / 4.0f + colorRange)),
                                rndRandom.Next(
                                    (int) Math.Round((data[z - 1].G + data[z - width].G + data[z - 2].G +
                                                      data[z - 2 * width].G + minAddition) / 4.0f - colorRange),
                                    (int) Math.Round((data[z - 1].G + data[z - width].G + data[z - 2].G +
                                                      data[z - 2 * width].G + maxAddition) / 4.0f + colorRange)),
                                rndRandom.Next(
                                    (int) Math.Round((data[z - 1].B + data[z - width].B + data[z - 2].B +
                                                      data[z - 2 * width].B + minAddition) / 4.0f - colorRange),
                                    (int) Math.Round((data[z - 1].B + data[z - width].B + data[z - 2].B +
                                                      data[z - 2 * width].B + maxAddition) / 4.0f + colorRange)))
                            ;
                        data[z] = cl;
                    }
                    else // first row inside
                    {
                        Color cl = new Color(
                                rndRandom.Next((int) Math.Floor((data[z - 1].R + data[z - width].R) / 2.0f - colorRange),
                                    (int) Math.Ceiling((data[z - 1].R + data[z - width].R) / 2.0f + colorRange)),
                                rndRandom.Next((int) Math.Floor((data[z - 1].G + data[z - width].G) / 2.0f - colorRange),
                                    (int) Math.Ceiling((data[z - 1].G + data[z - width].G) / 2.0f + colorRange)),
                                rndRandom.Next((int) Math.Floor((data[z - 1].B + data[z - width].B) / 2.0f - colorRange),
                                    (int) Math.Ceiling((data[z - 1].B + data[z - width].B) / 2.0f + colorRange)))
                            ;
                        data[z] = cl;
                    }
                }
                else // top edge
                {
                    Color cl = new Color(
                        rndRandom.Next(data[z - 1].R - colorRange, data[z - 1].R + colorRange),
                        rndRandom.Next(data[z - 1].G - colorRange, data[z - 1].G + colorRange),
                        rndRandom.Next(data[z - 1].B - colorRange, data[z - 1].B + colorRange)
                    );
                    data[z] = cl;
                }
            }
            else if (z - width >= 0 && data[z - width] != empty) // left edge
            {
                Color cl = new Color(
                    rndRandom.Next((data[z - width].R) - colorRange, (data[z - width].R) + colorRange),
                    rndRandom.Next((data[z - width].G) - colorRange, (data[z - width].G) + colorRange),
                    rndRandom.Next((data[z - width].B) - colorRange, (data[z - width].B) + colorRange));

                data[z] = cl;
            }
            else //  first pixel
            {
                Color cl = new Color(rndRandom.Next(startMin, startMax), rndRandom.Next(startMin, startMax),
                    rndRandom.Next(startMin, startMax));
                data[z] = cl;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Game1.self.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //Darker.CurrentTechnique.Passes[0].Apply();
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
