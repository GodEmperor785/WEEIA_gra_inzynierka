using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class MainMenu : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;
        private GUI Gui;
        private Grid grid;
        private List<IClickable> Clickable;
        public MainMenu(GraphicsDeviceManager gr, SpriteBatch sp, GraphicsDevice g)
        {
            graphics = gr;
            IsMouseVisible = true;
            spriteBatch = new SpriteBatch(g);
            graphicsDevice = g;
            Clickable = new List<IClickable>();
        }

        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Button z = new Button(new Point(100, 200), 200, 100, graphicsDevice, Gui)
            {
                Text = "z1 button"
            };
            Button z2 = new Button(new Point(100, 200), 200, 100, graphicsDevice, Gui)
            {
                Text = "z2 button"
            };
            Button z3 = new Button(new Point(100, 200), 200, 200, graphicsDevice, Gui)
            {
                Text = "z3 button"
            };
            Clickable.Add(z);
            Clickable.Add(z2);
            Clickable.Add(z3);
            grid = new Grid();
            grid.AddChild(z, 0, 0);
            grid.AddChild(z2, 1, 0);
            grid.AddChild(z3,2,0);
            z3.clickEvent += ExitClick;
            grid.Origin = new Point((int)(graphics.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f),(int)(graphics.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
        }

        protected override void LoadContent()
        {

        }

        private void ExitClick()
        {
            Exit();
        }
        public void UpdateP(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            Console.WriteLine(mouseState.Position);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                int x = mouseState.X;
                int y = mouseState.Y;
                Point xy = new Point(x,y);
                Clickable.Single(p=> p.GetBoundary().Contains(xy)).OnClick();
            }
        }

        public void DrawP(GameTime gameTime)
        {
            Draw(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.AntiqueWhite);
            spriteBatch.Begin();
            grid.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
