using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Microsoft.Xna.Framework.Content;

namespace Client_PC.Scenes
{
    class DeckMenu : Menu
    {
        private RelativeLayout layout;

        public void Initialize(ContentManager Content)
        {
            Gui = new GUI(Content);
        }


    }
}
