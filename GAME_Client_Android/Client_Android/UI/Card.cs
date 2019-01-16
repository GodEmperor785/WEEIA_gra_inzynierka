using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Client_Android;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Client_PC.UI
{
    class Card : GuiElement, IClickable, IHasText
    {
        public enum status { clear,clicked,target,hasMove,isTarget}

        public Dictionary<Rarity, Color> RarityColor = new Dictionary<Rarity, Color>
        {
            {Rarity.COMMON, new Color(180, 180, 180)},
            {Rarity.RARE, new Color(29, 131, 247)},
            {Rarity.VERY_RARE, new Color(135, 69, 172)},
            {Rarity.LEGENDARY, new Color(255, 123, 17)}
        };
        
        public status Status { get; set; } = status.clear;
        private status LastStatus = status.clear;
        private Texture2D skin;
        private Texture2D hpIcon;
        private Texture2D armorIcon;
        private Vector2 hpIconPosition;
        private Vector2 armorIconPosition;
        private Vector2 hpIconScale;
        private Vector2 armorIconScale;
        private int hp;
        private int armor;
        private string name;
        private Vector2 hpPosition;
        private Vector2 armorPosition;
        private Vector2 namePosition;
        private RelativeLayout overlay;
        private Ship ship;
        public bool Active { get; set; }
        public bool ActiveChangeable { get; set; }
        public Tooltip Tooltip { get; set; }
        public string Text { get; set; }
        public Vector2 TextPosition { get; set; }
        public SpriteFont Font { get; set; }
        public bool TextWrappable { get; set; }
        public delegate void ElementClicked(object sender);
        public event ElementClicked clickEvent;
        private bool clicked;
        public bool CanMove { get; set; }
        public Line line;
        public bool IsOver { get; set; }
        public bool HeightDerivatingFromText { get; set; }
        public Card(int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable, Ship shipInc) : base( width, height, device, gui)
        {
            ship = shipInc;
            Font = font;
            ActiveChangeable = true;
            TextWrappable = wrapable;
            overlay = new RelativeLayout();
            this.Width = width;
            this.Height = height;
            using (FileStream fileStream = new FileStream("Content/Icons/Health.png", FileMode.Open))
            {
                hpIcon = Texture2D.FromStream(Game1.self.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
            using (FileStream fileStream = new FileStream("Content/Icons/Armor.png", FileMode.Open))
            {
                armorIcon = Texture2D.FromStream(Game1.self.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
            hpIconPosition = new Vector2(5, height - 10);
            armorIconPosition = new Vector2(25, height - 10);
            hpIconScale = new Vector2(0.15f * Width / hpIcon.Width ,0.10f * Height / hpIcon.Height);
            armorIconScale = new Vector2(0.15f * Width /  armorIcon.Width,0.10f * Height / armorIcon.Height);
            Graphic g = new Graphic
            {
                Scale = hpIconScale,
                Texture = hpIcon,
                Position = hpIconPosition
            };
            overlay.AddChild(g, "hpIcon");
            Graphic armorGraphic = new Graphic
            {
                Texture = armorIcon,
                Scale = armorIconScale,
                Position = armorIconPosition
            };
            hp = (int)ship.Hp;
            armor = (int)ship.Armor;
            overlay.AddChild(armorGraphic,"armorIcon");
            Graphic hpText = new Graphic()
            {
                Text = hp.ToString(),
                Font =  Gui.mediumFont
            };
            Graphic armorText = new Graphic()
            {
                Text = armor.ToString(),
                Font = Gui.mediumFont
            };
            overlay.AddChild(hpText,"hpText");
            overlay.AddChild(armorText,"armorText");
            Tooltip = createTooltip();
        }

        private Tooltip createTooltip()
        {
            int width = 450;
            Tooltip tooltip = new Tooltip(width,Game1.self.GraphicsDevice,Gui,Gui.smediumFont,true);
            tooltip.Text = "Name:    " + ship.Name+" \n"
                           +"Faction:    "+ship.Faction.Name+" \n"
                           +"Rarity:    "+ship.Rarity.GetRarityName()+" \n"
                           +"Weapons: \n";
            tooltip.Text += "Name    Chance to hit    Damage" + " \n";
            foreach (var shipWeapon in ship.Weapons)
            {
                tooltip.Text += shipWeapon.Name + "    "+shipWeapon.ChanceToHit*100+"%    "+shipWeapon.Damage +" \n";
            }
            tooltip.Text += "Defences:\n";
            tooltip.Text += "Name    Type    Defence value: \n";
            foreach (var defenceSystem in ship.Defences)
            {
                tooltip.Text += defenceSystem.Name + "    " + defenceSystem.SystemType.GetDefenceSystemTypeName() + "    " +
                                defenceSystem.DefenceValue + " \n";
            }
            return tooltip;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            spriteBatch.Draw(Texture, Boundary, Color.White);
            //spriteBatch.End();
            overlay.Draw(spriteBatch);
        }
        public override void Update()
        {
            Graphic hpIcon = (Graphic) overlay.GetChild("hpIcon");
            hpIcon.Position= new Vector2(Origin.X + 0.01f * Width, Origin.Y + Height - 0.10f * Height);

            Graphic armorIcon = (Graphic) overlay.GetChild("armorIcon");
            armorIcon.Position = new Vector2(Origin.X + 0.58f * Width, Origin.Y + Height - 0.10f * Height);

            Graphic hpText = (Graphic) overlay.GetChild("hpText");
            hpText.Position = hpIcon.Position + new Vector2(0.175f * Width, - 0.025f * Height);

            Graphic armorText = (Graphic) overlay.GetChild("armorText");
            armorText.Position = armorIcon.Position + new Vector2(0.175f * Width, -0.025f * Height);
            if (NeedNewTexture)
            { 
                Texture = Util.CreateTexture(Device, Width, Height,RarityColor[ship.Rarity],InsideColor);
            }
            /// blue - has chosen move
            /// green - is clicked
            /// red - is target of current action
            /// yellow - is target

            Color z = new Color(100,100,100);
            if (Status != LastStatus)
            {
                if (Status == status.target)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Red,
                        z
                    );
                }
                else if (Status == status.isTarget)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Yellow,
                        z
                    );
                }
                else if (Status == status.clicked)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Green,
                        z
                    );
                }
                else if (Status == status.hasMove)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Blue,
                        z
                    );
                }
                else if (Status == status.clear)
                {
                    Texture = Util.CreateTexture(Device, Width, Height);
                }
            }

            LastStatus = Status;

        }

        public void ChangeParent(Grid from, Grid to)
        {
            if (to.CanHaveMoreChildren())
            {
                from.RemoveChild(this);
                to.AddChild(this);
                this.Active = false;
                Console.WriteLine(1);
            }
        }
        public void OnClick()
        {
            if(Active)
                clickEvent(this);
        }

        public Ship GetShip()
        {
            return ship;
        }
        public bool Equals(Card cd)
        {
            if (this.ship.Id == cd.ship.Id)
                return true;
            else
                return false;
        }
        public Rectangle GetBoundary()
        {
            return Boundary;
        }
    }
}
