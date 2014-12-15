using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class RoleMap : MapBase<role>
    {
        public RoleMap()
        {
            Map(m => m.siteid);
            Map(m => m.name).Length(1000);
        }
    }
}
