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
        /// 菜单ID
        /// </summary>
        public virtual int carteid { get; set; }
    }
}
