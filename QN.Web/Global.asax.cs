using QN.Service;
using QN.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace QN.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            this.Error += MvcApplication_Error;
            this.AuthorizeRequest += MvcApplication_AuthorizeRequest;
            this.BeginRequest += MvcApplication_BeginRequest;
        }

        void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            string first = R.site.firstdomain();
            string reqdomain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;

            if (!string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(reqdomain))
            {
                if (string.Compare(reqdomain, first, true) != 0)
                {
                    QHttp.Jmp301(first + HttpContext.Current.Request.Url.PathAndQuery);
                }
            }
        }

        protected void Application_Start()
        {
            Stater.Initlize();
        }

        void MvcApplication_AuthorizeRequest(object sender, EventArgs e)
        {
            AccountService.AuthorizeProcess();
        }

        void MvcApplication_Error(object sender, EventArgs e)
        {
            var error = HttpContext.Current.Error;

            if (null == error)
            {
                return;
            }
            if (error is Theme404Exception)
            {
                QHttp.Write404("<div style='font-family:微软雅黑; font-weight:bold; font-size:30px; text-align:center;margin-top:100px;'><strong>:(&nbsp;&nbsp;&nbsp;&nbsp;无法找到您要查看的页面。<br/>404.</strong><div style='display:none;'>无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面</div></div>");
                HttpContext.Current.ClearError();
            } 
            else if (error is QRunException)
            {
                QHttp.Write500("<div style='font-family:微软雅黑; font-weight:bold; font-size:30px; text-align:center;margin-top:100px;'><strong>:( " + error.Message + "</strong></div>");
                HttpContext.Current.ClearError();
            }
            else
            {
                return;

                if (error is HttpException)
                {
                    var httpError = error as HttpException;
                    if (httpError != null)
                    {
                        if (httpError.GetHttpCode() == 404)
                        {
                            QHttp.Write404("<div style='font-family:微软雅黑; font-weight:bold; font-size:30px; text-align:center;margin-top:100px;'><strong>:(&nbsp;&nbsp;&nbsp;&nbsp;无法找到您要查看的页面。<br/>404.</strong><div style='display:none;'>无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面看的页面无法找到您要查看的页面无法找到您要查看的页面无法找到您要查看的页面</div></div>");
                            Server.ClearError();
                            return;
                        }
                    }
                }

                QHttp.Write500("<div style='font-family:微软雅黑; font-weight:bold; font-size:30px; text-align:center;margin-top:100px;'><strong>:( 抱歉，发生了一些问题。</strong></div>");
            }
        }
    }
}