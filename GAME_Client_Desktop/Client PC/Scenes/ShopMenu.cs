using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class ShopMenu : Menu
    {
        enum State { normal,cards}

        private State state;
        private Grid grid;
        private Popup popup;

        private Grid BoxesGrid;
        private Grid BoughtShipsGrid;
        private List<LootBoxElement> Lootboxes;
        public override void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
            BoxesGrid = new Grid(3,1, (int)(Game1.self.graphics.PreferredBackBufferWidth* 0.25),(int)(Game1.self.graphics.PreferredBackBufferWidth* 0.4));
            BoxesGrid.DrawBackground = false;
            BoxesGrid.WitdhAndHeightColumnDependant = false;
            BoxesGrid.ConstantRowsAndColumns = true;
            grid = new Grid();
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
            grid.AddChild(exitButton,0,0);
            Clickable.Add(exitButton);
            exitButton.clickEvent += GoToMenu;
            grid.UpdateP();
            SetClickables(true);
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
                }
                else
                {
                    
                }
            }
            else
            {
                
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
                BoughtShipsGrid.AddChild(c,0,column);
                column++;
            });
            BoughtShipsGrid.Origin = new Point((Game1.self.graphics.PreferredBackBufferWidth - BoughtShipsGrid.Width) / 2,300);
            BoughtShipsGrid.UpdateP();
            
        }
        public void GoToMenu()
        {
            if (state == State.normal)
            {
                Game1.self.UpdatePlayer();
                Game1.self.state = Game1.State.MainMenu;
            }
            else if (state == State.cards)
                state = State.normal;
        }
        public void Reinitialize(List<LootBox> loots)
        {
            int column = 0;
            loots.ForEach(p=>
            {
                LootBoxElement lb = new LootBoxElement(200, 200, Game1.self.GraphicsDevice, Gui, GetRarity(p), p);
                BoxesGrid.AddChild(lb,0,column);
                lb.clickEvent += GetLootbox;
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
