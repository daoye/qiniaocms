using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Sites.Repository
{
    public class Site : BaseEntity<string>
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 网站名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 网站描述
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Meta { get; set; }

        /// <summary>
        /// 版权信息
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// 主题名称
        /// </summary>
        public string Theme { get; set; }
    }
}
