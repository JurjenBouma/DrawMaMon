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
    class Button
    {
        Rectangle m_drawRectangle;
        Texture2D m_buttonTex;
        string m_text;
        SpriteFont m_font;
        bool m_drawText;
        Vector2 m_textPos;
        float m_fontSize;

        public Button(Point drawLocation, Size size, Texture2D tex,string text,SpriteFont font,float fontSize = 2.0f)
        {
            m_buttonTex = tex;
            m_text = text;
            m_fontSize = fontSize;
            m_drawRectangle = new Rectangle(drawLocation.X, drawLocation.Y, size.Width, size.Height);
            if (font == null)
                m_drawText = false;
            else
            {
                m_drawText = true;
                m_font = font;
                m_textPos = new Vector2(m_drawRectangle.X + m_drawRectangle.Width / 2 - (m_font.MeasureString(m_text).X / 2) * fontSize, m_drawRectangle.Y + m_drawRectangle.Height / 2 - (m_font.MeasureString(m_text).Y / 2) * m_fontSize);
            }
           
        }

        public Point GetLocation()
        {
            return new Point(m_drawRectangle.X, m_drawRectangle.Y);
        }
        public void SetLocation(Point location)
        {
            m_drawRectangle.X = location.X;
            m_drawRectangle.Y = location.Y;
            if (m_drawText)
            {
                m_textPos = new Vector2(m_drawRectangle.X + m_drawRectangle.Width / 2 - (m_font.MeasureString(m_text).X / 2) * m_fontSize, m_drawRectangle.Y + m_drawRectangle.Height / 2 - (m_font.MeasureString(m_text).Y / 2) * m_fontSize);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color color,float depth)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_buttonTex, m_drawRectangle, null, color, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            if (m_drawText)
            {
                spriteBatch.DrawString(m_font, m_text, m_textPos, Color.Black, 0.0f, Vector2.Zero, m_fontSize,SpriteEffects.None,depth);
            }
            spriteBatch.End();
        }
        public void Draw(SpriteBatch spriteBatch, Color color,Color textColor, float depth)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_buttonTex, m_drawRectangle, null, color, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
            if (m_drawText)
            {
                spriteBatch.DrawString(m_font, m_text, m_textPos, textColor, 0.0f, Vector2.Zero, m_fontSize, SpriteEffects.None, depth);
            }
            spriteBatch.End();
        }

        public bool IsClicked()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseState.X >= m_drawRectangle.X && mouseState.X < (m_drawRectangle.X + m_drawRectangle.Width))
                {
                    if (mouseState.Y >= m_drawRectangle.Y && mouseState.Y < (m_drawRectangle.Y + m_drawRectangle.Height))
                    {
                        Vector2 scale = new Vector2((float)m_drawRectangle.Width / m_buttonTex.Width, (float)m_drawRectangle.Height / m_buttonTex.Height);
                        Point localScaledMousePos = new Point((int)((mouseState.X - m_drawRectangle.X) /scale.X), (int)((mouseState.Y - m_drawRectangle.Y)/scale.Y));
           
                        if(localScaledMousePos.X >= 0 && localScaledMousePos.X < m_buttonTex.Width)
                        {
                            if (localScaledMousePos.Y >= 0 && localScaledMousePos.Y < m_buttonTex.Height)
                            {
                                Color[] clickPixel = new Color[m_buttonTex.Width * m_buttonTex.Height];
                                m_buttonTex.GetData<Color>(clickPixel);
                          
                                if (clickPixel[localScaledMousePos.X + localScaledMousePos.Y* m_buttonTex.Width].A > 0)
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

    }
}
