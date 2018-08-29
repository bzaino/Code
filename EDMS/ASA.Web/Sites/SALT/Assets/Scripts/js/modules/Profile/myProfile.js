define([
    'jquery',
    'salt',
    'underscore',
    'modules/Profile/Views',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'modules/organizationDropdown',
    'asa/ASAWebService'
], function ($, SALT, _, Views, SiteMember, Utility, OrganizationDropdown) {

    function instantiateViews(questionsAnswers) {
        var personalInformationView = new Views.PersonalInformation({ el: '#personal-information-section', Questions: questionsAnswers, model: SiteMember }),
            financialInformationView = new Views.FinancialInformation({ el: '#financial-information-section', Questions: questionsAnswers, model: SiteMember }),
            organizationInformationView = new Views.OrganizationInformation({ el: '#school-information-section', Questions: questionsAnswers, model: SiteMember }),
            accountDetailsView = new Views.AccountDetails({ el: '#account-details-section', Questions: questionsAnswers, model: SiteMember });
    }

    SALT.on('set:ProfileQandAs', function (profQandA) {
        SiteMember.set('ProfileQAndAs', profQandA);
    });

    SiteMember.done(function (siteMember) {
        var questionsAnswers = Utility.parseQuestionsAnswers({ questions: siteMember.ProfileQAs, responses: _.last(siteMember.ProfileQAs).Responses });
        instantiateViews(questionsAnswers);
    });

    SALT.on('Profile:Updated', function (formData, view) {
        Utility.setMemberResponses(formData);
        SiteMember.set(formData);

        // treating YOB as a string as a workaround a .NET bug with smallint type
        var yob = SiteMember.get('YearOfBirth');
        if (yob && typeof yob === 'number') {
            SiteMember.set('YearOfBirth', yob.toString());
        }

        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: '/Account/ManageAccount',
            data: JSON.stringify(SiteMember.toJSON()),
            dataType: 'JSON',
            success: function (data, textStatus, jqXHR) {
                view.find('.js-profile-saved-img').hide();
                var $newPasswordField = $('#profile-new-password');
                if ($newPasswordField.is(':visible')) {
                    // Cleanup after update password
                    SiteMember.set('Password', '');
                    SiteMember.set('NewPassword', '');
                    SALT.trigger('changePasswordSuccess', $newPasswordField);
                }
                if (!data.ErrorList.length) {
                    //refresh OrganizationDropdown attributes
                    OrganizationDropdown.organizations = data.Organizations;
                    //update new affiliations in the UI to be treated as existing
                    $('#OrgAffiliations').find('.js-organization.js-newAffiliation').removeClass('js-newAffiliation').addClass('js-existingAffiliation');
                    //Update the usersegment in case orgs have changed
                    Utility.findUserSegment(data);
                    view.find('.js-member-backend-error').hide();
                    Utility.popMessage(view.find('.js-profile-saved-message'));
                    if (data.RedirectURL) {
                        window.location.replace(data.RedirectURL);
                    }
                } else {
                    Utility.popMessage(view.find('.js-profile-failed-message'));
                    view.find('.js-member-backend-error').addClass('error').show();
                }
                view.find('.js-profile-saved-overlay').fadeOut(2500);
                view.find('button.submit').attr('disabled', 'disabled');
            },
            error: function (data, textStatus, jqXHR) {
                view.find('.js-profile-saved-img').hide();
                Utility.popMessage(view.find('.js-profile-failed-message'));
                view.find('.js-member-backend-error').addClass('error').show();
                view.find('.js-profile-saved-overlay').fadeOut(2500);
            }
        });
    });

    SALT.on('Form:Changed', function (event) {
        $(event.delegateTarget).find('button.submit').removeAttr('disabled');
    });

    $(document.body).on('click', '.js-show-hide', function () {
        $(this).closest('header').next('article').slideToggle('slow');
    });
});
