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
                IEnumerable<site> result = QCache.Get<IEnumerable<site>>("site-list");

                if (null == result)
                {
                    result = session.CreateCriteria<site>().List<site>();

                    QCache.Set(null, "site-list", result, 60);
                }

                site site = result.SingleOrDefault(m => string.Compare(m.domain, HttpContext.Current.Request.Url.Authority, true) == 0);
                if (null != site)
                {
                    return site;
                }
                else
                {
                    throw new InvalidOperationException("没有找到站点配置信息。");
                }
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
    }
}
