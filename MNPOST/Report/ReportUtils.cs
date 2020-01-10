using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
namespace MNPOST.Report
{
    public class ReportUtils
    {
        public string GetContentType (string type = ".pdf")
        {
            switch(type)
            {
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".pdf":
                    return "application/pdf";
                default:
                    return "application/octet-stream";
            } 
        }

        public Stream GetReportStreamFromDatabase(string reportPath, Dictionary<string, string> paramaters, string type = ".pdf")
        {
            ReportDocument rptH = new ReportDocument();

            rptH.Load(HttpContext.Current.Server.MapPath(reportPath));

            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();
            crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DataBaseName"].ToString();
            crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserId"].ToString();
            crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"].ToString();

            rptH.DataSourceConnections[0].SetConnection(crConnectionInfo.ServerName, crConnectionInfo.DatabaseName, crConnectionInfo.UserID, crConnectionInfo.Password);

            foreach (KeyValuePair<string, string> item in paramaters)
            {
                rptH.SetParameterValue(item.Key, item.Value);
            }

            //   rptH.VerifyDatabase();
            Stream stream = null;
            switch (type)
            {
                case ".xlsx":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    break;
                case ".pdf":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
                default:
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
            }

            rptH.Dispose();
            rptH.Close();

            return stream;
        }

        public Stream GetReportStream(string reportPath, Object data, string type = ".pdf")
        {
            ReportDocument rptH = new ReportDocument();

            rptH.Load(HttpContext.Current.Server.MapPath(reportPath));

            if (data != null)
            {
                rptH.SetDataSource(data);
            }

            Stream stream = null;
            switch (type)
            {
                case ".xlsx":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    break;
                case ".pdf":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
                default:
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
            }
            rptH.Dispose();
            rptH.Close();
            return stream;
        }
        public Stream GetReportDocStream(string reportPath, Object data, string type = ".pdf")
        {
            ReportDocument rptH = new ReportDocument();

            rptH.Load(HttpContext.Current.Server.MapPath(reportPath));

            if (data != null)
            {
                rptH.SetDataSource(data);
            }

            Stream stream = null;
            switch (type)
            {
                case ".xlsx":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    break;
                case ".pdf":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
                default:
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
            }
            rptH.Dispose();
            rptH.Close();
            return stream;
        }

        public Stream GetReportStream(string reportPath, Object data, Dictionary<string, Dictionary<string, string>> values, string type = ".pdf")
        {
            ReportDocument rptH = new ReportDocument();

            rptH.Load(HttpContext.Current.Server.MapPath(reportPath));


            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in values)
            {
                var key = kvp.Key.ToString();

                foreach (KeyValuePair<string, string> item in kvp.Value)
                {
                    TextObject _txt = (TextObject)rptH.ReportDefinition.Sections[key].ReportObjects[item.Key];
                    _txt.Text = item.Value;
                }
            }

            if (data != null)
            {
                rptH.SetDataSource(data);
            }

            Stream stream = null;
            switch (type)
            {
                case ".xlsx":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelWorkbook);
                    break;
                case ".pdf":
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
                default:
                    stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    break;
            }
            rptH.Dispose();
            rptH.Close();
            return stream;
        }
    }


    public class ReportPath
    {
        public static string RptPhieuGui = "~/Report/MNPOSTReport.rpt";
        public static string RptAC_CustomerDebitDetails = "~/Report/customer/AC_CustomerDebitDetails.rpt";
    }
}