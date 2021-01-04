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
    public delegate void ExitEventHandler();
    public delegate void SaveEventHandler();

    class EscapeMenu
    {
        Size m_exitButtonSize;
        Point m_exitButtonLocation;
        Button m_exitButton;
        Size m_saveButtonSize;
        Point m_saveButtonLocation;
        Button m_saveButton;
        public ExitEventHandler ExitEvent;
        public SaveEventHandler SaveEvent;

        public EscapeMenu(Texture2D menuSaveButtonTex, Texture2D exitSaveButtonTex, Size windowSize, Point screenOffset, SpriteFont buttonFont)
        {
            m_saveButtonSize = new Size(256, 64);
            m_saveButtonLocation = new Point(screenOffset.X + (windowSize.Width / 2) - m_saveButtonSize.Width / 2, screenOffset.Y + (windowSize.Height / 2));
            m_saveButton = new Button(m_saveButtonLocation, m_saveButtonSize, menuSaveButtonTex, "Save Game", null);

            m_exitButtonSize = new Size(256, 64);
            m_exitButtonLocation = new Point(screenOffset.X + (windowSize.Width / 2) - m_exitButtonSize.Width / 2, screenOffset.Y + (windowSize.Height / 2) + m_saveButtonSize.Height + 40);
            m_exitButton = new Button(m_exitButtonLocation, m_exitButtonSize, exitSaveButtonTex, "Exit Game", null);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            m_exitButton.Draw(spriteBatch, Color.White, 0.0f);
            m_saveButton.Draw(spriteBatch,Color.White,0.0f);
        }
        public void HandleInput()
        {
            if (m_exitButton.IsClicked())
                ExitEvent();
            if (m_saveButton.IsClicked())
                SaveEvent();
        }
    }
}
