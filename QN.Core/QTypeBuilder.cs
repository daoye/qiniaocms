using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QN
{
    /// <summary>
    /// 类型构造器
    /// </summary>
    public static class QTypeBuilder<T>
    {
        /// <summary>
        /// 用于拆箱
        /// </summary>
        public readonly static Converter<object, T> Unbox;

        static QTypeBuilder()
        {
            QTypeBuilder<T>.Unbox = QTypeBuilder<T>.Create(typeof(T));
        }

        private static Converter<object, T> Create(Type type)
        {
            if (!type.IsValueType)
            {
                return new Converter<object, T>(QTypeBuilder<T>.ReferenceField);
            }
            if (!type.IsGenericType || type.IsGenericTypeDefinition || !(typeof(Nullable<>) == type.GetGenericTypeDefinition()))
            {
                return new Converter<object, T>(QTypeBuilder<T>.ValueField);
            }
            Type type1 = typeof(Converter<object, T>);
            MethodInfo method = typeof(QTypeBuilder<T>).GetMethod("NullableField", BindingFlags.Static | BindingFlags.NonPublic);
            Type[] genericArguments = new Type[] { type.GetGenericArguments()[0] };
            return (Converter<object, T>)Delegate.CreateDelegate(type1, method.MakeGenericMethod(genericArguments));
        }

        private static Nullable<TElem> NullableField<TElem>(object value)
        where TElem : struct
        {
            if (DBNull.Value == value)
            {
                return null;
            }
            return new Nullable<TElem>((TElem)value);
        }

        private static T ReferenceField(object value)
        {
            if (DBNull.Value != value)
            {
                return (T)value;
            }
            return default(T);
        }

        private static T ValueField(object value)
        {
            if (DBNull.Value == value)
            {
                throw new InvalidCastException("Can't cast value to :" + typeof(T).ToString());
            }

            if (value is string)
            {
                value = Convert.ChangeType(value, typeof(T));
            }

            return (T)value;
        }
    }
}