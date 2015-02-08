using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public class actionresult
    {
        /// <summary>
        /// 执行是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public object result { get; set; }
    }
}