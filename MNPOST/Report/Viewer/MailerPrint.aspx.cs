using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MNPOSTCOMMON;
using MNPOST.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace MNPOST.Report.Viewer
{
    public partial class MailerPrint : System.Web.UI.Page
    {

        MNPOSTEntities db = new MNPOSTEntities();
        ReportDocument rptH;
        string searchText = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            searchText = Request.QueryString["mailer"];
            LoadReport();
        }

        private void LoadReport()
        {
            var mailer = db.MAILER_GETINFO_BYLISTID(searchText).Select(p => new MailerRpt()
            {
                SenderName = p.SenderName,
                MailerID = "*"+ p.MailerID + "*",
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
                COD = p.COD.Value.ToString("C", MNPOST.Utils.Cultures.VietNam),
                Weight = p.Weight.ToString(),
                Quantity = p.Quantity.ToString(),
                Amount= p.Amount.Value.ToString("C", MNPOST.Utils.Cultures.VietNam),
                Price = p.Price.Value.ToString("C", MNPOST.Utils.Cultures.VietNam),
                ServicePrice = p.PriceService.Value.ToString("C", MNPOST.Utils.Cultures.VietNam)
            }).ToList();

            if (mailer == null)
                return;

            if(rptH != null)
            {
                rptH.Close();
                rptH.Dispose();
            }

            rptH = new ReportDocument();
            string reportPath = Server.MapPath("~/Report/MNPOSTReport.rpt");
            rptH.Load(reportPath);

            rptH.SetDataSource(mailer);


            MailerRptViewer.ReportSource = rptH;
        }

        protected void btnprint_Click(object sender, EventArgs e)
        {
            if(rptH != null)
            {
               
                rptH.PrintToPrinter(1, false, 0, 0);
            }
        }
    }
}