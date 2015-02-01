using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class modifypassview : model<modifypassview>
    {
        [DisplayName("用户名")]
        public string login { get; set; }

        [DisplayName("原密码")]
        [QRequired]
        [QStringLength(50, MinimumLength = 4)]
        [Remote("PassIsRight", "Mine", "Admin", AdditionalFields = "id", ErrorMessage = "原密码不正确。")]
        public string oldpass { get; set; }

        [DisplayName("新密码")]
        [QRequired]
        [QStringLength(50, MinimumLength = 4)]
        public string newpass { get; set; }

        [DisplayName("再输入一次")]
        [QRequired]
        [QStringLength(50, MinimumLength = 4)]
        [Compare("newpass", ErrorMessage = "两次密码输入不一致。")]
        public string newpass2 { get; set; }
    }
}