using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class site : entity<site>, IMeta
    {
        /// <summary>
        /// 域名
        /// </summary>
        [DisplayName("网站域名")]
        [QRequired]
        [Remote("DomainExists", "Sites", "Admin", AdditionalFields = "id")]
        public virtual string domain { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("网站名称")]
        public virtual string name { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [DisplayName("网站简介")]
        public virtual string info { get; set; }

        /// <summary>
        /// logo路径
        /// </summary>
        [DisplayName("网站Logo")]
        public virtual string logo { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [DisplayName("联系电话")]
        public virtual string tel { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        [DisplayName("联系邮箱")]
        public virtual string email { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        [DisplayName("联系地址")]
        public virtual string addr { get; set; }

        /// <summary>
        /// 版权信息
        /// </summary>
        [DisplayName("版权信息")]
        public virtual string copyright { get; set; }

        /// <summary>
        /// 主题名称
        /// </summary>
        [DisplayName("主题")]
        [QRequired(ErrorMessage = "必须选择")]
        public virtual string theme { get; set; }

        /// <summary>
        /// 顺序编号
        /// </summary>
        [DisplayName("顺序编号")]
        public virtual int order { get; set; }

        /// <summary>
        /// 是否是主站
        /// </summary>
        public virtual bool super { get; set; }

        /// <summary>
        /// 备案号
        /// </summary>
        [DisplayName("备案号")]
        public virtual string icpnumber { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime createtime { get; set; }

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