
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.Web.Routing;

namespace QN
{
    public class ThemeControllerFactory : System.Web.Mvc.DefaultControllerFactory
    {
        public override System.Web.Mvc.IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            try
            {
                return base.CreateController(requestContext, controllerName);
            }
            catch (HttpException ex)
            {
#if DEBUG
                throw ex;
#else
                throw new HttpException404(ex.Message);
#endif
            }
        }
    }
}
