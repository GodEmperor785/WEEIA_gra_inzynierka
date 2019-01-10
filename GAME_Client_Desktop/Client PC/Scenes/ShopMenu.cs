using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.Scenes
{
    class ShopMenu : Menu
    {
        enum State { normal,cards}

        private State state;
        private Grid grid;
        private Texture2D credits;
        private Grid BoxesGrid;
        private Grid BoughtShipsGrid;
        private List<LootBoxElement> Lootboxes;
        private RelativeLayout layout;
        private Label CreditsAmount, lbl1;
        private Graphic g;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            BoxesGrid = new Grid(3,1, (int)(Game1.self.graphics.PreferredBackBufferWidth* 0.25),(int)(Game1.self.graphics.PreferredBackBufferWidth* 0.4));
            BoxesGrid.DrawBackground = false;
            BoxesGrid.WitdhAndHeightColumnDependant = false;
            BoxesGrid.ConstantRowsAndColumns = true;
            grid = new Grid();
            layout = new RelativeLayout();
            //grid.DrawBackground = true;
            grid.Width = 300;
            grid.Height = 300;
            grid.Origin = new Point(300,300);
            //grid.Origin = new Point((Game1.self.graphics.PreferredBackBufferWidth - 200) / 2, Game1.self.graphics.PreferredBackBufferHeight - 300);
            //Game1.self.graphics.PreferredBackBufferWidth
            BoxesGrid.Origin = new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * 0.1), (int)(Game1.self.graphics.PreferredBackBufferWidth * 0.1));
            Button exitButton = new Button(200, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                text = "Back"
            };
            using (FileStream fileStream = new FileStream("Content/Icons/Credits.png", FileMode.Open))
            {
                credits = Texture2D.FromStream(Game1.self.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
            g = new Graphic
            {
                Scale = new Vector2(60f / credits.Width,60f / credits.Height),
                Texture = credits,
                Position = new Vector2(10,10)
            };

            popup = new Popup(new Point((int)(Game1.self.graphics.PreferredBackBufferWidth * 0.5), (int)(Game1.self.graphics.PreferredBackBufferHeight * 0.5)), 100, 400, Game1.self.GraphicsDevice, Gui);
            Grid popupGrid = new Grid();
            lbl1 = new Label(200, 200, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true);
            Button b1 = new Button(100, 100, Game1.self.GraphicsDevice, Gui, Gui.mediumFont, true)
            {
                Text = "Ok"
            };
            lbl1.DrawBackground = false;
            b1.DrawBackground = false;
            popup.grid = popupGrid;
            popupGrid.AddChild(lbl1, 0, 0);
            popupGrid.AddChild(b1, 1, 0);
            b1.clickEvent += onPopupExit;
            Clickable.Add(b1);
            popup.SetToGrid();




            CreditsAmount = new Label(new Point((int)g.Position.X + g.Width + 60,(int) g.Position.Y + g.Height / 2),100,100,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true );
            CreditsAmount.WidthDerivatingFromText = true;
            CreditsAmount.HeightDerivatingFromText = true;
            CreditsAmount.Text = 123412.ToString();
            CreditsAmount.InsideColor = new Color(160,160,160);
            CreditsAmount.OutsideColor = new Color(120, 120, 120);
            layout.AddChild(g,"CreditsIcon");
            layout.AddChild(CreditsAmount,"CreditsAmount");
            grid.AddChild(exitButton,0,0);
            Clickable.Add(exitButton);
            exitButton.clickEvent += GoToMenu;
            grid.UpdateP();
            SetClickables(true);
        }
        public void onPopupExit()
        {
            popup.SetActive(false);
            foreach (var clickable in Clickable.Except(Clickable.Where(p => p.Parent == popup.grid)))
            {
                clickable.Active = true;
            }

            Game1.self.popupToDraw = null;

        }
        public override void Clean()
        {
            BoxesGrid.RemoveChildren();
            
        }

        public void GetLootbox(object sender)
        {
            LootBoxElement el = (LootBoxElement) sender;
            GamePacket packet = new GamePacket(OperationType.BUY,el.Lootbox);
            Game1.self.Connection.Send(packet);
            packet = Game1.self.Connection.GetReceivedPacket();
            if (packet.OperationType == OperationType.SUCCESS)
            {
                packet = Game1.self.Connection.GetReceivedPacket();
                if (packet.OperationType == OperationType.BOUGHT_SHIPS)
                {
                    List<Ship> ships = (List<Ship>) packet.Packet;
                    InitializeBoughtShipsGrid(ships);
                    state = State.cards;
                    Game1.self.UpdatePlayer();
                    SetMoney(Game1.self.player.Money);
                    BoxesGrid.UpdateActive(false);
                    BoughtShipsGrid.UpdateActive(true);
                }
                else
                {
                    
                }
            }
            else
            {
                lbl1.Text = packet.Packet.ToString();
                popup.SetActive(true);
                Game1.self.popupToDraw = popup;
                SetClickables(false);
            }
        }

        public void InitializeBoughtShipsGrid(List<Ship> ships)
        {
            int columns = ships.Count;
            int cardWidth = 133;
            int cardHeight = 200;
            BoughtShipsGrid = new Grid(columns,1, cardWidth, cardHeight);
            BoxesGrid.DrawBackground = false;
            BoxesGrid.WitdhAndHeightColumnDependant = false;
            BoxesGrid.ConstantRowsAndColumns = true;
            int column = 0;
            ships.ForEach(p =>
            {
                Card c = new Card(cardWidth,cardHeight,Game1.self.GraphicsDevice,Gui,Gui.mediumFont,true,p);
                Clickable.Add(c);
                c.Active = true;
                BoughtShipsGrid.AddChild(c,0,column);
                column++;
            });
            BoughtShipsGrid.Origin = new Point((Game1.self.graphics.PreferredBackBufferWidth - BoughtShipsGrid.Width) / 2,300);
            BoughtShipsGrid.UpdateP();
            
        }

        public override void UpdateLast()
        {
            CreditsAmount.Origin = new Point((int)g.Position.X + (int)(g.Texture.Width * g.Scale.X) + 60, (int)g.Position.Y + (int)(g.Texture.Height * g.Scale.Y) / 2 -  CreditsAmount.Height / 2);
        }

        public void GoToMenu()
        {
            if (state == State.normal)
            {
                Game1.self.UpdatePlayer();
                Game1.self.state = Game1.State.MainMenu;
            }
            else if (state == State.cards)
            {
                state = State.normal;
                BoughtShipsGrid.UpdateActive(false);
                BoxesGrid.UpdateActive(true);
            }
        }

        public void SetMoney(int amount)
        {
            CreditsAmount.Text = amount.ToString();
        }
        public void Reinitialize(List<LootBox> loots)
        {
            int column = 0;
            loots.ForEach(p=>
            {
                LootBoxElement lb = new LootBoxElement(200, 200, Game1.self.GraphicsDevice, Gui, GetRarity(p), p);
                BoxesGrid.AddChild(lb,0,column);
                lb.clickEvent += GetLootbox;
                lb.ActiveChangeable = true;
                Clickable.Add(lb);
                column++;
            });
            grid.Origin = new Point((Game1.self.graphics.PreferredBackBufferWidth - 200) / 2, Game1.self.graphics.PreferredBackBufferHeight - 300);
            grid.UpdateP();
            SetClickables(true);
        }

        public string GetRarity(LootBox loot)
        {
            string result = "0";
            if (loot.Name.Equals("basic lootbox"))
                result = "common";
            if (loot.Name.Equals("better lootbox"))
                result = "uncommon";
            if (loot.Name.Equals("supreme lootbox"))
                result = "rare";
            return result;
        }

        public void Draw(GameTime gameTime)
        {
            layout.Draw(Game1.self.spriteBatch);
            if (state == State.normal)
            {
                
                BoxesGrid.Draw(Game1.self.spriteBatch);
            }
            else if(state == State.cards)
            {
                BoughtShipsGrid.Draw(Game1.self.spriteBatch);
            }
            grid.Draw(Game1.self.spriteBatch);
        }
    }
}
