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
            ((System.ComponentModel.ISupportInitialize)(this.camDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // camDisplay
            // 
            this.camDisplay.Location = new System.Drawing.Point(14, 52);
            this.camDisplay.Name = "camDisplay";
            this.camDisplay.Size = new System.Drawing.Size(1112, 640);
            this.camDisplay.TabIndex = 0;
            this.camDisplay.TabStop = false;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(14, 698);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(1114, 55);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "문열기";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // UserID
            // 
            this.UserID.AutoSize = true;
            this.UserID.Location = new System.Drawing.Point(12, 17);
            this.UserID.Name = "UserID";
            this.UserID.Size = new System.Drawing.Size(50, 12);
            this.UserID.TabIndex = 2;
            this.UserID.Text = "UserID :";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(68, 12);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(137, 21);
            this.txtUserID.TabIndex = 3;
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(386, 12);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(137, 21);
            this.txtServerIP.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(320, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "ServerIP :";
            // 
            // btnIDsave
            // 
            this.btnIDsave.Location = new System.Drawing.Point(211, 13);
            this.btnIDsave.Margin = new System.Windows.Forms.Padding(0);
            this.btnIDsave.Name = "btnIDsave";
            this.btnIDsave.Size = new System.Drawing.Size(75, 21);
            this.btnIDsave.TabIndex = 6;
            this.btnIDsave.Text = "ID저장";
            this.btnIDsave.UseVisualStyleBackColor = true;
            this.btnIDsave.Click += new System.EventHandler(this.btnIDsave_Click);
            // 
            // btnIPsave
            // 
            this.btnIPsave.Location = new System.Drawing.Point(529, 13);
            this.btnIPsave.Margin = new System.Windows.Forms.Padding(0);
            this.btnIPsave.Name = "btnIPsave";
            this.btnIPsave.Size = new System.Drawing.Size(75, 21);
            this.btnIPsave.TabIndex = 7;
            this.btnIPsave.Text = "IP저장";
            this.btnIPsave.UseVisualStyleBackColor = true;
            this.btnIPsave.Click += new System.EventHandler(this.btnIPsave_Click);
            // 
            // labState
            // 
            this.labState.AutoSize = true;
            this.labState.Font = new System.Drawing.Font("굴림", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labState.ForeColor = System.Drawing.Color.Red;
            this.labState.Location = new System.Drawing.Point(949, 9);
            this.labState.Name = "labState";
            this.labState.Size = new System.Drawing.Size(179, 35);
            this.labState.TabIndex = 8;
            this.labState.Text = "연결 안 됨";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1138, 755);
            this.Controls.Add(this.labState);
            this.Controls.Add(this.btnIPsave);
            this.Controls.Add(this.btnIDsave);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.UserID);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.camDisplay);
            this.Name = "Form1";
            this.Text = "Form1";
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
    }
}

