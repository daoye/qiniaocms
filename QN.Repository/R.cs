using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using QN.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace QN
{
    /// <summary>
    /// 辅助类
    /// </summary>
    public static class R
    {
        /// <summary>
        /// Nhibernate会话
        /// </summary>
        public static ISession session
        {
            get
            {
                return SessionFactory.Instance.GetSession();
            }
        }

        /// <summary>
        /// 当前网站的基本信息
        /// </summary>
        public static site site
        {
            get
            {
                try
                {
                    site _s = System.Web.HttpContext.Current.Items["siteinfo"] as site;

                    if (null == _s)
                    {
                        IEnumerable<site> result = QCache.Get<IEnumerable<site>>("site-list");

                        if (null == result)
                        {
                            result = session.CreateCriteria<site>().List<site>();

                            QCache.Set(null, "site-list", result, 60);
                        }

                        string reqdomain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;

                        site site = result.SingleOrDefault(m => m.domain.Split(';').Any(x => string.Compare(x.Trim(), reqdomain, true) == 0));
                        if (null != site)
                        {
                            _s = site;
                            System.Web.HttpContext.Current.Items["siteinfo"] = site;
                        }
                    }

                    if (null != _s)
                    {
                        return _s;
                    }

                    throw new InvalidOperationException("没有找到网站配置信息。");
                }
                catch (GenericADOException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 当前发起请求的域对应的网站ID
        /// </summary>
        public static int siteid
        {
            get
            {
                return site.id;
            }
        }

        /// <summary>
        /// 表示所有网站的ID
        /// </summary>
        public const int global_siteid = -1;

        /// <summary>
        /// 网站根目录
        /// </summary>
        public static string root
        {
            get
            {
                string path = HttpContext.Current.Request.ApplicationPath;

                if(!path.EndsWith("/"))
                {
                    path += "/";
                }

                return path;
            }
        }

        #region 全局变量

        /// <summary>
        /// 发布状态
        /// </summary>
        public const string status_publish = "publish";

        /// <summary>
        /// 草稿状态
        /// </summary>
        public const string status_trash = "trash";

        /// <summary>
        /// 用户状态，正常
        /// </summary>
        public const string user_status_nomal = "nomal";

        /// <summary>
        /// 用户状态，禁止
        /// </summary>
        public const string user_status_ban = "ban";

        /// <summary>
        /// 默认导航的ID
        /// </summary>
        public const string default_nav_id = "default_nav_id";

        /// <summary>
        /// 默认分类ID
        /// </summary>
        public const string default_term_id = "super_term_id";

        /// <summary>
        /// 创始人用户ID
        /// </summary>
        public const string superuser_id = "superuser_id";

        /// <summary>
        /// 普通用户角色ID
        /// </summary>
        public const int role_user = 4;

        /// <summary>
        /// 编辑者角色ID
        /// </summary>
        public const int role_editor = 3;

        /// <summary>
        /// 管理员角色ID
        /// </summary>
        public const int role_manager = 2;

        /// <summary>
        /// 超级管理员角色ID
        /// </summary>
        public const int role_super = 1;

        #endregion

        #region 版本信息

        /// <summary>
        /// 程序主版本号
        /// </summary>
        public static int VersionMain { get { return 1; } }

        /// <summary>
        /// 程序次版本号
        /// </summary>
        public static int VersionMinor { get { return 0; } }

        /// <summary>
        /// 程序修订号
        /// </summary>
        public static int VersionUpdate { get { return 0; } }

        /// <summary>
        /// 版本类型(Dev 开发, Alpha 次要测试, Beta 主要测试, Major 主要)
        /// </summary>
        public static string VersionType { get { return "Alpha"; } }

        /// <summary>
        /// 版本序号
        /// </summary>
        public static string Version { get { return string.Format("{0}.{1}.{2} {3}", VersionMain, VersionMinor, VersionUpdate, VersionType); } }

        #endregion

        /// <summary>
        /// 网站是否已经安装
        /// </summary>
        public static bool Installed
        {
            get
            {
                return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/install.lock"));
            }
        }
    }
}
