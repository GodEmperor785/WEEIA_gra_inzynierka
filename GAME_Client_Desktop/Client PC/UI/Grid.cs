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
    class Grid : Layout 
    {
        #region fields
        public bool Active { get; set; }
        public bool WitdhAndHeightColumnDependant { get; set; }
        public bool ConstantRowsAndColumns { get; set; }
        private int width;
        public int ChildMaxAmount;
        public bool MaxChildren =  false;
        public bool AllVisible = true;
        public int VisibleRows;
        private int ShowedRow = 0;
        public override int  Width
        {
            get
            {
                if (WitdhAndHeightColumnDependant)
                {
                    int sum = 0;
                    ColumnsSize.ForEach(p => sum += (int) p);
                    sum += (ColumnsSize.Count - 1) * columnOffset;
                    return sum;
                }
                else
                    return width;

            }
            set { width = value; }
        }

        private int height;
        public override int Height
        {
            get
            {
                if (WitdhAndHeightColumnDependant)
                {
                    int sum = 0;
                    RowsSize.ForEach(p => sum += (int) p);
                    sum += (RowsSize.Count - 1) * rowOffset;
                    return sum;
                }
                else return height;

            }
            set { height = value; }
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
        
        private List<float> ColumnsSize;
        private List<float> RowsSize;

        #endregion





        #region methods
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
        public Grid() : base()
        {
            WitdhAndHeightColumnDependant = true;
            ColumnsSize = new List<float>();
            RowsSize = new List<float>();
            ConstantRowsAndColumns = false;
        }

        public Grid(int columns, int rows, int columnWidth, int rowHeight) : base()
        {
            WitdhAndHeightColumnDependant = true;
            ColumnsSize = new List<float>();
            RowsSize = new List<float>();
            ConstantRowsAndColumns = true;
            ColumnsSize = new List<float>(columns);
            RowsSize = new List<float>(rows);

            for (int i = 0; i < columns; i++)
            {
                ColumnsSize.Add(columnWidth);
            }

            for (int i = 0; i < rows; i++)
            {
                RowsSize.Add(rowHeight);
            }

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

        public void AddChild(GuiElement element, int row, int column, int columnWidth)
        {
            Child ch = new Child
            {
                element = element,
                column = column,
                row = row,
                columnWidth = columnWidth
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
        public void AddChild(GuiElement element)
        {
            if (MaxChildren)
            {
                Vector2 pos = new Vector2();
                if (Children.Count < ChildMaxAmount)
                {
                    pos = GetFreeSpace();
                }
                Child ch = new Child()
                {
                    element = element,
                    column = (int)pos.Y,
                    row = (int)pos.X
                };

                ch.id = Children.Count;
                if (Children.Count < ChildMaxAmount)
                    Children.Add(ch);
                Update();
                UpdateChildren();
            }
            else
            {
                Child ch = new Child()
                {
                    element = element,
                    column = 0
                };

                ch.id = Children.Count;
                ch.row = Rows;
                
                Children.Add(ch);
                Update();
                UpdateChildren();
            }
        }

        private Vector2 GetFreeSpace()
        {
            Vector2 t = new Vector2();
            if(Children.Count == 0)
                return Vector2.Zero;
            int row = Children.Max(p => p.row);
            int column = Children.Where(p=> p.row == row).Max(p => p.column);
            if (column < 4)
            {
                t.X = row;
                t.Y = column + 1;

            }
            else
            {
                t.X = row + 1;
                t.Y = 0;
            }

            return t;
        }

        public void ChangeRow(int i)
        {
            int newRow = ShowedRow + i;
            if (newRow >= 0 && newRow < RowsSize.Count)
            {
                ShowedRow = newRow;
            }
        }
        public void UpdateP()
        {
            Update();
            UpdateChildren();
            UpdateActive(Active);
        }
        private void Update()
        {
            
            if (!ConstantRowsAndColumns)
            {
                float[] ColumnsSiz = new float[Columns];
                float[] RowsSiz = new float[Rows];
                float[] columnMaxWidth = new float[Columns];
                List<Child> multiElements = new List<Child>();
                if (AllVisible)
                {
                    for (int i = 0; i < Rows; i++)
                    {
                        List<GuiElement> elements = new List<GuiElement>();
                        Children.Where(p => p.row == i).ToList().ForEach(p => elements.Add(p.element));
                        int maxHeight = 0;
                        if (elements.Count > 0)
                        {
                            maxHeight = elements.Max(p => p.Height);
                        }

                        RowsSiz[i] = (maxHeight);
                    }
                }


                for (int i = 0; i < Columns; i++)
                {

                    var elements = Children.Where(p => p.column == i).ToList();
                    if (elements.Count > 0)
                    {
                        foreach (var element in elements)
                        {
                            if (element.element.Width > columnMaxWidth[i])
                            {
                                if (element.columnWidth > 1)
                                {
                                    multiElements.Add(element);
                                }
                                else
                                {
                                    columnMaxWidth[i] = element.element.Width;
                                }
                            }
                        }
                    }
                }
                ColumnsSiz = columnMaxWidth;
                RowsSize = RowsSiz.ToList();
                ColumnsSize = ColumnsSiz.ToList();
            }

            



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
                if(AllVisible)
                    child.element.Origin = new Point(this.Origin.X + (int)ColumnOffset(child.column),this.Origin.Y + (int)RowOffset(child.row));
                else
                {
                    child.element.Origin = new Point(this.Origin.X + (int)ColumnOffset(child.column), this.Origin.Y + (int)RowOffset(0));
                }
                if (child.element is Grid)
                {
                    Grid g = (Grid) child.element;
                    g.UpdateP();
                }
                else
                {
                    /*
                    if (child.element is IHasText)
                    {
                        IHasText element = (IHasText) child.element;
                        element.Update();
                    }
                    */
                    child.element.Update();
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
                    for (int j = 1; j <= element.columnWidth; j++)
                    {
                        if(j == 1)
                            element.element.Width = (int)ColumnsSize[element.column];

                        else if (Columns >=  (element.column + j))
                        {
                            element.element.Width += (int) ColumnsSize[element.column + j - 1];
                            element.element.Width += columnOffset;
                        }
                            
                    }
                    //element.element.Width = (int)ColumnsSize[element.column];
                    element.element.Height = (int) RowsSize[element.row];
                }
            }

            UpdateChildren();
        }
        public void ResizeChildren(int width,int height)
        {
            foreach (var child in Children)
            {
                child.element.Width = width;
                child.element.Height = height;
            }

            UpdateChildren();
        }
        public GuiElement GetChild(int row, int column)
        {
            return Children.Single(p => p.column == column && p.row == row).element;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (DrawBorder)
            {
                if (Background == null)
                    Background = Util.CreateTexture(Game1.self.GraphicsDevice, Width, Height, pixel => Color.Black,
                        BorderSize, 0);
                spriteBatch.Begin();
                spriteBatch.Draw(Background, Boundary, Color.White);
                spriteBatch.End();
            }

            if (AllVisible)
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
            else
            {
                List<Child> childrenToShow = Children.Where(p => p.row >= ShowedRow && p.row < ShowedRow + VisibleRows).ToList();
                foreach (var child in childrenToShow)
                {
                    if (child.name != null && child.name.Equals("gridW"))
                    {
                        int z = 0;
                    }
                    child.element.Draw(spriteBatch);
                }
            }
        }
#endregion
    }
}
