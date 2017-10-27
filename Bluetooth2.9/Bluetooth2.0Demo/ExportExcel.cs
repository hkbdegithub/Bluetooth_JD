using System;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace Bluetooth2._0Demo
{
    /// <summary>
    /// 导Excel类
    /// </summary>
    public class ExportExcel
    {
        #region doExport
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dt">需要导的数据</param>
        /// <param name="tile">excel中列的标题</param>
        /// <returns>返回为空</returns>
        public static void DoExport(DataTable dt, string[] tile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = @"Execl files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;            
            saveFileDialog.Title = @"Export Excel File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == "")
            {
                return;
            }

            var myStream = saveFileDialog.OpenFile();
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
            string str = "";

            try
            {
                //写标题
                for (int i = 0; i < tile.Length; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += tile[i].Trim();
                }
                sw.WriteLine(str);

                //写内容
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    string tempStr = "";
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        
                        tempStr += dt.Rows[j][k].ToString();                        
                    }
                    sw.WriteLine(tempStr);
                }

                sw.Close();
                myStream.Close();
            }

            catch (Exception error)
            {
                sw.Close();
                myStream.Close();
                MessageBox.Show(error.ToString());
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }
        #endregion
    }
}
