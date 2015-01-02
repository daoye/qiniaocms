using System.Web.Mvc;

namespace QN.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "admin/{controller}/{action}/{id}",
                new { controller = "home", action = "dashboard", id = UrlParameter.Optional },
                namespaces: new string[] { "QN.Controllers.Areas.Admin" }
            );
        }
    }
}
