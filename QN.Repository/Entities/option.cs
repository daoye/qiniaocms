using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class option : entity<option>
    {
        /// <summary>
        /// 子站ID
        /// </summary>
        public virtual int siteid { get; set; }

        /// <summary>
        /// 配置项名称
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public virtual string value { get; set; }

        /// <summary>
        /// 启动时是否自动加载
        /// </summary>
        public virtual bool autoload { get; set; }
    }
}
