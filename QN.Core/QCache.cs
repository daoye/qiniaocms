#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	QCache.cs
	Author:		DaoYe
	History: 	30/11/2014 18:56 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace QN
{
    /// <summary>
    /// 缓存辅助类
    /// </summary>
    public static class QCache
    {
        /// <summary>
        /// 获取一个唯一标识符
        /// </summary>
        public static string GetID
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// 添加一个永不过期的缓存信息（如果已存在指定ID的缓存，将被覆盖）
        /// </summary>
        /// <param name="key">键 可以使用GetID<see cref="GetID"/>获取一个唯一的键。</param>
        /// <param name="value">值</param>
        public static void Set(string key, object value)
        {
            Set(null, key, value);
        }

        /// <summary>
        /// 添加一个永不过期的缓存信息（如果已存在指定ID的缓存，将被覆盖）
        /// </summary>
        /// <param name="prefix">使用一个特定的前缀用来区分可能存在相同Key的缓存信息</param>
        /// <param name="key">键 可以使用GetID<see cref="GetID"/>获取一个唯一的键。</param>
        /// <param name="value">值</param>
        public static void Set(string prefix, string key, object value)
        {
            if (null != value)
            {
                HttpRuntime.Cache.Insert(CombeID(key, prefix), value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }
        }

        /// <summary>
        /// 添加一个添加一个在指定时间段内没有被访问就自动删除的缓存（如果已存在指定ID的缓存，将被覆盖）
        /// </summary>
        /// <param name="prefix">使用一个特定的前缀用来区分可能存在相同Key的缓存信息</param>
        /// <param name="key">键 可以使用GetID<see cref="GetID"/>获取一个唯一的键。</param>
        /// <param name="value">值</param>
        /// <param name="expries">将在x分钟后过期（如果在这段时间内没有被使用的话）</param>
        public static void Set(string prefix, string key, object value, int expries)
        {
            Set(prefix, key, value, expries, null);
        }

        /// <summary>
        /// 添加一个添加一个在指定时间段内没有被访问就自动删除的缓存（如果已存在指定ID的缓存，将被覆盖）
        /// </summary>
        /// <param name="key">键 可以使用GetID<see cref="GetID"/>获取一个唯一的键。</param>
        /// <param name="value">值</param>
        /// <param name="expries">将在x分钟后过期（如果在这段时间内没有被使用的话）</param>
        /// <param name="onRemoveCallback">在从缓存中移除对象时所调用的委托（如果提供）。当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        public static void Set(string key, object value, int expries, CacheItemRemovedCallback onRemoveCallback)
        {
            Set(null, key, value, expries, null);
        }

        /// <summary>
        /// 添加一个添加一个在指定时间段内没有被访问就自动删除的缓存（如果已存在指定ID的缓存，将被覆盖）
        /// </summary>
        /// <param name="prefix">使用一个特定的前缀用来区分可能存在相同Key的缓存信息</param>
        /// <param name="key">键 可以使用GetID<see cref="GetID"/>获取一个唯一的键。</param>
        /// <param name="value">值</param>
        /// <param name="expries">将在x分钟后过期（如果在这段时间内没有被使用的话）</param>
        /// <param name="onRemoveCallback">在从缓存中移除对象时所调用的委托（如果提供）。当从缓存中删除应用程序的对象时，可使用它来通知应用程序。</param>
        public static void Set(string prefix, string key, object value, int expries, CacheItemRemovedCallback onRemoveCallback)
        {
            if(null!= value)
            {
                HttpRuntime.Cache.Insert(CombeID(key, prefix), value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(expries), CacheItemPriority.NotRemovable, onRemoveCallback);
            }
        }

        /// <summary>
        /// 从缓存中获取值。
        /// </summary>
        /// <param name="key">要获取的缓存的Key</param>
        /// <returns>检索到的缓存项，未找到该键时为 null。</returns>
        public static Object Get(string key)
        {
            return Get(null, key);
        }

        /// <summary>
        /// 从缓存中获取值。
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="key">要获取的缓存的Key</param>
        /// <returns>检索到的缓存项，未找到该键时为 null。</returns>
        public static T Get<T>(string key) where T : class
        {
            return Get(null, key) as T;
        }

        /// <summary>
        /// 从缓存中获取值。
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="prefix">使用一个特定的前缀用来区分可能存在相同Key的缓存信息</param>
        /// <param name="key">要获取的缓存的Key</param>
        /// <returns>检索到的缓存项，未找到该键时为 null。</returns>
        public static T Get<T>(string prefix, string key) where T : class
        {
            return Get(prefix, key) as T;
        }

        /// <summary>
        /// 从缓存中获取值。
        /// </summary>
        /// <param name="prefix">使用一个特定的前缀用来区分可能存在相同Key的缓存信息</param>
        /// <param name="key">要获取的缓存的Key</param>
        /// <returns>检索到的缓存项，未找到该键时为 null。</returns>
        public static Object Get(string prefix, string key)
        {
            return HttpRuntime.Cache.Get(CombeID(key, prefix));
        }

        /// <summary>
        /// 从缓存中删除指定ID标记的缓存信息
        /// </summary>
        /// <param name="key">指定的ID</param>
        public static void Remove(string key)
        {
            Remove(null, key);
        }

        /// <summary>
        /// 从缓存中删除指定ID标记的缓存信息
        /// </summary>
        /// <param name="prefix">使用一个特定的前缀用来区分可能存在相同Key的缓存信息</param>
        /// <param name="key">指定的ID</param>
        public static void Remove(string prefix, string key)
        {
            HttpRuntime.Cache.Remove(CombeID(key, prefix));
        }

        /// <summary>
        /// 将ID和网站ID进行组合
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        private static string CombeID(string id, string id2)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(id2))
            {
                id2 = string.Empty;
            }

            return id + id2;
        }
    }
}