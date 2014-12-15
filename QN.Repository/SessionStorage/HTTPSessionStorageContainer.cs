
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace QN.Repository
{
    public class HTTPSessionStorageContainer : ISessionStorageContainer
    {
        private const string sessionKey = "nhibernate_session";

        public ISession GetCurrentSession()
        {
            ISession session = null;

            if (System.Web.HttpContext.Current.Items.Contains(sessionKey)) session = System.Web.HttpContext.Current.Items[sessionKey] as ISession;

            return session;
        }

        public void Store(ISession _session)
        {
            if (System.Web.HttpContext.Current.Items.Contains(sessionKey)) System.Web.HttpContext.Current.Items[sessionKey] = _session;
            else System.Web.HttpContext.Current.Items.Add(sessionKey, _session);
        }

        public void Clear()
        {
            if (System.Web.HttpContext.Current.Items.Contains("nhibernate_session"))
            {
                System.Web.HttpContext.Current.Items.Remove("nhibernate_session");
            }
        }
    }
}
