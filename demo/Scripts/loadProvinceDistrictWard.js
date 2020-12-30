// File javascript để lấy dữ liệu

// Khai báo URL service của bạn ở đây
var baseService = "/Shop";
var provinceUrl = baseService + "/GetAllProvinceByCountryId";
var districtUrl = baseService + "/GetAllDistrictByProvinceId";
var wardUrl = baseService + "/GetAllWardByDistrictId";
$(document).ready(function () {
  
    $("#province").on('change', function () {
        var id = $(this)[0].value;
        if (id != undefined && id != '') {
            _getDistrict(id);
        }
    });
    $("#district").on('change', function () {
        var id = $(this)[0].value;
        if (id != undefined && id != '') {
            _getWard(id);
        }
    });
    $("#ward").on('change', function () {
        var provinceText = $("#province option:selected").text();
        var districtText = $("#district option:selected").text();
        var wardText = $("#ward option:selected").text();
    });
});
// truyền id của country vào
function _getProvince(id) {
    $.get(provinceUrl + "/" + id, function (data) {
        if (data != null && data != undefined && data.length) {
            var html = '';
            html += '<option value="">--Không chọn--</option>';
            $.each(data, function (key, item) {
                html += '<option value=' + item.Id + '>' + item.Name + '</option>';
            });
            $("#province").html(html);
        }
    });
}
// truyền id của province vào
function _getDistrict(id) {
    $.get(districtUrl + "/" + id, function (data) {
        if (data != null && data != undefined && data.length) {
            var html = '';
            html += '<option value="">--Không chọn--</option>';
            $.each(data, function (key, item) {
                html += '<option value=' + item.Id + '>' + item.Name + '</option>';
            });
            $("#district").html(html);
        }
    });
}
// truyền id của district vào
function _getWard(id) {
    $.get(wardUrl + "/" + id, function (data) {
        if (data != null && data != undefined && data.length) {
            var html = '';
            html += '<option value="">--Không chọn--</option>';
            $.each(data, function (key, item) {
                html += '<option value=' + item.Id + '>' + item.Name + '</option>';
            });
            $("#ward").html(html);
        }
    });
}