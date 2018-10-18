using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private int OriginalHeight;
        private List<Child> Children;
        private int IdSelected;
        private Grid grid;
        public bool ShowChildren = false;
        public int Id { get; set; }
        public bool Active { get; set; }
        public object Parent { get; set; }
        public bool ActiveChangeable { get; set; }

        public Dropdown(Point origin, int width, int height, GraphicsDevice device, GUI gui) : base(origin,width,height,device,gui)
        {
            Children = new List<Child>();
            grid = new Grid();
            grid.Origin = origin;
            OriginalHeight = height;
            Active = true;
            ActiveChangeable = false;
            IdSelected = -1;
        }
        public override void Update()
        {
            grid.Origin = Origin;
            grid.UpdateP();
            if (ShowChildren)
            {
                grid.Active = true;
                Height = grid.Height;
            }
            else
            {
                grid.Active = false;
                Height = OriginalHeight;
            }
        }

        public void ResizeChildren()
        {
            grid.ResizeChildren(Width,Height);
        }
        public void Add(GuiElement element, string name, Object parent)
        {
            if (element is Button)
            {
                Button b = (Button) element;
                b.Parent = parent;
                b.clickEventInt += ChildClicked;
                grid.AddChild(b, name);
            }
            else
            {
                grid.AddChild(element, name);
            }
            
        }

        public void ChildClicked(int n)
        {
            IdSelected = n;
            Debug.WriteLine("BUtton id:\t"+ n);
            ShowChildren = false;
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
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Util.CreateTexture(Device, Width, Height, pixel => Color.Black), Boundary,
                    Color.White);
                if (IdSelected != -1)
                {
                    GuiElement el = grid.GetChild(IdSelected);
                    if (el is Button)
                    {
                        Button b = (Button) el;
                        Vector2 z = b.Font.MeasureString(b.Text);
                        Vector2 TextPosition = new Vector2(((Origin.X + Width / 2.0f)) - z.X / 2.0f,
                            (Origin.Y + Height / 2.0f) - z.Y / 2.0f);
                        spriteBatch.DrawString(b.Font, b.Text, TextPosition, Color.Black);
                    }
                    
                }
                spriteBatch.End();
            }
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
