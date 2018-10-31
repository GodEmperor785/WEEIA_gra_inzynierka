using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC;
using Client_PC.UI;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class MainMenu : Menu
    {
        private Grid grid;


        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Button z = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Deck"
            };
            Button z2 = new Button(new Point(100, 200), 70, 125, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Settings"
            };
            Button z3 = new Button(new Point(100, 200), 200, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Exit"
            };
            InputBox inputBox = new InputBox(new Point(0,0),100,100,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,false );
            inputBox.TextLimit = 30;
            Clickable.Add(inputBox);
            Clickable.Add(z);
            Clickable.Add(z2);
            Clickable.Add(z3);
            grid = new Grid();
            grid.AddChild(z, 0, 0);
            grid.AddChild(z2, 1, 0);
            grid.AddChild(z3,2,0);
            grid.AddChild(inputBox,4,0);
            z3.clickEvent += ExitClick;
            z2.clickEvent += GoToSettings;
            z.clickEvent += GoToDeck;
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f),(int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            grid.ResizeChildren();
        }

        public void GoToDeck()
        {
            Game1.self.state = Game1.State.DeckMenu;
        }
        public void ExitClick()
        {
            Game1.self.Exit();
        }

        public void GoToSettings()
        {
            Game1.self.state = Game1.State.OptionsMenu;
        }
       

        public override void UpdateGrid()
        {
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
        }
        public void Draw(GameTime gameTime)
        {
            Game1.self.GraphicsDevice.Clear(Color.AntiqueWhite);
           // Game1.self.spriteBatch.Begin();
            grid.Draw(Game1.self.spriteBatch);
            //Game1.self.spriteBatch.End();
        }
    }
}
