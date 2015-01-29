using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public interface IMeta<T>
    {
        /// <summary>
        /// 获取实体的扩展属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property">属性名称</param>
        /// <returns></returns>
        string meta(string property);

        /// <summary>
        /// 设置实体的扩展属性值
        /// </summary>
        /// <param name="entity">将被设置属性的实体对象</param>
        /// <param name="property">属性名称</param>
        /// <param name="value">属性值</param>
        void meta(string property, string value);

        /// <summary>
        /// 获取所有Meta数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> metas();
    }
}
