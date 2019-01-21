function showModel(id) {

    $('#' + id).modal('show');
}
function showModelFix(id) {
    $('#' + id).modal({
        backdrop: 'static',
        keyboard: false
    })
    $('#' + id).modal('show');
}
function hideModel(id) {
    $('#' + id).modal('hide');
}


function submitForm(id) {
    $("#loader").css("display", "block");
    $('#' + id).submit();
}

function showLoader(isShow) {

    if (isShow) {
        $("#loader").css("display", "block");
    } else {
        $("#loader").css("display", "none");
    }
}

function fsubmit(msg) {
    if (!confirm(msg)) {
       
    } else {
        showLoader(true);
    }
}

function fsubmit(msg, id) {
    if (!confirm('Is the form filled out correctly?')) { return false; }
    if (!confirm(msg)) {
   
    } else {
        showLoader(true);
        hideModel(id);
    }
}


// auto complete address

function createAutoCompleteAddress(id, fillInAddress) {
    var catchID = document.getElementById(id);
    // Create the autocomplete object, restricting the search to geographical
    // location types.
    var autocomplete = new google.maps.places.Autocomplete(catchID,
        { types: ['geocode'] });
    autocomplete.setComponentRestrictions(
           { 'country': ['vn'] });
    // When the user selects an address from the dropdown, populate the address
    // fields in the form.
    autocomplete.addListener('place_changed', fillInAddress);
    return autocomplete;
}


function handleAutoCompleteAddress(place) {
            /*
        var componentForm = {
            street_number: 'short_name',
            route: 'long_name',
            locality: 'long_name',
            administrative_area_level_1: 'short_name',
            country: 'short_name',
            postal_code: 'short_name',
            administrative_area_level_2: 'long_name'
        };
        */

    //var place = autocomplete.getPlace();

    var result = {
        street_number: '',
        route: '',
        political: '',
        administrative_area_level_1: '',
        country: '',
        postal_code: '',
        administrative_area_level_2: ''
    };

    for (var i = 0; i < place.address_components.length; i++) {
        var addressType = place.address_components[i].types[0];

        switch (addressType) {
            case 'street_number':
                result.street_number = place.address_components[i]['short_name'];
                break;
            case 'route':
                result.route = place.address_components[i]['long_name'];
                break;
            case 'political':
                result.political = place.address_components[i]['long_name'];
                break;
            case 'sublocality_level_1':
                result.sublocality_level_1 = place.address_components[i]['long_name'];
                break;
            case 'administrative_area_level_1':
                result.administrative_area_level_1 = place.address_components[i]['short_name'];
                break;
            case 'country':
                result.country = place.address_components[i]['short_name'];
                break;
            case 'postal_code':
                result.postal_code = place.address_components[i]['short_name'];
                break;
            case 'administrative_area_level_2':
                result.administrative_area_level_2 = place.address_components[i]['long_name'];
                break;
        }
    }

    return result;
}


// show notify
function showNotify(title, mesenger) {

    $.notify({
        title: title,
        message: mesenger
    }, {
        type: 'success'
    });

}

function showNotify( mesenger) {

    $.notify(mesenger, {
        type: 'success'
    });

}

function showNotifyWarm(mesenger) {

    $.notify(mesenger, {
        type: 'warning'
    });

}

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

//
var mailerStatusData = [{ "code": 0, "name": "KHỞI TẠO" }, { "code": 1, "name": "ĐANG GỬI LIÊN TUYÊN" }, { "code": 2, "name": "ĐÃ NHẬN" }, { "code": 3, "name": "ĐANG PHÁT" }, { "code": 4, "name": "ĐÃ PHÁT" }, { "code": 5, "name": "CHUYỂN HOÀN" }, { "code": 6, "name": "CHƯA PHÁT ĐƯỢC" }, { "code": 7, "name": "ĐANG ĐI LẤY HÀNG" }, { "code": 8, "name": "ĐÃ LẤY HÀNG" }, { "code": 9, "name": "GIAO ĐỐI TÁC PHÁT" }, { "code": 10, "name": "HỦY ĐƠN" }, { "code": 11, "name": "ĐÃ HOÀN" }];
var deliveryStatusData = [
    {
        "code": "0",
        "name": "KHỞI TẠO"
    },
    {
        "code": "1",
        "name": "ĐANG PHÁT"
    },
    {
        "code": "2",
        "name": "CHƯA PHÁT XONG"
    },
    {
        "code": "3",
        "name": "ĐÃ PHÁT"
    }
];

