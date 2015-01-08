using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Service
{
    public class OptionService
    {
        /// <summary>
        /// 获取指定网站的某个配置信息
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public option Get(int siteid, string name)
        {
            IList<option> options = R.session.CreateCriteria<option>()
                                              .Add(Expression.Eq("name", name))
                                              .Add(Expression.Eq("siteid", siteid))
                                              .List<option>();

            return options.FirstOrDefault();
        }

        /// <summary>
        /// 获取当前网站的某个配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public option Get(string name)
        {
            return Get(R.siteid, name);
        }

        /// <summary>
        /// 获取当前网站的某个配置信息的值，如果没有此值，则返回空字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetValue(string name)
        {
            return GetValue(R.siteid, name);
        }

        /// <summary>
        /// 获取指定网站的摸个配置信息的值，如果没有此值，则返回空字符串
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetValue(int siteid, string name)
        {
            option o = Get(siteid, name);
            if (null != o)
            {
                return o.value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 为某个指定网站添加配置信息（包含事物）
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(int siteid, string name, string value)
        {
            using(ITransaction trans = R.session.BeginTransaction())
            {
                try
                {
                    SetNoTrans(siteid, name, value);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 为某个指定网站添加配置信息，不包含事物
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetNoTrans(int siteid, string name, string value)
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
        }


        /// <summary>
        /// 为当前网站添加配置信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(string name, string value)
        {
            Set(R.siteid, name, value);
        }

        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="o"></param>
        public void Set(option o)
        {
            R.session.SaveOrUpdate(o);
        }
    }
}
