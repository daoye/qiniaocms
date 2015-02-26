using QN.Theme;
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
    [HandleError]
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

        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            base.Execute(requestContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (R.Installed && filterContext.Exception is InvalidOperationException)
            {
                throw new Theme404Exception();
            }
            else
            {
                base.OnException(filterContext);
            }
        }
    }
}