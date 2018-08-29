
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

            this.submitButton.click(this.submitEmail);
            SALT.ForgotPasswordManager.successMsgSection.hide();
        },
        submitEmail: function () {
            SALT.ForgotPasswordManager.showLoadingGif();
            if (!SALT.Utility.Email.validate(SALT.ForgotPasswordManager.emailField.val())) {
                SALT.ForgotPasswordManager.errorField.html(SALT.ErrorMessages.InvalidEmailAddress);
                SALT.ForgotPasswordManager.hideLoadingGif();
                return false;
            }
            var data = SALT.ForgotPasswordManager.formElement.serialize();
            $.post(SALT.ForgotPasswordManager.formElement.attr('action'), data, SALT.ForgotPasswordManager.handleResponse, 'json');
            $.colorbox.resize({
                maxWidth: 870
            });
            return false;

        },
        handleResponse: function (result, status) {
            SALT.ForgotPasswordManager.hideLoadingGif();
            if (result.Success) {
                SALT.ForgotPasswordManager.showSuccessMsg();
            }
            else {
                SALT.ForgotPasswordManager.showErrorMsg(result.Message);
            }
        },
        showSuccessMsg: function () {
            SALT.ForgotPasswordManager.defaultSection.hide();
            SALT.ForgotPasswordManager.successMsgSection.show();

        },
        showErrorMsg: function (msg) {
            SALT.ForgotPasswordManager.errorField.html(msg);
            $.colorbox.resize({
                maxWidth: 870
            });
        },
        // in Reveal overlays, we don't want to be hiding and showing the cboxloading gif
        // But we need to preserve the colorbox functions for use in Lessons overlays.
        showLoadingGif: function () {
            if ($('#forgotPasswordOverlay').hasClass('reveal-modal')) {
                $('#defaultPasswordSection .loading').show();
            } else {
                $('#cboxLoadingOverlay, #cboxLoadingGraphic').show(); //QC 4190
            }
        },
        hideLoadingGif: function () {
            if ($('#forgotPasswordOverlay').hasClass('reveal-modal')) {
                $('#defaultPasswordSection .loading').hide(); //QC 4190
            } else {
                $('#cboxLoadingOverlay, #cboxLoadingGraphic').hide();
            }
        }
    };

    return SALT.ForgotPasswordManager;
});