using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class TermsController : AdminController
    {
        private readonly TermService termService = new TermService();

        #region 编辑

        #region 文章分类

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPost(term term)
        {
            term.type = "post";
            return Modify(term, "list");
        }

        public ActionResult UpdatePost(int id)
        {
            ViewBag.Type = "post";
            ViewBag.ListTitle = lang.Lang("文章分类");
            ViewBag.active_nav = "posttermlist";
            ViewBag.ListView = "posts";

            term term = termService.Get(id);

            if (null == term)
            {
                return Jmp404();
            }

            return View("update", term);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePost(term term)
        {
            ViewBag.Type = "post";
            ViewBag.ListTitle = lang.Lang("文章分类");
            ViewBag.active_nav = "posttermlist";

            ViewBag.AddView = "addpost";
            ViewBag.UpdateView = "updatepost";
            ViewBag.ListView = "posts";

            return Modify(term, "update");
        }

        #endregion

        #region 图册分类

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAlbum(term term)
        {
            term.type = "album";
            return Modify(term, "list");
        }

        public ActionResult UpdateAlbum(int id)
        {
            ViewBag.Type = "album";
            ViewBag.ListTitle = lang.Lang("图册分类");
            ViewBag.active_nav = "albumtermlist";
            ViewBag.ListView = "albums";

            term term = termService.Get(id);

            if (null == term)
            {
                return Jmp404();
            }

            return View("update", term);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAlbum(term term)
        {
            ViewBag.Type = "album";
            ViewBag.ListTitle = lang.Lang("图册分类");
            ViewBag.active_nav = "albumtermlist";

            ViewBag.AddView = "addalbum";
            ViewBag.UpdateView = "updatealbum";
            ViewBag.ListView = "albums";

            return Modify(term, "update");
        }

        #endregion

        private ActionResult Modify(term term, string view)
        {
            if (!ModelState.IsValid)
            {
                return View(view, term);
            }

            if (term.id == 0)
            {
                if (string.IsNullOrWhiteSpace(term.name))
                {
                    term.name = lang.Lang("新分类");
                }

                termService.Add(term);
            }
            else
            {
                termService.Update(term);
            }

            string listview = string.Empty;

            if (term.type == "post")
            {
                listview = "Posts";
            }
            else if (term.type == "album")
            {
                listview = "Albums";
            }

            return RedirectToAction(listview, new { state = "new", id = term.id });
        }

        #endregion

        #region 查询

        public ActionResult Posts(string action)
        {
            ViewBag.Title = lang.Lang("文章分类");
            ViewBag.active_nav = "posttermlist";
            ViewBag.AddView = "addpost";
            ViewBag.ListView = "posts";
            ViewBag.UpdateView = "updatepost";

            ViewBag.Type = "post";

            return View("list", new term());
        }

        public ActionResult Albums(string action)
        {
            ViewBag.Title = lang.Lang("图册分类");
            ViewBag.active_nav = "albumtermlist";
            ViewBag.AddView = "addalbum";
            ViewBag.ListView = "albums";
            ViewBag.UpdateView = "updatealbum";

            ViewBag.Type = "album";

            return View("List", new term());
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