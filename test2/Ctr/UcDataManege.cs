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
using test2;

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
        // === 新增变量 ===
        private int _currentUserId = -1;
        private int _currentProjectId = -1;

        public UcDataManege()
        {
            InitializeComponent();
            // 初始化状态：禁用所有功能，直到登录
            cbDataGet.Enabled = false;
            cbDBUpdate.Enabled = false;
            cbModelUpdate.Enabled = false;
            btBreakConnect.Enabled = false;

            // 确保更新复选框可见
            cbDBUpdate.Visible = true;
            cbModelUpdate.Visible = true;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Dock = DockStyle.Fill;
        }





        #region UI 事件处理


        // === 替换旧的连接按钮，改为新的登录逻辑 ===
        private void btLogin_MouseUp(object sender, MouseEventArgs e)
        {
            // 如果已经连接，点击则视为“注销”
            if (conn != null && conn.State == ConnectionState.Open)
            {
                CloseDatabase();
                return;
            }

            // 弹出登录框
            LoginForm login = new LoginForm();
            if (login.ShowDialog() == DialogResult.OK)
            {
                _currentUserId = login.LoggedInUserId;
                string userName = login.LoggedInUserName;

                try
                {
                    // 连接数据库 (注意：这里直接复用你之前的连接字符串逻辑)
                    string stringConnect = "server=localhost;port=3306;user id=root;password=123456;database=ximalu;Allow User Variables=True;AllowLoadLocalInfile=True;";
                    conn = new MySqlConnection(stringConnect);
                    conn.Open();

                    // 更新 UI 状态
                    btLogin.Text = "注销 / 切换账号";
                    btLogin.BackColor = System.Drawing.Color.IndianRed;
                    lblUserInfo.Text = $"欢迎您，{userName}";

                    // 激活其他功能区
                    btBreakConnect.Enabled = true;
                    cbDataGet.Enabled = true;
                    cbDBUpdate.Enabled = true;
                    cbModelUpdate.Enabled = false; // 先禁用，选了项目才启用

                    MessageBox.Show("登录成功！数据库已连接。");

                    // 加载项目列表
                    LoadUserProjects(_currentUserId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据库连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // === 新增：加载用户关联的项目 ===
        private void LoadUserProjects(int userId)
        {
            cbProjectList.Items.Clear();
            try
            {
                // 联表查询：sys_user_project -> sys_project
                string sql = @"
                    SELECT p.id, p.project_name 
                    FROM sys_project p
                    JOIN sys_user_project up ON p.id = up.project_id
                    WHERE up.user_id = @uid";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    DataTable dt = new DataTable();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        cbProjectList.DisplayMember = "project_name";
                        cbProjectList.ValueMember = "id";
                        cbProjectList.DataSource = dt;
                        cbProjectList.Enabled = true;
                        cbProjectList.SelectedIndex = 0; // 默认选中第一个
                    }
                    else
                    {
                        cbProjectList.Items.Add("该账号未绑定项目");
                        cbProjectList.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载项目列表失败: " + ex.Message);
            }
        }

        // === 新增：项目选择改变事件 ===
        private void cbProjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProjectList.SelectedValue is int pid)
            {
                _currentProjectId = pid;
                cbModelUpdate.Enabled = true; // 只有选了项目，才允许提取进度
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

                cbDataGet.Checked = false;
                progressBar.Value = 0;
                lblProgressPercentage.Text = "0%";
            }
        }


        // “更新模型数据库” (已修改：硬编码表名为 model_data)
        private async void cbDBUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbDBUpdate.Checked) return;

            if (listMaterials == null || listMaterials.Count == 0)
            {
                MessageBox.Show("没有可供保存的数据。请先“获取数据”。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbDBUpdate.Checked = false;
                return;
            }

            // === 修改点：直接锁定表名为 model_data ===
            string tableName = "model_data";

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
                // 锁定 UI
                btLogin.Enabled = false; // 暂时禁用登录按钮
                btBreakConnect.Enabled = false;
                cbDataGet.Enabled = false;
                cbModelUpdate.Enabled = false;
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
                    // 恢复 UI
                    btLogin.Enabled = true;
                    btBreakConnect.Enabled = true;
                    cbDataGet.Enabled = true;
                    cbModelUpdate.Enabled = true;
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






        // “更新进度数据库”按钮（核心）
        // “更新进度数据库”按钮（核心）
// “更新进度数据库”按钮（核心修改：支持项目ID）
        private async void cbModelUpdate_CheckedChanged(object sender, EventArgs e)
        {
            // 1. 基础检查
            if (!cbModelUpdate.Checked) return;

            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc == null || string.IsNullOrEmpty(doc.CurrentFileName))
            {
                MessageBox.Show("请先打开并保存一个Navisworks文档！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbModelUpdate.Checked = false;
                return;
            }

            // === 修改点：检查是否绑定了项目 ===
            if (_currentProjectId == -1)
            {
                MessageBox.Show("请先在上方选择归属项目！", "未选择项目", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbModelUpdate.Checked = false;
                return;
            }

            // === 修改点：直接锁定表名为 plan_progress ===
            string tableName = "plan_progress";

            // 2. 锁定 UI
            btLogin.Enabled = false;
            btBreakConnect.Enabled = false;
            cbDataGet.Enabled = false;
            cbModelUpdate.Enabled = false;
            cbDBUpdate.Enabled = false;

            progressRecords.Clear();
            tbDataManege.Clear();

            try
            {
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.MarqueeAnimationSpeed = 30;

                // 3. 执行提取
                ExtractBuildingProgressRecords();

                UpdateProgress(100, $"提取阶段完成，准备写入数据库...");

                // 4. 数据库保存
                bool shouldProceed = true; // 既然是追加或带ID管理，通常直接写入，或者你可以保留询问逻辑
                // 这里为了简单，我们还是保留询问，或者直接保存。考虑到多项目共存，其实直接保存最合适。
                // 但为了保险，还是问一下用户
                var result = MessageBox.Show($"即将向项目 (ID:{_currentProjectId}) 写入 {progressRecords.Count} 条进度数据。\n\n确认继续？", 
                                             "确认写入", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                shouldProceed = (result == DialogResult.Yes);

                if (shouldProceed)
                {
                    UpdateProgress(0, "正在批量写入数据库...");

                    await Task.Run(() =>
                    {
                        MySqlDataSaver saver = new MySqlDataSaver(conn);
                        // === 修改点：传入 _currentProjectId ===
                        saver.SaveProgressRecords(progressRecords, tableName, _currentProjectId);
                    });

                    UpdateProgress(100, $"成功！共 {progressRecords.Count} 条进度数据已保存。");
                    tbDataManege.AppendText($"======== 操作成功 ========\r\n项目ID：{_currentProjectId}\r\n记录数：{progressRecords.Count}\r\n");
                    MessageBox.Show($"进度数据已成功保存！", "操作完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    tbDataManege.AppendText("用户取消了数据库写入操作。\r\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbDataManege.AppendText($"[错误] {ex.Message}\r\n");
            }
            finally
            {
                // 5. 恢复 UI 状态
                btLogin.Enabled = true;
                btBreakConnect.Enabled = true;
                cbDataGet.Enabled = true;
                cbModelUpdate.Enabled = true;
                cbDBUpdate.Enabled = true;
                
                cbModelUpdate.Checked = false;

                progressBar.Style = ProgressBarStyle.Blocks;
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
            // 1. 跨所有同名 "元素" 分类，地毯式搜索“体积”和“面积”
            var propertyVolume = item.PropertyCategories
                .Where(c => c.DisplayName == "元素")
                .SelectMany(c => c.Properties)
                .FirstOrDefault(p => p.DisplayName == "体积");

            if (propertyVolume == null) return null;

            var propertyArea = item.PropertyCategories
                .Where(c => c.DisplayName == "元素")
                .SelectMany(c => c.Properties)
                .FirstOrDefault(p => p.DisplayName == "面积");

            if (propertyArea == null) return null;

            var volume = GetPropertyValue(propertyVolume);
            var area = GetPropertyValue(propertyArea);
            var name = item.DisplayName;

            // 2. 跨所有同名 "项目" 分类，地毯式搜索“源文件”和“层”
            var propertyFile = item.PropertyCategories
                .Where(c => c.DisplayName == "项目")
                .SelectMany(c => c.Properties)
                .FirstOrDefault(p => p.DisplayName == "源文件");
            var file = propertyFile != null ? GetPropertyValue(propertyFile) : null;

            var propertyLayer = item.PropertyCategories
                .Where(c => c.DisplayName == "项目")
                .SelectMany(c => c.Properties)
                .FirstOrDefault(p => p.DisplayName == "层");
            var layer = propertyLayer != null ? GetPropertyValue(propertyLayer) : null;

            // 3. 获取 ID
            var propertyId = item.PropertyCategories
                .Where(c => c.DisplayName == "元素")
                .SelectMany(c => c.Properties)
                .FirstOrDefault(p => p.DisplayName == "Id");
            var id = propertyId != null ? GetPropertyValue(propertyId) : null;

            // 4. 获取 TimeLiner 数据
            DateTime? plannedStart = null, plannedEnd = null, actualStart = null, actualEnd = null;

            // TimeLiner通常只有一个，但为了严谨也用 SelectMany 展开查
            var timeLinerProps = item.PropertyCategories
                .Where(c => c.DisplayName == "TimeLiner")
                .SelectMany(c => c.Properties).ToList();

            if (timeLinerProps.Any())
            {
                foreach (var prop in timeLinerProps)
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
            tbDataManege.AppendText("======= 警告：新代码已成功加载并运行 =======\r\n");

            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            var rootItems = doc.Models.Select(m => m.RootItem);

            var picked = new HashSet<string>();
            int processedCount = 0, storedCount = 0;

            // 1. 进度条策略调整：不预估总数，只显示当前状态
            // 将进度条样式设为连续/不确定模式，防止用户看着卡住
            if (progressBar.Style != ProgressBarStyle.Marquee)
            {
                // 注意：由于在非UI线程，这里简单处理，后面UpdateProgress会更新文本
            }

            UpdateProgress(0, "⚡ 极速提取引擎已启动...");

            Stack<ModelItem> stack = new Stack<ModelItem>(rootItems);

            while (stack.Count > 0)
            {
                ModelItem item = stack.Pop();
                processedCount++;

                // 获取原始属性用于判断
                string rawFile = GetProperty(item, "项目", "源文件")?.ToString() ?? "";
                string rawLayer = GetProperty(item, "项目", "层")?.ToString() ?? "";

                // 2. AR 文件过滤 (排除包含 "AR" 且不包含 "ST" 的文件，防止误杀结构)
                // 这里的逻辑是：如果文件名里明确写了AR，通常是建筑模型，跳过。
                if (rawFile.IndexOf("AR", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }

                // 老版代码、提取二十分钟
                //// 3. 宽松正则匹配
                //string building = ExtractBuilding(rawFile);
                //string floor = ExtractFloor(rawLayer);

                //// ================== 终极探针开始 ==================
                //// 只要这个构件身上带有“层”属性，不管它叫什么名字，全部抓出来暴露在阳光下！
                //bool hasLayer = false;
                //foreach (var category in item.PropertyCategories)
                //{
                //    if (category.DisplayName == "项目" && category.Properties.FindPropertyByDisplayName("层") != null)
                //    {
                //        hasLayer = true;
                //        break;
                //    }
                //}

                //if (hasLayer || (item.DisplayName != null && item.DisplayName.Contains("1F(-0.100)")))
                //{
                //    string debugMsg = $"【终极捕捉】节点: [{item.DisplayName ?? "NULL"}]\r\n" +
                //                      $" -> 遍历读到的 rawFile : [{GetProperty(item, "项目", "源文件")?.ToString() ?? "NULL"}]\r\n" +
                //                      $" -> 遍历读到的 rawLayer: [{GetProperty(item, "项目", "层")?.ToString() ?? "NULL"}]\r\n" +
                //                      $"--------------------------------------------------\r\n";
                //    if (this.InvokeRequired) this.Invoke(new Action(() => tbDataManege.AppendText(debugMsg)));
                //    else tbDataManege.AppendText(debugMsg);
                //}
                //// ================== 终极探针结束 ==================


                //// 有效节点判断：必须同时具备“楼栋”和“楼层”信息
                //if (!string.IsNullOrEmpty(building) && !string.IsNullOrEmpty(floor))
                //{
                //    string key = $"{building}-{floor}";

                //    // 如果这个楼层还没提取过
                //    if (!picked.Contains(key))
                //    {
                //        // ================= 核心优化：智能探针 =================
                //        // 问题：TimeLiner 属性可能挂在当前节点(Group)，也可能挂在子节点(Geometry)
                //        // 策略：先查自己，没有则查后代中的第一个有数据的节点

                //        ModelItem targetItem = item;

                //        // 检查自己有没有 TimeLiner
                //        if (targetItem.PropertyCategories.FindCategoryByDisplayName("TimeLiner") == null)
                //        {
                //            // 自己没有，找后代中有 TimeLiner 的第一个（广度优先可能更准，但这里用First即可）
                //            targetItem = item.Descendants.FirstOrDefault(d => d.PropertyCategories.FindCategoryByDisplayName("TimeLiner") != null);
                //        }

                //        // 如果找到了有效的载体节点（无论是自己还是子孙）
                //        if (targetItem != null)
                //        {
                //            var timeRecord = GetTimeLinerData(targetItem);

                //            // 只有当至少有一个时间有效才记录
                //            if (timeRecord.HasValidData)
                //            {
                //                picked.Add(key);
                //                progressRecords.Add(new ProgressRecord
                //                {
                //                    Building = building,
                //                    Floor = floor,
                //                    PlannedStart = timeRecord.PlannedStart,
                //                    PlannedEnd = timeRecord.PlannedEnd,
                //                    ActualStart = timeRecord.ActualStart,
                //                    ActualEnd = timeRecord.ActualEnd
                //                });
                //                storedCount++;
                //            }
                //        }
                //    }

                //    // 4. 剪枝 (Pruning)：
                //    // 既然已经到了“楼层”这一级（无论是否提取成功），都没必要再遍历其内部的成千上万个构件了
                //    // 直接跳过该节点的所有子节点
                //    continue;
                //}


                //// 5. 继续向下遍历（如果当前还不是楼层节点）
                //if (item.Children.Any())
                //{
                //    foreach (var child in item.Children) stack.Push(child);
                //}

                // 3. 宽松正则匹配
                string building = ExtractBuilding(rawFile);
                string floor = ExtractFloor(rawLayer);

                // ================ 【第一步：提取进度数据】 ================
                // 有效节点判断：必须同时具备“楼栋”和“楼层”信息，才去提取数据
                if (!string.IsNullOrEmpty(building) && !string.IsNullOrEmpty(floor))
                {
                    string key = $"{building}-{floor}";

                    // 如果这个楼层还没提取过
                    if (!picked.Contains(key))
                    {
                        ModelItem targetItem = item;

                        // 检查自己有没有 TimeLiner，没有就去底下的子节点里找
                        if (targetItem.PropertyCategories.FindCategoryByDisplayName("TimeLiner") == null)
                        {
                            targetItem = item.Descendants.FirstOrDefault(d => d.PropertyCategories.FindCategoryByDisplayName("TimeLiner") != null);
                        }

                        // 如果找到了有效的载体节点（无论是自己还是子孙）
                        if (targetItem != null)
                        {
                            var timeRecord = GetTimeLinerData(targetItem);

                            // 只有当至少有一个时间有效才记录
                            if (timeRecord.HasValidData)
                            {
                                picked.Add(key);
                                progressRecords.Add(new ProgressRecord
                                {
                                    Building = building,
                                    Floor = floor,
                                    PlannedStart = timeRecord.PlannedStart,
                                    PlannedEnd = timeRecord.PlannedEnd,
                                    ActualStart = timeRecord.ActualStart,
                                    ActualEnd = timeRecord.ActualEnd
                                });
                                storedCount++;
                            }
                        }
                    }
                }

                // ================ 【第二步：超强力独立剪枝 (解决遍历十多分钟的元凶)】 ================
                // 判断 1：只要 rawLayer 有值，说明它在 Navisworks 里被识别为一个“层”（比如“地库”、“1F”）。
                // 判断 2：或者它身上带有“元素”属性，说明它已经是墙、柱、板等实体物理构件了，下面全是几何三角面。
                // 只要符合以上任意一点，直接 continue 剪枝！绝对不再往下深究它的子节点！
                // 关键修复：使用正则提取出的纯净 floor，彻底避开"元素属性内没有ID项"的干扰！
                bool isFloorNode = !string.IsNullOrEmpty(floor);
                bool isElement = item.PropertyCategories.FindCategoryByDisplayName("元素") != null;

                if (isFloorNode || isElement)
                {
                    continue; // 核心剪枝：一刀切断后续的几十万次无用遍历！
                }

                // ================ 【第三步：继续向下遍历】 ================
                // 只有上面没被拦截的（比如总项目节点、楼栋分组节点），才允许把子节点压入栈继续找
                if (item.Children.Any())
                {
                    foreach (var child in item.Children) stack.Push(child);
                }

                // 6. UI反馈：只更新计数，不计算百分比
                if (processedCount % 200 == 0) // 频率稍微调高点，因为现在遍历速度很快
                    
                {
                    UpdateProgress(0, $"⚡ 极速扫描中... 已采集: {storedCount} 条 | 已扫描节点: {processedCount}");
                }
            }

            // 最后强制刷新一下
            UpdateProgress(100, $"提取完成！共采集 {storedCount} 条进度数据。");
        }

        // 辅助类：用于传递时间提取结果
        private class TimeData
        {
            public DateTime? PlannedStart;
            public DateTime? PlannedEnd;
            public DateTime? ActualStart;
            public DateTime? ActualEnd;
            public bool HasValidData => PlannedStart.HasValue || PlannedEnd.HasValue || ActualStart.HasValue || ActualEnd.HasValue;
        }

        // 封装时间提取逻辑
        private TimeData GetTimeLinerData(ModelItem item)
        {
            var data = new TimeData();
            var cat = item.PropertyCategories.FindCategoryByDisplayName("TimeLiner");
            if (cat == null) return data;

            foreach (var prop in cat.Properties)
            {
                string propName = prop.DisplayName;
                string rawValue = GetPropertyValue(prop)?.ToString();

                // 智能时间解析
                DateTime? parsedTime = null;
                if (!string.IsNullOrEmpty(rawValue))
                {
                    if (DateTime.TryParse(rawValue, out DateTime dt1)) parsedTime = dt1;
                    else
                    {
                        int idx = rawValue.IndexOf(':');
                        if (idx >= 0 && rawValue.Length > idx + 1)
                        {
                            string cutStr = rawValue.Substring(idx + 1).Trim();
                            if (DateTime.TryParse(cutStr, out DateTime dt2)) parsedTime = dt2;
                        }
                    }
                }

                if (propName.Contains("任务起点 (计划)")) data.PlannedStart = parsedTime;
                else if (propName.Contains("任务终点 (计划)")) data.PlannedEnd = parsedTime;
                else if (propName.Contains("任务起点 (实际)")) data.ActualStart = parsedTime;
                else if (propName.Contains("任务终点 (实际)")) data.ActualEnd = parsedTime;
            }
            return data;
        }

        // 更新后的宽松正则：楼栋
        private string ExtractBuilding(string fileStr)
        {
            if (string.IsNullOrEmpty(fileStr)) return null;

            // 1. 预处理：只取文件名，去掉路径
            int lastSlash = Math.Max(fileStr.LastIndexOf('/'), fileStr.LastIndexOf('\\'));
            if (lastSlash >= 0) fileStr = fileStr.Substring(lastSlash + 1);

            // 2. 正则匹配：支持 "13#"、"13号"、"13号楼"
            // \d+ 匹配数字
            // (?: ... ) 是非捕获分组
            // [#号] 匹配 # 或 号
            // 楼? 匹配可选的“楼”字
            var match = System.Text.RegularExpressions.Regex.Match(fileStr, @"(\d+)(?:#|号楼|号)");

            if (match.Success)
                return match.Groups[1].Value; // 返回数字部分

            return null;
        }

        // 更新后的宽松正则：楼层
        private string ExtractFloor(string layerStr)
        {
            if (string.IsNullOrEmpty(layerStr)) return null;

            // 正则匹配：支持 "1F"、"1层"
            // 不强制开头或结尾，只要包含即可
            var match = System.Text.RegularExpressions.Regex.Match(layerStr, @"(\d+)(?:F|层)");

            if (match.Success)
                return match.Groups[1].Value; // 返回数字部分

            return null;
        }

        private object GetProperty(ModelItem item, string category, string prop)
        {
            var cat = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == category);
            if (cat == null) return null;
            return GetPropertyValue(cat.Properties.FindPropertyByDisplayName(prop));
        }




        #endregion


        #region 其它方法...
        public void CloseDatabase()
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Dispose();
                conn = null;

                // 恢复 UI 到未登录状态
                btLogin.Text = "点击登录系统";
                btLogin.BackColor = System.Drawing.Color.SteelBlue;
                lblUserInfo.Text = "状态：未登录";

                cbProjectList.DataSource = null;
                cbProjectList.Items.Clear();
                cbProjectList.Enabled = false;

                btBreakConnect.Enabled = false;
                cbDataGet.Enabled = false;
                cbDBUpdate.Enabled = false;
                cbModelUpdate.Enabled = false;

                _currentUserId = -1;
                _currentProjectId = -1;

                MessageBox.Show("已注销并断开连接！");
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

                // 🟢 1. 直接使用系统临时目录，不再询问数据库
                string csvFileName = $"import_{Guid.NewGuid():N}.csv";
                string csvFilePath = Path.Combine(Path.GetTempPath(), csvFileName);

                // 生成 CSV 文件
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
                        NumberOfLinesToSkip = 0,

                        // 🟢 2. 关键修改：开启本地上传模式
                        Local = true
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
                    // 清理临时文件
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

            // === 修改点：增加 projectId 参数 ===
            public void SaveProgressRecords(List<ProgressRecord> progressRecords, string tableName, int projectId)
            {
                if (progressRecords == null || progressRecords.Count == 0) return;

                string csvFileName = $"progress_{Guid.NewGuid():N}.csv";
                string csvFilePath = Path.Combine(Path.GetTempPath(), csvFileName);

                // 传入 projectId 生成 CSV
                File.WriteAllText(csvFilePath, ConvertProgressToCsv(progressRecords, projectId), Encoding.UTF8);

                try
                {
                    // 注意：这里不再自动创建表，因为表结构比较复杂且已经存在，
                    // 如果表不存在，抛出异常或提示用户运行SQL脚本更好。
                    // 但为了兼容原有逻辑，保留 Check，如果不存在则尝试创建（但创建逻辑里需要加上project_id字段才行）
                    // 建议：默认表已存在（因为你已经运行过SQL了）

                    if (!CheckProgressTableExists(tableName))
                    {
                        // 如果表真的不存在，这里可以抛个错提示用户去建表
                        throw new Exception($"表 {tableName} 不存在，请联系管理员执行数据库初始化脚本！");
                    }

                    // === 可选：清理当前项目的旧数据 ===
                    // string deleteSql = $"DELETE FROM `{tableName}` WHERE project_id = {projectId}";
                    // using (MySqlCommand cmd = new MySqlCommand(deleteSql, conn)) { cmd.ExecuteNonQuery(); }

                    var bulkLoader = new MySqlBulkLoader(conn)
                    {
                        TableName = tableName,
                        FileName = csvFilePath,
                        FieldTerminator = ",",
                        LineTerminator = "\r\n",
                        CharacterSet = "utf8",
                        NumberOfLinesToSkip = 0,
                        Local = true
                    };

                    // === 修改点：映射列包含 project_id ===
                    bulkLoader.Columns.AddRange(new[] {
                        "project_id", "Building", "Floor",
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
                    if (File.Exists(csvFilePath)) File.Delete(csvFilePath);
                }
            }

            // === 修改点：增加 projectId 并写入 CSV 第一列 ===
            private string ConvertProgressToCsv(List<ProgressRecord> records, int projectId)
            {
                var sb = new StringBuilder();
                foreach (var rec in records)
                {
                    var values = new List<string>
                    {
                        projectId.ToString(), // 第一列：项目ID
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
                // === 修改：增加了 project_id 字段定义 ===
                string createTableQuery = $@"
                CREATE TABLE IF NOT EXISTS `{tableName}` (
                    `P_Id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    `project_id` INT NOT NULL COMMENT '项目ID', 
                    `Building` VARCHAR(100),
                    `Floor` VARCHAR(50),
                    `PlannedStart` DATETIME,
                    `PlannedEnd` DATETIME,
                    `ActualStart` DATETIME,
                    `ActualEnd` DATETIME,
                    INDEX `idx_Building_Floor` (`Building`, `Floor`),
                    INDEX `idx_project_id` (`project_id`)
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
