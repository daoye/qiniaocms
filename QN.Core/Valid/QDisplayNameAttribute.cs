using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public class QDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public QDisplayNameAttribute(string name)
        {
            DisplayName = name;
        }
    }
}