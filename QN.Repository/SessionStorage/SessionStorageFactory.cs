
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository
{
    public static class SessionStorageFactory
    {
        private static ISessionStorageContainer httpStorageContainer;
        private static ISessionStorageContainer threadStorageContainer;

        public static ISessionStorageContainer GetSessionStorageContainer()
        {
            if (System.Web.HttpContext.Current == null)
            {
                if (null == threadStorageContainer)
                {
                    threadStorageContainer = new ThreadSessionStorageContainer();
                }

                return threadStorageContainer;
            }
            else
            {
                if(null== httpStorageContainer)
                {
                    httpStorageContainer = new HTTPSessionStorageContainer();
                }

                return httpStorageContainer;
            }
        }
    }
}
