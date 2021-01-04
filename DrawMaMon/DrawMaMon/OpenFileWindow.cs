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
    struct navItem
    {
        public string Name;
        public bool IsFolder;
        public navItem(string name,bool isFolder)
        {
            IsFolder = isFolder;
            Name = name;
        }
    }

    partial class OpenFileWindow
    {
        public void Show() 
        {
            m_prevMouseState = Mouse.GetState();
            m_visible = true; 
        }
        public bool IsVisible(){ return m_visible; }
        public string GetFileName() { return m_fileName; }

        void ConfigurateScrollBar()
        {
            int rowCount = (int)((float)m_navItemList.Count / m_maxItemsHeight + 0.9999f);
            if (rowCount > m_maxItemsWidth)
            {
                m_fileScrollBar.Show();
                m_fileScrollBar.SetMaxValue(rowCount - m_maxItemsWidth);
            }
            else
            {
                m_fileScrollBar.Hide();
                m_fileScrollBar.SetMaxValue(0);
            }
        }

        void SetFileList()
        {
            string[] filePaths = Directory.GetFiles(m_path);
            string[] directoryPaths = Directory.GetDirectories(m_path);
            FileInfo fileInfo;
            DirectoryInfo folderInfo;

            m_navItemList.RemoveRange(0, m_navItemList.Count);
            foreach (string folder in directoryPaths)
            {

                folderInfo = new DirectoryInfo(folder);
                m_navItemList.Add(new navItem(folderInfo.Name, true)); 
                //Only add accesable folders
                try
                {
                    Directory.GetFiles(m_path + "\\" + folderInfo.Name);
                }
                catch
                {
                    m_navItemList.RemoveAt(m_navItemList.Count-1);
                }
                
            }
            foreach (string file in filePaths)
            {
                fileInfo = new FileInfo(file);
                if(m_fileTypes.Contains(fileInfo.Extension)) 
                m_navItemList.Add(new navItem(fileInfo.Name, false));
            }
        }

        void SetNavButtons()
        {  
            m_folderButtons.RemoveRange(0, m_folderButtons.Count);
            m_fileButtons.RemoveRange(0, m_fileButtons.Count);
            int offSet = m_fileScrollBar.GetValue() * m_maxItemsHeight;

            for (int i = 0; i < m_maxItems; i++)
            {
                if (i+offSet < m_navItemList.Count)
                {
                    int row = i / m_maxItemsHeight;
                    if (m_navItemList[i + offSet].IsFolder)
                    {
                        Point location = new Point(m_drawRectangle.X + m_navButtonsBorderOffset.Width + (row * (m_navButtonSize.Width + m_navButtonSpace.Width)), m_drawRectangle.Y + m_navButtonsBorderOffset.Height + (i - row * m_maxItemsHeight) * (m_navButtonSize.Height + m_navButtonSpace.Height));
                        m_folderButtons.Add(new Button(location, m_navButtonSize, m_folderTex,"",null));
                    }
                    else
                    {
                        Point location = new Point(m_drawRectangle.X + m_navButtonsBorderOffset.Width + (row * (m_navButtonSize.Width + m_navButtonSpace.Width)), m_drawRectangle.Y + m_navButtonsBorderOffset.Height + (i - row * m_maxItemsHeight) * (m_navButtonSize.Height + m_navButtonSpace.Height));
                        m_fileButtons.Add(new Button(location, m_navButtonSize, m_fileTex,"",null));
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (m_visible == true)
            {
                DrawBackGround(spriteBatch);
                DrawInfoText(spriteBatch);
                DrawNavButtons(spriteBatch);
                m_closeButton.Draw(spriteBatch, Color.IndianRed,0.0f);
                m_backButton.Draw(spriteBatch, Color.Gold,0.0f);
                m_fileScrollBar.Draw(spriteBatch,0.0f);
            }
        }

        void DrawBackGround(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_bgTex, m_drawRectangle, Color.White);
            spriteBatch.End();
        }

        void DrawInfoText(SpriteBatch spriteBatch)
        {

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            int offSet = m_fileScrollBar.GetValue() * m_maxItemsHeight;

            for (int i = 0; i < m_maxItems; i++)
            {
                if (i + offSet < m_navItemList.Count)
                {
                    int row = i / m_maxItemsHeight;
                    int stringLengt = m_navItemList[i+offSet].Name.Length;
                    if (stringLengt > 16)
                        stringLengt = 16;
                    Vector2 location = new Vector2(m_drawRectangle.X + m_navButtonsBorderOffset.Width + (row * (m_navButtonSize.Width + m_navButtonSpace.Width)), m_drawRectangle.Y + m_navButtonsBorderOffset.Height + (i - row * m_maxItemsHeight) * (m_navButtonSize.Height + m_navButtonSpace.Height) + m_navButtonSize.Height);
                    spriteBatch.DrawString(m_font, m_navItemList[i + offSet].Name.Substring(0, stringLengt), location, Color.Black, 0.0f, Vector2.Zero, m_fontSize,SpriteEffects.None,0.0f);
                }
            }
            spriteBatch.End();
        }

        void DrawNavButtons(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < m_folderButtons.Count; i++)
            {
                m_folderButtons[i].Draw(spriteBatch, Color.Yellow,0.0f);
            }
            for (int i = 0; i < m_fileButtons.Count; i++)
            {
                m_fileButtons[i].Draw(spriteBatch, Color.White,0.0f);
            }
               
        }
        public void HandleInput()
        {
            if(m_visible == true)
            {
                MouseState mouseState = Mouse.GetState();
                if (m_closeButton.IsClicked())
                    m_visible = false;
                HandleInputBackButton();
                m_fileScrollBar.HandleInput();
                HandleInputNavButtons();
                SetNavButtons();

                m_prevMouseState = Mouse.GetState();
            }
        }
        void HandleInputBackButton()
        {
            MouseState mouseState = Mouse.GetState();
            if (m_backButton.IsClicked() && mouseState.LeftButton != m_prevMouseState.LeftButton)
            {
                DirectoryInfo parentDirectory = Directory.GetParent(m_path);
                if (parentDirectory != null)
                {
                    m_path = parentDirectory.FullName;
                    SetFileList();
                    ConfigurateScrollBar();
                }
            }
        }

        void HandleInputNavButtons()
        {
            MouseState mouseState = Mouse.GetState();
            int offSet = m_fileScrollBar.GetValue() * m_maxItemsHeight;

            if (mouseState.LeftButton != m_prevMouseState.LeftButton)
            {
                for (int i = 0; i < m_folderButtons.Count; i++)
                {
                    if (m_folderButtons[i].IsClicked())
                    {
                        m_path += "\\" + m_navItemList[i + offSet].Name;
                        SetFileList();
                        ConfigurateScrollBar();
                    }
                }

                for (int i = 0; i < m_fileButtons.Count; i++)
                {
                    if (m_fileButtons[i].IsClicked())
                    {
                        m_fileName = m_path + "\\" + m_navItemList[i + offSet + m_folderButtons.Count].Name;
                        m_visible = false;
                        WindowOkEvent();
                    }
                }
            }
        }
    }
}
