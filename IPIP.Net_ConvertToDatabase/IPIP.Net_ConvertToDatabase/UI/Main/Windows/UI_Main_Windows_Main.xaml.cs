using IPIP.Net_ConvertToDatabase.Commom.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IPIP.Net_ConvertToDatabase.UI.Main.Windows
{
    /// <summary>
    /// UI_Main_Windows_Main.xaml 的交互逻辑
    /// </summary>
    public partial class UI_Main_Windows_Main : Window
    {
        private long sum = 256 * 256 * 256;
        private long curr = 0;
        private static ManualResetEvent mre = new ManualResetEvent(true);
        private BackgroundWorker bgw = null;
        public UI_Main_Windows_Main()
        {
            InitializeComponent();
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            //根据当前设置的开始和结束IP来计算总大小
            string[] strstart =  textbox_StartIP.Text.Split('.');
            string[] strend = textbox_EndIP.Text.Split('.');

            if (strstart.Length != 3 && strend.Length != 3)
            {
                return;
            }

            if (strstart.Length==3 && strend.Length ==3)
            {
                //sum = (Convert.ToInt32(strend[0]) * Convert.ToInt32(strend[1]) * Convert.ToInt32(strend[2]))
                //    - (Convert.ToInt32(strstart[0]) * Convert.ToInt32(strstart[1]) * Convert.ToInt32(strstart[2]));
                sum = IPHelper.IpToInt(textbox_EndIP.Text + ".0")
                    - IPHelper.IpToInt(textbox_StartIP.Text + ".0");
            }

            //点击开始，后台进行IP地址验证并循环插入数据库操作
            bgw = new BackgroundWorker();
            
                bgw.DoWork += Bgw_DoWork;
                bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
                bgw.ProgressChanged += Bgw_ProgressChanged;
                bgw.RunWorkerAsync(new string[] { textbox_StartIP.Text ,textbox_EndIP.Text});
            
        }

        /// <summary>
        /// 进度改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar_IP.Value = e.ProgressPercentage;
            textblock_CurrIP.Text = e.UserState.ToString();
            textblock_Percent.Text = e.ProgressPercentage + "%";
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] address = e.Argument as string[];
            string startipaddress = address[0];
            string endipaddress = address[1];
            string currIpAddress = startipaddress;

            //读取ip执行过的文件
            if(File.Exists("ip.txt"))
            {
                string tempIpAddress = File.ReadAllText("ip.txt");
                if (!string.IsNullOrEmpty(tempIpAddress))
                    currIpAddress = tempIpAddress;
            }

            //获取IP的前三个地址段
            string[] startIPArray = currIpAddress.Split('.');
            if (startIPArray.Length != 3)
                return;

            string[] endIPArray = endipaddress.Split('.');
            if (endIPArray.Length != 3)
                return;



            int startip1, startip2, startip3;
            int endip1, endip2, endip3;
            try
            {
                startip1 = Convert.ToInt32(startIPArray[0]);
                startip2 = Convert.ToInt32(startIPArray[1]);
                startip3 = Convert.ToInt32(startIPArray[2]);

                endip1 = Convert.ToInt32(endIPArray[0]);
                endip2 = Convert.ToInt32(endIPArray[1]);
                endip3 = Convert.ToInt32(endIPArray[2]);
            }
            catch
            {
                return;
            }

            //打开IP文件数据库的读取
            IPIPHelper.EnableFileWatch = true;
            IPIPHelper.Load("17monipdb.dat");

            BLL.IPBLL bll = new BLL.IPBLL();

            //循环执行
            for(int x=startip1;x<=endip1;x++)
            {
                for (int y = startip2; y <= endip2; y++)
                {
                    for (int z = startip3; z <= endip3; z++)
                    {
                        mre.WaitOne();

                        BackgroundWorker bgw = sender as BackgroundWorker;
                        if(bgw.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }


                        string[] realAddress = IPIPHelper.Find(x + "." + y + "." + z + ".0");

                        if(bll.Add(realAddress[3]
                            , realAddress[1]
                            , realAddress[2]
                            , realAddress[0]
                            , new System.Net.IPAddress(new byte[] {
                                Convert.ToByte(x),Convert.ToByte(y),Convert.ToByte(z),0
                        })))
                        {

                            StreamWriter sw = File.CreateText("ip.txt");
                            sw.Write(x + "." + y + "." + z);
                            sw.Close();

                            
                            string[] strstart = startipaddress.Split('.');

                            bgw.ReportProgress(Convert.ToInt32 ((Convert.ToDouble(IPHelper.IpToInt(x + "." + y + "." + z + ".0") - IPHelper.IpToInt(startip1 + "." + startip2 + "." + startip3 + ".0")) / sum) * 100)
                                , x + "." + y + "." + z + ".0");
                        }
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        


        private void button_Pause_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if(btn.Content.ToString() == "暂停")
            {
                btn.Content = "继续";
                mre.Reset();
            }
            else
            {
                btn.Content = "暂停";
                mre.Set();
            }
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            bgw.CancelAsync();
        }
    }
}
