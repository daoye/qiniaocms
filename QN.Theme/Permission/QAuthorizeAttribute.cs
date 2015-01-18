using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class QAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            bool flag = base.AuthorizeCore(httpContext);
            if (flag)
            {
                
            }

            return flag;
        }
    }
}
