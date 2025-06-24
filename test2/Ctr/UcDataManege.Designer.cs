namespace test2.Ctr
{
    partial class UcDataManege
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        private void InitializeComponent()
        {
            this.tbDataManege = new System.Windows.Forms.TextBox();
            this.cbDataGet = new System.Windows.Forms.CheckBox();
            this.cbModelUpdate = new System.Windows.Forms.CheckBox();
            this.btBreakConnect = new System.Windows.Forms.Button();
            this.cbDBUpdate = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgressPercentage = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.cbProjectList = new System.Windows.Forms.ComboBox();
            this.lblProjectTip = new System.Windows.Forms.Label();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.btLogin = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.tlpCheckBoxes = new System.Windows.Forms.TableLayoutPanel();
            this.tlpProgress = new System.Windows.Forms.TableLayoutPanel();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.tlpCheckBoxes.SuspendLayout();
            this.tlpProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbDataManege
            // 
            this.tbDataManege.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDataManege.Location = new System.Drawing.Point(0, 210);
            this.tbDataManege.Margin = new System.Windows.Forms.Padding(4);
            this.tbDataManege.Multiline = true;
            this.tbDataManege.Name = "tbDataManege";
            this.tbDataManege.ReadOnly = true;
            this.tbDataManege.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDataManege.Size = new System.Drawing.Size(420, 570);
            this.tbDataManege.TabIndex = 0;
            // 
            // cbDataGet
            // 
            this.cbDataGet.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbDataGet.AutoSize = true;
            this.cbDataGet.Font = new System.Drawing.Font("宋体", 10F);
            this.cbDataGet.Location = new System.Drawing.Point(64, 26);
            this.cbDataGet.Margin = new System.Windows.Forms.Padding(4);
            this.cbDataGet.Name = "cbDataGet";
            this.cbDataGet.Size = new System.Drawing.Size(82, 18);
            this.cbDataGet.TabIndex = 0;
            this.cbDataGet.Text = "获取数据";
            this.cbDataGet.UseVisualStyleBackColor = true;
            this.cbDataGet.CheckedChanged += new System.EventHandler(this.cbDataGet_CheckedChanged);
            // 
            // cbModelUpdate
            // 
            this.cbModelUpdate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbModelUpdate.AutoSize = true;
            this.tlpCheckBoxes.SetColumnSpan(this.cbModelUpdate, 2);
            this.cbModelUpdate.Font = new System.Drawing.Font("宋体", 10F);
            this.cbModelUpdate.Location = new System.Drawing.Point(148, 98);
            this.cbModelUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.cbModelUpdate.Name = "cbModelUpdate";
            this.cbModelUpdate.Size = new System.Drawing.Size(124, 18);
            this.cbModelUpdate.TabIndex = 2;
            this.cbModelUpdate.Text = "更新进度数据库";
            this.cbModelUpdate.UseVisualStyleBackColor = true;
            this.cbModelUpdate.CheckedChanged += new System.EventHandler(this.cbModelUpdate_CheckedChanged);
            // 
            // btBreakConnect
            // 
            this.btBreakConnect.BackColor = System.Drawing.Color.RosyBrown;
            this.btBreakConnect.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btBreakConnect.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.btBreakConnect.Location = new System.Drawing.Point(0, 195);
            this.btBreakConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btBreakConnect.Name = "btBreakConnect";
            this.btBreakConnect.Size = new System.Drawing.Size(420, 75);
            this.btBreakConnect.TabIndex = 22;
            this.btBreakConnect.Text = "断开数据库连接";
            this.btBreakConnect.UseVisualStyleBackColor = false;
            this.btBreakConnect.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btBreakConnect_MouseUp);
            // 
            // cbDBUpdate
            // 
            this.cbDBUpdate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbDBUpdate.AutoSize = true;
            this.cbDBUpdate.Font = new System.Drawing.Font("宋体", 10F);
            this.cbDBUpdate.Location = new System.Drawing.Point(267, 26);
            this.cbDBUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.cbDBUpdate.Name = "cbDBUpdate";
            this.cbDBUpdate.Size = new System.Drawing.Size(96, 18);
            this.cbDBUpdate.TabIndex = 1;
            this.cbDBUpdate.Text = "更新模型库";
            this.cbDBUpdate.UseVisualStyleBackColor = true;
            this.cbDBUpdate.CheckedChanged += new System.EventHandler(this.cbDBUpdate_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(12, 12);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(306, 28);
            this.progressBar.TabIndex = 26;
            // 
            // lblProgressPercentage
            // 
            this.lblProgressPercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProgressPercentage.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold);
            this.lblProgressPercentage.Location = new System.Drawing.Point(326, 8);
            this.lblProgressPercentage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProgressPercentage.Name = "lblProgressPercentage";
            this.lblProgressPercentage.Size = new System.Drawing.Size(82, 36);
            this.lblProgressPercentage.TabIndex = 27;
            this.lblProgressPercentage.Text = "0%";
            this.lblProgressPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.cbProjectList);
            this.panelTop.Controls.Add(this.lblProjectTip);
            this.panelTop.Controls.Add(this.lblUserInfo);
            this.panelTop.Controls.Add(this.btLogin);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(4);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(420, 210);
            this.panelTop.TabIndex = 1;
            // 
            // lblProjectTip
            // 
            this.lblProjectTip.AutoSize = true;
            this.lblProjectTip.Font = new System.Drawing.Font("宋体", 10F);
            this.lblProjectTip.Location = new System.Drawing.Point(13, 115); // 修改位置：稍微往下一点
            this.lblProjectTip.Name = "lblProjectTip";
            this.lblProjectTip.Size = new System.Drawing.Size(119, 14);
            this.lblProjectTip.TabIndex = 23;
            this.lblProjectTip.Text = "请选择归属项目：";

            // 
            // cbProjectList
            // 
            this.cbProjectList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbProjectList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProjectList.Enabled = false;
            this.cbProjectList.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.cbProjectList.FormattingEnabled = true;
            this.cbProjectList.Location = new System.Drawing.Point(13, 140); // 修改位置：移动到文字下方
            this.cbProjectList.Name = "cbProjectList";
            this.cbProjectList.Size = new System.Drawing.Size(390, 29); // 修改宽度：让它占满一行，不再挤在右边
            this.cbProjectList.TabIndex = 24;
            this.cbProjectList.SelectedIndexChanged += new System.EventHandler(this.cbProjectList_SelectedIndexChanged);
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblUserInfo.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblUserInfo.Location = new System.Drawing.Point(13, 80);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(93, 19);
            this.lblUserInfo.TabIndex = 22;
            this.lblUserInfo.Text = "状态：未登录";
            // 
            // btLogin
            // 
            this.btLogin.BackColor = System.Drawing.Color.SteelBlue;
            this.btLogin.Dock = System.Windows.Forms.DockStyle.Top;
            this.btLogin.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.btLogin.ForeColor = System.Drawing.Color.White;
            this.btLogin.Location = new System.Drawing.Point(0, 0);
            this.btLogin.Margin = new System.Windows.Forms.Padding(4);
            this.btLogin.Name = "btLogin";
            this.btLogin.Size = new System.Drawing.Size(420, 60);
            this.btLogin.TabIndex = 21;
            this.btLogin.Text = "点击登录系统";
            this.btLogin.UseVisualStyleBackColor = false;
            this.btLogin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btLogin_MouseUp);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.tlpCheckBoxes);
            this.panelBottom.Controls.Add(this.tlpProgress);
            this.panelBottom.Controls.Add(this.btBreakConnect);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 780);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(4);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(420, 270);
            this.panelBottom.TabIndex = 2;
            // 
            // tlpCheckBoxes
            // 
            this.tlpCheckBoxes.ColumnCount = 2;
            this.tlpCheckBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCheckBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCheckBoxes.Controls.Add(this.cbDataGet, 0, 0);
            this.tlpCheckBoxes.Controls.Add(this.cbDBUpdate, 1, 0);
            this.tlpCheckBoxes.Controls.Add(this.cbModelUpdate, 0, 1);
            this.tlpCheckBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCheckBoxes.Location = new System.Drawing.Point(0, 52);
            this.tlpCheckBoxes.Margin = new System.Windows.Forms.Padding(4);
            this.tlpCheckBoxes.Name = "tlpCheckBoxes";
            this.tlpCheckBoxes.RowCount = 2;
            this.tlpCheckBoxes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCheckBoxes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCheckBoxes.Size = new System.Drawing.Size(420, 143);
            this.tlpCheckBoxes.TabIndex = 31;
            // 
            // tlpProgress
            // 
            this.tlpProgress.ColumnCount = 2;
            this.tlpProgress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProgress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpProgress.Controls.Add(this.progressBar, 0, 0);
            this.tlpProgress.Controls.Add(this.lblProgressPercentage, 1, 0);
            this.tlpProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpProgress.Location = new System.Drawing.Point(0, 0);
            this.tlpProgress.Margin = new System.Windows.Forms.Padding(4);
            this.tlpProgress.Name = "tlpProgress";
            this.tlpProgress.Padding = new System.Windows.Forms.Padding(8);
            this.tlpProgress.RowCount = 1;
            this.tlpProgress.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProgress.Size = new System.Drawing.Size(420, 52);
            this.tlpProgress.TabIndex = 30;
            // 
            // UcDataManege
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbDataManege);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UcDataManege";
            this.Size = new System.Drawing.Size(420, 1050);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.tlpCheckBoxes.ResumeLayout(false);
            this.tlpCheckBoxes.PerformLayout();
            this.tlpProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDataManege;
        private System.Windows.Forms.CheckBox cbDataGet;
        private System.Windows.Forms.CheckBox cbModelUpdate;
        private System.Windows.Forms.Button btBreakConnect;
        private System.Windows.Forms.CheckBox cbDBUpdate;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgressPercentage;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.TableLayoutPanel tlpCheckBoxes;
        private System.Windows.Forms.TableLayoutPanel tlpProgress;

        // 新增的控件定义
        private System.Windows.Forms.Button btLogin;
        private System.Windows.Forms.ComboBox cbProjectList;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Label lblProjectTip;
    }
}