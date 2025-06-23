namespace test2.Ctr
{
    partial class UcProperties
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
            this.BtFind = new System.Windows.Forms.Button();
            this.lbPropertyValue = new System.Windows.Forms.Label();
            this.tbPropertyValue = new System.Windows.Forms.TextBox();
            this.lbPropertyName = new System.Windows.Forms.Label();
            this.tbPropertyName = new System.Windows.Forms.TextBox();
            this.lbCategoryName = new System.Windows.Forms.Label();
            this.tbCategoryName = new System.Windows.Forms.TextBox();
            this.TbOut = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BtFind
            // 
            this.BtFind.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BtFind.Location = new System.Drawing.Point(0, 583);
            this.BtFind.Name = "BtFind";
            this.BtFind.Size = new System.Drawing.Size(251, 34);
            this.BtFind.TabIndex = 15;
            this.BtFind.Text = "查找";
            this.BtFind.UseVisualStyleBackColor = true;
            this.BtFind.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtFind_MouseUp);
            // 
            // lbPropertyValue
            // 
            this.lbPropertyValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbPropertyValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPropertyValue.Location = new System.Drawing.Point(2, 552);
            this.lbPropertyValue.Name = "lbPropertyValue";
            this.lbPropertyValue.Size = new System.Drawing.Size(96, 21);
            this.lbPropertyValue.TabIndex = 14;
            this.lbPropertyValue.Text = "目标属性值：";
            this.lbPropertyValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbPropertyValue
            // 
            this.tbPropertyValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPropertyValue.Location = new System.Drawing.Point(103, 552);
            this.tbPropertyValue.Name = "tbPropertyValue";
            this.tbPropertyValue.Size = new System.Drawing.Size(146, 21);
            this.tbPropertyValue.TabIndex = 13;
            // 
            // lbPropertyName
            // 
            this.lbPropertyName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbPropertyName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPropertyName.Location = new System.Drawing.Point(2, 528);
            this.lbPropertyName.Name = "lbPropertyName";
            this.lbPropertyName.Size = new System.Drawing.Size(96, 21);
            this.lbPropertyName.TabIndex = 12;
            this.lbPropertyName.Text = "目标属性名：";
            this.lbPropertyName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbPropertyName
            // 
            this.tbPropertyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPropertyName.Location = new System.Drawing.Point(103, 528);
            this.tbPropertyName.Name = "tbPropertyName";
            this.tbPropertyName.Size = new System.Drawing.Size(146, 21);
            this.tbPropertyName.TabIndex = 11;
            // 
            // lbCategoryName
            // 
            this.lbCategoryName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbCategoryName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCategoryName.Location = new System.Drawing.Point(2, 504);
            this.lbCategoryName.Margin = new System.Windows.Forms.Padding(3);
            this.lbCategoryName.Name = "lbCategoryName";
            this.lbCategoryName.Size = new System.Drawing.Size(96, 21);
            this.lbCategoryName.TabIndex = 10;
            this.lbCategoryName.Text = "项目种类：";
            this.lbCategoryName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbCategoryName
            // 
            this.tbCategoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCategoryName.Location = new System.Drawing.Point(103, 504);
            this.tbCategoryName.Name = "tbCategoryName";
            this.tbCategoryName.Size = new System.Drawing.Size(146, 21);
            this.tbCategoryName.TabIndex = 9;
            // 
            // TbOut
            // 
            this.TbOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TbOut.Location = new System.Drawing.Point(-1, 0);
            this.TbOut.Multiline = true;
            this.TbOut.Name = "TbOut";
            this.TbOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TbOut.Size = new System.Drawing.Size(253, 455);
            this.TbOut.TabIndex = 8;
            // 
            // UcProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BtFind);
            this.Controls.Add(this.lbPropertyValue);
            this.Controls.Add(this.tbPropertyValue);
            this.Controls.Add(this.lbPropertyName);
            this.Controls.Add(this.tbPropertyName);
            this.Controls.Add(this.lbCategoryName);
            this.Controls.Add(this.tbCategoryName);
            this.Controls.Add(this.TbOut);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UcProperties";
            this.Size = new System.Drawing.Size(251, 617);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtFind;
        private System.Windows.Forms.Label lbPropertyValue;
        private System.Windows.Forms.TextBox tbPropertyValue;
        private System.Windows.Forms.Label lbPropertyName;
        private System.Windows.Forms.TextBox tbPropertyName;
        private System.Windows.Forms.Label lbCategoryName;
        private System.Windows.Forms.TextBox tbCategoryName;
        private System.Windows.Forms.TextBox TbOut;
    }
}
