using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class AccountMenu : Menu
    {
        private RelativeLayout layout;
        private InputBox newPasswordInputBox;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            layout = new RelativeLayout();
            int buttonWidth = 100;
            int buttonHeight = 50;
            Button exitButton = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                text = "Exit"
            };
            exitButton.clickEvent += onExit;
            Clickable.Add(exitButton);


            
        }

        private void onExit()
        {
            Game1.self.state = Game1.State.MainMenu;
        }

        
    }
}
