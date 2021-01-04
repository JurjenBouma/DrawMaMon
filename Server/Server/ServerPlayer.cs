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
    class ServerPlayer
    {
        Point m_playerPosition;
        Size m_tilePixSize;
        Point m_displayTile;
        Point m_lastWalkDir;

        public ServerPlayer(Point playerPos,Point displayTile, Size pixelSizeTile)
        {       
            m_tilePixSize = pixelSizeTile;
            m_displayTile = displayTile;
            m_playerPosition = new Point(playerPos.X,playerPos.Y);
        }

        public Size GetTileSize() { return m_tilePixSize; }
        public void SetTileSize(Size tileSize) { m_tilePixSize = tileSize; }
        public Point GetPosition() { return m_playerPosition; }
        public void SetPosition(Point position) {  m_playerPosition = position;}
        public Point GetDisplayTile() { return m_displayTile; }
        public void SetDisplayTile(Point tile) {m_displayTile = tile; }
        public Point GetLastWalkDir() { return m_lastWalkDir; }
        public void SetLastWalkDir(Point direction) { m_lastWalkDir = direction; }
    }
}
