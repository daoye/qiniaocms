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
        private readonly UserService userService = new UserService();
        private readonly AccountService accountService = new AccountService();

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
            switch (accountService.Login(loginname, pass, null))
            {
                case LoginError.OK:
                    if (!string.IsNullOrWhiteSpace(Request.QueryString["returnurl"]))
                    {
                        return Redirect(Server.UrlDecode(Request.QueryString["returnurl"]));
                    }

                    return RedirectToAction("dashboard", "home", new { @area = "admin" });
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

        public ActionResult Sinup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sinup(sinupview model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user user = new user();
            user.avatar = root + "content/images/avatar.png";
            user.date = DateTime.Now;
            user.email = model.email;
            user.login = model.login;
            user.nicename = model.nicename;
            user.siteid = R.siteid;
            user.roleid = R.role_user;
            user.status = R.user_status_nomal;
            user.pass = QEncryption.MD5Encryption(model.pass);

            userService.Add(user);

            if (accountService.Login(user.login, model.pass, null) == LoginError.OK)
            {
                return RedirectToAction("dashboard", "home", new { @area = "admin" });
            }

            return View();
        }
    }
}