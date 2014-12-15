#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	UserService.cs
	Author:		DaoYe
	History: 	30/11/2014 18:58 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace QN.Service
{
    /// <summary>
    /// 用户管理服务
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// 当前会话是否是已经授权登录过的会话
        /// </summary>
        public static bool IsLogined
        {
            get
            {
                if (null == HttpContext.Current.User)
                {
                    return false;
                }

                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// 获取当前用户的角色ID
        /// </summary>
        public static int Roles
        {
            get
            {
                return CurrentUser.member.role;
            }
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public static QUser CurrentUser
        {
            get
            {
                return HttpContext.Current.User as QUser;
            }
        }
    }
}
