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
            if (string.IsNullOrEmpty(s)) return "";

            if (s.Length < len) return s.ToString();

            return s.Substring(0, len);
        }

        public static string substr(this string s, int start, int len)
        {
            if (string.IsNullOrEmpty(s)) return "";

            if (start > s.Length - 1) start = s.Length - 1;

            if (len > s.Length - start) len = s.Length - start;
            
            return s.Substring(start, len);

        }
    }
}
