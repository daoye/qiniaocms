using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class TermMap : MapBase<term>
    {
        public TermMap()
        {
            Map(m => m.siteid);
            Map(m => m.name);
            Map(m => m.count);
            Map(m => m.deep);
            Map(m => m.deeppath).CustomSqlType("text");
            Map(m => m.info).CustomSqlType("text");
            Map(m => m.name);
            Map(m=>m.order);
            Map(m => m.parent);
            Map(m=>m.pic);
            Map(m=>m.slug);
            Map(m=>m.type);
            //Map(m => m.super);
            Map(m => m.date);
            Map(m => m.modified);
        }
    }
}
