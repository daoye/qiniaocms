using QN.Service;
using QN.Theme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (R.Installed)
            {
                site site = R.site;
                string url = null;
                if (null != site)
                {
                    string first = site.firstdomain();
                    string reqdomain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;

                    if (!string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(reqdomain))
                    {
                        if (string.Compare(reqdomain, first, true) != 0)
                        {
                            url = first + HttpContext.Current.Request.Url.PathAndQuery;
                            QHttp.Jmp301(url);
                        }
                    }
                }
            }
            else
            {
                if (null == HttpContext.Current || null == HttpContext.Current.Request)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(HttpContext.Current.Request.CurrentExecutionFilePathExtension) || string.Compare(HttpContext.Current.Request.CurrentExecutionFilePathExtension, ".aspx") == 0)
                {
                    if (!Request.Path.ToLower().StartsWith("/install/"))
                    {
                        string url = null;
                        string root = Request.ApplicationPath;
                        if (!root.EndsWith("/"))
                        {
                            root += "/";
                        }

                        url = HttpContext.Current.Request.Url.Scheme + "://" + Request.Url.Authority + root + "install/index.aspx";

                        Response.Redirect(url, true);
                    }
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
                WriteError(404, null);
                HttpContext.Current.ClearError();
            } 
            else if (error is QRunException)
            {
                WriteError(500, error.Message);
            }
            else
            {
                if (error is HttpException)
                {
                    var httpError = error as HttpException;
                    if (httpError != null)
                    {
                        if (httpError.GetHttpCode() == 404)
                        {
                            WriteError(404, null);
                            return;
                        }
                    }
                }

                WriteError(500, "抱歉，发生了一些问题。");
            }
        }

        void WriteError(int errorcode, string message)
        {
            string fuckie = "<div style='display:none;'>FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!FUCKIE!!</div>";

            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<meta charset=\"utf-8\">");
            sb.Append("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\">");
            sb.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0\">");

            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("{0}");
            sb.Append("</body>");
            sb.Append("</html>");

            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.ClearError();
            switch(errorcode)
            {
                case 404:
                    QHttp.Write404(string.Format(sb.ToString(), "<div style='font-family:微软雅黑; font-weight:bold; font-size:30px; text-align:center;margin-top:100px;'><strong>:(&nbsp;&nbsp;&nbsp;&nbsp;无法找到您要查看的页面。<br/>404.</strong></div>" + fuckie));
                    break;
                default:
                    QHttp.Write500(string.Format(sb.ToString(), "<div style='font-family:微软雅黑; font-weight:bold; font-size:30px; text-align:center;margin-top:100px;'><strong>:( " + message + "</strong></div>" + fuckie));
                    break;
            }
        }
    }
}