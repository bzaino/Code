define([
    'jquery',
    'underscore',
    'salt',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'jquery.cookie'
], function ($, _, SALT, SiteMember, Utility) {

    $(document.body).on('click', '.js-salt-courses-link', function () {
        var _this = this;
        //SWD-8550: Show login windowshare if someone is in the remembered state but not logged in
        if (!($.cookie('IndividualId')) && $.cookie('RememberMe')) {
            SALT.trigger('open:customOverlay', 'loginOverlay');
        }
        else if ($.cookie('IndividualId')) {
            SiteMember.done(function (siteMember) {
                if (ReportingStatus.showOverlay || ReportingStatus.checkReportIdStatus(siteMember)) {
                    var enrollmentStatus = SiteMember.get('EnrollmentStatus');
                    $('#' + enrollmentStatus + '-selected').attr('selected', '');
                    //targetCoursePath captures the current (in context) link's 
                    //original href to eventually send the user to
                    overlayHolderObject.targetCoursePath = $(_this).data('coursepath');
                    $(overlayHolderObject.overlay).foundation('reveal', 'open');
                } else {
                    //reporting ID entered or not needed, go to courses
                    Utility.deepLinkToCourse($(_this).data('coursepath'));
                }
            });
        }
    });

    $(document.body).on('valid', '#FrmReportId', function () {
        var bGotoSALTCourses = ReportingStatus.submitChanges();
        if (bGotoSALTCourses === true) {
            Utility.deepLinkToCourse(overlayHolderObject.targetCoursePath);
        }
    });

    $(document.body).on('change', '#selectEnrollmentStatusOverlay', function () {
        $('#js-reporting-id-status-message').text('');
        //if status in not one of the excluding answers and no reportingId has been collected
        if (!ReportingStatus.validateStatus($(overlayHolderObject.selectBoxEnrollmentStatus).val()) && ReportingStatus.showOverlay) {
            $('.js-reporting-id-block').show();
        } else {
            $('.js-reporting-id-block').hide();
        }
    });

    var ReportingStatus = {
        showOverlay: false, //initially assume no overlay
        overlayHolderObject: '',
        initOverlay: function (bShowReportIdQuestion) {
            //if enrollment status answer is not one of these (there was no valid answer)
            //or no reportingId collected show overlay
            if (!ReportingStatus.validateStatus(this.overlayHolderObject.EnrollmentStatus) && bShowReportIdQuestion) {
                ReportingStatus.showOverlay = true;
            }
            //************************************************
            //Hide if reportingId already collected, or no enrollemnt status collected
            //reportingId box depends on enrollment status answer
            //show overlay with enrollment status but hide reportingId box until it is needed
            //************************************************
            if (!bShowReportIdQuestion || this.overlayHolderObject.EnrollmentStatus === '') {
                $('.js-reporting-id-block').hide();
            }
            //************************************************
            // overlay setup for all mm101 links
            //************************************************
            if (ReportingStatus.showOverlay) {
                $('.js-salt-courses-link').attr({
                    href: '#IDFrm'
                });
                if ($.cookie('IndividualId')) {
                    $(this.overlayHolderObject.selectBoxEnrollmentStatus).val(this.overlayHolderObject.EnrollmentStatus);
                }
            }
        },
        submitChanges: function () {
            var bGotoSALTCourses = true;
            objectPlaceHolder = $(overlayHolderObject.selectBoxEnrollmentStatus).val();

            var $reportingId = $('.js-reporting-id');

            //if no valid enrollment status provided
            if (objectPlaceHolder === 'Q' || objectPlaceHolder === null) {
                //prompt user for valid enrollment status
                $('#js-reporting-id-status-message').text('Please select a status from the drop down');
                bGotoSALTCourses = false;
            } else {
                //user has selected an enrollment status
                //hide overlay
                $('.close-reveal-modal').click();

                _.each($('.js-organization-id'), function (element, ind, arr) {
                    var organizationId = parseInt($('.js-organization-id')[ind].value, 10);
                    var reportingId = $reportingId[ind].value || null;
                    if (reportingId) {
                        overlayHolderObject.validReportingIdSubmitted = true;
                        _.findWhere(overlayHolderObject.Organizations, {'OrganizationId': organizationId}).ReportingId = reportingId;
                    }
                });
                //if at least one valid reportingId submitted, update reportingId
                if (overlayHolderObject.validReportingIdSubmitted) {
                    SALT.services.UpdateMemberOrganizations(overlayHolderObject.MemberId, overlayHolderObject.Organizations, ReportingStatus.reportIDSuccess, ReportingStatus.serviceCallFailure);
                }
                
                SiteMember.done(function (siteMember) {
                    //update enrollment status only if there's a valid change 
                    if (siteMember.EnrollmentStatus !== objectPlaceHolder) {
                        ReportingStatus.updateEnrollmentStatus(siteMember);
                    }
                });  
            }
            return bGotoSALTCourses;
        },
        checkReportIdStatus: function (siteMember) {
            var showReportIdQuestion = false;
            //check that all Member's Organizations have a reportingId for organizations that participate
            var organizations = _.filter(siteMember.Organizations, function (organization) {
                return _.findWhere(siteMember.OrganizationProducts, {OrganizationId: organization.OrganizationId, ProductID: 1, IsOrgProductActive: true});
            });
            if (organizations) {
                var reportingID = _.findWhere(organizations, {ReportingId: null});
                if (reportingID) {
                    showReportIdQuestion = true;
                }
            }

            return showReportIdQuestion;
        },
        showReportingIdQuestion: function () {
            SiteMember.done(function (siteMember) {
                if (siteMember.IsAuthenticated === 'true') {
                    var showReportIdQuestion = ReportingStatus.checkReportIdStatus(siteMember);
                    ReportingStatus.setupReportingIdOverlay(showReportIdQuestion, siteMember);
                }
            });
        },
        setupReportingIdOverlay: function (bRptIdShow, siteMember) {
            overlayHolderObject = {
                targetCoursePath: '',
                validReportingIdSubmitted: false,
                overlay: '#reportModal',
                selectBoxEnrollmentStatus: '#selectEnrollmentStatusOverlay',
                MemberId: siteMember.MembershipId,
                EnrollmentStatus: siteMember.EnrollmentStatus,
                Organizations: siteMember.Organizations,
                OrganizationProducts: _.where(siteMember.OrganizationProducts, {ProductID: 1, IsOrgProductActive: true})
            };

            ReportingStatus.populateReportingId();
            ReportingStatus.initOverlay(bRptIdShow);
        },
        validateStatus: function (selectEnrollmentStatus) {
            //these values don't require reportingId . no textbox should show
            if (selectEnrollmentStatus === 'X' || selectEnrollmentStatus === 'Q' || selectEnrollmentStatus === 'G' || selectEnrollmentStatus === 'W' || selectEnrollmentStatus === 'N') {
                return true;
            }
            return false;
        },
        populateReportingId: function () {
            //check if organizations participates in reportingId product
            var organizations = _.filter(overlayHolderObject.Organizations, function (organization) {
                return _.findWhere(overlayHolderObject.OrganizationProducts, {OrganizationId: organization.OrganizationId});
            });
            if (organizations.length > 0) {
                context = {
                    Organizations : organizations
                };
                Utility.renderDustTemplate('Overlays/ReportingIdRow', context, function (error, out) {
                    $('.js-individual-reporting-id').append(out);
                    ReportingStatus.getUserSegment();
                }, null); 
            }
        },
        updateEnrollmentStatus: function (member) {
            //here objectPlaceHolder is a string value
            member.EnrollmentStatus = objectPlaceHolder;

            // treating YOB as a string as a workaround a .NET bug with smallint type
            var yob = member.YearOfBirth;
            if (yob && typeof yob === 'number') {
                member.YearOfBirth = yob.toString();
            }
            SALT.services.ManageAccount(member, ReportingStatus.statusUpdateSuccess, ReportingStatus.serviceCallFailure);
        },
        reportIDSuccess: function (updateSuccessful) {
            //update the UI links immediately up on successful valid reportingid save
            if (updateSuccessful && overlayHolderObject.validReportingIdSubmitted) { 
                $('.js-salt-courses-link').each(function (index) {
                    $(this).attr('href', '#');
                });
                ReportingStatus.showOverlay = false;
            }
        },
        statusUpdateSuccess: function (data) {
            //console.log('EnrollmentStatus updated successfully');
        },
        getUserSegment: function () {
            if ($.cookie('UserSegment')) {
                SALT.services.GetUserSegment($.cookie('UserSegment'), ReportingStatus.updateLabel, ReportingStatus.serviceCallFailure);
            }
        },
        updateLabel: function (data) {
            try {
                var endecaDefaultLabel = _.findWhere(data.secondaryContent[4].contents, {name: 'Default'});
                _.each(overlayHolderObject.OrganizationProducts, function (element, ind, arr) {
                    organization = _.findWhere(overlayHolderObject.Organizations, {OrganizationId: element.OrganizationId});
                    var endecaContent = _.findWhere(data.secondaryContent[4].contents, {name: organization.OrganizationLogoName});
                    var labelIndentifier = '#js-reporting-id-label' + organization.OrganizationId;
                    if (endecaContent) {
                        $('' + labelIndentifier + '').html($(endecaContent.content));
                    } else if (endecaDefaultLabel) {
                        $('' + labelIndentifier + '').html($(endecaDefaultLabel.content));
                    }
                    else {
                        $('' + labelIndentifier + '').html('<p>Enter your Organization/School ID to ensure you receive credit for the SALT Courses you complete.</p>');
                    }
                });
            } catch (e) {
                if (console && console.log) {
                    console.log('updateLabel Error:' + e.message);
                }
            }
        },
        serviceCallFailure: function (jqxhr, textStatus, error) 
        {
            if (console && console.log) {
                console.log('JSON Request Failed--- TextStatus:' + textStatus + ' -- Error: ' + error);
            }
        }
    };
    return ReportingStatus;
});
