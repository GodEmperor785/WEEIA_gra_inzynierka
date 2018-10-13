using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client_PC.UI
{
    class Util
    {
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = SetColor(GetPosition(pixel, width, height), width, height);
                // data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);
            return texture;
        }

        private static Color SetColor(Point position, int width, int height)
        {
            
            if (position.X < 10)
            {
                int difference = position.X;
                if (position.Y < 10)
                {
                    int differenceY = position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 255);
                }
                else if (position.Y > height - 10)
                {
                    int differenceY = height - position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 255);
                }
                else
                {
                    return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4,255);
                }
            }
            else if (position.X > width - 10)
            {
                int difference = width - position.X ;
                if (position.Y > height - 10)
                {
                    int differenceY = height - position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 255);
                }
                else if (position.Y < 10)
                {
                    int differenceY = position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 255);
                }
                else
                {
                    return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4, 255);
                }
            }
            else if (position.Y < 10)
            {
                int difference = position.Y;
                return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4, 255);
            }
            else if (position.Y > height - 10)
            {
                int difference = height - position.Y;
                return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4, 255);
            }
            else
            {
                return new Color(100,100,100,255);
            }


        }

        private static Point GetPosition(int position, int width, int height)
        {
            int x = position % width;
            int y = position / width;
            Point pointPosition = new Point(x, y);
            return pointPosition;
        }
    }
}
