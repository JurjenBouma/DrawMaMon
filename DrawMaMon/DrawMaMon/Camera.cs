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

namespace DrawMaMon
{
    class Camera
    {
        Point m_position;

        public Camera(Point pos)
        {
            m_position = pos;
        }
        public Point Position() { return m_position; }

        public void MoveCamera(Point pos, int tilePixSize, int nTilesX, int nTilesY, int mapWidth, int mapHeight)
        {
            //Check for minimum
            if (pos.X > ((nTilesX / 2) * tilePixSize))
                m_position.X = pos.X;
            else
                m_position.X = ((nTilesX / 2) * tilePixSize);

            if (pos.Y > ((nTilesY / 2) * tilePixSize))
                m_position.Y = pos.Y;
            else
                m_position.Y = ((nTilesY / 2) * tilePixSize);

            //Check for maximum
            if (pos.X > (((mapWidth) * tilePixSize) - (((nTilesX+1) / 2) * tilePixSize))-1)
                m_position.X = ((mapWidth) * tilePixSize) - (((nTilesX+1) / 2) * tilePixSize) -1;
            
            if (pos.Y > (((mapHeight) * tilePixSize) - (((nTilesY+1) / 2) * tilePixSize))-1)
                m_position.Y = ((mapHeight) * tilePixSize) - (((nTilesY+1) / 2) * tilePixSize) -1;
            
        }
    }
}
