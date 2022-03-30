using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowJHelp.Entry
{
    /// <summary>
    /// 日志
    /// </summary>
    [SugarTable("YellowJ_Log")]
    public class LogInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        /// 编码(日志记录自动生成记录编码-不用传值)
        /// </summary>
        [SugarColumn(ColumnDataType = "Nvarchar(100)")]//自定格式的情况 length不要设置
        public string Number { get; set; }
        /// <summary>
        /// 创建时间（日志记录自动生成当前日期-不用传值）
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 状态级
        /// </summary>
        [SugarColumn(ColumnDataType = "Nvarchar(20)")]//自定格式的情况 length不要设置
        public string State { get; set; }
        /// <summary>
        /// 耗时
        /// </summary>
        //[SugarColumn(ColumnDataType = "Nvarchar(20)")]//自定格式的情况 length不要设置
        public double ElapsedTime { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 来源编码
        /// </summary>
        [SugarColumn(ColumnDataType = "Nvarchar(500)")]//自定格式的情况 length不要设置
        public string SourceCode { get; set; }
        /// <summary>
        /// 来源名称
        /// </summary>
        [SugarColumn(ColumnDataType = "Nvarchar(100)")]//自定格式的情况 length不要设置
        public string SourceName { get; set; }
        /// <summary>
        /// 接收值
        /// </summary>
        [SugarColumn(ColumnDataType = "Nvarchar(MAX)")]//自定格式的情况 length不要设置
        public string Acceptor { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        [SugarColumn(ColumnDataType = "Nvarchar(MAX)")]//自定格式的情况 length不要设置
        public string Returbed { get; set; }
    }
}
