using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Navisworks.Api;
using MySql.Data.MySqlClient;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api.ComApi;
using Inw = Autodesk.Navisworks.Api.Interop.ComApi;
using Autodesk.Navisworks.Gui.Roamer;

//using System.Windows.Controls;

namespace test2.Ctr
{
    public partial class UcProperties : UserControl
    {
        public Timer UpTimer = new Timer { Enabled = true, Interval = 1000 };

        public UcProperties()
        {
            InitializeComponent();
            UpTimer.Tick += TimerTicked;  // 订阅定时器的 Elapsed 事件
        }

        private void TimerTicked(object sender, EventArgs e)
        {
            ListenSelection(null, null);  // 这里的两个null表示：如果没有参数，则表示监听所有事件
            Autodesk.Navisworks.Api.Application.ActiveDocumentChanged += ListenSelection;  // 监听文档变化，活动文档变化时，重新调用ListenSelection方法

        }

        private void ListenSelection(object value1, object value2)
        {
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Changed += GetProperties;  // 文本框获取选中构件的属性
        }

        private void GetProperties(object sender, EventArgs e)
        {
            // 打印之前先清空文本框
            TbOut.Clear();

            var ResultList = new List<string>();  // 创建一个列表用来存储结果

            foreach (var item in Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems)  // 对当前文档的选中内容的所有构件进行遍历
            {

                ResultList.Add(item.DisplayName);  // 构件名
                foreach (var category in item.PropertyCategories)  // 对选中构件的每一个种类名（项目名？）进行遍历
                {
                    ResultList.Add(string.Concat(".   ", category.DisplayName));
                    foreach (var prop in category.Properties)  // 对选中种类（项目？）的每一个属性进行遍历
                    {
                        ResultList.Add(string.Concat(".   .   ", prop.DisplayName, ": ", GetPropertyValue(prop)));
                        // 分别添加属性名和属性值，String.Contact用来格式化语句
                    }
                }

                ResultList.Add(Environment.NewLine); // 收集完所有的属性后，添加一个换行符，以开始下一个项目的属性查找
            }

            // 将结果列表里的每一项在文本框里打印出来
            TbOut.Text = string.Join(Environment.NewLine, ResultList);  // 将列表里的每一项【用换行符连接】起来，作为文本框的文本内容
        }

        public static object GetPropertyValue(DataProperty prop)
        {
            if (prop== null)
            {
                return "元素属性内没有ID项";
            }

            if (prop.Value.IsDisplayString)
            {
                return prop.Value.ToDisplayString();
            }
            if (prop.Value.IsDoubleArea)
            {
                decimal result = (decimal)prop.Value.ToDoubleArea();
                result /= (decimal)10.763915;
                decimal roundedResult = Math.Round(result, 3, MidpointRounding.AwayFromZero);
                return roundedResult;
            }
            if (prop.Value.IsDoubleVolume)
            {

                decimal result = (decimal)prop.Value.ToDoubleVolume();
                result /= (decimal)35.314667;
                decimal roundedResult = Math.Round(result, 3, MidpointRounding.AwayFromZero);
                return roundedResult;
            }
            if (prop.Value.IsDoubleLength)
            {
                decimal result = (decimal)prop.Value.ToDoubleLength();
                result /= (decimal)3.28084;
                decimal roundedResult = Math.Round(result, 3, MidpointRounding.AwayFromZero);   
                return roundedResult;
           
            }
            else
            {
                var result = prop.Value.ToString();
                // 示例输入：result = "温度: 25℃: 实验室数据"
                int colonIndex = result.IndexOf(':');
                if (colonIndex >= 0)
                {
                    return result.Substring(colonIndex + 1).Trim(); // 输出 "25℃: 实验室数据
                }
                else
                {
                    return result; // 未找到冒号时的处理（返回原字符串）
                }
            }


        }


        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Dock = DockStyle.Fill;
        }

        private void TbOut_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtFind_MouseUp(object sender, MouseEventArgs e)
        {
            var r = new List<ModelItem>();
            foreach (var item in Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems)
            {
                var cat = item.DescendantsAndSelf.Where(i => i.PropertyCategories.FindCategoryByDisplayName(tbCategoryName.Text) != null);
                var pro = cat.Where(m => m.PropertyCategories.FindCategoryByDisplayName(tbCategoryName.Text).Properties.FindPropertyByDisplayName(tbPropertyName.Text) != null);
                foreach (var modelItem in pro)
                {
                    var property = modelItem.PropertyCategories.FindCategoryByDisplayName(tbCategoryName.Text).Properties.FindPropertyByDisplayName(tbPropertyName.Text);
                    var propertyValue = GetPropertyValue(property).ToString();
                    if (propertyValue.Contains(tbPropertyValue.Text) || property!=null)
                    {
                        r.Add(modelItem);
                    }
                }
            }
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Clear();
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.AddRange(r);
        }




    }
}
