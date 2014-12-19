using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    /// <summary>
    /// 所有模版页面的Controller
    /// </summary>
    public class ThemeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        protected override IActionInvoker CreateActionInvoker()
        {
            return new ThemeActionInvoker();
        }
    }
}