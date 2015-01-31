using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class siteview : model<siteview>
    {
        public site site { get; set; }

        public user user { get; set; }
    }
}
