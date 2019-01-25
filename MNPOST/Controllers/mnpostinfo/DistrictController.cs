using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;

namespace MNPOST.Controllers.mnpostinfo
{
    public class DistrictController : BaseController
    {
        //
        // GET: /District/
        public ActionResult Show()
        {
            ViewBag.AllProvince = db.BS_Provinces.ToList();
            return View();
        }


        [HttpGet]
        public ActionResult GetDistrict(int? page, string search = "")
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);


            var data = db.BS_Districts.Where(p => p.ProvinceID == search).ToList();

            if(search == "")
            {
                data = db.BS_Districts.OrderBy(p=> p.ProvinceID).ToList();
            }

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

        [HttpGet]
        public ActionResult VSVX(string code, bool vsvx)
        {
            var find = db.BS_Districts.Where(p => p.DistrictID == code).FirstOrDefault();

            if(find != null)
            {
                find.VSVS = vsvx;
                db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new ResultInfo()
            {

            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult create(BS_Districts district)
        {

            if (String.IsNullOrEmpty(district.DistrictID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Districts.Find(district.DistrictID);

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);

            db.BS_Districts.Add(district);

            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = district }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult edit(BS_Districts district)
        {
            if (String.IsNullOrEmpty(district.DistrictID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Districts.Find(district.DistrictID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.DistrictName = district.DistrictName;
            check.ProvinceID = district.ProvinceID;

            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);

        }
    }
}
