<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowReport.aspx.cs" Inherits="MNPOST.Report.ShowReport" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BÁO CÁO MIỀN NAM POST</title>
    <!-- Bootstrap -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" integrity="sha384-HSMxcRTRxnN+Bdg0JdbxYKrThecOKuH5zCYotlSAcp1+c8xmyTe9GYg1l9a69psu" crossorigin="anonymous">
</head>
<body>
    <form id="form1" runat="server" ng-app="myApp" ng-controller="myCtrl">
        <div>
            <asp:Image ID="Image1" runat="server" Height="85px" ImageUrl="~/Content/logo.jpg" Style="margin-right: 0px" Width="119px" />
        </div>

        <div class="panel panel-default">
            <div class="panel-heading">CHỌN BÁO CÁO</div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-sm-6">
                        <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="True" type="text" class="form-control" ui-mask="99/99/9999" model-view-value="true" ui-mask-placeholder ui-mask-placeholder-char="" ng-model="fromDate"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <div class="panel panel-success">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12 col-md-offset-1">
                        <CR:CrystalReportViewer ID="ReportMain" runat="server" AutoDataBind="true" />
                    </div>
                </div>
            </div>
        </div>
    </form>


    <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
    <script src="https://code.jquery.com/jquery-1.12.4.min.js" integrity="sha384-nvAa0+6Qg9clwYCGGPpDQLVpLNn0fRaROjHqs13t4Ggj3Ez50XnGQqc/r8MhnRDZ" crossorigin="anonymous"></script>
    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js" integrity="sha384-aJ21OjlMXNL5UyIl/XNwTMqvzeRMZH2w8c5cRVpzpU8Y5bApTppSuUkhZXN0VxHd" crossorigin="anonymous"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.7.2/angular.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.7.2/angular-route.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.7.2/angular-animate.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.7.2/angular-sanitize.min.js"></script>

    <script src="~/Scripts/ui-bootstrap-tpls-2.5.0.min.js"></script>
    <script src="~/Scripts/mask.min.js"></script>
    <script src="~/Scripts/myscript.keypress.js"></script>
    <script src="~/Scripts/mydirective.js"></script>


    <script src="~/Scripts/myscripts.js"></script>

    <script type="text/html">
        var app = angular.module('myApp', ['ui.bootstrap', 'ui.mask']);

        app.controller('myCtrl', function ($scope, $http) {
            $scope.fromDate = '';
        });
    </script>
</body>
</html>
