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
        private int cardWidth = 100;
        private int cardHeight = 150;
        public override void Initialize(ContentManager Content)
        {
            layout = new RelativeLayout();
            Gui = new GUI(Content);

            int ColumnWidth = (int) ((int) Game1.self.graphics.PreferredBackBufferWidth * CardGridWidthMulti * 0.2);
            int rowHeight = (int) ((int) Game1.self.graphics.PreferredBackBufferHeight * CardGridHeightMulti);
            gridTopLeft = new Grid(5, 3, ColumnWidth, rowHeight);
            gridRight = new Grid();
            gridRightBottom = new Grid();
            gridCenter = new Grid();
            ;
            gridTopLeft.DrawBorder = true;
            gridRight.DrawBorder = true;
            gridRightBottom.DrawBorder = false;
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

            Button b = new Button(new Point(0, 0), (int) (gridRightBottom.Width * 0.5 - gridCenter.columnOffset / 2),
                (int)(gridRightBottom.Height * 0.5 - gridCenter.rowOffset / 2), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Add"
            };
            Button b2 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset / 2), (int)(gridRightBottom.Height * 0.5 - gridCenter.rowOffset / 2), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Save"
            };
            Button b3 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset / 2), (int)(gridRightBottom.Height * 0.5 - gridCenter.rowOffset / 2), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Remove"
            };
            Button b4 = new Button(new Point(0, 0), (int)(gridRightBottom.Width * 0.5 - gridCenter.columnOffset / 2), (int)(gridRightBottom.Height * 0.5 - gridCenter.rowOffset / 2), Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "Exit"
            };
            Button up = new Button(new Point(gridTopLeft.Origin.X + (int)(gridTopLeft.Width / 2), gridTopLeft.Origin.Y + gridTopLeft.Height - 10 - 20), 60, 30, Game1.self.GraphicsDevice,
            Gui, Gui.mediumFont, true)
            {
                Text = "up"
            };
            Button down = new Button(new Point(gridTopLeft.Origin.X + (int)(gridTopLeft.Width / 2), gridTopLeft.Origin.Y + gridTopLeft.Height ), 60, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "down"
            };
            
            up.Update();
            down.Update();

            up.clickEvent += UpClick;
            down.clickEvent += DownClick;

            Clickable.Add(up);
            Clickable.Add(down);

            RelativeLayout rl = new RelativeLayout();
            rl.AddChild(up);
            rl.AddChild(down);
            
            Clickable.Add(b4);
            b4.clickEvent += OnExit;
            gridRightBottom.AddChild(b,0,0);
            gridRightBottom.AddChild(b2, 0, 1);
            gridRightBottom.AddChild(b3, 1, 0);
            gridRightBottom.AddChild(b4, 1, 1);






            layout.AddChild(gridTopLeft);
            layout.AddChild(gridRight);
            layout.AddChild(gridRightBottom);
            layout.AddChild(gridCenter);
            layout.AddChild(rl);
            List<Ship> ships = new List<Ship>();
            Random rndRandom = new Random();
            for (int i = 0; i < 30; i++)
            {
                Ship ship = new Ship();
                ship.Armor = rndRandom.Next(1, 30);
                ship.Hp = rndRandom.Next(1, 30);
                ships.Add(ship);
            }

            gridTopLeft.ConstantRowsAndColumns = true;
            gridTopLeft.MaxChildren = true;
            gridTopLeft.ChildMaxAmount = 15;
            cardHeight = gridTopLeft.Height;
            Card dc = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[0]);
            Card dc1 = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[1]);
            Card dc2 = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[2]);
            Card dc3 = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[3]);
            Card dc4 = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[4]);
            Card dc5 = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[5]);
            Card dc6 = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[6]);
            Card dc7 = new Card(new Point(), cardWidth, cardHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true, ships[7]);
            gridTopLeft.AllVisible = false;
            gridTopLeft.VisibleRows = 1;
            gridTopLeft.AddChild(dc);
            gridTopLeft.AddChild(dc1);
            gridTopLeft.AddChild(dc2);
            gridTopLeft.AddChild(dc3);
            gridTopLeft.AddChild(dc4);
            gridTopLeft.AddChild(dc5);
            gridTopLeft.AddChild(dc6);
            gridTopLeft.AddChild(dc7);

            gridTopLeft.Update();

        }

        private void UpClick()
        {
            Console.WriteLine(-1);
            gridTopLeft.ChangeRow(-1);
        }

        private void DownClick()
        {
            Console.WriteLine(1);
            gridTopLeft.ChangeRow(1);
        }
        
        private void OnExit()
        {
            Game1.self.state = Game1.State.MainMenu;
        }

        public override void UpdateGrid()
        {

        }
        public void Draw(GameTime gameTime)
        {
            if (false)
            {
                Clickable.ForEach(p =>
                {
                    var z = (GuiElement) p;
                    z.Draw(Game1.self.spriteBatch);
                });
            }
            else
            {
                layout.Draw(Game1.self.spriteBatch);
            }
        }


    }
}
