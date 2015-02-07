using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class carte : entity<carte>
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// 网站ID
        /// </summary>
        public virtual int siteid { get; set; }

        /// <summary>
        /// Action名称
        /// </summary>
        public virtual string action { get; set; }

        /// <summary>
        /// Controller名称
        /// </summary>
        public virtual string controller { get; set; }

        /// <summary>
        /// Area名称
        /// </summary>
        public virtual string area { get; set; }

        /// <summary>
        /// 父级菜单ID
        /// </summary>
        public virtual int parent { get; set; }

        /// <summary>
        /// 顺序编号
        /// </summary>
        public virtual int order { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public virtual string icon { get; set; }

        /// <summary>
        /// 其他允许访问的action
        /// </summary>
        public virtual string allowactions { get; set; }

        /// <summary>
        /// 标志菜单是否激活的标识符
        /// </summary>
        public virtual string activeflag { get; set; }
    }
}
