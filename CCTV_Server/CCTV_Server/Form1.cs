using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;


namespace CCTV_Server
{
    public partial class Form1 : Form
    {
        #region 변수들
        Socket server;

        TcpClient tcpClient;

        List<Socket> connectedClients = new List<Socket>();

        int m_port = 5000;
        #endregion

        #region socketStart 소켓을 연다
        public void Start()
        {
            try
            {
                //소켓을 생성한다
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //목적지를 정해준다
                IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), m_port);
                //목적지와 묶어준다
                server.Bind(serverEP);
                //받아들이기 시작한다 (최대 10개의 Client까지)
                server.Listen(10);
                //연결시도가 감지되면 AcceptCallBack으로 이동하게 설정
                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);

                server.AcceptAsync(socketArgs);

                MessageBox.Show("Socket Open Success!!");
            }
            catch (Exception e)
            {
                MessageBox.Show("Socket Open Fail ㅠㅠ");
            }
        }
        #endregion

        #region 생성자
        public Form1()
        {
            InitializeComponent();

        }
        #endregion

        #region 폼로드
        private void Form1_Load(object sender, EventArgs e)
        {
            Start();
        }
        #endregion

        #region 클라이언트 붙을때 실행되는 콜백함수
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                System.Net.Sockets.Socket clientSocket = e.AcceptSocket;
                IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;

                Console.WriteLine("client: [" + clientEndPoint.Address.ToString() + "] connected");

                connectedClients.Add(clientSocket);

                //EventSocketClient client = new EventSocketClient(clientSocket);

            }
            catch (Exception exception)
            {
                ;
            }
        }
        #endregion

        #region 데이터 받는 함수
        void DataReceived(IAsyncResult ar)
        {
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            int received = obj.WorkingSocket.EndReceive(ar);

            byte[] buffer = new byte[received];

            Array.Copy(obj.Buffer, 0, buffer, 0, received);

            MessageBox.Show("받음");
        }
        #endregion

        #region client 클래스
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
        #endregion

    }
}
