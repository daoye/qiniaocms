using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 本地化语言辅助类
        /// </summary>
        protected QLang lang = QLang.Instance();

        protected ActionResult Jmp404()
        {
            return Redirect("");
        }

        protected ActionResult GoBack()
        {
            string returnurl = null;

            if (null != Request.QueryString["returnurl"])
            {
                returnurl = Server.UrlDecode(Request.QueryString["returnurl"]);
            }
            else if (null != Request.UrlReferrer)
            {
                returnurl = Request.UrlReferrer.PathAndQuery;
            }

            if (!string.IsNullOrWhiteSpace(returnurl))
            {
                return Redirect(returnurl);
            }

            return View();
        }

        /// <summary>
        /// 当前登录的用户
        /// </summary>
        protected QUser CurrentUser
        {
            get { return User as QUser; }
        }
    }
}