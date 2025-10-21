using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GitHelper
{
    public class AppConfig
    {
        public string LastDirectory { get; set; }
        public List<string> RecentDirectories { get; set; } = new List<string>();
    }

    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "config.json"
        );

        public static AppConfig LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    
                    // 过滤不存在的目录
                    if (config != null)
                    {
                        config.RecentDirectories = config.RecentDirectories
                            .Where(d => Directory.Exists(d))
                            .Distinct()
                            .ToList();
                        
                        if (!Directory.Exists(config.LastDirectory))
                        {
                            config.LastDirectory = null;
                        }
                        
                        return config;
                    }
                }
            }
            catch
            {
                // 配置文件读取失败，返回默认配置
            }

            return new AppConfig();
        }

        public static void SaveConfig(AppConfig config)
        {
            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                };
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch
            {
                // 忽略保存错误
            }
        }

        public static void AddRecentDirectory(string directory, int maxCount = 5)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                return;

            var config = LoadConfig();
            
            // 移除已存在的相同路径
            config.RecentDirectories.Remove(directory);
            
            // 添加到列表开头
            config.RecentDirectories.Insert(0, directory);
            
            // 限制数量
            if (config.RecentDirectories.Count > maxCount)
            {
                config.RecentDirectories = config.RecentDirectories.Take(maxCount).ToList();
            }
            
            // 更新最后使用的目录
            config.LastDirectory = directory;
            
            SaveConfig(config);
        }

        public static List<string> GetRecentDirectories()
        {
            var config = LoadConfig();
            return config.RecentDirectories;
        }

        public static string GetLastDirectory()
        {
            var config = LoadConfig();
            return config.LastDirectory;
        }
    }
}
