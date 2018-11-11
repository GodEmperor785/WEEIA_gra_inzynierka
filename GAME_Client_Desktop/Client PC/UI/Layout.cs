using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Layout : GuiElement
    {
        protected Texture2D Background;
        protected class Child
        {
            public GuiElement element;
            public int id;
            public int column;
            public int row;
            public string name;
            public int columnWidth = 1;
            public Point origin;
        }

        public Layout()
        {
            Children = new List<Child>();
        }
        protected List<Child> Children;
        public bool DrawBorder { get; set; }
        public int BorderSize { get; set; }
        public GuiElement GetChild(string name)
        {
            return Children.SingleOrDefault(p => p.name.Equals(name)).element;
        }
        public GuiElement GetChild(int id)
        {
            return Children.SingleOrDefault(p => p.element.Id == id).element;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var child in Children)
            {
                if (child.name != null && child.name.Equals("gridW"))
                {
                    int z = 0;
                }
                child.element.Draw(spriteBatch);
            }
        }
    }
}