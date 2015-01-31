using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class MediasController : AdminController
    {
        private readonly PostService postService = new PostService();

        #region 编辑

        public ActionResult Add()
        {
            return View(new post() { author = CurrentUser.info.nicename });
        }

        public ActionResult Update(int id)
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
        public ActionResult Update(post post)
        {
            return Modify(post);
        }

        private ActionResult Modify(post post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }

            if (post.id == 0)
            {
                if (string.IsNullOrWhiteSpace(post.title))
                {
                    post.title = lang.Lang("新文件");
                }

                post.status = R.status_publish;
                postService.Add(post);
            }
            else
            {
                postService.Update(post);
            }

            return RedirectToAction("list", new { state = "new", id = post.id });
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
            postService.Remove(id);

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