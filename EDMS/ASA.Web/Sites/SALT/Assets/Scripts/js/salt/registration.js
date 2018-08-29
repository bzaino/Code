define(['jquery',
        'salt',
        'asa/ASAUtilities',
        'salt/analytics/webtrends-config',
        'modules/organizationDropdown',
        'configuration',
        'modules/ASALocalStore',
        'asa/ASAWebService',
        'jquery.serializeObject'
    ], function ($, SALT, Utility, WT, OrganizationDropdown, configuration, asaLocalStore) {
    $('#clearForm').click(function (e) {
        e.preventDefault();
        SALT.registration.clearForm();
    });
    SALT.registration = {
        querystringVars: null,
        orgIsValid: false, //used to determine if a value from autocomplete has been selected
        prepoppedOrg: false,
        $passwordField: $('#Password'),
        $jsShowHideTextField: $('.js-show-hide-text'),
        returnURLOverride: null,

        init: function (config) {
            this.querystringVars = SALT.registration.handleQueryString(window.location.href);
            this.returnURLOverride = config.returnURLOverride || this.returnURLOverride;

            prepoppedOrg = false;
            orgIsValid = false;
            SALT.registration.prePopulate();

            SALT.on('resetControlsToDefault', function () {
                SALT.registration.clearForm();
            });

            SALT.on('showRadioButtons', function () {
                SALT.registration.setFieldVisible('#rb-container');
            });

            OrganizationDropdown.init();

            $('input:checkbox').show();

            SALT.on('TooYoung', this.disableRegForm);

            //If the YOB in the form is <13 or they have a previously set cookie that they are too young, dont let the user register, show an error message, and disable the reg form
            if ($.cookie('TooYoung')) {
                this.disableRegForm();
                this.setErrorMsg('Looks like you\'re not quite old enough...');
            }

            $('.hide-for-small.medium-6.columns').remove();

            this.WT.init();
            setTimeout(SALT.registration.WTcheckboxRouter, 2500);
        },

        prePopulate: function () {
            var oe = this.querystringVars.oe,
                br = this.querystringVars.br,
                token = this.querystringVars.t,
                regSourceId = this.querystringVars.src,
                wtmcID = this.querystringVars['wt.mc_id'],
                $organizationFilter,
                $oeCode,
                $branchCode,
                $orgId,
                orgTypeClass = 'SCHL',
                refOrgId = '',
                extOrgId = this.querystringVars.orgid ? this.querystringVars.orgid : '';


            if (token) {
                $('#inviteToken').val(token);
            }
            else if ($.cookie('ActivationToken')) {
                $('#inviteToken').val($.cookie('ActivationToken'));
            }

            if (wtmcID) {
                //set expiration date to 30 days.
                asaLocalStore.setLocalStorage('WT.mc_id', wtmcID, 30);
            }

            //only prepopulate school if orgID or both OE and Branch are provided
            if ((oe && br) || extOrgId) {
                orgIsValid = true; //assume that autocomplete would be valid when pre-populating
                prepoppedOrg = true;
                SALT.registration.prePopulateOrgList(oe, br);

                SALT.on('prePopulateSchool', function () {
                    $.get(configuration.apiEndpointBases.MemberService + '/PreRegistration?oe=' + oe + '&br=' + br + '&extOrgId=' + extOrgId, function (returnedOrg) {
                        if (!returnedOrg.IsSchool) {
                            orgTypeClass = 'WELL';
                            OrganizationDropdown.updateOrgParam(orgTypeClass);
                            //Grad Year not required
                            $('.js-organization.js-firstAffiliation.js-SCHL').find('.yearDropdown').prop('required', false);
                            //uncheck school radio
                            $('#Organization').prop('checked', true);
                            $('#School').prop('checked', false);
                            $('.js-organization.js-firstAffiliation.js-SCHL').addClass('hidden');
                            $('.js-organization.js-firstAffiliation.js-WELL').removeClass('hidden');
                        }
                        $organizationFilter = $('.js-firstAffiliation.js-' + orgTypeClass + ' .organizationFilter');
                        $oeCode = $('.js-firstAffiliation.js-' + orgTypeClass + ' .OECode');
                        $branchCode = $('.js-firstAffiliation.js-' + orgTypeClass + ' .OEBranch');
                        $orgId = $('.js-firstAffiliation.js-' + orgTypeClass + ' .OrganizationId');

                        if (!$organizationFilter.val()) {
                            $organizationFilter.val(returnedOrg.OrgName);
                        }
                        if (!$oeCode.val()) {
                            $oeCode.val(oe);
                        }
                        if (!$branchCode.val()) {
                            $branchCode.val(br);
                        }
                        if (!$orgId.val()) {
                            $orgId.val(returnedOrg.OrgId);
                        }

                        if (regSourceId) {
                            $('#registration-source').val(regSourceId);
                        }

                        SALT.registration.prePopulateOrgList(oe, br);

                        $('.js-affiliation-add-buttons').removeClass('hidden');
                        $('#rb-container').addClass('hidden');
                    });
                });

            }
        },

        prePopulateOrgList: function (oe, br) {
            var $inputContainer = $('.js-organization:visible.js-firstAffiliation'),
                prepoppedOrganizationId = $inputContainer.find('.OrganizationId').val();
            if (prepoppedOrganizationId) {
                var organization = {
                    OECode: oe,
                    BranchCode: br,
                    OrganizationId: prepoppedOrganizationId,
                    ExpectedGraduationYear: 1900 //default value, updated on submission
                };
                // disable inputs because a real school/org has been populated
                $('#qa-sch-1, #qa-org-1').attr('disabled', '');
                //add prepopulated org to selected org list
                OrganizationDropdown.organizations.push(organization);
                var index = $inputContainer.index();
                SALT.trigger('ValidOrg', index);
            }
        },

        handleQueryString: function (href) {
            var vars = [],
            hash;

            var hashes = href.toLowerCase().slice(href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }

            return vars;
        },

        //leaving stepNumber because they are assigned in the scenario at WebTrends.
        //Don't want to reuse them unless scenario is redesigned.
        WT: {
            initialInputSteps: {
                'FirstName': 2,
                'LastName': 3,
                'EmailAddress': 4,
                'organizationFilter': 7,
                // 'noSchoolCheckBox': 8,
                'Password': 5,
                'myContactPref': 10,
                'AcceptTerms': 11,
                'ExpectedGraduationYear': 9,
                'YearOfBirth': 6
                //'submit' : 12
                //placeholder for stepNumber - they are fired on js click function down below
                // 'affiliationRadioBtnSchool': 13,
                // 'affiliationRadioBtnOrg': 14,
                // 'affiliationRadioBtnNeither': 15,
            },

            init: function () {

                function initialInputStepSet(inputStep) {
                    WT.initialInputChange('#' + i, function () {
                        SALT.publish('Registration:Steps', {
                            stepNumber: inputStep
                        });
                    });
                }

                for (var i in this.initialInputSteps) {
                    var initialInputStep = this.initialInputSteps[i];
                    initialInputStepSet(initialInputStep);
                }
            }
        },
        disableRegForm: function () {
            $('#regForm :input').attr('disabled', true);
        },
        WTcheckboxRouter: function () {
            SALT.registration.checkboxWT('#myContactPref', 10);
            SALT.registration.checkboxWT('#AcceptTerms', 11);
        },
        checkboxWT: function (checkBoxElem, stepNumber) {
            //fix issue which webtrends failing to fire on change event and some click events
            $(checkBoxElem).keydown(function (evt) {
                if (evt.which === 32) {
                    SALT.publish('Registration:Steps', {
                        stepNumber: stepNumber
                    });
                }
            });

            $(checkBoxElem).click(function () {
                SALT.publish('Registration:Steps', {
                    stepNumber: stepNumber
                });
            });
        },
        registerSuccessCallback: function (member) {
            var redirectUrl = Utility.getParameterByName('ReturnUrl');

            /* if we are in the main registration page, refer to previous page. If document.referrer is null,
            redirect user to homepage. This happens when user goes to registration from an email. if it's special event page that url contains
            '/landing/', refer to the current page.
            */
            if (!redirectUrl) {
                redirectUrl = SALT.registration.adjustRedirectUrl(redirectUrl);
            }
            //SWD-2574 QC-5984 Fix WT events not logging for iphone, ipad, android browser - registration is successful fire WebTrends event
            dcsMultiTrack('WT.z_type', 'Tool Usage',
                WT.tags.EXTERNAL_VISITOR_ID, member.MembershipId,
                WT.tags.DCS_AUTHENTICATED_USERNAME, member.MembershipId,
                WT.tags.SCENARIO_ANALYSIS_NAME, 'Member Registration',
                WT.tags.SCENARIO_ANALYSIS_STEP_NUM, 12,
                WT.tags.ACTIVITY_NAME, 'activation',
                WT.tags.ACTIVITY_TYPE, 'complete',
                WT.tags.ACTIVITY_TRANSACTION, '1',
                WT.tags.SCENARIO_ANALYSIS_CONVERSION_STEP, '1',
                WT.tags.REGISTERED_VISITOR, '1',
                WT.tags.CAMPAIGN_ID, asaLocalStore.getLocalStorage('WT.mc_id'),
                WT.tags.SERVER_CALL_IDENTIFIER, WT.SERVER_CALL_IDENTIFIERS.SALT_CUSTOM_EVENT);

            SALT.registration.handleCookies(member, redirectUrl);

            if (member) {
                Utility.findUserSegment(member);
            }


            SALT.registration.registrationRedirect(redirectUrl);

            Utility.handleRegWallWT('Wall_Signup_Complete');
            $.removeCookie('regWall');
        },
        handleCookies: function (member, redirectUrl) {
            if (member.IndividualId) {
                document.cookie = 'IndividualId=' + member.IndividualId + '; PATH=/; Domain=saltmoney.org;';
            }
            if ($.cookie('RememberMe')) {
                $.removeCookie('RememberMe', { path: '/', domain: 'saltmoney.org' });
                $.removeCookie('RememberMeEmail', { path: '/', domain: 'saltmoney.org' });
                $.removeCookie('RememberMeUrl', { path: '/', domain: 'saltmoney.org' });
            }
            if ($.cookie('ActivationToken')) {
                $.removeCookie('ActivationToken');
            }
            if (asaLocalStore.getLocalStorage('WT.mc_id')) {
                asaLocalStore.removeLocalStorage('WT.mc_id');
            }
            //remove any previously cached courses data
            if (asaLocalStore.getLocalStorage('coursesCompletion')) {
                asaLocalStore.removeLocalStorage('coursesCompletion');
            }
            //when document.referrer === '' && oeCodeFromQS !== '' && oeBranchFromQS !== '' when coming from a vanity URL. They should see welcome overlay
            if (redirectUrl === '/index.html' || redirectUrl === '/logon' || (!document.referrer && ((this.querystringVars.oe && this.querystringVars.br) || this.querystringVars.orgid))) {
               //set show welcome cookie so that welcome overlay will show on first visit to Auth HomePage
                document.cookie = 'ShowWelcome = true; path=/';
            }
            //If the user is dashboard enabled, set a cookie that is used by the front-end to render the new todo-tile style
            if (member.DashboardEnabled) {
                document.cookie = 'useTodoTileDesign = true; path=/';
            }
        },
        clearForm: function () {
            this.$passwordField.val('');
            this.passwordShowState();
            $('#regForm').trigger('reset');
            termBoxDisable(true);
            SALT.registration.toggleAffiliation('School');
        },
        passwordShowState: function () {
            this.$passwordField.attr('type', 'password');
            this.$jsShowHideTextField.text('Show');
        },
        passwordHideState: function () {
            this.$passwordField.attr('type', 'text');
            this.$jsShowHideTextField.text('Hide');
        },
        togglePassword: function () {
            if (this.$passwordField.attr('type') === 'password') {
                this.passwordHideState(this.$passwordField);
            } else {
                this.passwordShowState(this.$passwordField);
            }
        },
        toggleAffiliation: function (selectedAffiliation) {
            OrganizationDropdown.organizations = [];
            if (selectedAffiliation === 'School') {
                SALT.registration.showSchool();
                SALT.registration.removeSchool();
                SALT.registration.hideOrganization();
                SALT.registration.setFieldHidden('#noAffiliationInfo');
            }
            else if (selectedAffiliation === 'Organization') {
                SALT.registration.showOrganization();
                SALT.registration.removeOrganization();
                SALT.registration.hideSchool();
                SALT.registration.setFieldHidden('#noAffiliationInfo');
            }
            else if (selectedAffiliation === 'Neither') {
                SALT.registration.clearAllAffiliations();
            }
        },
        showSchool: function () {
            //The first school radio button click add an js-first-Affilation.js_SCH.
            SALT.registration.setFieldVisible('.js-organization.js-firstAffiliation.js-SCHL');
            SALT.registration.setFieldVisible('.js-select-aff-msg');
            $('.js-organization.js-firstAffiliation.js-SCHL').find('.yearDropdown').prop('required', true);
        },
        showOrganization: function () {
            //The first org radio button click add an js-first-Affilation.js_WELL.
            SALT.registration.setFieldVisible('.js-WELL.js-organization.js-firstAffiliation');
            SALT.registration.setFieldHidden('.js-WELL.js-organization.js-firstAffiliation .js-remove-affiliation');
            SALT.registration.setFieldVisible('.js-select-aff-msg');
        },
        showAddButtons: function () {
            if ($('.js-organization:visible').length === 1) {
                SALT.registration.setFieldVisible('.js-affiliation-add-buttons');
                if (prepoppedOrg) {
                    var $theseInputs =  $('.js-organization:visible').find('input[type=text], select');
                    //find the inputs in the parent container and loop through them to see if they are filled in
                    //if it has value then display message
                    $theseInputs.each(function (i) {
                        if ($(this).val() !== '') {
                            if ($(this).hasClass('organizationFilter')) {
                                SALT.registration.setFieldVisible('.js-pre-popped-msg');
                            }
                        }
                    });
                }
            }
        },
        hideSchool: function () {
            SALT.registration.setFieldHidden('.js-SCHL');
            $('.organizationFilter').val('');
            $('.yearDropdown').val('');
            $('.OrganizationId').val('');
            $('.OECode').val('');
            $('.OEBranch').val('');
            $('.js-organization.js-firstAffiliation.js-SCHL').find('.yearDropdown').prop('required', false);
            $('.js-organization.js-newAffiliation.js-SCHL').find('.yearDropdown').prop('required', false);
        },
        hideOrganization: function () {
            SALT.registration.setFieldHidden('.js-WELL');
            $('.organizationFilter').val('');
            $('.OrganizationId').val('');
            $('.OECode').val('');
            $('.OEBranch').val('');
        },
        clearAllAffiliations: function () {
            SALT.registration.hideSchool();
            SALT.registration.hideOrganization();
            SALT.registration.setFieldVisible('#noAffiliationInfo');
            SALT.registration.setFieldHidden('.js-affiliation-add-buttons');
        },
        addAffiliation: function ($addDiv) {
            var $affiliationAddButtons = $('.js-affiliation-add-buttons');
            //Add School or Add Org button clicks create a clone of a new-Affiliation.
            //This checks to see if there is already a visible org with no remove link showing (the default condition of firstAffiliation divs).
            //If so, add the remove link to the first org when you add a new org.
            if ($('.js-organization:visible').length === 1 && $('.js-remove-affiliation:visible').length === 0) {
                var $removeLink = $('.js-organization:visible').find('.js-remove-affiliation');
                SALT.registration.setFieldVisible($removeLink);
            }
            //insert the clone before the add Buttons to keep the order of placement stacked in order of user add
            $addDiv.insertBefore($affiliationAddButtons);
            SALT.registration.setFieldVisible($addDiv);
            $('form#regForm').foundation('abide');
            OrganizationDropdown.init();
            $('.js-newAffiliation .js-organization-search .organizationFilter').focus();
        },
        addSchool: function () {
            var $schoolAddDiv = $('.schoolAddTemplate').children(':first').clone();
            this.addAffiliation($schoolAddDiv);
            $schoolAddDiv.find('.yearDropdown').prop('required', true);
        },
        addOrganization: function () {
            var $organizationAddDiv = $('.organizationAddTemplate').children(':first').clone();
            this.addAffiliation($organizationAddDiv);
        },
        removeSchool: function () {
            SALT.registration.setFieldHidden('.js-organization.js-newAffiliation.js-SCHL');
            SALT.registration.setFieldVisible('.js-affiliation-add-buttons');
            $('.js-organization.js-newAffiliation.js-SCHL').find('.yearDropdown').prop('required', false);
        },
        removeOrganization: function () {
            SALT.registration.setFieldHidden('.js-organization.js-newAffiliation.js-WELL');
            SALT.registration.showAddButtons();
        },
        removeAffiliation: function (el) { //el is event.currentTarget
            var $containerToHide = $(el).closest('.js-organization'),
                currentOrganizationId = $containerToHide.find('.OrganizationId').val();
            OrganizationDropdown.removeOrganization(currentOrganizationId);

            $containerToHide.find('input').val('');
            if ($containerToHide.hasClass('js-SCHL')) {
                $containerToHide.find('.yearDropdown').val('').prop('required', false);
            }

            // reenable editable school/org field once we clear it (for prepopulated vanity urls)
            if (prepoppedOrg && ($containerToHide.find('#qa-sch-1, #qa-org-1').length)) {
                $('#qa-sch-1, #qa-org-1').attr('disabled', null);
            }

            SALT.registration.setFieldHidden($containerToHide);//this will hide firstAffiliations
            $containerToHide.find('.organizationFilter').attr('data-active', null); // for validation if reshown

            // all of the org fields left after this is hidden
            var $otherVisibleOrgs = $('.js-organization:visible'),
                orgsAreValid = true;

            // if deleting the last org field, reset
            if (!$otherVisibleOrgs.length) {
                SALT.registration.setToDefault();
                $('.autocomplete-suggestions.organization:visible').empty().hide();
                return false;
            }

            // determine if all other remaining org fields are valid (valid value and grad date if applicable)
            $otherVisibleOrgs.each(function (ind, el) {
                var $orgFieldContainer = $(el),
                    $orgField = $orgFieldContainer.find('.organizationFilter'),
                    hasOrgValue = !!$orgField.val(),
                    $orgDateDropdown = $orgFieldContainer.find('.yearDropdown'),
                    filterIndex = $('.organizationFilter').index($orgField),
                    hasValidOrgField = OrganizationDropdown.organizationNameValidity[filterIndex],
                    noDateNeeded = $orgField.hasClass('WELL'),
                    // either doesn't need a date or must have a valid one
                    hasValidDateField = noDateNeeded ? true : !!$orgDateDropdown.val(),
                    isValid = hasValidOrgField && hasValidDateField;
                // not sure what this does, but this was being executed in these circumstance
                // when this section was refactored
                if (hasOrgValue) {
                    OrganizationDropdown.updateAutocompleteSelection();
                }
                // update the validity variable
                orgsAreValid = orgsAreValid && isValid;
            });

            //if all of the remaining orgs have been validated (orgsAreValid)
            if (orgsAreValid) {
                SALT.registration.showAddButtons();
                //when removing a school/org, set focus on the "Add School button" if a school/org still exists
                $('.js-school-add-button').focus();
            }
            $('.autocomplete-suggestions.organization:visible').empty().hide();
        },
        setToDefault: function () {
            SALT.registration.setFieldHidden('.js-affiliation-add-buttons');
            SALT.registration.setFieldVisible('#rb-container');
            $('#Organization').prop('checked', false);
            $('#School').prop('checked', true);
            SALT.registration.hideSchool();
            SALT.registration.hideOrganization();
            SALT.registration.setFieldVisible('.js-select-aff-msg');
            SALT.registration.setFieldVisible('.js-organization.js-firstAffiliation.js-SCHL');
            SALT.registration.setFieldHidden('.js-remove-affiliation:first');
            SALT.registration.setFieldHidden('.js-pre-popped-msg');
            //Set focus when removing all org/schools
            $('.js-organization.js-firstAffiliation.js-SCHL .js-organization-search .organizationFilter').focus();
        },
        fieldsOnChange: function (el, index) {
            var $orgContainer = $(el).parents('.js-organization'),
                hasValue = !!el.value,
                $gradDateContainer = $orgContainer.find('.yearDropdown'),
                needsGradDate = !$orgContainer.hasClass('js-WELL'),
                hasValidGradDate = needsGradDate ? !!$gradDateContainer.val() : true;

            // is the value and (if applicable) grad date filled in?
            var isFilledIn = hasValue && hasValidGradDate;

            if (isFilledIn && orgIsValid) {
                SALT.registration.hideRadioButtons();
                SALT.registration.showRemoveLink($orgContainer);
                SALT.registration.showAddButtons();
                if (typeof index !== 'undefined' && undefined !== index) {
                    OrganizationDropdown.organizationNameValidity[index] = true;
                }
            } else {
                SALT.registration.setFieldHidden('.js-affiliation-add-buttons');
            }
        },
        adjustRedirectUrl: function (redirectUrl) {
            var hostname = document.referrer.split('/').slice(2, 3).toString().toLowerCase();
            if (hostname === '') {
                hostname = document.location.href.split('/').slice(2, 3).toString().toLowerCase();
            }
            var saltHost = hostname.search('saltmoney'),
                localHost = hostname.search('localhost');

            if ((((saltHost !== -1) || (localHost !== -1)) &&
                (document.referrer !== '' || $('.js-SPA-enabled'))) || Utility.IsCurrentPage('/landing/')) {
                if (Utility.IsCurrentPage('/register')) {
                    if (document.referrer.indexOf('/logon') > -1) {
                        var lastvistitedURL = Utility.lastvisitedURL();
                        if (lastvistitedURL !== null) {
                            redirectUrl =  '/' + lastvistitedURL.split('/').slice(3).join('/');
                        }
                    } else {
                        redirectUrl = '/' + document.referrer.split('/').slice(3).join('/');
                    }
                } else {
                    // if we are NOT in the main registration page (using a partial registration), refer to current page
                    redirectUrl = window.location.pathname + Utility.getLocationSearch();
                }
            } else {
                redirectUrl = '/index.html';
            }
            return redirectUrl;
        },
        registrationRedirect: function (redirectUrl) {
            //TODO: add analytics logger callback/promise pattern to guarantee
            //      we don't navigate away from the page until all logging is complete
            if (window.location.pathname.indexOf('/search/') > 0) {
                setTimeout(function () {
                    //registration from ppc page done, redirect to the same url.
                    window.location.href = window.location.href + '-thankyou';
                }, 500);
            } else if (Utility.getLocationSearch().indexOf('%2fAssets%2fPublications%2fThe_Military_Smartbook_For_Defeating_Student_Debt.pdf') > -1) {
                setTimeout(function () {
                    window.location.href = '/landing/MilitaryDefeatStudentDebtEbook';
                }, 500);
            } else if ((location.pathname.indexOf('index.html') > -1 && redirectUrl.indexOf('idp.saltmoney.org') === -1 && !redirectUrl.match(/pdv-idp[0-9].saltmoney.org/g)) || location.href.indexOf('Type=') > -1) {
                setTimeout(function () {
                    var tempUrl = '/home' + Utility.getLocationSearch();
                    //remove tour from URL so SALT tour video will not replay when logging in
                    tempUrl = tempUrl.replace('tour=true', '');
                    tempUrl = tempUrl.replace('?&', '?');
                    window.location.href = tempUrl;
                }, 500);
            } else {
                setTimeout(function () {
                    window.location.href = redirectUrl;//result.ReturnUrl;
                }, 500);
            }
        },

        registerErrorCallback: function () {
            console.error('Registration: registerErrorCallback: Populating regErrorCode element and closing overlay.');
            SALT.registration.setErrorMsg('Our system is having trouble with your registration. Please check that everything\'s correct-especially your email and password-and then try again. If you still can\'t register, let Member Support know by calling 855.469.2724 (toll-free).');

            SALT.closeOverlay('#regMsgOverlay');
            return false;
        },

        callSuccess: function (data) {

            if (data.ErrorList.length > 0) {
                if (data.ErrorList[0].Code === 'DuplicateUserName') {
                    //console.error(data.ErrorList[0].Code);
                    SALT.registration.setErrorMsg('It looks like our system already has an account registered to this email address.');
                    SALT.registration.setFieldVisible('div#email-reused');
                } else {
                    //console.error(data.ErrorList[0].Code);
                    SALT.registration.setErrorMsg('Our system is having trouble with your registration. Please check that everything\'s correct-especially your email and password-and then try again. If you still can\'t register, let Member Support know by calling 855.469.2724 (toll-free).');
                }

                SALT.closeOverlay('#regMsgOverlay');
                return false;
            } else {
                SALT.registration.registerSuccessCallback(data);
            }
        },
        setErrorMsg: function (msg) {
            $('#regErrorCode').html(msg);
        },
        setFieldHidden: function (field) {
            $(field).addClass('hidden');
        },
        setFieldVisible: function (field) {
            $(field).removeClass('hidden');
        },
        hideRadioButtons: function () {
            SALT.registration.setFieldHidden('#rb-container');
            SALT.registration.setFieldHidden('.js-select-aff-msg');
        },
        showRemoveLink: function ($el) {
            var $closestRemoveLink =  $el.find('.js-remove-affiliation');
            SALT.registration.setFieldVisible($closestRemoveLink);
        },
        handleSubmit: function () {

            var formData = $('#regForm').serializeObject();

            if (!Utility.CheckAgeValidity(formData.YearOfBirth)) {
                $('#js-confirm-first').text(formData.FirstName);
                $('#js-confirm-last').text(formData.LastName);
                $('#js-confirm-yob').text(formData.YearOfBirth);
                SALT.trigger('open:customOverlay', 'confirmPersonalInfo');
            } else {
                //The progress overlay opens asynchronously, we need to wait to run code that may try to close it until the overlay has been opened
                //Bind to the "opened" event, using $.one in order to have it only fire once
                $(document).one('opened.fndtn.reveal', '[data-reveal]', function () {
                    //clear out error msg
                    SALT.registration.setErrorMsg('');
                    SALT.registration.setFieldHidden('div#email-reused');

                    // Give our submit button that cool animation
                    $('button.submit').addClass('progress');

                    formData.InvitationToken = $('#inviteToken').val();
                    formData.ContactFrequency = 'true';

                    if ($('#myContactPref').is(':checked')) {
                        // continue to receive emails
                        formData.ContactFrequency = false;
                    }

                    //Set school data
                    if ($('input:radio[name=affiliationRadioBtn]:checked').val() === 'Neither') {
                        // not affiliated with a school ('No School Confirmed')
                        formData.Schools = [{
                            OrganizationId : '',
                            ExpectedGraduationYear : 1900,
                            OECode : '000000',
                            BranchCode : '01'
                        }];
                    } else {
                        var organizationIds = [], organizationNames = [], oeCodes = [], branchCodes = [], gradYears = [];
                        $('.OrganizationId').each(function () {
                            if (this.value) {
                                var theOrgFilter = $(this).parent().find('.organizationFilter').val();
                                var theOECode = $(this).parent().find('.OECode').val();
                                var theOEBranch = $(this).parent().find('.OEBranch').val();
                                var theGradYear = $(this).parents('.js-organization').find('.yearDropdown').val();
                                if (!theGradYear) {
                                    theGradYear = null;
                                }
                                organizationIds.push(this.value);
                                organizationNames.push(theOrgFilter);
                                oeCodes.push(theOECode);
                                branchCodes.push(theOEBranch);
                                gradYears.push(theGradYear);
                            }
                        });
                        if (organizationIds.length === 1 && oeCodes.length === 1) {
                            formData.Organizations = [{
                                OrganizationId : organizationIds[0],
                                ExpectedGraduationYear : gradYears[0],
                                OECode : oeCodes[0],
                                BranchCode : branchCodes[0]
                            }];
                        } else {
                            formData.Organizations = [{
                                OrganizationId : organizationIds[0],
                                ExpectedGraduationYear : gradYears[0],
                                OECode : oeCodes[0],
                                BranchCode : branchCodes[0]
                            },
                            {
                                OrganizationId : organizationIds[1],
                                ExpectedGraduationYear : gradYears[1],
                                OECode : oeCodes[1],
                                BranchCode : branchCodes[1]
                            }];
                        }
                    }

                    SALT.registration.SetRegistrationSource(formData);

                    SALT.services.RegisterMember(formData, SALT.registration.callSuccess, SALT.registration.registerErrorCallback, this, false, true);
                });
                SALT.openOverlay('#regMsgOverlay', 'Registration Status', '');
            }
            return false;
        },
        SetRegistrationSource: function (formData) {
            //SWD-8090: This is a hack to recognize registrations from the share-your-success campaign
            //          by referUrl cookie
            var referUrl = $.cookie('referUrl'),
            queryStringSource = '';

            if (this.querystringVars.src) {
                queryStringSource = this.querystringVars.src;
            }


            //If its a special event page, there will be a hidden element with name "Source"; we dont need to set the value, just return from the function
            if (formData.Source && $('.js-special-event-page').length) {
                return;
            } else if (queryStringSource) {
                formData.Source = queryStringSource;
            } else if (formData.InvitationToken === '') {
                formData.Source = '1';
            } else {
                formData.Source = '2';
            }
        }
    };

    var registrationHandler = _.once(function (returnUrl) {
        if (!returnUrl) {
            returnUrl = location.pathname + location.search;
        }
        SALT.registration.init({
            returnURLOverride: returnUrl
        });
    });

    $(function () {

        Utility.trackLastVisitedURL();
        var returnUrl = Utility.getRerouteURL();
        registrationHandler(returnUrl);

        $('#regForm').on('valid', function () {
            SALT.registration.handleSubmit();
        });

        $('.js-pw-show-hide').click(function () {
            SALT.registration.togglePassword();
        });

        $(document.body).on('change keyup', '.yearDropdown', function (e) {
            SALT.registration.fieldsOnChange(e.currentTarget);
        });

        $('input:radio[name=affiliationRadioBtn]').click(function () {
            var selectedAffiliation = $(this).val();
            //setting for webtrends stepNumber
            var stepNumber;
            if (selectedAffiliation === 'School') {
                stepNumber = 13;
            }
            else if (selectedAffiliation === 'Organization') {
                stepNumber = 14;
            }
            else if (selectedAffiliation === 'Neither') {
                stepNumber = 15;
            }

            SALT.publish('Registration:Steps', {
                stepNumber: stepNumber
            });

            SALT.registration.toggleAffiliation(selectedAffiliation);
            SALT.registration.setFieldHidden('.js-affiliation-add-buttons');
        });

        termBoxDisable = function (val) {
            $('.registrationSubmit').prop('disabled', val);
        };

        $(document.body).on('change keyup', '#AcceptTerms', function () {
            if ($('#AcceptTerms').prop('checked') === true) {
                termBoxDisable(false);
            } else {
                termBoxDisable(true);
            }
        });

        $('.js-school-add-button').click(function () {
            SALT.registration.addSchool();
            SALT.registration.setFieldHidden('.js-affiliation-add-buttons');
        });

        $('.js-organization-add-button').click(function () {
            SALT.registration.addOrganization();
            SALT.registration.setFieldHidden('.js-affiliation-add-buttons');
        });

        $(document.body).on('click', '.js-remove-affiliation', function (e) {
            SALT.registration.removeAffiliation(e.currentTarget);
        });

        //listening for the trigger in checkValue() in organizationDropdown.js
        SALT.on('InvalidOrg', function (index) {
            // set variable to false for use in FieldsOnChange
            orgIsValid = false;
            var orgField = $('.organizationFilter')[index];
            SALT.registration.fieldsOnChange(orgField, index);
        });
        // listening for the trigger in onSelect() in organizationDropdown.js
        SALT.on('ValidOrg', function (index) {
            orgIsValid = true; //set the global variable
            var orgField = $('.organizationFilter')[index];
            SALT.registration.fieldsOnChange(orgField, index);
        });
    });
    return SALT.registration;
});
