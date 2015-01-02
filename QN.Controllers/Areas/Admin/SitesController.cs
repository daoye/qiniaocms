using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class SitesController : AdminController
    {
        private readonly SiteService siteService = new SiteService();
        private readonly ThemeService themeService = new ThemeService();

        #region 编辑

        public ActionResult Add()
        {
            ViewBag.Themes = themeService.SharedThemeList();

            return View(new site());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(site site)
        {
            site.createtime = DateTime.Now;

            return Modify(site);
        }

        public ActionResult Update(int id)
        {
            ViewBag.Themes = themeService.SharedThemeList();

            site site = siteService.Get(id);

            if (null == site)
            {
                return Jmp404();
            }

            return View(site);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(site site)
        {
            return Modify(site);
        }

        private ActionResult Modify(site site)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Themes = themeService.SharedThemeList();

                return View(site);
            }

            SiteModifyError error = SiteModifyError.OK;
            if (site.id == 0)
            {
                error = siteService.Add(site);
            }
            else
            {
                error = siteService.Update(site);
            }

            switch (error)
            {
                case SiteModifyError.DomainExists:
                    ModelState.AddModelError("domain", lang.Lang("域名已被使用。"));
                    ViewBag.Themes = themeService.SharedThemeList();
                    return View(site);
            }

            return RedirectToAction("list", new { state = "new", id = site.id });
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
            siteService.Remove(id);

            return GoBack();
        }

        #endregion

        #region 其他操作

        public ActionResult Operate(string act, int[] id)
        {
            if("del".Equals(act))
            {
                return Delete(id);
            }

            return GoBack();
        }

        #endregion

        public ActionResult DomainExists(string domain, int id)
        {
            return Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}