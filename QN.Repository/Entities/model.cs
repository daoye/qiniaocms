using QN.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QN
{
    /// <summary>
    /// 模型基类，所有数据模型，应该从此类派生
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class model<T> where T : class
    {
        /// <summary>
        /// 从另一个实体对象中将所有属性值拷贝到本对象中
        /// </summary>
        /// <param name="src">源实体</param>
        /// <param name="filterProperty">复制过程中跳过这些指定的属性</param>
        /// <returns></returns>
        public virtual void AssigningForm(T src, params string[] filterProperty)
        {
            Type me = this.GetType();
            foreach (PropertyInfo srcP in typeof(T).GetProperties())
            {
                if (filterProperty.Contains(srcP.Name))
                {
                    continue;
                }

                PropertyInfo p = me.GetProperty(srcP.Name);

                if (null != p)
                {
                    p.SetValue(this, srcP.GetValue(src, null), null);
                }
            }
        }

        /// <summary>
        /// 将此对象序列化为json字符串
        /// </summary>
        public virtual string ToJson()
        {
            return QJson.Serialize(this);
        }

        /// <summary>
        /// 通过json字符串实例化此对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T FromJson(string json)
        {
            return QJson.Deserialize<T>(json);
        }
    }
}