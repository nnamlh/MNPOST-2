var app = angular.module('myApp', ['ui.bootstrap', 'myDirective', 'ui.select2', 'ui.mask']);

app.controller('myCtrl', function ($scope, $http, $rootScope, $interval) {

    $scope.select2Options = {
        width: '100%'
    };
    $scope.tabds = true;
    $scope.mailers = [];

    $scope.dateimport = currentDate;

    $scope.customers = angular.copy(customerDatas);
    $scope.customers.unshift({
        name: 'Tất cả',
        code: ''
    });

    $scope.customerSearch = { SenderID: '' };

    $scope.sendTakeInfo = {};

    $scope.checkMailers = false;

    $scope.mailerId = '';

    $scope.checkAll = function () {
        for (var i = 0; i < $scope.mailers.length; i++) {

            if ($scope.customerSearch.SenderID === '') {
                $scope.mailers[i].isCheck = $scope.checkMailers;
            } else {

                if ($scope.mailers[i].SenderID === $scope.customerSearch.SenderID) {
                    $scope.mailers[i].isCheck = $scope.checkMailers;
                } else {
                    if ($scope.checkMailers) {
                        $scope.mailers[i].isCheck = !$scope.checkMailers;
                    }
                }
            }

        }
    };

    $scope.removeCheck = function () {
        $scope.checkMailers = false;
        for (var i = 0; i < $scope.mailers.length; i++) {

            $scope.mailers[i].isCheck = $scope.checkMailers;

        }
    };

    function getSelectedIndex(mailerId) {
        for (var i = 0; i < $scope.mailers.length; i++)
            if ($scope.mailers[i].MailerID === mailerId)
                return i;
        return -1;
    };

    $scope.getData = function () {
        var url = '/mailerimport/getdata?postId=' + $scope.postHandle;
        $http.get(url).then(function (response) {

            $scope.checkMailers = false;
            $scope.mailers = angular.copy(response.data.data);

        });

    };

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

    $scope.returnTake = function (mailerId, idx) {
        showLoader(true);
        $http({
            method: 'POST',
            url: '/mailerimport/CancelTake',
            data: {
                documentID: $scope.takeInfo.DocumentID,
                mailerId: mailerId
            }

        }).then(function sucess(response) {
            showLoader(false);
            if (response.data.error === 1) {
                alert(response.data.msg);
            } else {
                $scope.takeDetailDatas.splice(idx, 1);
                $scope.getData();
                $scope.sendGetTakeMailers();
            }
        });
    };

    $scope.sendImport = function () {


        var listSends = [];
        for (i = 0; i < $scope.mailers.length; i++) {
            if ($scope.mailers[i].isCheck) {
                listSends.push($scope.mailers[i].MailerID);
            }
        }

        if (listSends.length > 0) {

            showLoader(true);

            $http({
                method: 'POST',
                url: '/mailerimport/addMailers',
                data: {
                    mailers: listSends,
                    postId: $scope.postHandle
                }
            }).then(function sucess(response) {
                showLoader(false);

                $scope.getData();
                showNotify("Đã nhập kho");
            }, function error(response, error) {
                showNotify("connect has disconnect");
                showLoader(false);

            });

        }



    };

    $scope.cancelTake = function (documentId, idx) {
        $http({
            method: 'POST',
            url: '/mailerimport/CancelDocument',
            data: {
                documentID: documentId
            }
        }).then(function sucess(res) {

            var result = res.data;
            if (result.error === 1) {
                alert(result.msg);
            } else {
                $scope.takeMailerDatas.splice(idx, 1);
                $scope.takeDetailDatas = [];
            }

        });
    }

    $scope.cancelMailer = function () {
        showModel('cancelMailerModal');
    };

    $scope.findCheck = function () {
        for (i = 0; i < $scope.mailers.length; i++) {
            if ($scope.mailers[i].isCheck) {
                return true;
            }
        }

        return false;
    }

    $scope.finishCancalMailer = function () {

        var listSends = [];
        for (i = 0; i < $scope.mailers.length; i++) {
            if ($scope.mailers[i].isCheck) {
                listSends.push($scope.mailers[i].MailerID);
            }
        }
        showLoader(true);
        $http({
            method: "POST",
            url: "/MailerImport/CancelMailers",
            data: {
                mailers: listSends,
                reason: $scope.reasonCancel
            }

        }).then(function sucess(response) {
            showLoader(false);

            $scope.getData();

        }, function error() {
            showNotify("connect has disconnect");
            showLoader(false);

        });

    };

    $scope.addMailerImport = function (isValid) {

        if (isValid) {
            var findIndex = getSelectedIndex($scope.mailerId);

            if (findIndex === -1) {
                $scope.mailerId = '';
                showNotify("Mã này không có trong danh sách");
            } else {
                var listSends = [];
                listSends.push($scope.mailerId);
                $http({
                    method: 'POST',
                    url: '/mailerimport/addMailers',
                    data: {
                        mailers: listSends,
                        postId: $scope.postHandle
                    }
                }).then(function sucess(response) {
                    // $scope.mailers.shift(findIndex);
                    $scope.getData();
                    $scope.mailerId = '';
                }, function error(response, error) {
                    showNotify("connect has disconnect");
                });

            }



        }

    };
    $scope.status = angular.copy(mailerStatusData);
    $scope.init = function () {
        $scope.postOffices = postOfficeDatas;

        $scope.postHandle = '';

        if ($scope.postOffices.length === 1) {
            $scope.postHandle = $scope.postOffices[0];
            $scope.getData();
            $scope.sendGetEmployees();
            $scope.sendGetTakeMailers();
            $interval(function () { $scope.getData(); $scope.sendGetTakeMailers(); }, 1000 * 60);
        } else {
            showModelFix('choosePostOfficeModal');
        }

    };

    $scope.refeshData = function () {
        $scope.getData();
        $scope.sendGetEmployees();
        $scope.sendGetTakeMailers();
    };

    $scope.sendGetEmployees = function () {

        $http.get("/MailerImport/GetEmployee?postId=" + $scope.postHandle).then(function (response) {

            $scope.employees = response.data;
        });

    };
    $scope.isrungettake = false;
    $scope.sendGetTakeMailers = function () {
        $scope.isrungettake = true;
        $http.get("/MailerImport/GetTakeMailers?postId=" + $scope.postHandle + "&date=" + $scope.dateimport).then(function (response) {
            console.log(response.data);
            $scope.takeMailerDatas = response.data;
            $scope.isrungettake = false;
        });

    };
    $scope.choosePostHandle = function () {
        if ($scope.postHandle === '') {
            alert('Chọn bưu cục nếu không sẽ không thể thao tác');
        } else {

            $scope.sendGetEmployees();
            $scope.getData();
            $scope.sendGetTakeMailers();
            $interval(function () { $scope.getData(); $scope.sendGetTakeMailers(); }, 1000 * 60);
            hideModel('choosePostOfficeModal');
        }
    };

    $scope.init();




    function findCustomerIndex(code) {
        for (i = 0; i < $scope.customers.length; i++) {
            if ($scope.customers[i].code === code) {
                return i;
            }
        }

        return -1;
    }

    $scope.takeMailers = function () {

        if ($scope.customerSearch.SenderID === '') {
            showNotify('Chỉ giao đi lấy cho khách hàng cố định');
        } else {

            var listSends = [];
            for (i = 0; i < $scope.mailers.length; i++) {
                if ($scope.mailers[i].isCheck) {
                    listSends.push($scope.mailers[i].MailerID);
                }
            }

            if (listSends.length == 0) {
                showNotify('Chọn các đơn hàng đi lấy');
            } else {

                var idx = findCustomerIndex($scope.customerSearch.SenderID);

                $scope.sendTakeInfo = { code: $scope.customers[idx].code, name: $scope.customers[idx].name, address: $scope.customers[idx].address, phone: $scope.customers[idx].phone };
                showModel('takemailers');
            }


        }



    };

    $scope.sendTake = function (valid) {

        if (valid) {
            showLoader(true);
            var listSends = [];
            for (i = 0; i < $scope.mailers.length; i++) {
                if ($scope.mailers[i].isCheck) {
                    listSends.push($scope.mailers[i].MailerID);
                }
            }
            $http({

                method: 'POST',
                url: '/MailerImport/CreateTakeMailer',
                data: {

                    cusCode: $scope.sendTakeInfo.code,
                    cusName: $scope.sendTakeInfo.name,
                    cusAddress: $scope.sendTakeInfo.address,
                    cusPhone: $scope.sendTakeInfo.phone,
                    content: $scope.sendTakeInfo.content,
                    employeeId: $scope.sendTakeInfo.employeeId,
                    mailers: listSends,
                    postId: $scope.postHandle
                }
            }).then(function success(response) {
                showLoader(false);
                hideModel('takemailers');
                showNotify("Đã tạo xong");
                $scope.getData();
                $scope.sendGetTakeMailers();
                $('.nav-tabs a[href="#tab_layhang"]').tab('show');

            }, function error(response) {
                showLoader(false);
                showNotify("disconect internet")
            });

        } else {

            showNotify('missing form');
        }

    };

    $scope.checkMailerTake = false;
    $scope.checkTakeAll = function () {
        for (var i = 0; i < $scope.takeDetailDatas.length; i++) {
            $scope.takeDetailDatas[i].isCheck = $scope.checkMailerTake;
        }
    };

    $scope.showTakeDetail = function (documentID, idx) {
        $('.nav-tabs a[href="#tab_chitiet"]').tab('show');

        $scope.takeInfo = $scope.takeMailerDatas[idx];

        $http.get('/mailerimport/showtakedetail?documentID=' + documentID).then(function (response) {
            $scope.updateTake = true;
            $scope.takeDetailDatas = response.data;

        });


    };


    $scope.sendUpdateTake = function () {
        showLoader(true);
        var listSends = [];
        for (i = 0; i < $scope.takeDetailDatas.length; i++) {
            if ($scope.takeDetailDatas[i].isCheck) {
                listSends.push($scope.takeDetailDatas[i].MailerID);
            }
        }


        if (listSends.length === 0) {
            showNotify('Chọn vận đơn cần cập nhật');
        } else {
            $http({

                method: 'POST',
                url: '/mailerimport/updatetakedetails',
                data: {

                    documentID: $scope.takeInfo.DocumentID,
                    mailers: listSends
                }

            }).then(function success(response) {

                if (response.data.error === 0) {
                    $scope.getData();
                    $scope.sendGetTakeMailers();
                    $http.get('/mailerimport/showtakedetail?documentID=' + $scope.takeInfo.DocumentID).then(function (response) {

                        $scope.takeDetailDatas = response.data;

                    });
                } else {
                    showNotify(response.data.msg);
                }
                showLoader(false);
            }, function error(response) {
                showNotify('disconnected internet')
                showLoader(false);
            });

        }

    };

});