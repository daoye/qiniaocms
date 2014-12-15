#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	QSession.cs
	Author:		DaoYe
	History: 	30/11/2014 18:56 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// Session辅助类
    /// </summary>
    public static class QSession
    {
        /// <summary>
        /// 添加一个session，如果指定键值的session已存在，将覆盖该值
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <param name="value">值</param>
        public static void Set(string key, object value)
        {
            System.Web.HttpContext.Current.Session[key] = value;
        }

        /// <summary>
        /// 获取一个session，如果指定键值的session不存在，将返回null
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <returns>获取到的session值</returns>
        public static object Get(string key)
        {
            return System.Web.HttpContext.Current.Session[key];
        }

        /// <summary>
        /// 获取一个session，如果指定键值的session不存在，将返回null
        /// </summary>
        /// <typeparam name="T">指定的类型</typeparam>
        /// <param name="key">指定的键</param>
        /// <returns>获取到的session值</returns>
        public static T Get<T>(string key) where T : class
        {
            return Get(key) as T;
        }

        /// <summary>
        /// 移除指定键值的session
        /// </summary>
        /// <param name="key">指定的键</param>
        public static void Remove(string key)
        {
            System.Web.HttpContext.Current.Session.Remove(key);
        }

        /// <summary>
        /// 清除所有的session
        /// </summary>
        public static void Clear()
        {
            System.Web.HttpContext.Current.Session.Clear();
        }

        /// <summary>
        /// 是否包含指定键的session
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <returns>一个布尔类型的值，true表示含有指定键的session</returns>
        public static bool HasKey(string key)
        {
            return System.Web.HttpContext.Current.Session[key] != null;
        }
    }
}
