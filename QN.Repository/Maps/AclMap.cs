using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class AclMap : MapBase<acl>
    {
        public AclMap()
        {
            Map(m => m.roleid);
            Map(m => m.carteid);
        }
    }
}
