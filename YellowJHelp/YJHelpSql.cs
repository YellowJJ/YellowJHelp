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
    public class YJHelpSql
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient Query()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Server=39.108.150.2;Database=YellowJCore;User ID=sa;Password=A1B2C3Dh.;",//必填, 数据库连接字符串
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

            //生成表
            //HelpSql.Query().CodeFirst.SetStringDefaultLength(200).InitTables(typeof(OPS_Log));//这样一个表就能成功创建了
            //HelpSql.SqlQuery().CodeFirst.InitTables(typeof(OPS_Log));//生成日志表

            LogInfo log = new LogInfo();

            //var entity = repository.Find(1);
            log = apilog.Adapt<LogInfo>();


            int OPSIndex = Convert.ToInt32(DateTime.Now.ToString("ffff"));
            //编码规则
            if (OPSIndex > 9) { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + "00" + OPSIndex.ToString(); }
            else if (OPSIndex > 99) { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + "0" + OPSIndex.ToString(); }
            else if (OPSIndex > 999) { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + OPSIndex.ToString(); }
            else { log.Number = "ApiLog" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + "000" + OPSIndex.ToString(); }

            log.Date = DateTime.Now;

            Query().Insertable(log).ExecuteCommand();


            //HelpSql help = new HelpSql();
            return "日志耗时记录【" + log.Number + "】：" + log;

        }

    }
}
