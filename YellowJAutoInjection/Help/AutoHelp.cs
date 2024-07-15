using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YellowJAutoInjection.Help
{
    public class AutoHelp
    {
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="text">消息</param>
        /// <param name="address">相对文件名</param>
        public void YellowJLog(string text, string address)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Path.Combine(path
            , "YellowJ_Logs/" + address + "\\");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileFullName = System.IO.Path.Combine(path
            , string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd")));


            using (StreamWriter output = System.IO.File.AppendText(fileFullName))
            {
                output.WriteLine(DateTime.Now.ToString() + " 日志信息：" + text);

                output.Close();
            }
        }
    }
}
