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
    class GameWindow : Menu
    {
        private RelativeLayout layout;
        private Grid enemyGrid;
        private Grid yourGrid;
        private Grid yourCards;
        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            layout = new RelativeLayout();
            enemyGrid = new Grid();
            yourGrid = new Grid();
            yourCards = new Grid();

            enemyGrid.WitdhAndHeightColumnDependant = false;
            yourGrid.WitdhAndHeightColumnDependant = false;
            yourCards.WitdhAndHeightColumnDependant = false;

            enemyGrid.Width = (int) (Game1.self.graphics.PreferredBackBufferWidth * 0.85);
            yourGrid.Width = (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.85);
            yourCards.Width = (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.85);

            enemyGrid.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.35);
            yourGrid.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.35);
            yourCards.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.15);

            enemyGrid.DrawBorder = true;
            yourGrid.DrawBorder = true;
            yourCards.DrawBorder = true;

            enemyGrid.BorderSize = 3;
            yourGrid.BorderSize = 3;
            yourCards.BorderSize = 3;

            enemyGrid.rowOffset = 20;
            yourGrid.rowOffset = 20;

            Button b1 = new Button(new Point(), 50, (int)((enemyGrid.Height - 2 * enemyGrid.rowOffset) / 3f) , Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            Button b2 = new Button(new Point(), 50, (int)((enemyGrid.Height - 2 * enemyGrid.rowOffset) / 3f), Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            Button b3 = new Button(new Point(), 50, (int)((enemyGrid.Height - 2 * enemyGrid.rowOffset) / 3f), Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            Button b4 = new Button(new Point(), 50, 50, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            Button b5 = new Button(new Point(), 50, 50, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);
            Button b6 = new Button(new Point(), 50, 50, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, false);

            enemyGrid.AddChild(b1,2,0);
            enemyGrid.AddChild(b2, 1, 0);
            enemyGrid.AddChild(b3, 0, 0);

            yourGrid.AddChild(b4, 0, 0);
            yourGrid.AddChild(b5, 0, 0);
            yourGrid.AddChild(b6, 0, 0);

            enemyGrid.Origin = new Point(10,10);
            yourGrid.Origin = new Point(10,10 + enemyGrid.Height + (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.02));
            yourCards.Origin = new Point(10,yourGrid.Origin.Y + yourGrid.Height + (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.02));

            enemyGrid.ResizeChildren();
            yourGrid.ResizeChildren();
            layout.AddChild(enemyGrid);
            layout.AddChild(yourGrid);
            layout.AddChild(yourCards);





        }
        public void Draw(GameTime gameTime)
        {
            Game1.self.GraphicsDevice.Clear(Color.BlueViolet);
            layout.Draw(Game1.self.spriteBatch);
        }

    }
}
