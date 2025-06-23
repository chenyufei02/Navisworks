using System;

namespace test2.Ctr
{
    partial class UcUpdate
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
            this.btClear = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.cbPause = new System.Windows.Forms.CheckBox();
            this.cbUpdate = new System.Windows.Forms.CheckBox();
            this.btUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btClear
            // 
            this.btClear.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btClear.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btClear.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btClear.Location = new System.Drawing.Point(0, 442);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(262, 47);
            this.btClear.TabIndex = 9;
            this.btClear.Text = "清除文本框";
            this.btClear.UseVisualStyleBackColor = false;
            this.btClear.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btClear_MouseUp);
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbLog.Location = new System.Drawing.Point(0, 126);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(262, 310);
            this.tbLog.TabIndex = 8;
            this.tbLog.Text = "正在监视模型...\r\n";
            // 
            // cbPause
            // 
            this.cbPause.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbPause.AutoSize = true;
            this.cbPause.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbPause.Location = new System.Drawing.Point(167, 70);
            this.cbPause.Name = "cbPause";
            this.cbPause.Size = new System.Drawing.Size(58, 20);
            this.cbPause.TabIndex = 7;
            this.cbPause.Text = "暂停";
            this.cbPause.UseVisualStyleBackColor = true;
            // 
            // cbUpdate
            // 
            this.cbUpdate.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbUpdate.AutoSize = true;
            this.cbUpdate.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbUpdate.Location = new System.Drawing.Point(19, 70);
            this.cbUpdate.Name = "cbUpdate";
            this.cbUpdate.Size = new System.Drawing.Size(90, 20);
            this.cbUpdate.TabIndex = 6;
            this.cbUpdate.Text = "自动更新";
            this.cbUpdate.UseVisualStyleBackColor = true;
            // 
            // btUpdate
            // 
            this.btUpdate.AutoSize = true;
            this.btUpdate.Dock = System.Windows.Forms.DockStyle.Top;
            this.btUpdate.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btUpdate.Location = new System.Drawing.Point(0, 0);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(262, 51);
            this.btUpdate.TabIndex = 5;
            this.btUpdate.Text = "手动检查更新";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btUpdate_MouseUp);
            // 
            // UcUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.cbPause);
            this.Controls.Add(this.cbUpdate);
            this.Controls.Add(this.btUpdate);
            this.Name = "UcUpdate";
            this.Size = new System.Drawing.Size(262, 489);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.CheckBox cbPause;
        private System.Windows.Forms.CheckBox cbUpdate;
        private System.Windows.Forms.Button btUpdate;
    }
}
  