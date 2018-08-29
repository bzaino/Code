define(function (require, exports, module) {
    var $    = require('jquery'),
        SALT = require('salt');

    require('jquery.colorbox');

    SALT.NewPasswordManager = {

        newPasswordField: null,
        passwordConfirmField: null,
        submitButton: null,
        successMsgSection: null,
        defaultSection: null,
        formElement: null,
        errorField: null,
        init: function (config) {

            this.formElement = $(config.formElement);
            this.errorField = $(config.errorField);
            this.successMsgSection = $(config.successMsgSection);
            this.defaultSection = $(config.defaultSection);
            SALT.NewPasswordManager.successMsgSection.hide();
            $('#resetPasswordForm').one('valid', function () {
                SALT.NewPasswordManager.submitPassword();
            });
            $('#reset-password-logon-button').one('click', function () {
                SALT.trigger('open:customOverlay', 'loginOverlay');
            });
        },
        submitPassword: function () {
            if ($('#resetPasswordOverlay').hasClass('reveal-modal')) {
                $('#defaultResetPasswordSection .loading').show();
            } else {
                $('#cboxLoadingOverlay').show();
                $('#cboxLoadingGraphic').show();
            }
            var data = SALT.NewPasswordManager.formElement.serialize();
            $.post(SALT.NewPasswordManager.formElement.attr('action'), data, SALT.NewPasswordManager.handleResponse, 'json');
            return false;
        },
        handleResponse: function (result) {
            if (result.Success) {
                SALT.NewPasswordManager.showSuccessMsg();
            }
            else {
                SALT.NewPasswordManager.setErrorMsg('We are sorry. We were unable to process your request. Please contact Customer Service for assistance.');
            }
            $('#cboxLoadingGraphic').hide();
            $('#cboxLoadingOverlay').hide();
            $.colorbox.resize({
                maxWidth: 870
            });
        },
        setErrorMsg: function (msg) {
            SALT.NewPasswordManager.errorField.html(msg);
        },
        showSuccessMsg: function () {
            SALT.trigger('open:customOverlay', 'resetPasswordSuccess');
        }
    };

    return SALT.NewPasswordManager;
});
