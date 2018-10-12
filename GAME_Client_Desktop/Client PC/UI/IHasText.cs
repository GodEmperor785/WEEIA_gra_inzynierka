using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    interface IHasText
    {
        void Update();
        string Text { get; set; }
        Vector2 TextPosition { get; set; }
        SpriteFont Font { get; set; }
    }
}
