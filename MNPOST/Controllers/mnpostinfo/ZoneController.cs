using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.mnpostinfo
{
    public class ZoneController : BaseController
    {
        //
        // GET: /Zone/
        public ActionResult Show()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetZone(int? page, string search = "")
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);


            var data = db.BS_Zones.Where(p => p.ZoneID.Contains(search) || p.ZoneName.Contains(search)).ToList();

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
        public ActionResult create(BS_Zones zone)
        {

            if (String.IsNullOrEmpty(zone.ZoneID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Zones.Find(zone.ZoneID);

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);


            zone.IsActive = true;
            zone.CreateDate = DateTime.Now;
            db.BS_Zones.Add(zone);

            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = zone }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult edit(BS_Zones zone)
        {
            if (String.IsNullOrEmpty(zone.ZoneID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_Zones.Find(zone.ZoneID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.ZoneName = zone.ZoneName;
            check.IsActive = zone.IsActive;
            check.UpdateDate = DateTime.Now;

            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);

        }
	}
}