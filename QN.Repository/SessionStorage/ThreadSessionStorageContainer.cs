
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Collections;
using System.Threading;

namespace QN.Repository
{
    public class ThreadSessionStorageContainer : ISessionStorageContainer
    {
        private static readonly Hashtable sessionTable = new Hashtable();

        public ISession GetCurrentSession()
        {
            ISession session= null;

            if (sessionTable.Contains(Thread.CurrentThread.Name)) session = sessionTable[Thread.CurrentThread.Name] as ISession;

            return session;
        }

        public void Store(ISession _session)
        {
            if (sessionTable.Contains(Thread.CurrentThread.Name)) sessionTable[Thread.CurrentThread.Name] = _session;
            else sessionTable.Add(Thread.CurrentThread.Name, _session);
        }


        public void Clear()
        {
            sessionTable.Clear();
        }
    }
}
