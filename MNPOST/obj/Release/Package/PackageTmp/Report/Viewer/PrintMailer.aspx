<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintMailer.aspx.cs" Inherits="MNPOST.Report.Viewer.PrintMailer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>IN PHIẾU MIỀN NAM POST</title>
    <!-- Bootstrap -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" integrity="sha384-HSMxcRTRxnN+Bdg0JdbxYKrThecOKuH5zCYotlSAcp1+c8xmyTe9GYg1l9a69psu" crossorigin="anonymous">
</head>
<body>
    <div class="container">
        <form id="form1" runat="server">
            <div>
                <asp:Image ID="Image1" runat="server" Height="85px" ImageUrl="~/Content/logo.jpg" Style="margin-right: 0px" Width="119px" />
            </div>
            <div class="panel panel-default">
                <div class="panel-heading">IN PHIẾU</div>
                <div class="panel-body">
                    <div class="row">

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label>Người gửi</label>
                                <asp:DropDownList ID="cbsender" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="cbsender_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label>SDT Ng gửi</label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Địa chỉ người gửi</label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>

                        </div>

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label>Tỉnh thành</label>
                                <asp:DropDownList ID="cbProvince" AutoPostBack="true" runat="server" CssClass="form-control" OnSelectedIndexChanged="cbProvince_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label>Quận huyện</label>
                                <asp:DropDownList ID="cbDistrict" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label>File Excel</label>
                                <asp:FileUpload ID="FileUploadControl" AutoPostBack="true" runat="server" CssClass="form-control" />
                            </div>
                            <asp:Button ID="btnxem" runat="server" Text="XEM" BackColor="Yellow" ForeColor="White" OnClick="btnxem_Click" Width="98px" CssClass="btn btn-success" />
                        </div>


                    </div>

                </div>
            </div>
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-12 col-md-offset-1">
                            <CR:CrystalReportViewer ID="MailerRptViewer" runat="server" AutoDataBind="true" ToolPanelView="None" HasCrystalLogo="False" HasSearchButton="False" Width="350px" />
                        </div>
                    </div>
                </div>
            </div>

        </form>

    </div>

    <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
    <script src="https://code.jquery.com/jquery-1.12.4.min.js" integrity="sha384-nvAa0+6Qg9clwYCGGPpDQLVpLNn0fRaROjHqs13t4Ggj3Ez50XnGQqc/r8MhnRDZ" crossorigin="anonymous"></script>
    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js" integrity="sha384-aJ21OjlMXNL5UyIl/XNwTMqvzeRMZH2w8c5cRVpzpU8Y5bApTppSuUkhZXN0VxHd" crossorigin="anonymous"></script>

  

</body>
</html>
