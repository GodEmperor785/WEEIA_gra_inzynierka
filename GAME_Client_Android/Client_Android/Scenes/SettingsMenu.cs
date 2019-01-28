using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Client_Android;
using Client_PC.UI;
using Client_PC.Utilities;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class SettingsMenu : Menu
    {


        private Dropdown drop;
        bool dropClicked = false;
        List<Menu> menus = new List<Menu>();
        private List<IClickable> ClickableToRemove;
        private int coupleHeight = 100;
        private int coupleWidth = 200;
        private Button up;
        private Button down;
        private Button back;
        class cardCouple
        {
            public Label lbl;
            public Button button;
        }

        private List<cardCouple> couples = new List<cardCouple>();
        private Grid gridCards;
        private RelativeLayout layout = new RelativeLayout();
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            

            gridCards = new Grid(2, 100, coupleWidth, coupleHeight);
            gridCards.DrawBorder = true;
            gridCards.BorderSize = 3;
            gridCards.WitdhAndHeightColumnDependant = false;
            gridCards.AllVisible = false;
            gridCards.VisibleRows = 5;
            gridCards.ConstantRowsAndColumns = true;
            gridCards.MaxChildren = true;
            gridCards.ChildMaxAmount = 200;
            gridCards.Height = 5 * coupleHeight + 6 * gridCards.rowOffset;
            gridCards.Width = 2 * coupleWidth + gridCards.columnOffset;
            up = new Button(gridCards.Width, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "up"
            };
            down = new Button(gridCards.Width, 30, Game1.self.GraphicsDevice,
                Gui, Gui.mediumFont, true)
            {
                Text = "down"
            };
            back = new Button(gridCards.Width, coupleHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true)
            {
                Text = "Back to menu"
            };
            up.clickEvent += UpClick;
            down.clickEvent += DownClick;
            back.clickEvent += OnExit;
            Clickable.Add(back);
            layout.AddChild(gridCards);
            layout.AddChild(down);
            layout.AddChild(up);
            layout.AddChild(back);
            Clickable.Add(up);
            Clickable.Add(down);
            back.Update();
            up.Update();
            down.Update();
            gridCards.UpdateP();
            int z = 243123;
            SetClickables(true);
        }

        public void SetMenus(List<Menu> menuList)
        {
            menus = menuList;
        }
        private void UpClick()
        {
            gridCards.ChangeRow(-1);
        }

        private void DownClick()
        {
            gridCards.ChangeRow(1);
        }
        
        public void OnExit()
        {
            Game1.self.state = Game1.State.MainMenu;
            ClickableToRemove.ForEach(p=> Clickable.Remove(p));
        }


        public override void UpdateGrid()
        {
            layout.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - gridCards.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - gridCards.Height / 2.0f));

            gridCards.Origin = layout.Origin;
            gridCards.UpdateP();
            up.Origin = new Point(gridCards.Origin.X, gridCards.Origin.Y + gridCards.Height);
            down.Origin = new Point(up.Origin.X, up.Origin.Y + 30);
            back.Origin = new Point(down.Origin.X,down.Origin.Y + down.Height + 10);
            layout.Update();
            gridCards.UpdateActive(true);
        }

        public override IClickable GetClickable(Point xy)
        {
            var z = Clickable.Where(p => p.Active).Where(p => p.GetBoundary().Contains(xy));
            return Clickable.Where(p => p.Active).SingleOrDefault(p => p.GetBoundary().Contains(xy));
        }
        public override void UpdateFields()
        {
            dropClicked = false;
        }
        public void FillGridWithCardTypes(List<Ship> ships)
        {
            var names = ships.Select(p => p.Name).Distinct().ToList();
            ClickableToRemove = new List<IClickable>();
            gridCards.RemoveChildren();
            int i = 0;
            foreach (var name in names)
            {
                cardCouple couple = new cardCouple();
                Label lbl = new Label(coupleWidth, coupleHeight, Game1.self.GraphicsDevice, Gui, Gui.smediumFont, true)
                {
                    Text = name
                };
                Button button = new Button(coupleWidth, coupleHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
                {
                    Text = "Select skin"
                };
                button.clickEventObject += setTexture;
                Clickable.Add(button);
                ClickableToRemove.Add(button);
                couple.lbl = lbl;
                couple.button = button;
                couples.Add(couple);
                gridCards.AddChild(lbl);
                gridCards.AddChild(button);
                i++;
            }

            gridCards.DrawBackground = false;
            gridCards.UpdateP();
            gridCards.UpdateActive(true);
            layout.Update();

        }
        public void setTexture(object sender)
        {
            Button b = sender as Button;
            var cardName = couples.Single(p => p.button == b).lbl.Text;

            Game1.self.activitySelf.getFile(cardName);
            
            
         //           Game1.self.SetTextureToShip(filePath, cardName);
        }
        public override void UpdateClick(IClickable button)
        {
            
            Game1.self.FocusedElement = button;
            if (button is Dropdown)
            {
                Dropdown d = (Dropdown)button;
                if (d.ShowChildren)
                {
                    button.OnClick();
                    d.Active = false;
                }
                else
                {
                    d.ShowChildren = true;
                    d.Update();
                    dropClicked = true;
                    button.Active = false;
                    d.Active = true;
                }
            }
            else if (button.Parent is Dropdown)
            {
                dropClicked = true;
                Dropdown d = (Dropdown)button.Parent;
                button.OnClick();
               // d.Active = true;
            }
            else
            {
                button.OnClick();
            }
        }
        public void Draw(GameTime gameTime)
        {

            layout.Draw(Game1.self.spriteBatch);

        }
    }
}
