using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBType
    {
        /// <summary>
        /// Sql server
        /// </summary>
        MSSql = 0,

        /// <summary>
        /// Mysql
        /// </summary>
        MySql = 1,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 2,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL = 3,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite = 4,

        /// <summary>
        /// FireBird
        /// </summary>
        FireBird = 5
    }

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
