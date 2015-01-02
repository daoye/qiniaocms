using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public class termdropdown : model<termdropdown>
    {
        /// <summary>
        /// 表单id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 选中项ID
        /// </summary>
        public int selected { get; set; }

        /// <summary>
        /// 排除指定id的分类及子分类
        /// </summary>
        public int exclude { get; set; }

        /// <summary>
        /// 默认选项名称
        /// </summary>
        public string defaulttext { get; set; }

        /// <summary>
        /// 分类类型
        /// </summary>
        public string termtype { get; set; }
    }
}