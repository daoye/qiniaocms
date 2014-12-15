
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace QN.Repository
{
    public interface ISessionStorageContainer
    {
        ISession GetCurrentSession();

        void Store(ISession _session);

        void Clear();
    }
}
