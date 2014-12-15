using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public class site : entity<site>, IMeta
    {
        /// <summary>
        /// 域名
        /// </summary>
        public virtual string domain { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public virtual string info { get; set; }

        /// <summary>
        /// logo路径
        /// </summary>
        public virtual string logo { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public virtual string tel { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public virtual string email { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public virtual string addr { get; set; }

        /// <summary>
        /// 版权信息
        /// </summary>
        public virtual string copyright { get; set; }

        /// <summary>
        /// 主题名称
        /// </summary>
        public virtual string theme { get; set; }

        /// <summary>
        /// 顺序编号
        /// </summary>
        public virtual int order { get; set; }

        /// <summary>
        /// 是否是主站
        /// </summary>
        public virtual bool super { get; set; }

        /// <summary>
        /// 备案号
        /// </summary>
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
