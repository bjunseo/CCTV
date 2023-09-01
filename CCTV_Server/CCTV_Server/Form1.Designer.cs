namespace CCTV_Server
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
            this.camDisplay = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CCTV = new System.Windows.Forms.GroupBox();
            this.txtCCTVIip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUser = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCCTVsave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.camDisplay)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.CCTV.SuspendLayout();
            this.SuspendLayout();
            // 
            // camDisplay
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.camDisplay, 2);
            this.camDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.camDisplay.Location = new System.Drawing.Point(3, 154);
            this.camDisplay.Name = "camDisplay";
            this.camDisplay.Size = new System.Drawing.Size(1607, 722);
            this.camDisplay.TabIndex = 0;
            this.camDisplay.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 21);
            this.textBox1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.72727F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.27273F));
            this.tableLayoutPanel1.Controls.Add(this.camDisplay, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.19457F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.80543F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1613, 879);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.panel2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1607, 145);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.CCTV);
            this.panel2.Controls.Add(this.btnUser);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1639, 142);
            this.panel2.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(372, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 122);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // CCTV
            // 
            this.CCTV.BackColor = System.Drawing.SystemColors.Control;
            this.CCTV.Controls.Add(this.btnCCTVsave);
            this.CCTV.Controls.Add(this.txtCCTVIip);
            this.CCTV.Controls.Add(this.label1);
            this.CCTV.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.CCTV.Location = new System.Drawing.Point(127, 7);
            this.CCTV.Name = "CCTV";
            this.CCTV.Size = new System.Drawing.Size(239, 122);
            this.CCTV.TabIndex = 6;
            this.CCTV.TabStop = false;
            this.CCTV.Text = "CCTV";
            // 
            // txtCCTVIip
            // 
            this.txtCCTVIip.Location = new System.Drawing.Point(64, 34);
            this.txtCCTVIip.Name = "txtCCTVIip";
            this.txtCCTVIip.Size = new System.Drawing.Size(150, 21);
            this.txtCCTVIip.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP : ";
            // 
            // btnUser
            // 
            this.btnUser.Location = new System.Drawing.Point(6, 33);
            this.btnUser.Name = "btnUser";
            this.btnUser.Size = new System.Drawing.Size(104, 23);
            this.btnUser.TabIndex = 5;
            this.btnUser.Text = "유저관리";
            this.btnUser.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 21);
            this.button1.TabIndex = 4;
            this.button1.Text = "신호";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCCTVsave
            // 
            this.btnCCTVsave.Location = new System.Drawing.Point(27, 78);
            this.btnCCTVsave.Name = "btnCCTVsave";
            this.btnCCTVsave.Size = new System.Drawing.Size(187, 23);
            this.btnCCTVsave.TabIndex = 2;
            this.btnCCTVsave.Text = "등록";
            this.btnCCTVsave.UseVisualStyleBackColor = true;
            this.btnCCTVsave.Click += new System.EventHandler(this.btnCCTVsave_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1613, 879);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.camDisplay)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.CCTV.ResumeLayout(false);
            this.CCTV.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox camDisplay;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox CCTV;
        private System.Windows.Forms.Button btnUser;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCCTVIip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCCTVsave;
    }
}

