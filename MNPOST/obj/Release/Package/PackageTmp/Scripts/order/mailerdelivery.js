var app = angular.module('myApp', ['ui.bootstrap', 'myKeyPress', 'myDirective', 'ui.mask', 'ui.select2']);
app.controller('myCtrl', function ($scope, $http, $rootScope, $interval) {

    $scope.select2Options = {
        width: '100%'
    };

    // phan trang
    $scope.showEdit = false;
    $scope.showUpdate = false;
    $scope.provinces = provinceData;
    $scope.districts = [];

    $scope.deliveryStatus = angular.copy(deliveryStatusData);
    $scope.mailerStatus = angular.copy(mailerStatusData);

    $scope.pageChanged = function () {
        $scope.GetData();
    };

    $scope.removeDocument = function (idx) {
        showLoader(true);
        var info = $scope.allDeliveries[idx];
        $http.get("/mailerdelivery/RemoveDocument?documentId=" + info.DocumentID).then(function (response) {
            
            $scope.allDeliveries.splice(idx, 1);
            showLoader(false);
        });
    };

    //nhan vien
    $scope.employees = [];
    $scope.licensePlates = [];
    $scope.listEmployeeMonitor = [];

    var findEmployeeIndex = function (code) {
        for (var i = 0; i < $scope.employees.length; i++) {
            if ($scope.employees[i].code === code) {
                return i;
            }
        }

        return -1;
    };

    $scope.addEmployeeMonitor = function () {

        var chek = true;

        for (var i = 0; i < $scope.listEmployeeMonitor.length; i++) {
            if ($scope.listEmployeeMonitor[i].code === $scope.monitorEmployeeChoose) {
                chek = false;
                break;
            }
        }

        if (chek) {
            $scope.listEmployeeMonitor.push($scope.employees[findEmployeeIndex($scope.monitorEmployeeChoose)]);
        } else {
            showNotify("Đã thêm");
        }

    };
    $scope.deliveryReports = [];
    $scope.getReportEmployeeDelivery = function () {

        var listTemps = [];

        for (var i = 0; i < $scope.listEmployeeMonitor.length; i++) {
            listTemps.push($scope.listEmployeeMonitor[i].code);
        }

        $http({
            method: "POST",
            url: "/mailerdelivery/GetReportEmployeeDelivery",
            data: {
                postId: $scope.postHandle,
                employees: listTemps
            }
        }).then(function sucess(response) {

            $scope.deliveryReports = response.data.data;

        }, function error(reponse) {

        });

    };

    $scope.removeMonitor = function (idx) {
        $scope.listEmployeeMonitor.splice(idx, 1);
    };

    $scope.getFirstData = function () {


        $http.get("/mailerdelivery/GetDataHandle?postId=" + $scope.postHandle).then(function (response) {

            $scope.employees = angular.copy(response.data.employees);

            $scope.licensePlates = angular.copy(response.data.licensePlates);

            console.log($scope.employees);

        });

    };

    $scope.choosePostHandle = function () {
        if ($scope.postHandle === '') {
            alert('Chọn bưu cục nếu không sẽ không thể thao tác');
        } else {
            $scope.getFirstData();
            $scope.getReportEmployeeDelivery();
            $interval(function () {  $scope.getReportEmployeeDelivery() }, 1000 * 60);
            hideModel('choosePostOfficeModal');
        }
    };
    $scope.init = function () {
        $scope.postOffices = postOfficeDatas;

        $scope.postHandle = '';

        if ($scope.postOffices.length === 1) {
            $scope.postHandle = $scope.postOffices[0];
            $scope.getFirstData();
            $scope.getReportEmployeeDelivery();
            $interval(function () {$scope.getReportEmployeeDelivery() }, 1000 * 60);
        } else {
            showModelFix('choosePostOfficeModal');
        }

    };

    var resfeshData = function () {
        $scope.getReportEmployeeDelivery();
    };

    // xu ly chi tiet
    $scope.currentDocument = {};
    $scope.mailers = [];
    $scope.mailerId = '';
    $scope.showDocumentDetail = function (employeeId) {
        $scope.mailers = [];
        $scope.showEdit = true;
        $scope.showUpdate = false;
        $scope.currentDocument = { DocumentDate: currentDate, EmployeeID: employeeId };

        $('.nav-tabs a[href="#tab_chitiet"]').tab('show');
        $scope.getDocumentDetail();

    };

    $scope.getDocumentDetail = function () {

        showLoader(true);
        $http({
            method: "post",
            url: "/mailerdelivery/GetDeliveryMailerDetail",
            data: {
                employeeId: $scope.currentDocument.EmployeeID,
                deliveryDate: $scope.currentDocument.DocumentDate,
                postId: $scope.postHandle
            }
            
        }).then(function sucess(response) {
            showLoader(false);

            var ressult = response.data;

            if (ressult.error === 1) {
                alert(ressult.msg);
            } else {
                $scope.currentDocument = ressult.data.document;
                $scope.mailers = ressult.data.details;
            }

        }, function error() {
            alert("connect error");
            showLoader(false);
        });
    };

    $scope.updateDocument = function () {
        showLoader(true);

        $http({
            method: "POST",
            url: "/mailerdelivery/UpdateDelivery",
            data: {
                documentId: $scope.currentDocument.DocumentID,
                numberPlate: $scope.currentDocument.NumberPlate,
                notes: $scope.currentDocument.Notes
            }
        }).then(function sucess(response) {
           
            showLoader(false);

        }, function error() {
            alert("connect error");
            showLoader(false);
        });

    };

    $scope.addMailer = function () {
        if ($scope.currentDocument.DocumentID === '') {
            alert("Không thể thêm");
        } else {
            showLoader(true);
            $http({
                method: "POST",
                url: "/mailerdelivery/AddMailer",
                data: {
                    documentId: $scope.currentDocument.DocumentID,
                    mailerId: $scope.mailerId
                }
            }).then(function sucess(response) {
                var result = response.data;

                if (result.error === 1) {
                    showNotifyWarm(result.msg);
                } else {
                    showNotify('Đã thêm : ' + $scope.mailerId);
                    $scope.mailers.push(result.data);
                }
                $scope.mailerId = "";

                showLoader(false);

            }, function error() {
                alert("connect error");
                showLoader(false);
            });
        }
    };

    // huy phat
    $scope.detroyMailerDelivery = function (detailId) {
        showLoader(true);
        $http({
            method: 'POST',
            url: '/mailerdelivery/DetroyMailerDelivery',
            data: {
                id: detailId
            }
        }).then(function success(response) {
            showLoader(false);
            $scope.getDocumentDetail();
            resfeshData();
            showNotify('Đã xóa');

        }, function error(response) {
            alert("connect error");
            showLoader(false);

        });
    };

    //  cap nhat mailer
    $scope.mailerUpdates = [];
    $scope.mailerIdUpdate = '';
    $scope.returnReasons = angular.copy(reasonReturnDatas);
    $scope.confirmAllMailers = function () {

        $scope.showEdit = false;
        $scope.showDeliveries = false;
        $scope.showUpdate = true;

        $('.nav-tabs a[href="#tab_capnhatphat"]').tab('show');

        $http.get("/mailerdelivery/GetDeliveryMailerDetailNotUpdate?documentID=" + $scope.currentDocument.DocumentID).then(
            function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    var mailerId = response.data[i].MailerID;
                    if (findMailerUpdatesIndex(mailerId) === -1) {
                        response.data[i].DeliveryStatus = 4;
                        response.data[i].DeliveryDate = currentDate;
                        response.data[i].DeliveryTime = currentTime;
                        $scope.mailerUpdates.push(response.data[i]);
                    }

                }

            }
        );

    };

    $scope.changeReason = function (idx) {
        var data = $scope.mailerUpdates[idx];

        for (i = 0; i < $scope.returnReasons.length; i++) {
            if ($scope.returnReasons[i].code === data.ReturnReasonID) {
                $scope.mailerUpdates[idx].DeliveryNotes = $scope.returnReasons[i].name;
            }
        }
        
    };

    function findMailerUpdatesIndex(mailerId) {
        for (var i = 0; i < $scope.mailerUpdates.length; i++) {
            if ($scope.mailerUpdates[i].MailerID === mailerId)
                return i;
        }

        return -1;
    }

    $scope.addMailerUpdate = function (isvalid) {

        $http.get('/mailerdelivery/GetMailerForReUpdate?mailerID=' + $scope.mailerIdUpdate).then(function (response) {

            var result = response.data;

            if (result.error === 1) {
                showNotifyWarm(result.msg);
            } else {

                if (findMailerUpdatesIndex(result.data.MailerID) === -1) {
                    result.data.DeliveryStatus = 4;
                    result.data.DeliveryDate = currentDate;
                    result.data.DeliveryTime = currentTime;

                    $scope.mailerUpdates.push(result.data);



                }
                showNotify('Đã thêm ' + $scope.mailerIdUpdate);
                $scope.mailerIdUpdate = '';
            }


        });

    };



    $scope.init();

    // tao tu dong
    $scope.autoRoutes = [];
    $scope.countMailers = 0;
    $scope.getAutoRoutes = function () {

        showLoader(true);
        $http.get('/mailerdelivery/AutoGetRouteEmployees?postId=' + $scope.postHandle).then(function (response) {

            showLoader(false);
            $scope.autoRoutes = angular.copy(response.data.routes);
            $scope.countMailers = response.data.coutMailer;
            showModel('autoRoutes');

        });

    };

    $scope.createAutoOneEmployee = function (idx) {

        var routesSend = [];
        routesSend.push($scope.autoRoutes[idx]);
        showLoader(true);
        $http({
            method: "POsT",
            url: "/mailerdelivery/CreateFromRoutes",
            data: {
                routes: routesSend,
                postId: $scope.postHandle
            }
        }).then(function sucess(response) {
            hideModel('autoRoutes');
            showLoader(false);
            $scope.resfeshData();
        }, function error(response) {
            showLoader(false);
            alert("connect error");
        });

    };


    $scope.createAutoAllEmployee = function () {

        showLoader(true);
        $http({
            method: "POsT",
            url: "/mailerdelivery/CreateFromRoutes",
            data: {
                routes: $scope.autoRoutes,
                postId: $scope.postHandle
            }
        }).then(function sucess(response) {
            showLoader(false);
            hideModel('autoRoutes');
            resfeshData();
           
        }, function error(response) {
            showLoader(false);
            alert("connect error");
        });

    };

    $scope.mailerNotFinishOfDate = [];
    $scope.showMailerOfDate = function (employeeId) {
        showLoader(true);

        $http.get("/mailerdelivery/ShowMailerNotFinishOfDate?employeeId=" + employeeId).then(function (response) {
            showLoader(false);

            $scope.mailerNotFinishOfDate = response.data;
            showModel('mailerNotfinish');
        });

    };

    // do danh sach tu dong
    $scope.fillMailerForEmployee = function () {


        showLoader(true);

        $http.get('/mailerdelivery/GetAutoMailerFromEmployeeRoute?postId=' + $scope.postHandle + '&employeeId=' + $scope.currentDocument.EmployeeID).then(function (response) {

            $scope.mailerEmployeeFinds = [];

            showLoader(false);

            $scope.employeeMailerFromRoutes = angular.copy(response.data.routes);
            $scope.countMailers = response.data.countMailer;

            showModel('getmailerdelivery');

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

    $scope.changeCheckAllMailerEmplpoyeeAuto = function () {

        for (var i = 0; i < $scope.employeeMailerFromRoutes.Mailers.length; i++) {
            $scope.employeeMailerFromRoutes.Mailers[i].IsCheck = $scope.checkAllMailerAuto;
        }

    };

    $scope.mailerEmployeeFinds = [];
    $scope.findAllMailerForEmployee = function () {

        showLoader(true);

        $http.get('/mailerdelivery/GetMailerForEmployee?postId=' + $scope.postHandle + '&province=' + $scope.provincesearch + '&district=' + $scope.districtsearch).then(function (response) {

            showLoader(false);

            $scope.mailerEmployeeFinds = angular.copy(response.data);

        });

    };

    $scope.checkAllMailerForEmployeeChange = function () {
        for (var i = 0; i < $scope.mailerEmployeeFinds.length; i++) {
            $scope.mailerEmployeeFinds[i].IsCheck = $scope.checkAllMailerForEmployee;
        }
    };

    // do danh sach tu danh sach lay tuyen tu donng
    // type : route --> lay tu employeeMailerFromRoutes
    // type : province --> lay tu mailerEmployeeFinds
    $scope.addMailerAutoFromRoutes = function (type) {
        showLoader(true);

        var listMailer = [];
        var listTemp = [];
        if (type === 'route') {
            listTemp = $scope.employeeMailerFromRoutes.Mailers;
        } else {
            listTemp = $scope.mailerEmployeeFinds;
        }

        for (var i = 0; i < listTemp.length; i++) {
            if (listTemp[i].IsCheck) {
                listMailer.push(listTemp[i].MailerID);
            }
        }

        $http(
            {
                method: 'POST',
                url: '/mailerdelivery/AddListMailer',
                data: {
                    documentId: $scope.currentDocument.DocumentID,
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
                $scope.getDocumentDetail();
                resfeshData();
            }

        }, function error(response) {
            showLoader(false);
            showNotify('internet disconnect');
        });

    };

    //
    $scope.confirmDeliveryMailer = function (idx) {

        var sendUpdate = true;
        var mailer = $scope.mailerUpdates[idx];

        if (mailer.DeliveryDate === "" || mailer.DeliveryTime === "") {
            showNotify(mailer.MailerID + ' chưa nhập ngày giờ');
            sendUpdate = false;
        } else if (mailer.DeliveryStatus === 4) {
            // da phat
            if (mailer.DeliveryTo === '') {
                showNotify(mailer.MailerID + ' chưa nhập người nhận');
                sendUpdate = false;
            }
        } else if (mailer.DeliveryStatus === 5) {
            // chuyen hoan
            if (mailer.ReturnReasonID === '') {
                showNotify(mailer.MailerID + ' chưa nhập lý do');
                sendUpdate = false;

            }
        } else if (mailer.DeliveryStatus === 6) {
            // chuyen hoan
            if (mailer.DeliveryNotes === '') {
                showNotify(mailer.MailerID + ' chưa nhập ghi chú');
                sendUpdate = false;

            }
        }


        if (sendUpdate) {
            showLoader(true);
            $http({
                method: 'POST',
                url: '/mailerdelivery/ConfirmDeliveyMailer',
                data: {
                    detail: mailer
                }
            }).then(function sucess(response) {

                showLoader(false);
                if (response.data.error === 1) {
                    showNotify(response.data.msg);
                } else {
                    $scope.mailerUpdates.splice(idx, 1);
                    showNotify('Đã cập nhật phát');
                    // $scope.mailers = [];
                    //   $scope.currentDocument = {};
                    //resfeshData();
                    $scope.getDocumentDetail();
                }

            }, function error(response) {
                showLoader(false);
                alert("connect error");
            });
        }
    };

});