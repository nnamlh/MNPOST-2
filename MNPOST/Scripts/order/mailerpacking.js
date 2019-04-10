var app = angular.module('myApp', ['ui.bootstrap', 'myDirective', 'myKeyPress', 'ui.select2', 'ui.mask']);

app.controller('myCtrl', function ($scope, $http) {

    $scope.select2Options = {
        width: '100%'
    };
    $scope.tabdetail = false;
    $scope.packingStatus = packingStatusData;
    $scope.postHandle = '';
    $scope.posts = [];
    $scope.createDocument = {};
    $scope.provinces = [];
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

        $http.get('/mailerpacking/GetDataRequire?postId=' + $scope.postHandle).then(function (res) {

            $scope.posts = res.data.Posts;
            $scope.provinces = res.data.Provinces;
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

    $scope.preCreateDocument = function () {
        $scope.createDocument = { currentPost: $scope.postHandle };
        showModel('createDocument');
    };

    $scope.transports = [];

    $scope.getTransport = function () {
        $http.get('/mailerpacking/gettransport?type=' + $scope.createDocument.transportType).then(function (res) {
            $scope.transports = res.data;
        });
    };


    $scope.sendCreate = function () {
        console.log(JSON.stringify($scope.createDocument));

        showLoader(true);

        $http({
            method: 'POST',
            url: '/mailerpacking/CreateDocument',
            data: {
                infos: $scope.createDocument
            }
        }).then(function sucess(res) {
            showLoader(false);

            hideModel('createDocument');



        });
    };


    $scope.searchInfo = {
        "fromDate": fromDate,
        "toDate": toDate,
        "status": 0,
        "postId": "",
        "documentCode": ""
    };
    $scope.documents = [];
    $scope.getData = function () {
        $scope.searchInfo.postId = $scope.postHandle;

        showLoader(true);

        $http({
            method: 'POST',
            url: '/mailerpacking/GetPacking',
            data: $scope.searchInfo

        }).then(function sucess(res) {
            showLoader(false);
            $scope.documents = res.data;
        });

    };

    $scope.document = {};
    $scope.details = [];
    $scope.showDetail = function (idx) {
        $scope.document = angular.copy($scope.documents[idx]);

        $http.get('/mailerpacking/gettransport?type=' + $scope.documents[idx].TransportID).then(function (res) {
            $scope.transports = res.data;

        });

        $http.get('/mailerpacking/GetPackingDetails?documentId=' + $scope.documents[idx].DocumentID).then(function (res) {
            $scope.details = res.data;
        });
        $scope.tabdetail = true;
        $('.nav-tabs a[href="#tab_chitiet"]').tab('show');
    };
    $scope.mailerId = '';
    $scope.addMailer = function () {
        showLoader(true);

        $http({
            method: 'POST',
            url: '/mailerpacking/AddMailer',
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
                $scope.details.unshift(res.data.data);
                showNotify('Đã thêm');
            }

        });
    };

    $scope.changeTransport = function () {
        $http.get('/mailerpacking/gettransport?type=' + $scope.document.TransportID).then(function (res) {
            $scope.transports = res.data;
        });
    };

    $scope.removeMailer = function (idx) {
        var mailer = $scope.details[idx];

        var r = confirm("Xóa mailer: " + mailer.MailerID);
        if (r === true) {
            showLoader(true);

            $http({
                method: 'POST',
                url: '/mailerpacking/RemoveMailer',
                data: {
                    documentId: $scope.document.DocumentID,
                    mailerId: mailer.MailerID
                }
            }).then(function sucess(res) {
                showLoader(false);
                if (res.data.error === 1) {
                    showNotify(res.data.msg);
                } else {
                    $scope.details.splice(idx, 1);
                    showNotify('Đã xóa');
                }
            });
        }
    };

    $scope.updateDocument = function () {
        showLoader(true);

        $http({
            method: 'POST',
            url: '/mailerpacking/UpdateDocument',
            data: {
                documentId: $scope.document.DocumentID,
                postId: $scope.document.PostOfficeIDAccept,
                transport: $scope.document.TransportID,
                transportName: $scope.document.TransportName,
                notes: $scope.document.SendDescription,
                tripNumber: $scope.document.TripNumber
            }
        }).then(function sucess(res) {
            showLoader(false);
            if (res.data.error === 1) {
                showNotify(res.data.msg);
            } else {
                $scope.getData();
                showNotify('Đã chỉnh sửa');
            }
        });
    };

    $scope.confirmSend = function () {

        var r = confirm("Xác nhận gửi liên tuyến");
        if (r === true) {
            showLoader(true);

            $http({
                method: 'POST',
                url: '/mailerpacking/ConfirmSend',
                data: {
                    documentId: $scope.document.DocumentID
                }
            }).then(function sucess(res) {
                showLoader(false);
                if (res.data.error === 1) {
                    showNotify(res.data.msg);
                } else {
                    $scope.getData();
                    $scope.document = {};
                    $scope.details = [];
                    $scope.tabdetail = false;
                    $('.nav-tabs a[href="#tab_bangke"]').tab('show');
                    showNotify('Đã chỉnh sửa');
                }
            });
        }

    };

    $scope.mailerEmployeeFinds = [];
    $scope.findAllMailerForEmployee = function () {

        showLoader(true);

        $http.get('/mailerpacking/GetMailerForEmployee?postId=' + $scope.postHandle + '&province=' + $scope.provincesearch + '&district=' + $scope.districtsearch).then(function (response) {

            showLoader(false);

            $scope.mailerEmployeeFinds = angular.copy(response.data);

        });

    };
    $scope.provincesearch = '';
    $scope.districtsearch = '';

    $scope.changeProvince = function () {
        var url = '/mailerinit/GetProvinces?';

        url = url + "parentId=" + $scope.provincesearch + "&type=district";

        $http.get(url).then(function (response) {
            $scope.districts = angular.copy(response.data);

        });
    };

    $scope.fillMailerForEmployee = function () {

        $scope.mailerEmployeeFinds = [];
        $scope.provincesearch = '';
        $scope.districtsearch = '';

        showModel('getmailerdelivery');

    };

    $scope.addMailerAutoFromRoutes = function () {
        showLoader(true);

        var listMailer = [];
        var listTemp = listTemp = $scope.mailerEmployeeFinds;

        for (var i = 0; i < listTemp.length; i++) {
            if (listTemp[i].IsCheck) {
                listMailer.push(listTemp[i].MailerID);
            }
        }

        $http(
            {
                method: 'POST',
                url: '/mailerpacking/AddListMailer',
                data: {
                    documentId: $scope.document.DocumentID,
                    mailers: listMailer
                }
            }
        ).then(function sucess(response) {

            var result = response.data;
            hideModel("getmailerdelivery");
            showLoader(false);

            if (result.error === 1) {
                showNotifyWarm(result.msg);
            } else {
                $http.get('/mailerpacking/GetPackingDetails?documentId=' + $scope.documents[idx].DocumentID).then(function (res) {
                    $scope.details = res.data;
                });
            }

        }, function error(response) {
            showLoader(false);
            showNotify('internet disconnect');
        });

    };

});