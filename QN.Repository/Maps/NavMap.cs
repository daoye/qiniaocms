using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class NavMap : MapBase<nav>
    {
        public NavMap()
        {
            Map(m => m.siteid);
            Map(m => m.icon);
            Map(m => m.name);
            Map(m => m.order);
            Map(m => m.parent);
            Map(m => m.type);
            Map(m => m.url);
            Map(m => m.deep);
            Map(m => m.deeppath).CustomSqlType("text");
        }
    }
}
