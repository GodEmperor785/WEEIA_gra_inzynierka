using Client_PC.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class RegisterMenu
    {
        private GUI Gui;
        private Grid grid;
        private List<IClickable> Clickable;
        private bool AbleToClick;
        private Keys[] LastPressedKeys;

        public RegisterMenu()
        {
            Clickable = new List<IClickable>();
        }

        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Button registerButton = new Button(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                text = "Register"
            };
            Button backButton = new Button(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "Back"
            };
            Label loginLabel = new Label(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                Text = "Login"
            };
            Label passwordLabel = new Label(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password"
            };
            Label passwordLabel2 = new Label(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password again"
            };
            InputBox loginInputBox = new InputBox(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false )
            {
                TextLimit = 30
            };
            InputBox passwordInputBox = new InputBox(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false)
            {
                TextLimit = 30
            };
            InputBox passwordInputBox2 = new InputBox(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false)
            {
                TextLimit = 30
            };
            Clickable.Add(registerButton);
            Clickable.Add(backButton);
            Clickable.Add(loginInputBox);
            Clickable.Add(passwordInputBox);
            Clickable.Add(passwordInputBox2);
            grid = new Grid();
            grid.AddChild(loginLabel,0,0);
            grid.AddChild(passwordLabel,1,0);
            grid.AddChild(passwordLabel2,2,0);
            grid.AddChild(loginInputBox,0,1);
            grid.AddChild(passwordInputBox,1,1);
            grid.AddChild(passwordInputBox2,2,1);
            grid.AddChild(registerButton,3,0);
            grid.AddChild(backButton,3,1);
            backButton.clickEvent += backClick;
            registerButton.clickEvent += registerClick;
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            grid.ResizeChildren();
        }

        public void backClick()
        {
            Game1.self.state = Game1.State.LoginMenu;
        }

        public void registerClick()
        {
            Game1.self.state = Game1.State.LoginMenu;
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
            Game1.self.GraphicsDevice.Clear(Color.YellowGreen);
            // Game1.self.spriteBatch.Begin();
            grid.Draw(Game1.self.spriteBatch);
            //Game1.self.spriteBatch.End();
        }
    }
}
