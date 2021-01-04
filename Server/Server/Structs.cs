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

        class TileMapIdentifier
        {
            public byte[] identifier;
            public string tileMapName;
            public TileMapIdentifier(byte[] ident, string MapName)
            {
                identifier = ident;
                tileMapName = MapName;
            }
        }

    }
}