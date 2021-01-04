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

namespace DrawMaMon
{
    public delegate void ConnectionLostHandler();
    public delegate void AddTileMapHandler(string path);
    public delegate void ConnectionMadeHandler(string worldName);

    partial class Network
    {
        Texture2D m_playerTex;

        Hero m_hero;
        GameFiles m_gameFiles;
        string m_onlineWorldName;
        string m_onlinePath;

        TcpSocket m_serverSocket;
        Thread m_connectThread;
        bool m_connected = false;
        string m_ipAdress;
        int m_tilePixSize;

        string m_recevedTileMapFileName;
        int nTotalPacketsTileMap;
        int nPacketsTileMapReceived;
        List<byte> m_tileMapReveivedTileBytes;
        byte[] m_tileMapidentifier;

        List<Player> m_players;

        public ConnectionLostHandler ConnectionLostEvent;
        public ConnectionMadeHandler ConnectionMadeEvent;
        public AddTileMapHandler AddTileMapEvent;

        public Network(Texture2D playerTex,Hero hero,int tilePixSize,GameFiles gameFiles)
        {
            m_playerTex = playerTex;
            m_hero = hero;
            m_gameFiles = gameFiles;
            m_players = new List<Player>();
            m_tilePixSize = tilePixSize;
            m_onlinePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\DrawMamon\\Online";
        }

        public bool IsConnected() { return m_connected; }
        public Player GetPlayer(int i) { return m_players[i]; }
        public int GetNumPlayers(){return m_players.Count;}

        public void Connect(string ip)
        {
            m_ipAdress = ip;
            m_connectThread = new Thread(TryConnect);
            m_connectThread.Start();
        }

        void TryConnect()
        {
            while (!m_connected)
            {
                try
                {
                    TcpClient tempClientTcp = new TcpClient();
                    tempClientTcp.Connect(m_ipAdress, 34000);
                    m_serverSocket = new TcpSocket(tempClientTcp, 0);
                    m_serverSocket.NewMessageEvent += new NewMessageHandler(NewMessage);
                    m_serverSocket.DisconnectClientEvent += new DisconnectClientHandler(ConnectionLost);
                    m_serverSocket.StartClient();
                    m_connected = true;
                    SendFileInfo();
                    SendPlayerData(m_hero.player);
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }

        void NewMessage(byte[] message, int clientID)
        {
            DecodeMessage(message);
        }

        void ConnectionLost(int iD)
        {
            if (m_serverSocket != null)
            {
                m_serverSocket.StopClient();
                m_serverSocket = null;
                m_connected = false;
                ConnectionLostEvent();
            }
        }

        public void UpdatePlayers(int tilePixSize,Size numTilesScreen,Size mapSize)
        {
            if (m_connected)
            {
                for (int i = 0; i < m_players.Count; i++)
                {
                    Player player = m_players[i];
                    player.Update(tilePixSize, numTilesScreen.Width, numTilesScreen.Height, mapSize.Width, mapSize.Height);
                }
            }
        }
        public void DrawPlayers(SpriteBatch spriteBatch,Size tileScreenSize,int tilePixSize,int nLayers,Size windowSize,Camera cam)
            {
                for (int i = 0; i < m_players.Count; i++)
                {
                    m_players[i].Draw(spriteBatch, tileScreenSize, tilePixSize, nLayers, windowSize, cam);
                }
            }

        public void CleanUp()
        {
            SendRemovePlayer();
            m_connectThread.Abort();
            if (m_serverSocket != null)
            {
                m_serverSocket.StopClient();
            }
        }
    }
}