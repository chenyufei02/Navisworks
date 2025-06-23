using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Navisworks.Api.Plugins;
using test2.Ctr;

namespace test2
{
    [Plugin("test2", "YUFEI", DisplayName = "AddinRibbon")]
    [RibbonLayout("AddinRibbon.xaml")]
    [RibbonTab("ID_CustomTab_1", DisplayName = "西马路项目")]
    [Command("ID_Button_1", Icon = "1_16.png", LargeIcon = "1_32.png", ToolTip = "Show a message")]
    public class CIAddin : CommandHandlerPlugin
    {
        public override int ExecuteCommand(string name, params string[] parameters)
        {
            switch (name)
            {
                case "ID_Button_1":
                    if (!Autodesk.Navisworks.Api.Application.IsAutomated)
                    {
                        var pluginRecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("ClDockPanelUpdate.YUFEI");
                        if (pluginRecord is DockPanePluginRecord && pluginRecord.IsEnabled)
                        {
                            var docPanel = (DockPanePlugin)(pluginRecord.LoadedPlugin ?? pluginRecord.LoadPlugin());
                            docPanel.ActivatePane();
                        }

                    }
                    MessageBox.Show("自定义插件已加载！");
                    break;
            }
            return 0;
        }
    }
}

namespace AddinDockPanel
{
    [Plugin("ClDockPanelUpdate", "YUFEI", DisplayName = "ClDockPanelUpdate")]
    [DockPanePlugin(200, 400, AutoScroll = true, MinimumHeight = 100, MinimumWidth = 200)]
    public class ClDockPanelUpdate : DockPanePlugin
    {
        private UcDataManege _ucDataManege;

        public override Control CreateControlPane()
        {
            var tc = new TabControl();
            tc.ParentChanged += SetDockStyle;

            var tp1 = new TabPage("动态更新");
            tp1.Controls.Add(new UcUpdate());
            tc.TabPages.Add(tp1);

            var tp2 = new TabPage("数据查询");
            tp2.Controls.Add(new UcProperties());
            tc.TabPages.Add(tp2);

            var tp3 = new TabPage("数据库管理");

            // **修正**: 创建实例并赋值给字段
            _ucDataManege = new UcDataManege();
            tp3.Controls.Add(_ucDataManege);
            tc.TabPages.Add(tp3);

            return tc;
        }

        private void SetDockStyle(object sender, EventArgs e)
        {
            try
            {
                var tc = sender as TabControl;
                tc.Dock = DockStyle.Fill;
            }
            catch (Exception)
            {
                //
            }
        }

        public override void DestroyControlPane(Control pane)
        {
            try
            {
                // **修正**: 确保在插件面板关闭时，数据库连接被正确断开
                if (_ucDataManege != null && _ucDataManege.conn != null && _ucDataManege.conn.State == System.Data.ConnectionState.Open)
                {
                    _ucDataManege.CloseDatabase();
                }
            }
            catch
            {
                // 忽略销毁时的错误
            }
            finally
            {
                base.DestroyControlPane(pane);
            }
        }
    }
}