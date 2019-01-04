using Client_PC.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.Utilities;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class RegisterMenu : Menu
    {
        private Grid grid;
        private InputBox passwordInputBox;
        private InputBox passwordInputBox2;
        private InputBox loginInputBox;
        private Popup popup;
        private Label lbl1;
        private Button registerButton;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            registerButton = new Button(new Point(0,0),120,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                text = "Register"
            };
            Button backButton = new Button(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "Back"
            };
            Label loginLabel = new Label(new Point(0,0), 120, 60,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true )
            {
                Text = "Login"
            };
            Label passwordLabel = new Label(new Point(0, 0), 120, 60, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password"
            };
            Label passwordLabel2 = new Label(new Point(0, 0), 120, 60, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password again"
            };
            loginInputBox = new InputBox(new Point(0,0), 300, 60, Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false )
            {
                TextLimit = 30
            };
            passwordInputBox = new InputBox(new Point(0, 0), 300, 60, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false)
            {
                TextLimit = 30
            };
            passwordInputBox2 = new InputBox(new Point(0, 0), 300, 60, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false)
            {
                TextLimit = 30
            };
            Tooltip tooltipLogin = new Tooltip(200, 150, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Maximum 30 characters"
            };

            Tooltip tooltipPassword1 = new Tooltip(200, 150, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Maximum 30 characters"
            };
            Tooltip tooltipPassword2 = new Tooltip(200, 150, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Write your password once again."
            };
            loginInputBox.Tooltip = tooltipLogin;
            passwordInputBox.Tooltip = tooltipPassword1;
            passwordInputBox2.Tooltip = tooltipPassword2;
            passwordInputBox.IsPassword = true;
            passwordInputBox2.IsPassword = true;


            #region popup
            popup = new Popup(new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * 0.5), (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.5)), 100, 400, Game1.self.GraphicsDevice, Gui);
            Grid popupGrid = new Grid();
            lbl1 = new Label(200, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Both passwords must be the same"
            };
            Button b1 = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
            };
            b1.Active = false;
            lbl1.DrawBackground = false;
            b1.DrawBackground = false;
            popup.grid = popupGrid;
            popupGrid.AddChild(lbl1, 0, 0);
            popupGrid.AddChild(b1, 1, 0);
            b1.clickEvent += onPopupExit;
            Clickable.Add(b1);
            popup.SetToGrid();
#endregion












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
            registerButton.ActiveChangeable = false;
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            grid.ResizeChildren();
            SetClickables(true);
            registerButton.Active = false;
        }

        protected override void SetClickables(bool active)
        {
            foreach (var clickable in Clickable)
            {
                clickable.Active = active;
                if (popup != null)
                    if (clickable.Parent == popup.grid)
                        clickable.Active = !active;
            }
        }
        public void onPopupExit()
        {
            popup.SetActive(false);
            foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
            {
                clickable.Active = true;
            }

            Game1.self.popupToDraw = null;

        }

        public override void DataInserted()
        {
            registerButton.Active = true;
            registerButton.ActiveChangeable = true;
        }

        public override void Clean()
        {
            loginInputBox.Text = "";
            passwordInputBox.Text = "";
            passwordInputBox2.Text = "";
        }

        public override void UpdateGrid()
        {
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            SetClickables(true);
        }
        public void backClick()
        {
            Game1.self.state = Game1.State.LoginMenu;
            Game1.self.CleanLogin();
        }

        public void registerClick()
        {
            if (!passwordInputBox.Text.Equals(""))
            {
                if (passwordInputBox.Text.Equals(passwordInputBox2.Text))
                {
                    Player player = new Player(loginInputBox.Text, passwordInputBox.Text);
                    GamePacket packet = new GamePacket(OperationType.REGISTER, player);
                    Game1.self.Connection.Send(packet);
                    GamePacket packetReceiver = Game1.self.Connection.GetReceivedPacket();
                    if (packetReceiver.OperationType == OperationType.SUCCESS)
                    {
                        lbl1.Text = "Succesfully registered new user";
                    }
                    else
                    {
                        lbl1.Text = "User already exists, change login";
                    }

                    popup.SetActive(true);
                    Clean();
                    Game1.self.popupToDraw = popup;
                    SetClickables(false);
                }
                else
                {
                    lbl1.Text = "Both passwords must be the same";
                    popup.SetActive(true);
                    Clean();
                    Game1.self.popupToDraw = popup;
                    SetClickables(false);
                }
            }


        }


        public override void UpdateTooltips(IClickable button, Point xy)
        {
            foreach (var clickable in Clickable.Where(p => p.Tooltip != null).ToList())
            {
                Game1.self.tooltipToDraw = null;
            }
            if (button != null)
            {
                if (button.Tooltip != null)
                {
                    Game1.self.tooltipToDraw = button.Tooltip;
                    button.Tooltip.Update(xy);
                }
            }
        }
        public void Draw(GameTime gameTime)
        {

            grid.Draw(Game1.self.spriteBatch);

        }
    }
}
