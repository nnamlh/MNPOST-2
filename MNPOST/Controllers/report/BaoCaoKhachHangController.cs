using MNPOST.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers.report
{
    public class BaoCaoKhachHangController : BaseController
    {
        ReportUtils rUtils = new ReportUtils();
        // GET: BaoCaoKhachHang
        public ActionResult Index()
        {
            ViewBag.CustomerGroup = db.BS_CustomerGroups.Select(p => new
            {
                code = p.CustomerGroupCode,
                name = p.CustomerGroupName
            }).ToList();
            return View();
        }

        public ActionResult BaoCao(string fromDate, string toDate, string reportId, string groupId, string type = ".pdf")
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

            paramter.Add("@fdate", paserFromDate.ToString("yyyy-MM-dd"));
            paramter.Add("@tdate", paserToDate.ToString("yyyy-MM-dd"));


            string path = "~/Report/baocao/test.rpt";
            if (reportId == "bcphat")
            {
                paramter.Add("@groupId", groupId);
                path = "~/Report/baocao/cus_baocaophat.rpt";
            } else
            if (reportId == "bchoan")
            {
                paramter.Add("@groupId", groupId);
                path = "~/Report/baocao/cus_baocaohoan.rpt";
            }

            Stream stream = rUtils.GetReportStreamFromDatabase(path, paramter, type);

            return File(stream, rUtils.GetContentType(type), "report_phat_hang" + type);
        }
    }
}