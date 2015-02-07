using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class CarteMap : MapBase<carte>
    {
        public CarteMap()
        {
            Map(m => m.name);
            Map(m => m.siteid);
            Map(m => m.action);
            Map(m => m.controller);
            Map(m => m.area);
            Map(m => m.icon);
            Map(m => m.order);
            Map(m => m.parent);
            Map(m => m.allowactions).CustomSqlType("text");
            Map(m => m.activeflag);
        }
    }
}