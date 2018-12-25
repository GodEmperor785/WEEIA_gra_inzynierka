using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC;
using Client_PC.UI;
using Client_PC.Utilities;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Scenes
{
    class MainMenu : Menu
    {
        private Grid grid;
        private Deck chosenDeck;
        private Grid g;
        private Popup popup;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            Button z = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Deck"
            };
            Button z2 = new Button(new Point(100, 200), 70, 125, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Settings"
            };
            Button z3 = new Button(new Point(100, 200), 200, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont,true)
            {
                Text = "Exit"
            };
            Button p1 = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont, true)
            {
                Text = "Play ranked"
            };
            Button p2 = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont, true)
            {
                Text = "Play custom"
            };
            Button z4 = new Button(new Point(100, 200), 120, 100, Game1.self.GraphicsDevice, Gui, Gui.bigFont, true)
            {
                Text = "Shop"
            };
            Clickable.Add(p1);
            Clickable.Add(z);
            Clickable.Add(z2);
            Clickable.Add(z3);
            Clickable.Add(z4);
            grid = new Grid();
            grid.AddChild(p1,0,0);
            grid.AddChild(p2,1,0);
            grid.AddChild(z, 2, 0);
            grid.AddChild(z4, 3, 0);
            grid.AddChild(z2, 4, 0);
            grid.AddChild(z3,5,0);
            p1.clickEvent += Play;
            z3.clickEvent += ExitClick;
            z4.clickEvent += GoToShop;
            z2.clickEvent += GoToSettings;
            z.clickEvent += GoToDeck;
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f),(int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            grid.ResizeChildren();
            SetClickables(true);
        }

        public void Play()
        {
            
            int topOrigin = 300;
            int leftOrigin = (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.3);
            int leftOffset = 10;
            int topOffset = 10;
            int buttonWidth = 200;
            int buttonHeight = 50;
            Point popupOrigin = new Point(leftOrigin,topOrigin);
            RelativeLayout layout = new RelativeLayout();
            Button up = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "up"
            };
            up.Origin = new Point(leftOrigin + leftOffset, topOrigin + topOffset);
            up.clickEvent += upClick;
            Clickable.Add(up);
            Button down = new Button(buttonWidth, buttonHeight, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "down"
            };
            down.clickEvent += downClick;
            Clickable.Add(down);
            g = new Grid(1, 5, buttonWidth, buttonHeight);
            g.Origin = new Point(leftOrigin + leftOffset, up.Origin.Y + up.Height + 10);
            Game1.self.Decks.ForEach(p =>
            {
                Deck d = new Deck(new Point(0,0), buttonWidth, buttonHeight, Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true,p.Name );
                d.SetFleet(p);
                d.clickEvent += DeckClick;
                Clickable.Add(d);
                g.AddChild(d);
            });

            g.WitdhAndHeightColumnDependant = false;
            g.AllVisible = false;
            g.VisibleRows = 5;
            g.ConstantRowsAndColumns = true;
            g.UpdateP();
            Point downPoint = new Point(leftOrigin + leftOffset, g.Origin.Y + g.RealHeight + 10);
            down.Origin = downPoint;
            layout.AddChild(up);
            layout.AddChild(down);
            layout.AddChild(g);
            popup = new Popup(popupOrigin, buttonWidth + 20, topOffset + up.Height + g.RealHeight + 10 + down.Height + 10 + 10,Game1.self.GraphicsDevice,Gui);
            popup.layout = layout;
            up.Update();
            down.Update();
            g.UpdateP();
            popup.SetBackground();
            Game1.self.popupToDraw = popup;
            popup.SetActive(true);
            SetClickables(false);
        }

        public void DeckClick(object sender)
        {
            Deck d = (Deck)sender;
            chosenDeck = d;
        }

        public void downClick()
        {
            g.ChangeRow(1);
        }

        public void upClick()
        {
            g.ChangeRow(-1);
        }
        public void GoToDeck()
        {
            Game1.self.state = Game1.State.DeckMenu;
            Game1.self.SetDecks(Game1.self.Decks);
            Game1.self.CleanDeck();
        }

        public void GoToShop()
        {
            GamePacket packet = new GamePacket(OperationType.GET_LOOTBOXES, null);
            Game1.self.Connection.Send(packet);
            packet = Game1.self.Connection.GetReceivedPacket();
            List<LootBox> loots = (List<LootBox>) packet.Packet;
            Game1.self.SetShop(loots);
            Game1.self.state = Game1.State.ShopMenu;
        }
        public void ExitClick()
        {
            Game1.self.Quit();
        }

        public void GoToSettings()
        {
            Game1.self.state = Game1.State.OptionsMenu;
        }
       

        public override void UpdateGrid()
        {
            grid.Origin = new Point((int)(Game1.self.GraphicsDevice.Viewport.Bounds.Width / 2.0f - grid.Width / 2.0f), (int)(Game1.self.GraphicsDevice.Viewport.Bounds.Height / 2.0f - grid.Height / 2.0f));
            grid.UpdateP();
            if(popup != null)
                SetClickables(!popup.Active);
            else
            {
                SetClickables(true);
            }
        }
        public void Draw(GameTime gameTime)
        {

            grid.Draw(Game1.self.spriteBatch);

        }
    }
}
