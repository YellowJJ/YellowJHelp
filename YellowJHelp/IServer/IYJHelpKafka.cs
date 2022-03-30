using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowJHelp.IServer
{
    /// <summary>
    /// Kafka链接通用方法
    /// </summary>
    public interface IYJHelpKafka
    {

        /// <summary>
        /// 发布者
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        /// <returns></returns>
        Task<string> Produce(string theme, string json, string BS);


        /// <summary>
        /// 发布者-随机分区
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        /// <param name="start">分区起始值：0</param>
        /// <param name="end">分区结束值：0</param>
        /// <returns></returns>
        Task<string> Produce(string theme, string json, string BS, int start = 0, int end = 0);

        /// <summary>
        /// 发布者（带账号密码）
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        /// <param name="name">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="skey"></param>
        Task<string> ProduceAdmin(string theme, string json, string BS, string name, string pwd, string? skey = null);

        /// <summary>
        /// 发布者-随机分区（带账号密码）
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        /// <param name="name">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="skey"></param>
        /// <param name="start">分区起始值：0</param>
        /// <param name="end">分区结束值：0</param>
        Task<string> ProduceAdminPartition(string theme, string json, string BS, string name, string pwd, int start = 0, int end = 0, string? skey = null);

    }
}
