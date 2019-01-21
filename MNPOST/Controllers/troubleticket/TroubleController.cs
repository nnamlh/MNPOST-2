using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.troubleticket
{
    public class TroubleController : BaseController
    {
        // GET: Trouble
        public ActionResult Show()
        {
            ViewBag.AllEmployee = db.BS_Employees.Where(p => p.IsActive == true).Select(p => new { EmployeeID = p.EmployeeID, EmployeeName = p.EmployeeName }).ToList();
            ViewBag.AllPostOffice = db.BS_PostOffices.ToList();
            ViewBag.AllCustomer = db.BS_Customers.Where(p => p.IsActive == true).ToList();
            return View();
        }
        [HttpGet]
        public ActionResult GetTrouble(int? page, string search = "")
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);


            var data = db.MM_TroubleTickets.Where(p => p.TicketID.Contains(search) || p.TicketName.Contains(search)).ToList().OrderBy(p=>p.TicketID);

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
        public ActionResult create(MM_TroubleTickets trouble)
        {

            if (String.IsNullOrEmpty(trouble.TicketID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.MM_TroubleTickets.Find(trouble.TicketID);

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);

            trouble.CreationDate = DateTime.Now;
            trouble.LastUpdateDate = DateTime.Now;
            db.MM_TroubleTickets.Add(trouble);

            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = trouble }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult edit(MM_TroubleTickets trouble)
        {
            if (String.IsNullOrEmpty(trouble.TicketID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.MM_TroubleTickets.Find(trouble.TicketID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.TicketName = trouble.TicketName;
            check.TicketDate = trouble.TicketDate;
            check.EmployeeID = trouble.EmployeeID;
            check.PostOfficeID = trouble.PostOfficeID;
            check.StatusID = trouble.StatusID;
            check.LastUpdateDate = DateTime.Now;

            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult delete(string ticketid)
        {
            if (String.IsNullOrEmpty(ticketid))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.MM_TroubleTickets.Find(ticketid);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            db.Entry(check).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);
        }        
        [HttpGet]
        public ActionResult getMaxid()
        {
            //lay ra gia tri maxid
            var maxid = db.MM_TroubleTickets.Where(p => p.PostOfficeID == "BCQ3").OrderByDescending(x => x.TicketID).FirstOrDefault();
            string maxndg = string.Empty;
            if (maxid != null)
            {
                maxndg = string.Format("BCQ3" + "{0}", (Convert.ToUInt32(maxid.TicketID.Substring(6)) + 1).ToString("D6"));
            }
            else
            {
                maxndg = "BCQ3" + "000001";
            }
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = maxndg
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult createsub(MM_TroubleTicketDetails trouble)
        {

            trouble.TicketID = trouble.TicketID;
            trouble.NoiDung = trouble.NoiDung;
            trouble.NguoiNhap = "LOIVV";
            trouble.TrangThai = trouble.TrangThai;
            db.MM_TroubleTicketDetails.Add(trouble);
            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = trouble }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult getDetail(string ticketid)
        {
            var results = db.MM_TroubleTicketDetails.Where(p => p.TicketID == ticketid).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetTracking(string mailerId)
        {
            var data = db.MAILER_GETTRACKING_BY_MAILERID(mailerId).ToList();

            return Json(new ResultInfo()
            {
                data = data,
                error = 0
            }, JsonRequestBehavior.AllowGet);
        }
    }
}