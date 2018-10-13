using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class GuiElement
    {
        public Rectangle Boundary => new Rectangle(Origin.X, Origin.Y, Width, Height);
        public int Width { get; set; }
        public int Height { get; set; }
        public Point Origin { get; set; }
        protected GUI Gui { get; set; }
        protected GraphicsDevice Device { get; set; }
        
        public virtual void Update() { }
        public virtual void Draw(SpriteBatch sp) { }
        public GuiElement(Point origin, int width, int height, GraphicsDevice device, GUI gui)
        {
            Origin = origin;
            Width = width;
            Height = height;
            Device = device;
            Gui = gui;
        }
    }
}
