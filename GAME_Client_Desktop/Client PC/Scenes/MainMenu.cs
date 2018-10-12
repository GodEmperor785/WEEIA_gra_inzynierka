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
    class MainMenu
    {
        private GUI Gui;
        private Grid grid;
        private List<IClickable> Clickable;
        public MainMenu()
        {
            Clickable = new List<IClickable>();
        }

        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Button z = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont)
            {
                Text = "z1 button"
            };
            Button z2 = new Button(new Point(100, 200), 70, 125, Game1.self.GraphicsDevice, Gui, Gui.bigFont)
            {
                Text = "Settings"
            };
            Button z3 = new Button(new Point(100, 200), 200, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont)
            {
                Text = "Exit"
            };
            Clickable.Add(z);
            Clickable.Add(z2);
            Clickable.Add(z3);
            grid = new Grid();
            grid.AddChild(z, 0, 0);
            grid.AddChild(z2, 1, 0);
            grid.AddChild(z3,2,0);
            z3.clickEvent += ExitClick;
            z2.clickEvent += GoToSettings;
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f),(int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
        }

        public void ExitClick()
        {
            Game1.self.Exit();
        }

        public void GoToSettings()
        {
            Game1.self.state = Game1.State.OptionsMenu;
        }
        public void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                int x = mouseState.X;
                int y = mouseState.Y;
                Point xy = new Point(x,y);
                IClickable button = Clickable.SingleOrDefault(p=> p.GetBoundary().Contains(xy));
                if(button != null)
                    button.OnClick();
            }
        }
        public void Draw(GameTime gameTime)
        {
            Game1.self.GraphicsDevice.Clear(Color.AntiqueWhite);
            Game1.self.spriteBatch.Begin();
            grid.Draw(Game1.self.spriteBatch);
            Game1.self.spriteBatch.End();
        }
    }
}
