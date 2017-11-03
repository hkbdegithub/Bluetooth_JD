using InTheHand.Net.Sockets;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bluetooth2._0Demo
{
    /// <summary>
    /// AddInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AddInfo : Window
    {
        private ObservableCollection<BluetoothInfo> _macAddressList;
        public AddInfo()
        {
            InitializeComponent();

            //WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Task.Factory.StartNew(() =>
            {
                _macAddressList = SearchBluetoothDevices();
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    //foreach (var item in _macAddressList)
                    //{
                    //    ListBoxShow.Items.Add(item.Name + "\n" + item.Name);
                    //}
                    label.Visibility = Visibility.Collapsed;
                    ListBoxShow.ItemsSource = _macAddressList;
                }));
            });
        }

        ObservableCollection<BluetoothInfo> SearchBluetoothDevices()
        {
            BluetoothClient blueclient = new BluetoothClient();
            BluetoothDeviceInfo[] bthDevices = blueclient.DiscoverDevices();
            ObservableCollection<BluetoothInfo> macAddessList = new ObservableCollection<BluetoothInfo>();


            foreach (BluetoothDeviceInfo device in bthDevices)
            {
                macAddessList.Add(new BluetoothInfo()
                {
                    Name = device.DeviceName,
                    Mac = device.DeviceAddress.ToString(),
                });
            }

            return macAddessList;
        }

        class BluetoothInfo
        {
            public string Name { get; set; }

            public string Mac { get; set; }
        }

        private void ListBoxShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(ListBoxShow.SelectedIndex.ToString());

            ChangeInfo face = new ChangeInfo(_macAddressList[ListBoxShow.SelectedIndex].Mac, "添加");
            face.ShowDialog();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BT_close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            DataTable tblDatas = new DataTable("4521");

            string[] strings = { "Student_ID", "Name", "Curriculum", "MAC", "HeadSculpture_Src", "Course_Info" };

            tblDatas.Columns.Add("Student_ID");
            tblDatas.Columns.Add("Name");
            tblDatas.Columns.Add("Curriculum");
            tblDatas.Columns.Add("MAC");
            tblDatas.Columns.Add("HeadSculpture_Src");
            tblDatas.Columns.Add("Course_Info");

            foreach (var item in _macAddressList)
            {
                tblDatas.Rows.Add("", item.Name + "请修改", "", item.Mac, "", "桌面软件开发C#");
            }

            ExportExcel.DoExport(tblDatas, strings);

        }
    }
}
