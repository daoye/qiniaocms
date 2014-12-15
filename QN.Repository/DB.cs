#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	DB.cs
	Author:		DaoYe
	History: 	30/11/2014 18:56 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using QN.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace QN
{
    public static class DB
    {
        private static string key = "db_entity_data_context_9434";
        private static EntityDataContext Create()
        {
            string conn = ConfigurationManager.ConnectionStrings["QNDBConnection"].ConnectionString;
            return  new EntityDataContext(conn);
        }

        public static EntityDataContext Context
        {
            get
            {
                EntityDataContext context = System.Web.HttpContext.Current.Items[key] as EntityDataContext;
                if (null == context)
                {
                    context = Create();
                    System.Web.HttpContext.Current.Items[key] = context;
                }

                return context;
            }
        }
    }
}
