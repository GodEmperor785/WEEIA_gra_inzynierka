using System.Net.Mime;
using Client_PC.Scenes;
using Client_PC.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        public float DeltaSeconds;
        public bool AbleToClick;
        internal object graphicsDevice;
        internal IClickable FocusedElement;
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
            //  gd = GraphicsDevice;
            mainMenu = new MainMenu();
            settingsMenu = new SettingsMenu();
            settingsMenu.Initialize(Content);
            mainMenu.Initialize(Content);
            loginMenu = new LoginMenu();
            loginMenu.Initialize(Content);
            registerMenu = new RegisterMenu();
            registerMenu.Initialize(Content);
            base.Initialize();
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
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
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
            }
            base.Draw(gameTime);
        }
    }
}
