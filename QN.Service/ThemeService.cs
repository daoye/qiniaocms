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

using NHibernate;
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
            IList<theme> specialTheme = PrivateThemeList();

            foreach (theme t in specialTheme)
            {
                t.name = QLang.Instance().Lang("（自定义）") + t.name;
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
        /// 当前网站的私有主题
        /// </summary>
        /// <returns></returns>
        public IList<theme> PrivateThemeList()
        {
            IList<theme> themes = new List<theme>();

            foreach (string d in Directory.GetDirectories(CurrentSiteThemePath))
            {
                string c = Path.Combine(d, ConfigName);
                if (File.Exists(c))
                {
                    theme t = theme.Load(c);
                    themes.Add(t);
                }
            }

            return themes;
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
        /// 设置默认主题
        /// </summary>
        /// <param name="dirname"></param>
        public void SetDefault(string dirname)
        {
            theme theme = List().FirstOrDefault(m => string.Compare(m.dirname, dirname, true) == 0);

            if (null != theme)
            {
                string themepath = Path.GetDirectoryName(theme.configfile);

                using (ITransaction trans = R.session.BeginTransaction())
                {
                    try
                    {
                        site site = R.site;
                        site.theme = theme.dirname;
                        R.session.Update(site);

                        SiteService.CreateTheme(site.firstdomain(), dirname);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
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
            if (string.IsNullOrWhiteSpace(dirname))
            {
                return false;
            }

            return List().Any(m => string.Compare(m.dirname, dirname, true) == 0);
        }

        /// <summary>
        /// 获取当前站点的主题安装目录
        /// </summary>
        public string CurrentSiteThemePath
        {
            get
            {
                return Path.Combine(System.Web.HttpContext.Current.Server.MapPath(SiteRoot), ThemeService.DomainToDirectoryName(R.site.firstdomain()));
            }
        }

        public void Remove(string dirname)
        {
            theme theme = List().FirstOrDefault(m => string.Compare(m.dirname, dirname, true) == 0);

            if (null != theme)
            {
                string themepath = Path.GetDirectoryName(theme.configfile);

                QFile.DeleteDirectory(themepath);
            }
        }

        public void LocalInstall(Stream stream, string filename)
        {
            string tempName = System.Guid.NewGuid().ToString() + ".zip";
            string fullPath = Path.Combine(QConfiger.TempPath, tempName);

            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
            }

            Install(fullPath);
        }

        private void Install(string path)
        {
            string unpackPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

            QZip.UnZip(path, unpackPath);

            File.Delete(path);

            FileInfo configFileInfo = new DirectoryInfo(unpackPath).GetFiles("theme.config", SearchOption.AllDirectories).FirstOrDefault();

            if (null == configFileInfo)
            {
                throw new QRunException("不是有效的主题包。");
            }

            theme theme = theme.Load(configFileInfo.FullName);

            IList<theme> installedThemes = List();

            if (installedThemes.Any(m => m.dirname == theme.dirname))
            {
                throw new QRunException("已安装过相同的主题，如果要重新安装，请先删除旧主题。");
            }

            string themePath = Path.Combine(CurrentSiteThemePath, theme.dirname);

            QFile.DeepCopy(themePath, configFileInfo.DirectoryName);

            QFile.DeleteDirectory(unpackPath);
        }
    }
}