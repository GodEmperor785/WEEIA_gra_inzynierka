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
        public virtual void Draw(SpriteBatch spriteBatch) { }
        public virtual void Update() { }
    }
}
