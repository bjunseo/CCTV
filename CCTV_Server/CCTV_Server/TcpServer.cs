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
using System.Collections;

namespace CCTV_Server
{
    class TcpServer
    {
        #region 변수

        public class socketInfo
        {
            public Socket socket;
            public string ID;

            public socketInfo(Socket socket, string iD)
            {
                this.socket = socket;
                ID = iD;
            }
        }

        public delegate void ReceiveData(string type, string data);
        public ReceiveData RunEvent;


        private Thread thread = null;

        Socket server;

        public List<Socket> connectedClients = new List<Socket>();
        public List<socketInfo> clientInfo = new List<socketInfo>();

        private int Port;
        private ArrayList SocketClients;
        private string masterIP;

        private Thread ReceiveThread;
        private NetworkStream NetworkStream;
        private StreamReader StreamReader;
        private StreamWriter StreamWriter;

        System.Net.Sockets.Socket tcpServer;
        #endregion

        #region 생성자
        public TcpServer(int port)
        {
            this.Port = port;
            this.SocketClients = new ArrayList();
        }
        #endregion

        #region StartServer
        public void StartServer()
        {
            try
            {
                if (Port == 0)
                {
                    return;
                }


                // Open Tcp Socket Server!
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 6000);
                tcpServer = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpServer.Bind(endPoint);
                tcpServer.Listen(50);

                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);

                // 소켓 연결 대기
                tcpServer.AcceptAsync(socketArgs);

                RunEvent("MSG", IPAddress.Any + " Event Server Open Start");

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        #endregion

        #region Accept_Completed
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                System.Net.Sockets.Socket clientSocket = e.AcceptSocket;
                IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;

                if (clientSocket.Connected)
                {
                    // 비인가 IP에서 서버 접속시 소켓 연결 종료
                    string clientIP = clientEndPoint.Address.ToString();
                    if (!clientIP.StartsWith("192.168.10") && !clientIP.Equals("127.0.0.1"))
                    {
                        //CallEvent("UNKNOWN", clientIP);
                        clientSocket.Close();
                        return;
                    }

                    // 중복 소켓 접속 시 기존 접속 클라이언트는 종료
                    foreach (TcpClient socketClient in SocketClients)
                    {
                        if (clientIP == socketClient.ClientIP)
                        {
                            //CallEvent("DELETE", clientIP);
                            socketClient.Close();
                            SocketClients.Remove(socketClient);
                            break;
                        }
                    }

                    TcpClient client = new TcpClient(clientSocket);
                    // 소켓 클라이언트 이벤트 추가
                    client.clientEvent += Client_EventProcess;

                    // 클라이언트 접속성공 처리
                    SocketClients.Add(client);
                    //CallEvent("ACCEPT", clientIP);

                    Console.WriteLine("client: [" + clientEndPoint.Address.ToString() + "] connected");

                    RunEvent("MSG", "client: [" + clientEndPoint.Address.ToString() + "] connected");
                }

                // 소켓 연결 대기
                e.AcceptSocket = null;
                tcpServer.AcceptAsync(e);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        #endregion

        #region Client_EventProcess
        private void Client_EventProcess(string type, string message)
        {
            try
            {
                    RunEvent(type, message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        #endregion

        #region SendData
        public void SendData(string clientIP, string data)
        {
            try
            {
                // string 데이터에 BCC, STX, ETX 추가하여 전송
                if (Encoding.UTF8.GetBytes(data).First() != 0x02)
                {
                    SendData(clientIP, GetClientPacket(data));
                }
                else
                {
                    SendData(clientIP, Encoding.UTF8.GetBytes(data));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        public void SendData(string clientIP, byte[] data)
        {
            try
            {
                TcpClient targetClient = null;

                foreach (TcpClient socketClient in SocketClients)
                {
                    if (socketClient.Connected && socketClient.ClientIP.Equals(clientIP))
                    {
                        targetClient = socketClient;
                    }
                }

                if (targetClient != null)
                {
                    targetClient.SendData(data);
                    // 미루컨버터에 짧은 시간내 여러 패킷보내면 일부패킷응답이 안오는경우 있음.
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        #endregion

        #region GetClientPacket 마스터에 보낼 패킷형태로 변환
        private static byte[] GetClientPacket(string raw)
        {
            try
            {
                byte[] clientPacket = StringToByte(raw);
                byte[] checkSum = GetChecksum(StringToByte(raw));
                // stx
                clientPacket = AddByteToArray(clientPacket, 0x02, true);
                // bcc
                clientPacket = AddBytesToArray(clientPacket, checkSum, false);
                // etx
                clientPacket = AddByteToArray(clientPacket, 0x03, false);

                return clientPacket;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static byte[] GetChecksum(byte[] raw)
        {
            int Sum = 0;
            try
            {
                foreach (byte b in raw)
                {
                    Sum += b;
                }

                string sCheckSum = Sum.ToString("X04");
                byte[] bytes = StringToByte(sCheckSum);
                return bytes.Skip(2).Take(2).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(str);
            return StrByte;
        }

        private static byte[] AddByteToArray(byte[] bArray, byte newByte, bool insertFirst)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            if (insertFirst)
            {
                bArray.CopyTo(newArray, 1);
                newArray[0] = newByte;
            }
            else
            {
                bArray.CopyTo(newArray, 0);
                newArray[newArray.Length - 1] = newByte;
            }
            return newArray;
        }

        private static byte[] AddBytesToArray(byte[] bArray, byte[] newBytes, bool insertFirst)
        {
            byte[] newArray = new byte[bArray.Length + newBytes.Length];
            if (insertFirst)
            {
                bArray.CopyTo(newArray, newBytes.Length);
                for (int i = 0; i < newBytes.Length; i++)
                {
                    newArray[i] = newBytes[i];
                }
            }
            else
            {
                bArray.CopyTo(newArray, 0);
                for (int i = newBytes.Length; i > 0; i--)
                {
                    newArray[newArray.Length - i] = newBytes[newBytes.Length - i];
                }
            }
            return newArray;
        }
        #endregion

    }
}
