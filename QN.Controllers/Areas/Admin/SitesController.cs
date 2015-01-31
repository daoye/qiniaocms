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
            ViewBag.create = true;

            return View(new site());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(site site, user user)
        {
            site.date = DateTime.Now;
            ViewBag.create = true;

            return Modify(site, user);
        }

        public ActionResult Update(int id)
        {
            ViewBag.Themes = themeService.SharedThemeList();

            site site = siteService.Get(id);

            if (null == site)
            {
                return Jmp404();
            }

            if (!string.IsNullOrWhiteSpace(site.domain))
            {
                site.domain = site.domain.Replace(";", "\r\n");
            }

            return View(site);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(site site)
        {
            return Modify(site, null);
        }

        private ActionResult Modify(site site, user user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Themes = themeService.SharedThemeList();

                return View(site);
            }

            string[] domains = site.domain.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(m =>
                                         {
                                             if (m.StartsWith("http://") || m.StartsWith("https://"))
                                             {
                                                 return m.Trim();
                                             }
                                             else
                                             {
                                                 return "http://" + m.Trim();
                                             }

                                         }).ToArray();

            foreach (string d in domains)
            {
                if (siteService.IsExistsDomain(d, site.id))
                {
                    ModelState.AddModelError("domain", string.Format(lang.Lang("域名:“{0}”已被使用。"), d));
                    ViewBag.Themes = themeService.SharedThemeList();
                    return View(site);
                }
            }

            site.domain = string.Join(";", domains);

            if (site.id == 0)
            {
                if (string.IsNullOrWhiteSpace(site.name))
                {
                    site.name = lang.Lang("新网站");
                }

                user.pass = QEncryption.MD5Encryption(user.pass);

                siteService.Add(site, user);
            }
            else
            {
                siteService.Update(site);
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
            if ("del".Equals(act))
            {
                return Delete(id);
            }

            return GoBack();
        }

        #endregion

        public ActionResult DomainExists(string domain, int id)
        {
            bool flag = true;

            if (!string.IsNullOrWhiteSpace(domain))
            {
                string[] domains = domain.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()).ToArray();

                foreach (string d in domains)
                {
                    string temp = d;

                    if (!temp.StartsWith("http://") && !temp.StartsWith("https://"))
                    {
                        temp = "http://" + d;
                    }

                    if (siteService.IsExistsDomain(temp, id))
                    {
                        flag = false;
                        break;
                    }
                }
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}