using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 配置信息辅助类
    /// </summary>
    public static class opt
    {
        /// <summary>
        /// 获取指定网站的某个配置信息
        /// </summary>
        /// <param name="siteid">网站ID</param>
        /// <param name="name">键名</param>
        /// <returns></returns>
        public static string get(int siteid, string name)
        {
            IList<option> options = R.session.CreateCriteria<option>()
                                              .Add(Expression.Eq("name", name))
                                              .Add(Expression.Eq("siteid", siteid))
                                              .List<option>();

            option result = options.FirstOrDefault();

            if (null == result)
            {
                return string.Empty;
            }

            return result.value;
        }

        /// <summary>
        /// 获取指定网站的某个配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="siteid">网站ID</param>
        /// <param name="name">键名</param>
        /// <returns></returns>
        public static T get<T>(int siteid, string name)
        {
            string result = get(siteid, name);

            if(string.IsNullOrWhiteSpace(result))
            {
                return default(T);
            }

            return QTypeBuilder<T>.Unbox(result);
        }

        /// <summary>
        /// 为某个指定网站添加配置信息
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void set(int siteid, string name, string value)
        {
            IList<option> options = R.session.CreateCriteria<option>()
                                             .Add(Expression.Eq("name", name))
                                             .Add(Expression.Eq("siteid", siteid))
                                             .List<option>();

            option o = options.FirstOrDefault();

            if (o == null)
            {
                o = new option()
                {
                    siteid = siteid,
                    name = name
                };
            }

            o.value = value;

            R.session.SaveOrUpdate(o);
            R.session.Flush();
        }

        /// <summary>
        /// 获取全局配置信息
        /// </summary>
        /// <param name="name">键名</param>
        /// <returns></returns>
        public static string getglobal(string name)
        {
            return get(0, name);
        }

        /// <summary>
        /// 获取全局配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">键名</param>
        /// <returns></returns>
        public static T getglobal<T>(string name)
        {
            return get<T>(0, name);
        }

        /// <summary>
        /// 设置全局信息
        /// </summary>
        /// <param name="name">键名</param>
        /// <param name="value">值</param>
        public static void setglobal(string name, string value)
        {
            set(0, name, value);
        }
    }
}
