﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Infrastructure.Security;

namespace Web.Areas.Admin.Controllers
{
    [TrackyAuthorizeAttribute(Users = "Admin", LoginArea = "Admin", LoginController = "AdminLogin")]
    public class AdminDashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
