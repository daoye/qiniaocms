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
        /// 当前发起请求的域对应的站点ID
        /// </summary>
        public static int siteid
        {
            get
            {
                IEnumerable<site> result = QCache.Get<IEnumerable<site>>("site-list");

                if (null == result)
                {
                    result = session.CreateCriteria<site>().Add(Expression.IsNotEmpty("domain")).List<site>();

                    QCache.Set(null, "site-list", result, 60);
                }

                site site = result.SingleOrDefault(m => string.Compare(m.domain, HttpContext.Current.Request.Url.Authority, true) == 0);
                if (null != site)
                {
                    return site.id;
                }
                else
                {
                    throw new InvalidOperationException("没有找到站点配置信息。");
                }
            }
        }

        /// <summary>
        /// 表示所有站点的ID
        /// </summary>
        public const int global_siteid = -1;
    }
}
