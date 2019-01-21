using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOST.Models;
using MNPOSTCOMMON;
namespace MNPOST.Controllers.mailer
{
    public class MailerChangeInfoController : Controller
    {
        // GET: MailerChangeInfo
        public ActionResult Show()
        {
            return View();
        }
        [HttpPost]
        public ActionResult edit(MM_Mailers mm)
        {
            if (String.IsNullOrEmpty(mm.MailerID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.MM_Mailers.Where(p => p.MailerID == mm.MailerID).FirstOrDefault();

            if (check != null)
            {
                check.Quantity = mm.Quantity;
                check.Weight = mm.Weight;
                check.Price = mm.Price;
                db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult getmailer(string mailerid)
        {
            var data = db.MM_Mailers.Where(p => p.MailerID == mailerid).First();

            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}