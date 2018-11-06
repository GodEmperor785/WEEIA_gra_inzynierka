using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Card : GuiElement
    {
        private Texture2D skin;
        private Texture2D hpIcon;
        private Texture2D armorIcon;
        private double hp;
        private double armor;
        private string name;
        private RelativeLayout overlay;
        public Card()
        {
            overlay = new RelativeLayout();
            using (FileStream fileStream = new FileStream("Content/Icons/hp.png", FileMode.Open))
            {
                hpIcon = Texture2D.FromStream(Game1.self.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
            using (FileStream fileStream = new FileStream("Content/Icons/armor.jpg", FileMode.Open))
            {
                armorIcon = Texture2D.FromStream(Game1.self.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
        }
    }
}
