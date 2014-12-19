using QN.Service;
using QN.Theme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QN
{
    public class ThemeViewEngine : RazorViewEngine
    {
        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext.RouteData.DataTokens["area"] == null)
            {
                this.MasterLocationFormats =
                    this.ViewLocationFormats = ThemeFilePath(controllerContext);
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext.RouteData.DataTokens["area"] == null)
            {

                this.MasterLocationFormats
                    = this.PartialViewLocationFormats = ThemeFilePath(controllerContext);
            }

            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        /// <summary>
        /// 当前发起请求的域名
        /// </summary>
        private string Domain
        {
            get
            {
                return HttpContext.Current.Request.Url.Authority;
            }
        }

        private string[] ThemeFilePath(ControllerContext controllerContext)
        {
            string endstr = "/Views/{1}/{0}.cshtml";

            string themeName = string.Empty;

            //site current = SiteService.CurrentSite();

            site current = new site()
            {
                theme = "default",
                domain = "localhost:7777"
            };

            if (null == current)
            {
                throw new ThemeException("没有找到网站的基本配置信息。");
            }
            else if (string.IsNullOrEmpty(current.theme))
            {
                themeName = "Default";
            }
            else
            {
                themeName = current.theme;
            }

            //模版预览
            bool usePrev = false;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string tmpTheme = HttpContext.Current.Request.QueryString["theme"];
                if (!string.IsNullOrWhiteSpace(tmpTheme))
                {
                    themeName = tmpTheme;
                    usePrev = true;
                }
            }

            string virtualPath = string.Concat("~/Sites/", ThemeService.DomainToDirectoryName(current.domain), themeName);

            if (usePrev)
            {
                if (!File.Exists(HttpContext.Current.Server.MapPath(virtualPath)))
                {
                    virtualPath = string.Concat("~/Themes/", themeName);
                }
            }

            virtualPath = string.Concat(virtualPath, endstr);

            return new string[] { virtualPath };
        }
    }
}