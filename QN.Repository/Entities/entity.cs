using QN.Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 实体基类，所有数据库映射实体，应从此类派生
    /// </summary>
    public abstract class entity<T> : model<T>, IAggregateRoot where T : class
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual int id { get; set; }
    }
}