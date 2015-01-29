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
        private string[] defaultMasterLocationFormats;
        private string[] defaultViewLocationFormats;
        private string[] defaultPartialViewLocationFormats;

        public ThemeViewEngine()
        {
            defaultMasterLocationFormats = this.MasterLocationFormats;
            defaultViewLocationFormats = this.ViewLocationFormats;
            defaultPartialViewLocationFormats = this.PartialViewLocationFormats;
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (ThemeHelper.IsForword(controllerContext.RouteData))
            {
                string pathRoot = ThemeFilePath(controllerContext);

                this.MasterLocationFormats = new string[] { 
                    pathRoot + "/Views/{0}.cshtml", 
                    pathRoot + "/Views/{0}.vbhtml", 
                    pathRoot + "/Views/{1}/{0}.cshtml", 
                    pathRoot + "/Views/{1}/{0}.vbhtml", 
                    pathRoot + "/Views/Shared/{0}.cshtml", 
                    pathRoot + "/Views/Shared/{0}.vbhtml",
                };

                this.ViewLocationFormats = new string[] { 
                    pathRoot + "/Views/{0}.cshtml", 
                    pathRoot + "/Views/{0}.vbhtml",
                    pathRoot + "/Views/{1}/{0}.cshtml", 
                    pathRoot + "/Views/{1}/{0}.vbhtml"
                };
            }
            else
            {
                this.MasterLocationFormats = this.defaultMasterLocationFormats;
                this.ViewLocationFormats = this.defaultViewLocationFormats;
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext.RouteData.DataTokens["area"] == null)
            {
                string pathRoot = ThemeFilePath(controllerContext);

                this.MasterLocationFormats = this.PartialViewLocationFormats = new string[] { 
                    pathRoot + "/Views/{0}.cshtml", 
                    pathRoot + "/Views/{0}.vbhtml", 
                    pathRoot + "/Views/{1}/{0}.cshtml", 
                    pathRoot + "/Views/{1}/{0}.vbhtml", 
                    pathRoot + "/Views/Shared/{0}.cshtml", 
                    pathRoot + "/Views/Shared/{0}.vbhtml",
                };
            }
            else
            {
                this.MasterLocationFormats = this.defaultMasterLocationFormats;
                this.defaultPartialViewLocationFormats = this.PartialViewLocationFormats;
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

        private string ThemeFilePath(ControllerContext controllerContext)
        {
            string themeName = string.Empty;

            site current = R.site;

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

            string Path = string.Concat("~/Sites/", ThemeService.DomainToDirectoryName(current.firstdomain()), themeName);

            if (usePrev)
            {
                if (!File.Exists(HttpContext.Current.Server.MapPath(Path)))
                {
                    Path = string.Concat("~/Themes/", themeName);
                }
            }

            return Path;
        }

    }
}