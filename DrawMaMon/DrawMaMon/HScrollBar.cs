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

namespace DrawMaMon
{
    class HScrollBar
    {
        int m_currentValue;
        int m_maxValue;
        Texture2D m_bgTex;
        Button m_scrollButton;
        Rectangle m_drawRectangle;
        Color m_color;
        bool m_visible = false;
        int m_buttonWidth;
        bool m_buttonClicked = false;
        MouseState m_prevMouseState;

        public HScrollBar(Point drawLocation,Size size,int buttonWidth,Texture2D bgTex,Texture2D scrollButtonTex,int startValue,int maxValue,Color color)
        {
            m_drawRectangle = new Rectangle(drawLocation.X, drawLocation.Y, size.Width, size.Height);
            m_currentValue = startValue;
            m_color = color;
            m_maxValue = maxValue;
            m_bgTex = bgTex;
            m_buttonWidth = buttonWidth;
            m_scrollButton = new Button(drawLocation, new Size(buttonWidth, size.Height), scrollButtonTex,"",null);
            m_prevMouseState = Mouse.GetState();
        }
        public void Show(){ m_visible = true; }
        public void Hide() { m_visible = false; }
        public int GetValue() { return m_currentValue; }
        public void SetMaxValue(int value)
        {
            m_maxValue = value;
            m_currentValue = (int)(((float)m_scrollButton.GetLocation().X - m_drawRectangle.X) / ((float)m_drawRectangle.Width - m_buttonWidth) * m_maxValue);
        }

        public void HandleInput()
        {
            if (m_visible)
            {
                if (m_scrollButton.IsClicked())
                {
                    m_buttonClicked = true;
                }

                if (m_buttonClicked)
                {
                    MouseState mouseState = Mouse.GetState();
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        Point oldLocation = m_scrollButton.GetLocation();
                        Point newLocation = new Point(oldLocation.X + (mouseState.X - m_prevMouseState.X), oldLocation.Y);
                        if (newLocation.X < m_drawRectangle.X)
                            newLocation.X = m_drawRectangle.X;
                        if (newLocation.X > m_drawRectangle.X + (m_drawRectangle.Width - m_buttonWidth))
                            newLocation.X = m_drawRectangle.X + (m_drawRectangle.Width - m_buttonWidth);
                        m_scrollButton.SetLocation(newLocation);
                    }
                    else
                    {
                        m_buttonClicked = false;
                    }
                }
                m_prevMouseState = Mouse.GetState();

                m_currentValue = (int)(((float)m_scrollButton.GetLocation().X - m_drawRectangle.X)/((float)m_drawRectangle.Width - m_buttonWidth)*m_maxValue);
            }
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            if (m_visible)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw(m_bgTex, m_drawRectangle,null , m_color,0.0f,Vector2.Zero,SpriteEffects.None,depth);
                spriteBatch.End();
                m_scrollButton.Draw(spriteBatch, m_color, depth);
            }
        }
    }
}
