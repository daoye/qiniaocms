using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Plugin
{
    /// <summary>
    /// 添加动作时，如果某个动作已存在将会引发该异常
    /// </summary>
    public class ActionExistException : Exception
    {
        public ActionExistException() { }

        public ActionExistException(string message) : base(message) { }


        /// <summary>
        /// Action名称
        /// </summary>
        public string ActionName { get; set; }
    }
}