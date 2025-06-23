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
            this.SuspendLayout();
            // 
            // tbDataManege
            // 
            this.tbDataManege.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDataManege.Location = new System.Drawing.Point(0, 78);
            this.tbDataManege.Multiline = true;
            this.tbDataManege.Name = "tbDataManege";
            this.tbDataManege.Size = new System.Drawing.Size(368, 339);
            this.tbDataManege.TabIndex = 0;
            // 
            // cbDataGet
            // 
            this.cbDataGet.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cbDataGet.AutoSize = true;
            this.cbDataGet.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbDataGet.Location = new System.Drawing.Point(14, 451);
            this.cbDataGet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDataGet.Name = "cbDataGet";
            this.cbDataGet.Size = new System.Drawing.Size(108, 24);
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
            this.cbModelUpdate.Location = new System.Drawing.Point(255, 450);
            this.cbModelUpdate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbModelUpdate.Name = "cbModelUpdate";
            this.cbModelUpdate.Size = new System.Drawing.Size(108, 24);
            this.cbModelUpdate.TabIndex = 20;
            this.cbModelUpdate.Text = "更新模型";
            this.cbModelUpdate.UseVisualStyleBackColor = true;
            // 
            // btConnect
            // 
            this.btConnect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btConnect.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btConnect.Location = new System.Drawing.Point(0, 0);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(371, 72);
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
            this.btBreakConnect.Location = new System.Drawing.Point(0, 523);
            this.btBreakConnect.Name = "btBreakConnect";
            this.btBreakConnect.Size = new System.Drawing.Size(371, 78);
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
            this.cbDBUpdate.Location = new System.Drawing.Point(122, 450);
            this.cbDBUpdate.Name = "cbDBUpdate";
            this.cbDBUpdate.Size = new System.Drawing.Size(128, 24);
            this.cbDBUpdate.TabIndex = 23;
            this.cbDBUpdate.Text = "更新数据库";
            this.cbDBUpdate.UseVisualStyleBackColor = true;
            this.cbDBUpdate.CheckedChanged += new System.EventHandler(this.cbDBUpdate_CheckedChanged);
            // 
            // UcDataManege
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbDBUpdate);
            this.Controls.Add(this.btBreakConnect);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.cbModelUpdate);
            this.Controls.Add(this.cbDataGet);
            this.Controls.Add(this.tbDataManege);
            this.Name = "UcDataManege";
            this.Size = new System.Drawing.Size(371, 601);
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
    }
}
