using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// 用于在navbar控件中传递数据
    /// </summary>
    public class navbar
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int termid { get; set; }

        /// <summary>
        /// 当前菜单项的父级ID
        /// </summary>
        public int parent { get; set; }
    }
}
