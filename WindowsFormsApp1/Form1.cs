using System;
using System.Diagnostics;
using Microsoft.Win32;
using static test2.Ctr.UcProperties;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using test2.Ctr;
using test2;
using Autodesk.Navisworks.Api.Automation;
using Autodesk.Navisworks.Api.Controls;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // 存储材质信息的列表
        public List<Material> listMaterials = new List<Material>();
        // 数据库连接对象
        public MySqlConnection conn = null;

        // Windows API 函数声明
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(IntPtr lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr hWnd);
        // 窗口显示命令
        const int SW_RESTORE = 9;  // 恢复窗口



        public Form1()
        {
            InitializeComponent();
            btStartNavis.Enabled = false;
            cbBreak.Enabled = false;
            cbData.Enabled = false;
            cbModel.Enabled = false;
            // 使窗体程序可见
            this.Visible = true;
        }

        // 重新生成类库项目并启动 Navisworks 的方法
        private void btStartNavis_MouseUp(object sender, MouseEventArgs e)
        {
            // 获取当前应用程序的启动路径
            string solutionRoot = System.Windows.Forms.Application.StartupPath;  // 当前程序的启动目录
            string projectRelativePath = @"E:\C#\test2 - 新\test2\test2\test2.csproj"; // 你的项目路径
            string msBuildRelativePath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"; // MSBuild 的路径

            // 获取绝对路径
            string projectPath = projectRelativePath;
            string msBuildPath =  msBuildRelativePath;

            // 尝试通过 MSBuild 重新生成类库项目
            try
            {
                // 构建命令
                string arguments = $"\"{projectPath}\" /p:Configuration=Release";

                // 启动 MSBuild 进行构建
                Process process = new Process();
                process.StartInfo.FileName = msBuildPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;  // 隐藏命令行窗口
                process.Start();
                process.WaitForExit();  // 等待构建完成
                MessageBox.Show("test2 类库项目已重新生成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成失败: " + ex.Message);
            }


            // 尝试通过注册表获取 Navisworks 安装路径并打开 Navisworks
            string navisworksPath = GetNavisworksInstallationPath();
            // 如果成功获取到路径，则尝试启动 Navisworks(如果已启动则移至前台)
            if (!string.IsNullOrEmpty(navisworksPath))
            {
                var existingProcesses = Process.GetProcessesByName("Roamer");
                try
                {
                    if (existingProcesses.Length > 0)
                    {
                        MessageBox.Show("Navisworks 已经在运行！");
                        // 如果已打开，跳转到已打开的Navisworks窗口
                        IntPtr hWnd = FindWindow(IntPtr.Zero, existingProcesses[0].MainWindowTitle);
                        if (hWnd != IntPtr.Zero)
                        {
                            // 尝试恢复窗口（如果最小化的话）
                            ShowWindow(hWnd, SW_RESTORE);
                            // 尝试将窗口移到最前面
                            BringWindowToTop(hWnd);
                            SetForegroundWindow(hWnd); // 确保窗口处于前台
                        }
                        return;
                    }
                    // 启动 Navisworks
                    Process.Start(navisworksPath);
                    MessageBox.Show("Navisworks 已启动！");
                }
                catch (Exception ex) { MessageBox.Show("启动 Navisworks 时出错: " + ex.Message); }
            }
            // 如果没有找到路径，则提示用户
            else { MessageBox.Show("未找到 Navisworks 安装路径！"); }
        }


        // 获取 Navisworks 安装路径的方法
        private string GetNavisworksInstallationPath()
        {
            string navisworksPath = string.Empty;

            try
            {
                // 版本号列表，后期根据顾客的实际情况进行添加
                string[] versionPaths = new string[]
                {
                    "19.0", "20.0","21.0", "2018", "2019","2020", "2022", "2023", "2024"   
                };

                foreach (var version in versionPaths)
                {
                    // 
                    string regKeyPath = $@"SOFTWARE\Autodesk\Navisworks Manage\{version}\Location";

                    // 
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regKeyPath))
                    {
                        if (key != null)
                        {
                            // 
                            string installPath = key.GetValue("Path") as string;
                            if (!string.IsNullOrEmpty(installPath))
                            {
                                // 
                                navisworksPath = System.IO.Path.Combine(installPath, "Roamer.exe");
                                break;  // 
                            }
                        }
                    }
                }

                // 如果在以上路径中没有找到，尝试检查 HKEY_CURRENT_USER 注册表
                if (string.IsNullOrEmpty(navisworksPath))
                {
                    foreach (var version in versionPaths)
                    {
                        string regKeyPath = $@"SOFTWARE\WOW6432Node\Autodesk\Navisworks Manage\{version}\Location";

                        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(regKeyPath))
                        {
                            if (key != null)
                            {
                                string installPath = key.GetValue("Path") as string;
                                if (!string.IsNullOrEmpty(installPath))
                                {
                                    navisworksPath = System.IO.Path.Combine(installPath, "Roamer.exe");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("读取注册表时发生错误: " + ex.Message); }
            return navisworksPath;
        }


        private void Bianli(ModelItem Item)
        {
            if (!Item.Children.Any())  // 如果没有子元素了（表示已经是有数据的最小构件），就获取数据
            {
                getDatas(Item);
            }
            else
            {
                foreach (var item in Item.Children)
                {
                    Bianli(item);
                }
            }
            MessageBox.Show("所有数据获取完毕！");
        }

        private void getDatas(ModelItem item)
        {
            // 如果没有元素属性，则返回
            var categoryElement = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "元素");
            if (categoryElement == null) { return; }
            // 如果没有体积属性，则返回
            var PropertyVolume = categoryElement.Properties.FirstOrDefault(c => c.DisplayName == "体积");
            if (PropertyVolume == null) { return; }
            // 如果没有面积属性，则返回
            var PropertyArea = categoryElement.Properties.FirstOrDefault(c => c.DisplayName == "面积");
            if (PropertyArea == null) { return; }
            var volume = UcProperties.GetPropertyValue(PropertyVolume);            // 获取体积属性的值
            var area = UcProperties.GetPropertyValue(PropertyArea);            // 获取面积值
            var name = item.DisplayName;            // 获取名称
            var categoryItem = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "项目");
            var PropertyFile = categoryItem.Properties.FindPropertyByDisplayName("源文件");            // 获取源文件值
            var file = UcProperties.GetPropertyValue(PropertyFile);            // 获取层值
            var PropertyLayer = categoryItem.Properties.FindPropertyByDisplayName("层");
            var layer = UcProperties.GetPropertyValue(PropertyLayer);
            var PropertyId = categoryElement.Properties.FindPropertyByDisplayName("Id");          // 获取ID值
            var id = UcProperties.GetPropertyValue(PropertyId);

            Material material = new Material();
            material.Name = name;
            material.ID = id;
            material.Volume = volume;
            material.Area = area;
            material.File = file;
            material.Layer = layer;
            UpdateMaterialList(material);
        }

        private void UpdateMaterialList(Material newMaterial)
        {
            var existingMaterial = listMaterials.FirstOrDefault(m => m.ID == newMaterial.ID);
            if (existingMaterial != null)
            {
                // 如果已经存在且属性有变化，更新原对象
                if (existingMaterial.Name != newMaterial.Name ||
                    existingMaterial.Volume != newMaterial.Volume ||
                    existingMaterial.Area != newMaterial.Area ||
                    existingMaterial.File != newMaterial.File ||
                    existingMaterial.Layer != newMaterial.Layer)
                {
                    existingMaterial.Name = newMaterial.Name;
                    existingMaterial.Volume = newMaterial.Volume;
                    existingMaterial.Area = newMaterial.Area;
                    existingMaterial.File = newMaterial.File;
                    existingMaterial.Layer = newMaterial.Layer;
                    SaveData(newMaterial);
                }
            }
            else
            {
                // 如果不存在该 Material，添加到列表
                listMaterials.Add(newMaterial);
                // 保存新数据到数据库
                SaveData(newMaterial);
            }
        }

        private void SaveData(Material newMaterial)
        {
            if (conn == null || conn.State != ConnectionState.Open)
            {
                MessageBox.Show("请先连接到数据库！");
                return;
            }
            MySqlDataSaver dataSaver = new MySqlDataSaver(conn);
            dataSaver.SaveMaterials(listMaterials);

        }

        private void cbConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConnect.Checked && cbConnect.Enabled)
            {
                conn = new MySqlConnection("server=localhost;port=3306;user id=root;password=123456;database=ximalu;");
                conn.Open();
                btStartNavis.Enabled = true;
                cbConnect.Enabled = false;
                cbBreak.Enabled = true;
                cbData.Enabled = true;
                cbModel.Enabled = true;
                MessageBox.Show("数据库连接成功！");
            }
        }

        private void cbBreak_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBreak.Checked && cbBreak.Enabled)
            {
                conn.Close();
                btStartNavis.Enabled = false;
                cbBreak.Enabled = false;
                cbBreak.Checked = false;
                cbConnect.Enabled = true;
                cbConnect.Checked = false;
                cbData.Enabled = false;
                cbModel.Enabled = false;
                MessageBox.Show("数据库连接已断开！");
            }
        }

        private void cbData_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("获取数据中...");
            // 如果Navisworks进程存在且勾选了获取数据，则遍历模型获取数据
            if (Process.GetProcessesByName("Roamer").Length > 0 && cbData.Checked && cbData.Enabled)
            {
                MessageBox.Show("获取数据中...");
                var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                if (doc == null)
                { 
                    MessageBox.Show("请先打开文档！");
                    return;
                }

                cbData.Enabled = false;  // 遍历模型获取数据时，禁用获取数据按钮，防止遍历到一半时被取消
                foreach (var model in doc.Models)
                {
                    Bianli(model.RootItem);
                }
                cbData.Enabled = true; // 遍历完成后，重新启用获取数据按钮
            }
            else if (Process.GetProcessesByName("Roamer").Length <= 0)
            {
                MessageBox.Show("请先打开软件！");
            }
            else
            {
                MessageBox.Show("请先打开文档！");
            }

        }




        public class MySqlDataSaver
        {
            private MySqlConnection conn;

            public MySqlDataSaver(MySqlConnection conn)
            {
                this.conn = conn;
            }


            // 检查表格是否存在，如果不存在则创建
            private void CreateTableIfNotExists()
            {
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Data (
                Name VARCHAR(255),
                ID VARCHAR(255) PRIMARY KEY,
                Volume DECIMAL(18, 2),
                Area DECIMAL(18, 2),
                File VARCHAR(255),
                Layer VARCHAR(255)
            );
        ";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(createTableQuery, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating table: {ex.Message}");
                }
            }

            // 保存或更新数据
            public void SaveMaterials(List<Material> listMaterials)
            {
                CreateTableIfNotExists();  // 确保表格存在

                foreach (var material in listMaterials)
                {
                    // 检查该 ID 是否已存在
                    bool exists = CheckIfExists((string)material.ID);

                    if (exists)
                    {
                        // 如果该 ID 存在，更新对应的记录
                        UpdateMaterial(material);
                    }
                    else
                    {
                        // 如果该 ID 不存在，插入新记录
                        InsertMaterial(material);
                    }
                }
            }

            // 检查数据库中是否已经存在某个 ID
            private bool CheckIfExists(string materialId)
            {
                string query = "SELECT COUNT(*) FROM Data WHERE ID = @ID";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", materialId);
                    long count = Convert.ToInt64(cmd.ExecuteScalar());

                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error checking if exists: {ex.Message}");
                    return false;
                }
            }

            // 插入新的 Material 数据
            private void InsertMaterial(Material material)
            {
                string query = @"
            INSERT INTO Data (Name, ID, Volume, Area, File, Layer) 
            VALUES (@Name, @ID, @Volume, @Area, @File, @Layer);
        ";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", material.Name);
                    cmd.Parameters.AddWithValue("@ID", material.ID);
                    cmd.Parameters.AddWithValue("@Volume", material.Volume);
                    cmd.Parameters.AddWithValue("@Area", material.Area);
                    cmd.Parameters.AddWithValue("@File", material.File);
                    cmd.Parameters.AddWithValue("@Layer", material.Layer);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting data: {ex.Message}");
                }
            }

            // 更新已有的 Material 数据
            private void UpdateMaterial(Material material)
            {
                string query = @"
            UPDATE Data 
            SET Name = @Name, Volume = @Volume, Area = @Area, 
                File = @File, Layer = @Layer 
            WHERE ID = @ID;
        ";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", material.Name);
                    cmd.Parameters.AddWithValue("@ID", material.ID);
                    cmd.Parameters.AddWithValue("@Volume", material.Volume);
                    cmd.Parameters.AddWithValue("@Area", material.Area);
                    cmd.Parameters.AddWithValue("@File", material.File);
                    cmd.Parameters.AddWithValue("@Layer", material.Layer);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating data: {ex.Message}");
                }
            }
        }



    }
}
