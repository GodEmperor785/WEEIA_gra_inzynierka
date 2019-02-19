using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TexturePackerLoader;

namespace Client_PC
{
    public class Animation
    {
        public enum Type { hit1, hit2, hit3, hit4, shield};
        private SpriteSheet sheet;
        private int CurrentFrame = 0;
        private int maxFrames;
        public TimeSpan diffferenceBetweenFrames = new TimeSpan(0,0,0,0,200);
        public Vector2 Position;
        private List<SpriteFrame> frames = new List<SpriteFrame>();

        public Animation(Type t)
        {
            if (t.Equals(Type.hit1))
            {
                var z = Game1.self.sheet.Sprites();
                foreach (var pair in z)
                {
                    if (pair.Key.Contains("hit1/hit1"))
                    {
                        frames.Add(pair.Value);
                    }
                }
            }
            else if (t.Equals(Type.hit2))
            {
                var z = Game1.self.sheet.Sprites();
                foreach (var pair in z)
                {
                    if (pair.Key.Contains("hit2/hit2"))
                    {
                        frames.Add(pair.Value);
                    }
                }
            }
            else if (t.Equals(Type.hit3))
            {
                var z = Game1.self.sheet.Sprites();
                foreach (var pair in z)
                {
                    if (pair.Key.Contains("hit3/hit3"))
                    {
                        frames.Add(pair.Value);
                    }
                }
            }
            else if (t.Equals(Type.hit4))
            {
                var z = Game1.self.hit4sheet.Sprites();
                foreach (var pair in z)
                {
                        frames.Add(pair.Value);
                }
            }
            else if (t.Equals(Type.shield))
            {
                var z = Game1.self.sheet.Sprites();
                foreach (var pair in z)
                {
                    if (pair.Key.Contains("shield1/shield1"))
                    {
                        frames.Add(pair.Value);
                    }
                }
            }

            maxFrames = frames.Count;
        }

        public bool Update()
        {
            if (CurrentFrame >= maxFrames)
                return true;
            else
                return false;
        }
        public void Draw(SpriteBatch sp)
        {
            if (CurrentFrame < maxFrames)
            {
                sp.Begin();
                Game1.self.renderer.Draw(frames[CurrentFrame], Position);
                sp.End();
                CurrentFrame++;
            }
        }

    }
}
