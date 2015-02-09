using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using NHibernate;
using System.Data;
using System.Data.SQLite;
using System.IO;
using QN.Repository;
using NHibernate.Cfg;
using System.Xml.Linq;
using System.Xml;

namespace QN.Web.Install
{
    public partial class Step2 : System.Web.UI.Page
    {
        string DbName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Server.MapPath("~/App_Data/install.lock")))
            {
                Response.Redirect("Index.aspx");
                return;
            }

            if (!R.session.Connection.ConnectionString.Contains(":memory:"))
            {
                Response.Redirect("Step3.aspx");
                return;
            }

            if (Request.ServerVariables["REQUEST_METHOD"] == "POST")
            {
                try
                {
                    DbName = Request.Form["DbName"];

                    InitSQLite();

                    Response.Redirect("Step3.aspx");
                }
                catch (Exception ex)
                {
                    lblError.Visible = true;
                    lblError.InnerHtml = ex.Message;
                }
            }
        }

        private void InitSQLite()
        {
            DbName = QFile.SecurityPath(DbName) + ".db3";

            CreateDatabase(Server.MapPath("~/App_Data/" + DbName));
            string connStr = "Data Source=|DataDirectory|" + DbName + ";Version=3";

            string dbconfigPath = Server.MapPath("~/App_Data/db.config");
            XNamespace ns = "urn:nhibernate-configuration-2.2";
            XDocument xdoc = XDocument.Load(dbconfigPath);
            XElement xel = xdoc.Root.Descendants(ns + "property").FirstOrDefault(m => m.Attribute("name").Value == "connection.connection_string");
            if (null != xel)
            {
                xel.Value = connStr;
            }
            else
            {
                throw new Exception("db.confg中存在配置错误。");
            }

            xdoc.Save(dbconfigPath);

            Configuration cfg = new Configuration();
            cfg.Configure();
            cfg.Properties["connection.connection_string"] = connStr;

            SessionFactory.Instance.UpdateConfigPropertys(cfg);

            using (System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("scripts/sqlite.sql")))
            {
                string sql = sr.ReadToEnd();

                using (IDbTransaction trans = R.session.Connection.BeginTransaction())
                {
                    try
                    {
                        IDbCommand cmd = R.session.Connection.CreateCommand();
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();

                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 创建一个数据库，若数据库已存在，则不创建
        /// </summary>
        /// <param name="dbname"></param>
        public static void CreateDatabase(string dbname)
        {
            if (string.IsNullOrEmpty(dbname))
            {
                throw new ArgumentNullException("dbname");
            }

            if (!File.Exists(dbname))
            {
                SQLiteConnection.CreateFile(dbname);
            }
        }
    }
}