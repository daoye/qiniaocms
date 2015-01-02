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
    }
}