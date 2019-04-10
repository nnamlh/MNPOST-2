using MNPOST.Models;
using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers.mailer
{
    public class MailerPackingController : BaseController
    {
        protected MNHistory HandleHistory = new MNHistory();
        // GET: MailerPacking
        public ActionResult Index()
        {

            ViewBag.PostOffices = EmployeeInfo.postOffices;

            return View();
        }

        [HttpGet]
        public ActionResult GetDataRequire(string postId)
        {

            var posts = db.POSTOFFICE_GETALL().Where(p => p.PostOfficeID != postId).Select(p=> new { code = p.PostOfficeID, name = p.PostOfficeName }).ToList();
            List<AddressCommom> allProvince = GetProvinceDatas("", "province");

            return Json(new
            {
                Posts = posts,
                Provinces = allProvince

            }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetTransport(string type)
        {
            var data = db.Transports.Where(p => p.TransportType == type).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPacking(string fromDate, string toDate, int status, string postId, string documentCode, string postAccept) 
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

            var data = db.PACKING_GETALL(postId, "%" + postAccept + "%", status, paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd")).Where(p => p.DocumentCode.Contains(documentCode)).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPackingDetails (string documentId)
        {
            var data = db.MAILERPACKING_GETMAILER(documentId).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateDocument(CreatePackingInfo infos)
        {
            if (String.IsNullOrEmpty(infos.currentPost) || String.IsNullOrEmpty(infos.postDes))
                return Json(new { error = 1, msg = "Thiếu thông tin" }, JsonRequestBehavior.AllowGet);

            var documentCode = infos.currentPost + "-" + infos.postDes + "-" + DateTime.Now.ToString("ddMMyyyyHHmm");


            var ins = new MM_PackingList()
            {
                DocumentID = Guid.NewGuid().ToString(),
                AcceptDescription = "",
                DocumentCode = documentCode,
                DocumentDate = DateTime.Now,
                EmployeeSend = EmployeeInfo.employeeId,
                StatusID = 0,
                PostOfficeID = infos.currentPost,
                PostOfficeIDAccept = infos.postDes,
                SendDescription = infos.notes,
                TransportID = infos.transportType,
                TransportName = infos.tranpostName,
                TripNumber = infos.tripNumber
            };

            db.MM_PackingList.Add(ins);
            db.SaveChanges();
            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddListMailer(List<string> mailers, string documentId)
        {
            var document = db.MM_PackingList.Where(p => p.DocumentID == documentId).FirstOrDefault();
            if (document.StatusID != 0)
                return Json(new { error = 1, msg = "Bảng kê không thể thêm" }, JsonRequestBehavior.AllowGet);

            foreach (var item in mailers)
            {
                var mailer = db.MM_Mailers.Find(item);

                if (mailer == null)
                    continue;

                if (mailer.CurrentStatusID != 2 && mailer.CurrentStatusID != 5)
                    continue;

                if (document.PostOfficeID != mailer.CurrentPostOfficeID)
                    continue;

                var checkDetail = db.MM_PackingListDetail.Where(p => p.DocumentID == document.DocumentID && p.MailerID == item).FirstOrDefault();

                if (checkDetail != null)
                    continue;

                var checkDetailOther = db.MM_PackingListDetail.Where(p => p.MailerID == item && p.StatusID == 12).FirstOrDefault();

                if (checkDetailOther != null)
                    continue;

                var ins = new MM_PackingListDetail()
                {
                    CreationDate = DateTime.Now,
                    DocumentID = document.DocumentID,
                    MailerID = mailer.MailerID,
                    StatusID = 12,
                    Notes = ""
                };
                db.MM_PackingListDetail.Add(ins);

                db.SaveChanges();

                mailer.CurrentStatusID = 12;
                mailer.LastUpdateDate = DateTime.Now;
                db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                HandleHistory.AddTracking(12, item, mailer.CurrentPostOfficeID, "Hàng đang được đóng giói chuẩn bị vận chuyển");
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

            if(mailer.CurrentStatusID != 2 && mailer.CurrentStatusID != 5)
            {
                return Json(new { error = 1, msg = "Đơn không thể thực hiện" }, JsonRequestBehavior.AllowGet);
            }

            if(document == null)
                return Json(new { error = 1, msg = "Sai thông tin" }, JsonRequestBehavior.AllowGet);

            if(document.StatusID != 0)
                return Json(new { error = 1, msg = "Bảng kê không thể thêm" }, JsonRequestBehavior.AllowGet);

            if (document.PostOfficeID  != mailer.CurrentPostOfficeID)
                return Json(new { error = 1, msg = "Đơn không thuộc bưu cục bạn" }, JsonRequestBehavior.AllowGet);

            var checkDetail = db.MM_PackingListDetail.Where(p => p.DocumentID == document.DocumentID && p.MailerID == mailerId).FirstOrDefault();

            if(checkDetail != null)
                return Json(new { error = 1, msg = "Đơn đã trong danh sách" }, JsonRequestBehavior.AllowGet);

            var checkDetailOther = db.MM_PackingListDetail.Where(p => p.MailerID == mailerId && p.StatusID == 12).FirstOrDefault();

            if(checkDetailOther != null)
                return Json(new { error = 1, msg = "Đơn đã trong bảng kê khác" }, JsonRequestBehavior.AllowGet);

            var ins = new MM_PackingListDetail()
            {
                CreationDate = DateTime.Now,
                DocumentID = document.DocumentID,
                MailerID = mailer.MailerID,
                StatusID = 12,
                Notes = ""
            };
            db.MM_PackingListDetail.Add(ins);

            db.SaveChanges();

            mailer.CurrentStatusID = 12;
            mailer.LastUpdateDate = DateTime.Now;
            db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            HandleHistory.AddTracking(12, mailerId, mailer.CurrentPostOfficeID, "Hàng đang được đóng giói chuẩn bị vận chuyển");

            var mailerDetail = db.MAILERPACKING_GETMAILER_ByID(document.DocumentID, mailer.MailerID).FirstOrDefault();

            return Json(new { error = 0, data = mailerDetail }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult RemoveMailer(string documentId, string mailerId)
        {
            try
            {
                var find = db.MM_PackingListDetail.Where(p => p.MailerID == mailerId && p.DocumentID == documentId).FirstOrDefault();

                if (find == null)
                    throw new Exception("Đơn không nằm trong bảng kê hiện tại");

                var mailer = db.MM_Mailers.Find(mailerId);

                if (mailer == null)
                    throw new Exception("Sai mã vận đơn");

                if (mailer.CurrentStatusID != 12)
                    throw new Exception("Đơn không thể xóa");

                var lastTracking = db.MM_Tracking.Where(p => p.MailerID == mailer.MailerID).OrderByDescending(p => p.CreateTime).FirstOrDefault();

                if (lastTracking != null)
                {
                    db.MM_Tracking.Remove(lastTracking);
                    db.SaveChanges();
                }
                lastTracking = db.MM_Tracking.Where(p => p.MailerID == mailer.MailerID).OrderByDescending(p => p.CreateTime).FirstOrDefault();

                if(lastTracking != null)
                {
                    db.MM_PackingListDetail.Remove(find);

                    mailer.CurrentStatusID = lastTracking.StatusID;
                    mailer.LastUpdateDate = DateTime.Now;

                    db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new { error = 0}, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { error = 1, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }


        [HttpPost]
        public ActionResult UpdateDocument(string postId, string documentId, string transport, string transportName, string notes, string tripNumber)
        {
            var find = db.MM_PackingList.Where(p => p.DocumentID == documentId).FirstOrDefault();

            if (find == null)
                return Json(new { error = 1, msg = "Sai chuyến thư" }, JsonRequestBehavior.AllowGet);

            find.PostOfficeIDAccept = postId;
            find.TransportID = transport;
            find.TransportName = transportName;
            find.SendDescription = notes;
            find.TripNumber = tripNumber;

            db.Entry(find).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult ConfirmSend(string documentId)
        {
            var find = db.MM_PackingList.Where(p => p.DocumentID == documentId).FirstOrDefault();

            if (find == null)
                return Json(new { error = 1, msg = "Sai bảng kê" }, JsonRequestBehavior.AllowGet);

            if(find.StatusID != 0)
                return Json(new { error = 1, msg = "Bảng kê đã chuyển bưu cục" }, JsonRequestBehavior.AllowGet);

            var findListDetail = db.MM_PackingListDetail.Where(p => p.DocumentID == documentId).ToList();
            var findPost = db.BS_PostOffices.Where(p => p.PostOfficeID == find.PostOfficeIDAccept).FirstOrDefault();
            foreach(var item in findListDetail)
            {
                item.StatusID = 1;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var findMailer = db.MM_Mailers.Find(item.MailerID);
             
                if(findMailer != null && findMailer.CurrentStatusID == 12)
                {
                    findMailer.LastStatus = findMailer.CurrentStatusID;
                    findMailer.CurrentStatusID = 1;
                    findMailer.LastUpdateDate = DateTime.Now;
                    db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    HandleHistory.AddTracking(1, findMailer.MailerID, findMailer.CurrentPostOfficeID, "Hàng đang đang được vận chuyển tới bưu cục của Bưu Chính Miền Nam : " + findPost.PostOfficeName + ", tỉnh/thành: " + findPost.ProvinceID);

                }
            }
            find.StatusID = 1;
            db.Entry(find).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetMailerForEmployee(string postId, string province, string district)
        {
            var mailers = db.MM_Mailers.Where(p => p.CurrentPostOfficeID == postId && (p.CurrentStatusID == 2 || p.CurrentStatusID == 5) && (p.RecieverProvinceID.Contains(province) || p.RecieverDistrictID.Contains(district))).ToList();
            var data = new List<MailerIdentity>();
            foreach (var item in mailers)
            {
                var mailer = db.MAILER_GETINFO_BYID(item.MailerID).FirstOrDefault();

                data.Add(new MailerIdentity()
                {
                    MailerID = mailer.MailerID,
                    COD = mailer.COD,
                    SenderName = mailer.SenderName,
                    SenderAddress = mailer.SenderAddress,
                    RecieverAddress = mailer.RecieverAddress,
                    RecieverProvinceID = mailer.RecieProvinceName,
                    RecieverDistrictID = mailer.ReceiDistrictName,
                    RecieverWardID = mailer.ReceiWardName,
                    RecieverPhone = mailer.RecieverPhone,
                    CurrentStatusID = mailer.CurrentStatusID,
                    MailerTypeID = mailer.MailerTypeID
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }



    }
}