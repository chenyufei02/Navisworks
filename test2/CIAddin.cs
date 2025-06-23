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
    [Plugin("test2","YUFEI",DisplayName ="AddinRibbon")]
    [RibbonLayout("AddinRibbon.xaml")]
    [RibbonTab("ID_CustomTab_1",DisplayName ="西马路项目")]
    [Command("ID_Button_1",Icon ="1_16.png",LargeIcon ="1_32.png",ToolTip ="Show a message")]

    public class CIAddin : CommandHandlerPlugin

    {
        public override int ExecuteCommand(string name, params string[] parameters)
        {
            switch (name)
            {
                case "ID_Button_1":
                    if (!Autodesk.Navisworks.Api.Application.IsAutomated)
                    {
                        //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                        var pluginRecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("ClDockPanelUpdate.YUFEI");
                        // 获取插件记录
                        if (pluginRecord is DockPanePluginRecord && pluginRecord.IsEnabled)
                        {
                            // 如果插件记录是DockPanePluginRecord类型且已启用
                            var docPanel = (DockPanePlugin)(pluginRecord.LoadedPlugin ?? pluginRecord.LoadPlugin());
                            docPanel.ActivatePane();
                        }

                    }
                    MessageBox.Show("自定义插件已加载！");
                    break;
            }
            return 0;
        }

        //private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    var assemblyName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), args.Name);
        //    if (args.Name.Contains("MySql.Data"))
        //    {
        //        assemblyName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MySql.Data.dll");
        //    }

        //    return Assembly.Load(assemblyName);
        //}
    }
}


namespace AddinDockPanel 
{
    [Plugin("ClDockPanelUpdate", "YUFEI", DisplayName = "ClDockPanelUpdate")]
    [DockPanePlugin(200,400,AutoScroll=true,MinimumHeight =100,MinimumWidth =200)]
    public class ClDockPanelUpdate : DockPanePlugin
    {
        //private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    // Extract the assembly name
        //    string assemblyName = new AssemblyName(args.Name).Name;

        //    // Load the assembly from embedded resources
        //    string resourceName = $"test2.{assemblyName}.dll";

        //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        //    {
        //        if (stream == null)
        //        {
        //            return null; // Resource not found
        //        }

        //        byte[] assemblyData = new byte[stream.Length];
        //        stream.Read(assemblyData, 0, assemblyData.Length);
        //        return Assembly.Load(assemblyData);
        //    }
        //}

        public override Control CreateControlPane()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            // 创建并返回一个自定义控件面板（面板里面的功能是UcUpdate控件实现，在前面该控件已经被添加至本项目并（在创建时）命名为"UcUpdate.cs"）。
            // return new UcUpdate();
            var tc = new TabControl(); // 创建一个TabControl面板。
            tc.ParentChanged += SetDockStyle;  // 创建方法使其能够在停靠时填满整个面板。

            // 接下来在插件面板中添加两个选项
            var tp1 = new TabPage("动态更新");  // 创建一个选项页面，页面名为"自动更新"
            tp1.Controls.Add(new UcUpdate());  // 把UcUpdate控件添加进该选项页面
            tc.TabPages.Add(tp1);  // 将整个页面添加到大的TabControl控件中。

            var tp2 = new TabPage("数据查询");  // 添加第二个页面选项，页面名为"目标查找"
            tp2.Controls.Add(new UcProperties()); // 把UserProperties控件添加进该选项页面
            tc.TabPages.Add(tp2); // 将整个页面添加到大的TabControl控件中。

            var tp3 = new TabPage("数据库管理");
            tp3.Controls.Add(new UcDataManege());
            tc.TabPages.Add(tp3);

            return tc;

        }


        private UcDataManege _ucDataManege;

        private void OnFormClosing(object sender, EventArgs e)
        {
            // 在 TabControl 关闭时检测并断开数据库连接
            if (_ucDataManege != null)
            {
                _ucDataManege.CloseDatabase() ;
            }
        }



        private void SetDockStyle(object sender, EventArgs e)
        {
            try
            {
                var tc = sender as TabControl;
                // sender as TabControl 理解为：把TabControl控件作为事件触发（也就是Sender）的判定物？   
                tc.Dock = DockStyle.Fill;
                // 设置TabControl控件的Dock属性为Fill，以填充整个面板。
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
                if (pane is TabControl tabControl)
                {
                    // 遍历 TabControl 的所有 TabPage
                    foreach (TabPage tabPage in tabControl.TabPages)
                    {
                        // 查找 UcDataManege 控件
                        var ucDataManege = tabPage.Controls.OfType<UcDataManege>().FirstOrDefault();
                        if (_ucDataManege != null)
                        {
                            _ucDataManege.CloseDatabase();
                            MessageBox.Show("数据库连接已自动关闭！");
                        }
                    }
                }
                var ctr = (UcUpdate)pane;
                ctr.Dispose();
                var ctr2 = (UcProperties)pane;
                ctr2.Dispose();
                var ctr3 = (UcDataManege)pane;
                ctr3.Dispose();
                // 调用基类方法以完成默认的销毁逻辑
                base.DestroyControlPane(pane);
            }
            catch (Exception)
            {

                //
            }
        }

    }
}