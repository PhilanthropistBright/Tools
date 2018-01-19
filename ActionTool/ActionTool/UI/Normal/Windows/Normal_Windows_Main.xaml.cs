using ActionTool.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ActionTool.UI.Normal.Windows
{
    /// <summary>
    /// Normal_Windows_Main.xaml 的交互逻辑
    /// </summary>
    public partial class Normal_Windows_Main : Window
    {
        public Normal_Windows_Main()
        {
            InitializeComponent();
        }

        private void button_Commit_Click(object sender, RoutedEventArgs e)
        {
            if(combobox_ActionType.Text!="GET")
            {
                textbox_ResponseData.Text = HttpHelper.PostWithJson(textbox_Data.Text, textbox_ActionUrl.Text, combobox_ActionType.Text);
            }
        }
    }
}
