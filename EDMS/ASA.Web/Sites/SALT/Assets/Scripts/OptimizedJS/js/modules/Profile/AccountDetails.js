define([
    'jquery',
    'salt',
    'backbone',
    'underscore',
    'asa/ASAUtilities',
    'require',
    'jquery.serializeObject'
], function ($, SALT, Backbone, _, Utility, req) {

    var AccountDetails = Backbone.View.extend({
        events: {
            'valid #account-details-form': 'saveChanges',
            'click .js-close-account': 'openDeactivateAccountOverlay',
            'change #account-details-form': 'enableSubmit',
            'input #account-details-form': 'enableSubmit',
            'click #edit-email': 'editEmail',
            'click #edit-password': 'editPassword'
        },
        initialize: function (options) {
            var _this = this;
            this.questions = options.Questions;
            this.render();
            SALT.on('changePasswordSuccess', function ($newPasswordField) {
                _this.cancelPassword($newPasswordField);
            });
            //Foundation reveal appends the modal code to body towards the end
            //and thus it is out of the view's scope
            //TODO: Take out binding of click events out of init
            $(document.body).on('click', '#deactivate-submit', function (e) {
                e.preventDefault();
                _this.deactivateSaltAccount();
            });
            $(document.body).on('click', '#deactivate-cancel', function (e) {
                e.preventDefault();
                _this.cancelDeactivation();
            });
        },
        render: function () {
            var _this = this;
            var context = this.prepareData();
            Utility.renderDustTemplate('Profile/AccountDetails', context, function () {
                _this.$el.foundation().updatePolyfill();
                require(['salt/pschecker']);
            }, this.el);
        },
        prepareData: function () {
            var context = this.model.toJSON();
            // special case: the backend expects ContactFrequency = false if user wants to keep receiving emails!
            if (!context.ContactFrequency) {
                context.ContactFrequencyCheck = 'checked';
            }
            context.QuestionsAnswers = this.questions;
            context.CurrentEmail = this.selectCurrentEmail(context);
            return context;
        },
        saveChanges: function (e) {
            var formData = $(e.currentTarget).serializeObject();
            var emailEnabled = !$('#profile-email').prop('disabled');
            if (!emailEnabled) {
                // Email field was disabled, add the email from the model we already have
                formData.EmailAddress = this.selectCurrentEmail(this.model.toJSON());
            } else if (!this.updateEmailAllowed(formData)) {
                return;
            }
            //now show the spinner
            this.$el.find('.js-profile-saved-overlay').show();
            this.$el.find('.js-profile-saved-img').show();
            this.setNewEmail(formData);
            // special case: the backend expects ContactFrequency = false if user wants to keep receiving emails!
            if ($('#myContactPref').is(':checked')) {
                formData.ContactFrequency = false;
            } else {
                formData.ContactFrequency = true;
            }
            SALT.trigger('Profile:Updated', formData, this.$el);
        },
        selectCurrentEmail: function (data) {
            return _.findWhere(data.Emails, {IsPrimary: true}).EmailAddress;
        },
        updateEmailAllowed: function (formData) {
            //always put the email back to the starting email because if
            //change fails on the backend then the cuurent email is actually the new not old email.
            this.resetEmail(this.model.get('PrimaryEmailKey'));
            var currentEmail = this.selectCurrentEmail(this.model.toJSON());
            if (currentEmail === formData.EmailAddress) {
                //fade fail message
                this.$el.find('.js-profile-email-message').addClass('error').show();
                Utility.popMessage(this.$el.find('.js-profile-email-message'), 3000);
                return false;
            } else {
                return true;
            }
        },
        openDeactivateAccountOverlay: function (e) {
            e.preventDefault();
            //dcsMultiTrack('WT.z_actname','manageprofileclose', 'WT.z_acttype', 'complete','WT.z_acttx', '1', 'WT.dl', '90');
            SALT.trigger('deactivation:start');
            $('#close-account-modal').foundation('reveal', 'open');
        },
        cancelDeactivation: function () {
            SALT.trigger('deactivation:abandon');
            $('#close-account-modal').foundation('reveal', 'close');
        },
        deactivateSaltAccount: function () {
            $('#close-account-modal .js-profile-saved-img').show();
            $.post('/Account/DeactivateAccount', function (data) {
                $('#close-account-modal .js-profile-saved-img').hide();
                if (data.message) {
                    $('#DeactivateAccountbackEndError').html(data.Message);
                } else {
                    //success
                    SALT.trigger('deactivation:complete');
                    $('#close-account-confirmation-modal').foundation('reveal', 'open');
                    setTimeout(function () {
                        location.href = data.NavigateTo;
                    }, 2000);
                }
            });
        },
        setNewEmail: function (formData) {
            var emails = this.model.get('Emails');
            _.each(emails, function (email) {
                if (email.IsPrimary) {
                    email.EmailAddress = this.EmailAddress;
                }
            }, formData);
            this.model.set('Emails', emails);
        },
        resetEmail: function (emailAddress) {
            var emails = this.model.get('Emails');
            _.each(emails, function (email) {
                if (email.IsPrimary) {
                    email.EmailAddress = emailAddress;
                }
            }, emailAddress);
            this.model.set('Emails', emails);
        },
        editEmail: function (e) {
            e.preventDefault();
            var $confirmEmailField = $('#profile-confirm-email');
            if ($confirmEmailField.is(':visible')) {
                this.cancelEmail($confirmEmailField);
            } else {
                $('#edit-email').text('Cancel');
                $('#edit-password').text('');
                $('#email-field-label').text('New Email Address');
                $('#profile-email').removeAttr('disabled');
                $('#profile-old-password').val('').removeAttr('disabled');
                $confirmEmailField.show().attr('data-equalto', 'profile-email').removeAttr('disabled');
                $('.js-toggle-email').show();
            }
        },
        cancelEmail: function ($confirmEmailField) {
            $('#edit-email').text('Edit');
            $('#edit-password').text('Edit');
            $('#email-field-label').text('Email Address');
            $confirmEmailField.val('').hide().removeAttr('data-equalto').prop('disabled', true);
            $('.js-toggle-email').hide().find('input').val('').trigger('reset');
            $('#profile-email').val(this.selectCurrentEmail(this.model.toJSON())).prop('disabled', true);
            $('#profile-old-password').val('XXXXXXXX').prop('disabled', true);
        },
        editPassword: function (e) {
            e.preventDefault();
            var $newPasswordField = $('#profile-new-password');
            if ($newPasswordField.is(':visible')) {
                this.cancelPassword($newPasswordField);
            } else {
                $('#edit-password').text('Cancel');
                $('#edit-email').text('');
                $('#old-password-label').text('Current Password');
                $('#profile-old-password').val('').removeAttr('disabled');
                $newPasswordField.prop('required', true);
                $('.js-toggle-password').show();
            }
        },
        cancelPassword: function ($newPasswordField) {
            $('#edit-password').text('Edit');
            $('#edit-email').text('Edit');
            $('#old-password-label').text('Password');
            $('.js-toggle-password').hide().find('input').val('').trigger('reset');
            $('#profile-old-password').val('XXXXXXXX').prop('disabled', true);
            $newPasswordField.removeAttr('required');
        },
        enableSubmit: function (e) {
            SALT.trigger('Form:Changed', e);
        }
    });

    return AccountDetails;
});
