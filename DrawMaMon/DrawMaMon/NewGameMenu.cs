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
    public delegate void CreateGameClickHandler(string saveName,string worldName);
    class NewGameMenu
    {
        Rectangle m_windowDrawRectangle;
        Texture2D m_bGTex;

        Size m_createButtonSize;
        Point m_createButtonLocation;
        Button m_createButton;

        Size m_textBoxGameNameSize;
        Point m_textBoxGameNameLocation;
        TextBox m_textBoxGameName;

        Point m_gameNameLabelLoc;
        Label m_gameNameLabel;

        Point m_worldLabelLoc;
        Label m_worldLabel;

        Size m_worldSelectBoxSize;
        Point m_worldSelectBoxLoc;
        SelectBox m_worldSelectBox;

        public CreateGameClickHandler CreateGameCliked;

        public NewGameMenu(Texture2D backGroundTex, Texture2D createButtonTex, Texture2D textBoxBGTex, Texture2D gameSelectBoxTex, Texture2D selectedBoxTex, Texture2D scrollBarBGTex, Texture2D scrollBarButtonTex, Texture2D fileIcon, Point screenOffset, Size windowSize, SpriteFont font, string worldFolderPath)
        {
            m_windowDrawRectangle = new Rectangle(screenOffset.X, screenOffset.Y, windowSize.Width - screenOffset.X * 2, windowSize.Height - screenOffset.Y * 2);
            m_bGTex = backGroundTex;

            m_createButtonSize = new Size(256, 64);
            m_createButtonLocation = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) +20, screenOffset.Y + (windowSize.Height / 2) + 200);
            m_createButton = new Button(m_createButtonLocation, m_createButtonSize, createButtonTex, "New Game", null);

            m_textBoxGameNameSize = new Size(320, 32);
            m_textBoxGameNameLocation = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) - m_textBoxGameNameSize.Width/2, screenOffset.Y + (windowSize.Height / 2)-150);
            m_textBoxGameName = new TextBox(textBoxBGTex, font, m_textBoxGameNameLocation, m_textBoxGameNameSize);

            m_gameNameLabelLoc = new Point(m_textBoxGameNameLocation.X, m_textBoxGameNameLocation.Y - (int)font.MeasureString("|").Y-5);
            m_gameNameLabel = new Label(font, m_gameNameLabelLoc, "Name:");
        
            m_worldLabelLoc = new Point(m_textBoxGameNameLocation.X, m_textBoxGameNameLocation.Y + m_textBoxGameNameSize.Height +5);
            m_worldLabel = new Label(font, m_worldLabelLoc, "Select World:");

            m_worldSelectBoxSize = new Size(m_textBoxGameNameSize.Width, 250);
            m_worldSelectBoxLoc = new Point(m_textBoxGameNameLocation.X,m_worldLabelLoc.Y + (int)font.MeasureString("|").Y + 5);
            m_worldSelectBox = new SelectBox(gameSelectBoxTex, selectedBoxTex, scrollBarBGTex, scrollBarButtonTex, font, m_worldSelectBoxLoc, m_worldSelectBoxSize, new Size(32, 32));
            foreach (string folder in Directory.GetDirectories(worldFolderPath))
            {
                if (Directory.Exists(folder + "\\Maps"))
                {
                    DirectoryInfo folderInfo = new DirectoryInfo(folder);
                    m_worldSelectBox.AddItem(folderInfo.Name, fileIcon);
                }
            }
            m_worldSelectBox.AddItem("Create new world", fileIcon);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_bGTex, m_windowDrawRectangle, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.End();
            m_createButton.Draw(spriteBatch, Color.White, 0.0f);
            m_textBoxGameName.Draw(spriteBatch,Color.White,0.0f);
            m_gameNameLabel.Draw(spriteBatch, Color.Black);
            m_worldLabel.Draw(spriteBatch, Color.Black);
            m_worldSelectBox.Draw(spriteBatch, 0.0f);
        }

        public void HandleInput()
        {
            m_textBoxGameName.HandleInput();
            m_worldSelectBox.HandleInput();
            if (m_createButton.IsClicked())
            {
                if (m_textBoxGameName.GetText().Length > 0)
                {
                    string worldName = m_worldSelectBox.GetSelectedItem();
                    if (worldName == "Create new world")
                    {
                        worldName = m_textBoxGameName.GetText();
                    }
                    CreateGameCliked(m_textBoxGameName.GetText(), worldName);
                }
            }
        }
    }
}
