using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Client_PC.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Grid : GuiElement
    {
        public bool Active { get; set; }
        private class Child
        {
            public GuiElement element;
            public int id;
            public int column;
            public int row;
            public string name;
        }
        public Rectangle Boundary { get; set; }

        public override int  Width
        {
            get
            {
                int sum = 0;
                ColumnsSize.ForEach(p=> sum+= (int)p);
                sum += (ColumnsSize.Count - 1) * columnOffset;
                return sum;
            }
        }

        public override int Height
        {
            get
            {
                int sum = 0;
                RowsSize.ForEach(p => sum += (int)p);
                sum += (RowsSize.Count - 1) * rowOffset;
                return sum;
            }
        }
        
        public int Rows
        {
            get
            {
                if(Children.Count > 0)
                    return Children.Max(p => p.row) + 1;
                else
                {
                    return 0;
                }
            }
        }

        public int Columns
        {
            get
            {
                if (Children.Count > 0)
                    return Children.Max(p => p.column) + 1;
                else
                {
                    return 0;
                }
            }
        }

        public int columnOffset = 5;
        public int rowOffset = 5;
        private List<Child> Children;
        private List<float> ColumnsSize;
        private List<float> RowsSize;

        private float ColumnOffset(int numberOfColumn)
        {
            float sum = 0.0f;
            int i = 0;
            while (i < numberOfColumn)
            {
                sum += ColumnsSize[i];
                sum += columnOffset;
                i++;
            }
            return sum;
        }
        private float RowOffset(int numberOfRow)
        {
            float sum = 0.0f;
            int i = 0;
            while (i < numberOfRow)
            {
                sum += RowsSize[i];
                sum += rowOffset;
                i++;
            }
            return sum;
        }
        public Grid()
        {
            Children = new List<Child>();
            ColumnsSize = new List<float>();
            RowsSize = new List<float>();
        }

        public void AddChild(GuiElement element, int row,int column)
        {
            Child ch = new Child
            {
                element = element,
                column = column,
                row = row
            };
            ch.id = Children.Count;
            Children.Add(ch);
            Update();
            UpdateChildren();
        }

        public void AddChild(GuiElement element, int row, int column, string name)
        {
            Child ch = new Child
            {
                element = element,
                column = column,
                row = row
            };
            ch.name = name;
            ch.id = Children.Count;
            Children.Add(ch);
            Update();
            UpdateChildren();
        }
        public void AddChild(GuiElement element, string name)
        {
            Child ch = new Child()
            {
                element = element,
                name = name,
                column =  0
            };
            ch.id = Children.Count;
            ch.row = Rows;
            Children.Add(ch);
            Update();
            UpdateChildren();
        }
        public void UpdateP()
        {
            Update();
            UpdateChildren();
            UpdateActive(Active);
        }
        private void Update()
        {
            float[] ColumnsSiz = new float[Columns];
            float[] RowsSiz = new float[Rows];
            for(int i = 0; i < Rows; i++)
            {
                List<GuiElement> elements = new List<GuiElement>();
                Children.Where(p=> p.row == i).ToList().ForEach(p=> elements.Add(p.element));
                int maxHeight = 0;
                if (elements.Count > 0)
                {
                    maxHeight = elements.Max(p => p.Height);
                }
                RowsSiz[i] = (maxHeight);
            }
            for (int i = 0; i < Columns; i++)
            {
                List<GuiElement> elements = new List<GuiElement>();
                Children.Where(p => p.column == i).ToList().ForEach(p => elements.Add(p.element));
                int maxWidth = 0;
                if (elements.Count > 0)
                {
                    maxWidth = elements.Max(p => p.Width);
                }
                ColumnsSiz[i] = (maxWidth);
            }

            RowsSize = RowsSiz.ToList();
            ColumnsSize = ColumnsSiz.ToList();
        }

        public void UpdateActive(bool isActive)
        {

            foreach (var child in Children)
            {
                if (child.element is IClickable)
                {
                    IClickable click = (IClickable) child.element;
                    if(click.ActiveChangeable)
                        click.Active = isActive;
                }

                
            }

        }
        private void UpdateChildren()
        {
            foreach (var child in Children)
            {
                child.element.Origin = new Point(this.Origin.X + (int)ColumnOffset(child.column),this.Origin.Y + (int)RowOffset(child.row));
                child.element.Update();
                if (child.element is Grid)
                {
                    Grid g = (Grid) child.element;
                    g.UpdateP();
                }
            }
        }
        public void ResizeChildren()
        {
            for (int i = 0; i< Rows;i++)
            {
                var elements = Children.Where(p => p.row == i).ToList();
                foreach (var element in elements)
                {
                    element.element.Width = (int)ColumnsSize[element.column];
                    element.element.Height = (int) RowsSize[element.row];
                }
            }
        }
        public void ResizeChildren(int width,int height)
        {
            foreach (var child in Children)
            {
                child.element.Width = width;
                child.element.Height = height;
            }
        }
        public GuiElement GetChild(int row, int column)
        {
            return Children.Single(p => p.column == column && p.row == row).element;
        }
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
