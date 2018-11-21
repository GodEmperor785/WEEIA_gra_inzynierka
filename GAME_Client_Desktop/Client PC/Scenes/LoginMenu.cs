using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class LoginMenu : Menu
    {
        private Grid grid;
        private Popup popup;
        private InputBox inputLogin;
        private InputBox inputPassword;

        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Label labelLogin = new Label(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "Login"
            };
            Label labelPassword = new Label(new Point(0, 0), 115, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password"
            };
            inputLogin = new InputBox(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false );
            inputLogin.TextLimit = 30;
            inputPassword = new InputBox(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            inputPassword.TextLimit = 30;
            Button loginButton = new Button(new Point(0,0),105,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "Log in"
            };
            Button registerButton = new Button(new Point(0, 0), 105, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Register"
            };
            Button exitButton = new Button(new Point(0, 0), 105, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
            };

            popup = new Popup(new Point((int)(Game1.self.graphics.PreferredBackBufferHeight * 0.25), (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.25)),100,400,Game1.self.GraphicsDevice,Gui);
            Grid popupGrid = new Grid();
            Label lbl1 = new Label(200, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Incorrect login or password"
            };
            Button b1 = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
            };
            lbl1.DrawBackground = false;
            b1.DrawBackground = false;
            popup.grid = popupGrid;
            popupGrid.AddChild(lbl1,0,0);
            popupGrid.AddChild(b1, 1, 0);
            b1.clickEvent += onPopupExit;
            Clickable.Add(b1);
            popup.SetToGrid();


            Clickable.Add(inputLogin);
            Clickable.Add(inputPassword);
            Clickable.Add(loginButton);
            Clickable.Add(registerButton);
            Clickable.Add(exitButton);
            grid = new Grid();
            Button refresh = new Button(new Point(0, 0), 50, 50, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            refresh.clickEvent += RefResh;
            Clickable.Add(refresh);
            grid.AddChild(labelLogin,0,0);
            grid.AddChild(inputLogin,0,1,2);
            grid.AddChild(labelPassword,1,0);
            grid.AddChild(inputPassword,1,1,2);
            grid.AddChild(loginButton,2,0);
            grid.AddChild(registerButton,2,1);
            grid.AddChild(exitButton,2,2);
            grid.AddChild(refresh,3,0);
            loginButton.clickEvent += LoginClick;
            exitButton.clickEvent += ExitClick;
            registerButton.clickEvent += RegisterClick;
            grid.ResizeChildren();
            SetClickables(true);
        }

        public void onPopupExit()
        {
            popup.SetActive(false);
            foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
            {
                clickable.Active = true;
            }
        }

        protected override void SetClickables(bool active)
        {
            foreach (var clickable in Clickable)
            {
                clickable.Active = active;
                if (clickable.Parent == popup.grid)
                    clickable.Active = !active;
            }
        }

        public void RefResh()
        {
            Game1.self.Wallpaper = Utils.CreateTexture(Game1.self.GraphicsDevice, Game1.self.graphics.PreferredBackBufferWidth, Game1.self.graphics.PreferredBackBufferHeight);
        }
        public override void UpdateGrid()
        {
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            SetClickables(!popup.Active);
        }
        public void Draw(GameTime gameTime)
        {
           
            grid.Draw(Game1.self.spriteBatch);
            if (popup.Active)
            {
                popup.Draw(Game1.self.spriteBatch);
            }
        }
        public void ExitClick()
        {
            Game1.self.Exit();
        }

        public void LoginClick()
        {
            if (inputLogin.Text == "1" && inputPassword.Text == "1")
            {
                Game1.self.state = Game1.State.MainMenu;
            }
            else
            {
                popup.SetActive(true);
                inputLogin.Text = "";
                inputPassword.Text = "";
                SetClickables(false);
            }
        }

        public void RegisterClick()
        {
            Game1.self.state = Game1.State.RegisterMenu;
        }

        
    }
}
