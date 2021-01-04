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
    partial class TileToolBar
    {
      
        public Texture2D GetBackGround() { return m_backGroundTexture; }
        public Point GetActiveTile() { return m_activeTile; }
        public Size GetToolBarSize() { return m_toolBarSize; }
        public Point GetToolBarLocation(){ return m_toolBarLocation;}
        public int GetActiveTileMap() { return m_nActiveTileMap; }
        public int GetActivelayer() { return m_activeLayer; }
        public bool IsReady() { return m_Ready; }

        public void Draw(SpriteBatch spriteBatch,TileMaps tileMaps, int nLayers)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            Rectangle drawRec = new Rectangle(m_toolBarLocation.X, m_toolBarLocation.Y, m_toolBarSize.Width, m_toolBarSize.Height);
            spriteBatch.Draw(m_backGroundTexture, drawRec,null, Color.White,0.0f,Vector2.Zero,SpriteEffects.None,0.7f);
            spriteBatch.End();
            m_tileMapSelecter.Draw(spriteBatch,0.68f);
            m_openTileMapButton.Draw(spriteBatch, Color.Gold,0.69f);
            m_deleteTileMapButton.Draw(spriteBatch, Color.White,0.69f);
            DrawTileBox(spriteBatch, tileMaps);
            DrawLayerBox(spriteBatch,nLayers);
            m_hScrollBarTileBox.Draw(spriteBatch,0.68f);
            m_vScrollBarTileBox.Draw(spriteBatch,0.68f);
            m_openFileWindow.Draw(spriteBatch);
        }

        void DrawLayerBox(SpriteBatch spriteBatch, int nLayers)
        {
            for (int i = 0; i < (nLayers + 1) / 2; i++)
            {
                Color color = Color.CornflowerBlue;
                if (i == m_activeLayer)
                    color = Color.White;

                m_floorLayerButtons[i].Draw(spriteBatch, color,0.69f);
            }

            for (int i = 0; i < nLayers/ 2; i++)
            {
                Color color = Color.RoyalBlue;
                if (i + (nLayers + 1) / 2 == m_activeLayer)
                    color = Color.White;

                m_roofLayerButtons[i].Draw(spriteBatch, color,0.68f);
            }
        }
     
        void DrawTileBox(SpriteBatch spriteBatch,TileMaps tileMaps)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            Texture2D tileMap = tileMaps.GetTileMap(m_nActiveTileMap);
            Rectangle boxRec = new Rectangle(m_toolBarLocation.X + m_tileBoxLocation.X - m_tileBoxBorderSize, m_toolBarLocation.Y + m_tileBoxLocation.Y - m_tileBoxBorderSize, m_tileBoxSize.Width + m_tileBoxBorderSize * 2, m_tileBoxSize.Height + m_tileBoxBorderSize*2);
            spriteBatch.Draw(m_tileBoxTexture, boxRec,null, Color.White,0.0f,Vector2.Zero,SpriteEffects.None,0.68f);
            Rectangle cropRec = new Rectangle(m_hScrollBarTileBox.GetValue()*tileMaps.TilePixSize(), m_vScrollBarTileBox.GetValue()*tileMaps.TilePixSize(), m_activeTileMapCropSize.Width, m_activeTileMapCropSize.Height);
            Rectangle drawRec = new Rectangle(m_toolBarLocation.X + m_tileBoxLocation.X, m_toolBarLocation.Y + m_tileBoxLocation.Y, m_activeTileMapCropSize.Width * m_tileZoom, m_activeTileMapCropSize.Height * m_tileZoom);
            spriteBatch.Draw(tileMap, drawRec, cropRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.67f);
            spriteBatch.End();
        }

        void ConfigurateTileMapSelecter()
        {
            m_tileMapSelecter.SetMaxValue(m_tileMaps.GetTileMapCount() - 1);
            m_tileMapSelecter.SetValue(m_tileMaps.GetTileMapCount() - 1);
            m_nActiveTileMap = m_tileMapSelecter.GetValue();
        }

        void CalculateCropBounds(Texture2D tileMap)
        {
            m_activeTileMapCropSize = new Size(0, 0);
            if (tileMap.Width * m_tileZoom < m_tileBoxSize.Width)
                m_activeTileMapCropSize.Width = tileMap.Width;
            else
                m_activeTileMapCropSize.Width  = m_tileBoxSize.Width / m_tileZoom;

            if (tileMap.Height * m_tileZoom < m_tileBoxSize.Height)
                m_activeTileMapCropSize.Height = tileMap.Height;
            else
                m_activeTileMapCropSize.Height = m_tileBoxSize.Height / m_tileZoom;
        }

        void ConfigurateScrollBars(int tilePixSize)
        {
            if (m_activeTileMapCropSize.Width == m_tileBoxSize.Width / m_tileZoom)
            {
                m_hScrollBarTileBox.Show();
                int maxValue = (m_activeTileMapSize.Width - m_activeTileMapCropSize.Width) / tilePixSize;
                m_hScrollBarTileBox.SetMaxValue(maxValue);
            }
            else
            {
                m_hScrollBarTileBox.Hide();
                int maxValue = (m_activeTileMapSize.Width - m_activeTileMapCropSize.Width) / tilePixSize;
                m_hScrollBarTileBox.SetMaxValue(maxValue);
            }

            if (m_activeTileMapCropSize.Height == m_tileBoxSize.Height / m_tileZoom)
            {
                m_vScrollBarTileBox.Show();
                int maxValue = (m_activeTileMapSize.Height - m_activeTileMapCropSize.Height) / tilePixSize;
                m_vScrollBarTileBox.SetMaxValue(maxValue);
            }
            else
            {
                m_vScrollBarTileBox.Hide();
                int maxValue = (m_activeTileMapSize.Height - m_activeTileMapCropSize.Height) / tilePixSize;
                m_vScrollBarTileBox.SetMaxValue(maxValue);
            }
        }

        public void HandleInput(int tileSize, int nLayers,TileMaps tileMaps,Map currentMap)
        {
            HandleInputTileBox(tileSize);
            HandleInputLayerBox(nLayers);
            HandleInputTileMapSelecter(tileMaps);
            HandleInputOpenButton();
            HandleInputOpenFileWindow();
            HandleInputDeleteButton(currentMap,nLayers);
            HandleInputScrollBars();
            m_prevMouseState = Mouse.GetState();
        }

        void HandleInputScrollBars()
        {
            m_hScrollBarTileBox.HandleInput();
            m_vScrollBarTileBox.HandleInput();
        }

        void HandleInputOpenFileWindow()
        {
            MouseState mouseState = Mouse.GetState();
            if(!m_Ready)
                if (!m_openFileWindow.IsVisible() && m_tileMaps.GetTileMapCount() > 0 && mouseState.LeftButton != m_prevMouseState.LeftButton)
                m_Ready = true;
            m_openFileWindow.HandleInput();
        }

        void HandleInputDeleteButton(Map currentMap,int nLayers)
        {
            MouseState mouseState = Mouse.GetState();
            if (m_deleteTileMapButton.IsClicked() && m_prevMouseState.LeftButton != mouseState.LeftButton)
            {
                if (m_tileMaps.GetTileMapCount() - 1 >= 0)
                {
                    for (int x = 0; x < currentMap.Width(); x++)
                    {
                        for (int y = 0; y < currentMap.Height(); y++)
                        {
                           Tile tile = currentMap.GetTile(x, y);
                           for (int l = 0; l < nLayers; l++)
                           {
                               if (tile.tileLayers[l].NumTileMap == m_tileMapSelecter.GetValue())
                               {
                                   tile.tileLayers[l].NumTileMap = -1;
                                   tile.tileLayers[l].Position = Point.Zero;
                               }
                               if (tile.tileLayers[l].NumTileMap > m_tileMapSelecter.GetValue())
                               {
                                   tile.tileLayers[l].NumTileMap--;
                               }
                           }
                        }
                    }
                    m_tileMaps.RemoveTileMap(m_tileMapSelecter.GetValue());
                    Refresh();
                }
            }
        }

        void HandleInputOpenButton()
        {
            MouseState mouseState = Mouse.GetState();
            if (m_openTileMapButton.IsClicked() && m_prevMouseState.LeftButton != mouseState.LeftButton)
            {
                m_Ready = false;
                m_openFileWindow.Show();
            }
        }

        void HandleInputTileMapSelecter(TileMaps tileMaps)
        {
            m_tileMapSelecter.HandleInput();
            if (m_nActiveTileMap != m_tileMapSelecter.GetValue())
            {
                m_nActiveTileMap = m_tileMapSelecter.GetValue();
                Texture2D tileMap = tileMaps.GetTileMap(m_nActiveTileMap);
                m_activeTileMapSize = new Size(tileMap.Width, tileMap.Height);
                CalculateCropBounds(tileMap);
                ConfigurateScrollBars(tileMaps.TilePixSize());
            }
        }

        void HandleInputLayerBox(int nLayers)
        {
            for (int i = 0; i < (nLayers + 1) / 2; i++)
            {
                if (m_floorLayerButtons[i].IsClicked())
                {
                    m_activeLayer = i;
                }
            }

            for (int i = 0; i < nLayers / 2; i++)
            {
                if (m_roofLayerButtons[i].IsClicked())
                {
                    m_activeLayer = i + (nLayers + 1) / 2;
                }
            }
        }

        void HandleInputTileBox(int tileSize)
        {
            MouseState mouseState = Mouse.GetState();
            Point mouseTileBoxLocalPos = new Point(mouseState.X - (m_tileBoxLocation.X +m_toolBarLocation.X), mouseState.Y -(m_tileBoxLocation.Y + m_toolBarLocation.Y));

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseTileBoxLocalPos.X > 0 && mouseTileBoxLocalPos.X < m_tileBoxSize.Width)
                {
                    if(mouseTileBoxLocalPos.Y > 0 && mouseTileBoxLocalPos.Y < m_tileBoxSize.Height)
                    {
                        m_activeTile = new Point(mouseTileBoxLocalPos.X / (tileSize * m_tileZoom) + m_hScrollBarTileBox.GetValue(), mouseTileBoxLocalPos.Y / (tileSize * m_tileZoom) + m_vScrollBarTileBox.GetValue());
                    }
                }
            }
        }

        void OpenFileWindowOk()
        {
            Stream iStream = new FileStream(m_openFileWindow.GetFileName(), FileMode.Open);
            Texture2D tileMap = Texture2D.FromStream(m_backGroundTexture.GraphicsDevice, iStream);
            iStream.Close();

            if (tileMap.Width < 254 * m_tileMaps.TilePixSize() && tileMap.Height < 254 * m_tileMaps.TilePixSize() && m_tileMaps.GetTileMapCount() < 254)
            {
                m_tileMaps.AddTileMap(tileMap);
                m_network.SendAddTileMap(m_openFileWindow.GetFileName());
                Refresh();
            }
        }

        public void Refresh()
        {
            ConfigurateTileMapSelecter();
            Texture2D tileMap = m_tileMaps.GetTileMap(m_tileMapSelecter.GetValue());
            m_activeTileMapSize = new Size(tileMap.Width, tileMap.Height);
            CalculateCropBounds(tileMap);
            ConfigurateScrollBars(m_tileMaps.TilePixSize());
        }
    }
}
