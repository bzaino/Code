define([
    'jquery',
    'salt',
    'backbone',
    'asa/ASAUtilities',
    'underscore',
    'jquery.serializeObject',
    'foundation5',
    'modules/overlays',
    'asa/ASAWebService',
    'jquery.cookie'
], function ($, SALT, Backbone, Utility, _) {

    function checkToDisableYOB() {
        var $yobDropdown = $('#profile-birth-year');
        if ($yobDropdown.val()) {
            $yobDropdown.attr('disabled', true);
            $yobDropdown.parent('.styled-select').addClass('disabled-select');
        }
    }

    var PersonalInformation = Backbone.View.extend({
        events: {
            'valid #personal-information-form': 'saveChanges',
            'change #personal-information-form': 'enableSubmit',
            'input #personal-information-form': 'enableSubmit'
        },
        initialize: function (options) {
            this.questions = options.Questions;
            this.render();
        },
        render: function () {
            var _this = this;
            var context = this.model.toJSON();
            context.QuestionsAnswers = this.questions;
            // a work around the DB returning a string of 5 spaces for zip codes. remove this check when SWD-5825 is fixed
            context.USPostalCode = context.USPostalCode ? context.USPostalCode.trim() : '';
            Utility.renderDustTemplate('Profile/PersonalInformation', context, function () {
                _this.$el.foundation().updatePolyfill();

                var boundDelete = _.bind(_this.deleteMember, _this);
                $('#invalid-age').on('click', boundDelete);
                checkToDisableYOB();
            }, this.el);
        },
        saveChanges: function (e) {
            var formData = $(e.currentTarget).serializeObject();
            var isValidAge = Utility.CheckAgeValidity(formData.YearOfBirth);
            if (!isValidAge) {
                $('.js-confirm-yob').text(formData.YearOfBirth);
                SALT.openOverlay('#profile-age-overlay');
                return false;
            }
            this.$el.find('.js-profile-saved-overlay').show();
            this.$el.find('.js-profile-saved-img').show();
            checkToDisableYOB();
            SALT.trigger('Profile:Updated', formData, this.$el);
        },
        enableSubmit: function (e) {
            SALT.trigger('Form:Changed', e);
        },
        deleteMember: function () {
            var _this = this;

            SALT.services.DeleteMember(this.model, function () {
                SALT.openOverlay('#deleting-account-overlay-closing');
                setTimeout(function () {
                    SALT.openOverlay('#deleting-account-overlay-closed');
                    setTimeout(function () {
                        SALT.services.LogOff(function () {
                            //We delete all salt made cookies on logoff, so the TooYoung cookie needs to be set in the callback function
                            $.cookie('TooYoung', 'true', { path: '/' });
                            window.location.href = '/';
                        }, _this);
                    }, 2500);
                }, 2500);
            }, this);

        }
    });
    return PersonalInformation;
});
