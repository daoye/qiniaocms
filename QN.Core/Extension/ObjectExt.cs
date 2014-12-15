using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public static class ObjectExt
    {
        public static string ToEmptyString(this object s)
        {
            if (s == null) return "";

            return s.ToString();
        }

        public static int ToInt32(this object s)
        {
            if (s == null) return 0;

            int result = 0;
            Int32.TryParse(s.ToString(), out result);

            return result;
        }

        public static decimal ToDecimal(this object s)
        {
            if (s == null) return 0;

            decimal result = 0;
            Decimal.TryParse(s.ToString(), out result);

            return result;
        }

        public static DateTime ToDateTime(this object s)
        {
            if (s == null) return DateTime.Now;

            DateTime result = new DateTime();
            DateTime.TryParse(s.ToString(), out result);

            return result;
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
    }
}
