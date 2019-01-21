var app = angular.module('myApp', ['ui.bootstrap', 'ui.select2', 'ui.mask', 'myDirective']);


app.controller('myCtrl', function ($scope, $http, $interval) {
    $scope.select2Options = {
        width: '100%'
    };

    $scope.groups = groups;

    $scope.tabcreate = false;
    // phan trang
    $scope.numPages;
    $scope.itemPerPage;
    $scope.totalItems;
    $scope.currentPage = 1;
    $scope.maxSize = 5;

    $scope.debitReports = [];
    $scope.documents = [];
    $scope.document = {};
    $scope.details = [];

    $scope.status = [
        {
            "code": 0,
            "name": "MỚI TẠO"
        },
        {
            "code": 1,
            "name": "CHƯA THANH TOÁN"
        },
        {
            "code": 2,
            "name": "ĐÃ THANH TOÁN"
        }
    ];

    $scope.ECoDStatus = [
        {
            "code": 0,
            "name": "CHƯA THU"
        },
        {
            "code": 1,
            "name": "ĐÃ THU"
        }
    ];

    $scope.pageChanged = function () {
        $scope.GetData();
    };

    $scope.reportFilter = { CustomerGroupCode: '' };

    $scope.searchInfo = {
        "fromDate": fromDate,
        "toDate": toDate,
        "page": $scope.currentPage,
        "customerId": ""
    };


    $scope.GetReport = function () {

        $http.get("/CustomerDebitVoucher/GetReport").then(function (response) {
            $scope.debitReports = angular.copy(response.data.data);
        });


    };

    $scope.GetData = function () {
        $scope.searchInfo.page = $scope.currentPage;

        showLoader(true);
        $http({
            method: "POST",
            url: "/CustomerDebitVoucher/GetDocument",
            data: $scope.searchInfo
        }).then(function mySuccess(response) {
            showLoader(false);
            if (response.data.error === 0) {
                $scope.itemPerPage = response.data.pageSize;
                $scope.totalItems = response.data.toltalSize;
                $scope.numPages = Math.round($scope.totalItems / $scope.itemPerPage);
                $scope.documents = angular.copy(response.data.data);
            } else {
                alert(response.data.msg);
            }

        }, function myError(response) {
            showLoader(false);
            showNotify('Connect error');
        });
    };

    $scope.showDetails = function (idx) {
        $scope.document = angular.copy($scope.documents[idx]);
        showLoader(true);
        $http.get("/CustomerDebitVoucher/GetDetails?documentId=" + $scope.document.DocumentID).then(function (response) {
            $('.nav-tabs a[href="#tab_chitiet"]').tab('show');
            showLoader(false);
            $scope.details = angular.copy(response.data.data);
        });
    };

    $scope.refeshData = function () {

        $scope.GetReport();

    };
    $scope.refeshData();
    $scope.GetData();
    $scope.mailerNotPaids = [];
    $scope.createDocumentInfo = { CustomerGroupCode: '' };
    $scope.getMailerNotPaid = function () {
        if ($scope.createDocumentInfo.CustomerGroupCode === '') {
            showNotify('Nhập mã khách hàng');
        } else {
            showLoader(true);
            $scope.createDocumentInfo.AllMoney = 0;
            $scope.createDocumentInfo.AllMailer = 0;
            $http.get("/CustomerDebitVoucher/GetMailerNotPaid?cutomerId=" + $scope.createDocumentInfo.CustomerGroupCode).then(function (response) {
                showLoader(false);
                $scope.mailerNotPaids = angular.copy(response.data.data);
            });
        }

    };
    $scope.prepareCreate = function () {
        $scope.tabcreate = true;
        $scope.createDocumentInfo = { CustomerGroupCode: '', AllMoney: 0, AllMailer: 0 };
    };


    $scope.sendToCreate = function (cusId) {
        $scope.tabcreate = true;
        $scope.createDocumentInfo = { CustomerGroupCode: cusId, AllMoney: 0, AllMailer: 0 };
        $('.nav-tabs a[href="#tab_taodon"]').tab('show');
        $scope.getMailerNotPaid();
    };

    $scope.finishPayment = function () {
        showModel('paymentCodModal');
        $scope.paymentInfo = { documentId: $scope.document.DocumentID };
    };

    $scope.sendPayment = function () {
        hideModel('paymentCodModal');
        showLoader(true);

        $http({
            method: "POST",
            url: "/CustomerDebitVoucher/FinishPayment",
            data: $scope.paymentInfo
        }).then(function sucess(response) {
            showLoader(false);
            if (response.data.error === 1) {
                showNotify(response.data.msg);
            } else {
                showNotify("Đã xong");
                $scope.document.StatusID = 2;
                $scope.document.InvoiceCode = $scope.paymentInfo.invoice;
                $scope.GetData();
            }

        }, function myError(response) {
            showLoader(false);
            showNotify('Connect error');
        });


    };

    $scope.chkAllPaid = function () {
        var sumCod = 0;
        var countMailer = 0;
        for (var i = 0; i < $scope.mailerNotPaids.length; i++) {
            $scope.mailerNotPaids[i].isCheck = $scope.isChkPaid;

            if ($scope.mailerNotPaids[i].isCheck) {
                sumCod = sumCod + $scope.mailerNotPaids[i].COD;
                countMailer++;
            }
        }
        $scope.createDocumentInfo.AllMoney = sumCod;
        $scope.createDocumentInfo.AllMailer = countMailer;
    };

    $scope.checkPaidItem = function () {
        var sumCod = 0;
        var countMailer = 0;
        for (var i = 0; i < $scope.mailerNotPaids.length; i++) {

            if ($scope.mailerNotPaids[i].isCheck) {
                sumCod = sumCod + $scope.mailerNotPaids[i].COD;
                countMailer++;
            }

        }

        $scope.createDocumentInfo.AllMoney = sumCod;
        $scope.createDocumentInfo.AllMailer = countMailer;
    };

    $scope.findMailerPaid = function () {
        var sumCod = 0;
        var countMailer = 0;
        for (var i = 0; i < $scope.mailerNotPaids.length; i++) {

            if ($scope.filerPaidList.MailerID === $scope.mailerNotPaids[i].MailerID) {
                if ($scope.autoChek) {
                    $scope.mailerNotPaids[i].isCheck = true;
                }
            }

            if ($scope.mailerNotPaids[i].isCheck) {
                sumCod = sumCod + $scope.mailerNotPaids[i].COD;
                countMailer++;
            }

        }

        $scope.createDocumentInfo.AllMoney = sumCod;
        $scope.createDocumentInfo.AllMailer = countMailer;
    };
    $scope.createDocument = function () {

        var listTemps = [];

        for (var i = 0; i < $scope.mailerNotPaids.length; i++) {
            if ($scope.mailerNotPaids[i].isCheck) {
                listTemps.push($scope.mailerNotPaids[i].MailerID);
            }
        }

        if ($scope.createDocumentInfo.CustomerGroupCode === '') {
            showNotify('Nhập mã khách hàng');
        } else if (listTemps.length === 0) {
            showNotify("Chọn các đơn phải thu");
        } else {
            showLoader(true);
            $http({
                method: "POST",
                url: "/CustomerDebitVoucher/CreateDocument",
                data: {
                    cutomerId: $scope.createDocumentInfo.CustomerGroupCode,
                    notes: $scope.createDocumentInfo.Notes,
                    mailers: listTemps
                }
            }).then(function sucess(response) {
                showLoader(false);
                if (response.data.error === 1) {
                    showNotify(response.data.msg);
                } else {
                    $scope.getMailerNotPaid();
                    $scope.refeshData();
                    $scope.GetData();
                }
            }, function error() {
                showLoader(false);
                showNotify("Mất kết nối mạng")
            });
        }


    };
});