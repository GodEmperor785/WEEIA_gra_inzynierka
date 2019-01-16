using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class GUI
    {
        public SpriteFont smallFont;
        public SpriteFont smediumFont;
        public SpriteFont mediumFont;
        public SpriteFont hugeFont;
        public SpriteFont bigFont;

        public GUI(ContentManager content)
        {
            LoadContent(content);
        }
        public void LoadContent(ContentManager content)
        {
            smallFont = content.Load<SpriteFont>("small");
            smediumFont = content.Load<SpriteFont>("smedium");
            mediumFont = content.Load<SpriteFont>("medium");
            hugeFont = content.Load<SpriteFont>("huge");
            bigFont = content.Load<SpriteFont>("big");
        }
    }
}
