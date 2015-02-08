using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Plugin
{
    /// <summary>
    /// 执行某个动作时，动作不存在则会应发该异常
    /// </summary>
    public class ActionNotFoundException : Exception
    {
        public ActionNotFoundException() { }

        public ActionNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Action名称
        /// </summary>
        public string ActionName { get; set; }
    }
}