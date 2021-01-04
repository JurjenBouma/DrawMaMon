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
    partial class Server
    {
        void DecodeMessage(byte[] message, int clientID)
        {
            if (message[0] == 0)
            {
                message[1] = Convert.ToByte(clientID);
                AddPlayer(message);
                SendToOthers(message, clientID);
            }
            else if (message[0] == 1)
            {
                message[1] = Convert.ToByte(clientID);
                UpdatePlayerPosition(message);
                SendToOthers(message, clientID);
            }
            else if (message[0] == 2)
            {
                message[1] = Convert.ToByte(clientID);
                UpdatePlayerDisplayTile(message);
                SendToOthers(message, clientID);
            }
            else if (message[0] == 3)
            {
                m_clientList[clientID].tcpSocket.StopClient();
            }
            else if (message[0] == 4 || message[0] == 5)
            {
                AddTileMap(message,clientID);
            }
            else if (message[0] == 8)
            {
                message[1] = Convert.ToByte(clientID);
                CheckTileMapIdentifier(message);
            }
            else if (message[0] == 9)
            {
                message[1] = Convert.ToByte(clientID);
                SendTileMaps(clientID);
            }
        }

        void UpdatePlayerPosition(byte[] data)
        {
            int iD = data[1];
            Point dir = new Point(data[2] - 1, data[3] - 1);
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == iD)
                {
                    m_clientList[i].player.SetLastWalkDir(dir);
                    Point newPos = m_clientList[i].player.GetPosition();
                    newPos.X += dir.X * m_tilePixSize;
                    newPos.Y += dir.Y * m_tilePixSize;
                    m_clientList[i].player.SetPosition(newPos);
                }
            }
        }
        void UpdatePlayerDisplayTile(byte[] data)
        {
            int iD = data[1];
            Point displayTile = new Point(data[2], data[3]);
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == iD)
                {
                    m_clientList[i].player.SetDisplayTile(displayTile);
                }
            }
        }

        void AddPlayer(byte[] data)
        {
            int iD = data[1];
            Size tileSize = new Size(data[2], data[3]);
            Point pos = new Point(BitConverter.ToInt32(data, 4), BitConverter.ToInt32(data, 8));
            Point displayTile = new Point(data[12], data[13]);
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == iD)
                {
                    m_clientList[i].player = new ServerPlayer(pos, displayTile, tileSize);
                }
            }
        }

        void CheckTileMapIdentifier(byte[] data)
        {
            List<byte> messageList = data.ToList();
            int iD = messageList[1];
            int nameLenght = messageList[2];
            string tileName = Encoding.ASCII.GetString(messageList.GetRange(3 , nameLenght).ToArray());
            byte[] ident = messageList.GetRange(3 + nameLenght, 4).ToArray();
            TileMapIdentifier identifier = new TileMapIdentifier(ident, tileName);
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == iD)
                {
                    m_clientList[i].tileMapIdendifiers.Add(identifier);
                }
            }
        }

        void AddTileMap(byte[] data,int clientID)
        {
            if (data[0] == 4)
            {
                m_tileMapReveivedTileBytes = new List<byte>();
                List<byte> messageList = data.ToList();
                nTotalPacketsTileMap = BitConverter.ToInt32(messageList.GetRange(1, 4).ToArray(), 0);
                int nameLenght = messageList[5];
                m_recevedTileMapFileName = Encoding.ASCII.GetString(messageList.GetRange(6, nameLenght).ToArray());
                int numFileBytes = messageList[6 + nameLenght];
                m_tileMapReveivedTileBytes.AddRange(messageList.GetRange(7 + nameLenght, numFileBytes));
                nPacketsTileMapReceived = 1;
                if (nPacketsTileMapReceived == nTotalPacketsTileMap)
                {
                    SaveTileMap(clientID);
                }
            }
            else if (data[0] == 5)
            {
                List<byte> messageList = data.ToList();
                int numFileBytes = messageList[1];
                m_tileMapReveivedTileBytes.AddRange(messageList.GetRange(2, numFileBytes));
                nPacketsTileMapReceived++;
                if (nPacketsTileMapReceived == nTotalPacketsTileMap)
                {
                    SaveTileMap(clientID);
                }
            }
        }
    }
}
