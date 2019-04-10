using MNPOST.Models;
using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers.mailer
{
    public class MailerPackingAcceptController : BaseController
    {
        protected MNHistory HandleHistory = new MNHistory();
        // GET: MailerPackingAccept
        public ActionResult Index()
        {
            ViewBag.PostOffices = EmployeeInfo.postOffices;
            return View();
        }

        [HttpGet]
        public ActionResult GetDataRequire(string postId)
        {

            var posts = db.POSTOFFICE_GETALL().Where(p => p.PostOfficeID != postId).Select(p => new { code = p.PostOfficeID, name = p.PostOfficeName }).ToList();

            return Json(new
            {
                Posts = posts

            }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetPackingDetails(string documentId)
        {
            var data = db.MAILERPACKING_GETMAILER(documentId).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPacking(string fromDate, string toDate, int status, string postId, string documentCode, string postSend)
        {
            DateTime paserFromDate = DateTime.Now;
            DateTime paserToDate = DateTime.Now;
            if (String.IsNullOrEmpty(documentCode))
                documentCode = "";

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

            var data = db.PACKING_GETALL("%" + postSend + "%", postId, status, paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd")).Where(p => p.DocumentCode.Contains(documentCode)).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AcceptDocument(string documentId)
        {
            var find = db.MM_PackingList.Where(p => p.DocumentID == documentId).FirstOrDefault();

            if (find == null || find.StatusID != 1)
                return Json(new { error = 1, msg = "Bảng kê không thể xác nhận" }, JsonRequestBehavior.AllowGet);

            find.StatusID = 2;

            db.Entry(find).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AcceptAll(string documentId)
        {
            var document = db.MM_PackingList.Where(p => p.DocumentID == documentId).FirstOrDefault();

            if (document == null)
                return Json(new { error = 1, msg = "Sai thông tin" }, JsonRequestBehavior.AllowGet);

            if (document.StatusID != 2)
                return Json(new { error = 1, msg = "Bảng kê chưa xác nhận" }, JsonRequestBehavior.AllowGet);


            var listMailer = db.MM_PackingListDetail.Where(p => p.DocumentID == documentId).ToList();

            foreach(var item in listMailer)
            {
                var mailer = db.MM_Mailers.Find(item.MailerID);
                if (mailer.CurrentStatusID != 1)
                    continue;

                if(item.StatusID == 1)
                {
                    item.StatusID = 2; // da nhan
                    item.AcceptDate = DateTime.Now;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;

                    mailer.CurrentStatusID = 2;
                    mailer.LastUpdateDate = DateTime.Now;
                    mailer.CurrentPostOfficeID = document.PostOfficeIDAccept;

                    db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    HandleHistory.AddTracking(2, mailer.MailerID, mailer.CurrentPostOfficeID, "Hàng đã được tiếp nhận tại bưu cục");
                }
            }


            return Json(new { error = 0}, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult AddMailer(string documentId, string mailerId)
        {
            var mailer = db.MM_Mailers.Find(mailerId);

            var document = db.MM_PackingList.Where(p => p.DocumentID == documentId).FirstOrDefault();
            if (mailer == null)
                return Json(new { error = 1, msg = "Sai mã vận đơn" }, JsonRequestBehavior.AllowGet);

            if (mailer.CurrentStatusID != 1)
            {
                return Json(new { error = 1, msg = "Đơn không thể thực hiện" }, JsonRequestBehavior.AllowGet);
            }

            if (document == null)
                return Json(new { error = 1, msg = "Sai thông tin" }, JsonRequestBehavior.AllowGet);

            if (document.StatusID != 2)
                return Json(new { error = 1, msg = "Bảng kê chưa xác nhận" }, JsonRequestBehavior.AllowGet);

            if (document.PostOfficeID != mailer.CurrentPostOfficeID)
                return Json(new { error = 1, msg = "Đơn không thuộc bưu cục gửi tới" }, JsonRequestBehavior.AllowGet);

            var checkDetail = db.MM_PackingListDetail.Where(p => p.DocumentID == document.DocumentID && p.MailerID == mailerId).FirstOrDefault();

            if (checkDetail == null)
                return Json(new { error = 1, msg = "Đơn không trong danh sách" }, JsonRequestBehavior.AllowGet);

            if(checkDetail.StatusID != 1)
                return Json(new { error = 1, msg = "Đơn đã xác nhận trong bảng kê này" }, JsonRequestBehavior.AllowGet);

            checkDetail.StatusID = 2; // da nhan
            checkDetail.AcceptDate = DateTime.Now;
            db.Entry(checkDetail).State = System.Data.Entity.EntityState.Modified;

            mailer.CurrentStatusID = 2;
            mailer.LastUpdateDate = DateTime.Now;
            mailer.CurrentPostOfficeID = document.PostOfficeIDAccept;

            db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            HandleHistory.AddTracking(2, mailerId, mailer.CurrentPostOfficeID, "Hàng đã được tiếp nhận tại bưu cục");

            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
        }

    }
}