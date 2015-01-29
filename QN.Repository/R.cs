using NHibernate;
using NHibernate.Criterion;
using QN.Repository;
using System;
using System.Collections.Generic;
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
        /// 当前站点的基本信息
        /// </summary>
        public static site site
        {
            get
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

                    site site = result.SingleOrDefault(m => m.domain.Split('\n').Any(x => string.Compare(x, reqdomain, true) == 0));
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

                throw new InvalidOperationException("没有找到站点配置信息。");
            }
        }

        /// <summary>
        /// 当前发起请求的域对应的站点ID
        /// </summary>
        public static int siteid
        {
            get
            {
                return site.id;
            }
        }

        /// <summary>
        /// 表示所有站点的ID
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
        /// 默认导航的ID
        /// </summary>
        public const string default_nav_id = "default_nav_id";

        /// <summary>
        /// 发布状态的post
        /// </summary>
        public const string post_type_publish = "publish";

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
    }
}
