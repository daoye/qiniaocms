#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	Map.cs
	Author:		DaoYe
	History: 	30/11/2014 18:57 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using QN;
using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace QN
{
    /// <summary>
    /// 一些键值映射辅助类
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// 子站列表缓存Key
        /// </summary>
        public const string SiteListKey = "site-list";

        /// <summary>
        /// 后台菜单缓存Key
        /// </summary>
        public const string CarteListKey = "menu-list";

        /// <summary>
        /// 当前发起请求的域对应的站点ID
        /// </summary>
        public static int SiteId
        {
            get
            {
                IEnumerable<Site> result = QCache.Get<IEnumerable<Site>>(SiteListKey);

                if (null == result)
                {
                    result = DB.Context.Site.Where(m => m.Domain != null && m.Domain != string.Empty);

                    QCache.Set(null, SiteListKey, result, 60);
                }

                Site site = result.SingleOrDefault(m => string.Compare(m.Domain, HttpContext.Current.Request.Url.Authority, true) == 0);
                if (null != site)
                {
                    return site.Id;
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
        public const int GlobalSiteId = -1;

        /// <summary>
        /// 当前程序是否处于调试阶段
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                string debug = WebConfigurationManager.AppSettings["isdebug"];

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
        /// 评论状态
        /// </summary>
        public enum CommentStatus
        {
            /// <summary>
            /// 未审核
            /// </summary>
            NoAudit = 0,
            /// <summary>
            /// 通过
            /// </summary>
            Pass = 1,

            /// <summary>
            /// 垃圾
            /// </summary>
            Trash = 2
        }

        /// <summary>
        /// Post类型
        /// </summary>
        public enum PostType
        {
            /// <summary>
            /// 文章
            /// </summary>
            Article = 0,
            /// <summary>
            /// 单页
            /// </summary>
            Page = 1,
            /// <summary>
            /// 文件
            /// </summary>
            Media = 2
        }
    }
}
