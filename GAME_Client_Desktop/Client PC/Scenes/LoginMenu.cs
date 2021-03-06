﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Client_PC.Utilities;
using GAME_connection;
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
        private Label lbl1;
        private Button b1;
        private bool triedReconnection;
        private int tries = 0;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Label labelLogin = new Label(new Point(0,0),100,50,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "Login"
            };
            Label labelPassword = new Label(new Point(0, 0), 115, 30, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password"
            };
            inputLogin = new InputBox(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false );
            inputLogin.TextLimit = 30;
            inputLogin.BasicText = "Login";
            inputPassword = new InputBox(new Point(0, 0), 100, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            inputPassword.TextLimit = 30;
            inputPassword.BasicText = "Password";
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

            #region Popup
            popup = new Popup(new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * 0.5),(int)(Game1.self.graphics.PreferredBackBufferHeight * 0.5)),100,400,Game1.self.GraphicsDevice,Gui);
            Grid popupGrid = new Grid();
            lbl1 = new Label(200, 200, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            b1 = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
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
            #endregion



            inputPassword.IsPassword = true;
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
            SetClickables(true);

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

        protected override void SetClickables(bool active)
        {
            foreach (var clickable in Clickable)
            {
                clickable.Active = active;
                if (clickable.Parent == popup.grid)
                    clickable.Active = !active;
            }
        }

        public override void Clean()
        {
            inputLogin.Text = "";
            inputPassword.Text = "";
        }
        public override void UpdateGrid()
        {
            triedReconnection = false;
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            SetClickables(!popup.Active);
            if (Game1.self.Connection == null)
            {
                if (tries > 10)
                {
                    lbl1.Text = "Unable to connect to server";
                    b1.clickEvent += Reconnect;
                    b1.text = "Reconnect";
                    popup.SetActive(true);
                    Clean();
                    Game1.self.popupToDraw = popup;
                    SetClickables(false);
                }
                else
                {
                    tries++;
                    ReconnectBare();
                }
            }
            else
            {

            }
        }
        public void ReconnectBare()
        {
            Game1.self.setUpConnection();
        }
        public void Reconnect()
        {
            if (!triedReconnection)
            {
                Game1.self.setUpConnection();
                onPopupExit();
                triedReconnection = true;
            }
        }
        public void Draw(GameTime gameTime)
        {
           
            grid.Draw(Game1.self.spriteBatch);
        }
        public void ExitClick()
        {
            Game1.self.Quit();
        }

        public void LoginClick()
        {
            Player player = new Player(inputLogin.Text,inputPassword.Text);
            GamePacket packet = new GamePacket(OperationType.LOGIN, player);
            Game1.self.Connection.Send(packet);

            GamePacket packetReceived = Game1.self.Connection.GetReceivedPacket();

            bool errors = false;

            if (packetReceived.OperationType == OperationType.SUCCESS)
            {
                b1.clickEvent += onPopupExit;
                lbl1.Text = "";
                b1.text = "Exit";
                packetReceived = Game1.self.Connection.GetReceivedPacket();
                if (packetReceived.OperationType == OperationType.PLAYER_DATA)
                {

                    Game1.self.player = (Player) packetReceived.Packet;

                    packetReceived = Game1.self.Connection.GetReceivedPacket();
                    if (packetReceived.OperationType == OperationType.BASE_MODIFIERS)
                    {
                        Game1.self.Modifiers = (BaseModifiers) packetReceived.Packet;

                    }
                    else
                    {
                        errors = true;
                        lbl1.Text += "\nError while getting modifiers";
                    }
                    packet = new GamePacket(OperationType.VIEW_FLEETS,null);
                    Game1.self.Connection.Send(packet);
                    packetReceived = Game1.self.Connection.GetReceivedPacket();
                    if (packetReceived.OperationType == OperationType.VIEW_FLEETS)
                    {
                        Game1.self.Decks = (List<Fleet>) packetReceived.Packet;
                    }
                    else
                    {
                        errors = true;
                        lbl1.Text += "\nError while getting fleets";
                    }
                    packet = new GamePacket(OperationType.VIEW_ALL_PLAYER_SHIPS, null);
                    Game1.self.Connection.Send(packet);
                    packetReceived = Game1.self.Connection.GetReceivedPacket();
                    if (packetReceived.OperationType == OperationType.VIEW_ALL_PLAYER_SHIPS)
                    {
                        Game1.self.OwnedShips = (List<Ship>) packetReceived.Packet;
                    }
                    else
                    {
                        errors = true;
                        lbl1.Text += "\nError while getting ships";
                    }

                    

                }
                else
                {
                    errors = true;
                }

                if (!errors)
                {
                    Game1.self.state = Game1.State.MainMenu;
                    Game1.self.LoadCardTextures();
                    Game1.self.LoginInitialize();
                    Game1.self.UpdateHistory();
                    Game1.self.UpdatePlayer();
                }
                else
                {
                    
                    popup.SetActive(true);
                    Clean();
                    Game1.self.popupToDraw = popup;
                    SetClickables(false);
                }
            }
            else
            {
                lbl1.Text = "Incorrect login or password";
                popup.SetActive(true);
                Clean();
                Game1.self.popupToDraw = popup;
                SetClickables(false);
            }
        }

        public void RegisterClick()
        {
            Game1.self.state = Game1.State.RegisterMenu;
            Game1.self.CleanRegister();
        }

        
    }
}
