using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class PostMetaMap : MapBase<postmeta>
    {
        public PostMetaMap()
        {
            Map(m => m.postid);
            Map(m => m.key).Length(1000);
            Map(m => m.value).CustomSqlType("text");
        }
    }
}
