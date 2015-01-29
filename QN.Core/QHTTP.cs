using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public static class QHttp
    {
        /// <summary>
        /// 显示自定义的500错误信息
        /// </summary>
        /// <param name="html"></param>
        public static void Write500(string html)
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            System.Web.HttpContext.Current.Response.StatusCode = 500;
            System.Web.HttpContext.Current.Response.StatusDescription = "500 Server Error";
            System.Web.HttpContext.Current.Response.ContentType = "text/html";
            System.Web.HttpContext.Current.Response.Write(html);
            System.Web.HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 显示自定义的404错误信息
        /// </summary>
        /// <param name="html"></param>
        public static void Write404(string html)
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            System.Web.HttpContext.Current.Response.StatusCode = 404;
            System.Web.HttpContext.Current.Response.StatusDescription = "404 Not Found";
            System.Web.HttpContext.Current.Response.ContentType = "text/html";
            System.Web.HttpContext.Current.Response.Write(html);
            System.Web.HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 显示自定义的403错误信息
        /// </summary>
        /// <param name="html"></param>
        public static void Write403(string html)
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            System.Web.HttpContext.Current.Response.StatusCode = 403;
            System.Web.HttpContext.Current.Response.StatusDescription = "403 Forbidden";
            System.Web.HttpContext.Current.Response.ContentType = "text/html";
            System.Web.HttpContext.Current.Response.Write(html);
            System.Web.HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 301跳转
        /// </summary>
        public static void Jmp301(string url)
        {
            if(string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.StatusCode = 301;
            System.Web.HttpContext.Current.Response.Status = "301 Moved Permanently";
            System.Web.HttpContext.Current.Response.AddHeader("Location", url);
            System.Web.HttpContext.Current.Response.End();
        }
    }
}