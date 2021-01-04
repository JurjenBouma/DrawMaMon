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
    class Label
    {
        Vector2 m_drawPos;
        string m_text;
        SpriteFont m_font;
        float m_fontSize;
        public Label(SpriteFont font, Point location, string text, float fontSize = 2.0f)
        {
            m_font = font;
            m_fontSize = fontSize;
            m_drawPos = new Vector2(location.X, location.Y);
            m_text = text;
        }
        public void SetText(string text) { m_text = text; }
        public string GetText() { return m_text; }

        public void Draw(SpriteBatch spriteBatch, Color color, float depth = 0.0f)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.DrawString(m_font, m_text, m_drawPos, color, 0.0f, Vector2.Zero, m_fontSize, SpriteEffects.None, depth);
            spriteBatch.End();
        }
    }
}
