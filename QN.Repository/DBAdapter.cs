using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository
{
    public class DBAdapter
    {
        /// <summary>
        /// 排序表达式
        /// </summary>
        public static string randExpression
        {
            get
            {
                switch (SessionFactory.Instance.DBType)
                {
                    case DBType.SQLite:
                        return "random() asc";
                    case DBType.MySql:
                        return "rand() asc";
                    case DBType.MSSql:
                        return "newid() asc";
                    default:
                        throw new InvalidOperationException("未实现此类型数据库的随机排序功能。");
                }
            }
        }
    }
}
