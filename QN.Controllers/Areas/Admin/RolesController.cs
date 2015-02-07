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
        private readonly CarteService carteService = new CarteService();

        #region 编辑

        public ActionResult Add()
        {
            return View(new role());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(role role, int[] carteid)
        {
            return Modify(role, carteid);
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
        public ActionResult Update(role role, int[] carteid)
        {
            return Modify(role, carteid);
        }

        private ActionResult Modify(role role, int[] carteid)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            IList<acl> acls = new List<acl>();

            if (null != carteid)
            {
                foreach (int cid in carteid)
                {
                    carte carte = carteService.Get(cid);

                    acls.Add(new acl()
                    {
                        carteid = carte.id
                    });
                }
            }

            if (role.id == 0)
            {
                if (string.IsNullOrWhiteSpace(role.name))
                {
                    role.name = lang.Lang("新文章");
                }
                role.siteid = R.siteid;

                roleService.Add(role, acls);
            }
            else
            {
                roleService.Update(role, acls);
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