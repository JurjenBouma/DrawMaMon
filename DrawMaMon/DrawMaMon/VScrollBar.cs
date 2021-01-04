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
    class VScrollBar
    {
        int m_currentValue;
        int m_maxValue;
        Texture2D m_bgTex;
        Button m_scrollButton;
        Rectangle m_drawRectangle;
        Color m_color;
        bool m_visible = false;
        int m_buttonLenght;
        bool m_buttonClicked = false;
        MouseState m_prevMouseState;

        public VScrollBar(Point drawLocation, Size size, int buttonLenght, Texture2D bgTex, Texture2D scrollButtonTex, int startValue, int maxValue, Color color)
        {
            m_drawRectangle = new Rectangle(drawLocation.X, drawLocation.Y, size.Width, size.Height);
            m_currentValue = startValue;
            m_color = color;
            m_maxValue = maxValue;
            m_bgTex = bgTex;
            m_buttonLenght = buttonLenght;
            m_scrollButton = new Button(drawLocation, new Size(size.Width, buttonLenght), scrollButtonTex, "", null);
            m_prevMouseState = Mouse.GetState();
        }
        public void Show(){ m_visible = true; }
        public void Hide() { m_visible = false; }
        public int GetValue() { return m_currentValue; }
        public void SetMaxValue(int value)
        {
            m_maxValue = value;
            m_currentValue = (int)(((float)m_scrollButton.GetLocation().Y - m_drawRectangle.Y) / ((float)m_drawRectangle.Height - m_buttonLenght) * m_maxValue);
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
                        Point newLocation = new Point(oldLocation.X, oldLocation.Y + (mouseState.Y - m_prevMouseState.Y));
                        if (newLocation.Y < m_drawRectangle.Y)
                            newLocation.Y = m_drawRectangle.Y;
                        if (newLocation.Y > m_drawRectangle.Y + (m_drawRectangle.Height - m_buttonLenght))
                            newLocation.Y = m_drawRectangle.Y + (m_drawRectangle.Height - m_buttonLenght);
                        m_scrollButton.SetLocation(newLocation);
                    }
                    else
                    {
                        m_buttonClicked = false;
                    }
                }
                m_prevMouseState = Mouse.GetState();

                m_currentValue = (int)(((float)m_scrollButton.GetLocation().Y - m_drawRectangle.Y)/((float)m_drawRectangle.Height - m_buttonLenght)*m_maxValue);
            }
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            if (m_visible)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw(m_bgTex, m_drawRectangle,null, m_color,0.0f,Vector2.Zero,SpriteEffects.None,depth);
                spriteBatch.End();
                m_scrollButton.Draw(spriteBatch, m_color, depth);
            }
        }
    }
}
