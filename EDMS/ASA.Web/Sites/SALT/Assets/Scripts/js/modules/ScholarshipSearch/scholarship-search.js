require([
    'jquery',
    'backbone',
    'configuration',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'underscore',
    'salt',
    'modules/ASALocalStore',
    'jquery.autocomplete',
    'asa/ASAWebService'
], function ($, Backbone, configuration, siteMember, utility, _, SALT, asaLocalStore) {
    var scholarshipResultsKey = 'scholarshipsFromMember:',
        individualAnswersKey = 'individualAnswers:',
        firstTimeFormSubmit = true,
        ScholarshipOverlay = Backbone.View.extend({
        el: '.js-tabs-content',
        events: {
            'click .js-panel-nav': 'panelNavClick',
            'click #js-answers-submit': 'submitClick',
            'click .js-remove-x': 'removeClick',
            'click .js-wt-next-btn': 'fireWebtrendsEvent',
            'focusout .js-scholarship-answer': 'inputFocusout'
        },
        initialize: function (context) {
            //because the some elements are not part of backbone, so bind it manually here.
            this.eventBinders();
            //cache the answers object and memberId in backbone so that it can be used in other functions;
            this.answers = context.answers;
            this.memberId = context.memberId;
            this.member = context.member;
            this.render(context);
        },
        eventBinders: function () {
            var _this = this;
            $(document.body).on('click', '.js-repeat', function () {
                scrollUpAndOpenOverlay();
                _this.updateClick();
                SALT.trigger('content:todo:inProgress', {contentId: '101-7416'});
            });
            $(document.body).on('click', '.js-scholarship-results-item', function (e) {
                var scholarshipId = $(e.currentTarget).attr('data-scholarshipId'),
                    numericAmount = $(e.currentTarget).find('.js-scholarship-amount').text();
                if (scholarshipId) {
                    SALT.services.GetScholarshipDetail(scholarshipId, function (response) {
                        response.numericDollarAmount = numericAmount;
                        utility.renderDustTemplate('Overlays/ScholarshipDetail', response, function (error, out) {
                            $('#js-scholarshipDetail').foundation().foundation('reveal', 'open');
                        }, '.js-scholarship-result-detail-container');
                    }, _this);
                }
            });
            //close any open suggestions box on reveal modal close event.
            $(document.body).on('close.fndtn.reveal', '#scholarshipSearch', function () {
                $('.js-scholarship-suggestions:visible').hide();
            });
            $(document.body).on('opened.fndtn.reveal', '#scholarshipSearch', function () {
                var panelId = $('div.content:visible').attr('id').substring(8);
                $('#scholarship-panel-' + panelId).focus();
            });
            //bind event handler here once the answers are rendered into each question.
            SALT.on('choices:changed', _this.choiceChangedEventHandler, _this);
        },
        inputFocusout: function (e) {
             //check Safari only. Chrome has both chrome and safari in userAgent string,
            //so to catch safari only, we have to make sure chrome doesn't exist in the useragent string.
            if (navigator.userAgent.indexOf('Safari') !== -1 && navigator.userAgent.indexOf('Chrome') === -1) {
                var $currentTarget = $(e.currentTarget);
                //do the timeout because we don't want to hide the suggestions box before the option click event triggers.
                //if we hide the suggestions box before the click event detects, users can't select any option.
                window.setTimeout(function () {
                    $currentTarget.autocomplete().hide();
                }, 200);
            }
        },
        render: function (context) {
            var _this = this,
                individualAnswers = JSON.parse(asaLocalStore.getLocalStorage(individualAnswersKey));
            utility.renderDustTemplate('Overlays/ScholarshipOverlay', context, function () {
                //close any open suggestions box on tab switched.
                $(document).foundation();
                _.each(context.answers, function (element, index, list) {
                    var $targetInputEl = $('.js-scholarship-answer[data-questionid="' + index + '"]');
                    //check element exists, because we only care about those questions showing up on the overlay.
                    if ($targetInputEl && $targetInputEl.length) {
                        var suggestions = _.map(element, function (ele, ind, array) {
                                return {value: ele.Description, data: ele.ID, questionId: ele.QuestionID};
                            }),
                            minChars = 0;
                        /*for County, city, high school, college, the answer list is very large, we don't want to pop up the dropdown on mouse focus
                        into the input, because it will be very slow. For questions like that, we will need users to type 3 characters first
                        and then we show the dropdown.*/
                        if (index === '6' || index === '7' || index === '16' || index === '18') {
                            minChars = 3;
                        }
                        _this.autoCompleteInitialize($targetInputEl, suggestions, minChars);
                    }
                });
                //if individualAnswers exist in the local store, we need to prepopulate them for users.
                if (individualAnswers && individualAnswers.length) {
                    _this.prePopulateSelections(individualAnswers);
                } else {
                    //if the individualAnswers don't exist in local storage, we will need to try to retrieve it from DB.
                    //The second parameter "1" is the sourceID for the scholarship tool in the DB (RefSource table)
                    if (_this.memberId) {
                        SALT.services.GetMemberQuestionAnswer(_this.memberId, 1, _this.prePopulateSelections, _this);
                    }
                }
                //context.answers can't be zero avoid error.
                if ($.isArray(context.answers)) {
                    _this.StudentUSMilitary();
                    _this.FamilyUSMilitary();
                }
            }, _this.el);
        },
        prePopulateSelections: function (answerList) {
            if (answerList && answerList.length) {
                var lastSearchDate = answerList[0].CreatedDate;
                _.each(answerList, function (element, index, list) {
                    utility.renderDustTemplate('Modules/ScholarshipAnswer', {value: element.choiceText, data: element.choiceId, questionId: element.questionId}, function (error, out) {
                        var $autoInput = $('.js-scholarship-answer[data-questionid="' + element.questionId + '"]');
                        $(out).insertBefore($autoInput);
                        SALT.trigger('choices:changed', $autoInput.closest('.js-qa-container'));
                    });
                });
                renderDaysSinceLS(lastSearchDate);
            }
        },
        choiceChangedEventHandler: function ($qaContainer) {
            var $currentAutoInput = $qaContainer.find('.js-scholarship-answer'),
                $deleteIcon = $qaContainer.find('.js-remove-x').last(),
                questionId = parseInt($currentAutoInput.attr('data-questionid'), 10),
                fullList = this.answers[questionId],
                numOfSelectedAnswers = $qaContainer.find('.js-choice-container').length,
                numOfMax = parseInt($currentAutoInput.attr('data-max-selection'), 10) || 1,
                suggestions = _.map(fullList, function (ele, ind, array) {
                    return {value: ele.Description, data: ele.ID, questionId: ele.QuestionID};
                }),
                selectedChoiceIds = _.map($qaContainer.find('.js-scholarship-choice'), function (ele, ind, array) {
                    return parseInt($(ele).attr('data-choiceid'), 10);
                });
            //determine show and hide for the auto-input.
            if (numOfSelectedAnswers === numOfMax) {
                $currentAutoInput.hide();
            } else {
                $currentAutoInput.show();
            }
            //get rid of selected choices to avoid duplication selection.
            if (selectedChoiceIds && selectedChoiceIds.length) {
                suggestions = _.filter(suggestions, function (suggestion) {
                    return !_.contains(selectedChoiceIds, suggestion.data);
                });
            }
            // Student U.S. Military Experience
            if (questionId === 30) {
                this.StudentUSMilitary();
            }
            // Family U.S. Military Experience
            if (questionId === 32) {
                this.FamilyUSMilitary();
            }
            $currentAutoInput.autocomplete().setOptions({lookup: suggestions});
            //When it comes to the last option removed, there will be no deleteIcon, so focus on the input in this case.
            if ($deleteIcon && $deleteIcon.length) {
                $deleteIcon.focus();
            }
        },
        panelNavClick: function (e) {
            var target = $(e.currentTarget).attr('data-target-title'),
                panelNumber = target.substring(12);
            e.preventDefault();
            //Scroll to the top of the page so that the user sees the inputs in order
            $('html, body').animate({scrollTop: 0}, 300);
            if ($('.js-scholarship-answer').autocomplete()) {
                $('.js-scholarship-answer').autocomplete().hide();
            }
            $('.js-panel-titles').removeClass('active');
            //Apply active styling to the tab we will now be on
            $(target).addClass('active');
            //Hide the content for the previously active tab
            $('.content.active').hide().removeClass('active');
            //Make the content for the tab we are going to active and show it
            $('#js-panel' + panelNumber).addClass('active').show();
            //do focus for 508
            $('#scholarship-panel-' + panelNumber).focus();
        },
        submitClick: function (e) {
            e.preventDefault();
            //close overlay
            $('#scholarshipSearch').foundation('reveal', 'close');
            var choicesList = _.map($('.js-scholarship-choice'), function (element, index, list) {
                return {questionId: $(element).attr('data-questionid'), choiceId: $(element).attr('data-choiceid'), choiceText: $(element).val(), sourceId: '1', CreatedDate: new Date().getTime()};
            });
            SALT.services.GetScholarships(function (data) {
                //set the expiration to 7 days, after 7 days users have to re-submit again to pull the latest scholarship data.
                asaLocalStore.setLocalStorage(scholarshipResultsKey, JSON.stringify(data), 7);
                renderResultsPage(data);
                //refresh daysSinceLS template.
                renderDaysSinceLS(new Date().getTime());
            }, JSON.stringify({'choices': _.pluck(choicesList, 'choiceId')}), this);
            var jsonObjectForDB = {memberId: this.memberId, choicesList: choicesList};
            //save the individual answers into local store for second time use.
            asaLocalStore.setLocalStorage(individualAnswersKey, JSON.stringify(jsonObjectForDB.choicesList));
            //Save the individual answers into DB on each submit request. Prevent zero length list
            if (this.memberId && choicesList.length) {
                var _member = this.member;
                SALT.services.UpsertQuestionAnswer(function (success) {
                    if (success && firstTimeFormSubmit) {
                        var product = {
                            RefProductID: 4,
                            IsMemberProductActive: true,
                            MemberProductValue: ''
                        };
                        var productModel = JSON.stringify(product);
                        SALT.services.ASAServiceCall('PUT', configuration.apiEndpointBases.MemberService, '/SubscribeToProduct/', function (subscribed) { if (subscribed) { firstTimeFormSubmit = false; }}, function () {}, productModel, null, null, null, false, true);
                        //Add the new member product to our site member object before triggering an event allowing widgets to make use of the updated status
                        _member.Products.push(product);
                        SALT.trigger('scholarship:search:acceptance');
                    }
                }, JSON.stringify(jsonObjectForDB), this);
            }
        },
        updateClick: function () {
            //Remove active styling from currently active tab
            $('.js-panel-titles').removeClass('active');
            //Apply active styling to the first tab
            $('.js-tt-panel1').addClass('active');
            //Hide the content for the previously active tab
            $('.content.active').hide().removeClass('active');
            //Make the content for the first tab we are going to active and show it
            $('#js-panel1').addClass('active').show();
        },
        removeClick: function (e) {
            var $currentTarget = $(e.currentTarget),
                $elementToRemove = $currentTarget.closest('.js-choice-container'),
                $qaContainer = $currentTarget.closest('.js-qa-container'),
                $autoInputElement = $qaContainer.find('.js-scholarship-answer');
            e.preventDefault();
            //remove the choice which user intended to close.
            $elementToRemove.remove();
            SALT.trigger('choices:changed', $qaContainer);
            window.setTimeout(function () {
                $autoInputElement.focus();
            }, 200);
        },
        autoCompleteInitialize: function ($targetEl, searchList, minChars) {
            $targetEl.autocomplete({
                minChars: minChars,
                lookupLimit: 50000,
                lookup: searchList,
                triggerSelectOnValidInput: false,
                maxHeight: 200,
                containerClass: 'autocomplete-suggestions js-scholarship-suggestions',
                showNoSuggestionNotice: true,
                noSuggestionNotice: 'No results',
                onSelect: function (suggestion) {
                    var _this = this;
                    utility.renderDustTemplate('Modules/ScholarshipAnswer', suggestion, function (err, out) {
                        $(out).insertBefore(_this);
                        SALT.trigger('choices:changed', $(_this).closest('.js-qa-container'));
                        _this.value = '';
                    });
                },
                onInvalidateSelection: function () {
                    this.value = '';
                }
            });
        },
        fireWebtrendsEvent: function (e) {
            SALT.publish('Standard:Action:Generic', { 'activity_name': $(e.currentTarget).attr('data-wt-actname'), 'activity_transaction': '1',  'content_group_type': 'Tool'});
        },
        StudentUSMilitary: function () {
            var $StudentUSMilitaryFields = $('.js-student-military-fields'),
                $StudentUSMilitaryChoices = $('.js-scholarship-choice[data-questionid=31]'),
                $StudentUSMilitaryAnswers = $('.js-scholarship-answer[data-questionid=31]');
            this.ShowHideMilitaryFields($('.js-scholarship-choice[data-questionid=30]'),
                                        $StudentUSMilitaryFields,
                                        $StudentUSMilitaryChoices,
                                        $StudentUSMilitaryAnswers);
        },
        FamilyUSMilitary: function () {
            var $FamilyUSMilitaryFields = $('.js-family-military-fields'),
                $FamilyUSMilitaryChoices = $('.js-scholarship-choice[data-questionid=33], .js-scholarship-choice[data-questionid=34]'),
                $FamilyUSMilitaryAnswers = $('.js-scholarship-answer[data-questionid=33], .js-scholarship-answer[data-questionid=34]');
            this.ShowHideMilitaryFields($('.js-scholarship-choice[data-questionid=32]'),
                                        $FamilyUSMilitaryFields,
                                        $FamilyUSMilitaryChoices,
                                        $FamilyUSMilitaryAnswers);
        },
        // Show/Hide USMilitary Input and Label fields and delete (choices if not "Yes" and matching "x")
        ShowHideMilitaryFields: function ($MilitaryDecision, $USMilitaryFields, $USMilitaryChoices, $USMilitaryAnswers) {
            if ($MilitaryDecision.val() === 'Yes') {
                $USMilitaryFields.show();
            } else {
                $USMilitaryFields.hide();
                $USMilitaryChoices.each(function () {
                    $(this).parent().remove();
                });
                var _this = this;
                // Reload the original answers to the questions
                $USMilitaryAnswers.each(function () {
                    var questionId = parseInt($(this).attr('data-questionid'), 10),
                        fullList = _this.answers[questionId],
                        suggestions = _.map(fullList, function (ele, ind, array) {
                            return {value: ele.Description, data: ele.ID, questionId: ele.QuestionID};
                        });
                    $(this).autocomplete().setOptions({lookup: suggestions});
                });
            }
        }
    });
    function renderResultsPage(data) {
        var currentDate = new Date(),
            currentTime = currentDate.getTime(),
            currentDateString = currentDate.toDateString(),
            filteredResults = _.filter(data, function (element) {
                var deadlineString = element.DeadlineDate,
                    deadlineDate = new Date(deadlineString),
                    deadlineTime = deadlineDate.getTime();
                return !!deadlineDate && deadlineDate.toDateString() !== 'Invalid Date' && (deadlineTime >= currentTime || deadlineDate.toDateString() === currentDateString);
            });
        //Sort the results by date ascending.  Sometimes Unigo returns them with date out of order, they have paid results which are at the top regardless of date.
        filteredResults = _.sortBy(filteredResults, function (result) { return new Date(result.DeadlineDate); });
        $('.js-scholarships-count').text('(' + filteredResults.length + ')');
        _.each(filteredResults, function (el, ind, arr) {
            //only render the result element when deadlineDate is valid and deadline is not in past.
            utility.renderDustTemplate('Tools/scholarship-search-results', el, function (err, out) {
                $('.js-scholarship-search-results-container').append(out);
            });
        });

        //Empty any previous results, so we don't stack duplicates
        $('.js-scholarship-search-results-container').empty();
        $('.js-hide').hide();
        $('.js-show').show();
        $('.js-scholarship-search-results-block').show();
    }

    function scrollUpAndOpenOverlay(argument) {
        var callbackCalled = false;
        $('html, body').animate({scrollTop: 0}, 300, function () {
            if (!callbackCalled) {
                callbackCalled = true;
                $('#scholarshipSearch').foundation('reveal', 'open');
            }
        });
    }

    function renderDaysSinceLS(lastSearchDate) {
        //if created date is string, that means it's from DB, we need to manually convert it to milliseconds.
        if (typeof lastSearchDate === 'string') {
            lastSearchDate = new Date(lastSearchDate.match(/\d+/)[0] * 1).getTime();
        }
        //render days since last search.
        var dayDiff = Math.round((new Date().getTime() - parseInt(lastSearchDate, 10)) / (1000 * 60 * 60 * 24));
        utility.renderDustTemplate('Modules/ScholarshipLastSearchDay', {daysSinceLS: dayDiff}, null, '.js-daysSinceLS');
    }

    $(document).ready(function () {
        siteMember.done(function (member) {
            scholarshipResultsKey += member.MembershipId;
            individualAnswersKey += member.MembershipId;
            var scholarshipResults = JSON.parse(asaLocalStore.getLocalStorage(scholarshipResultsKey));
            if (scholarshipResults) {
                //The user has previously used this tool on the machine
                //The current user hit submit to get results before.
                renderResultsPage(scholarshipResults);
            }
            //the data is about 2.8 mb, which is too big for safari local storage, therefore calling api everytime.
            SALT.services.GetUnigoAnswers(function (data) {
                new ScholarshipOverlay({answers: data, memberId: member.MembershipId, member: member});
            }, this);
        });
        $('.js-scholarship-search-btn').click(function (e) {
            scrollUpAndOpenOverlay();
            SALT.publish('Standard:Action:Generic', { 'activity_name': 'Scholarship Search 0 Get Started Button', 'activity_transaction': '1',  'content_group_type': 'Tool'});
            SALT.trigger('content:todo:inProgress', {contentId: '101-7416'});
        });

        $(document.body).on('click', '.js-scholarship-results-item', function () {
            SALT.publish('Standard:Action:Generic', { 'activity_name': 'Scholarship Search 10 Scholarship Popup Viewed', 'activity_transaction': '1',  'content_group_type': 'Tool'});
        });
    });
});
