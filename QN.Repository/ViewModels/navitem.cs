using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 表示菜单信息
    /// </summary>
    public class navitem: model<navitem>
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string slug { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 顺序编号
        /// </summary>
        public int order { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string itemid { get; set; }

        /// <summary>
        /// 父级id
        /// </summary>
        public string parentid { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        public int deep { get; set; }
    }
}
