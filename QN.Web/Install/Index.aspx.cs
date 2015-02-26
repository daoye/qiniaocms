using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QN.Web.install
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Server.MapPath("~/App_Data/install.lock");
            bool isCanInstall  = !System.IO.File.Exists(path);

            InstallBtn.Visible = isCanInstall;
            MsgBox.Visible = !isCanInstall;
        }
    }
}