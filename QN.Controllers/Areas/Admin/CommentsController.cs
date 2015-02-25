using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class CommentsController : AdminController
    {
        private readonly CommentService commentService = new CommentService();

        #region 编辑

        public ActionResult Add()
        {
            return View(new comment() { author = CurrentUser.info.nicename });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(comment comment)
        {
            comment.date = DateTime.Now;

            return Modify(comment);
        }

        public ActionResult Update(int id)
        {
            comment comment = commentService.Get(id);

            if (null == comment)
            {
                return Jmp404();
            }

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(comment comment)
        {
            return Modify(comment);
        }

        private ActionResult Modify(comment comment)
        {
            if (!ModelState.IsValid)
            {
                return View(comment);
            }

            if (comment.id == 0)
            {
                comment.status = R.status_publish;

                commentService.Add(comment);
            }
            else
            {
                commentService.Update(comment);
            }

            return RedirectToAction("list", new { state = "new", id = comment.id });
        }

        [HttpPost]
        public ActionResult Reply(comment comment)
        {
            comment.date = DateTime.Now;
            comment.author = CurrentUser.info.nicename;
            comment.userid = CurrentUser.info.id;
            comment.authoremail = CurrentUser.info.email;
            comment.authorip = Request.UserHostAddress;
            comment.authorurl = CurrentUser.info.url;
            comment.siteid = R.siteid;
            comment.status = R.status_publish;

            commentService.Add(comment);

            return GoBack();
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
            commentService.Remove(id);

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
    }
}