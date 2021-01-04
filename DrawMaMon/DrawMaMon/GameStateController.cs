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
    class GameStateController
    {
        GameState m_gameMode;
        public GameStateController(GameState gameMode)
        {
            m_gameMode = gameMode;
        }

        public GameState GetGameState(){ return m_gameMode;  }

        public void SetGameState(GameState gameMode)
        {
            m_gameMode = gameMode;
        }
    }
}
