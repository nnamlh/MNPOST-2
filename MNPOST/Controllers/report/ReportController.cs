using CrystalDecisions.CrystalReports.Engine;
using MNPOSTCOMMON;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using MNPOST.Models;
using MNPOST.Report;
using System.Collections.Generic;
using System.Configuration;
using CrystalDecisions.Shared;

namespace MNPOST.Controllers.report
{

    public class ReportController : BaseController
    {

        [HttpGet]
        public ActionResult GetTest()
        {
            /*
            Dictionary<string, string> paramter = new Dictionary<string, string>();

            paramter.Add("postId", postId);
            paramter.Add("search", "");

            Stream stream = REPORTUTILS.GetReportStreamWithParamter("~/Report/test.rpt", null, paramter);
            */

            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string reportPath = Server.MapPath("~/Report/baocao/test.rpt");
            ReportDocument rptH = new ReportDocument();
            rptH.Load(reportPath);

            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();
            crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DataBaseName"].ToString();
            crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserId"].ToString();
            crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"].ToString();



           
            rptH.SetParameterValue("@fdate","2018-01-01");
            rptH.SetParameterValue("@tdate", "2019-01-01");
            rptH.SetParameterValue("@postId", "%BCQ3%");
            rptH.SetParameterValue("@mailerId", "%%");

            
            rptH.DataSourceConnections[0].SetConnection(crConnectionInfo.ServerName, crConnectionInfo.DatabaseName, crConnectionInfo.UserID, crConnectionInfo.Password);
          //  rptH.VerifyDatabase();

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return File(stream, "application/pdf", "test.pdf");

        }

        // GET: /Report/
        public ActionResult PhieuGui(string mailerId)
        {
            var mailer = db.MAILER_GETINFO_BYLISTID(mailerId).Select(p => new MailerRpt()
            {
                SenderName = p.SenderName,
                MailerID = "*" + p.MailerID + "*",
                SenderID = p.SenderID,
                SenderPhone = p.SenderPhone,
                SenderAddress = p.SenderAddress,
                RecieverName = p.RecieverName,
                RecieverAddress = p.RecieverAddress,
                RecieverPhone = p.RecieverPhone,
                ReceiverDistrictName = p.ReceiDistrictName,
                ReceiverProvinceName = p.RecieProvinceName,
                SenderDistrictName = p.SendDistrictName,
                SenderProvinceName = p.SendProvinceName,
                PostOfficeName = p.PostOfficeName,
                TL = p.MerchandiseID == "T" ? "X" : ".",
                HH = p.MerchandiseID == "H" ? "X" : ".",
                MH = p.MerchandiseID == "M" ? "X" : ".",
                N = p.MailerTypeID == "SN" ? "X" : ".",
                DB = p.MerchandiseID == "ST" ? "X" : ".",
                TK = p.MerchandiseID == "TK" ? "X" : ".",
                COD = p.COD != null ? p.COD.Value.ToString("C", MNPOST.Utils.Cultures.VietNam) : "0",
                Weight = p.Weight + "",
                Quantity = p.Quantity + "",
                Amount = p.Amount != null ? p.Amount.Value.ToString("C", MNPOST.Utils.Cultures.VietNam) : "0",
                Price = p.Price != null ? p.Price.Value.ToString("C", MNPOST.Utils.Cultures.VietNam) : "0",
                ServicePrice = p.PriceService != null ? p.PriceService.Value.ToString("C", MNPOST.Utils.Cultures.VietNam): "0",
                TotalMoney = p.PaymentMethodID == "NGTT"? p.Amount.Value.ToString("C", MNPOST.Utils.Cultures.VietNam) : (p.Amount + p.COD).Value.ToString("C", MNPOST.Utils.Cultures.VietNam)
            }).ToList();

            Stream stream = REPORTUTILS.GetReportStream(ReportPath.RptPhieuGui, mailer);

          //  return File(stream, "application/msword", mailerId + ".doc");
            return File(stream, "application/pdf", mailerId + ".pdf");
        }

    }
}