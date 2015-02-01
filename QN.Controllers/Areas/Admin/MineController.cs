using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class MineController : AdminController
    {
        private readonly UserService userService = new UserService();
        private readonly AccountService accountService = new AccountService();

        #region 编辑

        public ActionResult Update()
        {
            user user = userService.Get(Convert.ToInt32(User.Identity.Name));

            if (null == user)
            {
                return Jmp404();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(user user, string newpass)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user srcuser = userService.Get(Convert.ToInt32(User.Identity.Name));

            srcuser.nicename = user.nicename;
            srcuser.avatar = user.avatar;
            srcuser.email = user.email;
            srcuser.tel = user.tel;
            srcuser.url = user.url;

            userService.Update(srcuser);

            ViewBag.updated = true;

            return View(user);
        }

        #endregion

        #region 修改密码

        public ActionResult ModifyPass()
        {
            modifypassview mpview = new modifypassview();

            user info = userService.Get(Convert.ToInt32(User.Identity.Name));

            mpview.login = info.login;

            return View(mpview);
        }

        [HttpPost]
        public ActionResult ModifyPass(modifypassview mpview)
        {
            if (!ModelState.IsValid)
            {
                return View(mpview);
            }

            user info = userService.Get(Convert.ToInt32(User.Identity.Name));

            info.pass = QEncryption.MD5Encryption(mpview.newpass);

            userService.Update(info);

            ViewBag.updated = true;

            return View(new modifypassview() { login = info.login });
        }

        public ActionResult PassIsRight(string oldpass)
        {
            bool flag = false;

            if (!string.IsNullOrWhiteSpace(oldpass))
            {
                user info = userService.Get(Convert.ToInt32(User.Identity.Name));
                oldpass = QEncryption.MD5Encryption(oldpass);

                flag = oldpass == info.pass;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}