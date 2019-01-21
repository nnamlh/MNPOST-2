<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MailerPrint.aspx.cs" Inherits="MNPOST.Report.Viewer.MailerPrint" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Show Report</title>
</head>
<body style="padding : 20px;">
     <form id="form1" runat="server">
     <h1>XEM PHIẾU&nbsp;
         <asp:Button ID="btnprint" runat="server" OnClick="btnprint_Click" Text="In phiếu" />
         </h1>
         <CR:CrystalReportViewer ID="MailerRptViewer" runat="server" AutoDataBind="true" EnableDatabaseLogonPrompt="False" EnableParameterPrompt="False" ToolPanelView="None" />
     </form>
    </body>
</html>
