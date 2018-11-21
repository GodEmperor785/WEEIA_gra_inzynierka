using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Client_PC.UI;
using Client_PC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class SettingsMenu : Menu
    {

        private Grid grid;

        private Dropdown drop;
        bool dropClicked = false;
        List<Menu> menus = new List<Menu>();
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            grid = new Grid();
            Label label1 = new Label(new Point(0, 0), 100, 55, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Resolution w cholere dlugi az prawie ze lorem ipsum ale kogo by to obchodzilo wazne ze wrapuje text chyba co nie?"
            };
             drop = new Dropdown(new Point(0,0),100,30,Game1.self.GraphicsDevice, Gui);
            Button button = new Button(new Point(0, 0), 100, 35, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Back",Id = 0
            };
            Button dropElement1 = new Button(new Point(0, 0), 100, 30, Game1.self.GraphicsDevice, Gui, Gui.mediumFont,true)
            {
                Text = Constants.fullhd,
                Id = 1
            };
            drop.Add(dropElement1,"fullHd", drop);
            Button dropElement2 = new Button(new Point(0, 0), 100, 30, Game1.self.GraphicsDevice, Gui, Gui.mediumFont,true)
            {
                Text = Constants.hd,
                Id = 2
            };
            Button buttonSave = new Button(new Point(0, 0), 100, 35, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Save",
                Id = 4
            };
            buttonSave.clickEvent += onSave;
            Grid grid2 = new Grid();
            grid2.AddChild(button,0,1);
            grid2.AddChild(buttonSave, 0, 0);
            drop.Add(dropElement2, "Hd", drop);
            button.clickEvent += OnExit;
            Clickable.Add(drop);
            Clickable.Add(button);
            Clickable.Add(buttonSave);
            Clickable.Add(dropElement1);
            Clickable.Add(dropElement2);
            button.Active = true;
            grid2.Active = true;
            grid2.UpdateActive(true);
            grid.AddChild(label1,0,0);
            grid.AddChild(drop, 1, 0);
            grid.AddChild(grid2,2,0, "gridW");
            grid.Active = true;
            grid.UpdateActive(true);
            grid.ResizeChildren();
            drop.ResizeChildren();
            int z = 243123;
            SetClickables(true);
        }

        public void SetMenus(List<Menu> menuList)
        {
            menus = menuList;
        }
        public void onSave()
        {
            var element = drop.GetSelected();
            int width = 800;
            int height = 600;
            Config conf = new Config();
            if (element is Button)
            {
                Button button = (Button) element;
                if (button.text.Equals(Constants.hd))
                {
                    width = 1280;
                    height = 720;
                    conf.Resolution = Constants.hd;
                }
                else if (button.text.Equals(Constants.fullhd))
                {
                    width = 1920;
                    height = 1080;
                    conf.Resolution = Constants.fullhd;
                }
            }
            Game1.self.Wallpaper = Utils.CreateTexture(Game1.self.GraphicsDevice, width, height);
            Game1.self.graphics.PreferredBackBufferHeight = height;
            Game1.self.graphics.PreferredBackBufferWidth = width;
            Game1.self.graphics.ApplyChanges();
            menus.ForEach(p=>
            {
                p.Reinitialize(Game1.self.Content);
            });
            

            TextWriter writer = new StreamWriter("Config");
            XmlSerializer xml = new XmlSerializer(typeof(Config));
            xml.Serialize(writer,conf);
            writer.Close();
        }
        public void OnExit()
        {
            Game1.self.state = Game1.State.MainMenu;
        }


        public override void UpdateGrid()
        {
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
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

            grid.Draw(Game1.self.spriteBatch);

        }
    }
}
