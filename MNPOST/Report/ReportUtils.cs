using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
namespace MNPOST.Report
{
    public class ReportUtils
    {
        public Stream GetReportStream(string reportPath, Object data)
        {
            ReportDocument rptH = new ReportDocument();

            rptH.Load(HttpContext.Current.Server.MapPath(reportPath));

            if (data != null)
            {
                rptH.SetDataSource(data);
            }

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return stream;
        }
       
        public Stream GetReportStream(string reportPath, Object data, Dictionary<string, Dictionary<string, string>> values)
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

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return stream;
        }
    }


    public class ReportPath
    {
        public static string RptPhieuGui = "~/Report/MNPOSTReport.rpt";
        public static string RptAC_CustomerDebitDetails = "~/Report/customer/AC_CustomerDebitDetails.rpt";
    }
}