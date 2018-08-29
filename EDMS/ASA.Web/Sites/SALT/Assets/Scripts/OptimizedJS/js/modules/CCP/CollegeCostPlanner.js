// jshint maxstatements:68
define([
    'jquery',
    'underscore',
    'backbone',
    'salt',
    'salt/models/SiteMember',
    'modules/CCP/CCPModels',
    'modules/CCP/CCPRouter',
    'modules/CCP/LoanLimits',
    'modules/CCP/CollegeCostGraph',
    'asa/ASAUtilities',
    'modules/paymentCalculator',
    'asa/ASAWebService',
    'modules/CCP/ToolTips'
], function ($, _, Backbone, SALT, SiteMember, models, CCPRouter, LoanLimits, Graph, utility, paymentCalc) {

    Backbone.history.bind("all", function (router) {
        SALT.publish('CCPAction:fire', {
            location: this.fragment
        });
    });

    SALT.on('update:graph', function () {
        SALT.trigger('sync:graph');
    });

    SALT.on('set:interestValues', function () {
        calculateLoanPayments();
    });

    SALT.on('show:graph', function () {
        $('#js-ccp-chart').show();
        //because graph was hidden to start need to resize.
        $('#js-ccp-chart').highcharts().reflow();
        $('#js-CCPMobileFlyOutButton').removeClass('hiddenitem');
    });

    SALT.on('update:headerBoxTotals', function (model) {
        //update the header box totals
        $('.js-remaining-costs').text(utility.currencyComma(model.get('costOfAttendanceLessAdjustmentsTotal')));
        var years = parseInt(model.get('yearsRemaining'), 10) || 0;
        if (years === 1) {
            $('.js-remaining-years').text(model.get('yearsRemaining') + ' year');
        } else {
            $('.js-remaining-years').text(model.get('yearsRemaining') + ' years');
        }

    });

    SALT.on('update:resultsPage', function (model) {
        //update the results page values
        $('.js-cost-of-attendence-results').html(utility.currencyComma(model.get('costOfAttendanceTotal')));
        $('.js-yearly-grants-results').html(utility.currencyComma(model.get('grantsTotal')));
        $('.js-total-contributions-results').html(utility.currencyComma(model.get('plannedContributionsTotal')));
        $('.js-monthly-installments-results').html(utility.currencyComma(model.get('monthlyInstallmentsTotal')));
        $('.js-total-amount-results').html(utility.currencyComma(model.get('loansTotal') + model.get('interestTotal')));
        $('.js-total-loans-results').html(utility.currencyComma(model.get('loansTotal')));
        $('.js-total-interest-results').html(utility.currencyComma(model.get('interestTotal')));
    });

    SALT.on('change:loanTotalsMsg', function (model) {
        // toggleLoansMessage
        if (model.get('costOfAttendanceLessAdjustmentsTotal') < 0) {
            $('.js-overpaying').show();
            $('.js-fed-student-or-parent').hide();
            $('.js-noloans').hide();
            $('.js-loans-total').prop('disabled', true);
            $('.js-minus-loan').prop('disabled', true);
            $('.js-plus-loan').prop('disabled', true);
        } else if (model.get('costOfAttendanceLessAdjustmentsTotal') > 0) {
            $('.js-overpaying').hide();
            $('.js-fed-student-or-parent').show();
            $('.js-noloans').hide();
            $('.js-loans-total').prop('disabled', false);
            $('.js-minus-loan').prop('disabled', false);
            $('.js-plus-loan').prop('disabled', false);
        } else {
            $('.js-overpaying').hide();
            $('.js-fed-student-or-parent').hide();
            $('.js-noloans').show();
            $('.js-loans-total').prop('disabled', true);
            $('.js-minus-loan').prop('disabled', true);
            $('.js-plus-loan').prop('disabled', true);
        }
    });

    SALT.on('change:yourPlanMsg', function () {
        var msgType = LoanLimits.determineYourPlanMessage(userInfo, totalLoansCurrentValues);

        // hide yourPlan messages
        $('.js-FSLCopy').hide();
        $('.js-FSLContent').hide();
        $('.js-PlusLoanCopy').hide();
        $('.js-PlusLoanContent').hide();
        $('.js-PrivateLoanCopy').hide();
        $('.js-PrivateLoanContent').hide();
        $('.js-GrantsCopy').hide();
        $('.js-GrantsContent').hide();
        $('.js-GradPlusLoanCopy').hide();
        $('.js-GradPlusLoanContent').hide();
        //show the correct ones
        $('.js-' + msgType  + 'Copy').show();
        $('.js-' + msgType  + 'Content').show();
    });

    SALT.on('reset:loanTotals', function (model) {
        $('.js-loans-total-fsl').val(model.get('maxFsl'));
        $('.js-loans-total-fppl').val(model.get('maxFppl'));
        $('.js-loans-total-other').val(0);

        setLoanTotalValues();
        SALT.trigger('set:interestValues', model);
    });

    SALT.on('change:costOfAttendanceLessAdjustmentsTotal', function (model) {
        // toggleCostOfAttendanceMessage
        if (model.get('costOfAttendanceLessAdjustmentsTotal') < 0) {
            $('.js-exceed-coa-message').show();
        } else {
            $('.js-exceed-coa-message').hide();
        }
    });

    var userInfo = new models.UserInfoModel();
    var totalLoansCurrentValues = new models.LoansModel();
    var monthlyLoans = new models.LoansModel();
    var yearlyLoans = new models.LoansModel();

    //global array to hold Grants and Scholarship values & max yearsToFund based on year in school
    var gsHelper = {
        numberOfQuestions: 20,
        numberOfQuestionsDisplayed: 0,
        startingQuestionId: 201,
        idList: []
    };
    var graph = new Graph({el: '#js-ccp-chart', model: userInfo});
    for (var i = gsHelper.startingQuestionId; i < gsHelper.startingQuestionId + gsHelper.numberOfQuestions; i++) {
        gsHelper.idList.push(i);
    }
    $('.WidgetsButton').click(function (e) {
        $('body').toggleClass('Widgetactive');
    });
    $('.js-year-in-school').click(function () {
        var yearInSchool = $('.js-year-in-school:checked').attr('data-choiceid');
        userInfo.set('gradeLevel', (yearInSchool - 1));
        var yearsToFundLimit = LoanLimits.getMaxYearsToFund(yearInSchool - 1);
        var currentYearsToFund = parseInt($('#count').val(), 10);
        //if limit is met for this grade level reset yearsRemaining
        if (currentYearsToFund > yearsToFundLimit) {
            $('#count').val(yearsToFundLimit).attr('data-choiceid', yearsToFundLimit + 5);
            userInfo.set('yearsRemaining', yearsToFundLimit);
        }
    });
    $('#minus').click(function () {
        var currentValueMinusOne = parseInt($('#count').val(), 10) - 1;
        if (currentValueMinusOne >= 1) {
            //The external id's for 1-7 start at 6 and end at 12, so we add 5 to whatever the value is to match
            $('#count').val(currentValueMinusOne).attr('data-choiceid', currentValueMinusOne + 5);
            userInfo.set('yearsRemaining', currentValueMinusOne);
        }
    });
    $('#plus').click(function () {
        var currentValuePlusOne = parseInt($('#count').val(), 10) + 1;
        //max years to fund varies depending on gradeLevel
        var yearsToFundLimit = LoanLimits.getMaxYearsToFund(userInfo.get('gradeLevel'));

        if (currentValuePlusOne <= yearsToFundLimit) {
            //The external id's for 1-7 start at 6 and end at 12, so we add 5 to whatever the value is to match
            $('#count').val(currentValuePlusOne).attr('data-choiceid', currentValuePlusOne + 5);
            userInfo.set('yearsRemaining', currentValuePlusOne);
        }
    });
    $('.js-minus-loan-fsl').click(function (e) {
        var increments = $(e.currentTarget).closest('div').find('.js-student-loans').attr('increments');
        setLoanTotalsField('fsl', userInfo, totalLoansCurrentValues, parseInt(increments, 10) * -1);
    });
    $('.js-plus-loan-fsl').click(function (e) {
        var increments = $(e.currentTarget).closest('div').find('.js-student-loans').attr('increments');
        setLoanTotalsField('fsl', userInfo, totalLoansCurrentValues, parseInt(increments, 10));
    });
    $('.js-minus-loan-fppl').click(function (e) {
        var increments = $(e.currentTarget).closest('div').find('.js-student-loans').attr('increments');
        setLoanTotalsField('fppl', userInfo, totalLoansCurrentValues, parseInt(increments, 10) * -1);
    });
    $('.js-plus-loan-fppl').click(function (e) {
        var increments = $(e.currentTarget).closest('div').find('.js-student-loans').attr('increments');
        setLoanTotalsField('fppl', userInfo, totalLoansCurrentValues, parseInt(increments, 10));
    });
    $('.js-minus-loan-other').click(function (e) {
        var increments = $(e.currentTarget).closest('div').find('.js-student-loans').attr('increments');
        setLoanTotalsField('other', userInfo, totalLoansCurrentValues, parseInt(increments, 10) * -1);
    });
    $('.js-plus-loan-other').click(function (e) {
        var increments = $(e.currentTarget).closest('div').find('.js-student-loans').attr('increments');
        setLoanTotalsField('other', userInfo, totalLoansCurrentValues, parseInt(increments, 10));
    });
    $('.js-minus-install').click(function (e) {
        var value = $(e.currentTarget).closest('div').find('.js-monthly-installments').val();
        var increments = $(e.currentTarget).closest('div').find('.js-monthly-installments').attr('increments');
        value = (parseInt(value, 10) || 0) - parseInt(increments, 10);
        $(e.currentTarget).closest('div').find('.js-monthly-installments').val(value);
        $('.js-monthly-installments').trigger('change');
    });
    $('.js-plus-install').click(function (e) {
        var value = $(e.currentTarget).closest('div').find('.js-monthly-installments').val();
        var increments = $(e.currentTarget).closest('div').find('.js-monthly-installments').attr('increments');
        value = (parseInt(value, 10) || 0) + parseInt(increments, 10);
        $(e.currentTarget).closest('div').find('.js-monthly-installments').val(value);
        $('.js-monthly-installments').trigger('change');
    });
    //buttons on the loans page
    $('.js-total-btn').click(function () {
        $('.js-loans-by-total').show();
        $('.js-loans-by-month').hide();
        $('.js-loans-by-year').hide();
    });
    $('.js-by-month-btn').click(function () {
        setLoanViews();
        $('.js-loans-by-total').hide();
        $('.js-loans-by-month').show();
        $('.js-loans-by-year').hide();
    });
    $('.js-by-year-btn').click(function () {
        setLoanViews();
        $('.js-loans-by-total').hide();
        $('.js-loans-by-month').hide();
        $('.js-loans-by-year').show();
    });
    function enableStartButton() {
        $('.js-landing-start').removeAttr('disabled');
    }

    function addGrantRow(element) {
        if (gsHelper.numberOfQuestionsDisplayed < gsHelper.numberOfQuestions) {
            utility.renderDustTemplate('Modules/GrantScholarshipRow', {}, function (error, out) {
                renderListBox(out, element);
            }, null);
        }
    }

    function getGrantQuestionId() {
        return gsHelper.idList.shift();
    }

    function renderListBox(out, element) {
        $('.js-individual-grants').append(out);
        //update global numbers for next values to be added
        gsHelper.numberOfQuestionsDisplayed += 1;
        var questionId = getGrantQuestionId();

        var response;
        if (element === null) {
            response = ['Choose One', null];
        } else {
            response = element.FreeformAnswerText.split(":");
        }

        var $grantSetupId = $('.js-grant-setup-id');
        //set the questionId
        $grantSetupId.attr('data-questionid', questionId);
        //add a name value
        $grantSetupId.attr('name', 'js-individual-grant' + questionId);
        //remove the setup class id so next row added will be able to identify the row
        $grantSetupId.removeClass('js-grant-setup-id');
        //change the id for uniqueness
        $("#js-grants-dropdown").attr('id', 'js-grants-dropdown' + questionId);
        //select the item in the dropdown
        $('#js-grants-dropdown' + questionId + '').find('option:contains("' + response[0] + '")').each(function () {
            if ($(this).text() === response[0]) {
                $(this).prop('selected', 'selected');
            }
        });
        //put the value into the input value
        $('input[name=js-individual-grant' + questionId + ']').val(response[1]);

        //hide add button if max number of questions/grants displayed
        if (gsHelper.numberOfQuestionsDisplayed >= gsHelper.numberOfQuestions) {
            $('.js-plus-grant-button').hide();
        }

        $('#js-grants-dropdown' + questionId).focus();
    }
    //Enable the start button when both sets of radio buttons have been selected
    $('[name="yearInSchool"]').change(enableStartButton);

    SiteMember.done(function (siteMember) {
        if (siteMember.IsAuthenticated === 'true') {
            SALT.services.GetMemberQuestionAnswer(siteMember.MembershipId, 2, function (answerList) {
                if (answerList && answerList.length) {
                    _.each(answerList, function (element, index, list) {
                        if (element.questionId >= gsHelper.startingQuestionId && element.questionId < (gsHelper.startingQuestionId + gsHelper.numberOfQuestions)) {
                            addGrantRow(element);
                        } else if (element.FreeformAnswerText) {
                            $('.js-free-answer[data-questionid="' + element.questionId + '"]').val(element.FreeformAnswerText);
                        } else if (element.RefQuestionID === 45) {
                            $('.js-years-remaining').val(element.choiceText);
                            //Since we already have a years remaining answer, we need to set that on the userInfo model, in case the user doesnt change their saved response
                            userInfo.set('yearsRemaining', parseInt(element.choiceText, 10));
                        } else {
                            $('.js-year-in-school[data-choiceid="' + element.choiceId + '"]').attr('checked', true);
                            //Since we already have an answer for yearInSchool, we can enable the start button right away
                            enableStartButton();
                        }
                    });

                    //set totals button on loans page
                    $('.js-total-btn').attr('checked', true);

                    //Set values on the backbone model if we already have an amount saved from a previous use of the tool
                    var costOfAttendance = parseInt($('.js-cost-of-attendance').val(), 10) || 0;
                    var yearlyGrants = parseInt($('.js-yearly-grants').val(), 10) || 0;
                    var totalContributions = parseInt($('.js-total-contributions').val(), 10) || 0;
                    var monthlyInstallments = parseInt($('.js-monthly-installments').val(), 10) || 0;
                    var yearInSchool = $('.js-year-in-school:checked').attr('data-choiceid');

                    //don't keep the total of student loans in the db so sum them up
                    var loansTotal = reduceIndividualFields('.js-loans-total');
                    setLoanTotalValues();

                    userInfo.set(
                        {
                            gradeLevel: (yearInSchool - 1),
                            costOfAttendancePerYear: costOfAttendance,
                            grantsPerYear: yearlyGrants,
                            plannedContributionsTotal: totalContributions,
                            monthlyInstallmentPerMonth: monthlyInstallments,
                            loansTotal: loansTotal,
                            setLoanDefaults: false
                        });

                    SALT.trigger('set:interestValues');
                    SALT.trigger('update:resultsPage', userInfo);
                    SALT.trigger('update:graph', userInfo);
                    //put back to true after values have been set
                    userInfo.set({ setLoanDefaults: true });
                }
            });
        }
    });

    $('.js-backend-save').click(function () {
        SiteMember.done(function (siteMember) {
            if (siteMember.IsAuthenticated === 'true') {
                var choices = _.map($('.js-years-remaining, .js-year-in-school:checked'), function (element, index, list) {
                    return {questionId: $(element).attr('data-questionid'), choiceId: $(element).attr('data-choiceid'), choiceText: $(element).val(), sourceId: '2', CreatedDate: new Date().getTime(), FreeformAnswerText: null};
                });

                var notEmptyFreeResponses = _.filter($('.js-free-answer'), function (element, index, list) {
                    return $(element).val().trim();
                });
                var freeResponses = _.map(notEmptyFreeResponses, function (element, index, list) {
                    var answer = $(element).val();
                    var questionId = parseInt($(element).attr('data-questionid'), 10);
                    if (questionId >= gsHelper.startingQuestionId && questionId < (gsHelper.startingQuestionId + gsHelper.numberOfQuestions)) {
                        //get the selected value from dropdown
                        answer = $('#js-grants-dropdown' + questionId + '').find('option:selected').text() + ':' + answer;
                    }
                    return {questionId: questionId, choiceId: $(element).attr('data-choiceid'), choiceText: 'Free Response', sourceId: '2', CreatedDate: new Date().getTime(), FreeformAnswerText: answer};
                });

                choices = choices.concat(freeResponses);

                var jsonObjectForDB = {memberId: siteMember.MembershipId, choicesList: choices};

                SALT.services.UpsertQuestionAnswer(function (data) {
                    //console.log('CCP Data sent to DB?' + data);
                }, JSON.stringify(jsonObjectForDB));

                //console.log(choices);
                //console.log(userInfo);
            }
        });
    });

    $('.js-next-page').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        //Per SWD-9135 the "start" buttons sets the todo in progress and the "results" button sets it complete
        if ($(this).hasClass('js-results-btn')) {
            SALT.publish('content:todo:completed', {
                contentId: $('.js-todoContainer').attr('data-primary-key'),
                contentType: $('.js-todoContainer').attr('data-content-type'),
                contentTitle: $('.js-todoContainer').attr('data-content-title')
            });
        } else if ($(this).hasClass('js-landing-start')) {
            SALT.trigger('content:todo:inProgress', {contentId: '101-23826'});
        }
        if (validateFieldsForNavigation()) {
            SALT.trigger('navigate', $(e.target).attr('href'));
        }
    });
    $('.js-prev-page').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (validateFieldsForNavigation()) {
            SALT.trigger('navigate', $(e.target).attr('href'));
        }
    });
    //edit buttons on results page
    $('.js-edit-button').click(function (e) {
        e.preventDefault();
        SALT.trigger('navigate', $(e.target).attr('href'));
    });
    function validateFieldsForNavigation() {
        var invalid_fields = [];
        invalid_fields = $('.js-ccp-form').find('[data-invalid]');
        if (invalid_fields.length === 0) {
            return true;
        } else {
            return false;
        }
    }
    $('.js-cost-of-attendance').change(function () {
        var inputValue = parseInt($(this).val(), 10) || 0;
        userInfo.set('costOfAttendancePerYear', inputValue);
        $(this).val(inputValue);
        //Clear individual expense fields since the user chose to manually enter a total
        $('.js-individual-expense').val('');
    });

    $('.js-yearly-grants').change(function () {
        var inputValue = parseInt($(this).val(), 10) || 0;
        userInfo.set('grantsPerYear', inputValue);
        $(this).val(inputValue);
        $('.js-individual-grants').empty();
    });

    $('.js-total-contributions').change(function () {
        var inputValue = Math.round(parseFloat($(this).val())) || 0;
        userInfo.set('plannedContributionsTotal', inputValue);
        $(this).val(inputValue);
        //Clear individual planned contribution fields since the user chose to manually enter a total
        $('.js-planned-contribution').val('');
    });
    function setLoanTotalValues() {
        totalLoansCurrentValues.set({
            fsl: parseInt($('.js-loans-total-fsl').val(), 10) || 0,
            fppl: parseInt($('.js-loans-total-fppl').val(), 10) || 0,
            other: parseInt($('.js-loans-total-other').val(), 10) || 0
        });
    }
    function setLoanViews() {
        $('.js-loans-monthly-fsl').text('$' + monthlyLoans.get('fsl') + ' /month');
        $('.js-loans-monthly-fppl').text('$' + monthlyLoans.get('fppl') + ' /month');
        $('.js-loans-monthly-other').text('$' + monthlyLoans.get('other') + ' /month');

        $('.js-loans-yearly-fsl').text('$' + yearlyLoans.get('fsl') + ' /year');
        $('.js-loans-yearly-fppl').text('$' + yearlyLoans.get('fppl') + ' /year');
        $('.js-loans-yearly-other').text('$' + yearlyLoans.get('other') + ' /year');
    }
    function reduceIndividualFields(selectorToReduce) {
        return _.reduce($(selectorToReduce), function (memo, num) {
            var valueOfField = Math.round(parseFloat(num.value)) || 0;
            //redisplay with decimal points removed.
            if (valueOfField !== 0) {
                num.value = valueOfField;
            }
            if (valueOfField) {
                return memo + valueOfField;
            } else {
                return memo;
            }
        }, 0);
    }
    $('.js-free-answer').on('blur', function () {
        $(this).val(parseInt($(this).val(), 10)); //strip out non numeric charcters
        $(this).removeAttr('data-invalid').next('.error').hide();//remove errors for decimals in Firefox
    });
    $('.js-individual-expense').change(function (e) {
        //Sum up the values in the individual expense fields and change the value of the total field to the sum
        var totalExpenses = reduceIndividualFields('.js-individual-expense');
        $('.js-cost-of-attendance').val(totalExpenses);
        userInfo.set('costOfAttendancePerYear', totalExpenses);
    });
    $('.js-planned-contribution').change(function (e) {
        //Sum up the values in the individual planned contribution fields and change the value of the total field to the sum
        var totalPlannedContributions = reduceIndividualFields('.js-planned-contribution');
        $('.js-total-contributions').val(totalPlannedContributions);
        userInfo.set('plannedContributionsTotal', totalPlannedContributions);
    });
    $('.js-monthly-installments').change(function (e) {
        if ((parseInt(e.target.value, 10) || 0) <= 0) {
            e.target.value = 0;
        }

        //Sum up the values in the monthly installment fields and change the value of the total field to the sum
        var totalMonthlyInstallmentContributions = reduceIndividualFields('.js-monthly-installments');
        userInfo.set('monthlyInstallmentPerMonth', totalMonthlyInstallmentContributions);
    });
    $('.js-loans-total-fsl').change(function (e) {
        var value = parseInt($('.js-loans-total-fsl').val(), 10) || 0;
        if (value <= 0) {
            value = 0;
        }
        var increment = value - totalLoansCurrentValues.get('fsl');
        setLoanTotalsField('fsl', userInfo, totalLoansCurrentValues, increment);
    });
    $('.js-loans-total-fppl').change(function (e) {
        var value = parseInt($('.js-loans-total-fppl').val(), 10) || 0;
        if (value <= 0) {
            value = 0;
        }
        var increment = value - totalLoansCurrentValues.get('fppl');
        setLoanTotalsField('fppl', userInfo, totalLoansCurrentValues, increment);
    });
    $('.js-loans-total-other').change(function (e) {
        var value = parseInt($('.js-loans-total-other').val(), 10) || 0;
        if (value <= 0) {
            value = 0;
        }
        var increment = value - totalLoansCurrentValues.get('other');
        setLoanTotalsField('other', userInfo, totalLoansCurrentValues, increment);
    });
    function setLoanTotalsField(loanType, userInfo, totalLoansCurrentValues, value) {
        var displayValues = LoanLimits.calculateLoanDisplayTotals(loanType, userInfo, totalLoansCurrentValues, value);
        $('.js-loans-total-fsl').val(displayValues.fsl);
        $('.js-loans-total-fppl').val(displayValues.fppl);
        $('.js-loans-total-other').val(displayValues.other);

        loansTotalProcess();
    }
    function loansTotalProcess() {
        //store current input values
        setLoanTotalValues();
        calculateLoanPayments();
        setLoanViews();
        SALT.trigger('update:resultsPage', userInfo);
        SALT.trigger('update:graph', userInfo);
        //Sum up the values in the student loan fields and change the value of the total field to the sum
        var totalLoans = reduceIndividualFields('.js-loans-total');
        userInfo.set('loansTotal', totalLoans);
    }
    function calculateTotalGrants() {
        //Sum up the values in the individual grant fields and change the value of the total field to the sum
        var summedGrants = reduceIndividualFields('.js-individual-grant');
        $('.js-yearly-grants').val(summedGrants);
        userInfo.set('grantsPerYear', summedGrants);
    }
    function calculateLoanPayments() {
        var totalInterest = 0;
        monthlyLoans.set({fsl: 0, fppl: 0, other: 0});
        yearlyLoans.set({fsl: 0, fppl: 0, other: 0});

        if (totalLoansCurrentValues.get('fsl') > 0) {
            var fslInfo = paymentCalc.calculateSimpleAmortizedPayment(userInfo.get('gradeLevel') === 0 ? '.06' : '.0445', totalLoansCurrentValues.get('fsl'), 120);
            monthlyLoans.set('fsl', fslInfo.monthly);
            yearlyLoans.set('fsl', fslInfo.annual);
            totalInterest += fslInfo.totalInterest;
        }
        if (totalLoansCurrentValues.get('fppl') > 0) {
            var fpplInfo = paymentCalc.calculateSimpleAmortizedPayment('.07', totalLoansCurrentValues.get('fppl'), 120);
            monthlyLoans.set('fppl', fpplInfo.monthly);
            yearlyLoans.set('fppl', fpplInfo.annual);
            totalInterest += fpplInfo.totalInterest;
        }
        if (totalLoansCurrentValues.get('other') > 0) {
            var otherInfo = paymentCalc.calculateSimpleAmortizedPayment('.1', totalLoansCurrentValues.get('other'), 120);
            monthlyLoans.set('other', otherInfo.monthly);
            yearlyLoans.set('other', otherInfo.annual);
            totalInterest += otherInfo.totalInterest;
        }
        userInfo.set('interestTotal', totalInterest);
    }

    $(document.body).on('change', '.js-individual-grant', calculateTotalGrants);

    $('#js-plus-grant').click(function () {
        addGrantRow(null);
    });

    $(document.body).on('click', '.js-remove-grant', function (e) {
        var questionId = $(e.currentTarget).closest('.row').find('.js-individual-grant').attr('data-questionid');
        $(e.currentTarget).closest('.row').remove();
        calculateTotalGrants();

        //update global numbers for next values to be added
        gsHelper.numberOfQuestionsDisplayed -= 1;
        gsHelper.idList.push(parseInt(questionId, 10));
        gsHelper.idList = _.sortBy(gsHelper.idList, function (num) {
            return num;
        });

        //hide add button if max number of questions/grants displayed
        if (gsHelper.numberOfQuestionsDisplayed < gsHelper.numberOfQuestions) {
            $('.js-plus-grant-button').show();
            $('#js-plus-grant').focus();
        }
    });

    $(document.body).on('click', '#js-CCPMobileFlyOutButton', function () {
        $('html, body').animate({
            scrollTop: ($('#CCPGraph').offset().top - 100)
        }, 1000);
    });

    //Instantiate a single CCPRouter object to handle page changes
    //THIS IS WHAT MAKES THIS FUNCTION LIKE AN SPA
    var ccpRouter = new CCPRouter();

    Backbone.history.start({ root: '/R-101-23826/' });

});
