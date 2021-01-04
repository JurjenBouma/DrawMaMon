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
    public delegate void NewGameClickHandler();
    public delegate void LoadGameClickHandler(string saveName);

    class GameSelectMenu
    {
        Rectangle m_windowDrawRectangle;
        Texture2D m_bGTex;

        Size m_gameSelectBoxSize;
        Point m_gameSelectBoxLoc;
        SelectBox m_gameSelectBox;

        Size m_newButtonSize;
        Point m_newButtonLocation;
        Button m_newButton;

        Size m_playButtonSize;
        Point m_playButtonLocation;
        Button m_playButton;

        public NewGameClickHandler NewGameClickEvent;
        public LoadGameClickHandler LoadGameClickEvent;

        public GameSelectMenu(Texture2D backGroundTex, Texture2D newButtonTex, Texture2D playButtonTex, Texture2D gameSelectBoxTex,Texture2D selectedBoxTex,Texture2D scrollBarBGTex, Texture2D scrollBarButtonTex, Texture2D fileIcon, Point screenOffset, Size windowSize, SpriteFont font, string saveFolderPath)
        {
            m_windowDrawRectangle = new Rectangle(screenOffset.X, screenOffset.Y, windowSize.Width - screenOffset.X * 2, windowSize.Height - screenOffset.Y * 2);
            m_bGTex = backGroundTex;

            m_newButtonSize = new Size(256, 64);
            m_newButtonLocation = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) - m_newButtonSize.Width - 20, screenOffset.Y + (windowSize.Height / 2) + 300);
            m_newButton = new Button(m_newButtonLocation, m_newButtonSize, newButtonTex, "New Game", font);

            m_playButtonSize = new Size(256, 64);
            m_playButtonLocation = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) + 20, screenOffset.Y + (windowSize.Height / 2) + 300);
            m_playButton = new Button(m_playButtonLocation, m_playButtonSize, playButtonTex, "New Game", null);

            m_gameSelectBoxSize = new Size(512, 512);
            m_gameSelectBoxLoc = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) - m_gameSelectBoxSize.Width / 2, m_playButtonLocation.Y - m_gameSelectBoxSize.Height - 20);
            m_gameSelectBox = new SelectBox(gameSelectBoxTex,selectedBoxTex,scrollBarBGTex,scrollBarButtonTex,font, m_gameSelectBoxLoc, m_gameSelectBoxSize,new Size(64,64));

            foreach (string folder in Directory.GetDirectories(saveFolderPath))
            {
                if (File.Exists(folder + "\\World.world"))
                {
                    FileInfo fileInfo = new FileInfo(folder);
                    m_gameSelectBox.AddItem(fileInfo.Name, fileIcon);
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_bGTex, m_windowDrawRectangle,null , Color.White,0.0f,Vector2.Zero,SpriteEffects.None,0.0f);
            spriteBatch.End();
            m_gameSelectBox.Draw(spriteBatch,0.0f);
            m_newButton.Draw(spriteBatch,Color.White,0.0f);
            m_playButton.Draw(spriteBatch, Color.White, 0.0f);
        }
        public void HandleInput()
        {
            m_gameSelectBox.HandleInput();
            if (m_newButton.IsClicked())
                NewGameClickEvent();
            if (m_playButton.IsClicked())
                LoadGameClickEvent(m_gameSelectBox.GetSelectedItem());
        }
    }
}
