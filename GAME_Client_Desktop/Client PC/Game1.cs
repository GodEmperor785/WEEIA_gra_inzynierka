﻿using System.IO;
using System.Net.Mime;
using System.Xml.Serialization;
using Client_PC.Scenes;
using Client_PC.UI;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameWindow = Client_PC.Scenes.GameWindow;

namespace Client_PC
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public enum State
        {
            LoginMenu,MainMenu,OptionsMenu,GameWindow,DeckMenu,RegisterMenu
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
        public float DeltaSeconds;
        public bool AbleToClick;
        internal Tooltip tooltipToDraw;
        public Config conf;
        internal object graphicsDevice;
        internal IClickable FocusedElement;
        internal Texture2D Wallpaper;
        public RasterizerState RasterizerState = new RasterizerState() { ScissorTestEnable = true };
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
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
            base.Initialize();
            
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
            // TODO: use this.Content to load your game content here
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
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Game1.self.spriteBatch.Begin();
            Game1.self.spriteBatch.Draw(Game1.self.Wallpaper, new Vector2(0, 0), Color.White);
            Game1.self.spriteBatch.End();
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
            }

            if (tooltipToDraw != null)
            {
                tooltipToDraw.Draw(spriteBatch);
            }
            base.Draw(gameTime);
        }
    }
}
