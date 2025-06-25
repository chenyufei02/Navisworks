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

// 2025.06.25  v1.6 进度与模型分离版
// 新增：
// 1. 独立提取每栋楼每一层的进度数据到专用进度表，提取字段：Building、Floor、PlannedStart、PlannedEnd、ActualStart、ActualEnd
// 2. 支持自定义进度数据表名，支持批量导入
// 3. UI支持进度表名输入和提取进度数据专用复选框
// 4. 进度条、按钮逻辑自适应模型和进度表两套流程
// 5. 优化遍历逻辑，确保每栋每层只提取一次,避免重复提取数据 对99%不满足的数据快速跳过
// 6. 逻辑及命名风格与原版完全一致

namespace test2.Ctr
{
    public partial class UcDataManege : UserControl
    {
        public List<Material> listMaterials = new List<Material>();
        public List<ProgressRecord> progressRecords = new List<ProgressRecord>();
        public MySqlConnection conn = null;

        public UcDataManege()
        {
            InitializeComponent();
            cbDataGet.Enabled = false;
            cbDBUpdate.Enabled = false;
            cbModelUpdate.Enabled = false;
            btBreakConnect.Enabled = false;
            tbTableName.Enabled = false;
            tbProgressTableName.Enabled = false;
            cbDBUpdate.Visible = true;
            cbModelUpdate.Visible = true;
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
                cbDBUpdate.Enabled = true;
                tbTableName.Enabled = true;
                tbProgressTableName.Enabled = true;

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

        // 获取数据（模型数据）
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
            cbDBUpdate.Enabled = false;
            tbTableName.Enabled = false;
            tbProgressTableName.Enabled = false;

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
                cbDBUpdate.Enabled = true;
                tbTableName.Enabled = true;
                tbProgressTableName.Enabled = true;

                cbDataGet.Checked = false;
                progressBar.Value = 0;
                lblProgressPercentage.Text = "0%";
            }
        }

        // “更新模型数据库”
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
                cbDBUpdate.Enabled = false;
                tbTableName.Enabled = false;
                tbProgressTableName.Enabled = false;

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
                    cbDBUpdate.Enabled = true;
                    tbTableName.Enabled = true;
                    tbProgressTableName.Enabled = true;

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

        // “更新进度数据库”按钮（核心）
        private async void cbModelUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbModelUpdate.Checked) return;

            // 准备提取进度数据
            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc == null || string.IsNullOrEmpty(doc.CurrentFileName))
            {
                MessageBox.Show("请先打开并保存一个Navisworks文档！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbModelUpdate.Checked = false;
                return;
            }
            string tableName = tbProgressTableName.Text.Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("请输入要保存的进度数据表名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbModelUpdate.Checked = false;
                return;
            }

            btBreakConnect.Enabled = false;
            cbDataGet.Enabled = false;
            cbModelUpdate.Enabled = false;
            cbDBUpdate.Enabled = false;
            tbTableName.Enabled = false;
            tbProgressTableName.Enabled = false;

            progressRecords.Clear();
            tbDataManege.Clear();

            try
            {
                UpdateProgress(0, "开始提取进度数据...");
                ExtractBuildingProgressRecords();
                UpdateProgress(100, $"提取完成，共 {progressRecords.Count} 个进度节点。");

                bool shouldProceed = false;
                var dataSaver = new MySqlDataSaver(conn);
                if (dataSaver.CheckProgressTableExists(tableName))
                {
                    var result = MessageBox.Show($"进度数据表 '{tableName}' 已存在。\n\n是否要清空该表并用新数据覆盖？", "确认操作", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    shouldProceed = (result == DialogResult.Yes);
                }
                else
                {
                    shouldProceed = true;
                }

                if (shouldProceed)
                {
                    UpdateProgress(0, "准备批量保存进度数据...");
                    await Task.Run(() =>
                    {
                        MySqlDataSaver saver = new MySqlDataSaver(conn);
                        saver.SaveProgressRecords(progressRecords, tableName);
                    });
                    UpdateProgress(100, $"所有进度数据已保存到 {tableName}。");
                    tbDataManege.AppendText($"保存完毕：共 {progressRecords.Count} 条进度数据存入表 {tableName}。\r\n");
                    MessageBox.Show($"进度数据已保存到表 '{tableName}'。", "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"提取或保存进度数据时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btBreakConnect.Enabled = true;
                cbDataGet.Enabled = true;
                cbModelUpdate.Enabled = true;
                cbDBUpdate.Enabled = true;
                tbTableName.Enabled = true;
                tbProgressTableName.Enabled = true;

                cbModelUpdate.Checked = false;
                progressBar.Value = 0;
                lblProgressPercentage.Text = "0%";
            }
        }

        #endregion

        #region 数据提取逻辑（模型数据）
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

                if (processedCount % 10000 == 0)
                {
                    UpdateProgress((int)((double)processedCount / approxTotal * 100), $"已扫描{processedCount}个构件...");
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

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

            // TimeLiner
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
                        plannedStart = TryParseTime(timeStr);
                    else if (propName.Contains("任务终点 (计划)"))
                        plannedEnd = TryParseTime(timeStr);
                    else if (propName.Contains("任务起点 (实际)"))
                        actualStart = TryParseTime(timeStr);
                    else if (propName.Contains("任务终点 (实际)"))
                        actualEnd = TryParseTime(timeStr);
                }
            }

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

        #region 新增：进度数据独立提取
        private void ExtractBuildingProgressRecords()
        {
            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            var rootItems = doc.Models.Select(m => m.RootItem);

            // 用HashSet判重 (楼栋-楼层)
            var picked = new HashSet<string>();
            int processedCount = 0, storedCount = 0;
            int approxTotal = rootItems.Any() ? rootItems.Sum(r => r.Descendants.Count()) : 1;
            if (approxTotal == 0) approxTotal = 10000;

            UpdateProgress(0, $"开始遍历，预计总数{approxTotal}...");

            Stack<ModelItem> stack = new Stack<ModelItem>(rootItems);

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

                    // 提取“楼栋”和“楼层”
                    string building = ExtractBuilding(GetProperty(item, "项目", "源文件"));
                    string floor = ExtractFloor(GetProperty(item, "项目", "层"));

                    if (string.IsNullOrEmpty(building) || string.IsNullOrEmpty(floor)) continue;

                    string key = $"{building}-{floor}";
                    if (picked.Contains(key)) continue;

                    // 提取TimeLiner字段
                    DateTime? plannedStart = null, plannedEnd = null, actualStart = null, actualEnd = null;
                    var timelinerCategory = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "TimeLiner");
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
                                plannedStart = TryParseTime(timeStr);
                            else if (propName.Contains("任务终点 (计划)"))
                                plannedEnd = TryParseTime(timeStr);
                            else if (propName.Contains("任务起点 (实际)"))
                                actualStart = TryParseTime(timeStr);
                            else if (propName.Contains("任务终点 (实际)"))
                                actualEnd = TryParseTime(timeStr);
                        }
                    }

                    // 只记录至少有一个时间的节点
                    if (plannedStart.HasValue || plannedEnd.HasValue || actualStart.HasValue || actualEnd.HasValue)
                    {
                        picked.Add(key);
                        progressRecords.Add(new ProgressRecord
                        {
                            Building = building,
                            Floor = floor,
                            PlannedStart = plannedStart,
                            PlannedEnd = plannedEnd,
                            ActualStart = actualStart,
                            ActualEnd = actualEnd
                        });
                        storedCount++;
                    }
                }
                if (processedCount % 10000 == 0)
                {
                    UpdateProgress((int)((double)processedCount / approxTotal * 100), $"已扫描{processedCount}个构件，已采集{storedCount}个进度节点...");
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        private object GetProperty(ModelItem item, string category, string prop)
        {
            var cat = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == category);
            if (cat == null) return null;
            return GetPropertyValue(cat.Properties.FindPropertyByDisplayName(prop));
        }

        // 对楼栋字段进行数据清洗，只提取楼栋名称
        private string ExtractBuilding(object fileObj)
        {
            var fileStr = fileObj?.ToString();
            if (string.IsNullOrEmpty(fileStr)) return null;

            // 去路径
            int lastSlash = Math.Max(fileStr.LastIndexOf('/'), fileStr.LastIndexOf('\\'));
            if (lastSlash >= 0)
                fileStr = fileStr.Substring(lastSlash + 1);

            // 去扩展名
            if (fileStr.EndsWith(".RVT", StringComparison.OrdinalIgnoreCase))
                fileStr = fileStr.Substring(0, fileStr.Length - 4);

            // 数字#开头，直接提取数字
            var match = System.Text.RegularExpressions.Regex.Match(fileStr, @"^(\d+)#");
            if (match.Success)
                return match.Groups[1].Value;

            // 去掉尾部ST/AR/AT/BT等，如果有
            fileStr = System.Text.RegularExpressions.Regex.Replace(fileStr, "(ST|AR)$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 返回剩下的
            return fileStr.Trim();
        }

        // 对楼层字段进行数据清洗，提取标准和非标准楼层
        private string ExtractFloor(object layerObj)
        {
            var layerStr = layerObj?.ToString();
            if (string.IsNullOrEmpty(layerStr)) return null;

            // 1. 处理标准层数（例如：15F, 16F等）
            var match = System.Text.RegularExpressions.Regex.Match(layerStr, @"(\d+)F");
            if (match.Success) return match.Groups[1].Value;

            // 2. 处理特殊楼层名：屋面层、机房层、机房屋面层、楼梯屋面层等
            if (layerStr.Contains("机房屋面"))
                return "机房屋面层";
            if (layerStr.Contains("楼梯屋面"))
                return "楼梯屋面层";
            if (layerStr.Contains("机房"))
                return "机房层";
            if (layerStr.Contains("屋面"))
                return "屋面层";

            // 3. 处理带有数字的楼层名（例如：S_104.3、其它类似格式）
            var numberMatch = System.Text.RegularExpressions.Regex.Match(layerStr, @"(\S+)");
            if (numberMatch.Success && !numberMatch.Value.Contains("="))
                return "其他层" + numberMatch.Groups[1].Value; // 直接返回“其他层”+数字部分

            // 4. 处理带有等号的层名（例如：=104.3）
            var equalMatch = System.Text.RegularExpressions.Regex.Match(layerStr, @"=(\S+)");
            if (equalMatch.Success)
                return "其他层=" + equalMatch.Groups[1].Value; // 返回“其他层”+数字

            // 5. 如果没有符合的规则，则直接返回原始层名
            return layerStr; // 如果是无法预测的命名，直接返回层名
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
                tbProgressTableName.Enabled = false;
                MessageBox.Show("数据库连接已断开！");
            }
        }
        #endregion

        #region 数据对象

        // 进度表数据结构
        public class ProgressRecord
        {
            public string Building { get; set; }
            public string Floor { get; set; }
            public DateTime? PlannedStart { get; set; }
            public DateTime? PlannedEnd { get; set; }
            public DateTime? ActualStart { get; set; }
            public DateTime? ActualEnd { get; set; }
        }
        #endregion

        #region MySqlDataSaver扩展
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
                        DropTable(tableName);
                    }
                    CreateTableIfNotExists(tableName);

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

            // ====================== 进度表部分 ========================
            public void SaveProgressRecords(List<ProgressRecord> progressRecords, string tableName)
            {
                if (progressRecords == null || progressRecords.Count == 0) return;

                string secureFilePriv = null;
                using (var cmd = new MySqlCommand("SHOW VARIABLES LIKE 'secure_file_priv'", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        secureFilePriv = reader.GetString(1);
                    }
                }

                string csvFileName = $"progress_{Guid.NewGuid():N}.csv";
                string csvFilePath = string.IsNullOrEmpty(secureFilePriv)
                    ? Path.Combine(Path.GetTempPath(), csvFileName)
                    : Path.Combine(secureFilePriv, csvFileName);

                File.WriteAllText(csvFilePath, ConvertProgressToCsv(progressRecords), Encoding.UTF8);

                try
                {
                    if (CheckProgressTableExists(tableName))
                    {
                        DropProgressTable(tableName);
                    }
                    CreateProgressTableIfNotExists(tableName);

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
                        "Building", "Floor",
                        "PlannedStart", "PlannedEnd", "ActualStart", "ActualEnd"
                    });
                    bulkLoader.Load();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"批量写入进度数据失败：{ex.Message}", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
                finally
                {
                    if (File.Exists(csvFilePath))
                        File.Delete(csvFilePath);
                }
            }

            private string ConvertProgressToCsv(List<ProgressRecord> records)
            {
                var sb = new StringBuilder();
                foreach (var rec in records)
                {
                    var values = new List<string>
                    {
                        EscapeCsvField(rec.Building),
                        EscapeCsvField(rec.Floor),
                        rec.PlannedStart.HasValue ? rec.PlannedStart.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N",
                        rec.PlannedEnd.HasValue ? rec.PlannedEnd.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N",
                        rec.ActualStart.HasValue ? rec.ActualStart.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N",
                        rec.ActualEnd.HasValue ? rec.ActualEnd.Value.ToString("yyyy-MM-dd HH:mm:ss") : @"\N"
                    };
                    sb.AppendLine(string.Join(",", values));
                }
                return sb.ToString();
            }

            public bool CheckProgressTableExists(string tableName)
            {
                string query = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @dbName AND table_name = @tableName";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dbName", conn.Database);
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }

            private void DropProgressTable(string tableName)
            {
                string query = $"DROP TABLE IF EXISTS `{tableName}`";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            private void CreateProgressTableIfNotExists(string tableName)
            {
                string createTableQuery = $@"
                CREATE TABLE IF NOT EXISTS `{tableName}` (
                    `P_Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    `Building` VARCHAR(100),
                    `Floor` VARCHAR(50),
                    `PlannedStart` DATETIME,
                    `PlannedEnd` DATETIME,
                    `ActualStart` DATETIME,
                    `ActualEnd` DATETIME,
                    INDEX `idx_Building_Floor` (`Building`, `Floor`)
                );";
                using (MySqlCommand cmd = new MySqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
    // Material类 增加TimeLiner字段 因为外部还要用 因此放到这里创建一个类
    public class Material
    {
        public object ID { get; set; }
        public string Name { get; set; }
        public object Volume { get; set; }
        public object Area { get; set; }
        public object File { get; set; }
        public object Layer { get; set; }
        public DateTime? PlannedStart { get; set; }
        public DateTime? PlannedEnd { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
    }
}
