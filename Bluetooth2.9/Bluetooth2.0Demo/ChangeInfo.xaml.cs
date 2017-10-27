using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;

namespace Bluetooth2._0Demo
{
    /// <summary>
    /// ChangeInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeInfo
    {
        string _Mac;
        string _dbpath = string.Empty;
        string _type;
        public ChangeInfo(string mac ,string type)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _Mac = mac;
            _type = type;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (_type == "添加")
            {
                _dbpath = AppDomain.CurrentDomain.BaseDirectory + "Bluetooth.db";
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection())
                    {
                        SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder { DataSource = _dbpath };
                        conn.ConnectionString = connsb.ToString();
                        conn.Open();

                        string sql = $"INSERT INTO Bluetooth(Student_ID, Name, Mac, Course_info) VALUES('{TB_ID.Text}', '{TB_Name.Text}', '{_Mac}', '{TB_Class.Text}')";
                        
                        SQLiteCommand command = new SQLiteCommand(sql, conn);
                        command.ExecuteNonQuery();

                        MessageBox.Show("学生信息添加成功！");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("出现异常" + error);
                }
            }
            else
            {
                _dbpath = AppDomain.CurrentDomain.BaseDirectory + "Bluetooth.db";
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection())
                    {
                        SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder { DataSource = _dbpath };
                        conn.ConnectionString = connsb.ToString();
                        conn.Open();

                        string sql = $"UPDATE Bluetooth SET Course_info = '{TB_Class.Text}' WHERE Mac = '{_Mac}'";

                        SQLiteCommand command = new SQLiteCommand(sql, conn);
                        command.ExecuteReader();

                        MessageBox.Show("学生信息修改成功！");
                    }

                }
                catch (Exception error)
                {
                    MessageBox.Show("出现异常" + error);
                }
            }

            Close();
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BT_close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
