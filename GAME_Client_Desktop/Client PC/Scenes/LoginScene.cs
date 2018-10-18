using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class LoginScene
    {
        private GUI Gui;
        private Grid grid;
        private List<IClickable> Clickable;
        private bool AbleToClick;
        private Keys[] LastPressedKeys;

        public LoginScene()
        {
            Clickable = new List<IClickable>();
        }

        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Label labelLogin = new Label(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "Login"
            };
            Label labelPassword = new Label(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password"
            };
            InputBox inputLogin = new InputBox(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false );
            inputLogin.TextLimit = 24;
            InputBox inputPassword = new InputBox(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            inputPassword.TextLimit = 24;
            Button loginButton = new Button(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "Log in"
            };
            Button registerButton = new Button(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Register"
            };
            Button exitButton = new Button(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
            };
            Clickable.Add(inputLogin);
            Clickable.Add(inputPassword);
            Clickable.Add(loginButton);
            Clickable.Add(registerButton);
            Clickable.Add(exitButton);
            grid = new Grid();
            grid.AddChild(labelLogin,0,0);
            grid.AddChild(inputLogin,0,1,2);
            grid.AddChild(labelPassword,1,0);
            grid.AddChild(inputPassword,1,1,2);
            grid.AddChild(loginButton,2,0);
            grid.AddChild(registerButton,2,1);
            grid.AddChild(exitButton,2,2);
            loginButton.clickEvent += LoginClick;
            exitButton.clickEvent += ExitClick;
            registerButton.clickEvent += RegisterClick;
            grid.ResizeChildren();
        }
        public void Update(GameTime gameTime)
        {
            Game1.self.DeltaSeconds += gameTime.ElapsedGameTime.Milliseconds;
            if (Game1.self.DeltaSeconds > 250)
            {
                Game1.self.AbleToClick = true;
            }
            var mouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();
            Utils.UpdateKeyboard(keyboardState, ref LastPressedKeys);
            if (mouseState.LeftButton == ButtonState.Pressed && Game1.self.AbleToClick)
            {
                Game1.self.DeltaSeconds = 0;
                Game1.self.AbleToClick = false;
                int x = mouseState.X;
                int y = mouseState.Y;
                Point xy = new Point(x, y);
                IClickable button = Clickable.SingleOrDefault(p => p.GetBoundary().Contains(xy));
                if (button != null)
                {
                    Game1.self.FocusedElement = button;
                    button.OnClick();
                }
                else
                {
                    Game1.self.FocusedElement = null;
                }
            }
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
        }
        public void Draw(GameTime gameTime)
        {
            Game1.self.GraphicsDevice.Clear(Color.IndianRed);
            grid.Draw(Game1.self.spriteBatch);
        }
        public void ExitClick()
        {
            Game1.self.Exit();
        }

        public void LoginClick()
        {
            Game1.self.state = Game1.State.MainMenu;
        }

        public void RegisterClick()
        {

        }

        
    }
}
