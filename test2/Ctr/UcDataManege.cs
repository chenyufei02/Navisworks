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

// 2025.06.24  01:48  v1.5稳定版 
// 新增：
// 1.提高提取数据速度从5分钟级到10-20秒
// 2.增加数据表名称输入框，用户可自定义要保存的数据表，数据表存在时，用户可选择一键清空该表并保存数据
// 3.增加进度显示与文本框提示
// 4.采用MySqlBulkLoader配合临时表批量导入数据，保存数据到数据库速度从5分钟级到1秒
// 5.增加自增主键P_ID，提升数据库查找速度
// 6.增加提示信息，避免用户误操作
// 7.2025.06.24 新增TimeLiner相关字段提取和MySQL入库

namespace test2.Ctr
{
    public partial class UcDataManege : UserControl
    {
        public List<Material> listMaterials = new List<Material>();
        public MySqlConnection conn = null;

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

            try
            {
                UpdateProgress(0, "开始提取数据...");
                IterativeTraversalWithMinimalPrompt();
                UpdateProgress(100, $"提取完成，共 {listMaterials.Count} 个有效构件。");
                tbDataManege.AppendText($"获取完毕，总计提取到 {listMaterials.Count} 个构件。\r\n");
                MessageBox.Show($"提取完成！一共提取了{listMaterials.Count}个构件。", "提取完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    UpdateProgress(0, "准备批量保存数据...");
                    await Task.Run(() =>
                    {
                        MySqlDataSaver dataSaver = new MySqlDataSaver(conn);
                        dataSaver.SaveMaterials(listMaterials, tableName);
                    });
                    UpdateProgress(100, "所有数据已成功保存！");
                    tbDataManege.AppendText($"保存完毕：共 {listMaterials.Count} 条数据存入表 {tableName}。\r\n");
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

        #region 数据提取逻辑（只关键节点刷新提示，极致性能）

        private void IterativeTraversalWithMinimalPrompt()
        {
            var rootItems = Autodesk.Navisworks.Api.Application.ActiveDocument.Models.Select(m => m.RootItem);
            Stack<ModelItem> stack = new Stack<ModelItem>(rootItems);
            int processedCount = 0;

            int approxTotal = rootItems.Any() ? rootItems.Sum(r => r.Descendants.Count()) : 1;
            if (approxTotal == 0) approxTotal = 10000;

            UpdateProgress(0, $"开始遍历，预计总数{approxTotal}...");

            while (stack.Count > 0)
            {
                ModelItem item = stack.Pop();
                processedCount++;

                if (item.Children.Any())
                {
                    foreach (var child in item.Children) stack.Push(child);
                }
                else
                {
                    var material = TryGetMaterial(item);
                    if (material != null) listMaterials.Add(material);
                }

                // 只每1万条刷新一次进度
                if (processedCount % 10000 == 0)
                {
                    UpdateProgress((int)((double)processedCount / approxTotal * 100), $"已扫描{processedCount}个构件...");
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        // 【核心修改】增加TimeLiner四字段的提取
        private Material TryGetMaterial(ModelItem item)
        {
            var categoryElement = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "元素");
            if (categoryElement == null) return null;

            var propertyVolume = categoryElement.Properties.FirstOrDefault(c => c.DisplayName == "体积");
            if (propertyVolume == null) return null;

            var propertyArea = categoryElement.Properties.FirstOrDefault(c => c.DisplayName == "面积");
            if (propertyArea == null) return null;

            var volume = GetPropertyValue(propertyVolume);
            var area = GetPropertyValue(propertyArea);
            var name = item.DisplayName;

            var categoryItem = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "项目");

            var file = (categoryItem != null) ? GetPropertyValue(categoryItem.Properties.FindPropertyByDisplayName("源文件")) : null;
            var layer = (categoryItem != null) ? GetPropertyValue(categoryItem.Properties.FindPropertyByDisplayName("层")) : null;

            var propertyId = categoryElement.Properties.FindPropertyByDisplayName("Id");
            var id = GetPropertyValue(propertyId);

            // === 新增TimeLiner提取 start ===
            var timelinerCategory = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "TimeLiner");
            DateTime? plannedStart = null, plannedEnd = null, actualStart = null, actualEnd = null;
            if (timelinerCategory != null)
            {
                foreach (var prop in timelinerCategory.Properties)
                {
                    string propName = prop.DisplayName;
                    string rawValue = GetPropertyValue(prop)?.ToString();

                    string timeStr = null;
                    if (!string.IsNullOrEmpty(rawValue))
                    {
                        int idx = rawValue.IndexOf(':');
                        if (idx >= 0 && rawValue.Length > idx + 1)
                            timeStr = rawValue.Substring(idx + 1).Trim();
                        else
                            timeStr = rawValue.Trim();
                    }

                    if (propName.Contains("任务起点 (计划)"))
                    {
                        plannedStart = TryParseTime(timeStr);
                    }
                    else if (propName.Contains("任务终点 (计划)"))
                    {
                        plannedEnd = TryParseTime(timeStr);
                    }
                    else if (propName.Contains("任务起点 (实际)"))
                    {
                        actualStart = TryParseTime(timeStr);
                    }
                    else if (propName.Contains("任务终点 (实际)"))
                    {
                        actualEnd = TryParseTime(timeStr);
                    }
                }
            }
            // === 新增TimeLiner提取 end ===

            return new Material
            {
                Name = name,
                ID = id,
                Volume = volume,
                Area = area,
                File = file,
                Layer = layer,
                PlannedStart = plannedStart,
                PlannedEnd = plannedEnd,
                ActualStart = actualStart,
                ActualEnd = actualEnd
            };
        }

        // 新增：辅助方法 时间字符串转DateTime?（解析失败返回null）
        private DateTime? TryParseTime(string timeStr)
        {
            if (DateTime.TryParse(timeStr, out DateTime dt))
                return dt;
            return null;
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
            if (!string.IsNullOrEmpty(message))
            {
                tbDataManege.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            }
        }

        #endregion

        #region 其它方法...

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

    // Material类 增加TimeLiner字段
    public class Material
    {
        public object ID { get; set; }
        public string Name { get; set; }
        public object Volume { get; set; }
        public object Area { get; set; }
        public object File { get; set; }
        public object Layer { get; set; }
        // 新增
        public DateTime? PlannedStart { get; set; }
        public DateTime? PlannedEnd { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
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
            if (materials == null || materials.Count == 0) return;

            // 自动检测secure_file_priv目录
            string secureFilePriv = null;
            using (var cmd = new MySqlCommand("SHOW VARIABLES LIKE 'secure_file_priv'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    secureFilePriv = reader.GetString(1);
                }
            }

            string csvFileName = $"import_{Guid.NewGuid():N}.csv";
            string csvFilePath = string.IsNullOrEmpty(secureFilePriv)
                ? Path.Combine(Path.GetTempPath(), csvFileName)
                : Path.Combine(secureFilePriv, csvFileName);

            File.WriteAllText(csvFilePath, ConvertToCsv(materials), Encoding.UTF8);

            try
            {
                if (CheckTableExists(tableName))
                {
                    DropTable(tableName); // 新增：彻底删除表
                }
                CreateTableIfNotExists(tableName); // 然后用新结构重建


                // 写入全部业务字段
                var bulkLoader = new MySqlBulkLoader(conn)
                {
                    TableName = tableName,
                    FileName = csvFilePath,
                    FieldTerminator = ",",
                    LineTerminator = "\r\n",
                    CharacterSet = "utf8",
                    NumberOfLinesToSkip = 0
                };
                bulkLoader.Columns.AddRange(new[] {
                    "ID", "Name", "Volume", "Area", "File", "Layer",
                    "PlannedStart", "PlannedEnd", "ActualStart", "ActualEnd"
                });
                bulkLoader.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"批量写入数据失败：{ex.Message}", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                if (File.Exists(csvFilePath))
                    File.Delete(csvFilePath);
            }
        }

        // 新增：TimeLiner字段导出
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
            EscapeCsvField(material.Layer),
            // 下面四个字段，如果没有值，直接写 \N（不带引号！）
            material.PlannedStart.HasValue ? material.PlannedStart.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N",
            material.PlannedEnd.HasValue ? material.PlannedEnd.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N",
            material.ActualStart.HasValue ? material.ActualStart.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N",
            material.ActualEnd.HasValue ? material.ActualEnd.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N"
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

        private void DropTable(string tableName)
        {
            string query = $"DROP TABLE IF EXISTS `{tableName}`";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }


        // 【核心修改】增加TimeLiner四字段
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
                `PlannedStart` DATETIME NULL,
                `PlannedEnd` DATETIME NULL,
                `ActualStart` DATETIME NULL,
                `ActualEnd` DATETIME NULL,
                INDEX `idx_ID` (`ID`)
            );";
            using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
