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
    public delegate void FinishedHandler();

    class MapEditor
    {
        TileToolBar m_tileToolBar;
        KeyboardState m_prevKeyState;
        public FinishedHandler FinishedEvent;
        Size m_exitButtonSize;
        Button m_exitButton;
        const int m_nLayers = 14;

        public MapEditor(Texture2D toolBarBgTex, Texture2D toolBarFloorLayerTex, Texture2D toolBarRoofLayerTex, Texture2D toolBarTileBoxTex, Texture2D arrowLeft, Texture2D arrowRight, Texture2D openTex, Texture2D deleteTex, Texture2D openFileBGTex, Texture2D closeButtonTex, Texture2D folderTex, Texture2D fileTex, Texture2D backButtonTex, Texture2D hScrollBarBGTex, Texture2D hScrollBarButtonTex, Texture2D vScrollBarBGTex, Texture2D vScrollBarButtonTex, SpriteFont numberFont, SpriteFont fileFont, GraphicsDeviceManager graphics, TileMaps tileMaps,Network network,Size windowSize,Point screenOffset)
        {
            m_tileToolBar = new TileToolBar(toolBarBgTex, toolBarFloorLayerTex, toolBarRoofLayerTex, 
                toolBarTileBoxTex, arrowLeft, arrowRight, openTex, deleteTex, openFileBGTex, 
                closeButtonTex, folderTex, fileTex, backButtonTex, hScrollBarBGTex, hScrollBarButtonTex,
                vScrollBarBGTex, vScrollBarButtonTex, numberFont, fileFont, graphics, m_nLayers, tileMaps, network);

            m_exitButtonSize = new Size(32, 32);
            InitializeExitButton(new Point(windowSize.Width - m_exitButtonSize.Width - screenOffset.X, 0 + screenOffset.Y), closeButtonTex);
        }
        public int GetLayerNumber() { return m_nLayers; }
        
        void InitializeExitButton(Point location,Texture2D buttonTex)
        {
            m_exitButton = new Button(location,m_exitButtonSize,buttonTex,"",null);
        }

        public void DrawUI(SpriteBatch spriteBatch,TileMaps tileMaps)
        {
            m_tileToolBar.Draw(spriteBatch, tileMaps, m_nLayers);
            m_exitButton.Draw(spriteBatch, Color.IndianRed, 0.0f);
        }

        public void HandleInput( Point screenOffset, Map currentMap, Size windowSize, Size tileScreenSize, Camera cam, int tilePixSize, Size numTilesScreen, TileMaps tileMaps)
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePosLocalGameWindow = new Point(mouseState.X - screenOffset.X, mouseState.Y - screenOffset.Y);
            
            if (mouseState.Y < m_tileToolBar.GetToolBarLocation().Y)
                if (mouseState.LeftButton == ButtonState.Pressed)
                    if (m_tileToolBar.IsReady())
                        if (mouseState.X > screenOffset.X && mouseState.X < (windowSize.Width - screenOffset.X))
                            if (mouseState.Y > screenOffset.Y && mouseState.Y < (windowSize.Height - screenOffset.Y))
                                if (mouseState.X < m_exitButton.GetLocation().X || mouseState.Y > m_exitButton.GetLocation().Y + m_exitButtonSize.Height)
                                {
                                    Vector2 clickOffSet = new Vector2(((float)cam.Position().X / tilePixSize) - (numTilesScreen.Width / 2), ((float)cam.Position().Y / tilePixSize) - (numTilesScreen.Height / 2));
                                    Point clickedTile = new Point((int)(((float)mousePosLocalGameWindow.X / tileScreenSize.Width) + clickOffSet.X), (int)(((float)mousePosLocalGameWindow.Y / tileScreenSize.Height) + clickOffSet.Y));
                                    currentMap.SetTile(clickedTile.X, clickedTile.Y, m_tileToolBar.GetActiveTile(), m_tileToolBar.GetActiveTileMap(), m_tileToolBar.GetActivelayer());
                                }
            ControlCamera(cam, tilePixSize, numTilesScreen,currentMap.Width(),currentMap.Height());
            m_tileToolBar.HandleInput(tilePixSize, m_nLayers, tileMaps, currentMap);
            
            if (m_exitButton.IsClicked())
                FinishedEvent();

            m_prevKeyState = Keyboard.GetState();
        }

        public void Refresh()
        {
            m_tileToolBar.Refresh();
        }

        void ControlCamera(Camera cam, int tilePixSize,Size numTilesScreen,int mapWidth,int mapHeight)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Point location = cam.Position();
                location.Y -=1;
                cam.MoveCamera(location, tilePixSize, numTilesScreen.Width, numTilesScreen.Height, mapWidth, mapHeight);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Point location = cam.Position();
                location.Y += 1;
                cam.MoveCamera(location, tilePixSize, numTilesScreen.Width, numTilesScreen.Height, mapWidth, mapHeight);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Point location = cam.Position();
                location.X -= 1;
                cam.MoveCamera(location, tilePixSize, numTilesScreen.Width, numTilesScreen.Height, mapWidth, mapHeight);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Point location = cam.Position();
                location.X += 1;
                cam.MoveCamera(location, tilePixSize, numTilesScreen.Width, numTilesScreen.Height, mapWidth, mapHeight);
            }
        }
    }
}
