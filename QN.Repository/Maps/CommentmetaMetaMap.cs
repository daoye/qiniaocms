using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class CommentmetaMetaMap : MapBase<commentmeta>
    {
        public CommentmetaMetaMap()
        {
            Map(m => m.commentid);
            Map(m => m.key).Length(1000);
            Map(m => m.value).CustomSqlType("text");
        }
    }
}
