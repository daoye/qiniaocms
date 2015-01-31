using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace QN
{
    /// <summary>
    /// 配置管理
    /// </summary>
    public sealed class QConfiger
    {
        public static QConfiger Instance { get; private set; }

        static QConfiger()
        {
            Instance = new QConfiger();
        }

        private QConfiger() { }

        /// <summary>
        /// 所有插件目录
        /// </summary>
        /// <returns></returns>
        public List<string> PluginPaths()
        {
            List<string> dir = new List<string>();

            string pluginRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            if (Directory.Exists(pluginRootPath))
            {
                foreach (string path in Directory.GetDirectories(pluginRootPath))
                {
                    dir.Add(path);
                }
            }

            return dir;
        }

        /// <summary>
        /// 当前程序是否处于调试阶段
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                string debug = ConfigurationManager.AppSettings["isdebug"];

                if (string.IsNullOrWhiteSpace(debug))
                {
                    return false;
                }

                bool result = false;
                bool.TryParse(debug, out result);

                return result;
            }
        }

        /// <summary>
        /// 获取当前程序的临时目录
        /// </summary>
        public static string TempPath
        {
            get
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Join(Path.DirectorySeparatorChar.ToString(), new string[] { "App_Data", "Temp" }));
                if(!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                return fullPath;
            }
        }

        /// <summary>
        /// 将域名转换为目录名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string DomainToDirectoryName(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return string.Empty;
            }

            domain = domain.ToLower().Trim('/').Trim('\\');
            if (domain.StartsWith("http://"))
            {
                domain = domain.Substring(7, domain.Length - 7);
            }
            else if (domain.StartsWith("https://"))
            {
                domain = domain.Substring(8, domain.Length - 8);
            }

            return domain.Replace(":", "_") + "/";
        }
    }
}
