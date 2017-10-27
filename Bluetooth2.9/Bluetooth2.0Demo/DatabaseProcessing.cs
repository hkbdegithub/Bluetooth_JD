using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;

namespace Bluetooth2._0Demo
{
    class DatabaseProcessing
    {
        /// <summary>
        /// 数据库查询功能，传入mac地址返回ID
        /// </summary>
        /// <param name="macAddress">需要查询的MAC地址，MAC由蓝牙扫描得出</param>
        /// <returns>返回通过MAC地址查询的学生ID</returns>
        public static string SelectId(string macAddress)
        {
            string dbpath = AppDomain.CurrentDomain.BaseDirectory + "Bluetooth.db";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder { DataSource = dbpath };
                    conn.ConnectionString = connsb.ToString();
                    conn.Open();

                    string sql = $"SELECT Student_ID FROM Bluetooth WHERE MAC= '{macAddress}'";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return (reader["Student_ID"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("SelectID函数出现异常" + e);
                throw;
            }

            return "无结果";
        }

        /// <summary>
        /// 数据库查询功能，传入mac地址返回名字
        /// </summary>
        /// <param name="macAddress">需要查询的MAC地址，MAC由蓝牙扫描得出</param>
        /// <returns>返回通过MAC地址查询的学生Name</returns>
        public static string SelectName(string macAddress)
        {

            string dbpath = AppDomain.CurrentDomain.BaseDirectory + "Bluetooth.db";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder { DataSource = dbpath };
                    conn.ConnectionString = connsb.ToString();
                    conn.Open();

                    string sql = $"SELECT Name FROM Bluetooth WHERE MAC= '{macAddress}'";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return (reader["Name"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("SelectName函数出现异常" + e);
            }
            return "无结果";
        }

        /// <summary>
        /// 数据库查询功能，传入课程名称返回相应MAC地址
        /// </summary>
        /// <param name="classInfo">需要查询的课程名称</param>
        /// <returns>返回通过MAC地址查询的学生Name</returns>
        public static ObservableCollection<Student> SelectClassInfo(string classInfo)
        {
            string dbpath = AppDomain.CurrentDomain.BaseDirectory + "Bluetooth.db";
            ObservableCollection<Student> courseStus = new ObservableCollection<Student>();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder { DataSource = dbpath };
                    conn.ConnectionString = connsb.ToString();
                    conn.Open();

                    string sql = $"SELECT Mac FROM Bluetooth WHERE Course_Info like '%{classInfo}%'";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        courseStus.Add(new Student() { Id = SelectId(reader["Mac"].ToString()), Name = SelectName(reader["Mac"].ToString()), Mac = reader["Mac"].ToString(), Time = "离线" , Path = SelectId(reader["Mac"].ToString()) });
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("出现异常" + error);
            }
            return courseStus;
        }

        /// <summary>
        /// 数据库删除功能，传入mac地址执行删除操作
        /// </summary>
        /// <param name="macInfo">需要删除内容的MAC地址</param>
        public static void DeleteInfo(string macInfo)
        {
            string dbpath = AppDomain.CurrentDomain.BaseDirectory + "Bluetooth.db";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection())
                {
                    SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder { DataSource = dbpath };
                    conn.ConnectionString = connsb.ToString();
                    conn.Open();

                    string sql = $"DELETE FROM Bluetooth WHERE Mac = '{macInfo}'";
                    SQLiteCommand command = new SQLiteCommand(sql, conn);
                    command.ExecuteReader();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("SelectName函数出现异常" + err);
            }
        }

    }
}
