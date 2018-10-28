using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class Menu
    {
        protected List<IClickable> Clickable;
        protected GUI Gui;
        protected bool AbleToClick = false;
        protected Keys[] LastPressedKeys;
        public Menu()
        {
            Clickable = new List<IClickable>();
        }

        public virtual void Update(GameTime gameTime)
        {
            Game1.self.DeltaSeconds += gameTime.ElapsedGameTime.Milliseconds;
            if (Game1.self.DeltaSeconds > 250)
            {
                Game1.self.AbleToClick = true;
            }
            else
            {
                Game1.self.AbleToClick = false;
            }
            var mouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();
            Utils.UpdateKeyboard(keyboardState, ref LastPressedKeys);
            UpdateGrid();
            int x = mouseState.X;
            int y = mouseState.Y;
            Point xy = new Point(x, y);
            IClickable button = GetClickable(xy);
            UpdateTooltips(button,xy);
            if (mouseState.LeftButton == ButtonState.Pressed && Game1.self.AbleToClick)
            {
                UpdateFields();
                Game1.self.DeltaSeconds = 0;
                Game1.self.AbleToClick = false;
                
                if (button != null)
                {
                    UpdateClick(button);
                }
                else
                {
                    Game1.self.FocusedElement = null;
                }
            }

            UpdateGrid();
        }

        public virtual void UpdateGrid()
        {

        }

        public virtual void UpdateTooltips(IClickable button, Point xy)
        {

        }
        public virtual IClickable GetClickable(Point xy)
        {
            return Clickable.SingleOrDefault(p => p.GetBoundary().Contains(xy));
        }
        public virtual void UpdateClick(IClickable button)
        {
            Game1.self.FocusedElement = button;
            button.OnClick();
        }

        public virtual void UpdateFields()
        {

        }
    }
}
