define([
    'jquery',
    'salt',
    'backbone',
    'salt/models/SiteMember',
    'asa/ASAUtilities'
], function ($, SALT, Backbone, SiteMember, Utility) {
    var profileQuestions = Backbone.View.extend({
        events: {
            'valid #questions-form': 'saveChanges'
        },
        initialize: function () {
            // hide wait spinner and messages
            $('.js-profile-saved-img').hide();
            SiteMember.done(function (member) {
                //We want to use "member" for checking IsAuthenticated, because we want to know what the server told us about the user's authentication
                //but for attribute 'ExpectedGraduationYear', we want to get it from SiteMember, because it could be changed on client side.
                if (member.IsAuthenticated === 'true') {
                    var organizations = SiteMember.get('Organizations');

                    if (!organizations || !organizations[0] || 
                        !organizations[0].ExpectedGraduationYear || organizations[0].ExpectedGraduationYear === 1900) {
                        $('#questions-form').show();
                    }
                }
            });
        },
        saveChanges: function (e) {
            var gradYear = $('#questions-form #selectGraduationYear').val();
            //set backbone view object which also triggers events
            this.setGraduationYear(gradYear, function (organizations) {
                SiteMember.set('Organizations', organizations);
            });
            // treating YOB as a string as a workaround a .NET bug with smallint type
            var yob = SiteMember.get('YearOfBirth');
            if (yob && typeof yob === 'number') {
                SiteMember.set('YearOfBirth', yob.toString());
            }
            var data = JSON.stringify(SiteMember.toJSON());
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Account/ManageAccount',
                data: data,
                dataType: 'JSON',
                success: function (data, textStatus, jqXHR) {
                    //hide question form 
                    $('#questions-form').hide();
                    //fade success message
                    Utility.popMessage($('.js-profile-saved-message'));
                    $('.js-profile-saved-overlay').fadeOut(2500);
                },
                error: function (data, textStatus, jqXHR) {
                     //fade fail message
                    Utility.popMessage($('.js-profile-failed-message'));
                    $('.js-profile-saved-overlay').fadeOut(2500);
                }
            });
        },
        setGraduationYear: function (gradYear, cbk) {
            //If no callback was passed set it to a no-op
            if (!_.isFunction(cbk)) {
                cbk = function () {};
            }
            var organizations = SiteMember.get('Organizations');
            //if array is empty initialize
            if (organizations.length === 0) {
                organizations = [{
                    SchoolId: '',
                    SchoolName: '',
                    BranchCode: '',
                    OECode: '',
                    OrganizationId: '',
                    ExpectedGraduationYear: null,
                    IsPrimary: true
                }];
            }
            _.each(organizations, function (organization) {
                if (organization.IsPrimary) {
                    organization.ExpectedGraduationYear = gradYear;
                }
            }, gradYear);
            cbk(organizations);
        }
    });
    return profileQuestions;
});
