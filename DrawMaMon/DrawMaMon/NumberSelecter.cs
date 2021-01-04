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
    class NumberSelecter
    {
        Button m_buttonLeft;
        Button m_buttonRight;
        Size m_size;
        Point m_drawLocation;
        int m_currentValue;
        int m_minValue;
        int m_maxValue;
        Color m_color;
        SpriteFont m_font;
        float m_fontSize;
        MouseState prevMouseState;

        public NumberSelecter(Point drawLocation,Size size, int startNum,Color color, Texture2D leftTex,Texture2D rightTex,SpriteFont font,int minValue,int maxValue,float fontSize =4.0f)
        {
            m_drawLocation = drawLocation;
            m_size = size;
            m_currentValue = startNum;
            m_color = color;
            m_font = font;
            m_fontSize = fontSize;
            m_maxValue = maxValue;
            m_minValue = minValue;
       
            m_buttonLeft = new Button(m_drawLocation, new Size(m_size.Width / 3, m_size.Height), leftTex,"",null);
            m_buttonRight = new Button(new Point(m_drawLocation.X + 2*(m_size.Width / 3), m_drawLocation.Y), new Size(m_size.Width / 3, m_size.Height), rightTex,"",null);
            prevMouseState = Mouse.GetState();
        }
        public int GetValue() { return m_currentValue; }
        public void SetValue(int value) 
        { 
            if(value <= m_maxValue && value >= m_minValue)
            m_currentValue = value; 
        }
        public void SetMaxValue(int maxValue) { m_maxValue = maxValue; }
        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            m_buttonLeft.Draw(spriteBatch, m_color, depth);
            m_buttonRight.Draw(spriteBatch, m_color, depth);
            spriteBatch.Begin();
            spriteBatch.DrawString(m_font, m_currentValue.ToString(), new Vector2(m_drawLocation.X + (m_size.Width / 3) + ((m_size.Width / 3) / 2) - (m_font.MeasureString(m_currentValue.ToString()).X / 2) * m_fontSize, m_drawLocation.Y), m_color, 0.0f, Vector2.Zero, m_fontSize, SpriteEffects.None, depth);
            spriteBatch.End();
        }

        public void HandleInput()
        {
            MouseState mouseState = Mouse.GetState();
            if (m_buttonLeft.IsClicked() && mouseState.LeftButton != prevMouseState.LeftButton)
                if (m_currentValue - 1 >= m_minValue)
                    m_currentValue--;


            if (m_buttonRight.IsClicked() && mouseState.LeftButton != prevMouseState.LeftButton)
                if (m_currentValue + 1 <= m_maxValue)
                    m_currentValue++;

            prevMouseState = mouseState;
        }
    }
}
