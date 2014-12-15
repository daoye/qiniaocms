using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    /// <summary>
    /// QSites运行时异常
    /// </summary>
    public class QRunException : Exception
    {
        public QRunException(string message) : base(message) { }
    }
}
