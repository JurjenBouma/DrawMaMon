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
    public delegate void JoinGameClickHandler(string ipAdress);
    class ServerSelectMenu
    {
        Rectangle m_windowDrawRectangle;
        Texture2D m_bGTex;

        Size m_serverSelectBoxSize;
        Point m_serverSelectBoxLoc;
        SelectBox m_serverSelectBox;

        Size m_joinButtonSize;
        Point m_joinButtonLocation;
        Button m_joinButton;

        MouseState m_prevMouseState;

        public JoinGameClickHandler JoinGameClickEvent;

        public ServerSelectMenu(Texture2D backGroundTex, Texture2D joinButtonTex, Texture2D gameSelectBoxTex, Texture2D selectedBoxTex, Texture2D scrollBarBGTex, Texture2D scrollBarButtonTex, Texture2D serverIcon, Point screenOffset, Size windowSize, SpriteFont font)
        {
            m_bGTex = backGroundTex;
            m_windowDrawRectangle = new Rectangle(screenOffset.X, screenOffset.Y, windowSize.Width - screenOffset.X * 2, windowSize.Height - screenOffset.Y * 2);

            m_serverSelectBoxSize = new Size(640, 512);
            m_serverSelectBoxLoc = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) -m_serverSelectBoxSize.Width/2, screenOffset.Y + (windowSize.Height / 2) - 200);
            m_serverSelectBox = new SelectBox(gameSelectBoxTex, selectedBoxTex, scrollBarBGTex, scrollBarButtonTex, font, m_serverSelectBoxLoc, m_serverSelectBoxSize, new Size(32, 32));

            m_joinButtonSize = new Size(256, 64);
            m_joinButtonLocation = new Point(screenOffset.X + ((windowSize.Width - screenOffset.X * 2) / 2) - m_joinButtonSize.Width / 2, screenOffset.Y + (windowSize.Height / 2) + 300);
            m_joinButton = new Button(m_joinButtonLocation, m_joinButtonSize, joinButtonTex, "Join", font);

            m_prevMouseState = Mouse.GetState();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(m_bGTex, m_windowDrawRectangle, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.End();
            m_serverSelectBox.Draw(spriteBatch, 0.0f);
            m_joinButton.Draw(spriteBatch, Color.White, 0.0f);
        }
        public void HandleInput()
        {
            MouseState mouseState = Mouse.GetState();
            m_serverSelectBox.HandleInput();
            if (m_joinButton.IsClicked() && mouseState.LeftButton != m_prevMouseState.LeftButton)
            {
                JoinGameClickEvent("192.168.0.177");
            }
            m_prevMouseState = mouseState;
        }
    }
}
