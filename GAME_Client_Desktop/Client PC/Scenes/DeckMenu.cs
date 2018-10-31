using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class DeckMenu : Menu
    {
        private RelativeLayout layout;

        public void Initialize(ContentManager Content)
        {
            layout = new RelativeLayout();
            Gui = new GUI(Content);
            Grid grid = new Grid();
            grid.DrawBorder = true;
            grid.BorderSize = 3;
            grid.WitdhAndHeightColumnDependant = false;
            grid.Width =(int) ((int)Game1.self.graphics.PreferredBackBufferWidth * 0.55);
            grid.Height = (int) ((int)Game1.self.graphics.PreferredBackBufferHeight * 0.10);
            layout.AddChild(grid,new Point(10,10));
        }
        public void Draw(GameTime gameTime)
        {
            Game1.self.GraphicsDevice.Clear(Color.BlueViolet);
            layout.Draw(Game1.self.spriteBatch);
        }


    }
}
