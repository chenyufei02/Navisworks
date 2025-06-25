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
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btClear = new System.Windows.Forms.Button();
            this.cbPause = new System.Windows.Forms.CheckBox();
            this.cbUpdate = new System.Windows.Forms.CheckBox();
            this.btUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbLog.Location = new System.Drawing.Point(0, 189);
            this.tbLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(457, 423);
            this.tbLog.TabIndex = 8;
            this.tbLog.Text = "正在监视模型...\r\n";
            // 
            // btClear
            // 
            this.btClear.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btClear.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btClear.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btClear.Location = new System.Drawing.Point(0, 624);
            this.btClear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(459, 70);
            this.btClear.TabIndex = 9;
            this.btClear.Text = "清除文本框";
            this.btClear.UseVisualStyleBackColor = false;
            this.btClear.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btClear_MouseUp);

            // 
            // cbPause
            // 
            this.cbPause.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbPause.AutoSize = true;
            this.cbPause.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbPause.Location = new System.Drawing.Point(283, 105);
            this.cbPause.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbPause.Name = "cbPause";
            this.cbPause.Size = new System.Drawing.Size(84, 28);
            this.cbPause.TabIndex = 7;
            this.cbPause.Text = "暂停";
            this.cbPause.UseVisualStyleBackColor = true;
            // 
            // cbUpdate
            // 
            this.cbUpdate.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbUpdate.AutoSize = true;
            this.cbUpdate.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbUpdate.Location = new System.Drawing.Point(61, 105);
            this.cbUpdate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbUpdate.Name = "cbUpdate";
            this.cbUpdate.Size = new System.Drawing.Size(132, 28);
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
            this.btUpdate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(459, 76);
            this.btUpdate.TabIndex = 5;
            this.btUpdate.Text = "手动检查更新";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btUpdate_MouseUp);
            // 
            // UcUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.cbPause);
            this.Controls.Add(this.cbUpdate);
            this.Controls.Add(this.btUpdate);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "UcUpdate";
            this.Size = new System.Drawing.Size(459, 694);
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
  