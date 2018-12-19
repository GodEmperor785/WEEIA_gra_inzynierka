using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class ShopMenu : Menu
    {
        private Grid grid;
        private Popup popup;

        private Grid BoxesGrid;
        private LootBoxElement common;
        private LootBoxElement uncommon;
        private LootBoxElement rare;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);

            BoxesGrid.DrawBackground = false;

            Button exitButton = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "Back"
            };
            
        }

        public override void Clean()
        {
            BoxesGrid.RemoveChildren();
            
        }

        public void Reinitialize(List<LootBox> loots)
        {
            int cost = 0;
            common = new LootBoxElement(200, 200, Game1.self.GraphicsDevice, Gui, "common", commonCost);
            uncommon = new LootBoxElement(200, 200, Game1.self.GraphicsDevice, Gui, "uncommon", uncommonCost);
            rare = new LootBoxElement(200, 200, Game1.self.GraphicsDevice, Gui, "rare", rareCost);
        }
    }
}
