using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 分类
    /// </summary>
    public class term : entity<term>
    {
        /// <summary>
        /// 网站ID
        /// </summary>
        public virtual int siteid { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public virtual string slug { get; set; }

        /// <summary>
        /// 分类说明
        /// </summary>
        public virtual string info { get; set; }

        /// <summary>
        /// 父级分类
        /// </summary>
        public virtual int parent { get; set; }

        /// <summary>
        /// 分类类型
        /// </summary>
        public virtual string type { get; set; }

        /// <summary>
        /// 分类图片
        /// </summary>
        public virtual string pic { get; set; }

        /// <summary>
        /// 文章数量
        /// </summary>
        public virtual int count { get; set; }

        /// <summary>
        /// 顺序编号
        /// </summary>
        public virtual int order { get; set; }

        /// <summary>
        ///  深度
        /// </summary>
        public virtual int deep { get; set; }

        /// <summary>
        /// 深度路径
        /// </summary>
        public virtual string deeppath { get; set; }

        /// <summary>
        /// 是否是默认分类
        /// </summary>
        public virtual bool super { get; set; }
    }
}
