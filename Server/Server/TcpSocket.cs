using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace DrawMaMon
{
    public delegate void NewMessageHandler(byte[] message, int clientID);
    public delegate void DisconnectClientHandler(int clientID);
    class TcpSocket
    {
        TcpClient m_clientSocket;
        int m_clientID;
        Thread m_clientInputThread;
        byte[] m_recivedData;
        bool m_connected = true;

        public NewMessageHandler NewMessageEvent;
        public DisconnectClientHandler DisconnectClientEvent;

        public TcpSocket(TcpClient clientSocket, int clientID)
        {
            m_clientSocket = clientSocket;
            m_clientInputThread = new Thread(HandleClientInput);
            m_clientID = clientID;
            m_recivedData = new byte[300];
        }
        public int GetClientID() { return m_clientID; }
        public void SetClientID(int iD) { m_clientID = iD; }

        public void StartClient()
        {
            m_clientInputThread.Start();
        }

        public void StopClient()
        {
            DisconnectClientEvent(m_clientID);
            m_clientInputThread.Abort();
            m_clientSocket.Close();
        }

        public void SendMessage(byte[] message)
        {
            //WriteMessages
            if (m_connected)
            {
                try
                {
                    NetworkStream nStream = m_clientSocket.GetStream();
                    byte messageLenght = Convert.ToByte(message.Length);
                    byte[] fullMessage = new byte[messageLenght + 1];
                    fullMessage[0] = messageLenght;
                    for (int i = 0; i < messageLenght; i++)
                    {
                        fullMessage[i + 1] = message[i];
                    }
                    nStream.Write(fullMessage, 0, fullMessage.Length);
                    nStream.Flush();
                }
                catch
                {
                    m_connected = false;
                    DisconnectClientEvent(m_clientID);
                }
            }
        }

        void ProcessMessage(int messageLenght)
        {
            if (messageLenght > 0)
            {
                byte[] message = new byte[messageLenght];
                for (int i = 0; i < messageLenght; i++)
                {
                    message[i] = m_recivedData[1 + i];
                }
                NewMessageEvent(message, m_clientID);
            }
        }

        void HandleClientInput()
        {
            while (m_connected)
            {
                try
                {
                    if (m_clientSocket.GetStream().DataAvailable)
                    {
                        //ReadMessages
                        NetworkStream nStream = m_clientSocket.GetStream();

                        nStream.Read(m_recivedData, 0, 1);
                        int messageLenght = m_recivedData[0];
                        int numRead = nStream.Read(m_recivedData, 1, messageLenght);
                        while (numRead != messageLenght)
                        {
                            numRead += nStream.Read(m_recivedData, 1 + numRead, messageLenght - numRead);
                        }
                        ProcessMessage(messageLenght);
                        nStream.Flush();
                        m_recivedData = new byte[300];
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
