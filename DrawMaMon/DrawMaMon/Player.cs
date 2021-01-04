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
    class Player
    {
        Point m_playerPosition;
        Texture2D m_playerTex;
        Size m_playerTilePixSize;
        Point m_displayTile;
        Animation m_aniWalkDown;
        Animation m_aniWalkLeft;
        Animation m_aniWalkRight;
        Animation m_aniWalkUp;
        int m_animationCounterDown = 0;
        int m_animationCounterLeft = 0;
        int m_animationCounterRight = 0;
        int m_animationCounterUp = 0;
        Point m_walkDirection;
        int m_stepCounterLeft;
        int m_stepCounterRight;
        int m_stepCounterUp;
        int m_stepCounterDown;
        bool m_mustReset;

        public int iD;

        public Player(Point playerPos, Texture2D playerTex, Size pixelSizePlayerTile,int tileSizePixels)
        {
            m_playerTex = playerTex;
            m_playerTilePixSize = pixelSizePlayerTile;
            m_displayTile = new Point(1, 0);
            m_playerPosition = new Point(playerPos.X, playerPos.Y);
            m_aniWalkDown = new Animation(new Point(1, 0), false, 2, 14);
            m_aniWalkLeft = new Animation(new Point(1, 1), false, 2, 14);
            m_aniWalkRight = new Animation(new Point(1, 2), false, 2, 14);
            m_aniWalkUp = new Animation(new Point(1, 3), false, 2, 14);
            m_stepCounterLeft = tileSizePixels;
            m_stepCounterUp = tileSizePixels;
            m_stepCounterDown = tileSizePixels;
            m_stepCounterRight = tileSizePixels;
            m_walkDirection = Point.Zero;
        }

        public Texture2D GetTexture() { return m_playerTex; }
        public void SetTexture(Texture2D tex) { m_playerTex = tex; }
        public Size GetPlayerTileSize() { return m_playerTilePixSize; }
        public Point GetPosition() { return m_playerPosition; }
        public void SetPosition(Point position) { m_playerPosition = position; }
        public Point GetDisplayTile() { return m_displayTile; }
        public void SetDisplayTile(Point tile) { m_displayTile = tile; }

        public void Update(int tileSizePixels, int screenTileNumberX, int screenTileNumberY, int mapWidth, int mapHeight)
        {
            if (m_stepCounterLeft < tileSizePixels)
            {
                Walk(new Point(-1, 0), tileSizePixels, screenTileNumberX, screenTileNumberY, mapWidth, mapHeight);
                m_stepCounterLeft++;
            }
            else if (m_stepCounterRight < tileSizePixels)
            {
                Walk(new Point(1, 0), tileSizePixels, screenTileNumberX, screenTileNumberY, mapWidth, mapHeight);
                m_stepCounterRight++;
            }
            else if (m_stepCounterUp < tileSizePixels)
            {
                Walk(new Point(0, -1), tileSizePixels, screenTileNumberX, screenTileNumberY, mapWidth, mapHeight);
                m_stepCounterUp++;
            }
            else if (m_stepCounterDown < tileSizePixels)
            {
                Walk(new Point(0, 1), tileSizePixels, screenTileNumberX, screenTileNumberY, mapWidth, mapHeight);
                m_stepCounterDown++;
            }
            else if (m_mustReset)
            {
                ResetAnimations(tileSizePixels);
                m_mustReset = false;
            }
        }

        void Walk(Point Direction, int tileMapPixSize, int screenTileNumberX, int screenTileNumberY, int mapWidth, int mapHeight)
        {
            if (m_walkDirection.X != 0 || m_walkDirection.Y != 0)
            {
                if (m_playerPosition.X + Direction.X >= 0 && m_playerPosition.X + Direction.X < ((mapWidth - 1) * tileMapPixSize))
                    m_playerPosition.X += Direction.X;

                if (m_playerPosition.Y + Direction.Y >= 0 && m_playerPosition.Y + Direction.Y < ((mapHeight - 1) * tileMapPixSize))
                    m_playerPosition.Y += Direction.Y;

                if (Direction.X > 0)
                    PlayAnimationWalkRight();
                else if (Direction.X < 0)
                    PlayAnimationWalkLeft();
                else if (Direction.Y > 0)
                    PlayAnimationWalkDown();
                else if (Direction.Y < 0)
                    PlayAnimationWalkUp();
            }
        }

        public void WalkTile(Point walkDirection, int tileSizePixels)
        {
            if (walkDirection.X > 0)
                m_stepCounterRight -= tileSizePixels;
            else if (walkDirection.X < 0)
                m_stepCounterLeft -= tileSizePixels;
            else if (walkDirection.Y > 0)
                m_stepCounterDown -= tileSizePixels;
            else if (walkDirection.Y < 0)
                m_stepCounterUp -= tileSizePixels;
           
            m_walkDirection = walkDirection;
            m_mustReset = true;
        }

        public void ResetAnimations(int tileSizePixels)
        {
            //reset player to standing position
            if (m_walkDirection.X > 0)
                SetDisplayTile(new Point(1, 2));
            else if (m_walkDirection.X < 0)
                SetDisplayTile(new Point(1, 1));
            else if (m_walkDirection.Y > 0)
                SetDisplayTile(new Point(1, 0));
            else if (m_walkDirection.Y < 0)
                SetDisplayTile(new Point(1, 3));
            m_walkDirection = Point.Zero;

            m_animationCounterDown = m_aniWalkDown.GetSpeed();
            m_animationCounterLeft = m_aniWalkDown.GetSpeed();
            m_animationCounterRight = m_aniWalkDown.GetSpeed();
            m_animationCounterUp = m_aniWalkDown.GetSpeed();
            m_aniWalkDown.SetState(1);
            m_aniWalkLeft.SetState(1);
            m_aniWalkRight.SetState(1);
            m_aniWalkUp.SetState(1);
        }

        void PlayAnimationWalkDown()
        {
            if (m_animationCounterDown == m_aniWalkDown.GetSpeed())
            {
                m_aniWalkDown.Progress();
                m_displayTile = m_aniWalkDown.GetState();
                m_animationCounterDown = 0;
            }
            m_animationCounterDown++;
        }

        void PlayAnimationWalkLeft()
        {
            if (m_animationCounterLeft == m_aniWalkLeft.GetSpeed())
            {
                m_aniWalkLeft.Progress();
                m_displayTile = m_aniWalkLeft.GetState();
                m_animationCounterLeft = 0;
            }
            m_animationCounterLeft++;
        }

        void PlayAnimationWalkRight()
        {
            if (m_animationCounterRight == m_aniWalkRight.GetSpeed())
            {
                m_aniWalkRight.Progress();
                m_displayTile = m_aniWalkRight.GetState();
                m_animationCounterRight = 0;
            }
            m_animationCounterRight++;
        }

        void PlayAnimationWalkUp()
        {
            if (m_animationCounterUp == m_aniWalkUp.GetSpeed())
            {
                m_aniWalkUp.Progress();
                m_displayTile = m_aniWalkUp.GetState();
                m_animationCounterUp = 0;
            }
            m_animationCounterUp++;
        }

        public void Draw(SpriteBatch spriteBatch, Size tileScreenSize, int tilePixSize, int nLayers, Size windowSize, Camera cam)
        {

            float magWidth = ((float)tileScreenSize.Width / tilePixSize);
            float magHeight = ((float)tileScreenSize.Height / tilePixSize);
            int tileDifferenceWidth = (int)((tilePixSize - m_playerTilePixSize.Width) * magWidth);
            int tileDifferenceHeight = (int)((tilePixSize - m_playerTilePixSize.Height) * magHeight);
            int posX = (windowSize.Width / 2) - (tileScreenSize.Width / 2);
            posX -= (int)((cam.Position().X - m_playerPosition.X) * magWidth);
            int posY = (windowSize.Height / 2) - (tileScreenSize.Height / 2);
            posY -= (int)((cam.Position().Y - m_playerPosition.Y) * magHeight);
            posX += tileDifferenceWidth;
            posY += tileDifferenceHeight;
            if (posX > -m_playerTilePixSize.Width* magWidth && posX < windowSize.Width)
            {
                if (posY > -m_playerTilePixSize.Height * magHeight && posY < windowSize.Height)
                {
                    Rectangle drawRec = new Rectangle(posX, posY , (int)(m_playerTilePixSize.Width * magWidth), (int)(m_playerTilePixSize.Height * magHeight));
                    Rectangle tileRec = new Rectangle(m_displayTile.X * m_playerTilePixSize.Width, m_displayTile.Y * m_playerTilePixSize.Height, m_playerTilePixSize.Width, m_playerTilePixSize.Height);
                    spriteBatch.Draw(m_playerTex, drawRec, tileRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f - (0.01f * ((nLayers + 1) / 2)) + 0.005f);
                }
            }
        }
    }
}
