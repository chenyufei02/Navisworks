namespace WindowsFormsApp1
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btStartNavis = new System.Windows.Forms.Button();
            this.cbData = new System.Windows.Forms.CheckBox();
            this.cbConnect = new System.Windows.Forms.CheckBox();
            this.cbBreak = new System.Windows.Forms.CheckBox();
            this.cbModel = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btStartNavis
            // 
            this.btStartNavis.Dock = System.Windows.Forms.DockStyle.Top;
            this.btStartNavis.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btStartNavis.Location = new System.Drawing.Point(0, 0);
            this.btStartNavis.Name = "btStartNavis";
            this.btStartNavis.Size = new System.Drawing.Size(350, 62);
            this.btStartNavis.TabIndex = 1;
            this.btStartNavis.Text = "打开NAVISWORKS";
            this.btStartNavis.UseVisualStyleBackColor = true;
            this.btStartNavis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btStartNavis_MouseUp);
            // 
            // cbData
            // 
            this.cbData.AutoSize = true;
            this.cbData.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbData.Location = new System.Drawing.Point(12, 361);
            this.cbData.Name = "cbData";
            this.cbData.Size = new System.Drawing.Size(108, 23);
            this.cbData.TabIndex = 3;
            this.cbData.Text = "更新数据";
            this.cbData.UseVisualStyleBackColor = true;
            this.cbData.CheckedChanged += new System.EventHandler(this.cbData_CheckedChanged);
            // 
            // cbConnect
            // 
            this.cbConnect.AutoSize = true;
            this.cbConnect.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbConnect.Location = new System.Drawing.Point(12, 316);
            this.cbConnect.Name = "cbConnect";
            this.cbConnect.Size = new System.Drawing.Size(128, 23);
            this.cbConnect.TabIndex = 4;
            this.cbConnect.Text = "连接数据库";
            this.cbConnect.UseVisualStyleBackColor = true;
            this.cbConnect.CheckedChanged += new System.EventHandler(this.cbConnect_CheckedChanged);
            // 
            // cbBreak
            // 
            this.cbBreak.AutoSize = true;
            this.cbBreak.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbBreak.Location = new System.Drawing.Point(170, 316);
            this.cbBreak.Name = "cbBreak";
            this.cbBreak.Size = new System.Drawing.Size(128, 23);
            this.cbBreak.TabIndex = 5;
            this.cbBreak.Text = "断开数据库";
            this.cbBreak.UseVisualStyleBackColor = true;
            this.cbBreak.CheckedChanged += new System.EventHandler(this.cbBreak_CheckedChanged);
            // 
            // cbModel
            // 
            this.cbModel.AutoSize = true;
            this.cbModel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbModel.Location = new System.Drawing.Point(170, 361);
            this.cbModel.Name = "cbModel";
            this.cbModel.Size = new System.Drawing.Size(108, 23);
            this.cbModel.TabIndex = 6;
            this.cbModel.Text = "更新模型";
            this.cbModel.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 419);
            this.Controls.Add(this.cbModel);
            this.Controls.Add(this.cbBreak);
            this.Controls.Add(this.cbConnect);
            this.Controls.Add(this.cbData);
            this.Controls.Add(this.btStartNavis);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btStartNavis;
        private System.Windows.Forms.CheckBox cbData;
        private System.Windows.Forms.CheckBox cbConnect;
        private System.Windows.Forms.CheckBox cbBreak;
        private System.Windows.Forms.CheckBox cbModel;
    }
}

