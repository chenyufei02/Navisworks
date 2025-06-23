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
        private List<string> _diagnosticLog; // 用于存储诊断日志

        public UcDataManege()
        {
            InitializeComponent();
            cbDataGet.Enabled = false;
            cbDBUpdate.Enabled = false;
            cbModelUpdate.Enabled = false;
            btBreakConnect.Enabled = false;
            tbTableName.Enabled = false;
            cbDBUpdate.Visible = true;
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
                cbDataGet.Enabled = true;
                cbModelUpdate.Enabled = true;
                tbTableName.Enabled = true;

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

        private void cbDataGet_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbDataGet.Checked) return;

            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc == null || string.IsNullOrEmpty(doc.CurrentFileName))
            {
                MessageBox.Show("请先打开并保存一个Navisworks文档！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDataGet.Checked = false;
                return;
            }

            btBreakConnect.Enabled = false;
            cbDataGet.Enabled = false;
            cbModelUpdate.Enabled = false;
            tbTableName.Enabled = false;
            cbDBUpdate.Enabled = false;

            listMaterials.Clear();
            tbDataManege.Clear();
            _diagnosticLog = new List<string>(); // 清空诊断日志

            try
            {
                UpdateProgress(5, "开始迭代遍历模型...");
                IterativeTraversal();

                string finalMessage = $"数据提取完成，共 {listMaterials.Count} 个有效构件。";
                UpdateProgress(100, finalMessage);

                // **诊断核心**: 如果有诊断信息，则弹窗显示
                if (_diagnosticLog.Count > 0)
                {
                    // 只显示前20条，避免弹窗过大
                    string logToShow = string.Join("\n", _diagnosticLog.Take(20));
                    MessageBox.Show("提取到 0 个构件，以下是部分构件被跳过的原因，请将此信息反馈给我：\n\n" + logToShow, "诊断信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (listMaterials.Count > 0)
                {
                    MessageBox.Show("数据提取完成！", "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("数据提取完成，但未找到任何符合条件的构件。", "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"提取数据时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btBreakConnect.Enabled = true;
                cbDataGet.Enabled = true;
                cbModelUpdate.Enabled = true;
                tbTableName.Enabled = true;
                cbDBUpdate.Enabled = true;

                cbDataGet.Checked = false;
                progressBar.Value = 0;
                lblProgressPercentage.Text = "0%";
            }
        }

        private async void cbDBUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbDBUpdate.Checked) return;

            if (listMaterials == null || listMaterials.Count == 0)
            {
                MessageBox.Show("没有可供保存的数据。请先“获取数据”。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDBUpdate.Checked = false;
                return;
            }

            string tableName = tbTableName.Text.Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("请输入要保存的数据表名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDBUpdate.Checked = false;
                return;
            }

            bool shouldProceed = false;
            try
            {
                var dataSaver = new MySqlDataSaver(conn);
                if (dataSaver.CheckTableExists(tableName))
                {
                    var result = MessageBox.Show($"数据表 '{tableName}' 已存在。\n\n是否要清空该表并用新提取的数据完全覆盖它？", "确认操作", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    shouldProceed = (result == DialogResult.Yes);
                }
                else
                {
                    shouldProceed = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检查数据表时出错: {ex.Message}", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbDBUpdate.Checked = false;
                return;
            }

            if (shouldProceed)
            {
                btBreakConnect.Enabled = false;
                cbDataGet.Enabled = false;
                cbModelUpdate.Enabled = false;
                tbTableName.Enabled = false;
                cbDBUpdate.Enabled = false;

                try
                {
                    UpdateProgress(0, "准备将数据批量保存到数据库(后台线程)...");
                    await Task.Run(() =>
                    {
                        MySqlDataSaver dataSaver = new MySqlDataSaver(conn);
                        dataSaver.SaveMaterials(listMaterials, tableName);
                    });
                    UpdateProgress(100, "所有数据已成功保存！");
                    MessageBox.Show($"数据成功保存到表 '{tableName}' 中！", "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存数据时发生严重错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btBreakConnect.Enabled = true;
                    cbDataGet.Enabled = true;
                    cbModelUpdate.Enabled = true;
                    tbTableName.Enabled = true;
                    cbDBUpdate.Enabled = true;

                    cbDBUpdate.Checked = false;
                    progressBar.Value = 0;
                    lblProgressPercentage.Text = "0%";
                }
            }
            else
            {
                cbDBUpdate.Checked = false;
            }
        }

        #endregion

        #region 核心逻辑

        private void IterativeTraversal()
        {
            var rootItems = Autodesk.Navisworks.Api.Application.ActiveDocument.Models.Select(m => m.RootItem);
            Stack<ModelItem> stack = new Stack<ModelItem>(rootItems);
            int processedCount = 0;

            int approxTotal = rootItems.Any() ? rootItems.Sum(r => r.Descendants.Count()) : 1;
            if (approxTotal == 0) approxTotal = 10000;

            while (stack.Count > 0)
            {
                ModelItem item = stack.Pop();
                processedCount++;

                if (item.Children.Any())
                {
                    foreach (var child in item.Children)
                    {
                        stack.Push(child);
                    }
                }
                else
                {
                    getDatas(item);
                }

                if (processedCount % 500 == 0)
                {
                    int percentage = 5 + (int)((double)processedCount / approxTotal * 95);
                    UpdateProgress(Math.Min(percentage, 100), null);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        private void UpdateProgress(int percentage, string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateProgress(percentage, message)));
                return;
            }

            progressBar.Value = Math.Min(100, percentage);
            lblProgressPercentage.Text = $"{Math.Min(100, percentage)}%";
            if (message != null && !string.IsNullOrEmpty(message))
            {
                tbDataManege.AppendText(message + Environment.NewLine);
            }
        }

        #endregion

        #region 辅助方法

        // **诊断版**: 添加了诊断日志记录
        private void getDatas(ModelItem item)
        {
            var categoryElement = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "元素");
            if (categoryElement == null)
            {
                _diagnosticLog.Add($"跳过'{item.DisplayName}': 未找到'元素'属性类别。");
                return;
            }

            var propertyVolume = categoryElement.Properties.FirstOrDefault(c => c.DisplayName == "体积");
            if (propertyVolume == null)
            {
                _diagnosticLog.Add($"跳过'{item.DisplayName}': 在'元素'类别中未找到'体积'属性。");
                return;
            }

            var propertyArea = categoryElement.Properties.FirstOrDefault(c => c.DisplayName == "面积");
            if (propertyArea == null)
            {
                _diagnosticLog.Add($"跳过'{item.DisplayName}': 在'元素'类别中未找到'面积'属性。");
                return;
            }

            var volume = GetPropertyValue(propertyVolume);
            var area = GetPropertyValue(propertyArea);
            var name = item.DisplayName;

            var categoryItem = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "项目");

            var file = (categoryItem != null) ? GetPropertyValue(categoryItem.Properties.FindPropertyByDisplayName("源文件")) : null;
            var layer = (categoryItem != null) ? GetPropertyValue(categoryItem.Properties.FindPropertyByDisplayName("层")) : null;

            var propertyId = categoryElement.Properties.FindPropertyByDisplayName("Id");
            var id = GetPropertyValue(propertyId);

            var material = new Material
            {
                Name = name,
                ID = id ?? DBNull.Value,
                Volume = volume ?? DBNull.Value,
                Area = area ?? DBNull.Value,
                File = file ?? DBNull.Value,
                Layer = layer ?? DBNull.Value
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
                cbDataGet.Enabled = false;
                cbDBUpdate.Enabled = false;
                cbModelUpdate.Enabled = false;
                tbTableName.Enabled = false;
                MessageBox.Show("数据库连接已断开！");
            }
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
                `P_Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                `ID` VARCHAR(255),
                `Name` TEXT COLLATE utf8_general_ci,
                `Volume` DECIMAL(18, 5),
                `Area` DECIMAL(18, 5),
                `File` TEXT COLLATE utf8_general_ci,
                `Layer` TEXT COLLATE utf8_general_ci,
                INDEX `idx_ID` (`ID`)
            );";

            using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    #endregion
}