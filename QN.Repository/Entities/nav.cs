using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 导航，菜单
    /// </summary>
    public class nav : entity<nav>
    {
        /// <summary>
        /// 网站ID
        /// </summary>
        public virtual int siteid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public virtual string url { get; set; }

        /// <summary>
        /// 顺序编号
        /// </summary>
        public virtual int order { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public virtual int parent { get; set; }

        /// <summary>
        /// 图标
        /// </summary>

        public virtual string icon { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public virtual string type { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        public virtual int deep { get; set; }

        /// <summary>
        /// 层级路径
        /// </summary>
        public virtual string deeppath { get; set; }
    }
}
