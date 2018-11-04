using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client_PC.Utilities
{
    class Utils
    {
        private static Keys[] UsableKeys =
        {
            Keys.D1,Keys.D2,Keys.D3,Keys.D4,Keys.D5,Keys.D6,Keys.D7,Keys.D8,Keys.D9,Keys.D0,
            Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P,
            Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L,
            Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M
        };
        public static void UpdateKeyboard(KeyboardState keyboardState, ref Keys[] lastPressedKeys)
        {
            if(Game1.self.FocusedElement != null)
                if (Game1.self.FocusedElement is InputBox)
                {
                    InputBox inputBox =(InputBox) Game1.self.FocusedElement;
                    var keys =  keyboardState.GetPressedKeys();
                    foreach (var key in keys)
                    {
                        if (!lastPressedKeys.Contains(key))
                        {
                            if (inputBox.Text.Length < inputBox.TextLimit)
                            {
                                if (UsableKeys.Contains(key))
                                {
                                    if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                                        inputBox.Text += ((char)key).ToString().ToUpper();
                                    else
                                        inputBox.Text += ((char)key).ToString().ToLower();
                                }
                                if (key == Keys.Space)
                                {
                                    inputBox.Text += " ";
                                }

                                if (key == Keys.Back)
                                {
                                    if(inputBox.Text.Length > 0)
                                        inputBox.Text = inputBox.Text.Substring(0, inputBox.Text.Length - 1);
                                }
                            }
                            else if (key == Keys.Back)
                            {
                                if (inputBox.Text.Length > 0)
                                    inputBox.Text = inputBox.Text.Substring(0, inputBox.Text.Length - 1);
                            }
                        }

                    }

                    lastPressedKeys = keys;

                }
        }

        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height)
        {
            //initialize a texture
            Random rndRandom = new Random();
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            Color empty = new Color();
            int colorRange = 5;
            int startMin = 0;
            int startMax = 255;
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                
                //the function applies the color according to the specified pixel
                // top left corner
                if (pixel % width > 0 && data[pixel - 1] != empty)
                {
                    if (pixel - width > 0 && data[pixel - width] != empty)
                    {
                        Color cl = new Color(
                            rndRandom.Next((data[pixel - 1].R + data[pixel - width].R) / 2 - colorRange, (data[pixel - 1].R + data[pixel - width].R) / 2 + colorRange), 
                            rndRandom.Next((data[pixel - 1].G + data[pixel - width].G) / 2 - colorRange, (data[pixel - 1].G + data[pixel - width].G) / 2 + colorRange), 
                            rndRandom.Next((data[pixel - 1].B + data[pixel - width].B) / 2 - colorRange, (data[pixel - 1].B + data[pixel - width].B) / 2 + colorRange))
                            ;
                        data[pixel] = cl;
                    }
                    else
                    {
                        Color cl = new Color(rndRandom.Next(data[pixel - 1].R - colorRange , data[pixel - 1].R + colorRange), rndRandom.Next(data[pixel - 1].G - colorRange, data[pixel - 1].G + colorRange), rndRandom.Next(data[pixel - 1].B - colorRange, data[pixel - 1].B + colorRange));
                        data[pixel] = cl;
                    }
                }
                else if(pixel - width >= 0 && data[pixel - width] != empty)
                {
                    Color first = new Color(rndRandom.Next(startMin, startMax), rndRandom.Next(startMin, startMax), rndRandom.Next(startMin, startMax));
                   /*
                     Color cl = new Color(
                    
                        rndRandom.Next((data[pixel - width].R + first.R) / 2 - colorRange, (data[pixel - width].R + first.R) / 2 + colorRange), 
                        rndRandom.Next((data[pixel - width].G + first.G) / 2 - colorRange, (data[pixel - width].G + first.G) / 2 + colorRange), 
                        rndRandom.Next((data[pixel - width].B + first.B) / 2 - colorRange, (data[pixel - width].B + first.B) / 2 + colorRange));

                     */
                    Color cl = new Color(
                        rndRandom.Next((data[pixel - width].R) - colorRange, (data[pixel - width].R) + colorRange),
                        rndRandom.Next((data[pixel - width].G) - colorRange, (data[pixel - width].G) + colorRange),
                        rndRandom.Next((data[pixel - width].B) - colorRange, (data[pixel - width].B) + colorRange));

                    data[pixel] = cl;
                }
                else //  first pixel
                {
                    Color cl = new Color(rndRandom.Next(startMin, startMax), rndRandom.Next(startMin, startMax), rndRandom.Next(startMin, startMax));
                    data[pixel] = cl;
                }
                




                // data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);
            return texture;
        }
    }
}
