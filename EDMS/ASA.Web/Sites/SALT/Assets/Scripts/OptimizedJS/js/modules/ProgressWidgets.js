require([
    'salt',
    'salt/models/SiteMember',
    'jquery',
    'modules/ASALocalStore',
    'modules/LoanCollection',
    'modules/FinancialStatus/Totals',
    'asa/ASAUtilities',
    'underscore',
    'configuration',
    'modules/ReportingStatus',
    'jquery.cookie'
], function (SALT, SiteMember, $, asaLocalStore, collections, Totals, utility, _, Configuration, reportingStatus) {
    var loans = new collections.LoansCollection(),
        COURSES_KEY = 'coursesCompletion',
        NUM_INITIAL_VISIBLE_COURSES = 3; //initially show 3 courses

    function updateLoanWidgets() {
        loans.fetch({reset : true, success: function (model, resp, options) {
            var kwyoLoans = loans.filter(function (currentModel) {
                if (currentModel.get('RecordSourceId') !== utility.SOURCE_SELF_REPORTED_REP_NAV) {
                    return true;
                }
            });

            updateKWYOWidget(kwyoLoans);

            //Find out if we have at least one loan that is elgigible for Repayment Nav (imported or Rep Nav self reported)
            //If we find even one, we should update the widget
            //.some short circuits when the first truth test is passed so we don't have to loop through each of the loans when the condition is true
            var hasRepNavEligibleLoans = loans.some(function (currentModel) {
                var source = currentModel.get('RecordSourceId');
                if (source === utility.SOURCE_IMPORTED_REP_NAV || source === utility.SOURCE_SELF_REPORTED_REP_NAV || source === utility.SOURCE_IMPORTED_KWYO) {
                    return true;
                }
            });

            if (hasRepNavEligibleLoans) {
                updateRepaymentNavWidget(loans);
            } else {
                $('#js-RepNav-widget').addClass('hide');
            }
        }});
    }

    updateLoanWidgets();
    //When our loan data changes, update the KWYO and RN widgets
    SALT.on('user:loans:changed', updateLoanWidgets);

    SiteMember.done(function (siteMember) {
        updateProfileWidget(siteMember);
        updateSaltCoursesWidget(siteMember, 'fromCache', true);
        SALT.services.getTodos(function (todos) {
            updateScholarshipSearchWidget(siteMember, todos);

            SALT.on('scholarship:search:acceptance', function () {
                updateScholarshipSearchWidget(siteMember, todos);
            });
        });
    });

    function updateKWYOWidget(kwyoLoans) {
        if (kwyoLoans.length > 0) {
            //We are not passing an element to this view because we arent going to render it to the screen, just using the calculation methods for now
            var totals = new Totals({ collection: new collections.LoansCollection(kwyoLoans) });
            $('#js-KWYO-total').text('$' + utility.currencyComma(Math.ceil(totals.calculate('PrincipalBalanceOutstandingAmount').total)));
            $('#js-KWYO-date').text(totals.findNewestLoanDate());
            $('#js-KWYO-widget').removeClass('hide');
            $('.js-quick-look').show();
        } else {
            $('#js-KWYO-widget').addClass('hide');
        }
    }

    function updateRepaymentNavWidget(loans) {
        var amountBorrowed = 0;
        var repNavSelfReported = loans.filter(function (currentModel) {
            if (currentModel.get('RecordSourceId') === utility.SOURCE_SELF_REPORTED_REP_NAV) {
                return true;
            }
        });

        var importedLoans = loans.filter(function (currentModel) {
            if (currentModel.get('RecordSourceId') === utility.SOURCE_IMPORTED_REP_NAV || currentModel.get('RecordSourceId') === utility.SOURCE_IMPORTED_KWYO) {
                return true;
            }
        });

        //Determine whether to use SelfReported or Imported data
        //Use the latest loans, e.g. if import was done last, use import
        var lastUpdatedDate;
        if ((repNavSelfReported.length && importedLoans.length && utility.convertSALtoJSdate(repNavSelfReported[0].get('DateAdded')) > utility.convertSALtoJSdate(importedLoans[0].get('DateAdded'))) || (repNavSelfReported.length && !importedLoans.length)) {
            amountBorrowed = repNavSelfReported[0].get('OriginalLoanAmount');
            lastUpdatedDate = utility.convertSALtoJSdate(repNavSelfReported[0].get('DateAdded'));
        } else {
            for (var i = 0; i < importedLoans.length; i++) {
                amountBorrowed += importedLoans[i].get('OriginalLoanAmount');
            }
            lastUpdatedDate = utility.convertSALtoJSdate(importedLoans[0].get('DateAdded'));
        }
        $('#js-RepNav-total').text('$' + utility.currencyComma(amountBorrowed));
        $('#js-RepNav-date').text(utility.formatDate(lastUpdatedDate));
        $('#js-RepNav-widget').removeClass('hide');
        $('.js-quick-look').show();
    }

    function updateProfileWidget(siteMember) {
        $('.js-quick-look').show();

        var responses = _.last(siteMember.ProfileQAs).Responses;
        var answeredQuestions = [];
        for (var i = 0; i < responses.length; i++) {
            var extId = responses[i].QuestionExternalId;
            // check for under 17 is temporary until onboarding questions
            // are aligned with profile questions.
            if (extId < 17 && !_.contains(answeredQuestions, extId))
            {
                answeredQuestions.push(extId);
            }
        }
        var totalPossibleQuestions = 20;
        // 5 Required Responses
        var totalResponses = 5;
        // +13 Profile Question Responses
        totalResponses += answeredQuestions.length;
        // +1 Zip Code
        if (siteMember.USPostalCode) {
            totalResponses++;
        }
        // +1 Enrollment Status
        if (siteMember.EnrollmentStatus !== '') {
            totalResponses++;
        }

        var percent = Math.round((totalResponses / totalPossibleQuestions) * 100);

        if (percent === 100)
        {
            $('#js-Profile-percent').text('Complete').removeClass('sidebar-module-data-color').addClass('sidebar-module-data-complete-color');

        } else {
            $('#js-Profile-percent').text(percent + '% Complete');
        }

    }

    function updateSaltCoursesWidget(siteMember, source, onInitialLoad) {
        var showCoursesWidget = false;
        //siteMemberNotRequired for just refreshing the module when it is already accessible
        if (siteMember === 'siteMemberNotRequired') {
            showCoursesWidget = true;
        } else {
            showCoursesWidget = utility.isSubscribedToProduct(siteMember, 2);
        }
        
        if (showCoursesWidget) {
            $('.js-quick-look').show();
            var coursesData;
            if (source === 'fromCache') {
                coursesData = JSON.parse(asaLocalStore.getLocalStorage(COURSES_KEY));
            }
            
            if (coursesData && coursesData.length) {
                renderCoursesWidget(coursesData, onInitialLoad);
            }
            else {
                if (source === 'fromMoodle') {
                    fromMoodle = true;
                    //trigger completion update to Sync Salt 
                    SALT.services.SyncCoursesCompletion(function (synced) {
                        if (synced) {
                            //This is a temporary approach so that the courses tiles would reflect the latest
                            window.location.reload(true);
                        }
                    });
                }
                else {
                    fromMoodle = false;
                }
                SALT.services.GetMemberCourses(function (coursesData) {
                    //set the expiration to 1 day
                    asaLocalStore.setLocalStorage(COURSES_KEY, JSON.stringify(coursesData), 1);    
                    renderCoursesWidget(coursesData, onInitialLoad);
                }, siteMember, fromMoodle);
            }
        }
    }

    function expandCoursesContainer() {
        $('#js-Courses-container, .js-rr-courses-toggle, .rr-courses-toggle').addClass('expanded');
    }

    function collapseCoursesContainer() {
        $('#js-Courses-container, .js-rr-courses-toggle, .rr-courses-toggle').removeClass('expanded');
    }

    function renderCoursesWidget(coursesData, onInitialLoad) {
        //sort courses by completionstatus, name
        //Sort the courses by CompleteStatus ascending, and IdNumber (which is same as ShortName).
        coursesData = _.sortBy(coursesData, 'CompleteStatus');
        $('#js-Courses-wrapper').removeClass('hide');
        //display each course's widget
        utility.renderDustTemplate('Modules/CourseWidget', coursesData, function (err, out) {
            var $courseRows = $(out);
            // populate the container
            $('#js-Courses-container').html($courseRows);
            if (onInitialLoad) {
                // start collapsed
                collapseCoursesContainer();
            } else {
                // if here, we are refreshing the courses, expand
                expandCoursesContainer();
            }
            $('.js-rr-course-widget a').each(function () {
                $(this).data('coursepath', $(this).attr('data-coursepath'));
            });
            //determine if need to intercept for ReportingId          
            SiteMember.done(function (siteMember) {
                var showReportIdQuestion = reportingStatus.checkReportIdStatus(siteMember);
                reportingStatus.initOverlay(showReportIdQuestion);
            });
        });
    }

    function updateScholarshipSearchWidget(siteMember, todos) {
        var toolTask = _.where(todos, {ContentID: '101-7416'}),
            memberSubscribedToProduct = utility.isMemberSubscribedToProduct(siteMember, 4);
        if (memberSubscribedToProduct) {
            $('.js-quick-look').show();
            $('#js-ScholarshipSearch-widget').removeClass('hide');
            //var lastUpdatedDate, dateString;
            var individualAnswersKey = 'individualAnswers:' + siteMember.MembershipId,
                individualAnswers = JSON.parse(asaLocalStore.getLocalStorage(individualAnswersKey));
            //if individualAnswers exist in the local store.
            if (individualAnswers && individualAnswers.length) {
                //Need to reformat date as SAL date so that the utility function can operate on it
                var salDate = '/Date(' + individualAnswers[0].CreatedDate + ')/';
                formatAndDisplayUpdatedDate(salDate, 'ScholarshipSearch');
            } else {
                //if the individualAnswers don't exist in local storage, retrieve it from DB,
                //The second parameter "1" is the sourceID for the scholarship tool questions.
                SALT.services.GetMemberQuestionAnswer(siteMember.MembershipId, 1, function (responses) {
                    if (responses && responses.length) {
                        formatAndDisplayUpdatedDate(responses[0].CreatedDate, 'ScholarshipSearch');
                    }
                });
            }
        }
    }
    
    function formatAndDisplayUpdatedDate(dateData, toolName) {
        var lastUpdatedDate, dateString;
        lastUpdatedDate = utility.convertSALtoJSdate(dateData);
        //Now that we know the latest date format as expected. e.g. Jan 5, 2015
        dateString =  utility.formatDate(lastUpdatedDate);
        $('#js-' + toolName + '-date').text(dateString).parent().show();
    }

    $(document.body).on('click', '.js-rr-courses-toggle', function (e) {
        if ($('#js-Courses-container').hasClass('expanded')) {
            collapseCoursesContainer();
        } else {
            expandCoursesContainer();
        }
    });

    $(document.body).on('click', '.js-refresh', function (e) {
        //siteMemberNotRequired for just refreshing the module when it is already accessible
        updateSaltCoursesWidget('siteMemberNotRequired', 'fromMoodle', false);
        // lets find the correct button in case we've added a second js-refresh trigger
        $('#js-Courses-widget .js-refresh').toggleClass('refreshRotate');
    });

    $(document.body).on('click', '.js-rr-module a, .js-rr-module input', function (e) {
        var activityType,
            $clickedModule = $(e.currentTarget).closest('.js-rr-module'),
            //the '%'' and '&nbsp;' replacements are for manageprofile WT call showing
            //'(unable to decode value)' in the post
            activityName = $clickedModule.find('.js-rr-module-header').text().replace('%', ' percent').replace(/\u00A0/g, ' ');

        //if sub module is Goal1
        if ($clickedModule.hasClass('js-top-goal')) {
            activityType = $clickedModule.find('input').attr('value');
        }
        else if ($clickedModule.hasClass('js-rr-course-widget')) {
            //if sub module is a course use 'open' since no button (it's a hyperlink)
            activityType = 'Open';
        }
        else {
            //the '&nbsp;' replacement is for Log Out WT call showing
            //'(unable to decode value)' in the post
            activityType = $clickedModule.find('a').text().replace(/\u00A0/g, ' ');
        }
        //Fire WT, SWD-9005 Right Rail, Event
        SALT.publish('dashboard:action:taken', {
            activity_name: 'Right Rail:' + activityName,
            activity_type: activityType
        });
    });
});
