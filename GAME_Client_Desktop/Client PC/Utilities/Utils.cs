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

        private static bool wallpaperChange = false;
        public static bool UpdateKeyboard(KeyboardState keyboardState, ref Keys[] lastPressedKeys)
        {
            
            bool update = false;
            if (Game1.self.IsActive)
            {
                var keys = keyboardState.GetPressedKeys();
                if (Game1.self.FocusedElement != null)
                {
                    if (Game1.self.FocusedElement is InputBox)
                    {
                        InputBox inputBox = (InputBox) Game1.self.FocusedElement;

                        foreach (var key in keys)
                        {
                            if (!lastPressedKeys.Contains(key))
                            {
                                if (inputBox.Text.Length < inputBox.TextLimit)
                                {

                                    if (UsableKeys.Contains(key))
                                    {
                                        if (keyboardState.IsKeyDown(Keys.LeftShift) ||
                                            keyboardState.IsKeyDown(Keys.RightShift))
                                        {
                                            inputBox.Text += ((char) key).ToString().ToUpper();
                                        }
                                        else
                                        {
                                            inputBox.Text += ((char) key).ToString().ToLower();
                                        }
                                    }


                                    if (key == Keys.Space)
                                    {
                                        inputBox.Text += " ";
                                    }

                                    if (key == Keys.Back)
                                    {
                                        if (inputBox.Text.Length > 0)
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



                    }
                }

                if (keyboardState.GetPressedKeys().Contains(Keys.F5) && !lastPressedKeys.Contains(Keys.F5))
                {

                    Game1.self.Wallpaper = Utils.CreateTexture(Game1.self.GraphicsDevice,
                        Game1.self.graphics.PreferredBackBufferWidth, Game1.self.graphics.PreferredBackBufferHeight);

                }

                if (keyboardState.GetPressedKeys().Contains(Keys.Delete) && !lastPressedKeys.Contains(Keys.Delete))
                {

                    update = true;

                }

                lastPressedKeys = keys;
            }

            return update;
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
            double minAddition = 2.56;
            double maxAddition = 1;
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                
                //the function applies the color according to the specified pixel
                
                if (pixel % width > 0 && data[pixel - 1] != empty)
                {
                    if (pixel - width > 0 && data[pixel - width] != empty) // inside
                    {

                        if (pixel - 2 * width > 0 && data[pixel - 2 * width] != empty && (pixel -2) % width > 0) // further rows inside
                        {
                            Color cl = new Color(
                                    rndRandom.Next((int)Math.Round((data[pixel - 1].R + data[pixel - width].R + data[pixel - 2].R + data[pixel -  2 * width].R + minAddition) / 4.0f - colorRange), (int)Math.Round((data[pixel - 1].R + data[pixel - width].R + data[pixel - 2].R + data[pixel - 2 * width].R + maxAddition) / 4.0f + colorRange)),
                                    rndRandom.Next((int)Math.Round((data[pixel - 1].G + data[pixel - width].G + data[pixel - 2].G + data[pixel - 2 * width].G + minAddition) / 4.0f - colorRange), (int)Math.Round((data[pixel - 1].G + data[pixel - width].G + data[pixel - 2].G + data[pixel - 2 * width].G + maxAddition) / 4.0f + colorRange)),
                                    rndRandom.Next((int)Math.Round((data[pixel - 1].B + data[pixel - width].B + data[pixel - 2].B + data[pixel - 2 * width].B + minAddition) / 4.0f - colorRange), (int)Math.Round((data[pixel - 1].B + data[pixel - width].B + data[pixel - 2].B + data[pixel - 2 * width].B + maxAddition) / 4.0f + colorRange)))
                                ;
                            data[pixel] = cl;
                        }
                        else // first row inside
                        {
                            Color cl = new Color(
                                rndRandom.Next((int)Math.Floor((data[pixel - 1].R + data[pixel - width].R) / 2.0f - colorRange), (int)Math.Ceiling((data[pixel - 1].R + data[pixel - width].R) / 2.0f + colorRange)), 
                                rndRandom.Next((int)Math.Floor((data[pixel - 1].G + data[pixel - width].G) / 2.0f - colorRange), (int)Math.Ceiling((data[pixel - 1].G + data[pixel - width].G) / 2.0f + colorRange)), 
                                rndRandom.Next((int)Math.Floor((data[pixel - 1].B + data[pixel - width].B) / 2.0f - colorRange), (int)Math.Ceiling((data[pixel - 1].B + data[pixel - width].B) / 2.0f + colorRange)))
                                ;
                            data[pixel] = cl;
                            
                        }
                    }
                    else // top edge
                    {



                        Color cl = new Color(
                            rndRandom.Next(data[pixel - 1].R - colorRange, data[pixel - 1].R + colorRange), 
                            rndRandom.Next(data[pixel - 1].G - colorRange, data[pixel - 1].G + colorRange), 
                            rndRandom.Next(data[pixel - 1].B - colorRange, data[pixel - 1].B + colorRange)
                            );
                        data[pixel] = cl;
                    }
                }
                else if(pixel - width >= 0 && data[pixel - width] != empty) // left edge
                {
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
