namespace test2.Ctr
{
    partial class UcDataManege
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tbDataManege = new System.Windows.Forms.TextBox();
            this.cbDataGet = new System.Windows.Forms.CheckBox();
            this.cbModelUpdate = new System.Windows.Forms.CheckBox();
            this.btConnect = new System.Windows.Forms.Button();
            this.btBreakConnect = new System.Windows.Forms.Button();
            this.cbDBUpdate = new System.Windows.Forms.CheckBox();
            this.lblTableName = new System.Windows.Forms.Label();
            this.tbTableName = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgressPercentage = new System.Windows.Forms.Label();
            this.lblProgressTableName = new System.Windows.Forms.Label();
            this.tbProgressTableName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbDataManege
            // 
            this.tbDataManege.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDataManege.Location = new System.Drawing.Point(4, 212);
            this.tbDataManege.Margin = new System.Windows.Forms.Padding(4);
            this.tbDataManege.Multiline = true;
            this.tbDataManege.Name = "tbDataManege";
            this.tbDataManege.ReadOnly = true;
            this.tbDataManege.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDataManege.Size = new System.Drawing.Size(546, 366);
            this.tbDataManege.TabIndex = 0;
            // 
            // cbDataGet
            // 
            this.cbDataGet.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cbDataGet.AutoSize = true;
            this.cbDataGet.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbDataGet.Location = new System.Drawing.Point(45, 668);
            this.cbDataGet.Name = "cbDataGet";
            this.cbDataGet.Size = new System.Drawing.Size(159, 34);
            this.cbDataGet.TabIndex = 19;
            this.cbDataGet.Text = "获取数据";
            this.cbDataGet.UseVisualStyleBackColor = true;
            this.cbDataGet.CheckedChanged += new System.EventHandler(this.cbDataGet_CheckedChanged);
            // 
            // cbModelUpdate
            // 
            this.cbModelUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cbModelUpdate.AutoSize = true;
            this.cbModelUpdate.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbModelUpdate.Location = new System.Drawing.Point(273, 731);
            this.cbModelUpdate.Name = "cbModelUpdate";
            this.cbModelUpdate.Size = new System.Drawing.Size(249, 34);
            this.cbModelUpdate.TabIndex = 20;
            this.cbModelUpdate.Text = "更新进度数据库";
            this.cbModelUpdate.UseVisualStyleBackColor = true;
            this.cbModelUpdate.CheckedChanged += new System.EventHandler(this.cbModelUpdate_CheckedChanged);
            // 
            // btConnect
            // 
            this.btConnect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btConnect.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btConnect.Location = new System.Drawing.Point(0, 0);
            this.btConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(556, 108);
            this.btConnect.TabIndex = 21;
            this.btConnect.Text = "连接数据库";
            this.btConnect.UseVisualStyleBackColor = false;
            this.btConnect.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btConnect_MouseUp);
            // 
            // btBreakConnect
            // 
            this.btBreakConnect.BackColor = System.Drawing.Color.RosyBrown;
            this.btBreakConnect.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btBreakConnect.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btBreakConnect.Location = new System.Drawing.Point(0, 785);
            this.btBreakConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btBreakConnect.Name = "btBreakConnect";
            this.btBreakConnect.Size = new System.Drawing.Size(556, 117);
            this.btBreakConnect.TabIndex = 22;
            this.btBreakConnect.Text = "断开数据库连接";
            this.btBreakConnect.UseVisualStyleBackColor = false;
            this.btBreakConnect.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btBreakConnect_MouseUp);
            // 
            // cbDBUpdate
            // 
            this.cbDBUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cbDBUpdate.AutoSize = true;
            this.cbDBUpdate.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbDBUpdate.Location = new System.Drawing.Point(273, 668);
            this.cbDBUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.cbDBUpdate.Name = "cbDBUpdate";
            this.cbDBUpdate.Size = new System.Drawing.Size(249, 34);
            this.cbDBUpdate.TabIndex = 23;
            this.cbDBUpdate.Text = "更新模型数据库";
            this.cbDBUpdate.UseVisualStyleBackColor = true;
            this.cbDBUpdate.CheckedChanged += new System.EventHandler(this.cbDBUpdate_CheckedChanged);
            // 
            // lblTableName
            // 
            this.lblTableName.AutoSize = true;
            this.lblTableName.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblTableName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTableName.Location = new System.Drawing.Point(4, 128);
            this.lblTableName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTableName.Name = "lblTableName";
            this.lblTableName.Size = new System.Drawing.Size(178, 24);
            this.lblTableName.TabIndex = 24;
            this.lblTableName.Text = "模型表名输入：";
            // 
            // tbTableName
            // 
            this.tbTableName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTableName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbTableName.Location = new System.Drawing.Point(194, 123);
            this.tbTableName.Margin = new System.Windows.Forms.Padding(4);
            this.tbTableName.Name = "tbTableName";
            this.tbTableName.Size = new System.Drawing.Size(356, 35);
            this.tbTableName.TabIndex = 25;
            this.tbTableName.Text = "请输入模型表表名";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(9, 588);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(432, 34);
            this.progressBar.TabIndex = 26;
            // 
            // lblProgressPercentage
            // 
            this.lblProgressPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgressPercentage.AutoSize = true;
            this.lblProgressPercentage.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblProgressPercentage.Location = new System.Drawing.Point(450, 592);
            this.lblProgressPercentage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProgressPercentage.Name = "lblProgressPercentage";
            this.lblProgressPercentage.Size = new System.Drawing.Size(36, 24);
            this.lblProgressPercentage.TabIndex = 27;
            this.lblProgressPercentage.Text = "0%";
            this.lblProgressPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblProgressTableName
            // 
            this.lblProgressTableName.AutoSize = true;
            this.lblProgressTableName.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblProgressTableName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblProgressTableName.Location = new System.Drawing.Point(4, 170);
            this.lblProgressTableName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProgressTableName.Name = "lblProgressTableName";
            this.lblProgressTableName.Size = new System.Drawing.Size(178, 24);
            this.lblProgressTableName.TabIndex = 28;
            this.lblProgressTableName.Text = "进度表名输入：";
            // 
            // tbProgressTableName
            // 
            this.tbProgressTableName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProgressTableName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbProgressTableName.Location = new System.Drawing.Point(194, 165);
            this.tbProgressTableName.Margin = new System.Windows.Forms.Padding(4);
            this.tbProgressTableName.Name = "tbProgressTableName";
            this.tbProgressTableName.Size = new System.Drawing.Size(356, 35);
            this.tbProgressTableName.TabIndex = 29;
            this.tbProgressTableName.Text = "请输入进度表表名";
            // 
            // UcDataManege
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblProgressTableName);
            this.Controls.Add(this.tbProgressTableName);
            this.Controls.Add(this.lblProgressPercentage);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tbTableName);
            this.Controls.Add(this.lblTableName);
            this.Controls.Add(this.cbDBUpdate);
            this.Controls.Add(this.btBreakConnect);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.cbModelUpdate);
            this.Controls.Add(this.cbDataGet);
            this.Controls.Add(this.tbDataManege);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UcDataManege";
            this.Size = new System.Drawing.Size(556, 902);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDataManege;
        private System.Windows.Forms.CheckBox cbDataGet;
        private System.Windows.Forms.CheckBox cbModelUpdate;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.Button btBreakConnect;
        private System.Windows.Forms.CheckBox cbDBUpdate;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.TextBox tbTableName;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgressPercentage;

        // 新增进度数据表名输入
        private System.Windows.Forms.Label lblProgressTableName;
        private System.Windows.Forms.TextBox tbProgressTableName;
    }
}
