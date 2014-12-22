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


        public ActionResult Add()
        {
            ViewBag.Themes = themeService.SharedThemeList();

            return View(new site());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(site site)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Themes = themeService.SharedThemeList();

                return View(site);
            }

            switch (siteService.Add(site))
            {
                case SiteModifyError.DomainExists:
                    ModelState.AddModelError("domain", lang.Lang("域名已被使用。"));
                    ViewBag.Themes = themeService.SharedThemeList();
                    return View(site);
            }

            return RedirectToAction("list", new { state = "new", id = site.id });
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult DomainExists(string domain, int id)
        {
            return Json(false);
        }
    }
}