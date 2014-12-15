using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class MemberMetaMap : MapBase<membermeta>
    {
        public MemberMetaMap()
        {
            Map(m => m.memberid);
            Map(m => m.key).Length(1000);
            Map(m => m.value).CustomSqlType("text");
        }
    }
}
