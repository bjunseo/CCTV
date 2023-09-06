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
using System.Data.SqlClient;

namespace CCTV_Server
{
    public partial class Form1 : Form
    {
        #region 변수들
        string connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};", DBConfig.Address, DBConfig.Database, DBConfig.Uid, DBConfig.Password1);

        Socket server;

        TcpClient tcpClient;

        TcpServer tcpServer;

        Thread frameDecodeThread;

        string cctvIP;
        string masterIP;

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
            if(ConnectionTest())
            {
                Console.WriteLine("DB connet!!");
            }
            else
            {
                Console.WriteLine("☆★☆★☆★ DB not connet!! ☆★☆★☆★");
            }

            setView();
            Start();

            rtspAddr = $"rtsp://admin:dmenc001!@{cctvIP}:554/ISAPI/streaming/channels/101";
            httpAddr = $"http://admin:dmenc001!@{cctvIP}:80/ISAPI/Streaming/channels/102/httpPreview";


            frameDecodeThread = new Thread(new ThreadStart(Run_frameDecodeThread));
            frameDecodeThread.IsBackground = true;
            FFmpegBinariesHelper.RegisterFFmpegBinaries(dllPath);

            frameDecodeThread.Start();


            dgvUser.AutoGenerateColumns = false;

            selectUserInfo();
        }
        #endregion

        #region view 초기화
        public void setView()
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVSERVER");

            cctvIP = reg.GetValue("CCTVIP", "값이 없습니다").ToString();
            txtCCTVIip.Text = cctvIP;

            masterIP = reg.GetValue("MASTERIP", "값이 없습니다").ToString();
            txtMasterIp.Text = masterIP;
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

        #region 폼클로즈
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frameDecodeThread.Abort();
        }
        #endregion

        #region socket run event
        private void RunEvent(string ID, string num, string data)
        {
            if (ID == "CLIENT")
            {
                
            }
            else if (ID == "DOOR")
            {

            }
            else if (ID == "MRUN")
            {
                
            }
        }
        #endregion


        private void button1_Click(object sender, EventArgs e)
        {
            tcpServer.sendData();
        }

        #region CCTV,Master IP 저장 버튼 이벤트
        private void btnCCTVsave_Click(object sender, EventArgs e)
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVSERVER");

            reg.SetValue("CCTVIP", txtCCTVIip.Text);
        }

        private void btnMasterSave_Click(object sender, EventArgs e)
        {
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("CCTVSERVER");

            reg.SetValue("MASTERIP", txtMasterIp.Text);
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
        #endregion

        #region 형변환
        /// <summary>
        /// String --> Byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(str);
            return StrByte;
        }
        #endregion

        #region byte 데이터 값 추가하기
        /// <summary>
        /// byte 하나 추가
        /// </summary>
        /// <param name="bArray"></param>
        /// <param name="newByte"></param>
        /// <param name="insertFirst"></param>
        /// <returns></returns>
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

        /// <summary>
        /// byte 다수 추가
        /// </summary>
        /// <param name="bArray"></param>
        /// <param name="newBytes"></param>
        /// <param name="insertFirst"></param>
        /// <returns></returns>
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

        #region checkSum 가져오기
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
        #endregion

        #region DB 열기
        public bool ConnectionTest()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    conn.Open();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void selectUserInfo()
        {
            string sql = "select * from UserMng";

            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(ds);

            }
            
            DataTable dt = ds.Tables[0];
            dgvUser.DataSource = dt;

        }
        #endregion
    }
}
