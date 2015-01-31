using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class site : entity<site>, IMeta<sitemeta>
    {
        /// <summary>
        /// 域名
        /// </summary>
        [DisplayName("网站域名")]
        [QRequired]
        [Remote("DomainExists", "Sites", "Admin", AdditionalFields = "id", ErrorMessage = "域名已被绑定在其他网站上。")]
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
        /// 备案号
        /// </summary>
        [DisplayName("备案号")]
        public virtual string icpnumber { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime date { get; set; }

        public virtual string meta(string property)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                return string.Empty;
            }

            sitemeta sm = R.session.CreateCriteria<sitemeta>()
                        .Add(Expression.Eq("siteid", id))
                        .Add(Expression.Eq("key", property))
                        .List<sitemeta>()
                        .FirstOrDefault();

            if (null == sm)
            {
                return string.Empty;
            }

            return sm.value ?? string.Empty;
        }

        public virtual void meta(string property, string value)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                return;
            }
            if (id <= 0)
            {
                throw new Exception("在实体对象未保存之前，无法设置附加属性。");
            }

            sitemeta sm = R.session.CreateCriteria<sitemeta>()
                                     .Add(Expression.Eq("siteid", id))
                                     .Add(Expression.Eq("key", property))
                                     .List<sitemeta>()
                                     .FirstOrDefault();

            if (null == sm)
            {
                sm = new sitemeta()
                {
                    siteid = id,
                    key = property
                };
            }

            sm.value = value;

            R.session.SaveOrUpdate(sm);
        }

        public virtual IEnumerable<sitemeta> metas()
        {
            return R.session.CreateCriteria<sitemeta>()
                            .Add(Expression.Eq("siteid", id))
                            .List<sitemeta>();

        }

        /// <summary>
        /// 当前网站的所有域名
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> domains()
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrWhiteSpace(domain))
            {
                result.AddRange(domain.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()));
            }

            return result;
        }

        /// <summary>
        /// 当前网站所绑定的第一个域名
        /// </summary>
        /// <returns></returns>
        public virtual string firstdomain()
        {
            return domains().FirstOrDefault();
        }
    }
}