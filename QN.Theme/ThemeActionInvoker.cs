using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class ThemeActionInvoker : ControllerActionInvoker
    {
        protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName)
        {
            return base.FindAction(controllerContext, controllerDescriptor, "Index");
        }
    }
}
