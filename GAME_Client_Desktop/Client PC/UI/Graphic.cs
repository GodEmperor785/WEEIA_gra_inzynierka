using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Graphic : GuiElement
    {
        public Texture2D Texture { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get;  set; }
        public Vector2 TextPosition { get;  set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if(Texture != null)
                spriteBatch.Draw(Texture, Position, scale: Scale);
            if (Text != null)
                spriteBatch.DrawString(Font, Text, Position, Color.Black);
            spriteBatch.End();
        }
    }
}
