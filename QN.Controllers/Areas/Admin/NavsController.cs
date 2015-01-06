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
        private readonly OptionService optionService = new OptionService();
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

            termService.Add(m);

            return GoBack();
        }

        public ActionResult Update(int id, string name, string items)
        {
            IList<navitem> navitems = null;
            if (!string.IsNullOrWhiteSpace(items))
            {
                items = Server.UrlDecode(items);
                navitems = QJson.Deserialize<IList<navitem>>(items);
            }

            postService.SaveNav(id, name, navitems);

            return GoBack();
        }

        #region 查询

        public ActionResult List()
        {
            string val = optionService.GetValue(R.default_nav_id);
            int defaultid = 0;

            int.TryParse(val, out defaultid);

            ViewBag.DefaultID = defaultid;

            return View();
        }

        #endregion

        #region 删除

        public ActionResult Delete(int[] id)
        {
            termService.Remove(id);

            return GoBack();
        }

        #endregion

        #region 其他操作

        public ActionResult SetDefault(int id)
        {
            optionService.Set(R.default_nav_id, id.ToString());

            return GoBack();
        }

        public ActionResult Operate(string act, int[] id)
        {
            if("del".Equals(act))
            {
                return Delete(id);
            }

            return GoBack();
        }

        #endregion
    }
}