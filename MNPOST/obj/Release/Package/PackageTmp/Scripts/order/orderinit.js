
// tao controller
var app = angular.module('myApp', ['ui.bootstrap', 'myDirective', 'myKeyPress', 'ui.uploader', 'ui.select2', 'ngSanitize']);

app.service('mailerService', function () {

    var mailerList = [];

    var merchandise = [{ 'code': 'H', 'name': 'Hàng hóa' }, { 'code': 'T', 'name': 'Tài liệu' }];

    var customers = {};

    var setCustomer = function (data) {
        customers = angular.copy(data);
    };

    var mailerTypes = mailerTypesGet;

    var payments = paymentsGet;


    var postchoose = '';

    var setPost = function (post) {
        postchoose = post;
    };

    var getPost = function () {
        return postchoose;
    };

    var actionEdit = true;

    var addMailer = function (newObj) {
        mailerList.unshift(newObj);
    };

    var addNewMailer = function () {
        var data = getNewMailer();

        mailerList.unshift(data);

    };


    var getNewMailer = function () {
        return {
            'MailerID': '', 'SenderID': '', 'SenderName': '', 'SenderAddress': '', 'SenderWardID': '', 'SenderDistrictID': '',
            'SenderProvinceID': '', 'SenderPhone': '', 'RecieverName': ''
            , 'RecieverAddress': '', 'RecieverWardID': '', 'RecieverDistrictID': '', 'RecieverProvinceID': '',
            'RecieverPhone': '', 'Weight': 0.01, 'Quantity': 1, 'PaymentMethodID': 'NGTT', 'MailerTypeID': 'SN',
            'PriceService': 0, 'MerchandiseID': 'H', 'Services': angular.copy(servicesGet), 'MailerDescription': '', 'Notes': '', 'COD': 0, 'LengthSize': 0, 'WidthSize': 0, 'HeightSize': 0, 'Amount': 0, 'PriceCoD': 0,
            'PriceDefault': 0, 'ListProvinceSend': provinceSendGet, 'ListDistrictSend': [], 'ListProvinceRecive': provinceSendGet, 'ListDistrictRecive': [], 'ListWardRecive':[]
        };
    };


    var resetMailers = function () {
        mailerList = [];
    };

    var getServices = function () {
        return services;
    };

    var getMailerCurrent = function () {
        return mailer;
    };
    var getActionEdit = function () {
        return actionEdit;
    };

    var getMailers = function () {
        return mailerList;
    };

    var removeMailer = function (idx) {
        mailerList.splice(idx, 1);
    };


    var getCustomers = function () {
        return customers;
    };

    var getMailerTypes = function () {
        return mailerTypes;
    };

    var getPayments = function () {
        return payments;
    };

    var findCustomer = function (code) {
        for (var i = 0; i < customers.length; i++)
            if (customers[i].code === code)
                return customers[i];
        return null;
    };

    var getMerchandises = function () {

        return merchandise;
    };

    return {
        addMailer: addMailer,
        getMailers: getMailers,
        addNewMailer: addNewMailer,
        removeMailer: removeMailer,
        getCustomers: getCustomers,
        getMailerTypes: getMailerTypes,
        getPayments: getPayments,
        findCustomer: findCustomer,
        getActionEdit: getActionEdit,
        getNewMailer: getNewMailer,
        getMerchandises: getMerchandises,
        resetMailers: resetMailers,
        getPost: getPost,
        setPost: setPost,
        setCustomer: setCustomer
    };

});


// controller main --> gird mailer
app.controller('myCtrl', function ($scope, $http, $rootScope, mailerService, uiUploader) {

    $scope.select2Options = {

    };
    $scope.select2OptionsWidth100 = {
        width: '100%'
    };

    $scope.hideSenderDetail = false;

    $scope.mailers = mailerService.getMailers();
    $scope.customers = mailerService.getCustomers();
    $scope.mailerTypes = mailerService.getMailerTypes();
    $scope.payments = mailerService.getPayments();
    $scope.merchandises = mailerService.getMerchandises();

    $scope.postchoose = '';

    $scope.currentIdx = -1;

    $scope.addNewMailer = function () {
        mailerService.addNewMailer();

    };

    $scope.removeMailer = function (idx) {
        mailerService.removeMailer(idx);
    };

    $scope.copyMailer = function (idx) {
        var mailer = angular.copy($scope.mailers[idx]);
        mailer.MailerID = '';
        mailerService.addMailer(mailer);
    };

    $scope.GetFirst = function () {

        $http.get("/mailerinit/GetCustomer?postId=" + $scope.postchoose).then(function (response) {
            mailerService.setCustomer(response.data);

            $scope.customers = mailerService.getCustomers();
        });

    };

    $scope.init = function () {
        $scope.postOffices = postOfficesData;

        $scope.postchoose = '';

        if ($scope.postOffices.length === 1) {
            $scope.postchoose = $scope.postOffices[0];
            mailerService.setPost($scope.postchoose);
            $scope.GetFirst();
        } else {
            showModelFix('choosePostOfficeModal');
        }

    };

    $scope.init();

    $scope.choosePostHandle = function () {
        if ($scope.postchoose === '') {
            alert('Chọn bưu cục nếu không sẽ không thể thao tác');
        } else {
            $scope.postchoose = $scope.postchoose;
            mailerService.setPost($scope.postchoose);
            $scope.GetFirst();
            hideModel('choosePostOfficeModal');
        }
    };
    $scope.changeCus = function (idx) {
        var mailer = $scope.mailers[idx];

        var cus = mailerService.findCustomer(mailer.SenderID);
      

        if (cus.code.indexOf('KHACHLE') === -1) {
            showNotify("Đang chọn: " + cus.name);
            $scope.hideSenderDetail = false;
        } else {
            showNotify("Khách lẻ phải thu tiền mặt và nhập địa chỉ");
            $scope.hideSenderDetail = true;
        }

        if (cus) {
            $scope.mailers[idx].SenderName = cus.name;
            $scope.mailers[idx].SenderPhone = cus.phone;
            $scope.mailers[idx].SenderProvinceID = cus.provinceId;
            $scope.mailers[idx].SenderAddress = cus.address;
           
            $scope.mailers[idx].SenderWardID = cus.wardId;

            $scope.provinceChange('district', 'send', idx, function (district) {
                $scope.mailers[idx].SenderDistrictID = cus.districtId;
            });
        }
    };

    $scope.editMailer = function (idx) {
        $scope.currentIdx = idx;
        var mailer = $scope.mailers[idx];
        $rootScope.$broadcast('insert-started', { mailer: mailer, actionEdit: true });

        showModel('insertmodal');

    };


    $scope.getLocation = function (val) {
        return $http.get('https://maps.googleapis.com/maps/api/place/findplacefromtext/json', {
            params: {
                input : val,
                key: 'AIzaSyADnh0hHCYqkIrqDaRoGtiSm04yd6PHkD4',
                language: 'vi'
            }
        }).then(function (response) {
            return response.data.results.map(function (item) {
                console.log(item);
                return item.formatted_address;
            });
        });
    };

    $scope.getAddressInfoTemp = function (val) {
        return $http.get('/mailer/GetAddressTemp?phone=' + val).then(function (response) {
            return response.data;
        });
    };

    $scope.showAddressTemp = function (phone, idx) {
        $scope.mailers[idx].RecieverName = phone.Name;
        $scope.mailers[idx].RecieverPhone = phone.Phone;
        $scope.mailers[idx].RecieverAddress = phone.AddressInfo;
        $scope.mailers[idx].RecieverProvinceID = phone.ProvinceId;
        $scope.mailers[idx].RecieverDistrictID = phone.DistrictId;
        $scope.mailers[idx].RecieverWardID = phone.WardId;
        $scope.provinceChange('district', 'recie', idx);

        $scope.provinceChange('ward', 'recie', idx);
    };

    $scope.getMailerCode = function (idx) {
        var mailer = $scope.mailers[idx];


        showLoader(true);
        var url = "/MailerInit/GeneralCode?postId=" + mailerService.getPost();

        $http.get(url).then(function (response) {

            $scope.mailers[idx].MailerID = response.data.code;
            showLoader(false);
        });


    };


    $scope.finishForm = function (valid) {

        if ($scope.postchoose === '') {
            alert('Chọn bưu cục xử lý');
        } else if ($scope.mailers.length === 0) {
            alert("Không có đơn nào để cập nhật");
        } else {
            showLoader(true);
            //'ListProvinceSend': provinceSendGet, 'ListDistrictSend': [], 'ListProvinceRecive': provinceSendGet, 'ListDistrictRecive': [], 'ListWardRecive':[]
            for (i = 0; i < $scope.mailers.length; i++) {
                $scope.mailers[i].ListProvinceSend = [];
                $scope.mailers[i].ListDistrictSend = [];
                $scope.mailers[i].ListProvinceRecive = [];
                $scope.mailers[i].ListDistrictRecive = [];
                $scope.mailers[i].ListWardRecive = [];
            }

            $http({
                method: "POST",
                url: "/mailerinit/InsertMailers",
                data: {
                    mailers: $scope.mailers,
                    postId: $scope.postchoose
                }
            }).then(function mySuccess(response) {

                mailerService.resetMailers();
                $scope.mailers = mailerService.getMailers();
                for (var i = 0; i < response.data.data.length; i++) {
                    mailerService.addMailer(response.data.data[i]);
                }

                showLoader(false);

                alert('Đã thêm hoàn thành');

            }, function myError(response) {
                alert('Connect error');
                showLoader(false);
            });
        }

    };

    $scope.createMailerDetail = function () {

        $rootScope.$broadcast('insert-started', { mailer: mailerService.getNewMailer(), actionEdit: false });

        showModel('insertmodal');

    };

    $scope.$on('result-edit-started', function (event, args) {
        var mailer = args.mailer;

        $scope.mailers[$scope.currentIdx] = angular.copy(mailer);
    });

    $scope.senderExcelInfo = {};
    $scope.provinceSend = provinceSendGet;
    $scope.districtSend = [];
    $scope.wardSend = [];
    $scope.addByExcelFile = function () {
        showModel('insertbyexcel');
    };


    $scope.provinceChange = function (pType, type, idx) {

        var url = '/mailerinit/GetProvinces?';

        if (type === 'send') {

            if (pType === "district") {
                url = url + "parentId=" + $scope.mailers[idx].SenderProvinceID + "&type=district";
            }

            if (pType === "ward") {
                url = url + "parentId=" + $scope.mailers[idx].SenderDistrictID + "&type=ward";
            }

        } else {
            if (pType === "district") {
                url = url + "parentId=" + $scope.mailers[idx].RecieverProvinceID + "&type=district";
            }

            if (pType === "ward") {
                url = url + "parentId=" + $scope.mailers[idx].RecieverDistrictID + "&type=ward";
                console.log(JSON.stringify($scope.mailers[idx].ListDistrictRecive));
                for (var i = 0; i < $scope.mailers[idx].ListDistrictRecive.length; i++) {
                    if ($scope.mailers[idx].ListDistrictRecive[i].code === $scope.mailers[idx].RecieverDistrictID) {
                       
                        for (j = 0; j < $scope.mailers[idx].Services.length; j++) {
                            if ($scope.mailers[idx].Services[j].code === 'VSVX') {
                                $scope.mailers[idx].Services[j].choose = $scope.mailers[idx].ListDistrictRecive[i].vsvx;
                            }
                        }

                        console.log(JSON.stringify($scope.mailers[idx].Services));
                    }
                }
            }
        }


        $http.get(url).then(function (response) {
            console.log(JSON.stringify(response.data));
            if (type === 'send') {

                if (pType === "district") {
                    $scope.mailers[idx].ListDistrictSend = angular.copy(response.data);
                }

                if (pType === "ward") {
                    $scope.mailers[idx].ListWardSend = angular.copy(response.data);
                }

            } else {
                if (pType === "district") {
                    $scope.mailers[idx].ListDistrictRecive = angular.copy(response.data);
                }

                if (pType === "ward") {
                    $scope.mailers[idx].ListWardRecive = angular.copy(response.data);
                }
            } 
            /*
            if (typeof (callback) === typeof (Function)) {
                callback();
            }*/
        });

    };

    $scope.changeExcelCus = function (idx) {

        var cus = mailerService.findCustomer($scope.senderExcelInfo.senderID);

        if (cus) {
            $scope.senderExcelInfo.senderName = cus.name;
            $scope.senderExcelInfo.senderPhone = cus.phone;
            $scope.senderExcelInfo.senderProvince = cus.provinceId;
            $scope.senderExcelInfo.senderAddress = cus.address;
            $scope.senderExcelInfo.senderDistrict = cus.districtId;
            $scope.senderExcelInfo.senderWard = cus.wardId;
            $scope.senderExcelInfo.postId = mailerService.getPost();

            $scope.provinceExcelChange("district");
        }
    };

    $scope.provinceExcelChange = function (pType) {

        var url = '/mailerinit/GetProvinces?';

        if (pType === "district") {
            url = url + "parentId=" + $scope.senderExcelInfo.senderProvince + "&type=district";
        }

        if (pType === "ward") {
            url = url + "parentId=" + $scope.senderExcelInfo.senderDistrict + "&type=ward";
        }

        $http.get(url).then(function (response) {

            if (pType === "district") {
                $scope.districtSend = angular.copy(response.data);
            }

            if (pType === "ward") {
                $scope.wardSend = angular.copy(response.data);
            }

        });
    };

    $scope.calPrice = function (index) {
        var info = $scope.mailers[index];
        $http({
            method: "POST",
            url: "/mailer/calbillprice",
            data: {
                'weight': info.Weight,
                'customerId': info.SenderID,
                'provinceId': info.RecieverProvinceID,
                'serviceTypeId': info.MailerTypeID,
                'postId': mailerService.getPost(),
                'cod': info.COD,
                'merchandiseValue': info.MerchandiseValue
            }
        }).then(function mySuccess(response) {

            $scope.mailers[index].PriceDefault = response.data.price;
            $scope.mailers[index].PriceCoD = response.data.codPrice;

            var total = 0;
            // $scope.mailer.Services = [];
            for (var i = 0; i < $scope.mailers[index].Services.length; i++) {
                if ($scope.mailers[index].Services[i].choose) {
                    if ($scope.mailers[index].Services[i].percent) {
                        total = total + ($scope.mailers[index].Services[i].price * $scope.mailers[index].PriceDefault) / 100;
                    } else {
                        total = total + $scope.mailers[index].Services[i].price;
                    }

                    //  $scope.mailer.Services.push($scope.otherServices[i]);
                }
            }

            $scope.mailers[index].PriceService = total;

            $scope.mailers[index].Amount = $scope.mailers[index].PriceDefault + $scope.mailers[index].PriceCoD + $scope.mailers[index].PriceService;

        }, function myError(response) {
            alert('Connect error');
        });

    };

    

    $scope.setMerchandiseValue = function (index) {
        $scope.mailers[index].MerchandiseValue = $scope.mailers[index].COD;
    };

    $scope.changePrice = function (index) {
        $scope.mailers[index].Amount = $scope.mailers[index].PriceDefault + $scope.mailers[index].PriceCoD + $scope.mailers[index].PriceService;
    };
    // upload file
    $scope.files = [];
    var inserByExcelElement = document.getElementById('insertByExcel');
    inserByExcelElement.addEventListener('change', function (e) {
        uiUploader.removeAll();
        var files = e.target.files;
        uiUploader.addFiles(files);
        $scope.files = uiUploader.getFiles();
        $scope.$apply();
    });

    $scope.insertByExcel = function () {

        if ($scope.senderExcelInfo.senderProvince === null || $scope.senderExcelInfo.senderDistrict === null || $scope.senderExcelInfo.senderWard === null) {
            alert('Thiếu thông tin tỉnh thành');
        } else {
            showLoader(true);
            uiUploader.startUpload({
                url: '/mailerinit/insertbyexcel',
                concurrency: 2,
                paramName: 'files',
                data: $scope.senderExcelInfo,
                onProgress: function (file) {
                    $scope.$apply();
                },
                onCompleted: function (file, response) {

                    showLoader(false);
                    var result = angular.fromJson(response);
                    console.log(result);
                    uiUploader.removeAll();
                    inserByExcelElement.value = "";
                    if (result.error === 1) {
                        alert(result.msg);
                    } else {
                       
                        for (var i = 0; i < result.data.length; i++) {

                            var item = result.data[i];
                            item.ListProvinceSend = provinceSendGet;
                            item.ListProvinceRecive = provinceSendGet;
                            item.Services = [];
                            mailerService.addMailer(item);
                        }
                        hideModel('insertbyexcel');
                        $scope.$apply();
                    }
                }
            });
        }


    };

});

// controller detail --> create, edit mailer
app.controller('ctrlAddDetail', function ($scope, $rootScope, $http, mailerService) {

    $scope.select2Options = {
        width: '100%'
        
    };

    $scope.customers = mailerService.getCustomers();
    $scope.mailerTypes = mailerService.getMailerTypes();
    $scope.payments = mailerService.getPayments();
    $scope.otherServices = angular.copy(servicesGet);
    $scope.merchandises = mailerService.getMerchandises();
    $scope.mailer = {};
    $scope.actionEdit = false;

    $scope.$on('insert-started', function (event, args) {
        $scope.customers = mailerService.getCustomers();
        $scope.mailer = angular.copy(args.mailer);
        console.log($scope.mailer);
      // $scope.otherServices = angular.copy(servicesGet);
        $scope.actionEdit = args.actionEdit;

        /*
        for (var i = 0; i < $scope.mailer.Services.length; i++) {
            for (var j = 0; j < $scope.otherServices.length; j++) {

                if ($scope.mailer.Services[i].code === $scope.otherServices[j].code) {
                    $scope.otherServices[j].choose = true;
                    $scope.otherServices[j].price = $scope.mailer.Services[i].price;
                }

            }
        }*/
    });


    $scope.actionEdit = mailerService.getActionEdit();

    $scope.addSeviceMorePrice = function () {
        var total = 0;
       // $scope.mailer.Services = [];
        for (var i = 0; i < $scope.mailer.Services.length; i++) {
            if ($scope.mailer.Services[i].choose) {
                if ($scope.mailer.Services[i].percent) {
                    total = total + ($scope.mailer.Services[i].price * $scope.mailer.PriceDefault)/100;
                } else {
                    total = total + $scope.mailer.Services[i].price;
                }
                
              //  $scope.mailer.Services.push($scope.otherServices[i]);
            }
        }
        console.log(total);
        $scope.mailer.PriceService = total;
        $scope.changePrice();
    };


    $scope.getAddressDetailInfo = function (url, type, address) {

        $http.get(url).then(function (response) {

            if (type === "send") {
                $scope.mailer.SenderAddress = address;
                $scope.mailer.SenderProvinceID = response.data.provinceId;
                $scope.mailer.SenderDistrictID = response.data.districtId;
                $scope.mailer.SenderWardID = response.data.wardId;
            } else {
                $scope.mailer.RecieverAddress = address;
                $scope.mailer.RecieverProvinceID = response.data.provinceId;
                $scope.mailer.RecieverDistrictID = response.data.districtId;
                $scope.mailer.RecieverWardID = response.data.wardId;
            }

            $scope.getDistrictAndWard(type);

        });
    };
    $scope.changeCus = function () {

        var cus = mailerService.findCustomer($scope.mailer.SenderID);
        console.log(cus);
        if (cus.code.indexOf('KHACHLE') === -1) {
            showNotify("Đang chọn: " + cus.name);
        } else {
            showNotify("Khách lẻ phải thu tiền và nhập địa chỉ");
        }
        if (cus) {
            $scope.mailer.SenderPhone = cus.phone;
            $scope.mailer.SenderProvinceID = cus.provinceId;
            $scope.mailer.SenderAddress = cus.address;
            $scope.mailer.SenderDistrictID = cus.districtId;
            $scope.mailer.SenderWardID = cus.wardId;
            $scope.mailer.SenderName = cus.name;

            $scope.getDistrictAndWard('send');

        }
    };

    $scope.getDistrictAndWard = function (type) {

        var url = '/mailerinit/GetDistrictAndWard?provinceId=';

        if (type === 'send') {
            url = url + $scope.mailer.SenderProvinceID + "&districtId=" + $scope.mailer.SenderDistrictID;
        } else {
            url = url + $scope.mailer.RecieverProvinceID + "&districtId=" + $scope.mailer.RecieverDistrictID;

            for (var i = 0; i < $scope.mailer.ListDistrictRecive.length; i++) {
                if ($scope.mailer.ListDistrictRecive[i].code === $scope.mailer.RecieverDistrictID) {
                    for (j = 0; i < $scope.mailer.Services.length; i++) {
                        if ($scope.mailer.Services[j].code === 'VSVX') {
                            $scope.mailer.Services[j].choose = $scope.mailer.ListDistrictRecive[i].vsvx;
                            if ($scope.mailer.ListDistrictRecive[i].vsvx)
                            {
                                $scope.mailer.Services.push($scope.otherServices[i]);
                            }
                        }
                    }
                }
            }

        }

        $http.get(url).then(function (response) {
            if (type === "send") {
                $scope.mailer.ListDistrictSend = angular.copy(response.data.districts);
                $scope.mailer.ListWardSend = angular.copy(response.data.wards);
            } else {
                $scope.mailer.ListDistrictRecive = angular.copy(response.data.districts);
                $scope.mailer.ListWardRecive = angular.copy(response.data.wards);
            }
        });
    };

    $scope.finishForm = function (isValid) {
        if ($scope.actionEdit) {
            $rootScope.$broadcast('result-edit-started', { mailer: $scope.mailer });
        } else {
            mailerService.addMailer($scope.mailer);
        }

        hideModel('insertmodal');
    };

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

                for (var i = 0; i < $scope.mailer.ListDistrictRecive.length; i++) {
                    if ($scope.mailer.ListDistrictRecive[i].code === $scope.mailer.RecieverDistrictID) {
                        for (j = 0; j < $scope.mailer.Services.length; j++) {
                            if ($scope.mailer.Services[j].code === 'VSVX') {
                                $scope.mailer.Services[j].choose = $scope.mailer.ListDistrictRecive[i].vsvx;
                            }
                        }
                    }
                }
            }
        }

        $http.get(url).then(function (response) {

            if (type === 'send') {

                if (pType === "district") {
                    $scope.mailer.ListDistrictSend = angular.copy(response.data);
                }

                if (pType === "ward") {
                    $scope.mailer.ListWardSend = angular.copy(response.data);
                }

            } else {
                if (pType === "district") {
                    $scope.mailer.ListDistrictRecive = angular.copy(response.data);
                }

                if (pType === "ward") {
                    $scope.mailer.ListWardRecive = angular.copy(response.data);
                }
            }

        });

    };


    $scope.showTest = function () {
        console.log($scope.mailer);
        console.log(mailerService.getMailerCurrent());
    };

    $scope.getMailerCode = function () {

        showLoader(true);
        var url = "/MailerInit/GeneralCode?postId=" + mailerService.getPost();

        $http.get(url).then(function (response) {

            $scope.mailer.MailerID = response.data.code;
            showLoader(false);
        });

    };

    $scope.calPrice = function () {

        $http({
            method: "POST",
            url: "/mailer/CalBillPrice",
            data: {
                'weight': $scope.mailer.Weight,
                'customerId': $scope.mailer.SenderID,
                'provinceId': $scope.mailer.RecieverProvinceID,
                'serviceTypeId': $scope.mailer.MailerTypeID,
                'postId': mailerService.getPost(),
                'cod': $scope.mailer.COD,
                'merchandiseValue': $scope.mailer.MerchandiseValue
            }
        }).then(function mySuccess(response) {
            console.log(response.data);
            $scope.mailer.PriceDefault = response.data.price;
            $scope.mailer.PriceCoD = response.data.codPrice;
            $scope.addSeviceMorePrice();
            $scope.mailer.Amount = $scope.mailer.PriceDefault + $scope.mailer.PriceCoD + $scope.mailer.PriceService;

        }, function myError(response) {
            alert('Connect error');
        });
    };

    $scope.changePrice = function () {
        console.log('tinh gia tong');
        $scope.mailer.Amount = $scope.mailer.PriceDefault + $scope.mailer.PriceCoD + $scope.mailer.PriceService;
    };
    $scope.setMerchandiseValue = function () {
        $scope.mailer.MerchandiseValue = $scope.mailer.COD;
    };
});


var autocompleteSend;
var autocompleteRecei;

function fillInAddressSend() {

    var place = autocompleteSend.getPlace();

    var result = handleAutoCompleteAddress(place);

    var url = "/MailerInit/GetProvinceFromAddress?province=" + result.administrative_area_level_1 + "&district=" + result.administrative_area_level_2 + "&ward=" + result.sublocality_level_1;

    angular.element(document.getElementById('ctrlAddDetail')).scope().getAddressDetailInfo(url, 'send', place.formatted_address);

};

function fillInAddressRecei() {

    var place = autocompleteRecei.getPlace();
    var result = handleAutoCompleteAddress(place);

    var url = "/MailerInit/GetProvinceFromAddress?province=" + result.administrative_area_level_1 + "&district=" + result.administrative_area_level_2 + "&ward=" + result.sublocality_level_1;

    angular.element(document.getElementById('ctrlAddDetail')).scope().getAddressDetailInfo(url, 'recei', place.formatted_address);

};



function init() {
    autocompleteSend = new createAutoCompleteAddress('autocompleteSend', fillInAddressSend);
    autocompleteRecei = new createAutoCompleteAddress('autocompleteRecei', fillInAddressRecei);
};



