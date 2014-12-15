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
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return base.FileExists(controllerContext, ToSitePath(virtualPath));
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return base.CreatePartialView(controllerContext, ToSitePath(partialPath));
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return base.CreateView(controllerContext, ToSitePath(viewPath), ToSitePath(masterPath));
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return base.FindView(controllerContext, viewName, masterName, useCache);
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

        static string[] paths = new[] { "admin/", "account/", "common/" };
        /// <summary>
        /// 是否需要转移目标解析文件的地址
        /// </summary>
        private bool IsForward
        {
            get
            {
                if (HttpContext.Current.Request.Url.Segments.Length > 1)
                {
                    return !paths.Any(m => string.Compare(HttpContext.Current.Request.Url.Segments[1], m, true) == 0);
                }

                return false;
            }
        }

        private string ToSitePath(string virtualPath)
        {
            if (IsForward)
            {
                string themeName = string.Empty;

                site current = SiteService.CurrentSite();

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

                string tmpVirtualPath = string.Concat("~/Sites/", ThemeService.DomainToDirectoryName(current.domain), themeName, "/Views", virtualPath);

                if (usePrev)
                {
                    if (!File.Exists(HttpContext.Current.Server.MapPath(tmpVirtualPath)))
                    {
                        virtualPath = string.Concat("~/Themes/", themeName, "/Views", virtualPath);
                    }
                    else
                    {
                        virtualPath = tmpVirtualPath;
                    }
                }
                else
                {
                    virtualPath = tmpVirtualPath;
                }

                return virtualPath;
            }

            return virtualPath;
        }
    }
}
