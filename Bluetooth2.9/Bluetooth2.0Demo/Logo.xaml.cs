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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bluetooth2._0Demo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        private int _totalTime;
        private DispatcherTimer _timer;
        public Window1()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += timer_Tick;
            _timer.Start();

            //sbar.Items.Clear();/////

            //Label lbl = new Label();
            //lbl.Background = new LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 90);
            //lbl.Content = "ProgressBar with one iteration.";
            //sbar.Items.Add(lbl);
            ProgressBar progbar = new ProgressBar();
            progbar.IsIndeterminate = false;
            progbar.Orientation = Orientation.Horizontal;
            progbar.Foreground = Brushes.Blue;
            progbar.Width = 480;
            progbar.Height = 15;
            Duration duration = new Duration(TimeSpan.FromSeconds(7.5));
            DoubleAnimation doubleanimation = new DoubleAnimation(100.0, duration);
            progbar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
            //sbar.Items.Add(progbar);/////

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _totalTime++; //时间自增
            //label.Content = _totalTime.ToString();
            if (_totalTime == 3)
            {
                Close();
            }
        }
    }
}
