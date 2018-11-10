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
            double cardWidthPercentage = 0.25;
            double cardHeightPercentage = 0.18;
            double widthPercentage = 0.2;
            double heightPercentage = 0.75;
            int ColumnWidth = (int) (Game1.self.graphics.PreferredBackBufferWidth * widthPercentage * cardWidthPercentage);
            int RowHeight = (int) (Game1.self.graphics.PreferredBackBufferHeight * heightPercentage * cardHeightPercentage);
            enemyGrid = new Grid(3,5, ColumnWidth, RowHeight);
            yourGrid = new Grid(3,5, ColumnWidth, RowHeight);
            yourCards = new Grid();

            

            


            enemyGrid.WitdhAndHeightColumnDependant = false;
            yourGrid.WitdhAndHeightColumnDependant = false;
            yourCards.WitdhAndHeightColumnDependant = false;

            enemyGrid.Width = (int) (Game1.self.graphics.PreferredBackBufferWidth * widthPercentage);
            yourGrid.Width = (int)(Game1.self.graphics.PreferredBackBufferWidth * widthPercentage);

            enemyGrid.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * heightPercentage);
            yourGrid.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * heightPercentage);
            yourCards.Height = (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.15);

            enemyGrid.DrawBorder = true;
            yourGrid.DrawBorder = true;
            yourCards.DrawBorder = true;

            enemyGrid.BorderSize = 3;
            yourGrid.BorderSize = 3;
            yourCards.BorderSize = 3;

            enemyGrid.rowOffset = (int) (enemyGrid.Height * 0.025);
            yourGrid.rowOffset = (int)(enemyGrid.Height * 0.025);

            enemyGrid.columnOffset =(int) (enemyGrid.Width * 0.125);
            yourGrid.columnOffset = (int) (yourGrid.Width * 0.125);


            yourCards.Width = (int)(enemyGrid.Width + yourGrid.Width + (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.4));



            yourGrid.Origin = new Point(10,10);
            enemyGrid.Origin = new Point(10 + yourGrid.Width + (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.4), 10 );

            yourCards.Origin = new Point(10,yourGrid.Origin.Y + yourGrid.Height + (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.02));

            int cardWidth = (int) (yourGrid.Width * cardWidthPercentage);
            int cardHeight = (int) (yourGrid.Height * cardHeightPercentage);

            Button b1 = new Button(new Point(0,0),cardWidth,cardHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true );
            Button b2 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b3 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b4 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b5 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b6 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b7 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b8 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b9 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b10 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b11 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b12 = new Button(new Point(0, 0), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);

            yourGrid.AddChild(b1,0,0);
            yourGrid.AddChild(b2, 1, 2);
            yourGrid.AddChild(b3, 2, 2);
            yourGrid.AddChild(b4, 0, 1);
            yourGrid.AddChild(b5, 0, 2);
            yourGrid.AddChild(b6, 3, 0);

            enemyGrid.AddChild(b7, 4, 2);
            enemyGrid.AddChild(b8, 0, 1);
            enemyGrid.AddChild(b9, 0, 2);
            enemyGrid.AddChild(b10, 2, 2);
            enemyGrid.AddChild(b11, 1, 2);
            enemyGrid.AddChild(b12, 3, 2);

            enemyGrid.ResizeChildren();
            yourGrid.ResizeChildren();
            layout.AddChild(enemyGrid);
            layout.AddChild(yourGrid);
            layout.AddChild(yourCards);





        }
        public void Draw(GameTime gameTime)
        {
            layout.Draw(Game1.self.spriteBatch);
        }

    }
}
