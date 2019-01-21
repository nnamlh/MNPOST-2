using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.mnpostinfo
{
    public class ReturnReasonController : BaseController
    {
        // GET: ReturnReason
        public ActionResult Show()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetReason(int? page, string search = "")
        {
            int pageSize = 50;
            int pageNumber = (page ?? 1);


            var data = db.BS_ReturnReasons.Where(p => p.ReasonName.Contains(search)).ToList();

            ResultInfo result = new ResultWithPaging()
            {
                error = 0,
                msg = "",
                page = pageNumber,
                pageSize = pageSize,
                toltalSize = data.Count(),
                data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult create(BS_ReturnReasons reason)
        {

            if (String.IsNullOrEmpty(reason.ReasonID.ToString()))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_ReturnReasons.Find(reason.ReasonID);

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);


            reason.IsActive = true;
            db.BS_ReturnReasons.Add(reason);

            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = reason }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult edit(BS_ReturnReasons reason)
        {
            if (String.IsNullOrEmpty(reason.ReasonID.ToString()))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_ReturnReasons.Find(reason.ReasonID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.ReasonName = reason.ReasonName;

            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);

        }
    }
}