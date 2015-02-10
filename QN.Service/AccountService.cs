#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/11/30
	FileName: 	AccountService.cs
	Author:		DaoYe
	History: 	30/11/2014 18:57 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using NHibernate.Criterion;
using QN.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace QN.Service
{
    public enum LoginError
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        OK,

        /// <summary>
        /// 验证码错误
        /// </summary>
        CodeError,

        /// <summary>
        /// 没有找到用户
        /// </summary>
        NotFoundUser
    }


    /// <summary>
    /// 账户管理服务
    /// </summary>
    public class AccountService
    {
        /// <summary>
        /// 用户登录，使用Cookie方式
        /// </summary>
        /// <returns>登录结果</returns>
        public LoginError Login(string loginname, string pass, string code)
        {
            //if (!QSession.HasKey("code") || string.Compare(QSession.Get("code").ToString(), Code, true) != 0)
            //{
            //    return LoginError.CodeError;
            //}
            //else
            //{
            //    QSession.Remove("code");
            //}

            user member = R.session.CreateCriteria<user>()
                                     .Add(Expression.Eq("login", loginname))
                                     .Add(Expression.Eq("pass", QEncryption.MD5Encryption(pass)))
                                     .Add(Expression.Eq("siteid", R.siteid))
                                     .List<user>()
                                     .FirstOrDefault();

            if (null != member)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                                            member.id.ToString(),
                                                            DateTime.Now,
                                                            DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                                                            false,
                                                            member.roleid.ToString());


                HttpCookie ticketCookie = new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(ticket)
                );
                ticketCookie.HttpOnly = true;
                ticketCookie.Expires = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);

                HttpCookie logintime = new HttpCookie("logintime", member.logined.ToString("yyyy-MM-dd HH:mm:ss"));
                logintime.HttpOnly = true;
                logintime.Expires = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);

                HttpContext.Current.Response.Cookies.Add(ticketCookie);
                HttpContext.Current.Response.Cookies.Add(logintime);

                member.logined = DateTime.Now;

                R.session.Update(member);

                return LoginError.OK;
            }

            return LoginError.NotFoundUser;
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public void Logout()
        {
            FormsAuthentication.SignOut();
            QSession.Clear();

            HttpCookie aCookie;
            string cookieName;
            int limit = HttpContext.Current.Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                cookieName = HttpContext.Current.Request.Cookies[i].Name;
                aCookie = new HttpCookie(cookieName);
                aCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(aCookie);
            }
        }

        /// <summary>
        /// 用户授权处理
        /// </summary>
        public static void AuthorizeProcess()
        {
            var id = HttpContext.Current.User.Identity as FormsIdentity;
            ////flash的cookie参数会通过Post方式传上来
            //if (null == id && !string.IsNullOrWhiteSpace(HttpContext.Current.Request.Form[FormsAuthentication.FormsCookieName]))
            //{
            //    FormsAuthenticationTicket ticket  = FormsAuthentication.Decrypt(HttpContext.Current.Request.Form[FormsAuthentication.FormsCookieName]);
            //    id = new FormsIdentity(ticket);
            //}

            if (null != id && id.IsAuthenticated)
            {
                var roles = id.Ticket.UserData.Split(',');
                QUser user = new QUser(id, roles);
                HttpContext.Current.User = user;

                if (null != user.info)
                {
                    //延长票据有效期
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                                                user.info.id.ToString(),
                                                                DateTime.Now,
                                                                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                                                                false,
                                                                user.info.roleid.ToString());

                    HttpCookie ticketCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                    ticketCookie.Expires = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);
                    ticketCookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add(ticketCookie);

                    HttpCookie logintime = HttpContext.Current.Request.Cookies["logintime"];
                    if (null != logintime)
                    {
                        logintime.Expires = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);
                        logintime.HttpOnly = true;
                        HttpContext.Current.Response.Cookies.Add(logintime);
                    }
                }
            }
        }
    }
}