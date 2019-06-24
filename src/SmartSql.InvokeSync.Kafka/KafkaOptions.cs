using System;
using System.Collections.Generic;

namespace SmartSql.InvokeSync.Kafka
{
    public class KafkaOptions
    {
        private bool _isInitialize;
        private const string BOOTSTRAP_SERVERS = "bootstrap.servers";

        public KafkaOptions()
        {
            Config = new Dictionary<string, string>();
        }

        public string Servers { get; set; }
        public String Topic { get; set; }

        public IDictionary<string, string> Config { get; set; }

        public IEnumerable<KeyValuePair<string, string>> AsKafkaConfig()
        {
            if (_isInitialize)
            {
                return Config;
            }

            _isInitialize = true;
            if (!Config.ContainsKey(BOOTSTRAP_SERVERS))
            {
                Config.Add(BOOTSTRAP_SERVERS, Servers);
            }

            return Config;
        }
    }
}