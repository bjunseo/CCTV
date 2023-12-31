﻿using System;
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
using System.Windows.Markup;

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

        bool awayStat = false;
        #endregion

        public void ReceiveEvent(string type, string data)
        {
            try
            {
                switch (type)
                {
                    case "RCV":
                        {
                            string[] datas = data.Split(',');

                            if (datas[1] == "CONNECT")
                            {
                                

                            }

                            if (datas[1].Trim() == "CLOSE")
                            {
                                
                            }

                            if (datas[1] == "DOOR")
                            {
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    this.TopMost = true;
                                    this.Focus();
                                    this.TopMost = false;
                                }));
                            }

                            break;
                        }
                    case "MSG":
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {

                            }));

                            break;
                        }
                    case "DISCONN":
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                labState.Text = "-Connect-";
                                labState.ForeColor = Color.Black;
                            }));

                            break;
                        }
                    default: { break; }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Connect()
        {
            try
            {
                socket = new SocketManager("", serverIP, 6000);
                socket.ReceiveEvent += ReceiveEvent;

                socket.SendData_($"CLIENT,CONNECT,{userID}");

                if (socket.IsConnected)
                {
                        labState.Text = "-Connect-";
                        labState.ForeColor = Color.GreenYellow;

                        Console.WriteLine("소켓 연결을 성공했습니다!!\r\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("소켓 연결을 실패했습니다.\r\n" + ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Application.Exit();
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

        public Form1()
        {
            InitializeComponent();

            try
            {
                frameDecodeThread = new Thread(new ThreadStart(Run_frameDecodeThread));
                frameDecodeThread.IsBackground = true;
                FFmpegBinariesHelper.RegisterFFmpegBinaries(dllPath);

                frameDecodeThread.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                Application.ExitThread();
                Environment.Exit(0);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            socket.SendData_($"CLIENT,OPEN,{userID}");

        }


        #region 카메라 코드
        private unsafe void Run_frameDecodeThread()
        {
            try
            {
                using (var decoder = new VideoStreamDecoder(rtspAddr))
                {
                    var srcSize = decoder.FrameSize;
                    var pixelFormat = decoder.PixelFormat;
                    var destSize = new System.Drawing.Size(800, 450);
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
            catch(Exception exception)
            {
                //Console.WriteLine(exception.Message);

                //if(txtUserID.Text != "" || txtServerIP.Text != "")
                //{
                //    Application.ExitThread();
                //    Environment.Exit(0);
                //}
                
            }
        }

        private unsafe Mat AVframeToMat(FFmpeg.AutoGen.Abstractions.AVFrame* frame)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                Application.ExitThread();
                Environment.Exit(0);
                return new Mat();
            }
            
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

            serverIP = reg.GetValue("serverIP", "").ToString();
            txtServerIP.Text = serverIP;

            userID = reg.GetValue("userID", "").ToString();
            txtUserID.Text = userID;

            if(userID.Trim() == "")
            {
                txtUserID.ReadOnly = false;

            }
        }

        #endregion

        private void btnAway_Click(object sender, EventArgs e)
        {
            if(awayStat == true)
            {
                socket.SendData_($"CLIENT,NAWAY,{userID}");
                awayStat = false;
                btnAway.BackColor = Color.LightGray;
            }
            else if (awayStat == false)
            {
                socket.SendData_($"CLIENT,AWAY,{userID}");
                awayStat = true;
                btnAway.BackColor = Color.DarkGray;
            }
            
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(!socket.IsConnected)
            {
                Connect();
            }
        }
    }
}
