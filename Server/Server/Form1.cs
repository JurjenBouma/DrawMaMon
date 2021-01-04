using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DrawMaMon
{
    public partial class Form1 : Form
    {
        Server server;
        public Form1()
        {
            InitializeComponent();
            server = new Server(Application.StartupPath);
            server.ClientConnectEvent += new ClientConnectedHandler(Connected);
            server.ClientDisconnectEvent += new ClientDisconnectedHandler(Disconnected);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server.StartServer();
        }

        void Connected()
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(Connected));
                return;
            }
            label2.Text = (server.GetClientCount().ToString());
        }

        void Disconnected()
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(Connected));
                return;
            }
            label2.Text = (server.GetClientCount().ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.StopServer();
        }
    }
}
