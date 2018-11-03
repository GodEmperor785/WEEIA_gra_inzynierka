using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client_PC.UI;
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
    }
}
