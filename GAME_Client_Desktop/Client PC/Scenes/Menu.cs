using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class Menu
    {
        protected List<IClickable> Clickable;
        protected GUI Gui;
        protected bool AbleToClick = false;
        protected Keys[] LastPressedKeys;
        private ButtonState lastState;
        public Menu()
        {
            Clickable = new List<IClickable>();
        }

        public virtual void Update(GameTime gameTime)
        {
            Game1.self.DeltaSeconds += gameTime.ElapsedGameTime.Milliseconds;
            Game1.self.AbleToClick = Game1.self.DeltaSeconds > Constants.clickDelay;
            var mouseState = Mouse.GetState();
            
            var keyboardState = Keyboard.GetState();
            Utils.UpdateKeyboard(keyboardState, ref LastPressedKeys);
            UpdateGrid();
            if(lastState != mouseState.LeftButton)
                CheckClickables(mouseState);
            CheckTooltips(mouseState);

            lastState = mouseState.LeftButton;
            UpdateGrid();
        }

        protected virtual void SetClickables(bool active)
        {
            foreach (var clickable in Clickable)
            {
                if (!(clickable is Card))
                    clickable.Active = active;
            }

        }

        public virtual void Clean()
        {

        }
        private void CheckClickables(MouseState mouseState)
        {
            int x = mouseState.X;
            int y = mouseState.Y;
            Point xy = new Point(x, y);
            IClickable button = GetClickable(xy);
            if (mouseState.LeftButton == ButtonState.Pressed && Game1.self.AbleToClick)
            {
                UpdateFields();
                Game1.self.DeltaSeconds = 0;
                Game1.self.AbleToClick = false;

                if (button != null)
                {
                    UpdateClick(button);
                    UpdateButtonNotNull();
                }
                else
                {
                    Game1.self.FocusedElement = null;
                    UpdateButtonNull();
                }

                UpdateClickables();
            }
        }

        public virtual void UpdateButtonNotNull()
        {

        }

        public virtual void UpdateButtonNull()
        {

        }

        public virtual void UpdateClickables()
        {

        }
        private void CheckTooltips(MouseState mouseState)
        {
            int x = mouseState.X;
            int y = mouseState.Y;
            Point xy = new Point(x, y);
            IClickable button = GetClickable(xy);
            UpdateTooltips(button, xy);
        }
        public virtual void Initialize(ContentManager Content)
        {

        }
        public void Reinitialize(ContentManager Content)
        {
            Clickable.Clear();
            Initialize(Content);
        }
        public virtual void UpdateGrid()
        {

        }

        public virtual void UpdateTooltips(IClickable button, Point xy)
        {

        }
        public virtual IClickable GetClickable(Point xy)
        {
            
            IClickable click = Clickable.Where(p=> p.Active).FirstOrDefault(p => p.GetBoundary().Contains(xy));
           
            return click;
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
