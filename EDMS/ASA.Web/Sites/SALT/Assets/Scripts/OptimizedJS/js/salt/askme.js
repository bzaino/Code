define([
    'jquery',
    'salt',
    'salt/analytics/webtrends-config',
    'asa/ASAWebService',
    'jquery.serializeObject'
], function ($, SALT, WT) {

    SALT.askMe = {
        init: function () {
            SALT.askMe.validation();

            this.WT.init();
        },

        WT: {
            initialInputSteps: {
                'FirstName': 2,
                'LastName': 3,
                'EmailAddress': 4,
                'Subject': 5,
                'Question': 6
            },

            init: function () {
                $('#thanksMessage').hide();
                var targetFocus = $('#FirstName');
                var giveFocus = function () {
                    try {
                        targetFocus.focus();
                    } catch (e) {
                        // empty
                        // IE <= 8 throws an exception if focus can't be given.
                        // IE9 does not.
                    }
                    if (!$.browser.msie) {
                        if (!targetFocus.is(':focus')) {
                            setTimeout(giveFocus, 1);
                        }
                    }
                };

                giveFocus();

                var WTTags = WT.tags;

                function initialInputStepSet(inputStep) {
                    WT.initialInputChange('#' + i, function () {
                        dcsMultiTrack('WT.z_type', 'Tool Usage', WTTags.SCENARIO_ANALYSIS_NAME, 'justAskMe', WTTags.SCENARIO_ANALYSIS_STEP_NUM, inputStep, WTTags.ACTIVITY_NAME, '', WTTags.ACTIVITY_TYPE, '', WTTags.ACTIVITY_TRANSACTION, '', WTTags.SERVER_CALL_IDENTIFIER, WT.SERVER_CALL_IDENTIFIERS.SALT_CUSTOM_EVENT);
                    });

                }

                for (var i in this.initialInputSteps) {
                    var initialInputStep = this.initialInputSteps[i];
                    initialInputStepSet(initialInputStep);
                }
            }
        },

        callSuccess: function (data) {

            if (data.ErrorList.length > 0) {
                // to convert the errorlist object that contains the error code to readable strings.
                var str = '';
                for (var p in data.ErrorList[0]) {
                    if (data.ErrorList[0].hasOwnProperty(p)) {
                        str += p + '::' + data.ErrorList[0][p] + '\n';
                    }
                }
                return false;
            } else {
                $('#Subject').val('');
                $('#YourQuestion').val('');
                setTimeout(function () {
                    $('#thanksMessage').show();
                }, 1500);
            }
        },

        validation: function () {

            $('#askMeForm').one('valid', function (e) {
                // Give our submit button that cool animation
                $('button.submit').addClass('progress');
                var askMe = setTimeout(function () {
                    clearTimeout(askMe);

                    var formData = $(e.currentTarget).serializeObject();

                    SALT.services.AskMe(formData.FirstName, formData.LastName, formData.Subject, formData.EmailAddress, formData.YourQuestion, formData.MembershipId, SALT.askMe.callSuccess, function () {}, this, false, true);
                }, 2000);

                return false;

            });
        }
    };

    $(function () {
        SALT.askMe.init();
    });

    return SALT.askMe;
});
