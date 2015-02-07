using QN.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class QAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly RoleService roleService = new RoleService();
        private readonly ACLService aclService = new ACLService();
        private readonly CarteService carteService = new CarteService();

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            bool flag = base.AuthorizeCore(httpContext);
            try
            {
                if (flag)
                {
                    QUser usr = httpContext.User as QUser;
                    role rl = roleService.Get(usr.info.roleid);
                    IList<acl> acls = null;
                    MvcHandler handler = null;
                    object action = null;
                    object controller = null;
                    object area = null;

                    handler = httpContext.CurrentHandler as MvcHandler;
                    if(!handler.RequestContext.RouteData.DataTokens.TryGetValue("area", out area))
                    {
                        area = string.Empty;
                    }
                    if(!handler.RequestContext.RouteData.Values.TryGetValue("action", out action))
                    {
                        action = string.Empty;
                    }
                    if(!handler.RequestContext.RouteData.Values.TryGetValue("controller", out controller))
                    {
                        controller = string.Empty;
                    }

                    if (null != area &&
                        string.Compare("admin", area.ToString(), true) == 0 &&
                        null != controller &&
                        string.Compare("home", controller.ToString(), true) != 0)
                    {
                        flag = false;

                        if (null == rl)
                        {
                            return false;
                        }

                        acls = aclService.List(rl.id);

                        if (null == acls)
                        {
                            return false;
                        }

                        foreach (acl a in acls)
                        {
                            carte c = carteService.Get(a.carteid);
                            if (null != c)
                            {
                                if (string.Compare(c.controller, controller.ToString(), true) == 0 && string.Compare(c.area, area.ToString(), true) == 0)
                                {
                                    if (c.allowactions == null)
                                    {
                                        c.allowactions = string.Empty;
                                    }

                                    if (c.allowactions.ToLower().Contains(action.ToString().ToLower()) || string.Compare(c.action, action.ToString(), true) == 0)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                QLog.Error(ex);
            }

            return flag;
        }
    }
}