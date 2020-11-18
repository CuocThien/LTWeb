
$('#confirm').keyup(function () {
    var password = $('#password').val();
    var re_password = $('#confirm').val();
    if (re_password != password) {
        $('#showError').html('Mật khẩu không trùng khớp');
        $('#showError').css('color', 'rgb(100, 230, 97)');
        return false;
    } else {
        $('#showError').html('');
        return true
    }
});