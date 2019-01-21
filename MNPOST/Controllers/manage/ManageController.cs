using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers.manage
{
    public class ManageController : AdminController
    {

        public ActionResult Index()
        {
            return View();
        }

	}
}