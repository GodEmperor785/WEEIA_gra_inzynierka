using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class ShopMenu : Menu
    {
        private Grid grid;
        private Popup popup;

        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);

            Button exitButton = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "Back"
            };
        }
    }
}
