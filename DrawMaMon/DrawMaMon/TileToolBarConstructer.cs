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
        bool m_Ready = false;
        MouseState m_prevMouseState;

        Texture2D m_tileBoxTexture;
        Size m_tileBoxSize;
        Point m_tileBoxLocation;
        int m_tileBoxBorderSize;
        int m_tileZoom;

        Point m_layerBoxLocation;
        int m_layerDrawDistance;
        int m_layerButtonWidth;
        Button[] m_floorLayerButtons;
        Button[] m_roofLayerButtons;

        Texture2D m_backGroundTexture;
        Point m_toolBarLocation;
        Size m_toolBarSize;

        Button m_openTileMapButton;
        Point m_openTileMapButtonLoc;
        Size m_openTileMapButtonSize;
        Button m_deleteTileMapButton;
        Point m_deleteTileMapButtonLoc;
        Size m_deleteTileMapButtonSize;

        NumberSelecter m_tileMapSelecter;
        Point m_tileMapSelecterLoc;
        Size m_tileMapSelecterSize;

        Size m_activeTileMapCropSize;
        int m_nActiveTileMap;
        int m_activeLayer = 0;
        Point m_activeTile;
        Size m_activeTileMapSize;

        OpenFileWindow m_openFileWindow;
        TileMaps m_tileMaps;
        Network m_network;

        Size m_hScrollBarTileBoxSize;
        Point m_hScrollBarTileBoxLoc;
        HScrollBar m_hScrollBarTileBox;
        Size m_vScrollBarTileBoxSize;
        Point m_vScrollBarTileBoxLoc;
        VScrollBar m_vScrollBarTileBox;

        public TileToolBar(Texture2D background, Texture2D floorTex, Texture2D roofTex, Texture2D tileBoxTex, Texture2D arrowLeft, Texture2D arrowRight, Texture2D openTex, Texture2D deleteTex, Texture2D openFileBGTex, Texture2D closeButtonTex, Texture2D folderTex, Texture2D fileTex,Texture2D backButtonTex, Texture2D hScrollBarBGTex, Texture2D hScrollBarButtonTex, Texture2D vScrollBarBGTex, Texture2D vScrollBarButtonTex, SpriteFont numberFont, SpriteFont fileFont, GraphicsDeviceManager graphics, int nLayers, TileMaps tileMaps,Network network)
        {
            if (tileMaps.GetTileMapCount() > 0)
                m_Ready = true;

            m_tileMaps = tileMaps;
            m_network = network;

            m_backGroundTexture = background;
            m_tileBoxTexture = tileBoxTex;

            m_toolBarSize = new Size(graphics.PreferredBackBufferWidth, 320);
            m_toolBarLocation = new Point(0, graphics.PreferredBackBufferHeight - m_toolBarSize.Height);

            m_layerDrawDistance = 17;
            m_layerBoxLocation = new Point(8, 48);
            m_layerButtonWidth = 48;
            InitializeFloorLayerButtons(floorTex, nLayers);
            InitializeRoofLayerButtons(roofTex, nLayers);

            m_tileBoxSize = new Size(640, 256);
            m_activeTileMapSize = m_tileBoxSize;
            m_nActiveTileMap = 0;
            m_tileBoxLocation = new Point(m_layerBoxLocation.X + 8 + m_layerButtonWidth, 48);
            m_tileZoom = 2;
            m_tileBoxBorderSize = 5;

            m_tileMapSelecterLoc = new Point(0, 14);
            m_tileMapSelecterSize = new Size(96, 32);
            InitializeTileMapSelecter(arrowLeft, arrowRight, numberFont);

            m_openTileMapButtonLoc = new Point(m_tileBoxLocation.X + m_tileBoxSize.Width + 17, m_tileBoxLocation.Y);
            m_openTileMapButtonSize = new Size(32, 32);
            m_deleteTileMapButtonLoc = new Point(m_openTileMapButtonLoc.X, m_openTileMapButtonLoc.Y + m_openTileMapButtonSize.Height + 5);
            m_deleteTileMapButtonSize = new Size(32, 32);
            InitializeTileMapButtons(openTex, deleteTex);
            m_prevMouseState = Mouse.GetState();

            Size openFileWindowSize = new Size(768, 512);
            List<string> fileTypes = new List<string>();
            fileTypes.Add(".png");
            Point openFileWindowLocation = new Point(graphics.PreferredBackBufferWidth / 2 - openFileWindowSize.Width / 2, graphics.PreferredBackBufferHeight / 2 - openFileWindowSize.Height / 2);
            m_openFileWindow = new OpenFileWindow(openFileWindowLocation, openFileWindowSize,
                openFileBGTex, closeButtonTex, backButtonTex,folderTex,fileTex,hScrollBarBGTex,
                hScrollBarButtonTex, fileFont, System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures),fileTypes);
            m_openFileWindow.WindowOkEvent += new OkHandler(OpenFileWindowOk);

            m_hScrollBarTileBoxSize = new Size(m_tileBoxSize.Width + m_tileBoxBorderSize*2, 16);
            m_hScrollBarTileBoxLoc = new Point(m_tileBoxLocation.X - m_tileBoxBorderSize, m_tileBoxLocation.Y + m_tileBoxSize.Height);
            m_vScrollBarTileBoxSize = new Size(16, m_tileBoxSize.Height + m_tileBoxBorderSize * 2);
            m_vScrollBarTileBoxLoc = new Point(m_tileBoxLocation.X + m_tileBoxSize.Width, m_tileBoxLocation.Y - m_tileBoxBorderSize);
            InitializeScrollBars(hScrollBarBGTex, hScrollBarButtonTex,vScrollBarBGTex,vScrollBarButtonTex);

            Texture2D tileMap = tileMaps.GetTileMap(m_nActiveTileMap);
            CalculateCropBounds(tileMap);
            ConfigurateScrollBars(tileMaps.TilePixSize());
        }

        void InitializeScrollBars(Texture2D hBGTex, Texture2D hScrollButtonTex, Texture2D vBGTex, Texture2D vScrollButtonTex)
        {
            m_hScrollBarTileBox = new HScrollBar(new Point(m_hScrollBarTileBoxLoc.X + m_toolBarLocation.X, m_hScrollBarTileBoxLoc.Y + m_toolBarLocation.Y), m_hScrollBarTileBoxSize, 60, hBGTex, hScrollButtonTex, 0, 100, Color.Gray);
            m_vScrollBarTileBox = new VScrollBar(new Point(m_vScrollBarTileBoxLoc.X + m_toolBarLocation.X, m_vScrollBarTileBoxLoc.Y + m_toolBarLocation.Y), m_vScrollBarTileBoxSize, 60, vBGTex, vScrollButtonTex, 0, 100, Color.Gray);
        }

        void InitializeTileMapButtons(Texture2D openTex, Texture2D deleteTex)
        {
            m_deleteTileMapButton = new Button(new Point(m_deleteTileMapButtonLoc.X + m_toolBarLocation.X, m_deleteTileMapButtonLoc.Y + m_toolBarLocation.Y), m_deleteTileMapButtonSize, deleteTex,"",null);
            m_openTileMapButton = new Button(new Point(m_openTileMapButtonLoc.X + m_toolBarLocation.X, m_openTileMapButtonLoc.Y + m_toolBarLocation.Y), m_openTileMapButtonSize, openTex,"",null);
        }

        void InitializeTileMapSelecter(Texture2D leftTex, Texture2D rightTex, SpriteFont font)
        {
            m_tileMapSelecter = new NumberSelecter(new Point(m_tileMapSelecterLoc.X + m_toolBarLocation.X, m_tileMapSelecterLoc.Y + m_toolBarLocation.Y), m_tileMapSelecterSize, m_nActiveTileMap, Color.White, leftTex, rightTex, font, 0, m_tileMaps.GetTileMapCount() - 1);
        }

        void InitializeRoofLayerButtons(Texture2D texture, int nLayers)
        {
            float aspecRatio = (float)texture.Height / texture.Width;
            m_roofLayerButtons = new Button[(nLayers) / 2];
            Size size = new Size(m_layerButtonWidth, (int)(m_layerButtonWidth * aspecRatio));
            Point location;

            for (int i = 0; i < (nLayers) / 2; i++)
            {
                location = new Point(m_layerBoxLocation.X + m_toolBarLocation.X, m_layerBoxLocation.Y + m_toolBarLocation.Y + m_layerDrawDistance * (nLayers / 2) - (m_layerDrawDistance * (i + 1)));
                m_roofLayerButtons[i] = new Button(location, size, texture,"",null);
            }
        }

        void InitializeFloorLayerButtons(Texture2D texture, int nLayers)
        {
            float aspecRatio = (float)texture.Height / texture.Width;
            m_floorLayerButtons = new Button[(nLayers + 1) / 2];
            Size size = new Size(m_layerButtonWidth, (int)(m_layerButtonWidth * aspecRatio));
            Point location;

            for (int i = 0; i < (nLayers + 1) / 2; i++)
            {
                location = new Point(m_layerBoxLocation.X + m_toolBarLocation.X, m_layerBoxLocation.Y + m_toolBarLocation.Y + m_layerDrawDistance * nLayers - (m_layerDrawDistance * (i + 1)));
                m_floorLayerButtons[i] = new Button(location, size, texture,"",null);
            }
        }
    }
}
