using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 内容
    /// </summary>
    public class post : entity<post>, IMeta
    {
        /// <summary>
        /// 网站ID
        /// </summary>
        public virtual int siteid { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [DisplayName("作者")]
        [QStringLength(100)]
        public virtual string author { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public virtual DateTime date { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        [DisplayName("内容")]
        public virtual string content { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [DisplayName("标题")]
        public virtual string title { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [DisplayName("摘要")]
        public virtual string excerpt { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public virtual string status { get; set; }

        /// <summary>
        /// ping通告状态
        /// </summary>
        public virtual string pingstatus { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("名称")]
        public virtual string name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [DisplayName("别名")]
        public virtual string slug { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public virtual DateTime modified { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [DisplayName("父页面")]
        public virtual int parent { get; set; }

        /// <summary>
        ///  内容类型
        /// </summary>
        public virtual string posttype { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public virtual int commentcount { get; set; }

        /// <summary>
        /// 浏览数
        /// </summary>
        public virtual int viewcount { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        [DisplayName("顺序编号")]
        public virtual int order { get; set; }

        /// <summary>
        /// 媒体类型
        /// </summary>
        public virtual string mimetype { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        public virtual string pic { get; set; }

        /// <summary>
        /// 封面图片链接地址
        /// </summary>
        [DisplayName("链接地址")]
        public virtual string piclink { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        [DisplayName("分类")]
        public virtual int termid { get; set; }

        /// <summary>
        /// 文件PostID
        /// </summary>
        public virtual int filepostid { get; set; }

        public virtual string meta(string property)
        {
            throw new NotImplementedException();
        }

        public virtual void meta(string property, string value)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<string> displaymeta()
        {
            throw new NotImplementedException();
        }
    }
}
