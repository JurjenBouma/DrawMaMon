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
    public delegate void OkHandler();

    partial class OpenFileWindow
    {
        string m_fileName;
        List<string> m_fileTypes;
        string m_path;
        Texture2D m_bgTex;
        Rectangle m_drawRectangle;
        Button m_closeButton;
        Size m_closeButtonSize;
        Button m_backButton;
        Size m_backButtonSize;
        Texture2D m_folderTex;
        Texture2D m_fileTex;
        List<Button> m_fileButtons;
        List<Button> m_folderButtons;
        Size m_navButtonSize;
        Size m_navButtonSpace;
        Size m_navButtonsBorderOffset;
        List<navItem> m_navItemList;
        int m_maxItemsHeight;
        int m_maxItemsWidth;
        int m_maxItems;
        Size m_fileScrollBarSize;
        Point m_fileScrollBarLoc;
        HScrollBar m_fileScrollBar;
        bool m_visible = false;
        SpriteFont m_font;
        float m_fontSize;
        MouseState m_prevMouseState;
        public OkHandler WindowOkEvent;

        public OpenFileWindow(Point drawLocation, Size windowSize, Texture2D bgTex, Texture2D closeButtonTex, Texture2D backButtonTex, Texture2D folderTex, Texture2D fileTex, Texture2D hScrollbgTex, Texture2D hScrollBarButtonTex, SpriteFont font, string defaultPath,List<string> fileTypes,float fontSize = 1.0f)
        {
            m_font = font;
            m_fontSize = fontSize;
            m_path = defaultPath;
            m_fileTypes = fileTypes;

            m_bgTex = bgTex;
            m_drawRectangle = new Rectangle(drawLocation.X, drawLocation.Y, windowSize.Width, windowSize.Height);

            m_closeButtonSize = new Size(32, 32);
            m_backButtonSize = new Size(64, 64);
            m_closeButton = new Button(new Point(m_drawRectangle.X + m_drawRectangle.Width - m_closeButtonSize.Width, drawLocation.Y), m_closeButtonSize, closeButtonTex,"",null);
            m_backButton = new Button(new Point(m_drawRectangle.X, drawLocation.Y), m_backButtonSize, backButtonTex,"",null);

            m_navItemList = new List<navItem>();
            m_folderButtons = new List<Button>();
            m_fileButtons = new List<Button>();
            m_folderTex = folderTex;
            m_fileTex = fileTex;
            m_navButtonSpace = new Size(100, 10);
            m_navButtonsBorderOffset = new Size(128, 64);
            m_navButtonSize = new Size(48, 48);
            m_maxItemsHeight = (m_drawRectangle.Height - m_navButtonsBorderOffset.Height) / (m_navButtonSize.Height + m_navButtonSpace.Height + (int)font.MeasureString("H").Y);
            m_maxItemsWidth = (m_drawRectangle.Width - m_navButtonsBorderOffset.Width) / (m_navButtonSize.Width + m_navButtonSpace.Width);
            m_maxItems = m_maxItemsHeight * m_maxItemsWidth;

            m_fileScrollBarSize = new Size(m_drawRectangle.Width, 16);
            m_fileScrollBarLoc = new Point(0, m_drawRectangle.Height - m_fileScrollBarSize.Height);
            InitializeScrollBar(hScrollbgTex, hScrollBarButtonTex);

            SetFileList();
            SetNavButtons();
            ConfigurateScrollBar();
            m_prevMouseState = Mouse.GetState();
        }

        void InitializeScrollBar(Texture2D hScrollbgTex, Texture2D hScrollBarButtonTex)
        {
            m_fileScrollBar = new HScrollBar(new Point(m_fileScrollBarLoc.X + m_drawRectangle.X, m_fileScrollBarLoc.Y + m_drawRectangle.Y), m_fileScrollBarSize, 60, hScrollbgTex, hScrollBarButtonTex, 0, 100, Color.Gray);
        }
    }
}