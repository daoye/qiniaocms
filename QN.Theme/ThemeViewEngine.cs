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
            if(R.Installed)
            {
                if (ThemeHelper.IsForword(controllerContext.RouteData))
                {
                    string pathRoot = ThemeFilePath(controllerContext);

                    List<string> masterLocations = new List<string>();
                    masterLocations.Add(pathRoot + "/views/{0}.cshtml");
                    masterLocations.Add(pathRoot + "/views/{0}.vbhtml");
                    masterLocations.Add(pathRoot + "/views/{1}/{0}.cshtml");
                    masterLocations.Add(pathRoot + "/views/{1}/{0}.vbhtml");
                    masterLocations.Add(pathRoot + "/views/shared/{0}.cshtml");
                    masterLocations.Add(pathRoot + "/views/shared/{0}.vbhtml");
                    if (null != controllerContext.RouteData.Values["action"])
                    {
                        string[] secions = controllerContext.RouteData.Values["action"].ToString().Split('-');
                        if (secions.Length > 1)
                        {
                            secions = secions.Take(secions.Length - 1).ToArray();
                            masterLocations.Insert(0, pathRoot + "/views/" + string.Join("/", secions) + "/{0}.cshtml");
                        }
                    }

                    this.MasterLocationFormats = masterLocations.ToArray();


                    List<string> viewLocations = new List<string>();
                    viewLocations.Add(pathRoot + "/views/{0}.cshtml");
                    viewLocations.Add(pathRoot + "/views/{0}.vbhtml");
                    viewLocations.Add(pathRoot + "/views/{1}/{0}.cshtml");
                    viewLocations.Add(pathRoot + "/views/{1}/{0}.vbhtml");
                    if (null != controllerContext.RouteData.Values["action"])
                    {
                        viewLocations.Insert(0, pathRoot + "/views/" + controllerContext.RouteData.Values["action"].ToString().Replace("-", "/") + ".cshtml");
                    }

                    this.ViewLocationFormats = viewLocations.ToArray();
                }
                else
                {
                    this.MasterLocationFormats = this.defaultMasterLocationFormats;
                    this.ViewLocationFormats = this.defaultViewLocationFormats;
                }
            }

            return base.FindView(controllerContext, viewName, masterName, false);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext.RouteData.DataTokens["area"] == null)
            {
                string pathRoot = ThemeFilePath(controllerContext);

                List<string> viewLocations = new List<string>();
                viewLocations.Add(pathRoot + "/views/{0}.cshtml");
                viewLocations.Add(pathRoot + "/views/{0}.vbhtml");
                viewLocations.Add(pathRoot + "/views/{1}/{0}.cshtml");
                viewLocations.Add(pathRoot + "/views/{1}/{0}.vbhtml");
                viewLocations.Add(pathRoot + "/views/shared/{0}.cshtml");
                viewLocations.Add(pathRoot + "/views/shared/{0}.vbhtml");

                if (null != controllerContext.RouteData.Values["action"])
                {
                    string[] secions = controllerContext.RouteData.Values["action"].ToString().Split('-');
                    if (secions.Length > 1)
                    {
                        secions = secions.Take(secions.Length - 1).ToArray();
                        viewLocations.Insert(0, pathRoot + "/views/" + string.Join("/", secions) + "/{0}.cshtml");
                    }
                }

                this.MasterLocationFormats = this.PartialViewLocationFormats = viewLocations.ToArray();
            }
            else
            {
                this.MasterLocationFormats = this.defaultMasterLocationFormats;
                this.defaultPartialViewLocationFormats = this.PartialViewLocationFormats;
            }

            return base.FindPartialView(controllerContext, partialViewName, false);
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
                themeName = "default";
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

            string Path = string.Concat("~/sites/", QConfiger.DomainToDirectoryName(current.firstdomain()), themeName);

            if (usePrev)
            {
                if (!File.Exists(HttpContext.Current.Server.MapPath(Path)))
                {
                    Path = string.Concat("~/themes/", themeName);
                }
            }

            return Path;
        }

    }
}