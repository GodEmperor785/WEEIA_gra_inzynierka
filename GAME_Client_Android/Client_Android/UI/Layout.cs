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
            public RelativeLayout parent;
            public int id;
            public int column;
            public int row;
            public string name;
            public int columnWidth = 1;
            public Point origin;

            public void print()
            {
                Console.WriteLine(row+"|"+column);
            }
        }

        public Layout()
        {
            Children = new List<Child>();
        }

        public int ChildrenCount
        {
            get { return Children.Count; }
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
            if (Children.SingleOrDefault(p => p.element.Id == id) == null)
                return null;
            else
                return Children.SingleOrDefault(p => p.element.Id == id).element;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var child in Children)
            {
                child.element.Draw(spriteBatch);
            }
        }

        public override void Update()
        {
            foreach (var child in Children)
            {
                child.element.Update();
            }
        }

        public void UpdateActive(bool isActive)
        {

            foreach (var child in Children)
            {
                if (child.element is IClickable)
                {
                    IClickable click = (IClickable)child.element;
                    if (click.ActiveChangeable)
                        click.Active = isActive;
                }
                else if (child.element is Layout)
                {
                    Layout z = (Layout) child.element;
                    z.UpdateActive(isActive);
                }


            }

        }
    }
}