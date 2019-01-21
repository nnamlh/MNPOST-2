var app = angular.module('myApp', ['ui.bootstrap', 'ui.select2', 'ui.mask']);

app.controller('myCtrl', function ($scope, $http, $interval) {

    $scope.select2Options = {
        width: '100%'
    };
    $scope.tabcreate = false;
    // phan trang
    $scope.numPages;
    $scope.itemPerPage;
    $scope.totalItems;
    $scope.currentPage = 1;
    $scope.maxSize = 5;

    $scope.debitReports = [];
    $scope.documents = [];

    $scope.pageChanged = function () {
        $scope.GetData();
    };

    $scope.reportFilter = { EmployeeID: '' };

    $scope.searchInfo = {
        "fromDate": fromDate,
        "toDate": toDate,
        "page": $scope.currentPage,
        "postId": "",
        "employeeId": ""
    };

    $scope.GetData = function () {
        $scope.searchInfo.page = $scope.currentPage;
        $scope.searchInfo.postId = $scope.postHandle;

        showLoader(true);
        $http({
            method: "POST",
            url: "/EmployeeDebit/GetDocument",
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

    $scope.getEmployeeDebitReport = function () {
        $http.get("/EmployeeDebit/GetEmployeeDebitReport?postId=" + $scope.postHandle).then(function (response) {
            $scope.debitReports = angular.copy(response.data.data);
        });
    };

    $scope.details = [];
    $scope.document = {};

    $scope.showDocumentDetail = function (index) {
        showLoader(true);

        $scope.document = angular.copy($scope.documents[index]);

        $http.get("/employeedebit/getdocumentdetail?documentId=" + $scope.document.DocumentID).then(function (response) {
            showLoader(false);
            $('.nav-tabs a[href="#tab_chitiet"]').tab('show');
            $scope.details = angular.copy(response.data.data);
        });
    };

    $scope.mailerNotPaids = [];
    $scope.createDocumentInfo = { EmployeeID: '' };
    $scope.getMailerNotPaid = function () {
        if ($scope.createDocumentInfo.EmployeeID === '') {
            showNotify('Chọn nhân viên');
        } else {
            showLoader(true);
            $scope.createDocumentInfo.AllMoney = 0;
            $scope.createDocumentInfo.AllMailer = 0;
            $http.get("/employeedebit/GetMailerNotPaid?emmployeeId=" + $scope.createDocumentInfo.EmployeeID).then(function (response) {
                showLoader(false);

                $scope.mailerNotPaids = angular.copy(response.data.data);
            });
        }

    };
    $scope.refeshData = function () {
        $scope.GetData();
        $scope.getEmployeeDebitReport();
    };

    $scope.choosePostHandle = function () {
        if ($scope.postHandle === '') {
            alert('Chọn bưu cục nếu không sẽ không thể thao tác');
        } else {
            $scope.refeshData();
            $scope.getFirstData();
            hideModel('choosePostOfficeModal');
        }
    };
    $scope.employees = [];
    $scope.getFirstData = function () {


        $http.get("/EmployeeDebit/GetDataHandle?postId=" + $scope.postHandle).then(function (response) {

            $scope.employees = angular.copy(response.data.employees);

            console.log($scope.employees);

        });

    };

    $scope.init = function () {
        $scope.postOffices = postOfficeDatas;

        $scope.postHandle = '';

        if ($scope.postOffices.length === 1) {
            $scope.postHandle = $scope.postOffices[0];
            $scope.refeshData();
            $scope.getFirstData();
        } else {
            showModelFix('choosePostOfficeModal');
        }

    };

    $scope.prepareCreate = function () {
        $scope.tabcreate = true;
        $scope.createDocumentInfo = { EmployeeID: '', AllMoney: 0, AllMailer: 0 };
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

        if ($scope.createDocumentInfo.EmployeeID === '') {
            showNotify('Chọn nhân viên');
        } else if (listTemps.length === 0) {
            showNotify("Chọn các đơn phải thu");
        } else {
            showLoader(true);
            $http({
                method: "POST",
                url: "/EmployeeDebit/CreateDocument",
                data: {
                    employeeId: $scope.createDocumentInfo.EmployeeID,
                    notes: $scope.createDocumentInfo.Notes,
                    mailers: listTemps,
                    postId: $scope.postHandle
                }
            }).then(function sucess(response) {
                showLoader(false);
                if (response.data.error === 1) {
                    showNotify(response.data.msg);
                } else {
                    $scope.getMailerNotPaid();
                    $scope.refeshData();
                }
            }, function error() {
                showLoader(false);
                showNotify("Mất kết nối mạng")
            });
        }


    };

    $scope.init();
});

