using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Grid
    {
        private class Child
        {
            public GuiElement element;
            public int column;
            public int row;
        }
        public Rectangle Boundary { get; set; }

        public int Width
        {
            get
            {
                int sum = 0;
                ColumnsSize.ForEach(p=> sum+= (int)p);
                sum += (ColumnsSize.Count - 1) * columnOffset;
                return sum;
            }
        }

        public int Height
        {
            get
            {
                int sum = 0;
                RowsSize.ForEach(p => sum += (int)p);
                sum += (RowsSize.Count - 1) * rowOffset;
                return sum;
            }
        }

        public Point Origin { get; set; }

        public int Rows
        {
            get { return Children.Max(p => p.row) + 1; }
        }

        public int Columns
        {
            get { return Children.Max(p => p.column) + 1; }
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
            Children.Add(ch);
            Update();
            UpdateChildren();
        }

        public void UpdateP()
        {
            Update();
            UpdateChildren();
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

        private void UpdateChildren()
        {
            foreach (var child in Children)
            {
                child.element.Origin = new Point(this.Origin.X + (int)ColumnOffset(child.column),this.Origin.Y + (int)RowOffset(child.row));
                child.element.Update();
            }
        }
        private void ResizeChildren()
        {
            List<GuiElement> elements = new List<GuiElement>();
            Children.ForEach(p => elements.Add(p.element));
            int maxHeight = elements.Max(p => p.Height);
            int maxWidth = elements.Max(p => p.Width);
            foreach (var child in Children)
            {
                child.element.Width = maxWidth;
                child.element.Height = maxHeight;
            }
        }
        public GuiElement GetChild(int row, int column)
        {
            return Children.Single(p => p.column == column && p.row == row).element;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var child in Children)
            {
                child.element.Draw(spriteBatch);
            }
        }
    }
}
