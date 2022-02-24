using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowJHelpFw.Entry;

namespace YellowJHelpFw
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
                    YJHelp.YellowJLog(YJHelp.message + e.Error.Reason, "异常");
                }
                finally
                {

                }
            }

            return ret;
        }


        /// <summary>
        /// 发布者-随机分区
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        /// <param name="start">分区起始值：0/param>
        /// <param name="end">分区结束值：0</param>
        public static async Task<string> Produce(string theme, string json, string BS, int start = 0, int end = 0)
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


                //主题设置随机分区
                Partition partition = new Partition();
                Random rd = new Random();
                int sjs = rd.Next(start, end);
                partition = sjs;
                TopicPartition topicPartition = new TopicPartition(theme, partition);

                try
                {

                    var dr = await p.ProduceAsync(topicPartition, new Message<Null, string> { Value = json });

                    log.Returbed = dr.Value;
                    ret = dr.Value;
                }
                catch (ProduceException<Null, string> e)
                {
                    log.Returbed = e.Error.Reason;
                    ret = e.Error.Reason;
                    YJHelp.YellowJLog(YJHelp.message + e.Error.Reason, "异常");
                }
                finally
                {

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
        /// <param name="skey"></param>
        public static async Task<string> ProduceAdmin(string theme, string json,string BS,string name,string pwd,string skey=null)
        {
            string ret = "";
            var config = new ProducerConfig { BootstrapServers = BS, SecurityProtocol = SecurityProtocol.SaslPlaintext, SaslMechanism = SaslMechanism.Plain, SaslUsername = name, SaslPassword = pwd };

            using (var p = new ProducerBuilder<string, string>(config).Build())
            {

                LogInfo log = new LogInfo();
                log.State = "0";
                log.Source = theme;
                log.SourceCode = "KafKa:" + config.BootstrapServers;
                log.SourceName = "发布者";

                try
                {

                    var dr = await p.ProduceAsync(theme, new Message<string, string> {Key= skey, Value = json });

                    log.Returbed = dr.Value;
                    ret = dr.Value;
                }
                catch (ProduceException<Null, string> e)
                {
                    log.Returbed = e.Error.Reason;
                    ret = e.Error.Reason;
                    YJHelp.YellowJLog(YJHelp.message + e.Error.Reason, "异常");
                }
                finally
                {
                    
                }
            }

            return ret;
        }

        /// <summary>
        /// 发布者-随机分区（带账号密码）
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        /// <param name="name">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="skey"></param>
        /// <param name="start">分区起始值：0/param>
        /// <param name="end">分区结束值：0</param>
        public static async Task<string> ProduceAdminPartition(string theme, string json, string BS, string name, string pwd, int start = 0, int end = 0, string skey = null)
        {
            string ret = "";
            var config = new ProducerConfig { BootstrapServers = BS, SecurityProtocol = SecurityProtocol.SaslPlaintext, SaslMechanism = SaslMechanism.Plain, SaslUsername = name, SaslPassword = pwd };

            using (var p = new ProducerBuilder<string, string>(config).Build())
            {

                LogInfo log = new LogInfo();
                log.State = "0";
                log.Source = theme;
                log.SourceCode = "KafKa:" + config.BootstrapServers;
                log.SourceName = "发布者";


                //主题设置随机分区
                Partition partition = new Partition();
                Random rd = new Random();
                int sjs = rd.Next(start, end);
                partition = sjs;
                TopicPartition topicPartition = new TopicPartition(theme, partition);

                try
                {

                    var dr = await p.ProduceAsync(topicPartition, new Message<string, string> { Key = skey, Value = json });

                    log.Returbed = dr.Value;
                    ret = dr.Value;
                }
                catch (ProduceException<Null, string> e)
                {
                    log.Returbed = e.Error.Reason;
                    ret = e.Error.Reason;
                    YJHelp.YellowJLog(YJHelp.message + e.Error.Reason, "异常");
                }
                finally
                {

                }
            }

            return ret;
        }

    }
}
