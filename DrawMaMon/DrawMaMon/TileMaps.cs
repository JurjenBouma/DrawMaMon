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
    class TileMaps
    {
        //Tile Size on Tilemap
        const int m_tilePixelSize = 16;
        Texture2D m_emptyTile;
        List<Texture2D> m_tileMaps;

        public TileMaps(Texture2D emptyTile, Size windowSize, Size numTilesScreen)
        {
            m_tileMaps = new List<Texture2D>();
            m_emptyTile = emptyTile;      
        }

        //return functions
        public int TilePixSize(){return m_tilePixelSize; }
        public Texture2D GetEmptyTile() { return m_emptyTile; }
        public void RemoveTileMap(int i) { m_tileMaps.RemoveAt(i); }
        public void AddTileMap(Texture2D tileMap) { m_tileMaps.Add(tileMap); }
        public int GetTileMapCount() { return m_tileMaps.Count; }
        public Texture2D GetTileMap(int i)
        {
            if (i >= m_tileMaps.Count)
                return m_emptyTile;

            return m_tileMaps[i];
        }
        public Size GetTileMapSize(int i)
        {
            Size size = new Size(m_tileMaps[i].Width, m_tileMaps[i].Height);
            return size;
        }
    }
}
