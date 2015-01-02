using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Account
{
    public class AccountController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            if (UserService.IsLogined)
            {
                return Redirect("~/");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(string loginname, string pass)
        {
            AccountService service = new AccountService();
            service.LoginName = loginname;
            service.Pass = pass;

            switch (service.Login())
            {
                case LoginError.OK:
                    if (!string.IsNullOrWhiteSpace(Request.QueryString["returnurl"]))
                    {
                        return Redirect(Server.UrlDecode(Request.QueryString["returnurl"]));
                    }
                    return Redirect("~/admin");
                default:
                    ViewBag.Error = lang.Lang("用户名或密码错误。");
                    return View();
            }
        }

        public ActionResult Logout()
        {
            if (UserService.IsLogined)
            {
                new AccountService().Logout();
            }

            return Redirect("~/");
        }
    }
}