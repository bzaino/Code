
define(function (require, exports, module) {
    var $    = require('jquery'),
        SALT = require('salt');

    SALT.ForgotPasswordManager = {
        submitButton: null,
        errorField: null,
        emailField: null,
        formElement: null,
        successMsgSection: null,
        defaultSection: null,
        init: function (config) {
            this.formElement = $(config.formElement);
            this.submitButton = $(config.submitButton);
            this.errorField = $(config.errorField);
            this.emailField = $(config.emailField);
            this.successMsgSection = $(config.successMsgSection);
            this.defaultSection = $(config.defaultSection);
            $('#forgotPasswordForm, #forgotPasswordExpireForm').off('valid').one('valid', function () {
                SALT.ForgotPasswordManager.submitEmail();
            });
            SALT.ForgotPasswordManager.successMsgSection.hide();
        },
        submitEmail: function () {
            $('#defaultPasswordSection .loading').show();
            var data = SALT.ForgotPasswordManager.formElement.serialize();
            $.post(SALT.ForgotPasswordManager.formElement.attr('action'), data, SALT.ForgotPasswordManager.handleResponse, 'json');
            return false;

        },
        handleResponse: function (result, status) {
            $('#defaultPasswordSection .loading').hide();
            if (result.Success) {
                SALT.ForgotPasswordManager.showSuccessMsg();
            }
            else {
                SALT.ForgotPasswordManager.showErrorMsg(result.Message);
            }
        },
        showSuccessMsg: function () {
            SALT.trigger('open:customOverlay', 'resetPassAlertSuccess');
        },
        showErrorMsg: function (msg) {
            $('#defaultPasswordSection .loading').hide();
            SALT.ForgotPasswordManager.errorField.html(msg);
        }
    };

    return SALT.ForgotPasswordManager;
});
