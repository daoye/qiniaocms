using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// Site的扩展属性
    /// </summary>
    public class sitemeta : entity<sitemeta>
    {
        /// <summary>
        /// Site ID
        /// </summary>
        public virtual int siteid { get; set; }
        
        /// <summary>
        /// 键
        /// </summary>
        public virtual string key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public virtual string value { get; set; }
    }
}
