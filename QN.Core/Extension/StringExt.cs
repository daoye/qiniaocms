using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public static class StringExt
    {
        public static string substr(this string s, int len)
        {
            return substr(s, 0, len);
        }

        public static string substr(this string s, int start, int len)
        {
            s = QHtml.StripHtml(s);

            if (start > s.Length - 1) start = s.Length - 1;

            if (len > s.Length - start) len = s.Length - start;

            return s.Substring(start, len);
        }
    }
}