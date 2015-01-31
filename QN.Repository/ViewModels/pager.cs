using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 表示分页信息
    /// </summary>
    public class pager : model<pager>
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int pageindex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int pagesize { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int datacount { get; set; }

        /// <summary>
        /// 页码总数
        /// </summary>
        public int pagecount { get; set; }

        /// <summary>
        /// 其他信息
        /// </summary>
        public dynamic extendinfo { get; set; }
    }
}
