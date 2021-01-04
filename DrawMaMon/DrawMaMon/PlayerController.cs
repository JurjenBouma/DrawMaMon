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

namespace DrawMaMon
{
    class PlayerController
    {
        bool m_keyDownWasPressed = false;
        bool m_keyUpWasPressed = false;
        bool m_keyRightWasPressed = false;
        bool m_keyLeftWasPressed = false;
        int m_clickTimerRight = 0;
        int m_clickTimerLeft = 0;
        int m_clickTimerUp = 0;
        int m_clickTimerDown = 0;
        int m_buttonDelay;
        int m_stepCounter;

        public PlayerController(int tileSizePixels, int buttonDelay)
        {
            m_stepCounter= tileSizePixels;
            m_buttonDelay = buttonDelay;
        }

        public void HandleInput(Player player, Camera cam, int tileSizePixels, Size numTilesScreen, int mapWidth, int mapHeight,Network network)
        {
            //If player != walking
            if (m_stepCounter == tileSizePixels)
            {
                HandleInput(player, tileSizePixels, network);
            }
            else
            {
                m_stepCounter++;
                cam.MoveCamera(player.GetPosition(), tileSizePixels, numTilesScreen.Width, numTilesScreen.Height, mapWidth, mapHeight);
            }
        }

        void HandleInput(Player player, int tilePixSize, Network network)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (!m_keyDownWasPressed && !m_keyLeftWasPressed && !m_keyUpWasPressed)
                {
                    if (m_clickTimerRight == m_buttonDelay)
                    {
                        m_keyRightWasPressed = true;
                        m_stepCounter = 0;
                        player.WalkTile(new Point(1, 0), tilePixSize);
                        network.SendPlayerWalk(new Point(1, 0));

                    }
                    else
                    {
                        player.SetDisplayTile(new Point(1, 2));
                        network.SendPlayerDisplayTile(player.GetDisplayTile());
                        m_clickTimerRight++;
                    }
                }
            }
            else
            {
                m_clickTimerRight = 0;
                m_keyRightWasPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (!m_keyDownWasPressed && !m_keyRightWasPressed && !m_keyUpWasPressed)
                {
                    if (m_clickTimerLeft == m_buttonDelay)
                    {
                        m_keyLeftWasPressed = true;
                        m_stepCounter = 0;
                        player.WalkTile(new Point(-1, 0), tilePixSize);
                        network.SendPlayerWalk(new Point(-1, 0));

                    }
                    else
                    {
                        player.SetDisplayTile(new Point(1, 1));
                        network.SendPlayerDisplayTile(player.GetDisplayTile());

                        m_clickTimerLeft++;
                    }
                }
            }
            else
            {
                m_clickTimerLeft = 0;
                m_keyLeftWasPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (!m_keyLeftWasPressed && !m_keyRightWasPressed && !m_keyDownWasPressed)
                {
                    if (m_clickTimerUp == m_buttonDelay)
                    {
                        m_keyUpWasPressed = true;
                        m_stepCounter = 0;
                        player.WalkTile(new Point(0, -1), tilePixSize);
                        network.SendPlayerWalk(new Point(0, -1));
                    }
                    else
                    {
                        player.SetDisplayTile(new Point(1, 3));
                        network.SendPlayerDisplayTile(player.GetDisplayTile());
                        m_clickTimerUp++;
                    }
                }
            }
            else
            {
                m_clickTimerUp = 0;
                m_keyUpWasPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (!m_keyLeftWasPressed && !m_keyRightWasPressed && !m_keyUpWasPressed)
                {
                    if (m_clickTimerDown == m_buttonDelay)
                    {
                        m_keyDownWasPressed = true;
                        m_stepCounter = 0;
                        player.WalkTile(new Point(0, 1), tilePixSize);
                        network.SendPlayerWalk(new Point(0, 1));

                    }
                    else
                    {
                        m_clickTimerDown++;
                        player.SetDisplayTile(new Point(1, 0));
                        network.SendPlayerDisplayTile(player.GetDisplayTile());
                    }
                }
            }
            else
            {
                m_clickTimerDown = 0;
                m_keyDownWasPressed = false;
            }
        }
    }
}
