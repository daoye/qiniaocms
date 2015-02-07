using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class UsersController : AdminController
    {
        private readonly UserService userService = new UserService();

        #region 编辑

        public ActionResult Add()
        {
            return View(new user());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(user user)
        {
            return Modify(user, null);
        }

        public ActionResult Update(int id)
        {
            user user = userService.Get(id);

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
            return Modify(user, newpass);
        }

        private ActionResult Modify(user user, string newpass)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            UserLoginError error = UserLoginError.OK;
            if (user.id == 0)
            {
                user.status = R.user_status_nomal;
                user.siteid = R.siteid;
                user.pass = QEncryption.MD5Encryption(user.pass);

                error = userService.Add(user);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(newpass))
                {
                    if (newpass.Length < 4 || newpass.Length > 50)
                    {
                        ModelState.AddModelError("pass", "密码长度必须位于4~50之间。");
                        return View(user);
                    }

                    user.pass = QEncryption.MD5Encryption(newpass);
                }

                error = userService.Update(user);
            }

            switch (error)
            {
                case UserLoginError.LoginExists:
                    ModelState.AddModelError("login", lang.Lang("用户名已被使用。"));
                    return View(user);
            }

            return RedirectToAction("list", new { state = "new", id = user.id });
        }

        #endregion

        #region 查询

        public ActionResult List(string action)
        {
            return View();
        }

        #endregion

        #region 删除

        public ActionResult Delete(int[] id)
        {
            userService.Remove(id);

            return GoBack();
        }

        #endregion

        #region 其他操作

        public ActionResult Operate(string act, int[] id)
        {
            if ("del".Equals(act))
            {
                return Delete(id);
            }

            return GoBack();
        }

        #endregion

        [AllowAnonymous]
        public ActionResult LoginExists(string login, int id)
        {
            bool flag = true;
            if (!string.IsNullOrWhiteSpace(login) && userService.IsExestsLoginName(login, id))
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}