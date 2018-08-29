define([
    'jquery',
    'salt',
    'backbone',
    'underscore',
    'asa/ASAUtilities',
    'modules/organizationDropdown'
], function ($, SALT, Backbone, _, Utility, OrganizationDropdown) {

    var OrganizationInformation = Backbone.View.extend({
        events: {
            'valid #school-information-form': 'saveChanges',
            'change #school-information-form': 'enableSubmit',
            'input #school-information-form': 'enableSubmit',
            'change .yearDropdown': 'updateCurrentSchoolGradYear',
            'click .js-school-add-button': 'addSchool',
            'click .js-organization-add-button': 'addOrganization',
            'click .js-remove-affiliation': 'removeAffiliation',
            'change input:radio[name=affiliationRadioBtn]': 'selectAffiliation'
        },
        initialize: function (options) {
            // Wait for the completion of the Profile/OrganizationInformation dust template to load.
            SALT.on('renderReportingIdRow', this.renderReportingIdRow, this);

            this.questions = options.Questions;
            this.render();
        },
        selectAffiliation: function (e) {
            var value = $(e.currentTarget).val();
            if (value === "School") {
                this.addSchool();
            } else {
                this.addOrganization();
            }
            $('.affiliationQuestion').addClass('hidden');
            $('.js-affiliation-add-buttons').removeClass('hidden');
        },
        addAffiliation: function (addDiv) {
            var affiliationAddButtons = $('.js-affiliation-add-buttons');          
            addDiv.insertBefore(affiliationAddButtons);
            $('form').foundation('abide');
            OrganizationDropdown.init();
            $('.org-affiliations .js-newAffiliation .js-organization-search .organizationFilter').last().focus();
        },
        addSchool: function () {
            var $schoolAddDiv = $('.schoolAddTemplate').children(':first').clone();            
            this.addAffiliation($schoolAddDiv);
        },
        addOrganization: function () {
            var $organizationAddDiv = $('.organizationAddTemplate').children(':first').clone();          
            this.addAffiliation($organizationAddDiv);
        },
        removeAffiliation: function (e) {
            e.preventDefault();
            var parentOrgTypeClasses = $(e.currentTarget).closest('.js-organization').attr('class').split(' ');
            var currentOrganizationId = $(e.currentTarget).closest('.js-organization').find('.OrganizationId').val();
            //Begin Fix for SWD-8618 //adjust validation result index before removing the element.
            var orgToRemove = $(e.currentTarget).closest('.js-organization').find('.organizationFilter');
            OrganizationDropdown.updateValidationIndex(orgToRemove);            
            //End Fix fr SWD-8618
            //when removing an org, by default set focus on the "Add School button"
            var shouldSetFocusAddSchoolButton = true;
            $(e.currentTarget).closest('.js-organization').remove();
            if ($('.js-organization-search').length === 2) {
                $('input:radio[name=affiliationRadioBtn]').prop('checked', false);
                $('.affiliationQuestion').removeClass('hidden');
                $('.js-affiliation-add-buttons').addClass('hidden');
                //when there are no orgs left, don't set focus on "Add School" button
                shouldSetFocusAddSchoolButton = false;
            }
            if ($('.js-organization-search').length === 11) {
                $('.js-affiliation-add-buttons').removeClass('hidden');
            }
            if (parentOrgTypeClasses.indexOf('js-newAffiliation') > -1) {
                OrganizationDropdown.removeOrganization(currentOrganizationId);
            } else if (parentOrgTypeClasses.indexOf('js-existingAffiliation') > -1) {
                OrganizationDropdown.organizations.forEach(function (org) {
                    if (parseInt(org.OrganizationId, 10) === parseInt(currentOrganizationId, 10)) {
                        org.IsOrganizationDeleted = true;
                    } 
                });
                this.toggleReportingIdDisplay(currentOrganizationId);
                this.enableSubmit(e);
            }
            //fix for SWD-8258
            $('.autocomplete-suggestions.organization:visible').empty().hide();
            this.setFocusAddSchoolButton(shouldSetFocusAddSchoolButton);
        },
        setFocusAddSchoolButton: function (shouldSetFocusAddSchoolButton) {
            if (shouldSetFocusAddSchoolButton) {
                $('.row.js-affiliation-add-buttons .js-school-add-button').focus();
            }
            else {
                $('#senderSchool').focus();
            }
        },
        toggleReportingIdDisplay: function (organizationId) {
            var toggleReportingIdRow = '.js-toggle' + organizationId;
            $('' + toggleReportingIdRow + '').hide();
            if ($('.js-profile-reporting-id-row').filter(function () {
                    return $(this).css('display') !== 'none';
                }).length === 0) {
                $('.js-profile-student-id').hide();
            }
        },
        updateCurrentSchoolGradYear: function (e) {
            e.preventDefault();
            $(e.currentTarget).trigger('change.fndtn.abide');
            var currentOrganizationId = $(e.currentTarget).closest('.js-organization').find('.OrganizationId').val();
            OrganizationDropdown.organizations.forEach(function (org) {
                if (parseInt(org.OrganizationId, 10) === parseInt(currentOrganizationId, 10)) {
                    org.ExpectedGraduationYear = $(e.currentTarget).val();
                }
            });
        },
        render: function () {
            var _this = this;
            var context = this.prepareData();
            Utility.renderDustTemplate('Profile/OrganizationInformation', context, function () {
                _this.$el.foundation().updatePolyfill();
                if ((context.Organizations.length === 1) && 
                    (context.Organizations[0].OrganizationName === 'No School Selected' || 
                        context.Organizations[0].OrganizationName === 'No School Confirmed')) {
                    $('.js-existingAffiliation').remove();
                } else {
                    $('.affiliationQuestion').addClass('hidden');
                    $('.js-affiliation-add-buttons').removeClass('hidden');
                }
                OrganizationDropdown.init();
                OrganizationDropdown.organizations = context.Organizations;
                SALT.trigger('renderReportingIdRow', context);
            }, this.el);
        },
        prepareData: function () {
            var context = this.model.toJSON();
            context.QuestionsAnswers = this.questions;
            context.HideOption = context.EnrollmentStatus ? 'hide' : '';
            context[context.EnrollmentStatus + '_Selected'] = 'selected';
            return context;
        },
        selectCurrentOrganization: function (data) {
            var currentOrganization = _.findWhere(data.Organizations, { IsPrimary: true });
            currentOrganization = currentOrganization ? currentOrganization.OrganizationName : '';
            data.GraduationDate = currentOrganization ? currentOrganization.ExpectedGraduationYear : '';
            return currentOrganization;
        },
        selectExpectedGraduationYear: function (data) {
            var currentOrganization = _.findWhere(data.Organizations, { IsPrimary: true });
            var expectedGraduationYear = currentOrganization ? currentOrganization.ExpectedGraduationYear : '';
            return expectedGraduationYear;
        },
        saveChanges: function (e) {
            this.$el.find('.js-profile-saved-overlay').show();
            this.$el.find('.js-profile-saved-img').show();
            var formData = {};
            formData = $(e.currentTarget).serializeObject();
            this.saveReportingID(OrganizationDropdown.organizations);
            //If all orgs removed add the default one
            if ($('.js-organization-search').length === 2) {
                OrganizationDropdown.AddDefaultOrganization();
            }
            this.deactivateDefaultAffiliation(OrganizationDropdown.organizations);
            SALT.trigger('Profile:Updated', formData, this.$el);
        },
        deactivateDefaultAffiliation: function (organizations) {
            if ((organizations.length > 1) && 
                (organizations[0].OrganizationName === 'No School Selected' ||
                    organizations[0].OrganizationName === 'No School Confirmed')) {
                organizations[0].IsOrganizationDeleted = true;
            }
            this.model.set('Organizations', organizations);
        },
        renderReportingIdRow: function (context) {
            _.each(context.Organizations, function (organization) {
                //check if organization participates if reportingId product
                var removeOrganizationId = '';
                var organizationReportingIDProduct = _.findWhere(context.OrganizationProducts, {OrganizationId: organization.OrganizationId, ProductID: 1, IsOrgProductActive: true});
                if (organizationReportingIDProduct === undefined) {
                    //remove row doesn't participate
                    var toggleReportingIdRow = '.js-toggle' + organization.OrganizationId;
                    $('' + toggleReportingIdRow + '').remove();
                }
            });
            if ($('.js-profile-reporting-id').length > 0) {
                $('.js-profile-student-id').show();
            }
        },
        saveReportingID: function (organizations) {
            _.each($('.js-profile-organization-id'), function (element, ind, arr) {
                var organizationId = parseInt($('.js-profile-organization-id')[ind].value, 10);
                var reportingId = $('.js-profile-reporting-id')[ind].value;
                _.findWhere(organizations, {'OrganizationId': organizationId}).ReportingId = reportingId;
            });
        },
        enableSubmit: function (e) {
            SALT.trigger('Form:Changed', e);
        }
    });
    
    return OrganizationInformation;
});
