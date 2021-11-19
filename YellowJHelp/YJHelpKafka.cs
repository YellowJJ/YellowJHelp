using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowJHelp.Entry;

namespace YellowJHelp
{
    public class YJHelpKafka
    {
        /// <summary>
        /// 发布者（带账号密码）
        /// </summary>
        /// <param name="theme">主题</param>
        /// <param name="json">数据</param>
        /// <param name="BS">kafka连接地址</param>
        public static async Task<string> Produce(string theme, string json,string BS,string name,string pwd)
        {
            string ret = "";

            //var config = new ProducerConfig { BootstrapServers = "127.0.0.1:9092"};
            var config = new ProducerConfig { BootstrapServers = BS, SecurityProtocol = SecurityProtocol.SaslPlaintext, SaslMechanism = SaslMechanism.Plain, SaslUsername = name, SaslPassword = pwd };
            //var config = new ProducerConfig { BootstrapServers = "192.168.1.254:9092" , SecurityProtocol = SecurityProtocol.SaslPlaintext, SaslMechanism= SaslMechanism.Plain, SaslUsername = "orico", SaslPassword = "kafka_f2b211" };

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
                    //Help.YellowJLog($"记录 '{DateTime.Now}：{dr.Value}'  主题：'{dr.TopicPartitionOffset}'", "kafka");
                }
                catch (ProduceException<Null, string> e)
                {
                    log.Returbed = e.Error.Reason;
                    ret = e.Error.Reason;
                }
                finally
                {
                    YJHelpSql.SqlApiLog(log);
                }
            }

            return ret;
        }
    }
}
