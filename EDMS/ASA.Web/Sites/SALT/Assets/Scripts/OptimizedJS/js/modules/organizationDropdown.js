define([
    'jquery',
    'salt',
    'underscore',
    'foundation5',
    'jquery.autocomplete'
], function ($, SALT, _) {

    var $organizationFilter,
        currentValue = '';

    // revalidates the field when you lose focus on it
    $(document.body).on('change', '.organizationFilter', function () {
        var $this = $(this);
        if ($this.val() !== currentValue) {
            var index = $organizationFilter.index(this);
            OrganizationDropdown.removeCurrentOrganization(this);
            SALT.trigger('InvalidOrg', index);
        }
        $this.attr('data-active', '');
    });

    // this is how Foundation Abide validates the org field 
    // check can be triggered programmatically with change.fndtn.abide
    $(document).foundation({
        abide: {
            validators: {
                organizationLookup: function (el, required, parent) {
                    var index = $organizationFilter.index(el);
                    if (OrganizationDropdown.organizationNameValidity[index] && el.value) {
                        return true;
                    }
                    if (!el.value && parent.closest('.hidden').length) {
                        return true;
                    }
                    return false;
                }
            }
        }
    });
    
    // record the current value of the input on keydown
    var recordValue = function (e) {
        var $orgField = $(e.currentTarget);
        // assign global variable for this keystroke
        currentValue = $orgField.val();
        if ($orgField.hasClass('SCHL')) {
            OrganizationAutoComplete.params.type = 'SCHL';
        } else if ($orgField.hasClass('WELL')) {
            OrganizationAutoComplete.params.type = 'WELL';
        }
    };

    // check the new value against the old and remove the organization if change has occured 
    var checkValue = function (e) {
        var thisVal = $(this).val(),
            oldVal = currentValue;
        if (thisVal !== oldVal) {
            var index = $organizationFilter.index(this);
            if (this.hasAttribute('data-active')) {
                OrganizationDropdown.removeCurrentOrganization(e);
                SALT.trigger('InvalidOrg', index);
            } else {
                // this happens in removeCurrentOrganization in the if
                OrganizationDropdown.organizationNameValidity[index] = false;
            }
            currentValue = thisVal;
        }
    };

    // listener for focusing on and typing inside of org/school fields
    $(document.body).on({
        'focus': recordValue,
        'keydown': recordValue,
        'keyup': checkValue
    }, '.organizationFilter'); 

    var OrganizationAutoComplete = {
            serviceUrl: '/api/SearchService/GetOrganizations',
            paramName: 'filter',
            params: { offset: 0, rows: 20, type: 'SCHL' }, //is for schools
            minChars: 2,
            deferRequestBy: 250,
            maxHeight: 250,
            containerClass: 'autocomplete-suggestions organization',
            transformResult: function (response) {
                var dataToTransform = typeof response === 'string' ? $.parseJSON(response) : response;
                //The plugin assumes data is formatted with an object containing an array called 'suggestions'
                var suggestionsContainer = { suggestions: [] };

                _.each(dataToTransform.Records, function (el, ind, arr) {
                    var schoolToAdd = {};
                    schoolToAdd.value = el.Fields[1].Value[0];
                    schoolToAdd.data = el.Fields;
                    var schoolToAddOECode = schoolToAdd.data[2].Value[0].substring(0, 6),
                    schoolToAddOEBranch = schoolToAdd.data[2].Value[0].substring(6),
                    schoolToAddOrganizationId = schoolToAdd.data[5].Value[0];
                    //filter out an already selected org
                    var exists = false;
                    OrganizationDropdown.organizations.forEach(function (org) {
                        if (parseInt(org.OrganizationId, 10) === parseInt(schoolToAddOrganizationId, 10) ||
                            parseInt(org.OECode, 10) === parseInt(schoolToAddOECode, 10) && parseInt(org.BranchCode, 10) === parseInt(schoolToAddOEBranch, 10)) {
                            exists = true;
                        }
                    });
                    if (!exists) {
                        suggestionsContainer.suggestions.push(schoolToAdd);
                    }
                });
                return suggestionsContainer;
            },
            // occurs when selecting from the autocomplete or by typing a matching option fully
            onSelect: function (value, data) {
                var $this = $(this),
                    $myParent = $this.parent();
                $this.attr('data-active', '');
                //clear previous values if changing selection
                var previousOrgId = $myParent.siblings('.OrganizationId').val();
                if (previousOrgId) {
                    OrganizationDropdown.removeOrganization(previousOrgId);
                }
                var index = $organizationFilter.index(this);
                OrganizationDropdown.organizationNameValidity[index] = true;
                $this.trigger('change.fndtn.abide');
                var selectedOECode = value.data[2].Value[0].substring(0, 6),
                    selectedOEBranch = value.data[2].Value[0].substring(6),
                    selectedOrganizationId = value.data[5].Value[0];
                // TODO get rid of these, save the information in data-attributes
                $myParent.siblings('.OECode').val(selectedOECode);
                $myParent.siblings('.OEBranch').val(selectedOEBranch);
                $myParent.siblings('.OrganizationId').val(selectedOrganizationId);

                //keep the gradYear if already set and populate it to the school
                var currentSchoolGradYear = $myParent.closest('.js-organization').find('.yearDropdown').val();
                var organization = {
                    OECode: selectedOECode,
                    BranchCode: selectedOEBranch,
                    OrganizationId: selectedOrganizationId,
                    ExpectedGraduationYear: currentSchoolGradYear ? currentSchoolGradYear: 1900
                };
                OrganizationDropdown.organizations.push(organization);
                SALT.trigger('ValidOrg', index);
            },
        };

    var OrganizationDropdown = {
        // TODO this array is really weird and should be replaced with a better (scalable) system
        organizationNameValidity: [true, true, true, true, true, true, true, true, true, true],
        organizations: [],
        init: function () {

            var orgTypeClass = 'SCHL';

            $('.affiliationTemplates').addClass('hidden');

            if ($('.js-organization-search').length > 11) {
                $('.js-affiliation-add-buttons').addClass('hidden');
            }
            //if we are prepopulating an organization, update the type param
            if ($('.js-organization.js-firstAffiliation.js-WELL').is(':visible')) {
                orgTypeClass = 'WELL';
                this.updateOrgParam(orgTypeClass);
            }
            $organizationFilter = $('.organizationFilter');
            $organizationFilter.autocomplete(OrganizationAutoComplete);
        },
        // Remove the organization associated with the event that led here
        removeCurrentOrganization: function (e) {
            var $inputField = $(e.currentTarget);
            if ($(e).is('input')) {
                $inputField = $(e);
            }
            var index = $organizationFilter.index($inputField);
            // set the value to false in the validity array
            OrganizationDropdown.organizationNameValidity[index] = false;
            // revalidate the form
            $inputField.trigger('change.fndtn.abide');
            var currentOrganizationId = $inputField.closest('.js-organization-search').find('.OrganizationId').val();
            if (currentOrganizationId) {
                // if we had saved an org already, remove it
                this.removeOrganization(currentOrganizationId); 
            }
        },
        updateOrgParam: function (value) {
            OrganizationAutoComplete.params.type = value;
        },
        updateAutocompleteSelection: function () {
            OrganizationAutoComplete.selection = 'refocused-org';
            $organizationFilter.autocomplete(OrganizationAutoComplete);
        },        
        removeOrganization: function (orgId) {
            OrganizationDropdown.organizations = OrganizationDropdown.organizations.filter(function (org) {
                return parseInt(org.OrganizationId, 10) !== parseInt(orgId, 10);
            });
        },
        updateValidationIndex: function (orgToRemove) {
            var OrgIndex = $organizationFilter.index(orgToRemove);
            while (OrgIndex + 1 && OrgIndex < 9) {
                OrganizationDropdown.organizationNameValidity[OrgIndex] = OrganizationDropdown.organizationNameValidity[OrgIndex + 1];
                OrgIndex++;
            }
        },
        AddDefaultOrganization: function () {
            var defaultOrganization = {
                OrganizationId : '',
                ExpectedGraduationYear : 1900,
                OECode : '000000',
                BranchCode : '01'
            };
            if (!this.isOrganizationInList(defaultOrganization)) {
                OrganizationDropdown.organizations.push(defaultOrganization);
            }
        },
        isOrganizationInList: function (orgToCheck) {
            var exists = false;
            OrganizationDropdown.organizations.forEach(function (org) {
                if (parseInt(org.OrganizationId, 10) === parseInt(orgToCheck.OrganizationId, 10) ||
                    parseInt(org.OECode, 10) === parseInt(orgToCheck.OECode, 10) && parseInt(org.BranchCode, 10) === parseInt(orgToCheck.BranchCode, 10)) {
                    exists = true;
                }
            });
            return exists;
        }
    };
    return OrganizationDropdown;
});
