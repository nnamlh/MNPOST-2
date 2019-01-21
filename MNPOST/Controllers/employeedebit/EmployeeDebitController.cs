using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.employeedebit
{
    public class EmployeeDebitController : BaseController
    {
        // GET: EmployeeDebit
        public ActionResult Show()
        {
            ViewBag.PostOffices = EmployeeInfo.postOffices;
            ViewBag.ToDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            ViewBag.FromDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");

            return View();
        }

        [HttpGet]
        public ActionResult GetDataHandle(string postId)
        {
            var data = GetEmployeeByPost(postId);

            return Json(new { employees = data}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet] 
        public ActionResult GetEmployeeDebitReport(string postId)
        {
            var data = db.EMPLOYEE_DEBIT_REPORT(postId).ToList();

            return Json(new ResultInfo()
            {
                error = 0,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetDocument(int? page, string fromDate, string toDate, string postId, string employeeId)
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

            var data = db.EMPLOYEE_DEBIT_ALL(paserFromDate.ToString("yyyy-MM-dd"),paserToDate.ToString("yyyy-MM-dd"), postId).Where(p=> p.EmployeeID.Contains(employeeId)).ToList();

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
        public ActionResult GetDocumentDetail(string documentId)
        {
            var data = db.EMPLOYEE_DEBIT_DETAIL(documentId).ToList();

            return Json(new ResultInfo()
            {
                error = 0,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMailerNotPaid(string emmployeeId)
        {
            var data = db.EMPLOYEE_DEBIT_GET_NOTPAID(emmployeeId).ToList();

            return Json(new ResultInfo()
            {
                error = 0,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateDocument(string employeeId, string notes, List<string> mailers, string postId) 
        {

            var checkEmployee = db.BS_Employees.Where(p => p.EmployeeID == employeeId).FirstOrDefault();

            if (checkEmployee == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }


            var checkPost = db.BS_PostOffices.Where(p => p.PostOfficeID == postId).FirstOrDefault();

            if (checkPost == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }

            var document = new MM_EmployeeDebitVoucher()
            {
                DocumentID = Guid.NewGuid().ToString(),
                DocumentNumber = DateTime.Now.ToString("ddMMyyyyHHmm"),
                DocumentDate=  DateTime.Now,
                EmployeeID = employeeId,
                PostOfficeID = postId,
                Notes = notes,
                MoneyColector = EmployeeInfo.employeeId
            };

            db.MM_EmployeeDebitVoucher.Add(document);
            db.SaveChanges();

            foreach(var item in mailers)
            {
                var checkMailer = db.EmpployeeDebitCODs.Where(p => p.MailerID == item).FirstOrDefault();

                if (checkMailer == null || checkMailer.AccountantConfirm == 1)
                    continue;

                var detail = new MM_EmployeeDebitVoucherDetails()
                {
                    COD = Convert.ToDecimal(checkMailer.COD),
                    DocumentID = document.DocumentID,
                    MailerID = checkMailer.MailerID,
                    ReciveCOD = Convert.ToDecimal(checkMailer.COD),
                    LastUpDate = DateTime.Now
                };

                db.MM_EmployeeDebitVoucherDetails.Add(detail);

                checkMailer.AccountantConfirm = 1;
                db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;
               
            }

            db.SaveChanges();


            return Json(new ResultInfo()
            {
                error = 0
            }, JsonRequestBehavior.AllowGet);


        }

    }
}