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
        private Grid gridTopLeft;
        private Grid gridRight;
        private Grid gridRightBottom;
        private Grid gridCenter;

        private double CardGridHeightMulti = 0.15;
        private double CardGridWidthMulti = 0.75;
        private double RightGridHeightMulti = 0.75;
        public void Initialize(ContentManager Content)
        {
            layout = new RelativeLayout();
            Gui = new GUI(Content);
            gridTopLeft = new Grid();
            gridRight = new Grid();
            gridRightBottom = new Grid();
            gridCenter = new Grid();
            ;
            gridTopLeft.DrawBorder = true;
            gridRight.DrawBorder = true;
            gridRightBottom.DrawBorder = true;
            gridCenter.DrawBorder = true;

            gridTopLeft.BorderSize = 3;
            gridRight.BorderSize = 3;
            gridRightBottom.BorderSize = 3;
            gridCenter.BorderSize = 3;

            gridTopLeft.WitdhAndHeightColumnDependant = false;
            gridRight.WitdhAndHeightColumnDependant = false;
            gridRightBottom.WitdhAndHeightColumnDependant = false;
            gridCenter.WitdhAndHeightColumnDependant = false;

            gridTopLeft.Width =(int) ((int)Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti);
            gridRight.Width = (int) ((int) Game1.self.graphics.PreferredBackBufferWidth * (1 - CardGridWidthMulti) - 30);
            gridRightBottom.Width = gridRight.Width;
            gridCenter.Width = gridTopLeft.Width;

            gridTopLeft.Height = (int) ((int)Game1.self.graphics.PreferredBackBufferHeight * CardGridHeightMulti);
            gridRight.Height = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * RightGridHeightMulti);
            gridRightBottom.Height = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * (1 - RightGridHeightMulti) - 30);
            gridCenter.Height = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * (1 - CardGridHeightMulti) - 30);


            gridTopLeft.Origin = new Point(10, 10);
            gridRight.Origin = new Point((int) (Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti + 20),10);
            gridRightBottom.Origin = new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti + 20),
                                    (int)(Game1.self.graphics.PreferredBackBufferHeight * RightGridHeightMulti + 20));
            gridCenter.Origin = new Point(10, (int)(Game1.self.graphics.PreferredBackBufferHeight - gridCenter.Height - 10));

            layout.AddChild(gridTopLeft);
            layout.AddChild(gridRight);
            layout.AddChild(gridRightBottom);
            layout.AddChild(gridCenter);
        }

        public override void UpdateGrid()
        {

        }
        public void Draw(GameTime gameTime)
        {
            Game1.self.GraphicsDevice.Clear(Color.BlueViolet);
            layout.Draw(Game1.self.spriteBatch);
        }


    }
}
