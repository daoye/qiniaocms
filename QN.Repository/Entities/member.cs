using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class member : entity<member>, IMeta
    {
        /// <summary>
        /// 网站id
        /// </summary>
        public virtual int siteid { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public virtual string login { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public virtual string pass { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string nicename { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public virtual string email { get; set; }

        /// <summary>
        /// 用户主页
        /// </summary>
        public virtual string url { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        public virtual DateTime registered { get; set; }

        /// <summary>
        /// 登录日期
        /// </summary>
        public virtual DateTime logined { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public virtual string status { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public virtual int role { get; set; }

        /// <summary>
        /// 是否创始人
        /// </summary>
        public virtual bool super { get; set; }

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
