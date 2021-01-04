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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace DrawMaMon
{
    partial class Network
    {
        void DecodeMessage(byte[] message)
        {
            if (message[0] == 0)
            {
                AddPlayer(message);
            }
            else if (message[0] == 1)
            {
                PlayerWalk(message);
            }
            else if (message[0] == 2)
            {
                SetPlayerDisplayTile(message);
            }
            else if (message[0] == 3)
            {
                RemovePlayer(message);
            }
            else if (message[0] == 4 || message[0] == 5)
            {
                AddTileMap(message);
            }
            else if (message[0] == 6)
            {
                m_gameFiles.SetFilePath(m_onlinePath);
                AddWorldData(message);
            }
            else if (message[0] == 7)
            {
                ConnectionMadeEvent(m_onlineWorldName);
            }
        }
        void AddPlayer(byte[] data)
        {
            int iD = data[1];
            Size tileSize = new Size(data[2], data[3]);
            Point pos = new Point(BitConverter.ToInt32(data, 4), BitConverter.ToInt32(data, 8));
            Point displayTile = new Point(data[12], data[13]);
            Player newPlayer = new Player(pos, m_playerTex, tileSize, m_tilePixSize);
            newPlayer.SetDisplayTile(displayTile);
            newPlayer.iD = iD;
            m_players.Add(newPlayer);
        }

        void RemovePlayer(byte[] data)
        {
            int iD = data[1];
            for (int i = 0; i < m_players.Count; i++)
            {
                if (m_players[i].iD == iD)
                {
                    m_players.RemoveAt(i);
                }
            }
        }

        void PlayerWalk(byte[] data)
        {
            int iD = data[1];
            Point dir = new Point(data[2]-1, data[3]-1);
            for (int i = 0; i < m_players.Count; i++)
            {
                if (m_players[i].iD == iD)
                {
                    m_players[i].WalkTile(dir, m_tilePixSize);
                }
            }
        }

        void SetPlayerDisplayTile(byte[] data)
        {
            int iD = data[1];
            Point displayTile  = new Point(data[2], data[3]);
            for (int i = 0; i < m_players.Count; i++)
            {
                if (m_players[i].iD == iD)
                {
                    m_players[i].SetDisplayTile(displayTile);
                }
            }
        }

        void AddWorldData(byte[] data)
        {
            int stringLenght = data[1];
            m_onlineWorldName = Encoding.ASCII.GetString(data, 2, stringLenght);
        }

        void AddTileMap(byte[] data)
        {
            if (data[0] == 4)
            {
                m_tileMapReveivedTileBytes = new List<byte>();
                List<byte> messageList = data.ToList();
                nTotalPacketsTileMap = BitConverter.ToInt32(messageList.GetRange(1, 4).ToArray(), 0);
                int nameLenght = messageList[5];
                m_recevedTileMapFileName = Encoding.ASCII.GetString(messageList.GetRange(6, nameLenght).ToArray());
                m_tileMapidentifier = messageList.GetRange(6+nameLenght, 4).ToArray();
                int numFileBytes = messageList[6 + nameLenght+4];
                m_tileMapReveivedTileBytes.AddRange(messageList.GetRange(7 + nameLenght+4, numFileBytes));
                nPacketsTileMapReceived = 1;
                if (nPacketsTileMapReceived == nTotalPacketsTileMap)
                {
                    SaveTileMap();
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
                    SaveTileMap();
                }
            }
        }

        void SaveTileMap()
        {
            if (!Directory.Exists(m_onlinePath + "\\Worlds\\" + m_onlineWorldName + "\\TileMaps"))
                Directory.CreateDirectory(m_onlinePath + "\\Worlds\\" + m_onlineWorldName + "\\TileMaps");

            Stream fileStream = new FileStream(m_onlinePath + "\\Worlds\\" + m_onlineWorldName + "\\TileMaps\\" + m_recevedTileMapFileName, FileMode.Create);
            BinaryWriter fileWriter = new BinaryWriter(fileStream);
            fileWriter.Write(m_tileMapReveivedTileBytes.ToArray());
            fileStream.Close();

            FileInfo fileInfo = new FileInfo(m_onlinePath + "\\Worlds\\" + m_onlineWorldName + "\\TileMaps\\" + m_recevedTileMapFileName);
            fileStream = new FileStream(m_onlinePath + "\\Worlds\\" + m_onlineWorldName + "\\TileMaps\\" + fileInfo.Name.Substring(0,fileInfo.Name.Length -fileInfo.Extension.Length) + ".ident", FileMode.Create);
            fileWriter = new BinaryWriter(fileStream);
            fileWriter.Write(m_tileMapidentifier);
            fileStream.Close();

            AddTileMapEvent(m_onlinePath + "\\Worlds\\" + m_onlineWorldName + "\\TileMaps\\" + m_recevedTileMapFileName);
        }
    }
}
