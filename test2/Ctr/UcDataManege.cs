using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using MySql.Data.MySqlClient;
using static test2.Ctr.UcProperties;

namespace test2.Ctr
{
    public partial class UcDataManege : UserControl
    {
        public List<Material> listMaterials = new List<Material>();
        public MySqlConnection conn = null;

        public class ProgressReport
        {
            public int Percentage { get; set; }
            public string StatusMessage { get; set; }
        }

        public UcDataManege()
        {
            InitializeComponent();
            SetControlsEnabled(false);
            cbDBUpdate.Visible = false;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Dock = DockStyle.Fill;
        }

        #region UI 事件处理

        private void btConnect_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                string stringConnect = "server=localhost;port=3306;user id=root;password=123456;database=ximalu;Allow User Variables=True;";
                conn = new MySqlConnection(stringConnect);
                conn.Open();

                btConnect.Enabled = false;
                btBreakConnect.Enabled = true;
                SetControlsEnabled(true);

                MessageBox.Show("数据库连接成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据库连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btBreakConnect_MouseUp(object sender, MouseEventArgs e)
        {
            CloseDatabase();
        }

        private async void cbDataGet_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbDataGet.Checked) return;

            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc == null || string.IsNullOrEmpty(doc.CurrentFileName))
            {
                MessageBox.Show("请先打开并保存一个Navisworks文档！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDataGet.Checked = false;
                return;
            }

            string tableName = tbTableName.Text.Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("请输入要保存的数据表名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDataGet.Checked = false;
                return;
            }

            bool shouldProceed = false;
            try
            {
                var dataSaver = new MySqlDataSaver(conn);
                if (dataSaver.CheckTableExists(tableName))
                {
                    var result = MessageBox.Show($"数据表 '{tableName}' 已存在。\n\n是否要清空该表并用当前模型的数据完全覆盖它？", "确认操作", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        shouldProceed = true;
                    }
                    else
                    {
                        cbDataGet.Checked = false;
                        return;
                    }
                }
                else
                {
                    shouldProceed = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检查数据表时出错: {ex.Message}", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbDataGet.Checked = false;
                return;
            }

            if (shouldProceed)
            {
                SetControlsEnabled(false);
                listMaterials.Clear();
                tbDataManege.Clear();

                var progress = new Progress<ProgressReport>(report =>
                {
                    progressBar.Value = report.Percentage;
                    lblProgressPercentage.Text = $"{report.Percentage}%";
                    if (!string.IsNullOrEmpty(report.StatusMessage))
                        tbDataManege.AppendText(report.StatusMessage + Environment.NewLine);
                });

                try
                {
                    await Task.Run(() => ProcessDataInBackground(doc, tableName, progress));
                    MessageBox.Show($"数据成功保存到表 '{tableName}' 中！", "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"处理过程中发生严重错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetControlsEnabled(true);
                    cbDataGet.Checked = false;
                    progressBar.Value = 0;
                    lblProgressPercentage.Text = "0%";
                }
            }
        }

        private void cbDBUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDBUpdate.Checked)
            {
                MessageBox.Show("此功能已集成到“获取数据”中。请勾选“获取数据”并按提示操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cbDBUpdate.Checked = false;
            }
        }

        #endregion

        #region 核心逻辑 (后台任务)

        private void ProcessDataInBackground(Document doc, string tableName, IProgress<ProgressReport> progress)
        {
            progress.Report(new ProgressReport { Percentage = 5, StatusMessage = "开始迭代遍历模型..." });

            // **优化**: 使用迭代法替代递归，进行全模型遍历
            IterativeTraversal(doc.Models.Select(m => m.RootItem), progress);

            progress.Report(new ProgressReport { Percentage = 80, StatusMessage = $"遍历和提取完成，共 {listMaterials.Count} 个有效构件。" });
            progress.Report(new ProgressReport { Percentage = 85, StatusMessage = "准备将数据批量保存到数据库..." });

            MySqlDataSaver dataSaver = new MySqlDataSaver(conn);
            dataSaver.SaveMaterials(listMaterials, tableName);

            progress.Report(new ProgressReport { Percentage = 100, StatusMessage = "所有数据已成功保存！" });
        }

        // **新方法**: 使用栈的迭代式深度优先遍历
        private void IterativeTraversal(IEnumerable<ModelItem> rootItems, IProgress<ProgressReport> progress)
        {
            Stack<ModelItem> stack = new Stack<ModelItem>(rootItems);
            int processedCount = 0;

            while (stack.Count > 0)
            {
                ModelItem item = stack.Pop();
                processedCount++;

                // 如果有子节点，将子节点压入栈中继续遍历
                if (item.Children.Any())
                {
                    foreach (var child in item.Children)
                    {
                        stack.Push(child);
                    }
                }
                // 如果没有子节点，则认为是叶节点，提取数据
                else
                {
                    getDatas(item);
                }

                // 更新进度条 (基于已处理的节点数，这是一个估算值)
                if (processedCount % 500 == 0)
                {
                    int percentage = 5 + (int)((double)processedCount / (processedCount + 20000) * 75);
                    progress.Report(new ProgressReport { Percentage = Math.Min(percentage, 79), StatusMessage = "" });
                }
            }
        }

        #endregion

        #region 辅助方法

        private void getDatas(ModelItem item)
        {
            var categoryElement = item.PropertyCategories.FindCategoryByDisplayName("元素");
            if (categoryElement == null) return;

            var categoryItem = item.PropertyCategories.FindCategoryByDisplayName("项目");
            if (categoryItem == null) return;

            var propertyId = categoryElement.Properties.FindPropertyByDisplayName("Id");
            if (propertyId == null) return;

            var propertyVolume = categoryElement.Properties.FindPropertyByDisplayName("体积");
            var propertyArea = categoryElement.Properties.FindPropertyByDisplayName("面积");
            var propertyFile = categoryItem.Properties.FindPropertyByDisplayName("源文件");
            var propertyLayer = categoryItem.Properties.FindPropertyByDisplayName("层");

            var material = new Material
            {
                ID = GetPropertyValue(propertyId) ?? DBNull.Value,
                Name = item.DisplayName,
                Volume = GetPropertyValue(propertyVolume) ?? DBNull.Value,
                Area = GetPropertyValue(propertyArea) ?? DBNull.Value,
                File = GetPropertyValue(propertyFile) ?? DBNull.Value,
                Layer = GetPropertyValue(propertyLayer) ?? DBNull.Value
            };

            listMaterials.Add(material);
        }

        public void CloseDatabase()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Dispose();
                conn = null;
                btConnect.Enabled = true;
                btBreakConnect.Enabled = false;
                SetControlsEnabled(false);
                MessageBox.Show("数据库连接已断开！");
            }
        }

        private void SetControlsEnabled(bool isEnabled)
        {
            cbDataGet.Enabled = isEnabled;
            cbModelUpdate.Enabled = isEnabled;
            tbTableName.Enabled = isEnabled;
            btConnect.Enabled = isEnabled && (conn == null || conn.State != ConnectionState.Open);
            btBreakConnect.Enabled = isEnabled && (conn != null && conn.State == ConnectionState.Open);
        }

        #endregion
    }

    #region 数据模型与数据库操作类

    public class Material
    {
        public object ID { get; set; }
        public string Name { get; set; }
        public object Volume { get; set; }
        public object Area { get; set; }
        public object File { get; set; }
        public object Layer { get; set; }
    }

    public class MySqlDataSaver
    {
        private MySqlConnection conn;

        public MySqlDataSaver(MySqlConnection connection)
        {
            this.conn = connection;
        }

        public void SaveMaterials(List<Material> materials, string tableName)
        {
            if (materials == null) return;

            CreateTableIfNotExists(tableName);
            TruncateTable(tableName);

            if (materials.Count == 0) return;

            string tempCsvFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempCsvFile, ConvertToCsv(materials), Encoding.UTF8);
                var bulkLoader = new MySqlBulkLoader(conn)
                {
                    TableName = tableName,
                    FileName = tempCsvFile,
                    FieldTerminator = ",",
                    LineTerminator = "\r\n",
                    CharacterSet = "utf8",
                    NumberOfLinesToSkip = 0,
                };
                bulkLoader.Load();
            }
            finally
            {
                if (File.Exists(tempCsvFile))
                {
                    File.Delete(tempCsvFile);
                }
            }
        }

        private string ConvertToCsv(List<Material> materials)
        {
            var sb = new StringBuilder();
            foreach (var material in materials)
            {
                var values = new List<string>
                {
                    EscapeCsvField(material.ID),
                    EscapeCsvField(material.Name),
                    EscapeCsvField(material.Volume),
                    EscapeCsvField(material.Area),
                    EscapeCsvField(material.File),
                    EscapeCsvField(material.Layer)
                };
                sb.AppendLine(string.Join(",", values));
            }
            return sb.ToString();
        }

        private string EscapeCsvField(object field)
        {
            if (field == null || field == DBNull.Value) return "";
            string value = field.ToString();
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        public bool CheckTableExists(string tableName)
        {
            string query = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @dbName AND table_name = @tableName";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@dbName", conn.Database);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
            }
        }

        private void TruncateTable(string tableName)
        {
            string query = $"TRUNCATE TABLE `{tableName}`";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void CreateTableIfNotExists(string tableName)
        {
            string createTableQuery = $@"
            CREATE TABLE IF NOT EXISTS `{tableName}` (
                `ID` VARCHAR(255) PRIMARY KEY,
                `Name` TEXT COLLATE utf8_general_ci,
                `Volume` DECIMAL(18, 5),
                `Area` DECIMAL(18, 5),
                `File` TEXT COLLATE utf8_general_ci,
                `Layer` TEXT COLLATE utf8_general_ci
            );";

            using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    #endregion
}