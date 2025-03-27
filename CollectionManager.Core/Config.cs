using System;
using System.IO;
using Newtonsoft.Json;

namespace CollectionManager.Core
{
    public static class Config
    {
        private static readonly string ConfigFilePath = "config.json";
        public static string OsuPath { get; set; } = string.Empty;
        public static string DestinationFolder { get; set; } = string.Empty;
        public static int Threads { get; set; } = 10;
        public static int RetryCount { get; set; } = 3;
        public static string Mirror { get; set; } = string.Empty;

        public static void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    OsuPath,
                    DestinationFolder,
                    Threads,
                    RetryCount,
                    Mirror
                }, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception)
            {
            }
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    var json = File.ReadAllText(ConfigFilePath);
                    var config = JsonConvert.DeserializeObject<ConfigData>(json);

                    if (config != null)
                    {
                        DestinationFolder = config.DestinationFolder;
                        Threads = config.Threads;
                        RetryCount = config.RetryCount;
                        Mirror = config.Mirror;
                    }
                }
                else
                {
                    Save();
                }
            }
            catch (Exception)
            {
            }
        }

        private class ConfigData
        {
            public static string OsuPath { get; set; } = string.Empty;
            public string DestinationFolder { get; set; } = string.Empty;
            public int Threads { get; set; }
            public int RetryCount { get; set; }
            public string Mirror { get; set; } = string.Empty;
        }
    }
}
