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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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

        /// <summary>
        /// 发送密码重置邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendResetPassEmail(string email)
        {
            string url = R.site.firstdomain();
            if (url.EndsWith("/"))
            {
                url = url.TrimEnd('/');
            }

            url += Url.Action("resetpass", "account");

            switch (accountService.SendResetEmail(email, url))
            {
                case SendResetEmailError.EmailSendFalid:
                    return Json(new { success = false, msg = "邮件发送失败。" });
                case SendResetEmailError.NotFoundUser:
                    return Json(new { success = false, msg = "不存在此用户。" });
                default:
                    return Json(new { success = true });
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ResetPass(string id)
        {
            user user = null;
            bool flag = false;
            try
            {
                id = QEncryption.Base64Decryption(id);
                user = accountService.FindByResetpassId(id);

                flag = user != null;
            }
            catch
            {

            }

            if (!flag)
            {
                ViewBag.msg = "链接已无效。";
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPass(string id, string pass)
        {
            bool flag = false;

            try
            {
                id = QEncryption.Base64Decryption(id);
                flag = accountService.ResetPass(id, pass);
            }
            catch
            {

            }

            if (!flag)
            {
                ViewBag.msg = "链接已失效。";
            }
            else
            {
                ViewBag.msg = "密码已重置，请使用新密码登录。";
            }

            return View();
        }
    }
}