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
    class SelectBox
    {
        Texture2D m_bGTex;
        Rectangle m_drawReclangle;

        Texture2D m_selecterBoxTex;

        List<string> m_itemList;
        List<Texture2D> m_iconList;
        int m_nSelectedItem;

        Size m_iconSize;
        Size m_itemBorderOffset;
        int m_itemSpacing = 5;

        Size m_scrollBarSize;
        Point m_scrollBarLoc;
        VScrollBar m_scrollBar;
        int m_nFittingItems;

        SpriteFont m_font;
        float m_fontSize;

        public SelectBox(Texture2D bgTex,Texture2D selectedBoxTex, Texture2D scrollBarBGTex,Texture2D scrollBarButtonTex,SpriteFont font,Point location,Size size,Size iconSize,float fontSize = 1.5f)
        {
            m_bGTex = bgTex;
            m_drawReclangle = new Rectangle(location.X, location.Y, size.Width, size.Height);

            m_selecterBoxTex = selectedBoxTex;

            m_itemBorderOffset = new Size(20, 40);
            m_iconSize = iconSize;
            m_font = font;
            m_fontSize = fontSize;

            m_itemList = new List<string>();
            m_iconList = new List<Texture2D>();
            m_nSelectedItem = 0;

            m_scrollBarSize = new Size(32, m_drawReclangle.Height);
            m_scrollBarLoc = new Point(m_drawReclangle.X + m_drawReclangle.Width - m_scrollBarSize.Width, m_drawReclangle.Y);
            m_scrollBar = new VScrollBar(m_scrollBarLoc, m_scrollBarSize, 60, scrollBarBGTex, scrollBarButtonTex, 0, 100, Color.RoyalBlue);

            m_nFittingItems = (m_drawReclangle.Height - m_itemBorderOffset.Height * 2) / (m_iconSize.Height + m_itemSpacing);
        }
        public string GetSelectedItem()
        {
            if (m_itemList.Count > 0)
                return m_itemList[m_nSelectedItem];
            else
                return "";
        }
        public void AddItem(string item,Texture2D Icon) 
        {
            m_itemList.Add(item);
            m_iconList.Add(Icon);
            ConfigurateScrollBar();
        }

        void ConfigurateScrollBar()
        {
            if ((m_iconSize.Height + m_itemSpacing) * m_itemList.Count > m_drawReclangle.Height - m_itemBorderOffset.Height * 2)
            {
                m_scrollBar.SetMaxValue(m_itemList.Count - m_nFittingItems);
                m_scrollBar.Show();
            }
        }

        public void Draw(SpriteBatch spriteBatch,float depth)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_bGTex, m_drawReclangle, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            spriteBatch.End();
            DrawSelectedBox(spriteBatch, depth);
            DrawIcons(spriteBatch, depth);
            DrawText(spriteBatch, depth);
            m_scrollBar.Draw(spriteBatch, depth);
        }

        void DrawSelectedBox(SpriteBatch spriteBatch, float depth)
        {
            if (m_nSelectedItem >= m_scrollBar.GetValue() && m_nSelectedItem < m_scrollBar.GetValue() + m_nFittingItems)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                Point drawPos = new Point(m_drawReclangle.X + m_itemBorderOffset.Width, m_drawReclangle.Y + m_itemBorderOffset.Height);
                drawPos.Y += (m_itemSpacing + m_iconSize.Height) * (m_nSelectedItem - m_scrollBar.GetValue());
                Rectangle drawRec = new Rectangle(drawPos.X, drawPos.Y - 4, m_drawReclangle.Width - m_itemBorderOffset.Width - m_scrollBarSize.Width, m_iconSize.Height + 8);
                spriteBatch.Draw(m_selecterBoxTex, drawRec, null, Color.Gray, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
                spriteBatch.End();
            }
        }

        void DrawText(SpriteBatch spriteBatch, float depth)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            Vector2 drawPos = new Vector2(m_drawReclangle.X + m_itemBorderOffset.Width + m_iconSize.Width + 5,m_drawReclangle.Y + m_itemBorderOffset.Height + m_iconSize.Height/2 - m_font.MeasureString("|").Y/2);
            for (int i = m_scrollBar.GetValue(); i < m_itemList.Count; i++)
            {
                int stringLengt = m_itemList[i + m_scrollBar.GetValue()].Length;
                if (stringLengt > 16)
                    stringLengt = 16;
                if (drawPos.Y + m_font.MeasureString("|").Y/2 - m_iconSize.Height / 2 + m_iconSize.Height < m_drawReclangle.Y + m_drawReclangle.Height - m_itemBorderOffset.Height)
                {
                    spriteBatch.DrawString(m_font, m_itemList[i].Substring(0, stringLengt), drawPos, Color.Black, 0.0f, Vector2.Zero, m_fontSize, SpriteEffects.None, depth);
                    drawPos.Y += (m_itemSpacing + m_iconSize.Height);
                }
            }
            spriteBatch.End();
        }

        void DrawIcons(SpriteBatch spriteBatch, float depth)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            Point drawPos = new Point(m_drawReclangle.X + m_itemBorderOffset.Width, m_drawReclangle.Y + m_itemBorderOffset.Height);
            for (int i = m_scrollBar.GetValue(); i < m_itemList.Count; i++)
            {
                if (drawPos.Y + m_iconSize.Height < m_drawReclangle.Y + m_drawReclangle.Height - m_itemBorderOffset.Height)
                {
                    Rectangle drawRec = new Rectangle(drawPos.X, drawPos.Y, m_iconSize.Width, m_iconSize.Height);
                    spriteBatch.Draw(m_iconList[i], drawRec, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
                    drawPos.Y += (m_itemSpacing + m_iconSize.Height);
                }
            }
            spriteBatch.End();
        }

        public void HandleInput()
        {
            m_scrollBar.HandleInput();
            CheckClicks();
        }

        void CheckClicks()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Point localMousePos = new Point(mouseState.X - (m_drawReclangle.X + m_itemBorderOffset.Width),mouseState.Y - (m_drawReclangle.Y + m_itemBorderOffset.Height));

                if (localMousePos.X >= 0 && localMousePos.X <= m_drawReclangle.Width - m_itemBorderOffset.Width - m_scrollBarSize.Width)
                {
                    if (localMousePos.Y >= 0 && localMousePos.Y < m_nFittingItems * (m_iconSize.Height + m_itemSpacing))
                    {
                        m_nSelectedItem = localMousePos.Y / (m_iconSize.Height + m_itemSpacing) + m_scrollBar.GetValue();
                        if (m_nSelectedItem >= m_itemList.Count)
                            m_nSelectedItem = m_itemList.Count - 1;

                    }
                }
            }
        }
    }
}
