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
    class Map
    {
        Tile[,] m_world;
        Size m_mapSize;
        Tile m_voidTile;
        RenderTarget2D[] m_layerMap;
        SpriteBatch m_layerSpriteBatch;

        public Map(int tileWidth, int tileHeight, GraphicsDevice device, int tilePixSize, Size numTilesScreen,int nLayers)
        {
            m_layerSpriteBatch = new SpriteBatch(device);
            m_mapSize = new Size(tileWidth,tileHeight);
            m_voidTile = new Tile(Point.Zero, 0, nLayers);
            m_world = new Tile[tileWidth, tileHeight];
            m_layerMap = new RenderTarget2D[nLayers];
            for (int l = 0; l < nLayers; l++)
            {
                m_layerMap[l] = new RenderTarget2D(device, tilePixSize * (numTilesScreen.Width + 1), tilePixSize * (numTilesScreen.Height + 1));
            }

            for (int x = 0; x < m_mapSize.Width; x++)
            {
                for (int y = 0; y < m_mapSize.Height; y++)
                {
                    m_world[x, y] = new Tile(new Point(0, 0), -1, nLayers);
                }
            }
  
        }

        public int Height() { return m_mapSize.Height; }
        public int Width() { return m_mapSize.Width; }
        public Size GetMapSize() { return m_mapSize; }

        public Tile GetTile(int x, int y)
        {
            if (x >= m_mapSize.Width)
                return m_voidTile;
            if (x < 0)
                return m_voidTile;
            if (y >= m_mapSize.Height)
                return m_voidTile;
            if (y < 0)
                return m_voidTile;

            return m_world[x, y];

        }

        public void SetTile(int x, int y, Point tilePosition,int nTileMap,int nLayer)
        {
            if (x >= m_mapSize.Width)
                x = m_mapSize.Width - 1;
            if (x < 0)
                x = 0;
            if (y >= m_mapSize.Height)
                y = m_mapSize.Height - 1;
            if (y < 0)
                y = 0;
            m_world[x, y].tileLayers[nLayer].SetTile(tilePosition, nTileMap);
        }

        public void Draw(SpriteBatch spriteBatch, Size numTilesScreen, Point camPos, TileMaps tileMaps,Point screenOffset,Size tileScreenSize)
        {
            int tilePixelSize = tileMaps.TilePixSize();
            RenderLayerMaps(tilePixelSize, camPos, numTilesScreen, tileMaps);

            Rectangle drawRec = new Rectangle(screenOffset.X, screenOffset.Y, tileScreenSize.Width * numTilesScreen.Width, tileScreenSize.Height * numTilesScreen.Height);
            Rectangle sRec = new Rectangle(camPos.X % tilePixelSize, camPos.Y % tilePixelSize, numTilesScreen.Width * tilePixelSize, numTilesScreen.Height * tilePixelSize);
            for (int l = 0; l < m_layerMap.Length; l++)
            {
                spriteBatch.Draw(m_layerMap[l], drawRec, sRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f - (0.01f * l));
            }
     
        }

        void RenderLayerMaps(int tilePixSize, Point camPos, Size numTilesScreen, TileMaps tileMaps)
        {
            for (int l = 0; l < m_layerMap.Length; l++)
            {
                m_layerSpriteBatch.GraphicsDevice.SetRenderTarget(m_layerMap[l]);
                m_layerSpriteBatch.GraphicsDevice.Clear(Color.Transparent);
                m_layerSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                int tileSize = tilePixSize;
                Tile worldTile;
                int mapOffsetX = (camPos.X / tileSize) - (numTilesScreen.Width / 2);
                int mapOffsetY = (camPos.Y / tileSize) - (numTilesScreen.Height / 2);
                for (int x = 0; x < (numTilesScreen.Width + 1); x++)
                    for (int y = 0; y < (numTilesScreen.Height + 1); y++)
                    {
                        worldTile = GetTile(x + mapOffsetX, y + mapOffsetY);
                        Rectangle drawRec = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                        Rectangle tileRec = new Rectangle(worldTile.tileLayers[l].Position.X * tileSize, worldTile.tileLayers[l].Position.Y * tileSize, tileSize, tileSize);
                        if(worldTile.tileLayers[l].NumTileMap == -1)
                            m_layerSpriteBatch.Draw(tileMaps.GetEmptyTile(), drawRec, tileRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                        else 
                            m_layerSpriteBatch.Draw(tileMaps.GetTileMap(worldTile.tileLayers[l].NumTileMap), drawRec, tileRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                    }
                m_layerSpriteBatch.End();
            }
            m_layerSpriteBatch.GraphicsDevice.SetRenderTarget(null);
            m_layerSpriteBatch.GraphicsDevice.Clear(Color.Black);
        }
    }
}
