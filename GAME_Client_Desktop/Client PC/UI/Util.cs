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
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint,int bordersize = 10,int alpha = 255)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = SetColor(GetPosition(pixel, width, height), width, height, false, bordersize, alpha);
                // data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);
            return texture;
        }
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint, Color brighter, Color darker, int bordersize = 10, int alpha = 255)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = NewSetColor(GetPosition(pixel, width, height), width, height, false, bordersize, alpha, brighter, darker);
                // data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);
            return texture;
        }
        public static Texture2D CreateTextureHollow(GraphicsDevice device, int width, int height, Func<int, Color> paint,int bordersize = 10,int alpha = 255)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = SetColor(GetPosition(pixel, width, height), width, height, true, bordersize,alpha);
                // data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);
            return texture;
        }
        public static Texture2D CreateTextureHollow(GraphicsDevice device, int width, int height, Func<int, Color> paint, Color brighter, Color darker, int bordersize = 10, int alpha = 255)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = NewSetColor(GetPosition(pixel, width, height), width, height, true, bordersize, alpha, brighter, darker);
                // data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);
            return texture;
        }
        private static Color SetColor(Point position, int width, int height, bool hollow, int bordersize,int alpha)
        {
            
            if (position.X < bordersize)
            {
                int difference = position.X;
                if (position.Y < bordersize)
                {
                    int differenceY = position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, alpha);
                }
                else if (position.Y > height - bordersize)
                {
                    int differenceY = height - position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, alpha);
                }
                else
                {
                    return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4, alpha);
                }
            }
            else if (position.X > width - bordersize)
            {
                int difference = width - position.X ;
                if (position.Y > height - bordersize)
                {
                    int differenceY = height - position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, alpha);
                }
                else if (position.Y < bordersize)
                {
                    int differenceY = position.Y;
                    return new Color(180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, 180 - differenceY * 4 - difference * 4, alpha);
                }
                else
                {
                    return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4, alpha);
                }
            }
            else if (position.Y < bordersize)
            {
                int difference = position.Y;
                return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4, alpha);
            }
            else if (position.Y > height - bordersize)
            {
                int difference = height - position.Y;
                return new Color(140 - difference * 4, 140 - difference * 4, 140 - difference * 4, alpha);
            }
            else
            {
                if (hollow)
                {
                     return new Color(240, 240, 240, alpha);
                }
                else
                {
                    return new Color(104, 104, 104, alpha);
                    
                }
            }


        }

        private static Color NewSetColor(Point position, int width, int height, bool hollow, int bordersize, int alpha,Color brighter, Color darker)
        {
            int RDifference = (brighter.R - darker.R) / 20;
            int GDifference = (brighter.G - darker.G) / 20;
            int BDifference = (brighter.B - darker.B) / 20;

            int RMedium = (brighter.R + darker.R) / 2;
            int GMedium = (brighter.G + darker.G) / 2;
            int BMedium = (brighter.B + darker.B) / 2;

            if (position.X < bordersize)
            {
                int difference = position.X;
                if (position.Y < bordersize)
                {
                    int differenceY = position.Y;
                    return new Color(brighter.R - differenceY * RDifference - difference * RDifference, brighter.G - differenceY * GDifference - difference * GDifference, brighter.B - differenceY * BDifference - difference * BDifference, alpha);
                }
                else if (position.Y > height - bordersize)
                {
                    int differenceY = height - position.Y;
                    return new Color(brighter.R - differenceY * RDifference - difference * RDifference, brighter.G - differenceY * GDifference - difference * GDifference, brighter.B - differenceY * BDifference - difference * BDifference, alpha);
                }
                else
                {
                    return new Color(RMedium - difference * RDifference, GMedium - difference * GDifference, BMedium - difference * BDifference, alpha);
                }
            }
            else if (position.X > width - bordersize)
            {
                int difference = width - position.X;
                if (position.Y > height - bordersize)
                {
                    int differenceY = height - position.Y;
                    return new Color(brighter.R - differenceY * RDifference - difference * RDifference, brighter.G - differenceY * GDifference - difference * GDifference, brighter.B - differenceY * BDifference - difference * BDifference, alpha);
                }
                else if (position.Y < bordersize)
                {
                    int differenceY = position.Y;
                    return new Color(brighter.R - differenceY * RDifference - difference * RDifference, brighter.G - differenceY * GDifference - difference * GDifference, brighter.B - differenceY * BDifference - difference * BDifference, alpha);
                }
                else
                {
                    return new Color(RMedium - difference * RDifference, GMedium - difference * GDifference, BMedium - difference * BDifference, alpha);
                }
            }
            else if (position.Y < bordersize)
            {
                int difference = position.Y;
                return new Color(RMedium - difference * RDifference, GMedium - difference * GDifference, BMedium - difference * BDifference, alpha);
            }
            else if (position.Y > height - bordersize)
            {
                int difference = height - position.Y;
                return new Color(RMedium - difference * RDifference, GMedium - difference * GDifference, BMedium - difference * BDifference, alpha);
            }
            else
            {
                if (hollow)
                {
                    return new Color(240, 240, 240, alpha);
                }
                else
                {
                    return new Color(darker.R, darker.G, darker.B, alpha);

                }
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
