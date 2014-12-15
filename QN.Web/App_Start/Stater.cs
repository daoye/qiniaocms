using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QN.Web
{
    /// <summary>
    /// 启动器
    /// </summary>
    public static class Stater
    {
        /// <summary>
        /// 初始化应用程序
        /// </summary>
        public static void Initlize()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ThemeViewEngine());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(typeof(ThemeControllerFactory));
        }
    }
}