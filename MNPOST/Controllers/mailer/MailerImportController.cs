using MNPOST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;

namespace MNPOST.Controllers.mailer
{
    public class MailerImportController : MailerController
    {
        // GET: MailerImport
        public ActionResult Show()
        {
            ViewBag.PostOffices = EmployeeInfo.postOffices;
            var customers = db.BS_Customers.Select(p => new {
                name = p.CustomerName,
                code = p.CustomerCode,
                address = p.Address,
                phone = p.Phone
            }).ToList();

  
            ViewBag.AllCustomer = customers;

            return View();
        }


        [HttpGet]
        public ActionResult GetEmployee(string postId)
        {
            var data = db.BS_Employees.Where(p => p.PostOfficeID == postId).Select(p => new CommonData()
            {
                code = p.EmployeeID,
                name = p.EmployeeName

            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateTakeMailer(string postId, string cusCode, string cusName, string cusAddress, string cusPhone, string content, string employeeId, List<string> mailers)
        {
            if(mailers == null || mailers.Count() == 0)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không có đơn nào để lấy"
                }, JsonRequestBehavior.AllowGet);
            }

            var findEmployee = db.BS_Employees.Where(p => p.EmployeeID == employeeId).FirstOrDefault();

            if(findEmployee == null)
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin nhân viên lấy hàng"
                }, JsonRequestBehavior.AllowGet);

            // tim ma document co trung id
            var documentCode = DateTime.Now.ToString("ddMMyyyy");

            var findDocument = db.MM_TakeMailers.Where(p => p.EmployeeID == findEmployee.EmployeeID && p.CustomerID == cusCode && p.DocumentCode == documentCode).FirstOrDefault();

            if(findDocument == null)
            {
                findDocument = new MM_TakeMailers()
                {
                    DocumentID = Guid.NewGuid().ToString(),
                    Content = content,
                    CreateTime = DateTime.Now,
                    CustomerAddress = cusAddress,
                    CustomerID = cusCode,
                    CustomerName = cusName,
                    CustomerPhone = cusPhone,
                    UserCreate = EmployeeInfo.user,
                    EmployeeID = employeeId,
                    StatusID = 7,
                    PostID = postId,
                    DocumentCode = DateTime.Now.ToString("ddMMyyyy")
      
                };

                db.MM_TakeMailers.Add(findDocument);
                db.SaveChanges();
            } else
            {
                findDocument.StatusID = 7;
                findDocument.Content = content;
                db.Entry(findDocument).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            foreach(var item in mailers)
            {
                var findMailer = db.MM_Mailers.Find(item);

                if(findMailer != null && findMailer.CurrentStatusID == 0)
                {
                    var deltail = new MM_TakeDetails()
                    {
                        DocumentID = findDocument.DocumentID,
                        MailerID = item,
                        StatusID = 7,
                        TimeTake = null
                    };

                    db.MM_TakeDetails.Add(deltail);
                    db.SaveChanges();


                    findMailer.CurrentStatusID = 7;
                    findMailer.LastUpdateDate = DateTime.Now;
                    db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    // luu tracking
                    HandleHistory.AddTracking(7, item, postId, "Giao nhân viên " + findEmployee.EmployeeName + " số điện thoại " + findEmployee.Phone + " , đi lấy hàng");

                }

            }


            return Json(new ResultInfo()
            {
                error = 0
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public ActionResult GetTakeMailers (string postId, string date)
        {

            try
            {
                var checkDate = DateTime.ParseExact(date, "dd/M/yyyy", null);

                checkDate = checkDate == null ? DateTime.Now : checkDate;

                var data = db.TAKEMAILER_GETLIST(postId, 7, checkDate.ToString("yyyy-MM-dd")).OrderByDescending(p => p.CreateTime).ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            } catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
       

        [HttpGet]
        public ActionResult GetData(string postId)
        {

            var data = db.MAILER_GET_NOT_INVENTORY("%" + postId + "%").ToList();

            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddMailers(List<string> mailers, string postId)
        {
            List<string> listAdds = new List<string>();
            foreach (var item in mailers)
            {
                var find = db.MM_Mailers.Where(p => p.MailerID == item && p.PostOfficeAcceptID == postId).FirstOrDefault();

                if (find != null && (find.CurrentStatusID == 0 || find.CurrentStatusID == 8))
                {
                    find.CurrentStatusID = 2; // nhap kho
                    find.LastUpdateDate = DateTime.Now;
                    db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    HandleHistory.AddTracking(2, item, find.CurrentPostOfficeID, "Đã nhận hàng và lưu kho");

                    listAdds.Add(item);
                }
            }


            return Json(new ResultInfo()
            {
                error = 0,
                data = listAdds

            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ShowTakeDetail (string documentID)
        {
            // lay nhung don chua lay
            var data = db.TAKEMAILER_GETDETAILs(documentID).Where(p=> p.CurrentStatusID == 7).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult CancelMailers(List<string> mailers, string reason)
        {
            foreach(var item in mailers)
            {
                var findMailer = db.MM_Mailers.Find(item);

                if(findMailer != null && findMailer.CurrentStatusID == 0)
                {
                    // moi tao moi dc huy
                    findMailer.CurrentStatusID = 10;
                    findMailer.LastUpdateDate = DateTime.Now;
                    findMailer.StatusNotes = reason;
                    db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    HandleHistory.AddTracking(10, item, findMailer.CurrentPostOfficeID, "Hủy đơn với lý do " + reason);
                }
            }

            return Json(new ResultInfo()
            {
                error = 0,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CancelTake(string documentID, string mailerId)
        {
            var checkDocument = db.MM_TakeMailers.Find(documentID);

            if (checkDocument == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"

                }, JsonRequestBehavior.AllowGet);
            }


            var checkMailer = db.MM_Mailers.Find(mailerId);

            var findDetail = db.MM_TakeDetails.Where(p => p.DocumentID == documentID && p.MailerID == mailerId).FirstOrDefault();


            if (findDetail == null || checkMailer == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"

                }, JsonRequestBehavior.AllowGet);
            }

            db.MM_TakeDetails.Remove(findDetail);
            db.SaveChanges();

            //
            checkMailer.CurrentStatusID = 0;
            checkMailer.LastUpdateDate = DateTime.Now;
            db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var lastTracking = db.MM_Tracking.Where(p => p.MailerID == mailerId).OrderByDescending(p => p.CreateTime).FirstOrDefault();

            if (lastTracking != null)
            {
                db.MM_Tracking.Remove(lastTracking);
                db.SaveChanges();
            }

            var checkCount = db.TAKEMAILER_GETDETAILs(documentID).Where(p => p.CurrentStatusID == 7).ToList();

            if (checkCount.Count() == 0)
            {
                checkDocument.StatusID = 8;
                db.Entry(checkDocument).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new
            {
                error = 0
            }, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult UpdateTakeDetails(string documentID, List<string> mailers)
        {

            var checkDocument = db.MM_TakeMailers.Find(documentID);

            if(checkDocument == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"

                }, JsonRequestBehavior.AllowGet);
            }

            foreach(var item in mailers)
            {

                var checkMailer = db.MM_Mailers.Find(item);

                if (checkMailer == null)
                    continue;

                var findDetail = db.MM_TakeDetails.Where(p => p.DocumentID == documentID && p.MailerID == item).FirstOrDefault();

                if (findDetail == null)
                    continue;

                findDetail.StatusID = 8;
                findDetail.TimeTake = DateTime.Now;
                db.Entry(findDetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                checkMailer.CurrentStatusID = 8;
                checkMailer.LastUpdateDate = DateTime.Now;
                db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                HandleHistory.AddTracking(8, item, checkMailer.CurrentPostOfficeID, "Đã lấy hàng, đang giao về kho");


            }

            var checkCount = db.TAKEMAILER_GETDETAILs(documentID).Where(p => p.CurrentStatusID == 7).ToList();

            if (checkCount.Count() == 0)
            {
                checkDocument.StatusID = 8;
                db.Entry(checkDocument).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new ResultInfo()
            {
                error = 0

            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult CancelDocument(string documentID)
        {

            var checkDocument = db.MM_TakeMailers.Find(documentID);

            if (checkDocument == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"

                }, JsonRequestBehavior.AllowGet);
            }

            var details = db.MM_TakeDetails.Where(p => p.DocumentID == documentID).ToList();

            foreach(var item in details)
            {
                var checkMailer = db.MM_Mailers.Find(item);

                if (checkMailer == null)
                    continue;


                item.StatusID = 8;
                item.TimeTake = DateTime.Now;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                checkMailer.CurrentStatusID = 8;
                checkMailer.LastUpdateDate = DateTime.Now;
                db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                HandleHistory.AddTracking(8, item.MailerID, checkMailer.CurrentPostOfficeID, "Đã lấy hàng, đang giao về kho");
            }

            var checkCount = db.TAKEMAILER_GETDETAILs(documentID).Where(p => p.CurrentStatusID == 7).ToList();

            if (checkCount.Count() == 0)
            {
                checkDocument.StatusID = 8;
                db.Entry(checkDocument).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               
            }
            return Json(new ResultInfo()
            {
                error = 0

            }, JsonRequestBehavior.AllowGet);


        }

    }
}