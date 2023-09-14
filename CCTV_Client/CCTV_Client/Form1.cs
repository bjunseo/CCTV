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
using Microsoft.Win32;

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

        private SocketManager socket; 


        string serverIP;
        string userID;
        #endregion

        public void Connect()
        {
            try
            {
                socket = new SocketManager("", serverIP, 5000);

                if (IsConnected == false)
                {
                        socket.SendData_("CLIENT,CONNECT," + userID);

                        Console.WriteLine("소켓 연결을 성공했습니다!!\r\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("소켓 연결을 실패했습니다.\r\n" + ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        public void Close()
        {
            try
            {
                socket.SendData_("CLIENT,CLOSE," + userID);


            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
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

        //        Console.WriteLine("붙음");

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

                            Console.WriteLine(packet);

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
            setView();

            Connect();

            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Close();
                frameDecodeThread.Abort();

                Application.ExitThread();
                Environment.Exit(0);

            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            socket.SendData_("CLIENT,OPEN,1");

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

        #region 저장버튼
        private void btnIDsave_Click(object sender, EventArgs e)
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVCLIENT");

            reg.SetValue("userID", txtUserID.Text);
        }

        private void btnIPsave_Click(object sender, EventArgs e)
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVCLIENT");

            reg.SetValue("serverIP", txtServerIP.Text);
        }
        #endregion

        #region view 초기화
        public void setView()
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVCLIENT");

            serverIP = reg.GetValue("serverIP", "값이 없습니다").ToString();
            txtServerIP.Text = serverIP;

            userID = reg.GetValue("userID", "값이 없습니다").ToString();
            txtUserID.Text = userID;

            if(userID == "값이 없습니다")
            {
                txtUserID.ReadOnly = false;

            }
        }
        #endregion
    }
}
