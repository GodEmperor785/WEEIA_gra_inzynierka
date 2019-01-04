using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using GAME_connection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Client_PC.UI
{
    class Card : GuiElement, IClickable, IHasText
    {
        public enum status { clear,clicked,target,hasMove,isTarget}

        public status Status { get; set; } = status.clear;
        private status LastStatus = status.clear;
        private Texture2D skin;
        private Texture2D hpIcon;
        private Texture2D armorIcon;
        private Vector2 hpIconPosition;
        private Vector2 armorIconPosition;
        private Vector2 hpIconScale;
        private Vector2 armorIconScale;
        private double hp;
        private double armor;
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
        public Card(int width, int height, GraphicsDevice device, GUI gui, SpriteFont font, bool wrapable, Ship shipInc) : base( width, height, device, gui)
        {
            ship = shipInc;
            Font = font;
            ActiveChangeable = true;
            TextWrappable = wrapable;
            overlay = new RelativeLayout();
            this.Width = width;
            this.Height = height;
            using (FileStream fileStream = new FileStream("Content/Icons/hp.png", FileMode.Open))
            {
                hpIcon = Texture2D.FromStream(Game1.self.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
            using (FileStream fileStream = new FileStream("Content/Icons/armor.jpg", FileMode.Open))
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
            hp = ship.Hp;
            armor = ship.Armor;
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
                Texture = Util.CreateTexture(Device, Width, Height);
            /// blue - has chosen move
            /// green - is clicked
            /// red - is target of current action
            /// yellow - is target


            if (Status != LastStatus)
            {
                if (Status == status.target)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Red,
                        new Color(100, 100, 100)
                    );
                }
                else if (Status == status.isTarget)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Yellow,
                        new Color(100, 100, 100)
                    );
                }
                else if (Status == status.clicked)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Green,
                        new Color(100, 100, 100)
                    );
                }
                else if (Status == status.hasMove)
                {
                    Texture = Util.CreateTexture(Device, Width, Height, Color.Blue,
                        new Color(100, 100, 100)
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
