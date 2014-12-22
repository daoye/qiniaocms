using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 本地化语言辅助类
        /// </summary>
        protected QLang lang = QLang.Instance();
    }
}
