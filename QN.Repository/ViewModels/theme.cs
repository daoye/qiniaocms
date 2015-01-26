using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QN
{
    /// <summary>
    /// 表示一个主题
    /// </summary>
    public class theme : model<theme>
    {
        private theme() { }

        /// <summary>
        /// 主题名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 主题目录名称
        /// </summary>
        public string dirname { get; private set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 在线预览地址
        /// </summary>
        public string prevurl { get; set; }

        /// <summary>
        /// 快照图片
        /// </summary>
        public string shot { get; private set; }

        /// <summary>
        /// 配置文件物理路径
        /// </summary>
        public string configfile { get; private set; }

        /// <summary>
        /// 保存修改
        /// </summary>
        public void Save()
        {
            XDocument xdoc = XDocument.Load(this.configfile);

            xdoc.Root.Descendants("Name").First().Value = this.name;
            xdoc.Root.Descendants("Author").First().Value = this.author;
            xdoc.Root.Descendants("Description").First().Value = this.description;
            xdoc.Root.Descendants("Url").First().Value = this.url;
            xdoc.Root.Descendants("Version").First().Value = this.version;
        }

        /// <summary>
        /// 重新加载配置信息
        /// </summary>
        public void Reload()
        {
            if (!File.Exists(configfile))
            {
                throw new FileNotFoundException("没有找到配置文件，在：" + configfile + "。");
            }

            string path = Path.GetFullPath(configfile);
            string shotjpg = "shot.png";

            XDocument xdoc = XDocument.Load(this.configfile);

            this.name = xdoc.Root.Descendants("name").First().Value;
            this.author = xdoc.Root.Descendants("author").First().Value;
            this.description = xdoc.Root.Descendants("description").First().Value;
            this.url = xdoc.Root.Descendants("url").First().Value;
            this.version = xdoc.Root.Descendants("version").First().Value;
            this.dirname = new DirectoryInfo(Path.GetDirectoryName(configfile)).Name;

            if (File.Exists(Path.Combine(path, shotjpg)))
            {
                this.shot = this.dirname + Path.DirectorySeparatorChar + shotjpg;
                if (this.configfile.ToLower().Contains(Path.DirectorySeparatorChar.ToString() + "sites" + Path.DirectorySeparatorChar.ToString()))
                {
                    this.shot = "~/sites" + Path.DirectorySeparatorChar + R.site.domain.Replace(":", "_") + Path.DirectorySeparatorChar + shot;
                }
                else
                {
                    this.shot = "~/themes" + Path.DirectorySeparatorChar + shot;
                }
            }
            else
            {
                this.shot = R.root + "content/images/shot.png";
            }
        }

        /// <summary>
        /// 创建一个主题配置信息对象
        /// </summary>
        /// <param name="configFilePath">配置文件的路径</param>
        /// <returns></returns>
        public static theme Load(string configFilePath)
        {
            theme theme = new theme();
            theme.configfile = configFilePath;
            theme.Reload();

            return theme;
        }
    }
}
