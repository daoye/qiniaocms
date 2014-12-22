using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace QN
{
    internal static class ThemeHelper
    {
        public static bool IsForword(RouteData routeData)
        {
            if (routeData.DataTokens["area"] != null)
            {
                return false;
            }
            else
            {
                try
                {
                    string controller = routeData.GetRequiredString("controller");

                    if (string.Compare(controller, "account", true) == 0)
                    {
                        return false;
                    }
                }
                catch
                {
                    return true;
                }
            }

            return true;
        }

    }
}