using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowJHelp.Entry;

namespace YellowJHelp
{
    /// <summary>
    /// Kafka链接通用方法
    /// </summary>
    public class YJHelpKafka
    {

        /// <summary>
        /// 发布者
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        public static async Task<string> Produce(string theme, string json, string BS)
        {
            string ret = "";
            var config = new ProducerConfig { BootstrapServers = BS };

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {

                LogInfo log = new LogInfo();
                log.State = "0";
                log.Source = theme;
                log.SourceCode = "KafKa:" + config.BootstrapServers;
                log.SourceName = "发布者";

                try
                {

                    var dr = await p.ProduceAsync(theme, new Message<Null, string> { Value = json });

                    log.Returbed = dr.Value;
                    ret = dr.Value;
                }
                catch (ProduceException<Null, string> e)
                {
                    log.Returbed = e.Error.Reason;
                    ret = e.Error.Reason;
                }
                finally
                {
                    try
                    {
                        YJHelpSql.SqlApiLog(log);
                    }
                    catch (Exception ex) { YJHelp.YellowJLog(YJHelp.message + "Kafka记录日志-上传网络连接失败（本地可忽略）", "异常"); }

                }
            }

            return ret;
        }

        /// <summary>
        /// 发布者（带账号密码）
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        /// <param name="name">账号</param>
        /// <param name="pwd">密码</param>
        public static async Task<string> ProduceAdmin(string theme, string json,string BS,string name,string pwd)
        {
            string ret = "";
            var config = new ProducerConfig { BootstrapServers = BS, SecurityProtocol = SecurityProtocol.SaslPlaintext, SaslMechanism = SaslMechanism.Plain, SaslUsername = name, SaslPassword = pwd };

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {

                LogInfo log = new LogInfo();
                log.State = "0";
                log.Source = theme;
                log.SourceCode = "KafKa:" + config.BootstrapServers;
                log.SourceName = "发布者";

                try
                {

                    var dr = await p.ProduceAsync(theme, new Message<Null, string> { Value = json });

                    log.Returbed = dr.Value;
                    ret = dr.Value;
                }
                catch (ProduceException<Null, string> e)
                {
                    log.Returbed = e.Error.Reason;
                    ret = e.Error.Reason;
                }
                finally
                {
                    try {
                        YJHelpSql.SqlApiLog(log);
                    } catch (Exception ex) { YJHelp.YellowJLog(YJHelp.message+ "Kafka记录日志-上传网络连接失败（本地可忽略）", "异常"); }
                    
                }
            }

            return ret;
        }
    }
}
