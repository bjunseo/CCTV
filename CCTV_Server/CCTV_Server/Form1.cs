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
using System.Runtime.InteropServices.WindowsRuntime;

namespace CCTV_Server
{
    public partial class Form1 : Form
    {
        #region 변수들
        string connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};", DBConfig.Address, DBConfig.Database, DBConfig.Uid, DBConfig.Password1);

        TcpClient tcpClient;

        TcpServer tcpServer;


        Thread frameDecodeThread;

        string cctvIP;
        string masterIP;
        int port = 6000;

        DataTable dtUser;

        // After adding the DLL to the project, change the "Copy to Output Directory" value to "Copy If newer".
        string dllPath = Path.Combine(Environment.CurrentDirectory, "FFmpeg.AutoGen", "bin", "x64");
        
        string rtspAddr;
        string httpAddr;
        string url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";

        bool init = false;

        bool doorStat;
        #endregion

        #region socketStart 소켓을 연다
        public void Start()
        {
            try
            {
                tcpServer = new TcpServer(port);
                tcpServer.RunEvent += RunEvent;
                tcpServer.StartServer();

                Console.WriteLine("Socket Open Success!!");
                txtLog.Text += "==================== Socket Open Success!! =====================\r\n";
            }
            catch (Exception e)
            {
                Console.WriteLine("Socket Open Fail ㅠㅠ");
                txtLog.Text += "Socket Open Fail ㅠㅠ\r\n";
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
                txtLog.Text += "DB connet!!\r\n";
            }
            else
            {
                Console.WriteLine(" DB not connet!! ");
                txtLog.Text += "DB not connet!!\r\n";
            }

            setView();
            

            

            rtspAddr = $"rtsp://admin:dmenc001!@{cctvIP}:554/ISAPI/streaming/channels/101";
            httpAddr = $"http://admin:dmenc001!@{cctvIP}:80/ISAPI/Streaming/channels/102/httpPreview";


            frameDecodeThread = new Thread(new ThreadStart(Run_frameDecodeThread));
            frameDecodeThread.IsBackground = true;
            FFmpegBinariesHelper.RegisterFFmpegBinaries(dllPath);

            frameDecodeThread.Start();

            Start();
            //connectMaster();

            dgvUser.AutoGenerateColumns = false;

            selectUserInfo();

            init = true;
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

        public void connectMaster()
        {
            try
            {
                //Master = new MasterConn("Door", masterIP, 6000);
                //Master.MasterEvent += new MasterConn.RunEventHandler(MRunEvent);
                //Master.Start();
            }
            catch(Exception ex)
            {
                txtLog.Text += ex.ToString() + "\r\n";
            }

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
        private void RunEvent(string type, string data)
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
                                string name = ""; 

                                Console.WriteLine(" connect userID : " + datas[2]);
                                
                                foreach (DataGridViewRow row in dgvUser.Rows)
                                {
                                    int i = dgvUser.Rows.IndexOf(row);

                                    if (row.Cells["ID"].Value != null)
                                    {
                                        if (row.Cells["ID"].Value.ToString().Trim() == datas[2].Trim())
                                        {
                                            dgvUser.Rows[i].DefaultCellStyle.BackColor = Color.LightYellow;

                                            row.Cells["IP"].Value = datas[3];

                                            row.Cells["stat"].Value = "1";

                                            //if (this.dgvUser.InvokeRequired)
                                            //{
                                            //    this.Invoke(new MethodInvoker(delegate ()
                                            //    {
                                            //        dgvUser.Rows[i].Cells["connect"].Value = "연결중";
                                            //    }));
                                            //}
                                        }
                                    }
                                }
                                
                                if (this.txtLog.InvokeRequired)
                                {
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        txtLog.Text += "Connect userID : " + datas[2] + "\r\n";
                                    }));
                                }
                                else
                                {
                                    this.txtLog.Text += "Connect userID : " + datas[2] + "\r\n";
                                }
                                
                            }

                            if (datas[1].Trim() == "CLOSE")
                            {
                                foreach (Socket socket in tcpServer.connectedClients)
                                {
                                    if (!socket.Connected)
                                    {
                                        tcpServer.connectedClients.Remove(socket);
                                    }
                                }

                                foreach (DataGridViewRow row in dgvUser.Rows)
                                {
                                    int i = dgvUser.Rows.IndexOf(row);
                                    
                                    string name = dgvUser.Rows[i].Cells["user"].Value.ToString();

                                    if (row.Cells["ID"].Value != null)
                                    {
                                        if (row.Cells["ID"].Value.ToString().Trim() == datas[2].Trim())
                                        {
                                            TcpServer.socketInfo clientInfo = tcpServer.clientInfo.Find(x => x.ID.Trim() == datas[2].Trim());
                                            dgvUser.Rows[i].DefaultCellStyle.BackColor = Color.White;
                                        }
                                    }
                                }

                                if (this.txtLog.InvokeRequired)
                                {
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        txtLog.Text += "Close userID : " + datas[2] + "\r\n";
                                    }));
                                }
                                else
                                {
                                    this.txtLog.Text += datas[0] + "Close userID : " + datas[2] + "\r\n";
                                }
                            }

                            if (datas[1] == "DOOR")
                            {
                                //Master.SendData("200000,DOOR,OK,");

                                if (datas[2].Trim() != "OK")
                                {
                                    doorStat = false;

                                    tcpServer.SendData(masterIP, "200000,DOOR,OK,");

                                    string ip;

                                    for (int j = 0; j < 4; j++)
                                    {
                                        if (doorStat == true)
                                        {
                                            doorStat = false;
                                            return;
                                        }

                                        for (int i = 0; i < dgvUser.Rows.Count; i++)
                                        {
                                            if (dgvUser.Rows[i].Cells["level"].Value.ToString().Trim() == j.ToString() /*&&
                        dgvUser.Rows[i].Cells["stat"].Value.ToString().Trim() == j.ToString()*/)
                                            {
                                                ip = dgvUser.Rows[i].Cells["IP"].Value.ToString().Split(':')[0];
                                                tcpServer.SendData(ip, "200000,DOOR,");
                                            }
                                        }

                                        Thread.Sleep(10000);
                                    }
                                }
                                else
                                {
                                   
                                }
                            }

                            if (datas[1] == "OPEN")
                            {
                                if (doorStat != true)
                                {
                                    tcpServer.SendData(masterIP, "200000,DOOR,");
                                }
                                else
                                {
                                    break;
                                }

                                doorStat = true;

                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    Console.WriteLine("Open userID : " + datas[2]);
                                    txtLog.Text += "Open userID : " + datas[2] + "\r\n";
                                }));
                            }

                            if(datas[1] == "MRUN")
                            {
                                tcpServer.SendData(masterIP, "200000,MRUN,OK,");
                            }

                            if (datas[1] == "AWAY")
                            {
                                foreach (DataGridViewRow row in dgvUser.Rows)
                                {
                                    int i = dgvUser.Rows.IndexOf(row);

                                    if (row.Cells["ID"].Value != null)
                                    {
                                        if (row.Cells["ID"].Value.ToString().Trim() == datas[2].Trim())
                                        {
                                            dgvUser.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                                        }
                                    }
                                }
                            }
                            if (datas[1] == "NAWAY")
                            {
                                foreach (DataGridViewRow row in dgvUser.Rows)
                                {
                                    int i = dgvUser.Rows.IndexOf(row);

                                    if (row.Cells["ID"].Value != null)
                                    {
                                        if (row.Cells["ID"].Value.ToString().Trim() == datas[2].Trim())
                                        {
                                            dgvUser.Rows[i].DefaultCellStyle.BackColor = Color.LightYellow;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "MSG":
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                txtLog.Text += data + "\r\n";
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
        #endregion


        private void button1_Click(object sender, EventArgs e)
        {
            tcpServer.SendData(masterIP, "200000,DOOR,");
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
            string sql = "select [user], level, ID, '' as IP, '' as stat from UserMng";

            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(ds);

            }

            dtUser = ds.Tables[0];
            dgvUser.DataSource = dtUser;
        }

        private void dgvUser_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        #endregion

        #region 유저 관리
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string sql1 = "insert into UserMng ([user], [level]) values ('', '')";

            string sql2 = "select  MAX(ID) from UserMng";

            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql1, conn);
                cmd.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(sql2, conn);
                da.Fill(ds);
            }

            DataTable dt = ds.Tables[0];

            dtUser.Rows.Add("", 0, dt.Rows[0][0].ToString(), "", "");
        }

        private void timer_connect_Tick(object sender, EventArgs e)
        {
            foreach(Socket socket in tcpServer.connectedClients)
            {
                if(!socket.Connected)
                {
                    tcpServer.connectedClients.Remove(socket);
                }
                else
                {
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(dgvUser.SelectedRows.Count != 1)
            {
                MessageBox.Show("행을 제대로 선택해주세요");
                return;
            }

            string id = dgvUser.Rows[dgvUser.SelectedRows[0].Index].Cells["ID"].Value.ToString(); 

            string sql1 = $"Delete from UserMng where ID = {id}";

            using (SqlConnection conn = new SqlConnection(connectString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql1, conn);
                cmd.ExecuteNonQuery();
            }

            dtUser.Rows.RemoveAt(dgvUser.SelectedRows[0].Index);
        }

        private void dgvUser_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(init)
            {
                if ( dgvUser.Rows.Count < 1)
                {
                    return;
                }

                string id = dgvUser.Rows[dgvUser.CurrentRow.Index].Cells["ID"].Value.ToString();
                string user = dgvUser.Rows[dgvUser.CurrentRow.Index].Cells["User"].Value.ToString();
                string level = dgvUser.Rows[dgvUser.CurrentRow.Index].Cells["level"].Value.ToString();

                string sql1 = $"UPDATE UserMng SET [user] = '{user}', [level] = {level} where ID = '{id}'";

                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql1, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion


    }
}
