using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace test2.Ctr
{
    public partial class UcUpdate : UserControl
    {
        public Timer UpTimer = new Timer { Enabled = true, Interval = 1000 };
        public List<FileInfo> ListInfos = new List<FileInfo>();

        public UcUpdate()
        {
            InitializeComponent();
            btUpdate.Enabled = false;

            UpTimer.Tick += UptimerOnTick;
            Autodesk.Navisworks.Api.Application.ActiveDocumentChanged += ApplicationOnActiveDocumentChanged;

        }

        private void ApplicationOnActiveDocumentChanged(object sender, EventArgs e)
        {
            ListInfos.Clear();
        }

        private void UptimerOnTick(object sender, EventArgs e)
        {

            if(cbPause.Checked)
            {
                cbUpdate.Enabled = false;
                btUpdate.Enabled = true;  // 启用手动点击的Update更新按钮
            }
            else if (!cbPause.Checked)
            {
                cbUpdate.Enabled = true;
                if (cbUpdate.Checked)
                {
                    btUpdate.Enabled = false;
                }
                else
                    btUpdate.Enabled = true;
            }

            if (cbPause.Checked) return;
            if (Autodesk.Navisworks.Api.Application.ActiveDocument == null) return;
            var activeDocument = Autodesk.Navisworks.Api.Application.ActiveDocument;
            foreach (var model in activeDocument.Models)
            {
                var currentInfo = new FileInfo(model.SourceFileName);
                var lastInfo = ListInfos.FirstOrDefault(i => i.FullName == currentInfo.FullName);

                if (lastInfo != null)
                {
                    var time = Math.Abs((lastInfo.LastWriteTime - currentInfo.LastWriteTime).TotalSeconds);
                    if (time > 1)
                    {
                        ListInfos.Remove(lastInfo);
                        ListInfos.Add(currentInfo);
                        tbLog.AppendText(string.Concat(currentInfo.Name, "模型信息已更新", Environment.NewLine));
                        if (cbUpdate.Checked)
                        {
                            UpdateModel();
                            tbLog.AppendText(string.Concat(currentInfo.FullName, "模型已自动更新！", "\r\n"));
                        }
                    }
                }
                else
                {
                    ListInfos.Add(currentInfo);
                }
            }
        }

        private void UpdateModel()
        {
            Autodesk.Navisworks.Api.Application.ActiveDocument.UpdateFiles();
            tbLog.AppendText(string.Concat("活动文档更新完成！" ,Environment.NewLine));
            btUpdate.Enabled = false;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Dock = DockStyle.Fill;
        }

        private void btUpdate_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateModel();
        }

        private void btClear_MouseUp(object sender, MouseEventArgs e)
        {
            tbLog.Clear();
        }

        MySqlConnection conn;
        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            conn = new MySqlConnection("server=localhost;port=3306;user id=root;password=123456;database=ximalu;");
            conn.Open();
            MessageBox.Show("数据库连接成功！");
        }
    }
}
