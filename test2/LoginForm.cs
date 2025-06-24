using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;

namespace test2
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblMsg;

        public int LoggedInUserId { get; private set; } = -1;
        public string LoggedInUserName { get; private set; } = "";

        // 数据库连接字符串
        private string connStr = "server=localhost;port=3306;user id=root;password=123456;database=ximalu;";

        public LoginForm()
        {
            InitCustomUI();
        }

        private void InitCustomUI()
        {
            this.Size = new Size(400, 250);
            this.Text = "系统登录";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblUser = new Label { Text = "账号:", Location = new Point(50, 50), AutoSize = true, Font = new Font("微软雅黑", 12) };
            txtUsername = new TextBox { Location = new Point(120, 48), Width = 200, Font = new Font("微软雅黑", 10) };

            Label lblPass = new Label { Text = "密码:", Location = new Point(50, 100), AutoSize = true, Font = new Font("微软雅黑", 12) };
            txtPassword = new TextBox { Location = new Point(120, 98), Width = 200, PasswordChar = '*', Font = new Font("微软雅黑", 10) };

            btnLogin = new Button { Text = "登 录", Location = new Point(120, 150), Width = 200, Height = 40, BackColor = Color.SteelBlue, ForeColor = Color.White, Font = new Font("微软雅黑", 12, FontStyle.Bold) };
            btnLogin.Click += BtnLogin_Click;

            lblMsg = new Label { Location = new Point(50, 20), Width = 300, ForeColor = Color.Red, TextAlign = ContentAlignment.MiddleCenter };

            this.Controls.Add(lblUser);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblMsg);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblMsg.Text = "请输入账号和密码";
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // === 修改点：我将 user_name 改为了 username，这是最可能的错误原因 ===
                    // 如果您的表里账号列叫 user_name，请手动把下面的 username 改回 user_name
                    string sql = "SELECT id, username FROM sys_user WHERE username = @u AND password = @p LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", user);
                        cmd.Parameters.AddWithValue("@p", pass);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 注意：这里读取的是第0列(id)和第1列(username)
                                LoggedInUserId = reader.GetInt32(0);
                                LoggedInUserName = reader.GetString(1);
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                lblMsg.Text = "账号或密码错误 (请确认数据库中有此账号)";
                            }
                        }
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                // === 诊断提示 ===
                // 如果是列名错误，这里会直接告诉你是哪一列错了
                if (sqlEx.Message.Contains("Unknown column"))
                {
                    MessageBox.Show("数据库列名不匹配！\n请检查代码中的 'username' 或 'password' 是否与数据库一致。\n\n错误详情: " + sqlEx.Message);
                }
                else
                {
                    lblMsg.Text = "数据库错误: " + sqlEx.Message;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "连接失败: " + ex.Message;
            }
        }
    }
}