using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class CommonController : AdminController
    {
        private readonly FileService fileService = new FileService();
        private readonly PostService postService = new PostService();
        private readonly TermService termService = new TermService();

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload(string view)
        {
            var file = Request.Files["file"];

            try
            {
                if (null != file)
                {
                    media media = fileService.Upload(file.InputStream, file.FileName, file.ContentType);

                    if(!string.IsNullOrWhiteSpace(view))
                    {
                        return PartialView(view, media);
                    }
                    else
                    {
                        return Json(new
                        {
                            success = true,
                            media = media
                        });
                    }
                }
                else
                {
                    throw new QRunException(lang.Lang("请选择文件。"));
                }

            }
            catch (Exception ex)
            {
                QLog.Error(ex);

                return Json(new
                {
                    success = false,
                    msg = ex.Message
                });
            }
        }

        public ActionResult MediaDialog()
        {
            return View();
        }

        public ActionResult Medias(int pageindex, string mimetype)
        {
            return View(new pager()
            {
                pageindex = pageindex,
                extendinfo = mimetype
            });
        }

        public ActionResult PostSlugExists(string slug, int id)
        {
            bool flag = true;

            if (postService.IsExistsSlug(slug, id))
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TermSlugExists(string slug, int id)
        {
            bool flag = true;

            if (termService.IsExistsSlug(slug, id))
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}