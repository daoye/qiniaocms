using QN.Service;
using System;
using System.Collections.Generic;
using System.IO;
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

            RegisterLanguage();

            ControllerBuilder.Current.SetControllerFactory(typeof(ThemeControllerFactory));

            SystemActionService.InitSystemAction();
        }

        private static void RegisterLanguage()
        {
            List<string> paths = new List<string>();
            paths.Add("content" + Path.DirectorySeparatorChar + "lang");

            for (int i = 0; i < paths.Count; i++)
            {
                paths[i] = AppDomain.CurrentDomain.BaseDirectory + paths[i];
            }

            QLang.Intlize(paths.ToArray());
        }
    }
}