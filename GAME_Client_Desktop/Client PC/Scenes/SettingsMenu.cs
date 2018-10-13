using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class SettingsMenu
    {
        private List<IClickable> Clickable;
        private Grid grid;
        private GUI Gui;
        private int i = 0;
        public SettingsMenu()
        {
            Clickable = new List<IClickable>();
        }

        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            grid = new Grid();
            Label label1 = new Label(new Point(0, 0), 100, 25, Game1.self.GraphicsDevice, Gui, Gui.mediumFont)
            {
                Text = "Resolution"
            };
            Dropdown drop = new Dropdown(new Point(0,0),100,30,Game1.self.GraphicsDevice, Gui);
            Button button = new Button(new Point(0, 0), 100, 35, Game1.self.GraphicsDevice, Gui, Gui.bigFont)
            {
                Text = "Back"
            };
            Button dropElement1 = new Button(new Point(0, 0), 100, 30, Game1.self.GraphicsDevice, Gui, Gui.mediumFont)
            {
                Text = "1920 x 1080"
            };
            drop.Add(dropElement1,"fullHd", drop);
            Button dropElement2 = new Button(new Point(0, 0), 100, 30, Game1.self.GraphicsDevice, Gui, Gui.mediumFont)
            {
                Text = "1280 x 720"
            };
            drop.Add(dropElement2, "Hd", drop);
            button.clickEvent += OnExit;
            Clickable.Add(drop);
            Clickable.Add(button);
            Clickable.Add(dropElement1);
            Clickable.Add(dropElement2);
            button.Active = true;
            grid.AddChild(label1,0,0);
            grid.AddChild(drop, 1, 0);
            grid.AddChild(button,2,0);
            grid.Active = true;
            grid.UpdateActive(true);
            int z = 243123;
        }

        public void OnExit()
        {
            Game1.self.state = Game1.State.MainMenu;
        }
        public void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                int x = mouseState.X;
                int y = mouseState.Y;
                i++;
                bool dropClicked = false;
                Point xy = new Point(x, y);
                IClickable button = Clickable.Where(p=> p.Active).SingleOrDefault(p => p.GetBoundary().Contains(xy));
                if (button != null)
                    if (button is Dropdown)
                    {
                        Dropdown d = (Dropdown) button;
                        Debug.WriteLine(d.Active + "|"+d.ActiveChangeable +"\t"+  d.ShowChildren + "\t" + i);
                        if (d.ShowChildren)
                        {
                            button.OnClick();
                        }
                        else
                        {
                            d.ShowChildren = true;
                            d.Update();
                            dropClicked = true;
                            button.Active = false;
                        }
                    }
                    else if (button.Parent is Dropdown)
                    {
                        dropClicked = true;
                    }
                    else
                    {
                        button.OnClick();
                    }

                if (!dropClicked)
                {
                    Dropdown drop = (Dropdown)Clickable.SingleOrDefault(p => p is Dropdown);
                    drop.ShowChildren = false;
                    drop.Update();
                    drop.Active = true;
                }
            }
            grid.UpdateP();
        }
        public void Draw(GameTime gameTime)
        {
            Game1.self.GraphicsDevice.Clear(Color.RosyBrown);
            Game1.self.spriteBatch.Begin();
            grid.Draw(Game1.self.spriteBatch);
            Game1.self.spriteBatch.End();
        }
    }
}
