using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace QN.Web.install
{
    public class Info
    {
        public string Path { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }
    }

    public partial class step1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Server.MapPath("~/App_Data/install.lock")))
            {
                Response.Redirect("index.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int error = 0;
                string[] dirs = new string[] { "App_Data", "content/upload", "sites", "themes" };

                List<Info> list = new List<Info>();

                foreach (string dir in dirs)
                {
                    Info info = new Info();
                    info.Path = dir;
                    info.Description = "正常";
                    try
                    {
                        string fullDir = Server.MapPath("~/" + dir);

                        if (!Directory.Exists(fullDir))
                        {
                            Directory.CreateDirectory(fullDir);
                        }

                        string p = fullDir + "/testright.txt";
                        using (System.IO.StreamWriter sw = new StreamWriter(p))
                        {
                            sw.WriteLine("ok");
                            info.Icon = "images/ok.png";
                        }
                        if (File.Exists(p))
                        {
                            File.Delete(p);
                        }
                    }
                    catch
                    {
                        info.Description = "要求此目录有读写权限。";
                        info.Icon = "images/error.png";
                        error++;
                    }

                    list.Add(info);
                }

                this.rptList.DataSource = list;
                this.rptList.DataBind();

                if (error == 0)
                {
                    btnNext.HRef = "step2.aspx";
                    btnNext.InnerText = "下一步";
                }
            }
        }
    }
}