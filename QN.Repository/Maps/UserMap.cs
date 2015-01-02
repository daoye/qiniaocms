using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Repository.Maps
{
    public class UserMap : MapBase<user>
    {
        public UserMap()
        {
            Map(m => m.siteid);
            Map(m => m.email);
            Map(m => m.tel);
            Map(m => m.login);
            Map(m => m.logined);
            Map(m => m.nicename);
            Map(m => m.pass);
            Map(m => m.registered);
            Map(m => m.roleid);
            Map(m => m.status);
            Map(m => m.super);
            Map(m => m.url);
        }
    }
}
