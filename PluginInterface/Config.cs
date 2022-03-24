using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;

namespace PluginInterface
{
    public class Config<T> where T : class, new()
    {
        private readonly string _configFileName;

        public T ConfigStorage { get; set; } = new T();

        public Config(string file)
        {
            _configFileName = file;
            LoadConfig();
        }

        public bool LoadConfig()
        {
            if (string.IsNullOrEmpty(_configFileName))
                return false;

            try
            {
                var json = JObject.Parse(File.ReadAllText(_configFileName));
                ConfigStorage = GetSection<T>(json, "");
            }
            catch
            {
                return false;
            }

            return true;
        }

        public TK GetSection<TK>(JObject json, string sectionName = null) where TK : class, new()
        {
            if (string.IsNullOrEmpty(_configFileName))
                return default;

            if (string.IsNullOrEmpty(sectionName))
                return json?.ToObject<TK>() ??
                       throw new InvalidOperationException($"Cannot find section {sectionName}");

            return json[sectionName]?.ToObject<TK>() ??
                   throw new InvalidOperationException($"Cannot find section {sectionName}");
        }

        public bool SaveConfig()
        {
            if (string.IsNullOrEmpty(_configFileName))
                return false;

            try
            {
                File.WriteAllText(_configFileName,
                    JsonConvert.SerializeObject(ConfigStorage, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        Converters = new List<Newtonsoft.Json.JsonConverter>
                        {
                            new Newtonsoft.Json.Converters.StringEnumConverter()
                        }
                    }));
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
