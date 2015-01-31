using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class NavsController : AdminController
    {
        private readonly TermService termService = new TermService();
        private readonly PostService postService = new PostService();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string name)
        {
            term m = new term()
            {
                name = name,
                type = "nav",
                date = DateTime.Now,
                modified = DateTime.Now
            };

            termService.AddNav(m);

            return RedirectToAction("list");
        }

        public ActionResult Update(int id, string name, string items)
        {
            IList<navitem> navitems = null;
            if (!string.IsNullOrWhiteSpace(items))
            {
                items = Server.UrlDecode(items);
                navitems = QJson.Deserialize<IList<navitem>>(items);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = lang.Lang("新菜单");
            }

            postService.SaveNav(id, name, navitems);

            ViewBag.SelectedID = id;

            return RedirectToAction("list", new { sid = id });
        }

        #region 查询

        public ActionResult List(int? sid)
        {
            int defaultid = 0;
            string val = opt.get(R.siteid, R.default_nav_id);
            int.TryParse(val, out defaultid);

            ViewBag.defaultid = defaultid;

            if (null == sid)
            {
                ViewBag.selectedid = defaultid;
            }
            else
            {
                ViewBag.selectedid = (int)sid;
            }

            return View();
        }

        #endregion

        #region 删除

        public ActionResult Delete(int[] id)
        {
            termService.Remove(id);

            return RedirectToAction("list");
        }

        #endregion

        #region 其他操作

        public ActionResult SetDefault(int id)
        {
            opt.set(R.siteid, R.default_nav_id, id.ToString());

            return RedirectToAction("list");
        }

        public ActionResult Operate(string act, int[] id)
        {
            if("del".Equals(act))
            {
                return Delete(id);
            }

            return RedirectToAction("list");
        }

        #endregion
    }
}