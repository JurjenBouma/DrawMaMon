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
    public delegate void EnterPressedEventHandler();

    class TextBox
    {
        string m_text;
   
        Texture2D m_bGTex;
        Rectangle m_drawRectangle;

        SpriteFont m_font;
        float m_fontSize;
        KeyInput keyInput;

        bool m_inputFocus = false;
        string m_inputCursor;
        int m_inputCursorCounter;
        string m_drawString;

        public EnterPressedEventHandler EnterPressedEvent;

        public TextBox(Texture2D backgroundTex ,SpriteFont font,Point drawLocation,Size size,string inputCursor = "|",float fontSize = 1.5f)
        {
            m_font = font;
            m_fontSize = fontSize;
            m_text = "";
            m_inputCursor = inputCursor;
            m_bGTex = backgroundTex;
            m_drawRectangle = new Rectangle(drawLocation.X, drawLocation.Y, size.Width, size.Height);
            keyInput = new KeyInput(40);
        }
        public void GiveFocus() { m_inputFocus = true; }
        public void TakeFocus() { m_inputFocus = false; }
        public bool IsInputFocus() { return m_inputFocus; }
        public string GetText() { return m_text; }

        public void Draw(SpriteBatch spriteBatch,Color bgColor,float depth)
        {
            m_drawString = m_text;
            while (m_font.MeasureString(m_drawString).X + 5 + m_font.MeasureString(m_inputCursor).X > m_drawRectangle.Width)
            {
                m_drawString = m_drawString.Substring(1);
            }

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_bGTex, m_drawRectangle,null, bgColor,0.0f,Vector2.Zero,SpriteEffects.None,depth);
            spriteBatch.DrawString(m_font, m_drawString, new Vector2(m_drawRectangle.X + 5, m_drawRectangle.Y + m_drawRectangle.Height / 2 - m_font.MeasureString(m_text).Y / 2), Color.Black, 0.0f, Vector2.Zero, m_fontSize,SpriteEffects.None,depth);
            spriteBatch.End();
            DrawInputCursor(spriteBatch,depth);
        }

        void DrawInputCursor(SpriteBatch spriteBatch,float depth)
        {
            if (m_inputFocus)
            {
                if (m_inputCursorCounter >= 30)
                {
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                    spriteBatch.DrawString(m_font, m_inputCursor, new Vector2(m_drawRectangle.X + 5 + m_font.MeasureString(m_drawString).X, m_drawRectangle.Y + m_drawRectangle.Height / 2 - m_font.MeasureString(m_inputCursor).Y / 2), Color.Black, 0.0f, Vector2.Zero, m_fontSize,SpriteEffects.None,depth);
                    spriteBatch.End();
                }
                if (m_inputCursorCounter == 60)
                    m_inputCursorCounter = -1;
                m_inputCursorCounter++;
            }
        }

        public void HandleInput()
        {
            if (m_inputFocus)
            {
                string newText = keyInput.AddTextInput(m_text);
                if (m_text != newText)
                {
                    m_text = newText;
                    m_inputCursorCounter = 30;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    TakeFocus();
                    if (EnterPressedEvent != null)
                    {
                        EnterPressedEvent();
                    }
                }
            }
            if (IsClicked())
            {
                GiveFocus();
            }
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
                        Vector2 scale = new Vector2((float)m_drawRectangle.Width / m_bGTex.Width, (float)m_drawRectangle.Height / m_bGTex.Height);
                        Point localScaledMousePos = new Point((int)((mouseState.X - m_drawRectangle.X) / scale.X), (int)((mouseState.Y - m_drawRectangle.Y) / scale.Y));
           

                        if (localScaledMousePos.X >= 0 && localScaledMousePos.X < m_bGTex.Width)
                        {
                            if (localScaledMousePos.Y >= 0 && localScaledMousePos.Y < m_bGTex.Height)
                            {
                                Color[] clickPixel = new Color[m_bGTex.Width * m_bGTex.Height];
                                m_bGTex.GetData<Color>(clickPixel);

                                if (clickPixel[localScaledMousePos.X + localScaledMousePos.Y * m_bGTex.Width].A > 0)
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
