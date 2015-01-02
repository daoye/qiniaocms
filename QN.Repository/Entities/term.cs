using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("分类名称")]
        [QStringLength(50)]
        public virtual string name { get; set; }

        /// <summary>
        /// 分类别名
        /// </summary>
        [DisplayName("分类别名")]
        [QStringLength(50)]
        public virtual string slug { get; set; }

        /// <summary>
        /// 分类说明
        /// </summary>
        [DisplayName("分类说明")]
        public virtual string info { get; set; }

        /// <summary>
        /// 父级分类
        /// </summary>
        [DisplayName("父级分类")]
        public virtual int parent { get; set; }

        /// <summary>
        /// 分类类型
        /// </summary>
        public virtual string type { get; set; }

        /// <summary>
        /// 分类图片
        /// </summary>
        [DisplayName("分类图片")]
        public virtual string pic { get; set; }

        /// <summary>
        /// 文章数量
        /// </summary>
        public virtual int count { get; set; }

        /// <summary>
        /// 顺序编号
        /// </summary>
        [DisplayName("顺序编号")]
        public virtual int order { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DisplayName("创建日期")]
        public virtual DateTime date { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        [DisplayName("修改日期")]
        public virtual DateTime modified { get; set; }

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
