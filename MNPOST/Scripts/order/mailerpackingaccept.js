var app = angular.module('myApp', ['ui.bootstrap', 'myDirective', 'myKeyPress', 'ui.select2', 'ui.mask']);

app.controller('myCtrl', function ($scope, $http) {

    $scope.select2Options = {
        width: '100%'
    };

    $scope.packingStatus = packingStatusData;
    $scope.postHandle = '';
    $scope.posts = [];
    $scope.status = angular.copy(mailerStatusData);
    $scope.choosePostHandle = function () {
        if ($scope.postHandle === '') {
            alert('Chọn bưu cục nếu không sẽ không thể thao tác');
        } else {
            $scope.getDataFrist();
            hideModel('choosePostOfficeModal');
        }
    };
    $scope.getDataFrist = function () {

        $http.get('/mailerpackingaccept/GetDataRequire?postId=' + $scope.postHandle).then(function (res) {

            $scope.posts = res.data.Posts;
        });

    };

    $scope.init = function () {
        $scope.postOffices = postOfficeDatas;

        $scope.postHandle = '';

        if ($scope.postOffices.length === 1) {
            $scope.postHandle = $scope.postOffices[0];
            $scope.getDataFrist();
        } else {
            showModelFix('choosePostOfficeModal');
        }

    };

    $scope.init();

    $scope.searchInfo = {
        "fromDate": fromDate,
        "toDate": toDate,
        "status": 1,
        "postId": "",
        "documentCode": "",
        "postSend": ""
    };
    $scope.documents = [];
    $scope.getData = function () {
        $scope.searchInfo.postId = $scope.postHandle;

        showLoader(true);

        $http({
            method: 'POST',
            url: '/mailerpackingaccept/GetPacking',
            data: $scope.searchInfo

        }).then(function sucess(res) {
            showLoader(false);
            $scope.documents = res.data;
        });

    };

    $scope.document = {};
    $scope.details = [];
    $scope.tabdetail = false;
    $scope.showDetail = function (idx) {
        $scope.document = angular.copy($scope.documents[idx]);

        $http.get('/mailerpackingaccept/GetPackingDetails?documentId=' + $scope.documents[idx].DocumentID).then(function (res) {
            $scope.details = res.data;
        });
        $scope.tabdetail = true;
        $('.nav-tabs a[href="#tab_chitiet"]').tab('show');
    };


    $scope.accpetDocument = function () {
        showLoader(true);

        $http({
            method: 'POST',
            url: '/mailerpackingaccept/AcceptDocument',
            data: {
                documentId: $scope.document.DocumentID
            }
        }).then(function sucess(res) {
            showLoader(false);
            if (res.data.error === 1) {
                showNotify(res.data.msg);
            } else {
                $scope.document.StatusID = 2;
                showNotify('Đã nhận');
            }
        });

    };

    $scope.mailerId = '';
    $scope.addMailer = function () {
        showLoader(true);

        $http({
            method: 'POST',
            url: '/mailerpackingaccept/AddMailer',
            data: {
                documentId: $scope.document.DocumentID,
                mailerId: $scope.mailerId
            }
        }).then(function sucess(res) {

            showLoader(false);

            if (res.data.error === 1) {
                showNotify(res.data.msg);
            } else {
                $scope.mailerId = '';
                $http.get('/mailerpackingaccept/GetPackingDetails?documentId=' + $scope.document.DocumentID).then(function (res) {
                    $scope.details = res.data;
                });
                showNotify('Đã nhận');
            }

        });
    };

    $scope.acceptAll = function () {

        var r = confirm("Xóa nhận toàn bộ danh sách, khuyến cáo nên kiểm tra kỹ");
        if (r === true) {
            showLoader(true);
            $http({
                method: 'POST',
                url: '/mailerpackingaccept/AcceptAll',
                data: {
                    documentId: $scope.document.DocumentID
                }
            }).then(function sucess(res) {

                showLoader(false);

                if (res.data.error === 1) {
                    showNotify(res.data.msg);
                } else {
                    $scope.mailerId = '';
                    $http.get('/mailerpackingaccept/GetPackingDetails?documentId=' + $scope.document.DocumentID).then(function (res) {
                        $scope.details = res.data;
                    });
                    showNotify('Đã xác nhận danh sách');
                }

            });
        }

    };

});