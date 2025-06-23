using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using MySql.Data.MySqlClient;
using static test2.Ctr.UcProperties;
using test2.Ctr;
using System.Runtime.InteropServices;
using System.Diagnostics;

// 开发版v1.0
// 0623 21.21

namespace test2.Ctr
{

    public partial class UcDataManege : UserControl
    {
        public List<Material> listMaterials = new List<Material>();

        // 数据库连接对象
        public MySqlConnection conn = null;
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Dock = DockStyle.Fill;
        }

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


        public UcDataManege()
        {
            InitializeComponent();
            cbDataGet.Enabled = false;
            cbDBUpdate.Enabled = false;
            cbModelUpdate.Enabled = false;
            btBreakConnect.Enabled = false;
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
        }


        private int count = 0;
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

            // 获取体积属性的值
            var volume = UcProperties.GetPropertyValue(PropertyVolume);
            // 获取面积值
            var area = UcProperties.GetPropertyValue(PropertyArea);
            // 获取名称
            var name = item.DisplayName;
            var categoryItem = item.PropertyCategories.FirstOrDefault(c => c.DisplayName == "项目");
            // 获取源文件值
            var PropertyFile = categoryItem.Properties.FindPropertyByDisplayName("源文件");
            var file = UcProperties.GetPropertyValue(PropertyFile);
            // 获取层值
            var PropertyLayer = categoryItem.Properties.FindPropertyByDisplayName("层");
            var layer = UcProperties.GetPropertyValue(PropertyLayer);
            // 获取ID值
            var PropertyId = categoryElement.Properties.FindPropertyByDisplayName("Id");
            var id = UcProperties.GetPropertyValue(PropertyId);

            Material material = new Material
            {
                Name = name,
                ID = id,
                Volume = volume,
                Area = area,
                File = file,
                Layer = layer
            };
            UpdateMaterialList(material);
        }

        private void UpdateMaterialList(Material newMaterial)
        {
            listMaterials.Add(newMaterial);
            count =listMaterials.Count;
            tbDataManege.Text = "已获取构件数：  " + count.ToString() + "  个";
        }


        // 连接数据库
        private void btConnect_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                string stringConnect = "server=localhost;port =3306;user id=root;password=123456;database=ximalu;";
                conn = new MySqlConnection(stringConnect);
                conn.Open();
                btBreakConnect.Enabled = true;
                btConnect.Enabled = false;
                cbDataGet.Enabled = true;
                // 连接数据库以后可以让用户更新模型：
                cbModelUpdate.Enabled = true;

                MessageBox.Show("数据库连接成功！");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"error：{ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // 捕获其他类型的异常
                MessageBox.Show($"error：{ex.Message}", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // 断开数据库连接
        private void btBreakConnect_MouseUp(object sender, MouseEventArgs e)
        {
            CloseDatabase();
        }
        // 封装关闭数据库连接的方法 方便后面在主插件程序被关闭时自动调用关闭数据库连接
        public void CloseDatabase()
        {
            conn.Dispose();
            btConnect.Enabled = true;
            btBreakConnect.Enabled = false;
            cbDataGet.Enabled = false;
            cbDBUpdate.Enabled = false;
            cbModelUpdate.Enabled = false;
            MessageBox.Show("数据库连接已断开！");
        }

        private void cbDataGet_CheckedChanged(object sender, EventArgs e)
        {
            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc == null || doc.CurrentFileName == "")
            {
                cbDataGet.Enabled = false;
                MessageBox.Show("请先打开文档或保存文档！");
                cbDataGet.Enabled = true;
                cbDataGet.Checked = false;
                return;
            }
            else if (cbDataGet.Checked && cbDataGet.Enabled && Process.GetProcessesByName("Roamer").Length > 0)
            {
                // 每次获取数据前，先清空列表
                listMaterials.Clear();

                cbDataGet.Enabled = false;
                cbDBUpdate.Enabled = false;
                cbModelUpdate.Enabled = false;
                tbDataManege.Enabled = true;
                MessageBox.Show("获取数据中...");


                // 确保对 Navisworks API 对象的访问在主线程中

                foreach (var model in doc.Models)
                {
                    Bianli(model.RootItem);

                    // 每处理完一个模型后，调用 DoEvents 让 UI 线程有机会处理消息，防止死锁
                    System.Windows.Forms.Application.DoEvents();
                }
                MessageBox.Show("获取最新数据完成！");
                cbDataGet.Enabled = true;
                cbDataGet.Checked = false;
                cbModelUpdate.Enabled = true;
                // 获取数据后可以让用户更新数据库：
                cbDBUpdate.Enabled = true;

            }
        } 


        // 和数据库更新数据
        private void cbDBUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDBUpdate.Checked && cbDBUpdate.Enabled && Process.GetProcessesByName("Roamer").Length > 0)
            {
                cbDataGet.Enabled = false;  // 更新数据库时禁用获取数据
                cbDBUpdate.Enabled = false;
                cbModelUpdate.Enabled = false;
                btBreakConnect.Enabled = false;  // 更新数据库时禁用断开数据库连接
                SaveData(listMaterials);
                btBreakConnect.Enabled = true;  // 更新数据库完成后启用断开数据库连接
                cbDBUpdate.Enabled = true;
                cbModelUpdate.Enabled = true;
                cbDBUpdate.Checked = false;
                cbDataGet.Enabled = true;
                MessageBox.Show("数据已更新！");
            }
        }

        // 保存数据到数据库
        private void SaveData(List<Material> listMaterials)
        {
            if (conn == null || conn.State != ConnectionState.Open)
            {
                MessageBox.Show("请先连接到数据库！");
                return;
            }
            MySqlDataSaver dataSaver = new MySqlDataSaver(conn);
            dataSaver.SaveMaterials(listMaterials);
        }

    }

    public class MySqlDataSaver
    {
        private string tableName1 = "Data8";

        private MySqlConnection conn;

        public MySqlDataSaver(MySqlConnection conn)
        {
            this.conn = conn;
        }

        // 保存或更新数据
        public void SaveMaterials(List<Material> listMaterials)
        {
            CreateTableIfNotExists();  // 确保表格存在

            HashSet<string> existingIds = GetExistingIdsFromDatabase();  // 获取已有ID集合
            HashSet<string> existingAreas = GetExistingAreasFromDatabase();  // 获取已有Area集合

            foreach (var material in listMaterials)
            {
                if (existingIds.Contains((string)material.ID))
                {
                    if (existingAreas.Contains((string)material.Area))
                    {
                        UpdateMaterial(material); // 如果ID存在，更新
                        // 每处理完一个模型后，调用 DoEvents 让 UI 线程有机会处理消息，防止死锁
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                else
                {
                    InsertMaterial(material); // 如果ID不存在，插入
                    // 每处理完一个模型后，调用 DoEvents 让 UI 线程有机会处理消息，防止死锁
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        private HashSet<string> GetExistingIdsFromDatabase()
        {
            HashSet<string> existingIds = new HashSet<string>();
            string query = $"SELECT ID,Area FROM {tableName1}";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                existingIds.Add(reader["ID"].ToString());
            }

            reader.Close();
            return existingIds;
        }

        private HashSet<string> GetExistingAreasFromDatabase()
        {
            HashSet<string> existingAreas = new HashSet<string>();
            string query = $"SELECT ID,Area FROM {tableName1}";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                existingAreas.Add(reader["Area"].ToString());
            }

            reader.Close();
            return existingAreas;
        }

        // 检查表格是否存在，如果不存在则创建；如果存在则检查并添加缺失的列

        private void CreateTableIfNotExists()
        {
            string createTableQuery = $@"
            CREATE TABLE IF NOT EXISTS {tableName1} (
                " + GetColumnsDefinition() + @"
            );
        ";

            try
            {
                MySqlCommand cmd = new MySqlCommand(createTableQuery, conn);
                cmd.ExecuteNonQuery();

                // 检查并添加缺失的列
                CheckAndAddMissingColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating table: {ex.Message}");
            }
        }

        // 获取列定义
        private string GetColumnsDefinition()
        {
            var properties = typeof(Material).GetProperties();
            var columns = new List<string>();

            foreach (var prop in properties)
            {
                string columnType = GetColumnType(prop.Name);
                string columnDefinition = $"{prop.Name} {columnType}";
                columns.Add(columnDefinition);
            }

            return string.Join(", ", columns);
        }

        // 检查并添加缺失的列
        private void CheckAndAddMissingColumns()
        {
            var properties = typeof(Material).GetProperties();

            foreach (var prop in properties)
            {
                if (!ColumnExists(prop.Name))
                {
                    AddColumn(prop.Name);
                }
            }
        }

        // 检查列是否存在
        private bool ColumnExists(string columnName)
        {
            string query = $"SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_NAME = '{tableName1}' AND COLUMN_NAME = '{columnName}'";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                long count = Convert.ToInt64(cmd.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking if column exists: {ex.Message}");
                return false;
            }
        }

        // 添加缺失的列
        private void AddColumn(string columnName)
        {
            string columnType = GetColumnType(columnName);
            string query = $"ALTER TABLE {tableName1} ADD COLUMN {columnName} {columnType}";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding column: {ex.Message}");
            }
        }

        // 根据列名获取列类型
        private string GetColumnType(string columnName)
        {
            var prop = typeof(Material).GetProperty(columnName);
            if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double) || prop.PropertyType == typeof(float))
            {
                return "DECIMAL(18, 2)";
            }
            return "VARCHAR(255)";
        }


        // 插入新的 Material 数据
        private void InsertMaterial(Material material)
        {
            var properties = typeof(Material).GetProperties();
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            string query = $"INSERT INTO {tableName1} ({columns}) VALUES ({values});";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                foreach (var prop in properties)
                {
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(material));
                }
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
            var properties = typeof(Material).GetProperties();
            // 排除 ID 属性（只设置了ID重置？）
            var setClause = string.Join(", ", properties.Where(p => p.Name != "ID").Select(p => $"{p.Name} = @{p.Name}"));

            string query = $"UPDATE {tableName1} SET {setClause} WHERE ID = @ID;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                foreach (var prop in properties)
                {
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(material));
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating data: {ex.Message}");
            }
        }
    }

    public class Material
    {
        public string Name { get; set; }
        public object Volume { get; set; }
        public object Area { get; set; }
        public object ID { get; set; }
        public object File { get; set; }
        public object Layer { get; set; }
    }
}
