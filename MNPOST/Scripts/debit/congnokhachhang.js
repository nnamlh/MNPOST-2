var app = angular.module('myApp', ['ui.bootstrap', 'ui.select2', 'ui.mask', 'myDirective']);

app.controller('myCtrl', function ($scope, $http) {

    $scope.select2Options = {
        width: '100%'
    };

    $scope.groups = groups;
    $scope.tabcreate = false;
    $scope.numPages;
    $scope.itemPerPage;
    $scope.totalItems;
    $scope.currentPage = 1;
    $scope.maxSize = 5;
    $scope.allDanhMuc = [];
    $scope.danhMuc = {};
    $scope.searchInfo = {
        "month": month,
        "year": year,
        "page": $scope.currentPage,
        "customerId": ""
    };
    $scope.allChiTiet = [];
    $scope.cusNotPayList = [];
    $scope.reportDebit = function () {

        $http.get('/customerdebit/ReportDebit').then(function (res) {
            $scope.cusNotPayList = res.data;
        });

    };

    $scope.createDocument = {
        DethTime: curentDate,
        CusId : ''
    };


    $scope.getData = function () {
       
        showLoader(true);
        $scope.reportDebit();

        $http({
            method: "POST",
            url: "/CustomerDebit/GetDebit",
            data: $scope.searchInfo
        }).then(function mySuccess(response) {
            showLoader(false);

            if (response.data.error === 0) {

                $scope.allDanhMuc = response.data.data;
   
            }

        }, function myError(response) {
            showLoader(false);
            showNotify('Connect error');
        });
    };


    $scope.getData();

    $scope.allMailerDebit = [];
    $scope.edit = function (index) {

        $scope.danhMuc = $scope.allDanhMuc[index];

        $('.nav-tabs a[href="#tab_chitiet"]').tab('show');
        //check khách hàng
        $http.get('/customerdebit/GetDetails?documentId=' + $scope.danhMuc.DocumentID).then(function (res) {
            var result = res.data;
            $scope.allChiTiet = result;
        });

    };

    $scope.getMailerDebit = function () {

        showLoader(true);

        $http(
            {
                method: 'POST',
                url: '/customerdebit/getnotdebit',
                data: $scope.createDocument
            }
        ).then(function sucess(res) {

            showLoader(false);
            var rs = res.data;

            if (rs.error === 1) {
                alert('Sai thông tin');
            } else {
                $scope.allMailerDebit = rs.data;
            }

        });

    };

    $scope.chkAllPaid = function () {
        for (i = 0; i < $scope.allMailerDebit.length; i++) {
            $scope.allMailerDebit[i].isCheck = $scope.isChkPaid;
        }
    };

    $scope.prepareCreate = function () {
        $scope.tabcreate = true;
        $scope.createDocument = {
            DethTime: curentDate,
            CusId: '',
            Notes: ''
        };

        $scope.allMailerDebit = [];
    };

    $scope.changeCusCreate = function () {
        $scope.allMailerDebit = [];
    };


    $scope.sendCreate = function () {
        showLoader(true);
        var listSend = [];
        for (i = 0; i < $scope.allMailerDebit.length; i++) {
            if ($scope.allMailerDebit[i].isCheck) {
                listSend.push($scope.allMailerDebit[i].MailerID);
            }
        }

        $http({
            method: 'POST',
            url: '/customerdebit/create',
            data: {
                ListMailers: listSend,
                CusId: createDocument.CusId,
                DethTime: createDocument.DethTime,
                Notes: createDocument.Notes
            }
        }).then(function sucess(res) {
            showLoader(false); 
            var rs = res.data;

            if (rs.error === 1) {
                alert(rs.msg);

            } else {
                $('.nav-tabs a[href="#tab_phieuthu"]').tab('show');
                $scope.getData();
            }
        });

        console.log(JSON.stringify(listSend));

    };
});