﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN.Controllers.Areas.Admin
{
    public class HomeController : AdminController
    {
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}
