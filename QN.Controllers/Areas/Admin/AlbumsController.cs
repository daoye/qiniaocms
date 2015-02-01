using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class AlbumsController : AdminController
    {
        private readonly PostService postService = new PostService();
        private readonly TermService termService = new TermService();

        #region 编辑

        #region 相册分类

        public ActionResult Add()
        {
            return View(new term());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(term term)
        {
            term.type = "album";
            return Modify(term, "list");
        }

        public ActionResult Update(int id)
        {
            term term = termService.Get(id);

            if (null == term)
            {
                return Jmp404();
            }

            return View("update", term);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(term term)
        {
            return Modify(term, "update");
        }

        public ActionResult UpdatePost(int id)
        {
            post post = postService.Get(id);

            if (null == post)
            {
                return Jmp404();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePost(post post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }

            postService.Update(post);

            return RedirectToAction("postlist", new { state = "update", id = post.termid });
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
                    term.name = lang.Lang("新相册");
                }

                termService.Add(term);
            }
            else
            {
                termService.Update(term);
            }

            return RedirectToAction("list", new { state = "new", id = term.id });
        }

        #endregion

        #region 查询


        public ActionResult List()
        {
            return View();
        }


        public ActionResult PostList(int id)
        {
            term term = termService.Get(id);

            return View(term);
        }

        public ActionResult PostListItem(int id, int termid)
        {
            post model = postService.Get(id);

            post p = new post();
            p.AssigningForm(model, new string[] { "id"});
            p.filepostid = id;
            p.termid = termid;
            p.date = DateTime.Now;
            p.modified = DateTime.Now;
            p.type = "album";
            p.status = R.status_publish;
            p.siteid = R.siteid;
            postService.Add(p);

            return PartialView("postlistitem", p);
        }

        #endregion

        #region 删除

        public ActionResult Delete(int[] id)
        {
            termService.Remove(id);

            return GoBack();
        }

        public ActionResult DeletePost(int[] id)
        {
            postService.Remove(id);

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

        public ActionResult OperatePost(string act, int[] id)
        {
            if ("del".Equals(act))
            {
                return DeletePost(id);
            }

            return GoBack();
        }

        #endregion
    }
}