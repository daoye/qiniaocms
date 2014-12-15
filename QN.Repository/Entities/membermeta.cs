using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// Members的扩展属性
    /// </summary>
    public class membermeta : entity<membermeta>
    {
        /// <summary>
        /// Member ID
        /// </summary>
        public virtual int memberid { get; set; }
        
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
