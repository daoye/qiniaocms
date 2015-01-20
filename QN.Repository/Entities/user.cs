using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum userstatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [QDisplayName("正常")]
        nomal = 0,

        /// <summary>
        /// 禁止登录
        /// </summary>
        [QDisplayName("禁止登录")]
        ban = 1
    }


    /// <summary>
    /// 用户信息
    /// </summary>
    public class user : entity<user>, IMeta
    {
        /// <summary>
        /// 网站id
        /// </summary>
        [DisplayName("所属网站")]
        public virtual int siteid { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        [DisplayName("用户名")]
        [QRequired]
        [QStringLength(50, MinimumLength = 2)]
        public virtual string login { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DisplayName("密码")]
        [QRequired]
        [QStringLength(50, MinimumLength = 4)]
        public virtual string pass { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [DisplayName("昵称")]
        public virtual string nicename { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public virtual string avatar { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        [DisplayName("联系邮箱")]
        public virtual string email { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DisplayName("联系电话")]
        public virtual string tel { get; set; }

        /// <summary>
        /// 用户主页
        /// </summary>
        [DisplayName("用户主页")]
        public virtual string url { get; set; }

        /// <summary>
        /// 注册日期
        /// </summary>
        [DisplayName("注册日期")]
        public virtual DateTime registered { get; set; }

        /// <summary>
        /// 登录日期
        /// </summary>
        [DisplayName("登录日期")]
        public virtual DateTime logined { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DisplayName("状态")]
        public virtual userstatus status { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [DisplayName("用户角色")]
        [QRequired(ErrorMessage = "请选择{0}")]
        public virtual int roleid { get; set; }

        /// <summary>
        /// 是否创始人
        /// </summary>
        [DisplayName("创始人")]
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
