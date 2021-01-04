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

namespace DrawMaMon
{
    namespace Structs
    {
        class Size
        {
            public int Height;
            public int Width;
            public Size(int width,int height)
            {
                Height = height;
                Width = width;
            }
        }

        class TileLayer
        {
            public Point Position;
            public int NumTileMap;

            public TileLayer(Point pos, int numTileMap)
            {
                Position = pos;
                NumTileMap = numTileMap;
            }

            public void SetTile(Point pos, int numTileMap)
            {
                Position = pos;
                NumTileMap = numTileMap;
            }
        }

        class Tile
        {
            public TileLayer[] tileLayers;
            
            public Tile(Point pos,int numTileMap,int numLayers)
            {
                tileLayers = new TileLayer[numLayers];
                for (int i = 0; i < numLayers; i++)
                {
                    tileLayers[i] = new TileLayer(pos,numTileMap);
                }
            }           
        }

        enum GameState {MainMenu,Play,TileEdit,EscapeMenu}
        enum MainMenuState { Main,GameSelect ,NewGame,ServerSelect}
    }
}