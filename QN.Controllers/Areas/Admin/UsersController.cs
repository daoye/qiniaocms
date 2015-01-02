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
        public ActionResult Add(user member)
        {
            return Modify(member);
        }

        public ActionResult Update(int id)
        {
            user member = userService.Get(id);

            if (null == member)
            {
                return Jmp404();
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(user member)
        {
            return Modify(member);
        }

        private ActionResult Modify(user member)
        {
            if (!ModelState.IsValid)
            {
                return View(member);
            }

            UserLoginError error = UserLoginError.OK;
            if (member.id == 0)
            {
                error = userService.Add(member);
            }
            else
            {
                error = userService.Update(member);
            }

            switch (error)
            {
                case UserLoginError.LoginExists:
                    ModelState.AddModelError("login", lang.Lang("用户名已被使用。"));
                    return View(member);
            }

            return RedirectToAction("list", new { state = "new", id = member.id });
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