using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 评论
    /// </summary>
    public class comment : entity<comment>, IMeta
    {
        /// <summary>
        /// Post ID
        /// </summary>
        public virtual int postid { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public virtual string author { get; set; }

        /// <summary>
        /// 作者联系邮箱
        /// </summary>
        public virtual string authoremail { get; set; }

        /// <summary>
        /// 作者主页
        /// </summary>
        public virtual string authorurl { get; set; }

        /// <summary>
        /// 作者IP
        /// </summary>
        public virtual string authorip { get; set; }

        /// <summary>
        /// 评论日期
        /// </summary>
        public virtual DateTime date { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public virtual string content { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public virtual int parent { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public virtual string status { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public virtual int memberid { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        public virtual int deep { get; set; }

        /// <summary>
        /// 层级路径
        /// </summary>
        public virtual string deeppath { get; set; }

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
