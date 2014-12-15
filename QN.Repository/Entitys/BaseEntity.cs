using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Sites.Repository
{
    public abstract class BaseEntity<Tid>
    {
        public Tid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 获取附加属性
        /// </summary>
        /// <param name="key">属性的名称</param>
        /// <returns></returns>
        public virtual string Meta(string key)
        {
            return null;
        }

        /// <summary>
        /// 获取附加属性
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="key">属性的名称</param>
        /// <returns></returns>
        public T Meta<T>(string key) where T : class
        {
            return Meta(key) as T;
        }

        /// <summary>
        /// 设置附加属性
        /// </summary>
        /// <param name="key">属性的名称</param>
        /// <param name="value">属性的值</param>
        /// <returns></returns>
        public virtual void Meta(string key, string value) { }

        /// <summary>
        /// 获取所有的附加属性
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<BaseMeta> DisplayMetas()
        {
            return null;
        }
    }
}
