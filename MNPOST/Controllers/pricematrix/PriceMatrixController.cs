using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.pricematrix
{
    public class PriceMatrixController : BaseController
    {
        // GET: PriceMatrix
        public ActionResult Show()
        {
            ViewBag.PostOffices = db.BS_PostOffices.ToList();
            ViewBag.PriceGroup = db.BS_PriceGroups.ToList();
            ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
            return View();
        }
        [HttpGet]
        public ActionResult getPriceMatrix(int? page, string fromDate, string toDate, string search = "")
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);
            if (String.IsNullOrEmpty(fromDate) || String.IsNullOrEmpty(toDate))
            {
                fromDate = DateTime.Now.ToString("dd/MM/yyyy");
                toDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            DateTime paserFromDate = DateTime.Now;
            DateTime paserToDate = DateTime.Now;

            try
            {
                paserFromDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                paserToDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
            }
            catch
            {
                paserFromDate = DateTime.Now;
                paserToDate = DateTime.Now;
            }

            var data = db.BS_PriceMaTrixs.Where(p => p.CreateDate >= paserFromDate.Date && p.CreateDate <= paserToDate.Date).ToList();

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
        //lay zone theo ma nhom
        [HttpGet]
        public ActionResult getRangeZone(string GroupID) 
        {
            var coddetail = db.BS_RangeZones.Where(p => p.GroupID == GroupID).ToList();

            List<RangeZoneInfo> detail = new List<RangeZoneInfo>();


            foreach (var item in detail)
            {
                detail.Add(new RangeZoneInfo()
                {
                    GroupID = item.GroupID,
                    ZoneID = item.ZoneID,
                    No = item.No
                });
            }
            return Json(new ResultInfo() { error = 0, msg = "", data = detail }, JsonRequestBehavior.AllowGet);
        }

        //lay weight theo ma nhom
        [HttpGet]
        public ActionResult getRangeWeight(string GroupID)
        {
            var coddetail = db.BS_RangeWeights.Where(p => p.GroupID == GroupID).ToList();

            List<RangeWeightInfo> detail = new List<RangeWeightInfo>();


            foreach (var item in detail)
            {
                detail.Add(new RangeWeightInfo()
                {
                    GroupID = item.GroupID,
                    FromWeight = item.FromWeight,
                    ToWeight = item.ToWeight,
                    IsNextWeight = item.IsNextWeight,
                    No = item.No
                });
            }
            return Json(new ResultInfo() { error = 0, msg = "", data = detail }, JsonRequestBehavior.AllowGet);
        }

        // update detail
        [HttpPost]
        public ActionResult updatePriceMatrix(List<IdentityCOD> detail, string documentID)
        {
            //update bảng master BS_PriceMatrix
            //update bảng BS_RangeValue
            //Update bảng BS_RangeZone
            //Update bảng BS_RangeWeight
            foreach (var item in detail)
            {
                if (String.IsNullOrEmpty(item.MailerID))
                    return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

                var check = db.MM_EmployeeDebitVoucherDetails.Where(p => p.DocumentID == documentID && p.MailerID == item.MailerID).FirstOrDefault();

                if (check == null)
                    return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

                check.ReciveCOD = item.ReciveCOD;
                db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();
            return Json(new ResultInfo() { }, JsonRequestBehavior.AllowGet);

        }
        
        [HttpPost]
        public ActionResult delete(string PriceMatrixID)
        {
            if (String.IsNullOrEmpty(PriceMatrixID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_PriceMaTrixs.Find(PriceMatrixID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            db.Entry(check).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);
        }
    }
}