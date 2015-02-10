using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    /// <summary>
    /// 注册
    /// </summary>
    public class sinupview : model<sinupview>
    {
        [DisplayName("昵称")]
        [QRequired]
        public string nicename { get; set; }

        [DisplayName("联系邮箱")]
        [QRequired]
        public string email { get; set; }

        [DisplayName("密码")]
        [QRequired]
        [QStringLength(50, MinimumLength = 4)]
        public string pass { get; set; }

        [DisplayName("用户名")]
        [QRequired]
        [QStringLength(50, MinimumLength = 2)]
        [Remote("LoginExists", "Users", "Admin", ErrorMessage = "用户名已存在。")]
        public string login { get; set; }
    }
}