using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Bluetooth2._0Demo
{
    class Function
    {


        /// <summary>
        /// 蓝牙模块，进行蓝牙设备的扫描，返回相应最新mac地址
        /// </summary>
        ///<returns>返回扫描到蓝牙设备的MAC地址</returns>
        public  static List<string> SearchBluetoothDevices()
        {
            try
            {
                BluetoothClient blueclient = new BluetoothClient();
                BluetoothDeviceInfo[] bthDevices = blueclient.DiscoverDevices();
                List<string> macAddessList = new List<string>();

                macAddessList.RemoveRange(0, macAddessList.Count);

                foreach (BluetoothDeviceInfo device in bthDevices)
                {
                    macAddessList.Add(device.DeviceAddress.ToString());
                }
                return macAddessList;
            }
            catch (Exception err)
            {
                MessageBox.Show("蓝牙设备异常，异常" + err);
                return null;
            }
        }


        /// <summary>
        /// 修改时间的显示形式，将秒数转换成“小时：分钟：秒”的形式
        /// </summary>
        /// <param name="seconds">秒数</param>
        ///<returns>返回“小时：分钟：秒”形式的</returns>
        public static string TimeCycle(int seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, seconds);
            return ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds;
        }


        /// <summary>
        /// 显示内容处理功能
        /// </summary>
        /// <param name="allStuInfo">所有的学生信息其中包括该上课的以及离线的同学</param>
        /// <param name="displayContent">显示内容参数包括出勤，离线，全部等</param>
        /// <param name="rank">排序参数包括按照学号，姓名</param>
        /// <param name="btId">判断是否要隐藏学号信息</param>
        public static ObservableCollection<Student> DisplayContent(ObservableCollection<Student> allStuInfo, int displayContent, int rank, bool btId)
        {
            ObservableCollection<Student> dc = new ObservableCollection<Student>();

            if (displayContent == 0 && rank == 0)
            {
                if (btId)
                {
                    foreach (var item in allStuInfo)
                    {
                        item.Id = "*************";
                    }
                }
                return allStuInfo;
            }
            else
            {
                if (displayContent == 1)
                {
                    foreach (var item in allStuInfo)
                    {
                        if (item.Time != "离线")
                        {
                            dc.Add(item);
                        }
                    }
                }

                if (displayContent == 2)
                {
                    foreach (var item in allStuInfo)
                    {
                        if (item.Time == "离线")
                        {
                            dc.Add(item);
                        }
                    }
                }
            }
            if (dc.Count == 0)
            {
                //return dc;
                dc = allStuInfo;
            }
            if (rank == 1)
            {
                dc = new ObservableCollection<Student>(dc.OrderBy(item => item.Id));
                if (btId)
                {
                    foreach (var item in dc)
                    {
                        item.Id = "*************";
                    }
                }
                return dc;
            }
            else if (rank == 2)
            {
                dc = new ObservableCollection<Student>(dc.OrderBy(item => item.Name));

                if (btId)
                {
                    foreach (var item in dc)
                    {
                        item.Id = "*************";
                    }
                }
                return dc;
            }

            if (btId)
            {
                foreach (var item in dc)
                {
                    item.Id = "*************";
                }
            }
            return dc;
        }


        /// <summary>
        /// 获取最新的蓝牙在线信息
        /// </summary>
        /// <param name="shouldStuInfo">应该来上课的学生信息</param>
        /// <param name="macAddressList">蓝牙模块扫描出的MAC地址信息</param>
        /// <param name="timeDictionary">存储学生时间的字典</param>
        public static ObservableCollection<Student> Refresh(ObservableCollection<Student> shouldStuInfo, List<string> macAddressList, Dictionary<string, int> timeDictionary)
        {
            ObservableCollection<Student> showStuInfo = new ObservableCollection<Student>();

            if (shouldStuInfo != null)
            {
                foreach (var md in shouldStuInfo)
                {
                    if (macAddressList.FirstOrDefault(x => x == md.Mac) != null)
                    {

                        showStuInfo.Add(new Student()
                        {
                            Id = DatabaseProcessing.SelectId(md.Mac),
                            Name = DatabaseProcessing.SelectName(md.Mac),
                            Mac = md.Mac,
                            Time = TimeCycle(timeDictionary[md.Mac]),
                            Path = DatabaseProcessing.SelectId(md.Mac),
                        });
                    }
                    else
                    {
                        showStuInfo.Add(new Student()
                        {
                            Id = DatabaseProcessing.SelectId(md.Mac),
                            Name = DatabaseProcessing.SelectName(md.Mac),
                            Mac = md.Mac,
                            Time = "离线",
                            Path = DatabaseProcessing.SelectId(md.Mac) + "灰",
                        });
                    }
                }
            }
            else
            {
                return null;
            }
            return showStuInfo;
        }

    }
}
