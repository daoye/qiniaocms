using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// Posts的扩展属性
    /// </summary>
    public class postmeta : entity<postmeta>
    {
        /// <summary>
        /// Post ID
        /// </summary>
        public virtual int postid { get; set; }
        
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
