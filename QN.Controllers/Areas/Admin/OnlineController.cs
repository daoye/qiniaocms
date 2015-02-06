using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using QN.Service;


namespace QN.Controllers.Areas.Admin
{
    public class OnlineController : AdminController
    {
        private readonly OnlineInfoService onlineService = new OnlineInfoService();

        public ContentResult  Weibo()
        {
            IList<OnlineMicroblogDTO> result = null;
            result = QCache.Get<IList<OnlineMicroblogDTO>>("onlinemicroblog-list-cache-id");
            if (null == result)
            {
                result = onlineService.GetMicroblogList();
                if (result != null)
                {
                    QCache.Set("onlinemicroblog-list-cache-id", result, 30, null);
                }
            }

            StringBuilder sb = new StringBuilder();
            result.ToList().ForEach(f =>
            {
                sb.Append("<div class=\"thread\">");
                sb.Append(f.Content);
                sb.Append("<div class=\"thread-info\">");
                sb.Append(f.Date);
                sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"" + f.Url + "\" title=\"\" target=\"_blank\">查看</a>");
                sb.Append("</div>");
                sb.Append("</div>");
            });

            return Content(sb.ToString());

        }
    }
}
