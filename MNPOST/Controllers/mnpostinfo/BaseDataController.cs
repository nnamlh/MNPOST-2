using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers.mnpostinfo
{
    public class BaseDataController : BaseController
    {
        // GET: BaseData
        public ActionResult ReasonReturn()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAllReason()
        {
            var data = db.BS_ReturnReasons.Where(p=> p.ReasonID != 3).Select(p => new { code = p.ReasonID, name = p.ReasonName }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost] 
        public ActionResult ReasonEdit(int code, string name)
        {
            var find = db.BS_ReturnReasons.Find(code);

            if(find != null)
            {
                find.ReasonName = name;
                db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReasonAdd(string name)
        {
            var insData = new BS_ReturnReasons()
            {
                IsActive = true,
                ReasonName = name
            };
            db.BS_ReturnReasons.Add(insData);
            db.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReasonDelete(int code)
        {
            var find = db.BS_ReturnReasons.Find(code);

            if (find != null)
            {
                db.BS_ReturnReasons.Remove(find);
                db.SaveChanges();
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}