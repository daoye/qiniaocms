using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class CommentMap : MapBase<comment>
    {
        public CommentMap()
        {
            Map(m => m.postid);
            Map(m => m.author);
            Map(m => m.authoremail);
            Map(m => m.authorip);
            Map(m => m.authorurl);
            Map(m => m.content).CustomSqlType("text").LazyLoad();
            Map(m => m.date);
            Map(m => m.memberid);
            Map(m => m.parent);
            Map(m=>m.status);
            Map(m => m.deep);
            Map(m => m.deeppath).CustomSqlType("text");
        }
    }
}
