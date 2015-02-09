using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using QN.Service;
using NHibernate.Exceptions;

namespace QN.Web.Install
{
    public partial class Step3 : System.Web.UI.Page
    {
        private readonly SiteService siteService = new SiteService();

        public string RealUrl
        {
            get
            {
                return Request.Url.Scheme + "://" + Request.Url.Authority;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Server.MapPath("~/App_Data/install.lock")))
            {
                Response.Redirect("Index.aspx");
                return;
            }

            try
            {
                if (siteService.Count() > 0)
                {
                    SaveInstall();

                    Response.Redirect("Step4.aspx");
                    return;
                }
            }
            catch(GenericADOException)
            {

            }

            if (Request.ServerVariables["REQUEST_METHOD"] == "POST")
            {
                try
                {
                    string LoginName = Request.Form["LoginName"];
                    string Email = Request.Form["Email"];
                    string NiceName = Request.Form["NiceName"];
                    string Password = Request.Form["Password"];
                    string SiteName = Request.Form["SiteName"];
                    string SiteInfo = Request.Form["SiteInfo"];
                    string SiteDomain = Request.Form["SiteDomain"];

                    if (string.IsNullOrWhiteSpace(LoginName))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "账号不能为空！";
                        return;
                    }
                    else if (!new Regex("[a-zA-Z0-9]{4,20}").IsMatch(LoginName))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "账号为4~20之间的字符或数字！";
                    }
                    if (string.IsNullOrWhiteSpace(NiceName))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "昵称不能为空！";
                        return;
                    }
                    else if (NiceName.Length > 20)
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "昵称长度不能大于20个字符！";
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(Email))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "邮箱不能为空！";
                        return;
                    }
                    else if (!new Regex(@"[a-zA-Z0-9]{1,20}@[a-zA-Z0-9]{1,20}\.[a-zA-Z0-9]{1,5}").IsMatch(Email))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "邮箱格式不正确！";
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "密码不能为空！";
                        return;
                    }
                    else if (Password.Length > 20)
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "密码长度不能大于20个字符！";
                        return;
                    }


                    if (string.IsNullOrWhiteSpace(SiteDomain))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "网站地址不能为空！";
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(SiteName))
                    {
                        lblError.Visible = true;
                        lblError.InnerHtml = "网站名称不能为空！";
                        return;
                    }

                    CreateSite(SiteName, SiteInfo, SiteDomain, Email, LoginName, Password);
                    Response.Redirect("Step4.aspx");
                }
                catch (Exception ex)
                {
                    lblError.Visible = true;
                    lblError.InnerHtml = ex.Message;
                }
            }
        }

        private void CreateSite(string siteName, string siteInfo, string siteDomain, string useremail, string username, string pass)
        {
            site site = new site()
            {
                addr = "",
                copyright = "",
                date = DateTime.Now,
                domain = siteDomain,
                email = "",
                icpnumber = "",
                info = siteInfo,
                logo = "",
                name = siteName,
                order = 0,
                tel = "",
                theme = "Default"
            };

            user user = new user()
            {
                avatar = null,
                date = DateTime.Now,
                email = useremail,
                login = username,
                nicename = "",
                pass = QEncryption.MD5Encryption(pass),
                status = R.user_status_nomal,
                roleid = 1,
                siteid = 1
            };

            siteService.Add(site, user);

            SaveInstall();
        }

        private void SaveInstall()
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Server.MapPath("~/App_Data/install.lock")))
                {
                    sw.WriteLine("install time:" + DateTime.Now.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}