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
    class MenuStateController
    {
        MainMenuState m_menuState;
        public MenuStateController(MainMenuState startState)
        {
            m_menuState = startState;
        }

        public MainMenuState GetMenuState() { return m_menuState; }

        public void SetMenuState(MainMenuState menuState)
        {
            m_menuState = menuState;
        }
    }
}
