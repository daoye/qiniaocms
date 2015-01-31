using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 角色
    /// </summary>
    public class role : entity<role>
    {
        /// <summary>
        /// 网站ID
        /// </summary>
        public virtual int siteid { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [DisplayName("角色名称")]
        [QRequired]
        public virtual string name { get; set; }
    }
}
