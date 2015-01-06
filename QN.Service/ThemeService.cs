#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	ThemeService.cs
	Author:		DaoYe
	History: 	6/12/2014 14:18 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using QN.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QN.Service
{
    public class ThemeService
    {
        private const string ThemeRoot = "~/Themes/";
        private const string SiteRoot = "~/Sites/";
        private const string ConfigName = "theme.config";


        /// <summary>
        /// 获取所有的主题信息
        /// </summary>
        /// <returns></returns>
        public IList<theme> List()
        {
            List<theme> result = new List<theme>();

            //公共主题
            IList<theme> sharedTheme = SharedThemeList();

            //专用主题
            IList<theme> specialTheme = new List<theme>();


            foreach (string d in Directory.GetDirectories(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(SiteRoot), ThemeService.DomainToDirectoryName(SiteService.CurrentSite().domain))))
            {
                string c = Path.Combine(d, ConfigName);
                if (File.Exists(c))
                {
                    theme t = theme.Load(c);
                    t.name = "（自定义）" + t.name;

                    specialTheme.Add(t);
                }
            }

            //从公共主题中去掉专用主题中已存在的主题
            sharedTheme = sharedTheme.Where(m => !specialTheme.Select(x => x.dirname).Contains(m.dirname))
                                     .ToList();

            result.AddRange(sharedTheme);
            result.AddRange(specialTheme);

            return result;
        }

        /// <summary>
        /// 所有共享主题
        /// </summary>
        /// <returns></returns>
        public IList<theme> SharedThemeList()
        {
            List<theme> result = new List<theme>();

            foreach (string d in Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath(ThemeRoot)))
            {
                string c = Path.Combine(d, ConfigName);
                if (File.Exists(c))
                {
                    theme t = theme.Load(c);

                    if (t.name.Trim() == "默认主题")
                    {
                        result.Insert(0, t);
                    }
                    else
                    {
                        result.Add(t);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取某个主题
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        public theme Get(string dirname)
        {
            return List().FirstOrDefault(m => string.Compare(m.dirname, dirname, true) == 0);
        }

        /// <summary>
        /// 获取当前站点的默认主题
        /// </summary>
        /// <returns></returns>
        public theme GetDefault()
        {
            return Get(R.site.theme);
        }

        /// <summary>
        /// 根据主题名称获取某个主题的觉得路径
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        public string GetThemePath(string dirname)
        {
            theme theme = List().FirstOrDefault(m => string.Compare(m.dirname, dirname, true) == 0);

            string dir = Path.GetDirectoryName(theme.configfile);

            return dir;
        }

        /// <summary>
        /// 获取某个主题所有可编辑文件
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        public List<string> GetThemeFiles(string dirname)
        {
            List<string> filePaths = new List<string>();

            string dir = GetThemePath(dirname);

            foreach (string p in Directory.GetFiles(dir, "*.cshtml", SearchOption.AllDirectories))
            {
                filePaths.Add(p.Replace(dir, ""));
            }
            foreach (string p in Directory.GetFiles(dir, "*.css", SearchOption.AllDirectories))
            {
                filePaths.Add(p.Replace(dir, ""));
            }
            foreach (string p in Directory.GetFiles(dir, "*.js", SearchOption.AllDirectories))
            {
                filePaths.Add(p.Replace(dir, ""));
            }

            return filePaths;
        }

        /// <summary>
        /// 获取主题文件的内容
        /// </summary>
        /// <param name="dirname"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetThemeFileContent(string dirname, string file)
        {
            string result = string.Empty;

            theme t = Get(dirname);

            if (null == t)
            {
                throw new QRunException("主题“" + dirname + "”，不存在。");
            }

            List<string> themeFiles = GetThemeFiles(t.dirname);

            string path = themeFiles.FirstOrDefault(m => string.Compare(m, file, true) == 0);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new QRunException("找不到指定的主题文件。");
            }

            string fullfile = Path.Combine(Path.GetDirectoryName(t.configfile), path.TrimStart(Path.DirectorySeparatorChar));

            if (!File.Exists(fullfile))
            {
                throw new QRunException("指定的主题文件：" + file + "不存在。");
            }

            using (StreamReader r = new StreamReader(fullfile))
            {
                result = r.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// 编辑指定主题指定文件的内容
        /// </summary>
        /// <param name="dirname"></param>
        /// <param name="file"></param>
        /// <param name="content"></param>
        public void ModifyThemeFileContent(string dirname, string file, string content)
        {
            string result = string.Empty;

            theme t = Get(dirname);

            if (null == t)
            {
                throw new QRunException("主题“" + dirname + "”，不存在。");
            }

            List<string> themeFiles = GetThemeFiles(t.dirname);

            string path = themeFiles.FirstOrDefault(m => string.Compare(m, file, true) == 0);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new QRunException("找不到指定的主题文件。");
            }

            string fullfile = Path.Combine(Path.GetDirectoryName(t.configfile), path.TrimStart(Path.DirectorySeparatorChar));

            if (!File.Exists(fullfile))
            {
                throw new QRunException("指定的主题文件：" + file + "不存在。");
            }

            using (StreamWriter r = new StreamWriter(fullfile, false))
            {
                r.Write(content);
            }
        }

        /// <summary>
        /// 将域名转换为目录名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string DomainToDirectoryName(string domain)
        {
            return domain.Replace(":", "_") + "/";
        }

        /// <summary>
        /// 某个指定的主题是否存在
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        public bool ThemeExists(string dirname)
        {
            if(string.IsNullOrWhiteSpace(dirname))
            {
                return false;
            }

            return List().Any(m => string.Compare(m.dirname, dirname, true) == 0);
        }
    }
}