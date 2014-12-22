using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public static class MVCUrlExtend
    {
        public static string RouteAction(this UrlHelper helper, string routeName, string actionName)
        {
            return helper.RouteUrl(routeName, new { action = actionName });
        }

        public static string RouteAction(this UrlHelper helper, string routeName, string actionName, string controllerName)
        {
            return helper.RouteUrl(routeName, new { action = actionName, controller = controllerName });
        }
    }
}
