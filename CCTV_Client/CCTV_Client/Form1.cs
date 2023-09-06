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
using System.IO;
using System.Threading;
using CCTV_Server;
using OpenCvSharp;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;

namespace CCTV_Client
{
    public partial class Form1 : Form
    {
        #region 변수
        Socket server;

        private TcpClient Client;
        private Thread ReceiveThread;
        private NetworkStream NetworkStream;
        private StreamReader StreamReader;
        private StreamWriter StreamWriter;

        int m_port = 5000;
        private bool IsConnected = false;

        Thread frameDecodeThread;

        // After adding the DLL to the project, change the "Copy to Output Directory" value to "Copy If newer".
        string dllPath = Path.Combine(Environment.CurrentDirectory, "FFmpeg.AutoGen", "bin", "x64");

        string rtspAddr = "rtsp://admin:dmenc001!@192.168.0.250:554/ISAPI/streaming/channels/101";
        string httpAddr = "http://admin:dmenc001!@192.168.0.250:80/ISAPI/Streaming/channels/102/httpPreview";
        string url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";

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

                    NetworkStream = Client.GetStream();
                    StreamReader = new StreamReader(NetworkStream);
                    StreamWriter = new StreamWriter(NetworkStream, System.Text.Encoding.Default);

                    ReceiveThread = new Thread(new ThreadStart(DataReceived));
                    ReceiveThread.Start();

                    //StreamWriter.Write("1");
                    //StreamWriter.Flush();

                    if (Client.Connected)
                    {
                        MessageBox.Show("소켓 연결을 성공했습니다!!\r\n");
                    }
                    IsConnected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("소켓 연결을 실패했습니다.\r\n" + ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
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
        //void ConnectCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket client = (Socket)ar.AsyncState;
        //        client.EndConnect(ar);
        //        AsyncObject obj = new AsyncObject(4096);
        //        obj.WorkingSocket = server;
        //        server.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

        //        MessageBox.Show("붙음");

        //        Send(Encoding.UTF8.GetBytes("123"));
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //}
        void DataReceived()
        {
            while(true)
            {
                while (true)
                {
                    try
                    {
                        string packet = "";
                        char[] rcvPacket = new char[1024];
                        int rcvLen = StreamReader.Read(rcvPacket, 0, rcvPacket.Length);

                        if (rcvLen > 0)
                        {
                            for (int i = 0; i < rcvLen; i++)
                            {
                                packet += (char)rcvPacket[i];
                            }

                            MessageBox.Show(packet);

                            break;
                        }
                        // 데이터 획득


                    }
                    catch (Exception ex)
                    {


                    }
                }
            }
        }
        public void Send(byte[] msg)
        {
            server.Send(msg);
        }

        public Form1()
        {
            InitializeComponent();

            frameDecodeThread = new Thread(new ThreadStart(Run_frameDecodeThread));
            frameDecodeThread.IsBackground = true;
            FFmpegBinariesHelper.RegisterFFmpegBinaries(dllPath);

            frameDecodeThread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Connect();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //server.Send(Encoding.UTF8.GetBytes("Close"));
            Close();

            frameDecodeThread.Abort();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            StreamWriter.Write("CLIENT,OPEN,");
            StreamWriter.Flush();

        }


        #region 카메라 코드
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
    }
}
