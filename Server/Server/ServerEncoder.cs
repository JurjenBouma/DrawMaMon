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
using System.IO;

namespace DrawMaMon
{
    partial class Server
    {
        void SendAddPlayerTo(int clientID, ServerPlayer player, int iD)
        {
            if (player != null)
            {
                List<byte> playerData = new List<byte>();
                playerData.Add(0);
                playerData.Add(Convert.ToByte(iD));
                playerData.Add(Convert.ToByte(player.GetTileSize().Width));
                playerData.Add(Convert.ToByte(player.GetTileSize().Height));

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

                for (int i = 0; i < m_clientList.Count; i++)
                {
                    if (m_clientList[i].iD == clientID)
                    {
                        m_clientList[i].tcpSocket.SendMessage(playerData.ToArray());
                    }
                }
            }
        }

        void SendRemovePlayerTo(int clientID, int RemovedClientID)
        {
            List<byte> removeData = new List<byte>();
            removeData.Add(3);
            removeData.Add(Convert.ToByte(RemovedClientID));
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == clientID)
                {
                    m_clientList[i].tcpSocket.SendMessage(removeData.ToArray());
                }
            }
        }

        void SendWorldData(int clientID)
        {
            List<byte> worldData = new List<byte>();
            worldData.Add(6);
            worldData.Add(Convert.ToByte(Encoding.ASCII.GetByteCount(m_worldName)));
            worldData.AddRange(Encoding.ASCII.GetBytes(m_worldName));
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == clientID)
                {
                    m_clientList[i].tcpSocket.SendMessage(worldData.ToArray());
                }
            }
        }

        void SendStartPlay(int clientID)
        {
            List<byte> playData = new List<byte>();
            playData.Add(7);
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == clientID)
                {
                    m_clientList[i].tcpSocket.SendMessage(playData.ToArray());
                }
            }
        }

        public void SendAddTileMap(int clientID, string path)
        {
            Stream fileStream = new FileStream(path, FileMode.Open);
            BinaryReader fileReader = new BinaryReader(fileStream);
            FileInfo fileInfo = new FileInfo(path);
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileInfo.Name);
            byte[] fileBytes = fileReader.ReadBytes(Convert.ToInt32(fileStream.Length));
            List<byte> fileBytesList = fileBytes.ToList();
            fileStream.Close();

            fileStream = new FileStream(path.Substring(0,path.Length - fileInfo.Extension.Length) + ".ident" , FileMode.Open);
            fileReader = new BinaryReader(fileStream);
            byte[] fileIdentBytes = fileReader.ReadBytes(Convert.ToInt32(fileStream.Length));
            fileStream.Close();

            int numPackets = (fileBytesList.Count + 49) / 50;
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == clientID)
                {
                    for (int p = 0; p < numPackets; p++)
                    {
                        int packetSize = 50;
                        if (p == numPackets - 1)
                            packetSize = fileBytesList.Count % 50;

                        List<byte> message = new List<byte>();
                        if (p == 0)
                        {
                            message.Add(4);
                            message.AddRange(BitConverter.GetBytes(numPackets));
                            message.Add(Convert.ToByte(fileNameBytes.Length));
                            message.AddRange(fileNameBytes);
                            message.AddRange(fileIdentBytes);
                            message.Add(Convert.ToByte(packetSize));
                            message.AddRange(fileBytesList.GetRange(p * 50, packetSize));
                            m_clientList[i].tcpSocket.SendMessage(message.ToArray());
                        }
                        else
                        {
                            message.Add(5);
                            message.Add(Convert.ToByte(packetSize));
                            message.AddRange(fileBytesList.GetRange(p * 50, packetSize));
                            m_clientList[i].tcpSocket.SendMessage(message.ToArray());
                        }
                    }
                }
            }
        }
    }
}

