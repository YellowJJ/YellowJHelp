using Mapster;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowJHelp.Entry;

namespace YellowJHelp
{
    /// <summary>
    /// 数据库通用方法（测试）
    /// </summary>
    public class YJHelpSql
    {
        /// <summary>
        /// sqlserver链接参数
        /// </summary>
        public static string sqlconntion = null;
        /// <summary>
        /// 数据库操作
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient Query()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = sqlconntion,//必填, 数据库连接字符串
                DbType = SqlSugar.DbType.SqlServer,         //必填, 数据库类型
                IsAutoCloseConnection = true,       //默认false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = InitKeyType.SystemTable    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
            });

            return db;
        }
        /// <summary>
        /// 数据库日志记录
        /// </summary>
        public static string SqlApiLog(LogInfo apilog)
        {
            LogInfo log = new LogInfo();
            log = apilog.Adapt<LogInfo>();
            int OPSIndex = Convert.ToInt32(DateTime.Now.ToString("ffff"));
            //编码规则
            if (OPSIndex > 9) { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + "00" + OPSIndex.ToString(); }
            else if (OPSIndex > 99) { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + "0" + OPSIndex.ToString(); }
            else if (OPSIndex > 999) { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + OPSIndex.ToString(); }
            else { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + "000" + OPSIndex.ToString(); }
            log.Date = DateTime.Now;
            //SYQuery().Insertable(log).ExecuteCommand();
            return "日志耗时记录【" + log.Number + "】：" + log;

        }

        /// <summary>
        /// 私有数据库
        /// </summary>
        /// <returns></returns>
        private static SqlSugarClient SYQuery(string cofing)
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = cofing,//必填, 数据库连接字符串
                DbType = SqlSugar.DbType.SqlServer,         //必填, 数据库类型
                IsAutoCloseConnection = true,       //默认false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = InitKeyType.SystemTable    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
            });

            return db;
        }

    }
}
