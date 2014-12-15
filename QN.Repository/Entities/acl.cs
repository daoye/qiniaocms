using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 权限
    /// </summary>
    public class acl : entity<acl>
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public virtual int roleid { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        public virtual string value { get; set; }
    }
}
