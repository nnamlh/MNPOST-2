using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.customerdebit
{
    public class CustomerDebitVoucherController : BaseController
    {
        // GET: CustomerDebitVoucher
        [HttpGet]
        public ActionResult Show()
        {
            ViewBag.CustomerGroup = db.BS_CustomerGroups.Select(p => new
            {
                code = p.CustomerGroupCode,
                name = p.CustomerGroupName
            }).ToList();
            ViewBag.ToDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            ViewBag.FromDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");

            return View();
        }


        [HttpGet]
        public ActionResult GetReport() {

            var data = db.CUSTOMER_COD_DEBIT_REPORT().OrderByDescending(p=> p.AllCOD).ToList();

            return Json(new ResultInfo()
            {
                error = 0,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMailerNotPaid(string cutomerId)
        {
            var data = db.CUSTOMER_COD_DEBIT_GETMAILER_NOTPAID(cutomerId).ToList();

            return Json(new ResultInfo()
            {
                error = 0,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateDocument(string cutomerId, string notes, List<string> mailers)
        {
            var checkCus = db.BS_CustomerGroups.Where(p => p.CustomerGroupCode == cutomerId).FirstOrDefault();

            if(checkCus == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai mã khách hàng"
                }, JsonRequestBehavior.AllowGet);
            }

            var document = new AC_CODDebitVoucher()
            {
                DocumentID = Guid.NewGuid().ToString(),
                StatusID = 1,
                CustomerGroupID = checkCus.CustomerGroupID,
                DocumentDate = DateTime.Now,
                EmployeeID = EmployeeInfo.employeeId,
                InvoiceCode = "",
                InvoiceNotes = "",
                Notes = notes,
                PostOfficeID = EmployeeInfo.currentPost
            };

            db.AC_CODDebitVoucher.Add(document);
            db.SaveChanges();

            foreach(var item in mailers)
            {
                var checkMailer = db.MM_Mailers.Find(item);

                if (checkMailer == null || checkMailer.CurrentStatusID != 4)
                    continue;

                var detail = new AC_CODDebitVoucherDetails()
                {
                    CreateDate = DateTime.Now,
                    DocumentID = document.DocumentID,
                    MailerID = item,
                    Money = Convert.ToDouble(checkMailer.COD)
                };
                db.AC_CODDebitVoucherDetails.Add(detail);
                db.SaveChanges();


                checkMailer.PaidCoD = 1;
                checkMailer.PaidDate = DateTime.Now;
                checkMailer.PaidNotes = notes;
                checkMailer.EmployeePaid = EmployeeInfo.employeeId;

                db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new ResultInfo()
            {
                error = 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FinishPayment(string documentId, string notes, string invoice)
        {
            var checkDocument = db.AC_CODDebitVoucher.Find(documentId);

            if(checkDocument == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }

            checkDocument.StatusID = 2;
            checkDocument.InvoiceNotes = notes;
            checkDocument.InvoiceCode = invoice;
            checkDocument.LastEditTime = DateTime.Now;

            db.Entry(checkDocument).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var allDetail = db.AC_CODDebitVoucherDetails.Where(p => p.DocumentID == checkDocument.DocumentID).ToList();

            foreach(var item in allDetail)
            {
                var findMailer = db.MM_Mailers.Find(item.MailerID);

                if(findMailer != null)
                {
                    findMailer.PaidCoD = 2;
                    findMailer.PaidCoDInvoice = invoice;
                    findMailer.PaidNotes = notes;
                    db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new ResultInfo()
            {
                error = 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDocument(int? page, string fromDate, string toDate, string customerId)
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

            var data = db.CUSTOMER_COD_DEBIT_GETDOCUMENTS(paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd"), "%" + customerId + "%").ToList();

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
        public ActionResult GetDetails(string documentId)
        {
            var data = db.CUSTOMER_COD_DEBIT_GETDOCUMENT_DETAILS(documentId).ToList();

            return Json(new ResultInfo()
            {
                error = 0,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

    }
}