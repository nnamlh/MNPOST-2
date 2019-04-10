using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
namespace MNPOST.Report
{
    public class ReportUtils
    {
        public Stream GetReportStreamWithParamter(string reportPath, Object data, Dictionary<string, string> paramaters)
        {
            ReportDocument rptH = new ReportDocument();

            rptH.Load(HttpContext.Current.Server.MapPath(reportPath));

            if (data != null)
            {
                rptH.SetDataSource(data);
            }
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            crConnectionInfo.ServerName = "103.77.167.37";
            crConnectionInfo.DatabaseName = "MNPOST";
            crConnectionInfo.UserID = "sa";
            crConnectionInfo.Password = "123qwe!@#mnpost123";
     
            rptH.DataSourceConnections[0].SetConnection("103.77.167.37", "MNPOST", "sa", "123qwe!@#mnpost123");
            rptH.VerifyDatabase();
            rptH.SetParameterValue(0, "BCQ3");
            rptH.SetParameterValue(1, "");
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return stream;
        }

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
        public Stream GetReportDocStream(string reportPath, Object data)
        {
            ReportDocument rptH = new ReportDocument();

            rptH.Load(HttpContext.Current.Server.MapPath(reportPath));

            if (data != null)
            {
                rptH.SetDataSource(data);
            }

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);

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