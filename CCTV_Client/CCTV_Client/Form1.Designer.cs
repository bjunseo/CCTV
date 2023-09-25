namespace CCTV_Client
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.camDisplay = new System.Windows.Forms.PictureBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.UserID = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnIDsave = new System.Windows.Forms.Button();
            this.btnIPsave = new System.Windows.Forms.Button();
            this.labState = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnAway = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.camDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // camDisplay
            // 
            this.camDisplay.Location = new System.Drawing.Point(14, 43);
            this.camDisplay.Name = "camDisplay";
            this.camDisplay.Size = new System.Drawing.Size(800, 459);
            this.camDisplay.TabIndex = 0;
            this.camDisplay.TabStop = false;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(12, 508);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(400, 55);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "문열기";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // UserID
            // 
            this.UserID.AutoSize = true;
            this.UserID.Location = new System.Drawing.Point(12, 19);
            this.UserID.Name = "UserID";
            this.UserID.Size = new System.Drawing.Size(24, 12);
            this.UserID.TabIndex = 2;
            this.UserID.Text = "ID :";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(40, 15);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(137, 21);
            this.txtUserID.TabIndex = 3;
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(341, 14);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(137, 21);
            this.txtServerIP.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(275, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "ServerIP :";
            // 
            // btnIDsave
            // 
            this.btnIDsave.Location = new System.Drawing.Point(180, 12);
            this.btnIDsave.Margin = new System.Windows.Forms.Padding(0);
            this.btnIDsave.Name = "btnIDsave";
            this.btnIDsave.Size = new System.Drawing.Size(65, 25);
            this.btnIDsave.TabIndex = 6;
            this.btnIDsave.Text = "ID저장";
            this.btnIDsave.UseVisualStyleBackColor = true;
            this.btnIDsave.Click += new System.EventHandler(this.btnIDsave_Click);
            // 
            // btnIPsave
            // 
            this.btnIPsave.Location = new System.Drawing.Point(481, 12);
            this.btnIPsave.Margin = new System.Windows.Forms.Padding(0);
            this.btnIPsave.Name = "btnIPsave";
            this.btnIPsave.Size = new System.Drawing.Size(65, 25);
            this.btnIPsave.TabIndex = 7;
            this.btnIPsave.Text = "IP저장";
            this.btnIPsave.UseVisualStyleBackColor = true;
            this.btnIPsave.Click += new System.EventHandler(this.btnIPsave_Click);
            // 
            // labState
            // 
            this.labState.AutoSize = true;
            this.labState.BackColor = System.Drawing.SystemColors.Desktop;
            this.labState.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labState.ForeColor = System.Drawing.Color.Black;
            this.labState.Location = new System.Drawing.Point(687, 12);
            this.labState.Name = "labState";
            this.labState.Size = new System.Drawing.Size(114, 24);
            this.labState.TabIndex = 8;
            this.labState.Text = "연       결";
            // 
            // btnAway
            // 
            this.btnAway.Location = new System.Drawing.Point(415, 508);
            this.btnAway.Margin = new System.Windows.Forms.Padding(0);
            this.btnAway.Name = "btnAway";
            this.btnAway.Size = new System.Drawing.Size(399, 55);
            this.btnAway.TabIndex = 9;
            this.btnAway.Text = "자리비움";
            this.btnAway.UseVisualStyleBackColor = true;
            this.btnAway.Click += new System.EventHandler(this.btnAway_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(616, 10);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(65, 25);
            this.btnConnect.TabIndex = 10;
            this.btnConnect.Text = "연결시도";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(834, 571);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnAway);
            this.Controls.Add(this.labState);
            this.Controls.Add(this.btnIPsave);
            this.Controls.Add(this.btnIDsave);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.UserID);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.camDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "CCTVClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.camDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox camDisplay;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label UserID;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnIDsave;
        private System.Windows.Forms.Button btnIPsave;
        private System.Windows.Forms.Label labState;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnAway;
        private System.Windows.Forms.Button btnConnect;
    }
}

