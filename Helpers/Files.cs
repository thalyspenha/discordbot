using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Discord_Bot.Helpers
{
    public class Files
    {
        private IConfigurationRoot _config;
        public Files()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public string GetRandomImage()
        {
            string path = _config.GetValue<string>("images:caminho");



            string file = null;
            if (!string.IsNullOrEmpty(path))
            {
                var extensions = new string[] { ".png", ".jpg", ".gif" };
                try
                {
                    var di = new DirectoryInfo(path);
                    var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                    Random R = new Random();
                    file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                }
                catch { }
            }
            return file;
        }
    }
}
