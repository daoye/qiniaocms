
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.Web.Routing;
using System.Web.Mvc;

namespace QN
{
    public class ThemeControllerFactory : System.Web.Mvc.DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            return base.CreateController(requestContext, controllerName);
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Type result = null;
            if (ThemeHelper.IsForword(requestContext.RouteData))
            {
                //如果不是area，则是前台页面，前台页面统一交付给ThemeController处理。
                result = typeof(ThemeController);
            }
            else
            {
                result = base.GetControllerType(requestContext, controllerName);
            }

            return result;
        }
    }
}