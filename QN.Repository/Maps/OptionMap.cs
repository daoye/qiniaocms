using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class OptionMap : MapBase<option>
    {
        public OptionMap()
        {
            Map(m => m.autoload);
            Map(m => m.siteid).UniqueKey("name");
            Map(m => m.name).Length(800).UniqueKey("siteid");
            Map(m => m.value).CustomSqlType("text").LazyLoad();
        }
    }
}
