using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;

namespace PluginInterface
{
    public class Config<T> where T : class, new()
    {
        private readonly string _configFileName;
        private T _configStorage = new T();

        public T ConfigStorage
        {
            get => _configStorage;
            set => _configStorage = value;
        }

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

        public T GetSection<T>(JObject json, string sectionName = null) where T : class
        {
            if (string.IsNullOrEmpty(_configFileName))
                return default;

            if (string.IsNullOrEmpty(sectionName))
            {
                return json?.ToObject<T>() ??
                       throw new InvalidOperationException($"Cannot find section {sectionName}");
            }

            return json[sectionName]?.ToObject<T>() ??
                   throw new InvalidOperationException($"Cannot find section {sectionName}");
        }

        public bool SaveConfig()
        {
            if (string.IsNullOrEmpty(_configFileName))
                return false;

            try
            {
                File.WriteAllText(_configFileName,
                    JsonConvert.SerializeObject(ConfigStorage, Formatting.Indented));
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
