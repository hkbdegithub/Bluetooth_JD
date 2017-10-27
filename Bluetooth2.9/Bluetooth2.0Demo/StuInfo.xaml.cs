using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Bluetooth2._0Demo
{
    /// <summary>
    /// StuInfo.xaml 的交互逻辑
    /// </summary>
    public partial class StuInfo
    {

        private ObservableCollection<Student> _stus;

        public StuInfo(List<string> a)
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _stus = new ObservableCollection<Student>();

            _stus.Add(new Student()
            {
                Id = a[1],
                Name = a[0],
                Time = a[2],
                Mac = a[3],
            });
            //MessageBox.Show(a[0]);

            ListBoxShow.ItemsSource = _stus;


        }

        private void BT_close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ListBoxShow_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            DatabaseProcessing.DeleteInfo(_stus[0].Mac);
            Close();
        }
    }
}
