using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class LootBox : GuiElement, IClickable

    {
        enum Rarity { common, uncommon, rare}
        
        private Rarity rarity;
        public bool Active { get; set; }
        public bool ActiveChangeable { get; set; }
        public Tooltip Tooltip { get; set; }
        private Vector2 Scale;

        public int Cost { get; set; }
        public LootBox(Point origin, int width, int height, GraphicsDevice device, GUI gui, string rarity, int cost) : base(origin, width,
            height, device, gui)
        {
            using (FileStream fileStream = new FileStream("Content/Lootbox/"+rarity+".png", FileMode.Open))
            {
                Texture = Texture2D.FromStream(Game1.self.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
            Scale = new Vector2( (float)width / (float)Texture.Width, (float)height / (float)Texture.Height);
            Cost = cost;
            if (rarity.Equals("common"))
                this.rarity = Rarity.common;
            else if (rarity.Equals("uncommon"))
                this.rarity = Rarity.uncommon;
            else if (rarity.Equals("rare"))
                this.rarity = Rarity.rare;


            Tooltip.Text = "Rarity:\t" + rarity + "\nCost:\t" + cost;

        }

        public override void Draw(SpriteBatch sp)
        {
            sp.Draw(Texture, Origin.ToVector2(), scale: Scale);
        }

        public Rectangle GetBoundary()
        {
            return Boundary;
        }

        public void OnClick()
        {
            throw new NotImplementedException();
        }
    }
}
