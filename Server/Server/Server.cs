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
    public delegate void ClientConnectedHandler();
    public delegate void ClientDisconnectedHandler();

    partial class Server
    {
        TcpListener m_listener;
        List<Client> m_clientList;
        Thread m_serverThread;
        int m_tilePixSize = 16;
        int m_newID = 0;

        Random m_randomGen;

        string m_savePath;
        string m_worldName;
        int m_nTileMaps=0;

        string m_recevedTileMapFileName;
        int nTotalPacketsTileMap;
        int nPacketsTileMapReceived;
        List<byte> m_tileMapReveivedTileBytes;

        public ClientConnectedHandler ClientConnectEvent;
        public ClientDisconnectedHandler ClientDisconnectEvent;

        public Server(string savePath)
        {
            m_clientList = new List<Client>();
            m_listener = new TcpListener(IPAddress.Any, 34000);
            m_savePath = savePath;
            m_worldName = "OnlineWorld";
            m_randomGen = new Random();

            if (!Directory.Exists(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps"))
            {
                Directory.CreateDirectory(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps");
            }
            else
            {
                foreach (string file in Directory.GetFiles(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps"))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension == ".png")
                    {
                        m_nTileMaps++;
                    }
                }
            }
        }
        public int GetClientCount() { return m_clientList.Count; }
        public void StartServer()
        {
            m_listener.Start();
            m_serverThread = new Thread(LookForClients);
            m_serverThread.Start();
        }

        public void StopServer()
        {
            for (int i = 0; i < m_clientList.Count; i++)
            {
                m_clientList[i].tcpSocket.StopClient();
            }
            m_serverThread.Abort();
            m_listener.Stop();
        }

        void SendAllPlayerDataTo(int clientID)
        {
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD != clientID)
                {
                    SendAddPlayerTo(clientID, m_clientList[i].player, m_clientList[i].iD);
                }
            }
        }

        void SendToOthers(byte[] message,int senderClientID)
        {
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD != senderClientID)
                {
                    m_clientList[i].tcpSocket.SendMessage(message);
                }
            }
        }

        void SaveTileMap(int clientID)
        {
            if (!Directory.Exists(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps"))
                Directory.CreateDirectory(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps");

            Stream fileStream = new FileStream(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps\\tilemap" + m_nTileMaps.ToString() + ".ident", FileMode.Create);
            BinaryWriter fileWriter = new BinaryWriter(fileStream);
            byte[] identifier = BitConverter.GetBytes(m_randomGen.Next(999999999));
            fileWriter.Write(identifier);
            fileStream.Close();

            fileStream = new FileStream(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps\\tilemap" + m_nTileMaps.ToString() + ".png", FileMode.Create);
            fileWriter = new BinaryWriter(fileStream);
            fileWriter.Write(m_tileMapReveivedTileBytes.ToArray());
            fileStream.Close();
          
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD != clientID)
                {
                    SendAddTileMap(i, m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps\\tilemap" + m_nTileMaps.ToString() + ".png");
                }
            }
            m_nTileMaps++;
        }

        void ConnectClient()
        {
            Client client = new Client(new TcpSocket(m_listener.AcceptTcpClient(), m_newID), m_newID);
            m_clientList.Add(client);
            m_clientList[m_clientList.Count -1].tcpSocket.NewMessageEvent += new NewMessageHandler(NewMessage);
            m_clientList[m_clientList.Count - 1].tcpSocket.DisconnectClientEvent += new DisconnectClientHandler(RemoveClient);
            m_clientList[m_clientList.Count - 1].tcpSocket.StartClient();
            SendWorldData(m_newID);
           
            SendAllPlayerDataTo(m_newID);
            SendStartPlay(m_newID);
            m_newID++;
            ClientConnectEvent();
        }

        void LookForClients()
        {
            while (true)
            {
                if (!m_listener.Pending())
                {
                    Thread.Sleep(1000);
                    continue;
                }
                ConnectClient();
            }
        }

        void SendTileMaps(int clientID)
        {
            foreach (string file in Directory.GetFiles(m_savePath + "\\Worlds\\" + m_worldName + "\\TileMaps"))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".ident")
                {
                    //Check in m_client voor identifiers, als ze niet gelijk zijn stuur tilemap
                    /*
                    Stream fileStream = new FileStream(file, FileMode.Open);
                    BinaryReader fileReader = new BinaryReader(fileStream);
                    byte[] identifier = fileReader.ReadBytes(4);
                    fileStream.Close();
                    */
                }
            }
        }

        void NewMessage(byte[] message, int clientID)
        {
            DecodeMessage(message, clientID);
        }

        void RemoveClient(int clientID)
        {
            int removeIndex =0;
            for (int i = 0; i < m_clientList.Count; i++)
            {
                if (m_clientList[i].iD == clientID)
                {
                    removeIndex = i;
                }
                else
                    SendRemovePlayerTo(m_clientList[i].iD, clientID);
            }
            m_clientList.RemoveAt(removeIndex);

            ClientDisconnectEvent();
        }
    }
}
