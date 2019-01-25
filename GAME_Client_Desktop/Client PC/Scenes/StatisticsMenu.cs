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
    class StatisticsMenu : Menu
    {
        private RelativeLayout layout;
        public override void Initialize(ContentManager Content)
        {
            layout = new RelativeLayout();
            Player d = new Player();
            Fleet f = new Fleet();
        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
