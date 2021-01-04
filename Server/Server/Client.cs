using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DrawMaMon.Structs;

namespace DrawMaMon
{
    class Client
    {
        public TcpSocket tcpSocket;
        public ServerPlayer player;
        public int iD;
        public List<TileMapIdentifier> tileMapIdendifiers;
        public Client(TcpSocket socket, int clientID)
        {
            tileMapIdendifiers = new List<TileMapIdentifier>();
            iD = clientID;
            tcpSocket = socket;
        }
    }
}
