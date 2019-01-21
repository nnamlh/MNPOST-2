using CrystalDecisions.CrystalReports.Engine;
using MNPOSTCOMMON;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using MNPOST.Models;
using MNPOST.Report;

namespace MNPOST.Controllers.report
{

    public class ReportController : BaseController
    {

  

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
                ServicePrice = p.PriceService != null ? p.PriceService.Value.ToString("C", MNPOST.Utils.Cultures.VietNam): "0"
            }).ToList();

            Stream stream = REPORTUTILS.GetReportStream(ReportPath.RptPhieuGui, mailer);

            return File(stream, "application/pdf");
        }

    }
}