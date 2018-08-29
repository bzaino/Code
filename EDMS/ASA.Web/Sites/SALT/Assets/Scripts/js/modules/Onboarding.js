/* jshint -W083 */
/* jshint maxstatements: 26 */
define([
    'salt',
    'salt/models/SiteMember',
    'jquery',
    'underscore',
    'asa/ASAUtilities',
    'jquery.cookie',
    'asa/ASAWebService',
    'jquery.serializeObject'
], function (SALT, SiteMember, $, _, utility) {

        // lowest question ID must be first in this list
    var goalQuestionIDs = [17, 18, 19, 20],
        $panelBeforeTagSelection = $(''),
        //Store the number of enabled goals
        numberofEnabledGoals,
        //ASCII code for key pad numbers
        keyCodeForOne = 49,
        // this is the same as above for keypress in jQuery, but not for keydown
        numPadKeyCodeForOne = 97;


    function validateNumber(e) {
        var key = e.which;
        //If not between 1 and numberOfEnabledGoals, the key entered is invalid, do not allow it
        //E.g. If there are 3 goals enabled, only allow keys 1, 2, or 3
        //also check if this is a tabbing to allow that action
        var validNumber = !e.shiftKey && (key >= keyCodeForOne && key < keyCodeForOne + numberofEnabledGoals) || (key >= numPadKeyCodeForOne && key < numPadKeyCodeForOne + numberofEnabledGoals),
            // tab, backspace, or delete
            isUtilityKey = key === 9 || key === 8 || key === 46;
        return  validNumber || isUtilityKey;
    }

    //Handles changes to the goal rank field, when valid, do nothing, when empty, set to be last ranked
    //Cant use keypress to handle empty field because it doesnt get called, only gets called when a key is entered, not when deleted
    function handleRankChange() {
        if (!$(this).val()) {
            //If there is no value in the field, set it to be last ranked
            $(this).val(numberofEnabledGoals);
        }
    }

    //Watch keypress to only allow valid characters, watch change to disallow empty field
    $(document.body).on('keydown', '.goal-rank', validateNumber);
    $(document.body).on('change', '.goal-rank', handleRankChange);

    function reorderItems(things, parent) {
        //access a goal ranking button from the index, clone the list before we move it around
        var $buttonsBeforeReordering = $(parent + ' li').clone();
        ////Empty the ranking list of buttons, they will be appended back in order below
        $(parent).empty();
        for (var i = 1; i <= $(things).length; i++) {
            // js hint says don't create functions inside a loop, but
            // I'm having trouble making this work otherwise.
            // Ignoring warning for now.
            $(things).each(function (index) {
                var x = parseInt($(this).find('input').val(), 10); 
                if (x === i) {
                    //append the button that was at this ranking to the list
                    $buttonsBeforeReordering.eq(index).appendTo(parent);
                }
            });
        }
    }

    function mergeResponses(existingResponses, newResponses) {
        var destinationArray = [];
        var answerQuestionIDs = _.pluck(newResponses, 'QuestionExternalId');
        _.each(existingResponses, function (obj, ind, arr) {
            //Find questions in the existing array that are not in the adding array and add them to the destination
            //We want to leave out any old responses to questions that have new answers
            if (answerQuestionIDs.indexOf(obj.QuestionExternalId) === -1) {
                return destinationArray.push(obj);
            }
        });
        destinationArray = destinationArray.concat(newResponses);
        return destinationArray;
    }

    function showTagSelectionPanel() {
        $('.js-profileQA-container, .js-qa-header, .js-onboarding-exit, .outer-progress-meter-wrapper').hide();
        $('.js-tag-selection-header').show();
        $('#js-tag-selection-container').children().first().addClass('active-panel');
        focusFirstInput();
    }

    function determineTagSelectionEntry($this) {
        //"Skip questions/exit" was clicked, or the user clicked "Next" on the last panel
        // If no form answers have been chosen, hide the Q+A onboarding questions and show the tag selection for the top ranked goal
        //Store a reference to the panel we clicked from, so that we can navigate back to it if the user clicks the "back" button from tag selection
        //If there are answers, they aren't eligible for tag selection, use the href on the <a> they clicked to navigate to dashboard
        if ($('.js-profileQA-container input:checked').length === 0) {
            showTagSelectionPanel();
        } else {
            $('.onboarding-container').hide();
            SALT.trigger('need:navigation', $this.attr('data-url'));
            $('.onboarding-header').hide();
        }
    }

    function saveGoalRanking() {
        SiteMember.done(function (siteMember) {
            var serializedRanking = _.map($('.js-goal-rank'), function (el, index, arr) {

                var externalID = $(el).attr('data-externalID');

                if (index === 0) {
                    document.cookie = 'TopGoal=' + $(el).attr('data-segmentName') + '; PATH=/';
                    //Put the container for the top-ranked goal's tag selections first in the selection container, so that if the user hits the skip button, they will see those
                    $('#js-tag-selection-container').prepend($('#js-tag-selection-' + externalID));
                    //Update the side bar Goal 1 with the recent
                    $('#js-goal1').text($(el).attr('data-description'));
                }

                //Re-order the Q+A sections based on the ranking
                $('.js-profileQA-container').append($('#js-onboarding-questions-' + externalID));
                //Re-order the sidebar goal widgets according to the ranking
                $('#js-goalwidget-container').append($('.js-goalwidget-' + externalID));
                //On the last panel, we need to have the "Next" button which links to the homepage (or the tag selection panel if there are no responses), rather than a next button which tries to show the next panel (which doesnt exist)
                if (index === arr.length - 1) {
                    $('#js-onboarding-questions-' + externalID).addClass('js-last-onboarding-panel').find('.js-get-started').show().prev().hide();
                } else {
                    $('#js-onboarding-questions-' + externalID).removeClass('js-last-onboarding-panel').find('.js-get-started').hide().prev().show();
                }

                //We need to add a specific class to the first Q+A panel "back" button so that it can show the goal rank rather than the previous Q+A panel when clicked
                $('.js-back').removeClass('js-first-back').first().addClass('js-first-back');

                var ansName = $(el).attr('data-segmentName');
                ansName = ansName.replace('Dashboard-TopGoal-', '');

                var ansDescription = $(el).attr('data-description');

                //The first goal ranking question has ExternalQuestionID = 17, the subsequent ranking questions are 18, 19, etc
                var firstGoalId = goalQuestionIDs[0];
                return { AnsExternalId: externalID, QuestionExternalId: index + firstGoalId, AnsName: ansName, AnsDescription: ansDescription, nameWithNoSpaces: ansName.replace(/[\W_]+/g, '')};
            });
            //Merge the new responses with the old so that our in memory version of the siteMember contains the up to date Q+A data
            _.last(siteMember.ProfileQAs).Responses = mergeResponses(_.last(siteMember.ProfileQAs).Responses, serializedRanking);
            //Set the new rankings to the goalRankResponses property which will get used by dust templates downstream
            siteMember.goalRankResponses = serializedRanking;
            SALT.services.upsertProfileResponse(siteMember.MembershipId, JSON.stringify(serializedRanking));
        });
    }

    // webtrends for Q&A section
    function onboardingWebtrendsHelper(navigationButton, currentPanel) {
        var navigationText = navigationButton.getAttribute('data-navigation'),
            $forms = $('.js-profileQA-container form'),
            goalName =  currentPanel.find('.js-onboarding-goal').text(),
            webtrendsGoalIdentifier = 'Follow Up Questions: ' + goalName,
            stepNumberOffset = 3,
            currentStep = $forms.index(currentPanel) + stepNumberOffset;

        if (navigationText === 'Next') {
            var questionContainers = currentPanel.children('.js-question-container'),
                isLastStep = (currentStep === 6) ? 1 : '';
            questionContainers.map(function () {
                var questionText = $(this).find('.js-onboarding-question').text(),
                    answers = $(this).find('input:checked+label'),
                    answersFormatted = answers.map(function () {
                        return $(this).text();
                    }).get().join('|');

                SALT.publish('onboarding:action:taken', {
                    step_num: currentStep,
                    activity_sub_name: questionText,
                    activity_sub_name_2: answersFormatted,
                    activity_type: navigationText,
                    activity_name: webtrendsGoalIdentifier,
                    is_final_step: isLastStep
                });
            });
        } else {
            SALT.publish('onboarding:action:taken', {
                step_num: currentStep,
                activity_type: navigationText,
                activity_name: webtrendsGoalIdentifier
            });
        }
    }

    function saveOnboardingQAResponses(siteMember, $currentlyShowingPanel) {
        //Serialize the form using serializeObject, which produces the datatype expected by "setMemberResponses"
        var serializedForm = $currentlyShowingPanel.serializeObject();
        //setMemberResponses parses the data into the models expected by the SAL
        var formattedResponseData = utility.setMemberResponses(serializedForm);

        //Send the data to the back end to be saved to the DB if we have any responses
        if (formattedResponseData.length) {
            // AnsName not set in formattedResponseData, but that should be fine for now. - mt 5/6/2016
            _.last(siteMember.ProfileQAs).Responses = mergeResponses(_.last(siteMember.ProfileQAs).Responses, formattedResponseData);
            SALT.services.upsertProfileResponse(siteMember.MembershipId, JSON.stringify(formattedResponseData));
        }

        //Find any questions from the currently showing panel that have user responses
        var $answeredQuestions = $currentlyShowingPanel.find('.js-question-container').filter(function (index) {
            return $(this).find('input:checked').length;
        });

        //Loop through the questions which have responses, looping through each of the answer in those questions
        //If the answer has a response, set a cookie to be used in returning endeca recommendations
        //If not, remove the coookie for that answer, as it might have been previously answered and no longer is
        _.each($answeredQuestions, function (question, ind, arr) {
            _.each($(question).find('input'), function (checkbox, index, checkboxes) {
                //Use the "id" of the checkbox as the name of the cookie to add/delete
                //The value of id will come in the form "ans-questionID-answerID" e.g. ans-3-1 for question 3, answer 1
                var $checkbox = $(checkbox);
                if ($checkbox.attr('checked') === 'checked') {
                    document.cookie = $checkbox.attr('id') + '=' + $checkbox.attr('data-segmentName') + '; PATH=/';
                } else {
                    $.removeCookie($checkbox.attr('id'));
                }
            });
        });
    }

    // general function for updating the progress meter
    function updateProgressMeter(pageNumber) {
        var numGoals = $('.goal-rank').length,
            percentIncr = 100;
        if (numGoals) {
            percentIncr = 100 * (1 / $('.goal-rank').length);  
        }
        var newPercent = percentIncr * (pageNumber + 1);
        $('.progress-percent').text(newPercent);
        $('#onboarding-meter .meter').width(newPercent + "%");
    }

    function initializeProgressMeter() {
        updateProgressMeter(0);
    }

    // used for tags selection remembered panel
    function setProgressMeterToQAPanel($currentPanel) {
        updateProgressMeter($('.js-profileQA-container form').index($currentPanel));
    }

    function incrementProgressMeter($currentPanel) {
        updateProgressMeter($('.js-profileQA-container form').index($currentPanel) + 1);
    }

    function decrementProgressMeter($currentPanel) {
        updateProgressMeter($('.js-profileQA-container form').index($currentPanel) - 1);
    }

    // sets new goal ranking order and goes to first page of QA
    function confirmGoalsAndContinue() {
        if ($('.js-goal-rank').length > 1) {
            reorderItems('#goal-rank-num li', '#sortable-goals');
        }
        //Make sure the number labels are in the proper order, the user might navigate back to goal ranking using the back button
        utility.getInitialOrder('#goal-rank-num li');
        SALT.trigger('goalrank:updated');
        $('#js-rank-container').removeClass('active-panel');
        $('.js-onboarding-exit, .outer-progress-meter-wrapper').show();
        //Show the first Q+A panel now that goal rank is hidden
        $('.js-profileQA-container').children().first().addClass('active-panel');
        focusFirstInput();
        //Scroll to the top of the screen in case so that the user is always seeing the top of the new panel
        $('html, body').animate({scrollTop: 0}, 300);
        initializeProgressMeter();
    }

    function focusFirstInput() {
        $('.active-panel input').first().focus();
    }

    function focusNavigationButton(dataNavigationValue) {
        $('.active-panel [data-navigation="' + dataNavigationValue + '"]').focus();
    }

    // handles calls to navigation links in the onboarding panels
    function navigateOnboarding($navigationButton, $currentPanel) {
        // we need to go back
        if ($navigationButton.hasClass('js-back')) {
            // hide current panel
            $currentPanel.removeClass('active-panel');
            // return to Goal Ranking
            if ($navigationButton.hasClass('js-first-back')) {
                $('.js-onboarding-exit, .outer-progress-meter-wrapper').hide();
                $('#js-rank-container').addClass('active-panel');
            // normal change to previous QA panel
            } else {
                $currentPanel.prev().addClass('active-panel');
                decrementProgressMeter($currentPanel);            
            }
            focusNavigationButton("Next");
        } // we are on tag selection, need to return to previous panel
        else if ($navigationButton.hasClass('js-tag-selection-back')) {
            // The "back" button on the "tag selection" panel was clicked.  
            $currentPanel.removeClass('active-panel');
            // hide tag selection header, should change this class...
            $('.js-tag-selection-header').hide();
            // redisplay QA html elements
            $('.js-profileQA-container, .js-onboarding-exit, .outer-progress-meter-wrapper').show();
            // We need to show the panel that we were on before coming to tag selection.
            // $panelBeforeTagSelection refers to the panel we want to return to
            $panelBeforeTagSelection.addClass('active-panel');
            focusNavigationButton("Next");
            setProgressMeterToQAPanel($panelBeforeTagSelection);
        } 
        //If we have clicked a "next" button on the last panel, check if we are eligible for tag selection
        else if ($currentPanel.hasClass('js-last-onboarding-panel') && $navigationButton.hasClass('js-get-started')) {
            // save current panel globally for the file to be accessed later if we need to come back from tag selection
            $panelBeforeTagSelection = $currentPanel.removeClass('active-panel');
            determineTagSelectionEntry($navigationButton);
        }
        // this is a regular "next" from one QA to another
        else if (!$navigationButton.hasClass('js-tag-selection-finished') && !$navigationButton.hasClass('js-skip-questions')) {
            $currentPanel.removeClass('active-panel').next().addClass('active-panel');
            focusFirstInput();
            incrementProgressMeter($currentPanel);
        }
        
        //Scroll to the top of the screen in case so that the user is always seeing the top of the new panel
        //Much more likely use case for mobile devices
        $('html, body').animate({scrollTop: 0}, 300);

        // send off webtrends with Q&A's if we are not on tags page
        if ($currentPanel.closest('.js-profileQA-container').length) {
            onboardingWebtrendsHelper($navigationButton[0], $currentPanel);
        }
    }

    SiteMember.done(function (siteMember) {

        // done for 508 in initial helper overlay so opening next overlay tabs straight to it
        // attempted to do this with the Foundation "reveal open" listener, but that wasn't firing 
        // on I.E. 11 or Edge
        $(document.body).on('focus', '#menu-btn, .js-rr-skip-link', function () { 
            var $openOverlay = $('.js-instr.open');
            if ($openOverlay.length) {
                $openOverlay.find('.close-reveal-modal').focus(); 
            }
        });

        SALT.on('goalrank:rendered', function () {
            // scroll to top so the full window is seen since this is SPA
            window.scroll(0, 0);
            numberofEnabledGoals = $('.goal-rank').length;
            if (!siteMember.goalRankResponses.length) {
                saveGoalRanking();
            }
        });

        SALT.on('goalrank:updated', saveGoalRanking);

        $(document.body).on('click', '.js-set-goal-rank', function () {
            confirmGoalsAndContinue();
        });

        $(document.body).on('click', '.js-panel-navigation', function (e) {
            //Find the current form
            var $navigationButton = $(this);
            var $currentlyShowingPanel = $('.active-panel');
            // submit responses to server
            saveOnboardingQAResponses(siteMember, $currentlyShowingPanel);
            // navigation through onboarding
            navigateOnboarding($navigationButton, $currentlyShowingPanel);
        });

        $(document.body).on('click', '.js-skip-questions', function (e) {
            // save panel for later in case we want to come back
            $panelBeforeTagSelection = $('.active-panel').removeClass('active-panel');
            determineTagSelectionEntry($(this));
        });
        // close helper overlay
        $(document.body).on('click', '.js-close-overlay', function (e) {
            var overlayToClose = $(e.currentTarget).closest('.reveal-modal');
            SALT.closeOverlay(overlayToClose);
        });
        // Webtrends listeners
        // Rank your Goal Listener
        $(document.body).on('click', '#js-rank-container .js-set-goal-rank, #js-rank-container .js-back-out', function (e) {
            var navigationText = e.target.getAttribute('data-navigation'),
                formattedGoals = '';
            if (navigationText === 'Next') {
                formattedGoals = $('.js-goal-rank').map(function () {
                    return this.getAttribute('data-description');
                }).get().join('|');
            }
            SALT.publish('onboarding:action:taken', {
                step_num: '2',
                activity_sub_name: formattedGoals,
                activity_type: navigationText,
                activity_name: 'Rank Your Goals'
            });
        });
    });

});
