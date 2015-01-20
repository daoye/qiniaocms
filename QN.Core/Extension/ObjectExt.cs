using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QN
{
    public static class ObjectExt
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T Cast<T>(this object s)
        {
            return QTypeBuilder<T>.Unbox(s);
        }

        /// <summary>
        /// 类型是否为指定类型的子类，支持空泛型
        /// </summary>
        /// <param name="toCheck"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string DisplayName(this object obj)
        {
            object[] result = obj.GetType().GetCustomAttributes(typeof(QDisplayNameAttribute), false);

            if (null != result && result.Length > 0)
            {
                string name = (result[0] as QDisplayNameAttribute).DisplayName;

                if (null != name)
                {
                    return QLang.Instance().Lang(name);
                }
            }

            return string.Empty;
        }
    }
}