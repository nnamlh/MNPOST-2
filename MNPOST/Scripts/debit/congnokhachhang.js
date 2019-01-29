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
        "cusId": ""
    };

    $scope.findNgayChot = function (ngay) {
        var ListNgayChot = [
            { Ngay: 2, Ten: "Thứ 2" },
            { Ngay: 3, Ten: "Thứ 3" },
            { Ngay: 4, Ten: "Thứ 4" },
            { Ngay: 5, Ten: "Thứ 5" },
            { Ngay: 6, Ten: "Thứ 6" },
            { Ngay: 7, Ten: "Thứ 7" },
            { Ngay: 8, Ten: "Chủ nhật" },
            { Ngay: 0, Ten: "Cuối tháng" }
        ];

        for (i = 0; i < ListNgayChot.length; i++) {
            if (ListNgayChot[i].Ngay === ngay) {
                return ListNgayChot[i].Ten;
            }
        }

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
    // xoa du lieu
    $scope.sendDelete = function (index) {
        var info = $scope.allDanhMuc[index];

        var r = confirm("Bạn muốn xóa không ?");
        if (r == true) {
            showLoader(true);

            $http({
                method: "POST",
                url: "/CustomerDebit/delete",
                data: { documentid: info.DocumentID }
            }).then(
                function success(response) {

                    var result = response.data;

                    if (result.error == 0) {
                      //  $scope.allDanhMuc.splice(index, 1);
                        $scope.getData();
                    } else {

                        alert(result.msg);

                    }

                    showLoader(false);

                },
                function errror(response) {
                    showLoader(false);
                    showNotify("connect has disconnect");
                }
                );
        } else {
        }
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
    $scope.findMailerPaid = function () {

        for (var i = 0; i < $scope.allMailerDebit.length; i++) {

            if ($scope.filerPaidList.MailerID === $scope.allMailerDebit[i].MailerID) {
                if ($scope.autoChek) {
                    $scope.allMailerDebit[i].isCheck = true;
                }
            }

            if ($scope.allMailerDebit[i].isCheck) {
                sumCod = sumCod + $scope.allMailerDebit[i].COD;
                countMailer++;
            }

        }

        $scope.createDocument.AllMoney = sumCod;
        $scope.createDocument.AllMailer = countMailer;
    };
    $scope.checkPaidItem = function () {
        var sumCod = 0;
        var countMailer = 0;
        for (var i = 0; i < $scope.allMailerDebit.length; i++) {

            if ($scope.allMailerDebit[i].isCheck) {
                sumCod = sumCod + $scope.allMailerDebit[i].Amount;
                countMailer++;
            }

        }

        $scope.createDocument.AllMoney = sumCod;
        $scope.createDocument.AllMailer = countMailer;
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
                CusId: $scope.createDocument.CusId,
                DethTime: $scope.createDocument.DethTime,
                Notes: $scope.createDocument.Notes
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

    $scope.showPDF = function (url) {

        runShowPDF(url);
    };

});