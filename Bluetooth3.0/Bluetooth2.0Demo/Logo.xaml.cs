using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Bluetooth2._0Demo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1
    {
        private int _totalTime;

        public Window1()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += timer_Tick;
            timer.Start();

            //sbar.Items.Clear();/////

            //Label lbl = new Label();
            //lbl.Background = new LinearGradientBrush(Colors.LightBlue, Colors.SlateBlue, 90);
            //lbl.Content = "ProgressBar with one iteration.";
            //sbar.Items.Add(lbl);
            ProgressBar progbar = new ProgressBar
            {
                IsIndeterminate = false,
                Orientation = Orientation.Horizontal,
                Foreground = Brushes.Blue,
                Width = 480,
                Height = 15
            };
            Duration duration = new Duration(TimeSpan.FromSeconds(7.5));
            DoubleAnimation doubleanimation = new DoubleAnimation(100.0, duration);
            progbar.BeginAnimation(RangeBase.ValueProperty, doubleanimation);
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
