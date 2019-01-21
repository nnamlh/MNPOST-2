using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;

namespace MNPOST.Controllers.mnpostinfo
{
    public class ProvinceController : BaseController
    {
        //
        // GET: /Province/
        public ActionResult Show() 
        {
            return View();
        }
            
        [HttpGet]
        public ActionResult GetProvince(int? page, string search = "")
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);
            

            var data = db.BS_Provinces.Where(p=> p.ProvinceID.Contains(search) || p.ProvinceName.Contains(search)).ToList();

            ResultInfo result = new ResultWithPaging()
            {
                error = 0,
                msg = "",
                page = pageNumber,
                pageSize  = pageSize,
                toltalSize = data.Count(),
                data = data.Skip((pageNumber - 1)*pageSize).Take(pageSize).ToList()
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult create(BS_Provinces province)
        {

            if (String.IsNullOrEmpty(province.ProvinceID))
                return Json(new ResultInfo() {error =1 , msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Provinces.Find(province.ProvinceID);

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);

            db.BS_Provinces.Add(province);

            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = province }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult edit(BS_Provinces province)
        {
            if (String.IsNullOrEmpty(province.ProvinceID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Provinces.Find(province.ProvinceID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.ProvinceName = province.ProvinceName;
            check.CountryID = province.CountryID;
            check.ZoneID = province.ZoneID;


            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);

        }
	}
}