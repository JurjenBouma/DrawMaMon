using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DrawMaMon.Structs;
using System.IO;

namespace DrawMaMon
{
    class KeyInput
    {
        List<string> keyCountNameList;
        List<int> keyCountList;
        int m_keyDelay;

        KeyboardState prevKeyState;

        public KeyInput(int keyDelay)
        {
            prevKeyState = Keyboard.GetState();
            keyCountNameList = new List<string>();
            keyCountList = new List<int>();
            m_keyDelay = keyDelay;
        }

        public string AddTextInput(string inputString)
        {
            KeyboardState keyState = Keyboard.GetState();
            Keys[] pressedKeys = keyState.GetPressedKeys();
            string outPutString = inputString;
            bool caps = false;

            if (keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift))
                caps = true;

            foreach (Keys key in pressedKeys)
            {
                if (!keyCountNameList.Contains(key.ToString()))
                {
                    keyCountNameList.Add(key.ToString());
                    keyCountList.Add(0);
                }
                else if (prevKeyState.IsKeyDown(key))
                {
                    keyCountList[keyCountNameList.IndexOf(key.ToString())]++;
                }
                else if (prevKeyState.IsKeyUp(key))
                {
                    keyCountList[keyCountNameList.IndexOf(key.ToString())] = 0;
                }

                int keyCount = keyCountList[keyCountNameList.IndexOf(key.ToString())];

                if (keyCount == 0 || keyCount > m_keyDelay)
                {
                    if (key == Keys.Back)
                    {

                        if (outPutString.Length - 1 >= 0)
                        {
                            outPutString = outPutString.Remove(outPutString.Length - 1);
                        }
                    }
                    else if (key == Keys.Space)
                        outPutString += " ";
                    else if (key.ToString().Length == 1)
                    {
                        if (caps)
                            outPutString += key.ToString();
                        else
                            outPutString += key.ToString().ToLower();
                    }
                    else if (key == Keys.D0)
                        outPutString += "0";
                    else if (key == Keys.D1)
                        outPutString += "1";
                    else if (key == Keys.D2)
                        outPutString += "2";
                    else if (key == Keys.D3)
                        outPutString += "3";
                    else if (key == Keys.D4)
                        outPutString += "4";
                    else if (key == Keys.D5)
                        outPutString += "5";
                    else if (key == Keys.D6)
                        outPutString += "6";
                    else if (key == Keys.D7)
                        outPutString += "7";
                    else if (key == Keys.D8)
                        outPutString += "8";
                    else if (key == Keys.D9)
                        outPutString += "9";
                    else if (key == Keys.NumPad0)
                        outPutString += "0";
                    else if (key == Keys.NumPad1)
                        outPutString += "1";
                    else if (key == Keys.NumPad2)
                        outPutString += "2";
                    else if (key == Keys.NumPad3)
                        outPutString += "3";
                    else if (key == Keys.NumPad4)
                        outPutString += "4";
                    else if (key == Keys.NumPad5)
                        outPutString += "5";
                    else if (key == Keys.NumPad6)
                        outPutString += "6";
                    else if (key == Keys.NumPad7)
                        outPutString += "7";
                    else if (key == Keys.NumPad8)
                        outPutString += "8";
                    else if (key == Keys.NumPad9)
                        outPutString += "9";
                    else if (key == Keys.OemQuestion)
                        outPutString += "?";
                    else if (key == Keys.OemPlus)
                        outPutString += "+";
                    else if (key == Keys.OemBackslash)
                        outPutString += "\\";
                    else if (key == Keys.OemCloseBrackets)
                        outPutString += "]";
                    else if (key == Keys.OemComma)
                        outPutString += ",";
                    else if (key == Keys.OemMinus)
                        outPutString += "-";
                    else if (key == Keys.OemOpenBrackets)
                        outPutString += "[";
                    else if (key == Keys.OemQuotes)
                        outPutString += "\"";
                    else if (key == Keys.OemSemicolon)
                        outPutString += ";";
                    else if (key == Keys.OemPipe)
                        outPutString += "\\";
                    else if (key == Keys.OemTilde)
                        outPutString += "~";
                    else if (key == Keys.OemPeriod)
                        outPutString += ".";
                }
            }
            prevKeyState = keyState;
            return outPutString;
        }
    }
}
