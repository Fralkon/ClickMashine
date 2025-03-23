var login_box = document.querySelector('.login-box');
if (login_box != null) {
    document.querySelector('[name="username"]').value = '{0}';
    document.querySelector('[name="password"]').value = '{1}';
    document.querySelector('#button-login > span').click();
    STATUS_OK;
}
else STATUS_END;