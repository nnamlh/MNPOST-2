using MNPOST.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers.report
{
    public class QuantityReportController : BaseController
    {
        ReportUtils rUtils = new ReportUtils();
        // GET: QuantityReport
        public ActionResult Index()
        {

            ViewBag.PostOffices = EmployeeInfo.postOffices;


            return View();
        }

        public ActionResult GetEmployee(string postId)
        {
            var data = GetEmployeeByPost(postId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PhatHang(string fromDate, string toDate,  string reportId, string postId, string employeeId, string type = ".pdf")
        {
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

            Dictionary<string, string> paramter = new Dictionary<string, string>();

            paramter.Add("@fDate", paserFromDate.ToString("yyyy-MM-dd"));
            paramter.Add("@tDate", paserToDate.ToString("yyyy-MM-dd"));
          

            string path = "~/Report/baocao/test.rpt";
            if (reportId == "reportphatchitiet")
            {
                paramter.Add("@postId", postId);
                paramter.Add("@employeeId", "%" + employeeId + "%");
                path = "~/Report/baocao/rpt_phathang_nhanvien_chitiet.rpt";
            } else if(reportId == "reportphatvanhan")
            {
                paramter.Add("@postId", postId);
                paramter.Add("@employeeId", "%" + employeeId + "%");
                path = "~/Report/baocao/rpt_sanluongphatvalayhang.rpt";
            }
            else if (reportId == "reportbcphatvanhan")
            {
                paramter.Add("@postId", "%" + postId+ "%");
                path = "~/Report/baocao/rpt_baocaosanluongbuucuc.rpt";
            }

            Stream stream = rUtils.GetReportStreamFromDatabase(path, paramter, type);

            return File(stream, rUtils.GetContentType(type), "report_phat_hang" + type);
        }
    }
}