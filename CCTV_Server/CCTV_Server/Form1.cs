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
using System.Diagnostics.Tracing;
using Microsoft.Win32;

namespace CCTV_Server
{
    public partial class Form1 : Form
    {
        #region 변수들
       

        Socket server;

        TcpClient tcpClient;

        TcpServer tcpServer;

        Thread frameDecodeThread;

        string cctvIP;

        // After adding the DLL to the project, change the "Copy to Output Directory" value to "Copy If newer".
        string dllPath = Path.Combine(Environment.CurrentDirectory, "FFmpeg.AutoGen", "bin", "x64");
        
        string rtspAddr;
        string httpAddr;
        string url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";

        

        #endregion

        #region socketStart 소켓을 연다
        public void Start()
        {
            try
            {
                tcpServer = new TcpServer();

                tcpServer.RunEvent = RunEvent;
                

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
            setView();
            Start();

            rtspAddr = $"rtsp://admin:dmenc001!@{cctvIP}:554/ISAPI/streaming/channels/101";
            httpAddr = $"http://admin:dmenc001!@{cctvIP}:80/ISAPI/Streaming/channels/102/httpPreview";


            frameDecodeThread = new Thread(new ThreadStart(Run_frameDecodeThread));
            frameDecodeThread.IsBackground = true;
            FFmpegBinariesHelper.RegisterFFmpegBinaries(dllPath);

            frameDecodeThread.Start();
        }
        #endregion


        public void setView()
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVSERVER");

            cctvIP = reg.GetValue("CCTVIP", "값이 없습니다").ToString();
            txtCCTVIip.Text = cctvIP;
        }

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

        #region 폼클로즈
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frameDecodeThread.Abort();
        }
        #endregion

        private void btnSend_Click(object sender, EventArgs e)
        {
            //connectedClients[0].Send(Encoding.UTF8.GetBytes("123"));
        }

        private void RunEvent(string num, string data)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tcpServer.sendData();
    }

        private void btnCCTVsave_Click(object sender, EventArgs e)
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVSERVER");

            reg.SetValue("CCTVIP", txtCCTVIip.Text);
        }
    }
}
