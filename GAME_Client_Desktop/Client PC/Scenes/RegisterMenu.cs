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
    class RegisterMenu : Menu
    {
        private Grid grid;


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
            Label passwordLabel2 = new Label(new Point(0, 0), 100, 55, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Password again"
            };
            InputBox loginInputBox = new InputBox(new Point(0,0),100,45,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false )
            {
                TextLimit = 30
            };
            InputBox passwordInputBox = new InputBox(new Point(0, 0), 300, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false)
            {
                TextLimit = 30
            };
            InputBox passwordInputBox2 = new InputBox(new Point(0, 0), 300, 45, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false)
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

        public override void UpdateGrid()
        {
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
        }
        public void backClick()
        {
            Game1.self.state = Game1.State.LoginMenu;
        }

        public void registerClick()
        {
            Game1.self.state = Game1.State.LoginMenu;
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
