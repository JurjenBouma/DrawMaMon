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
    public delegate void NewGameHandler(string saveName,string worldName);
    public delegate void LoadGameHandler(string saveName);
    public delegate void JoinGameHandler(string ipAdress);
    class MainMenu
    {
        Rectangle m_windowDrawRectangle;
        Texture2D m_bGTex;

        Size m_sPButtonSize;
        Point m_sPButtonLocation;
        Button m_sPButton;

        Size m_mPButtonSize;
        Point m_mPButtonLocation;
        Button m_mPButton;

        GameSelectMenu m_gameSelectMenu;
        NewGameMenu m_newGameMenu;
        ServerSelectMenu m_serverSelectMenu;
        MenuStateController m_menuState;

        public NewGameHandler NewGameEvent;
        public LoadGameHandler LoadGameEvent;
        public JoinGameHandler JoinGameEvent;

        public MainMenu(Texture2D backGroundTex, Texture2D menuButtonTex, Texture2D newButtonTex, Texture2D playButtonTex, Texture2D textBoxBGTex, Texture2D gameSelectBoxTex, Texture2D selectedBoxTex, Texture2D scrollBarBGTex, Texture2D scrollBarButtonTex, Texture2D createButtonTex, Texture2D joinButtonTex, Texture2D fileIcon, Size windowSize, Point screenOffset, SpriteFont font, string saveFolderPath, string worldFolderPath)
        {
            m_windowDrawRectangle = new Rectangle(screenOffset.X, screenOffset.Y, windowSize.Width - screenOffset.X * 2, windowSize.Height - screenOffset.Y * 2);
            m_bGTex = backGroundTex;
            m_sPButtonSize = new Size(256, 64);
            m_sPButtonLocation = new Point(screenOffset.X+((windowSize.Width - screenOffset.X*2)/2) - m_sPButtonSize.Width/2, screenOffset.Y+(windowSize.Height/2)-300);
            m_sPButton = new Button(m_sPButtonLocation, m_sPButtonSize, menuButtonTex, "SINGLEPLAYER", font,2.0f);

            m_mPButtonSize = new Size(256, 64);
            m_mPButtonLocation = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) - m_mPButtonSize.Width / 2, screenOffset.Y + (windowSize.Height / 2) - 200);
            m_mPButton = new Button(m_mPButtonLocation, m_mPButtonSize, menuButtonTex, "MULTIPLAYER", font,2.0f);
            
            m_gameSelectMenu = new GameSelectMenu(m_bGTex, newButtonTex, playButtonTex, gameSelectBoxTex, selectedBoxTex, scrollBarBGTex, scrollBarButtonTex,
                fileIcon,screenOffset, windowSize, font,saveFolderPath);
            m_gameSelectMenu.NewGameClickEvent += new NewGameClickHandler(ShowNewGameMenu);
            m_gameSelectMenu.LoadGameClickEvent += new LoadGameClickHandler(LoadGame);

            m_newGameMenu = new NewGameMenu(m_bGTex, createButtonTex,textBoxBGTex,gameSelectBoxTex,selectedBoxTex,scrollBarBGTex,scrollBarButtonTex,fileIcon, screenOffset, windowSize,font,worldFolderPath);
            m_newGameMenu.CreateGameCliked += new CreateGameClickHandler(CreateNewGame);

            m_serverSelectMenu = new ServerSelectMenu(m_bGTex,joinButtonTex, gameSelectBoxTex, selectedBoxTex, scrollBarBGTex, scrollBarButtonTex, fileIcon, screenOffset, windowSize, font);
            m_serverSelectMenu.JoinGameClickEvent += new JoinGameClickHandler(JoinGame);

            m_menuState = new MenuStateController(MainMenuState.Main);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (m_menuState.GetMenuState() == MainMenuState.Main)
            {
                spriteBatch.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw(m_bGTex, m_windowDrawRectangle, Color.White);
                spriteBatch.End();
                m_sPButton.Draw(spriteBatch, Color.White,Color.White, 0.0f);
                m_mPButton.Draw(spriteBatch, Color.White,Color.White, 0.0f);
            }
            if (m_menuState.GetMenuState() == MainMenuState.GameSelect)
            {
                m_gameSelectMenu.Draw(spriteBatch);
            }
            if (m_menuState.GetMenuState() == MainMenuState.NewGame)
            {
                m_newGameMenu.Draw(spriteBatch);
            }
            if (m_menuState.GetMenuState() == MainMenuState.ServerSelect)
            {
                m_serverSelectMenu.Draw(spriteBatch);
            }
        }

        public void HandleInput()
        {
            if (m_menuState.GetMenuState() == MainMenuState.GameSelect)
            {
                m_gameSelectMenu.HandleInput();
            }
            if (m_menuState.GetMenuState() == MainMenuState.Main)
            {
                if (m_sPButton.IsClicked())
                {
                    m_menuState.SetMenuState(MainMenuState.GameSelect);
                }
                if (m_mPButton.IsClicked())
                {
                    m_menuState.SetMenuState(MainMenuState.ServerSelect);
                }
            }
            if(m_menuState.GetMenuState() == MainMenuState.NewGame)
            {
                m_newGameMenu.HandleInput();
            }
            if (m_menuState.GetMenuState() == MainMenuState.ServerSelect)
            {
                m_serverSelectMenu.HandleInput();
            }
        }

        void ShowNewGameMenu()
        {
            m_menuState.SetMenuState(MainMenuState.NewGame);
        }

        void CreateNewGame(string saveName,string worldName)
        {
            NewGameEvent(saveName,worldName);
        }

        void LoadGame(string saveName)
        {
            LoadGameEvent(saveName);
        }

        void JoinGame(string ip)
        {
            JoinGameEvent(ip);
        }
    }
}
