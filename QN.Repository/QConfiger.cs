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

            if(Directory.Exists(pluginRootPath))
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

    }
}
