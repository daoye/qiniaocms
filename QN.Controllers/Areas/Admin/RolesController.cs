using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class RolesController : AdminController
    {
        private readonly RoleService roleService = new RoleService();

        #region 编辑

        public ActionResult Add()
        {
            return View(new role());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(role role)
        {
            return Modify(role);
        }

        public ActionResult Update(int id)
        {
            role role = roleService.Get(id);

            if (null == role)
            {
                return Jmp404();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(role role)
        {
            return Modify(role);
        }

        private ActionResult Modify(role role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            if (role.id == 0)
            {
               roleService.Add(role, null);
            }
            else
            {
                roleService.Update(role, null);
            }

            return RedirectToAction("list", new { state = "new", id = role.id });
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
            roleService.Remove(id);

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
    }
}