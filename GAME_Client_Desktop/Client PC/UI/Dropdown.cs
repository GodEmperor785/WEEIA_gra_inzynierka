using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Client_PC.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Dropdown : GuiElement, IClickable
    {
        public delegate void ElementClicked();

        public ElementClicked OnIndexSelected;
        private class Child
        {
            public GuiElement element;
            public int id;
            public string name;
        }

        private List<Child> Children;
        private int IdSelected;
        private Grid grid;
        public bool ShowChildren = false;
        public int Id { get; set; }

        public Dropdown(Point origin, int width, int height, GraphicsDevice device, GUI gui) : base(origin,width,height,device,gui)
        {
            Children = new List<Child>();
            grid = new Grid();
            grid.Origin = origin;
        }
        public override void Update()
        {
            grid.Origin = Origin;
            grid.UpdateP();
        }

        public void Add(GuiElement element, string name)
        {
            grid.AddChild(element, name);
        }

        public void ChildClicked(int n)
        {
            IdSelected = n;
        }



        public GuiElement GetChild(string name)
        {
            return grid.GetChild(name);
        }

        public GuiElement GetSelected()
        {
            return grid.GetChild(IdSelected);
        }

        public string GetSelectedName()
        {
            return Children.SingleOrDefault(p => p.id == IdSelected).name;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (ShowChildren)
            {
                grid.Draw(spriteBatch);
            }
            else
                spriteBatch.Draw(Util.CreateTexture(Device, Width, Height, pixel => Color.Black), Boundary, Color.White);
        }


        public Rectangle GetBoundary()
        {
            return Boundary;
        }

        public void OnClick()
        {
            
        }
    }
}
