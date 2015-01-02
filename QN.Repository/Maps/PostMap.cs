using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class PostMap : MapBase<post>
    {
        public PostMap()
        {
            Map(m => m.siteid);
            Map(m => m.name).Length(1000);
            Map(m => m.slug);
            Map(m => m.author);
            Map(m => m.content).CustomSqlType("text").LazyLoad();
            Map(m => m.date);
            Map(m => m.excerpt).Length(4000);
            Map(m => m.mimetype);
            Map(m => m.modified);
            Map(m => m.order);
            Map(m => m.parent);
            Map(m => m.pingstatus);
            Map(m => m.posttype);
            Map(m => m.status);
            Map(m => m.title).Length(1000);
            Map(m => m.commentcount);
            Map(m => m.viewcount);
            Map(m => m.termid);
            Map(m => m.pic);
            Map(m => m.piclink);
            Map(m => m.filepostid);
        }
    }
}