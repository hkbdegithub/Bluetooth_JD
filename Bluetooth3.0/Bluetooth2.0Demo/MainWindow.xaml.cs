using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace Bluetooth2._0Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private List<string> _macAddressList;
        private List<string> _intermediateVariable;
        private ObservableCollection<Student> _stus;           //实际上课的同学信息
        private ObservableCollection<Student> _courseStus;     //应该上课的同学信息

        private int _totalTime;
        private DispatcherTimer _timer;

        private readonly Dictionary<string, int> _timeList = new Dictionary<string, int>();
        int _displayContent;
        int _rank;
        int _windowInfo = 1;

        private bool _btNow;
        public MainWindow()
        {
            InitializeComponent();



            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Task.Factory.StartNew(() =>
            {
                _macAddressList = Function.SearchBluetoothDevices();

            });

            Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            Grid.Visibility = Visibility.Hidden;

            Window1 aa = new Window1();
            aa.Show();
            // 在此点下面插入创建对象所需的代码。 
            //show timer by_songgp 
            var showTimer = new DispatcherTimer();

            showTimer.Tick += ShowCurTimer;//起个Timer一直获取当前时间 
            showTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            showTimer.Start();
            grid.Visibility = Visibility.Collapsed;
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    ResizeMode = ResizeMode.NoResize;
                }
            };
            PreviewMouseLeftButtonUp += (sender, e) =>
            {
                if (ResizeMode == ResizeMode.NoResize)
                {
                    ResizeMode = ResizeMode.CanResize;
                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxClass.Items.Add("桌面软件开发C#");

            System.Timers.Timer t = new System.Timers.Timer(3000);//实例化Timer类，设置间隔时间为200毫秒；     
            t.Elapsed += NewInterface;  //到达时间的时候执行事件；   
            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；      
            t.Enabled = true;  //是否执行System.Timers.Timer.Elapsed事件；  ,调用start()方法也可以将其设置为true    

        }

        /// <summary>
        /// 更新界面让主界面显示出啦
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewInterface(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    Grid.Visibility = Visibility.Visible;
                }));
        }

        private void BtNamed_OnClick(object sender, RoutedEventArgs e)
        {
            if (_macAddressList == null || _courseStus == null)
            {
                MessageBox.Show("蓝牙模块正在初始化中，或者您还没有选择一门课程");
                return;
            }

            if (BtNamed.Content.ToString() == "课程结束")
            {
                MessageBox.Show("下课啦！！！");
                _timer.Stop();
                TimeTb.Text = "0:0:0";
                BtNamed.Content = "开始上课";

                BtStatistics_Click(sender, e);
            }
            else
            {
                BtNamed.Content = "课程结束";


                _stus = new ObservableCollection<Student>();

                _totalTime = 0;

                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _timer.Tick += timer_Tick;
                _timer.Start();

                _intermediateVariable = _macAddressList;              //将获取的最新mac地址存储在中转list中

                _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
                Redraw(_stus);
            }
        }

        private void ComboBox_Class_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _courseStus = DatabaseProcessing.SelectClassInfo(ComboBoxClass.SelectedItem.ToString());

            _timeList.Clear();

            foreach (var item in _courseStus)
            {
                _timeList.Add(item.Mac, 0);
            }
            Redraw(_courseStus);
        }

        private void timer_Tick(object sender, EventArgs e)        //计时器功能，每秒调用一次此函数
        {
            _totalTime++;                                          //时间自增
            TimeTb.Text = Function.TimeCycle(_totalTime);         //将时间显示在前台空间上

            Dictionary<string, int> timeList2 = new Dictionary<string, int>();

            foreach (var item in _timeList)
            {
                timeList2.Add(item.Key, item.Value);
            }

            int i = 0;

            foreach (var item in timeList2.Keys)
            {
                if (_stus[i].Time != "离线")
                {
                    _timeList[item] = _timeList[item] + 1;
                }

                i++;
            }


            _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
            Redraw(_stus);

            if (_totalTime % 5 == 0)
            {
                Task.Factory.StartNew(() => _intermediateVariable = Function.SearchBluetoothDevices());
                _macAddressList = _intermediateVariable;
            }
        }


        /// <summary>
        /// 刷新界面
        /// </summary>
        /// <param name="allStuInfo">所有的学生信息其中包括该上课的以及离线的同学</param>
        void Redraw(ObservableCollection<Student> allStuInfo)
        {
            ListBoxShow.ItemsSource = null;
            ListBoxShow.ItemsSource = Function.DisplayContent(allStuInfo, _displayContent, _rank, _btNow);
        }


        private void BtAdd_OnClick(object sender, RoutedEventArgs e)
        {
            string dbpath = AppDomain.CurrentDomain.BaseDirectory + "Bluetooth.db";

            var fd = new OpenFileDialog { Filter = "Excel文件|*.xlsx|Excel文件|*.xls" };

            fd.ShowDialog();

            string temp1 = Path.GetFileNameWithoutExtension(fd.SafeFileName);

            DataSet ds = new DataSet();
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fd.FileName + ";" + "Extended Properties=Excel 12.0;";
            OleDbConnection con = new OleDbConnection(strConn);
            con.Open();
            var strExcel = $"select * from [{temp1}$]";
            var myCommand = new OleDbDataAdapter(strExcel, strConn);
            myCommand.Fill(ds, temp1);

            string insertstrSql = "insert into Bluetooth(";
            foreach (DataColumn c in ds.Tables[0].Columns)
            {
                insertstrSql += $"{c.ColumnName},";
            }

            insertstrSql = insertstrSql.Trim(',') + ")";
            insertstrSql += "values(";

            using (SQLiteConnection conn = new SQLiteConnection())
            {
                SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder { DataSource = dbpath };
                conn.ConnectionString = connsb.ToString();
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    foreach (DataRow mDr in ds.Tables[0].Rows)
                    {
                        string temp = "";
                        foreach (DataColumn mDc in ds.Tables[0].Columns)
                        {
                            temp += $"'{mDr[mDc]}',";
                        }
                        var insertstrSql1 = insertstrSql + temp;
                        insertstrSql1 = insertstrSql1.Trim(',') + ")";
                        cmd.CommandText = insertstrSql1;
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("导入完毕！");
            }
        }


        private void BtInquire_OnClick(object sender, RoutedEventArgs e)
        {
            if (BtNamed.Content.ToString() == "开始上课")
            {
                MessageBox.Show("请先开始您的课程。");
                return;
            }

            Regex regex = new Regex("^[0-9]{13}$");
            Regex regex2 = new Regex("^[\u4e00-\u9fa5]{2,4}$");
            List<string> findInfo = new List<string>();
            if (regex.IsMatch(InquiryTb.Text) || regex2.IsMatch(InquiryTb.Text))
            {
                foreach (var item in _stus)
                {
                    if (item.Id == InquiryTb.Text || item.Name == InquiryTb.Text)
                    {
                        findInfo.Add(item.Name);
                        findInfo.Add(item.Id);
                        findInfo.Add(item.Time);
                        findInfo.Add(item.Mac);
                    }
                }

                if (findInfo.Count == 0)
                {
                    MessageBox.Show("未能找到这名学生，请核实信息");
                    return;
                }
            }
            else
            {
                MessageBox.Show("输入的内容有误！");
                return;
            }

            StuInfo ll = new StuInfo(findInfo);
            ll.ShowDialog();

        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        private void BT_close(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void RB_All(object sender, RoutedEventArgs e)
        {
            if ((string)BtNamed.Content == "开始上课")
            {
                MessageBox.Show("您得先开始上课才能进行排序操作。");
                return;
            }

            _displayContent = 0;
            _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
            Redraw(_stus);
        }


        private void RB_Y(object sender, RoutedEventArgs e)
        {
            if ((string)BtNamed.Content == "开始上课")
            {
                MessageBox.Show("您得先开始上课才能进行排序操作。");
                return;
            }

            _displayContent = 1;
            _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
            Redraw(_stus);
        }


        private void RB_N(object sender, RoutedEventArgs e)
        {
            if ((string)BtNamed.Content == "开始上课")
            {
                MessageBox.Show("您得先开始上课才能进行排序操作。");
                return;
            }

            _displayContent = 2;
            _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
            Redraw(_stus);
        }


        private void RB_BS(object sender, RoutedEventArgs e)
        {
            if ((string)BtNamed.Content == "开始上课")
            {
                MessageBox.Show("您得先开始上课才能进行排序操作。");
                return;
            }

            _rank = 1;
            _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
            Redraw(_stus);
        }


        private void RB_SB(object sender, RoutedEventArgs e)
        {
            if ((string)BtNamed.Content == "开始上课")
            {
                MessageBox.Show("您得先开始上课才能进行排序操作。");
                return;
            }

            _rank = 2;
            _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
            Redraw(_stus);
        }


        private void InquiryTB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (InquiryTb.Text == "姓名或学号")
            {
                InquiryTb.Text = "";
            }
        }


        private void InquiryTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (InquiryTb.Text == "")
            {
                InquiryTb.Text = "姓名或学号";
            }

        }


        private void BtStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (_timeList.Count == 0 || _stus == null)
            {
                MessageBox.Show("请在点名后导出点名数据。");
                return;
            }

            DataTable tblDatas = new DataTable("点名结果");

            string[] strings = { "学号", "姓名", "点名时长", "在线时长", "是否通过点名" };

            tblDatas.Columns.Add("学号");
            tblDatas.Columns.Add("姓名");
            tblDatas.Columns.Add("点名时长");
            tblDatas.Columns.Add("在线时长");
            tblDatas.Columns.Add("是否通过点名");

            foreach (var item in _stus)
            {
                tblDatas.Rows.Add(item.Id, item.Name, Function.TimeCycle(_totalTime), Function.TimeCycle(_timeList[item.Mac]),
                    // ReSharper disable once PossibleLossOfFraction
                    _timeList[item.Mac] / _totalTime >= 0.8 ? "是" : "否");
            }

            ExportExcel.DoExport(tblDatas, strings);
        }


        private void min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void max_Click(object sender, RoutedEventArgs e)
        {
            if (_windowInfo == 1)
            {
                WindowState = WindowState.Maximized;
                ListBoxShow.Width = ActualWidth - 20;
                ListBoxShow.Height = ActualHeight - 150;
                Max.ToolTip = "还原";
                Label.FontSize = 40;
                Label.Width = 1000;
                Label.Margin = new Thickness(ActualWidth / 2 - 100, 7, 0, 0);
                Tt1.Height = 200;
                Tt1.Width = 1000;
                Tt1.Margin = new Thickness(ActualWidth / 2 - 100, 65, 0, 0);
                Tt1.FontSize = 60;
                Tt.Margin = new Thickness(27, 10, 0, 0);
                Caidan.Visibility = Visibility.Collapsed;
                grid.Visibility = Visibility.Collapsed;
                _windowInfo = 0;
            }
            else if (_windowInfo == 0)
            {
                WindowState = WindowState.Normal;
                ListBoxShow.Width = 736;
                ListBoxShow.Height = 419;
                Max.ToolTip = "最大化";
                Label.FontSize = 11;
                Label.Width = 101;
                Label.Margin = new Thickness(27, 7, 0, 0);
                Tt1.Height = 30;
                Tt1.Width = 250;
                Tt1.Margin = new Thickness(280, 10, 0, 0);
                Tt1.FontSize = 15;
                Tt.Margin = new Thickness(115, 10, 0, 0);
                Caidan.Visibility = Visibility.Visible;
                _windowInfo = 1;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            int temp = 0;
            PixelFormat format = PixelFormats.Gray4;

            string processPath = AppDomain.CurrentDomain.BaseDirectory + @"Image\";
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "选择文件",
                Filter = "jpg|*.jpg|jpeg|*.jpeg",
                FilterIndex = 1,
                RestoreDirectory = true,
                Multiselect = true,
                DefaultExt = "jpg"
            };

            if (dialog.ShowDialog() == true)
            {
                string[] lnames = dialog.FileNames;
                string[] names = dialog.SafeFileNames;
                for (int i = 0; i < names.Length; i++)
                {
                    //lblFileName.Content = names[i];
                    Img.Source = new BitmapImage(new Uri(lnames[i]));
                    string proImgPath = processPath + names[i];//要保存的图片的地址，包含文件名
                    BitmapSource bs = (BitmapSource)Img.Source;
                    //PngBitmapEncoder PBE = new PngBitmapEncoder();
                    JpegBitmapEncoder pbe = new JpegBitmapEncoder();
                    pbe.Frames.Add(BitmapFrame.Create(bs));
                    using (Stream stream = File.Create(proImgPath))
                    {
                        pbe.Save(stream);
                    }
                    Img.Source = ConverToGray(format);

                    proImgPath = processPath + Regex.Replace(names[i], @"[^\d]*", "") + "灰.jpg";//要保存的图片的地址，包含文件名
                    bs = (BitmapSource)Img.Source;
                    //PngBitmapEncoder PBE = new PngBitmapEncoder();
                    JpegBitmapEncoder pb = new JpegBitmapEncoder();
                    pb.Frames.Add(BitmapFrame.Create(bs));
                    using (Stream stream = File.Create(proImgPath))
                    {
                        pb.Save(stream);
                    }
                    temp++;
                }

            }

            if (temp != 0)
            {
                MessageBox.Show("保存成功！");
            }
        }

        private FormatConvertedBitmap ConverToGray(PixelFormat format)
        {
            FormatConvertedBitmap fcb = new FormatConvertedBitmap();
            fcb.BeginInit();
            fcb.Source = (BitmapSource)Img.Source;
            fcb.DestinationFormat = format;
            fcb.EndInit();
            return fcb;
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            if ((string)BtNamed.Content == "开始上课")
            {
                MessageBox.Show("您得先开始上课才能进行此操作。");
                return;
            }

            if (_btNow == false)
            {
                //默认显示，点击一下隐藏学号
                _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
                Redraw(_stus);
                BtC.ToolTip = "显示学号";
                _btNow = true;
            }
            else
            {
                //显示出学号
                _stus = Function.Refresh(_courseStus, _macAddressList, _timeList);
                Redraw(_stus);
                BtC.ToolTip = "隐藏学号";
                _btNow = false;
            }
        }

        public void ShowCurTimer(object sender, EventArgs e)
        {
            //"星期"+DateTime.Now.DayOfWeek.ToString(("d")) 

            //获得星期几 
            Tt.Text = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            Tt.Text += " ";
            //获得年月日 
            Tt.Text += DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日 
            Tt.Text += " ";
            //获得时分秒 
            Tt1.Text = DateTime.Now.ToString("HH:mm:ss");
            //System.Diagnostics.Debug.Print("this.ShowCurrentTime {0}", this.ShowCurrentTime); 
        }

        private void BT_T_Click(object sender, RoutedEventArgs e)
        {
            grid.Visibility = grid.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtAdd_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddInfo face = new AddInfo();
            face.ShowDialog();
        }
    }
}
