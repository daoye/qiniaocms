using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text.RegularExpressions;

namespace QN.Web.Install
{
    public partial class Step4 : System.Web.UI.Page
    {
        public string homepage
        {
            get
            {
                string root = Request.ApplicationPath;
                if(!root.EndsWith("/"))
                {
                    root += "/";
                }

                return root;
            }
        }

        public string accountpage
        {
            get
            {
                return homepage + "account/login";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}