using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class MapBase<T> : ClassMap<T> where T : entity<T>
    {
        public MapBase()
        {
            Id(m => m.id).GeneratedBy.Increment();
        }
    }
}
