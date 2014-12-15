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
        public IEnumerable<theme> List()
        {
            List<theme> result = new List<theme>();

            //公共主题
            List<theme> sharedTheme = new List<theme>();

            //专用主题
            List<theme> specialTheme = new List<theme>();

            foreach (string d in Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath(ThemeRoot)))
            {
                string c = Path.Combine(d, ConfigName);
                if (File.Exists(c))
                {
                    theme t = theme.Load(c);

                    if (t.name.Trim() == "默认主题")
                    {
                        sharedTheme.Insert(0, t);
                    }
                    else
                    {
                        sharedTheme.Add(t);
                    }
                }
            }

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
        /// 将域名转换为目录名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string DomainToDirectoryName(string domain)
        {
            return domain.Replace(":", "_") + "/";
        }
    }
}
