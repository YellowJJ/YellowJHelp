using SqlSugar;
using YellowJHelp.Entry;

namespace YellowJHelp.IServer
{
    /// <summary>
    /// 数据库通用方法（测试）
    /// </summary>
    public interface IYJHelpSql
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        /// <returns></returns>
        SqlSugarClient Db();
        /// <summary>
        /// 数据库日志记录
        /// </summary>
        string SqlApiLog(LogInfo apilog);

        /// <summary>
        /// 私有数据库
        /// </summary>
        /// <returns></returns>
        SqlSugarClient SYDb(string cofing);

    }
}
