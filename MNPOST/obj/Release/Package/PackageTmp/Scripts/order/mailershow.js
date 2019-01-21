var app = angular.module('myApp', ['ui.bootstrap', 'myDirective', 'myKeyPress', 'ui.select2', 'ui.mask']);

app.controller('myCtrl', function ($scope, $http, $rootScope) {


    $scope.select2Options = {
        width: 'element'
    };

    $scope.numPages;
    $scope.itemPerPage;
    $scope.totalItems;
    $scope.currentPage = 1;
    $scope.maxSize = 5;
    $scope.showEdit = false;
    $scope.checkMailers = false;

    $scope.customers = angular.copy(customerDatas);
    $scope.customers.unshift({
        name: 'Tất cả',
        code: ''
    });

    $scope.status = angular.copy(mailerStatusData);
    $scope.status.unshift({ "code": -1, "name": "TẤT CẢ" });
    $scope.findStatus = function (code) {
        for (var i = 0; i < $scope.status.length; i++) {
            if ($scope.status[i].code === code)
                return $scope.status[i].name;
        }

    };

    $scope.pageChanged = function () {
        $scope.GetData();
    };

    $scope.searchInfo = {
        "search": "",
        "customerId": '',
        "fromDate": fromDate,
        "toDate": toDate,
        "status": -1,
        "page": $scope.currentPage,
        "postId": ""
    };

    $scope.mailers = [];
    $scope.GetData = function () {
        $scope.searchInfo.page = $scope.currentPage;
        $scope.searchInfo.postId = $scope.postHandle;

        showLoader(true);
        $http({
            method: "POST",
            url: "/mailer/getmailers",
            data: $scope.searchInfo
        }).then(function mySuccess(response) {
            showLoader(false);

            if (response.data.error === 0) {
                $scope.itemPerPage = response.data.pageSize;
                $scope.totalItems = response.data.toltalSize;
                $scope.numPages = Math.round($scope.totalItems / $scope.itemPerPage);

                $scope.mailers = response.data.data;
                console.log($scope.mailers);
            } else {
                alert(response.data.msg);
            }

        }, function myError(response) {
            showLoader(false);
            showNotify('Connect error');
        });
    }
    $scope.GetData();
    $scope.checkAll = function () {
        for (var i = 0; i < $scope.mailers.length; i++) {
            $scope.mailers[i].isCheck = $scope.checkMailers;
        }
    };
    $scope.reportUrl = '#';
    $scope.printMailers = function () {

        var listMailers = '';
        for (var i = 0; i < $scope.mailers.length; i++) {
            if ($scope.mailers[i].isCheck) {
                listMailers = listMailers + ',' + $scope.mailers[i].MailerID;
            }
        }

        if (listMailers.charAt(0) === ',') {
            listMailers = listMailers.substr(1);
        }


        if (listMailers === '') {
            showNotify("Phải chọn vận đơn để in");
        } else {
            $scope.showPDF('/report/PhieuGui?mailerId=' + listMailers);
        }
        
    };

    $scope.tracks = [];

    $scope.getTracking = function () {
        showLoader(true);
        var mailerId = $scope.mailer.MailerID;
        $http.get('/mailer/GetTracking?mailerId=' + mailerId).then(function (response) {
            showLoader(false);
            var result = response.data;
            if (result.error === 0) {
                $scope.tracks = result.data;
                showModel('showtracking');
            } else {
                showNotify("Không lấy được")
            }

        });

    };


    $scope.init = function () {
        $scope.postOffices = postOfficeDatas;

        $scope.postHandle = '';

        if ($scope.postOffices.length === 1) {
            $scope.postHandle = $scope.postOffices[0];
        } else {
            showModelFix('choosePostOfficeModal');
        }

    };


    $scope.choosePostHandle = function () {
        if ($scope.postHandle === '') {
            alert('Chọn bưu cục nếu không sẽ không thể thao tác');
        } else {
            $scope.getCustomerData();
            hideModel('choosePostOfficeModal');
        }
    };

    //
    $scope.mailer = {};
    $scope.customers = customerDatas;
    $scope.mailerTypes = mailerTypesGet;
    $scope.payments = paymentsGet;
    $scope.otherServices = servicesGet;
    $scope.merchandises = [{ 'code': 'H', 'name': 'Hàng hóa' }, { 'code': 'T', 'name': 'Tài liệu' }];
    $scope.showMailerDetail = function (idx) {
        $scope.showEdit = true;
        $('.nav-tabs a[href="#tab_chitiet"]').tab('show');
        $scope.mailer = $scope.mailers[idx];
        $scope.getDistrictAndWard("send");
        $scope.getDistrictAndWard("recei");
        $scope.otherServices = angular.copy(servicesGet);
        console.log($scope.otherServices);
        $http.get("/mailer/GetMailerService?mailerId=" + $scope.mailer.MailerID).then(function (response) {
            $scope.mailer.Services = response.data;

            for (var i = 0; i < $scope.mailer.Services.length; i++) {
                for (var j = 0; j < $scope.otherServices.length; j++) {

                    if ($scope.mailer.Services[i].code === $scope.otherServices[j].code) {
                        $scope.otherServices[j].choose = true;
                        $scope.otherServices[j].price = $scope.mailer.Services[i].price;
                    }

                }
            }
        });

    };
    $scope.setMerchandiseValue = function () {
        $scope.mailer.MerchandiseValue = $scope.mailer.COD;
    };

    $scope.cancelReturn = function (mailerId) {

        $http({

            method: 'POST',
            url: '/mailer/cancelReturn',
            data: {
                mailerId: mailerId
            }

        }).then(function sucess(response) {
            if (response.data.error === 1) {
                alert(response.data.msg);
            } else {
                $scope.GetData();
            }
        });

    };

    $scope.changePrice = function () {
        console.log('tinh gia tong');
        $scope.mailer.Amount = $scope.mailer.PriceDefault + $scope.mailer.PriceCoD + $scope.mailer.PriceService;
    };


    $scope.addSeviceMorePrice = function () {
        var total = 0;
        $scope.mailer.Services = [];
        for (var i = 0; i < $scope.otherServices.length; i++) {
            if ($scope.otherServices[i].choose) {
                total = total + $scope.otherServices[i].price;
                $scope.mailer.Services.push($scope.otherServices[i]);
            }
        }
        console.log(total);
        $scope.mailer.PriceService = total;
        $scope.changePrice();
    };

    $scope.updateMailer = function () {

        showLoader(true);

        $http({
            method: "POST",
            url: "/mailer/UpdateMailers",
            data: {
                mailer: $scope.mailer
            }
        }).then(function sucess(response) {
            var result = response.data;
            showLoader(false);
            if (result.error === 1) {
                showNotify(result.msg);
            } else {
                $scope.GetData();
            }

        }, function error(response) {
            showLoader(false);
            showNotify("Mất kết nối mạng");
        });

    };

    $scope.init();
    $scope.provinceSend = provinceSendGet;
    $scope.provinceRecei = angular.copy($scope.provinceSend);

    $scope.districtSend = [];
    $scope.wardSend = [];

    $scope.districtRecei = [];
    $scope.wardRecei = [];
    $scope.provinceChange = function (pType, type) {

        var url = '/mailerinit/GetProvinces?';

        if (type === 'send') {

            if (pType === "district") {
                url = url + "parentId=" + $scope.mailer.SenderProvinceID + "&type=district";
            }

            if (pType === "ward") {
                url = url + "parentId=" + $scope.mailer.SenderDistrictID + "&type=ward";
            }

        } else {
            if (pType === "district") {
                url = url + "parentId=" + $scope.mailer.RecieverProvinceID + "&type=district";
            }

            if (pType === "ward") {
                url = url + "parentId=" + $scope.mailer.RecieverDistrictID + "&type=ward";
            }
        }

        $http.get(url).then(function (response) {

            if (type === 'send') {

                if (pType === "district") {
                    $scope.districtSend = angular.copy(response.data);
                }

                if (pType === "ward") {
                    $scope.wardSend = angular.copy(response.data);
                }

            } else {
                if (pType === "district") {
                    $scope.districtRecei = angular.copy(response.data);
                }

                if (pType === "ward") {
                    $scope.wardRecei = angular.copy(response.data);
                }
            }

        });

    };

    $scope.getDistrictAndWard = function (type) {

        var url = '/mailerinit/GetDistrictAndWard?provinceId=';

        if (type === 'send') {
            url = url + $scope.mailer.SenderProvinceID + "&districtId=" + $scope.mailer.SenderDistrictID;
        } else {
            url = url + $scope.mailer.RecieverProvinceID + "&districtId=" + $scope.mailer.RecieverDistrictID;
        }

        $http.get(url).then(function (response) {
            if (type === "send") {
                $scope.districtSend = angular.copy(response.data.districts);
                // $scope.mailer.ListWardSend = angular.copy(response.data.wards);
            } else {
                $scope.districtRecei = angular.copy(response.data.districts);
                $scope.wardRecei = angular.copy(response.data.wards);
            }
        });
    };
    $scope.calPrice = function () {
        console.log("tinh gia");
        showLoader(true);
        $http({
            method: "POST",
            url: "/mailer/CalBillPrice",
            data: {
                'weight': $scope.mailer.Weight,
                'customerId': $scope.mailer.SenderID,
                'provinceId': $scope.mailer.SenderProvinceID,
                'serviceTypeId': $scope.mailer.MailerTypeID,
                'postId': $scope.mailer.PostOfficeAcceptID,
                'cod': $scope.mailer.COD,
                'merchandiseValue': $scope.mailer.MerchandiseValue
            }
        }).then(function mySuccess(response) {
            console.log(response.data);
            showLoader(false);
            $scope.mailer.PriceDefault = response.data.price;
            $scope.mailer.PriceCoD = response.data.codPrice;

            $scope.mailer.Amount = $scope.mailer.PriceDefault + $scope.mailer.PriceCoD + $scope.mailer.PriceService;

        }, function myError(response) {
            alert('Connect error');
        });
    };
    $scope.changePrice = function () {
        console.log('tinh gia tong');
        $scope.mailer.Amount = $scope.mailer.PriceDefault + $scope.mailer.PriceCoD + $scope.mailer.PriceService;
    };
    $scope.showPDF = function (url) {

        runShowPDF(url);
    };

});


var autocompleteSend;
var autocompleteRecei;

function fillInAddressSend() {

    var place = autocompleteSend.getPlace();

    var result = handleAutoCompleteAddress(place);



};

function fillInAddressRecei() {

    var place = autocompleteRecei.getPlace();
    var result = handleAutoCompleteAddress(place);


};



function initAutocomplete() {
    autocompleteSend = new createAutoCompleteAddress('autocompleteSend', fillInAddressSend);
    autocompleteRecei = new createAutoCompleteAddress('autocompleteRecei', fillInAddressRecei);
};

