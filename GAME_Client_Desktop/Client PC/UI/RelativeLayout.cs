using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Client_PC.UI
{
    class RelativeLayout : Layout
    {
        public void AddChild(GuiElement element)
        {
            Child ch = new Child();
            ch.element = element;
            ch.id = Children.Count;
            ch.parent = this;
            ch.element.Parent = this;
            Children.Add(ch);
        }
        public void AddChild(GuiElement element, string name)
        {
            Child ch = new Child();
            ch.element = element;
            ch.id = Children.Count;
            ch.name = name;
            Children.Add(ch);
        }
    }
}
