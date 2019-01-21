using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers
{
    public class NoticeController : BaseController
    {
        // GET: Notice
        public ActionResult Send()
        {
            return View();
        }
    }
}