/* global console, vlcRouter, progressWidget, Loans, importedLoans:true, currentLoansself whatYouveLearnedWidget, dust */
/* jshint maxstatements: 35 */
require([
    'jquery',
    'salt',
    'underscore',
    'backbone',
    'dust',
    'modules/paymentCalculator',
    'modules/VLC/VLCModels',
    'modules/LoanCollection',
    'modules/VLC/VLCViews',
    'modules/VLC/VLCRouter',
    'modules/VLC/VLCUtilities',
    'dust-helpers',
    'salt/analytics/webtrends',
    'datejs',
    'modules/SaltDustHelpers'
], function ($, SALT, _, Backbone, dust, paymentCalc, models, collections, views, VLCRouter, utilities) {

    Backbone.history.bind("all", function (router) {
        SALT.publish('vlcAction:fire', {
            location: this.fragment
        });
    });

    SALT.on('calculation:needed', function (model) {
        if (currentLoans.length) {
            //Use 'Plan' model property to determine which calculation to call
            var plan = model.get('Plan');
            var payment = paymentCalc.calculationChooser(plan, currentLoans.toJSON(), model.toJSON());
            SALT.trigger('set:progressModel', { initialMonthlyPayment: Math.floor(payment.initial) });
        }
    });

    SALT.on('saveAnswers:called', function (event) {
        var responseModel = new models.ResponseModel({
            MemberID: 0,
            Question: {
                QuestionID: parseInt($('#questionID').text().substr($('#questionID').text().indexOf('-') + 1), 10),
                QuestionVersion: parseInt($('#itemVersion').text(), 10),
                QuestionText: $('#Question').children().text()
            },
            ResponseText: event.target.textContent
        });
        responseModel.save();
    });

    SALT.on('setHowMuch', function () {
        SALT.trigger('vlcWT:fire', 'complete');
        $('.errorMessage').hide();
        var howMuch = $('#howMuch').val();
        var interest = $('#interestRate').val();
        //Create a variable to store the model data, we may need to use this in different code paths
        var modelData = {
            PrincipalBalanceOutstandingAmount: howMuch,
            OriginalLoanAmount: howMuch,
            InterestRate: interest,
            LoanName: 'Repayment Navigator',
            MonthlyPaymentAmount: null,
            ServicingOrganizationName: 'Navigator Servicer',
            LoanTerm: 10
        };

        //Make sure there are self reported models
        if (selfReportedloans.models[0]) {
            //If so, set the data rather than creating a new one
            selfReportedloans.models[0].set(modelData);
        } else {
            //No selfReported, add one
            selfReportedloans.add(modelData);
        }

        //Save the model we just updated/added to the server
        selfReportedloans.models[0].save(modelData, {wait: true, success: function (model, response) {
            //Change the current loans collection to the data for the new loan
            currentLoans = selfReportedloans;
            //Trigger an event noting that our user loans changed, in case any widgets need to re-calculate
            SALT.trigger('user:loans:changed');
            SALT.trigger('set:progressModel', {InterestRate: interest, AmountBorrowed: howMuch, initialMonthlyPayment: response.MonthlyPaymentAmount});
        }});

    });

    SALT.on('showSkip', function () {
        if (importedLoans.length || selfReportedloans.length) {
            $('#hiddenSkip').show();
        }
    });

    SALT.on('ManageablePaymentYNView:initialize', function (self) {
        progressWidget.model.on('change:Plan', self.setUiFields);
        progressWidget.model.on('change:AmountBorrowed', self.setUiFields);
        progressWidget.model.on('change:initialMonthlyPayment', self.setUiFields);
    });

    SALT.on('ManageablePaymentYNView:setUiFields', function () {
        var amountBorrowed = progressWidget.model.get('AmountBorrowed');
        $('#amountBorrowed').html(amountBorrowed);
        $('#monthlyPayment').html(progressWidget.model.get('initialMonthlyPayment'));
    });

    SALT.on('getLoansAndSetModels', function () {
        getLoansAndSetModels();
    });

    SALT.on('vlcWT:fire', function (activity, path) {
        if (!path) {
            path = Backbone.history.fragment;
        }
        SALT.publish('vlcAction:fire', {
            location: path,
            actType : activity
        });
    });

    //Why is this one event using dcsMultiTrack????
    SALT.on('trigger:dcsMultiTrack', function () {
        dcsMultiTrack('WT.cg_s', 'HomePage');
    });

    $(document.body).on('click', '#js-widget-scroll', function () {
        $('html, body').animate({
            scrollTop: ($('#Widgets').offset().top - 100)
        }, 1000);
    });

    function getLoansAndSetModels() {
        Loans.fetch({
            success: function (model, response, options) {
                importedLoans.reset(model.filter(function (model) {
                    var source = model.get('LoanSource');
                    return (source === 'Imported' ||  source === 'Imported-KWYO');
                }));
                //Filter for first manual loan with the RepaymentNavigator LoanTypeId
                selfReportedloans.reset(model.findWhere({ LoanSource: 'Member', LoanTypeId: 'UE' }));

                //Determine whether to use SelfReported or Imported for collections (set CurrentLoans to either one)
                //Use the latest loans, e.g. if import was done last, use import
                if ((selfReportedloans.length && importedLoans.length && utilities.convertDate(selfReportedloans.toJSON()[0].DateAdded) > utilities.convertDate(importedLoans.toJSON()[0].DateAdded)) || (selfReportedloans.length && !importedLoans.length)) {
                    currentLoans = selfReportedloans;
                    progressWidget.model.set({ AmountBorrowed: selfReportedloans.models[0].get('OriginalLoanAmount') });
                } else {
                    var calculatedImportedAmountBorrowed = 0;
                    var balancesArray = importedLoans.pluck('OriginalLoanAmount');
                    for (var i = 0; i < balancesArray.length; i++) {
                        calculatedImportedAmountBorrowed += balancesArray[i];
                    }
                    currentLoans = importedLoans;
                    progressWidget.model.set({ AmountBorrowed: calculatedImportedAmountBorrowed });
                    SALT.trigger('calculation:needed', progressWidget.model);
                }
            }
        });
    }

    //Create instances of Page components
    var Loans = new collections.LoansCollection();
    var selfReportedloans = new collections.LoansCollection();
    var importedLoans = new collections.LoansCollection();
    var currentLoans = new collections.LoansCollection();

    var progressModel = new models.ProgressModel();

    var questionHeadline = new views.QuestionHeadline({el: '#Question'});
    var featuredContent = new views.FeaturedContent({el: '#FeaturedContent'});
    var hiddenFields = new views.HiddenFieldsView({el: '#hiddenFields'});
    var applicationView = new views.ApplicationView({el: '#applicationView'});
    var vlcBody = new views.VLCBody({el: '#VLCBody'});
    var contextualHelp = new views.ContextualHelp({el: '#ContextualHelp'});
    window.progressWidget = new views.ProgressWidget({ el: '#ProgressWidget', model: progressModel });
    var amountBorrowed = new views.AmountBorrowedView({ el: '#amountBorrowed', model: progressModel });
    var loanRelatedInfo = new views.LoanRelatedInfo({el: '#LoanRelatedInfoWidget', model: progressModel });
    var enrollmentStatus = new views.EnrollmentStatusView({el: '#enrollmentStatus', model: progressModel });
    var monthlyPayment = new views.MonthlyPaymentView({el: '#monthlyPaymentID', model: progressModel });
    window.paymentPlans = new views.PaymentPlansView({el: '#paymentPlans', model: new models.WhatYouveLearnedModel() });
    window.postponePayment = new views.PostponePaymentView({el: '#postponePayment', model: new models.WhatYouveLearnedModel() });
    window.loanForgiveness = new views.LoanForgivenessView({el: '#loanForgiveness', model: new models.WhatYouveLearnedModel() });
    window.loanDefault = new views.LoanDefaultView({el: '#loanDefault', model: new models.WhatYouveLearnedModel() });

    //Instantiate a single VLCRouter object to handle page changes
    //THIS IS WHAT MAKES THIS FUNCTION LIKE AN SPA
    window.vlcRouter = new VLCRouter();

    Backbone.history.start({ root: '/Navigator/' });
});
