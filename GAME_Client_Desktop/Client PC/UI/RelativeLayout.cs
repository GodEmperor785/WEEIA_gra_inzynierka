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
        public void AddChild(GuiElement element, Point origin)
        {
            Child ch = new Child();
            ch.element = element;
            ch.id = Children.Count;
            ch.origin = origin;
            Children.Add(ch);
        }
    }
}
