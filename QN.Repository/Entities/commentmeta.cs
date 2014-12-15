using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// Comments的扩展属性
    /// </summary>
    public class commentmeta : entity<commentmeta>
    {
        /// <summary>
        /// Comment ID
        /// </summary>
        public virtual int commentid { get; set; }
        
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
