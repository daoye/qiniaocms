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
            return Modify(user);
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
        public ActionResult Update(user user)
        {
            return Modify(user);
        }

        private ActionResult Modify(user user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            UserLoginError error = UserLoginError.OK;
            if (user.id == 0)
            {
                user.status = R.user_status_nomal;

                error = userService.Add(user);
            }
            else
            {
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

        public ActionResult LoginExists(string login, int id)
        {
            return Json(userService.IsExestsLoginName(login, id), JsonRequestBehavior.AllowGet);
        }
    }
}