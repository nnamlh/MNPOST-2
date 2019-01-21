using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.mnpostinfo
{
    public class WardController : BaseController
    {
        //
        // GET: /Ward/
        public ActionResult Show()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetWard(int? page, string search = "")
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);


            var data = db.BS_Wards.Where(p => p.WardID.Contains(search) || p.WardName.Contains(search)).ToList();

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
        public ActionResult create(BS_Wards ward)
        {

            if (String.IsNullOrEmpty(ward.WardID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Wards.Find(ward.WardID);

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);

            db.BS_Wards.Add(ward);

            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = ward }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult edit(BS_Wards ward)
        {
            if (String.IsNullOrEmpty(ward.WardID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Wards.Find(ward.WardID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.WardName = ward.WardName;
            check.DistrictID = ward.DistrictID;


            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);

        }
	}
}