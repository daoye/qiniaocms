using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;

namespace QN
{
    public abstract class ThemeViewPage<TModel> : WebViewPage<TModel>
    {
        public ThemeViewPage()
        {
        }

        public virtual string posts()
        {
            return "哈哈哈";
        }

        public string __(string message)
        {
            return string.Empty;
        }
    }
}
