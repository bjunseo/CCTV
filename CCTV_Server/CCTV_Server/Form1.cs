using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

using OpenCvSharp;
using System.IO;
using System.Threading;

namespace CCTV_Server
{
    public partial class Form1 : Form
    {
        #region 변수들
        Socket server;

        TcpClient tcpClient;

        List<Socket> connectedClients = new List<Socket>();

        int m_port = 5000;

        Thread frameDecodeThread;

        // After adding the DLL to the project, change the "Copy to Output Directory" value to "Copy If newer".
        string dllPath = Path.Combine(Environment.CurrentDirectory, "FFmpeg.AutoGen", "bin", "x64");

        string rtspAddr = "rtsp://admin:dmenc001!@192.168.0.250:554/ISAPI/streaming/channels/101";
        string httpAddr = "http://admin:dmenc001!@192.168.0.250:80/ISAPI/Streaming/channels/102/httpPreview";
        string url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";

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

            frameDecodeThread = new Thread(new ThreadStart(Run_frameDecodeThread));
            frameDecodeThread.IsBackground = true;
            FFmpegBinariesHelper.RegisterFFmpegBinaries(dllPath);

            frameDecodeThread.Start();
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


        #region 카메라쪽 코드
        private unsafe void Run_frameDecodeThread()
        {
            using (var decoder = new VideoStreamDecoder(rtspAddr))
            {
                var srcSize = decoder.FrameSize;
                var pixelFormat = decoder.PixelFormat;
                var destSize = srcSize;
                var destPixelFormat = FFmpeg.AutoGen.Abstractions.AVPixelFormat.AV_PIX_FMT_BGR24;

                using (var frameConverter = new VideoFrameConverter(srcSize, pixelFormat, destSize, destPixelFormat))
                {
                    while (decoder.TryDecodeNextFrame(out var frame))
                    {
                        var convertedframe = frameConverter.Convert(frame);
                        var bitmap = new Bitmap(convertedframe.width, convertedframe.height,
                            convertedframe.linesize[0], PixelFormat.Format24bppRgb, (IntPtr)convertedframe.data[0]);

                        if (camDisplay.InvokeRequired)
                        {
                            camDisplay.Invoke(new MethodInvoker(delegate {
                                camDisplay.Image = bitmap;
                            }));
                        }
                        else camDisplay.Image = bitmap;
                    }
                }
            }
        }

        private unsafe Mat AVframeToMat(FFmpeg.AutoGen.Abstractions.AVFrame* frame)
        {
            int width = frame->width, height = frame->height;
            int[] lSize = new int[1];
            byte*[] pByteArr = new byte*[1];

            Mat mat, tmpMat = new Mat(height, width, MatType.CV_8UC3);
            FFmpeg.AutoGen.Abstractions.SwsContext* conversion;
            pByteArr[0] = tmpMat.DataPointer;
            lSize[0] = (int)tmpMat.Step1();

            conversion = FFmpeg.AutoGen.Abstractions.ffmpeg.sws_getContext(
                width, height, (FFmpeg.AutoGen.Abstractions.AVPixelFormat)frame->format, width, height,
                FFmpeg.AutoGen.Abstractions.AVPixelFormat.AV_PIX_FMT_BGR24, FFmpeg.AutoGen.Abstractions.ffmpeg.SWS_BICUBIC,
                null, null, null);
            FFmpeg.AutoGen.Abstractions.ffmpeg.sws_scale(conversion, frame->data, frame->linesize, 0, height, pByteArr, lSize);
            mat = new Mat(height, width, MatType.CV_8UC3, (IntPtr)pByteArr[0]);

            FFmpeg.AutoGen.Abstractions.ffmpeg.sws_freeContext(conversion);
            return mat;
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frameDecodeThread.Abort();
        }
    }
}
