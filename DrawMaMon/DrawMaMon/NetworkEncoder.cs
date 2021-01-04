using System;
using System.Text;
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

namespace DrawMaMon
{
    partial class Network
    {
        void SendPlayerData(Player player)
        {
            if (m_connected)
            {
                List<byte> playerData = new List<byte>();
                playerData.Add(0);
                playerData.Add(Convert.ToByte(0));
                playerData.Add(Convert.ToByte(player.GetPlayerTileSize().Width));
                playerData.Add(Convert.ToByte(player.GetPlayerTileSize().Height));

                //Add positionInfo
                byte[] xBytes = BitConverter.GetBytes(player.GetPosition().X);
                for (int n = 0; n < 4; n++)
                {
                    playerData.Add(xBytes[n]);
                }
                byte[] yBytes = BitConverter.GetBytes(player.GetPosition().Y);
                for (int n = 0; n < 4; n++)
                {
                    playerData.Add(yBytes[n]);
                }

                playerData.Add(Convert.ToByte(player.GetDisplayTile().X));
                playerData.Add(Convert.ToByte(player.GetDisplayTile().Y));

                m_serverSocket.SendMessage(playerData.ToArray());
            }
        }

        public void SendPlayerWalk(Point Direction)
        {
            if (m_connected)
            {
                List<byte> walkData = new List<byte>();
                walkData.Add(1);
                walkData.Add(Convert.ToByte(0));
                walkData.Add(Convert.ToByte(Direction.X + 1));
                walkData.Add(Convert.ToByte(Direction.Y + 1));
                m_serverSocket.SendMessage(walkData.ToArray());
            }
        }

        public void SendPlayerDisplayTile(Point displayTile)
        {
            if (m_connected)
            {
                List<byte> displayTileData = new List<byte>();
                displayTileData.Add(2);
                displayTileData.Add(Convert.ToByte(0));
                displayTileData.Add(Convert.ToByte(displayTile.X));
                displayTileData.Add(Convert.ToByte(displayTile.Y));
                m_serverSocket.SendMessage(displayTileData.ToArray());
            }
        }

        public void SendRemovePlayer()
        {
            if (m_connected)
            {
                List<byte> removeData = new List<byte>();
                removeData.Add(3);
                removeData.Add(Convert.ToByte(0));
                m_serverSocket.SendMessage(removeData.ToArray());
            }
        }

        void SendFileInfo()
        {
            foreach (string file in Directory.GetFiles(m_onlinePath + "\\Worlds\\" + m_onlineWorldName + "\\TileMaps"))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".ident")
                {
                    SendTileMapIdentifier(file);
                }
            }
            SendAllTileIdentSend();
        }

        void SendAllTileIdentSend()
        {
            List<byte> DoneBytesList = new List<byte>();
            DoneBytesList.Add(9);
            DoneBytesList.Add(0);
            m_serverSocket.SendMessage(DoneBytesList.ToArray());
        }

        void SendTileMapIdentifier(string filePath)
        {
            Stream fileStream = new FileStream(filePath, FileMode.Open);
            BinaryReader fileReader = new BinaryReader(fileStream);
            byte[] identifier = fileReader.ReadBytes(4);
            fileStream.Close();
            FileInfo fileInfo = new FileInfo(filePath);
            List<byte> identBytesList = new List<byte>();
            byte[] nameTileMapBytes = Encoding.ASCII.GetBytes((fileInfo.Name));
            identBytesList.Add(8);
            identBytesList.Add(0);
            identBytesList.Add(Convert.ToByte(nameTileMapBytes.Length));
            identBytesList.AddRange(nameTileMapBytes);
            identBytesList.AddRange(identifier);
            m_serverSocket.SendMessage(identBytesList.ToArray());
        }


        public void SendAddTileMap(string path)
        {
            if (m_connected)
            {
                Stream fileStream = new FileStream(path, FileMode.Open);
                BinaryReader fileReader = new BinaryReader(fileStream);
                FileInfo fileInfo = new FileInfo(path);
                byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileInfo.Name);
                byte[] fileBytes = fileReader.ReadBytes(Convert.ToInt32(fileStream.Length));
                List<byte> fileBytesList = fileBytes.ToList();

                int numPackets = (fileBytesList.Count + 49) / 50;
                for (int i = 0; i < numPackets; i++)
                {
                    int packetSize = 50;
                    if (i == numPackets - 1)
                        packetSize = fileBytesList.Count % 50;

                    List<byte> message = new List<byte>();
                    if (i == 0)
                    {
                        message.Add(4);
                        message.AddRange(BitConverter.GetBytes(numPackets));
                        message.Add(Convert.ToByte(fileNameBytes.Length));
                        message.AddRange(fileNameBytes);
                        message.Add(Convert.ToByte(packetSize));
                        message.AddRange(fileBytesList.GetRange(i * 50, packetSize));
                        m_serverSocket.SendMessage(message.ToArray());
                    }
                    else
                    {
                        message.Add(5);
                        message.Add(Convert.ToByte(packetSize));
                        message.AddRange(fileBytesList.GetRange(i * 50, packetSize));
                        m_serverSocket.SendMessage(message.ToArray());
                    }
                }
                fileStream.Close();
            }
        }
    }
}
