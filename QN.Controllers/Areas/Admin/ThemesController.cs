using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class ThemesController : AdminController
    {
        private readonly SiteService siteService = new SiteService();
        private readonly ThemeService themeService = new ThemeService();

        public ActionResult Edit(string file, string theme)
        {
            theme t = null;
            string content = string.Empty;

            if (!string.IsNullOrWhiteSpace(file))
            {
                file = Server.UrlDecode(file);
            }

            if (!string.IsNullOrWhiteSpace(theme) && themeService.ThemeExists(theme))
            {
                t = themeService.Get(theme);
            }
            else
            {
                t = themeService.GetDefault();
            }

            IList<string> files = themeService.GetThemeFiles(t.dirname);

            if (!files.Any(m => string.Compare(file, m, true) == 0))
            {
                file = files.FirstOrDefault();
            }

            content = themeService.GetThemeFileContent(t.dirname, file);

            ViewBag.Theme = t.dirname;
            ViewBag.Content = content;
            ViewBag.File = file;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string content, string theme, string file)
        {
            ViewBag.Theme = theme;
            ViewBag.Content = content;
            ViewBag.File = file;

            themeService.ModifyThemeFileContent(theme, file, content);

            return View();
        }

        public ActionResult SetDefault(string id)
        {
            themeService.SetDefault(id);

            return GoBack();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Delete(string id)
        {
            themeService.Remove(id);

            return GoBack();
        }

        public ActionResult List()
        {
            theme model = themeService.GetDefault();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocalInstall()
        {
            string message = string.Empty;

            var file = Request.Files["file"];
            if (null == file || !file.FileName.ToLower().EndsWith(".zip"))
            {
                message = lang.Lang("请上传.ZIP格式的主题文件包。");

                ViewBag.message = message;
                return View("add");
            }

            themeService.LocalInstall(file.InputStream, file.FileName);

            return View("list");
        }
    }
}