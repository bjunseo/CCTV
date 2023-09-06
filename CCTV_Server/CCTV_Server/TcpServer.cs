using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CCTV_Server
{
    class TcpServer
    {
        public delegate void ReceiveData(string ID, string num, string data);
        public ReceiveData RunEvent;


        private Thread thread = null;



        #region 변수
        Socket server;

        List<Socket> connectedClients = new List<Socket>();

        int m_port = 5000;

        private Thread ReceiveThread;
        private NetworkStream NetworkStream;
        private StreamReader StreamReader;
        private StreamWriter StreamWriter;
        #endregion

        public TcpServer() 
        {
            thread = new Thread(startThread);
            thread.IsBackground = true;
            thread.Start();
        }

        private void startThread()
        {
            try
            {
                int count = 0;
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 5000);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(ipe);
                server.Listen(10000);

                while (true)
                {
                    Socket client = server.Accept();
                    IPEndPoint ip = (IPEndPoint)client.RemoteEndPoint;
                    
                    connectedClients.Add(client);

                    //client[count] = server.Accept();

                    Thread v = new Thread(() => _Receive(count, ip))
                    {
                        IsBackground = true

                    };
                    v.Start();

                    count++;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void _Receive(int count1, IPEndPoint ip)
        {
            string _buf = "";
            byte[] _data;
             
            while (true)
            {
                while (true)
                {
                    try
                    {
                        _data = new Byte[1024];
                        int bytesRec = ((Socket)connectedClients[(int)count1 - 1]).Receive(_data);
                        _buf += Encoding.UTF8.GetString(_data, 0, bytesRec);
                        Thread.Sleep(1000);

                        if (_buf.Trim().Length > 0)
                        {
                            string[] info = _buf.Split(',');
                            
                            RunEvent(info[0] ,info[1], info[2]);
                            

                            break;
                        }
                    }
                    catch (SocketException ex)
                    {
                    }
                }

                _buf = "";
            }
        }

        public void sendData()
        {
            connectedClients[0].Send(System.Text.Encoding.UTF8.GetBytes("8"));
        }
    }
}
