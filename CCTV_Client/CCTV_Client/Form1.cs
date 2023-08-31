using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCTV_Client
{
    public partial class Form1 : Form
    {
        #region 변수
        Socket server;

        private TcpClient Client;

        int m_port = 5000;
        private bool IsConnected = false;
        #endregion

        public void Connect()
        {
            try
            {
                if (IsConnected == false)
                {
                    Client = new TcpClient();
                    Client.LingerState = new LingerOption(true, 1);
                    Client.Connect(IPAddress.Parse("127.0.0.1"), 5000);
                    IsConnected = true;
                }

            }
            catch (Exception ex)
            {

            }
        }
        public void Close()
        {
            if (server != null)
            {
                server.Close();
                server.Dispose();
            }
        }
        public class AsyncObject
        {
            public byte[] Buffer;
            public Socket WorkingSocket;
            public readonly int BufferSize;
            public AsyncObject(int bufferSize)
            {
                BufferSize = bufferSize;
                Buffer = new byte[(long)BufferSize];
            }

            public void ClearBuffer()
            {
                Array.Clear(Buffer, 0, BufferSize);
            }
        }
        void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                AsyncObject obj = new AsyncObject(4096);
                obj.WorkingSocket = server;
                server.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

                MessageBox.Show("붙음");

                Send(Encoding.UTF8.GetBytes("123"));
            }
            catch (Exception e)
            {
            }
        }
        void DataReceived(IAsyncResult ar)
        {
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            int received = obj.WorkingSocket.EndReceive(ar);

            byte[] buffer = new byte[received];

            Array.Copy(obj.Buffer, 0, buffer, 0, received);
        }
        public void Send(byte[] msg)
        {
            server.Send(msg);
        }

        public Form1()
        {
            InitializeComponent();

            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Connect();
        }
    }
}
