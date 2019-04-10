using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MNPOST.Report
{
    [PrincipalPermission(SecurityAction.Demand, Role = "user")]
    public partial class ShowReport : System.Web.UI.Page
    {

        MNPOSTEntities db = new MNPOSTEntities();
        ReportDocument rptH;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (rptH != null)
            {
                rptH.Close();
                rptH.Dispose();
            }
            rptH = new ReportDocument();
            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string reportPath = Server.MapPath("~/Report/baocao/test.rpt");

            rptH.Load(reportPath);

            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            crConnectionInfo.ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();
            crConnectionInfo.DatabaseName = ConfigurationManager.AppSettings["DataBaseName"].ToString();
            crConnectionInfo.UserID = ConfigurationManager.AppSettings["UserId"].ToString();
            crConnectionInfo.Password = ConfigurationManager.AppSettings["Password"].ToString();

            rptH.DataSourceConnections[0].SetConnection(crConnectionInfo.ServerName, crConnectionInfo.DatabaseName, crConnectionInfo.UserID, crConnectionInfo.Password);
           
            
            ParameterFields paramFields = new ParameterFields();

            ParameterField paramFdate = new ParameterField()
            {
                Name = "@fdate"
            };
            paramFdate.CurrentValues.Add(new ParameterDiscreteValue()
            {
                Value = "2018-01-01"
            });
            paramFields.Add(paramFdate);

            ///
            ParameterField paramtdate = new ParameterField()
            {
                Name = "@tdate"
            };
            paramtdate.CurrentValues.Add(new ParameterDiscreteValue()
            {
                Value = "2019-01-01"
            });
            paramFields.Add(paramtdate);

            //
            ParameterField parampostId = new ParameterField()
            {
                Name = "@postId"
            };
            parampostId.CurrentValues.Add(new ParameterDiscreteValue()
            {
                Value = "%BCQ3%"
            });
            paramFields.Add(parampostId);
            //
            ParameterField parammailerId = new ParameterField()
            {
                Name = "@mailerId"
            };
            parammailerId.CurrentValues.Add(new ParameterDiscreteValue()
            {
                Value = "%%"
            });
            paramFields.Add(parammailerId);

    
            ReportMain.ParameterFieldInfo = paramFields;
            
            rptH.VerifyDatabase();

            ReportMain.ReportSource = rptH;

        }
    }
}