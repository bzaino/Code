define([
    'jquery',
    'salt',
    'underscore',
    'backbone',
    'modules/myDataUpload',
    'modules/VLC/VLCUtilities',
    'dust',
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'configuration',
    'foundation5'
], function ($, SALT, _, Backbone, myDataUpload, utilities, dust, Utility, SiteMember, Configuration) {

    function getDataAndRender(query, self, fire) {
        $('#VLCBody').undelegate();
        $.getJSON(Configuration.apiEndpointBases.GenericEndeca + 'VLC/' + query)
            .done(function (json) {
                SALT.trigger('slide:change', json);
                renderTemplate(query, json, self, fire);
            })
            .fail(function (jqxhr, textStatus, error) {
                console.log('JSON Request Failed--- TextStatus:' + textStatus + ' -- Error: ' + error);
            });
    }

    function getWhatYouveLearnedData(model) {

        SiteMember.done(function (memberModel) {
            var pageViews = _.where(memberModel.ContentInteractions, { TypeID: 3 });
            _.each(pageViews, function (viewObj, idx, arr) {
                model.set(viewObj.MemberContentInteractionValue, 'Visited');
            });
        });
    }

    function renderTemplate(tmpl, data, self, fire) {
        SiteMember.done(function (memberModel) {
            data.SiteMember = memberModel;
            // the enable proactive chat array may not necessarily exist
            if (data.mainContent && data.mainContent[0].records[0].attributes.enable_proactive_chat && 
                data.mainContent[0].records[0].attributes.enable_proactive_chat[0].trim() === "true") {
                lhnInviteEnabled = 1;
                lhnInviteChime = 1;
            } else {
                lhnInviteEnabled = 0;
                lhnInviteChime = 0;
            }
            Utility.renderDustTemplate('VLC/' + tmpl, data, function (err, out) {
                self.$el.html(out);
                if (fire) {
                    SALT.trigger('vlc:template:rendered');
                }
                SALT.trigger('rendered:' + tmpl);
            });
        });
    }

    var views = {

        AmountBorrowedView: Backbone.View.extend({
            initialize: function () {
                SALT.trigger('getLoansAndSetModels');
                this.model.on('change:AmountBorrowed', this.render, this);
                this.render();
                // to show "Add loan" button when show:addLoansbutton is triggered
                var showButton = function () {
                    if (this.model.get('AmountBorrowed') === 0) {
                        $('#addLoans').show();
                    }
                };
                var bindShowButton = _.bind(showButton, this);
                SALT.on('show:addLoansbutton', bindShowButton);
            },
            render: function () {
                renderTemplate('AmountBorrowed', this.model.toJSON(), this);
                if (this.model.get('AmountBorrowed') === 0) {
                    $('#addLoans').show();
                } else {
                    $('#addLoans').hide();
                }
            }
        }),

        ApplicationView: Backbone.View.extend({
            events: {
                'click .answer': 'saveAnswers',
                'click #backButton': 'goBack',
                'click #startOver': 'startOverClicked',
                'click .WidgetsButton': 'mobileWidgetToggle'
            },
            saveAnswers: function (e) {
                SALT.trigger('saveAnswers:called', e);
            },
            goBack: function () {
                history.back();
            },
            startOverClicked: function () {
                SALT.trigger('navigate', '');
            }
        }),

        ContextualHelp: Backbone.View.extend({
            initialize: function () {
                SALT.on('slide:change', this.render, this);
            },
            render: function (data) {
                SALT.on('rendered:ContextualHelp', function () {
                    $(document).foundation();
                });
                renderTemplate('ContextualHelp', data, this);
            }
        }),

        DoYouKnowHowMuchView: Backbone.View.extend({
            events: {
                'click #saveDebt': 'validateForm',
                'click #skipButton': 'skipSlide',
                'valid #how-much-form': 'setHowMuch'
            },
            initialize: function () {
                $('#Question').empty();
                getDataAndRender('DoYouKnowHowMuch', this, true);
                SALT.once('vlc:template:rendered', this.handleButtons, this);
                SALT.trigger('vlcWT:fire', 'start');
            },
            validateForm: function (e) {
                $('#how-much-form').one('invalid', function () {
                    e.preventDefault();
                    e.stopPropagation();
                });
                $('#how-much-form').trigger('submit');
            },
            setHowMuch: function () {
                //The user estimating a loan should trigger setting a todo to "In Progress"
                SALT.trigger('content:todo:inProgress', {contentId: '101-8645'});
                SALT.trigger('setHowMuch');
            },
            handleButtons: function () {
                // hide the "add loans" button if user has no loans
                // then show it when user leaves this slide without adding loans
                $('#addLoans').hide();
                // call showAddButton function once when we change the current slide
                SALT.once('slide:change', this.showAddButton);

                SALT.trigger('showSkip');
                this.$el.foundation();
            },
            skipSlide: function () {
                SALT.trigger('vlcWT:fire', 'complete');
                var targetPage = $('#saveDebt').attr('href');
                targetPage = targetPage.substr(targetPage.indexOf('#') + 1);
                SALT.trigger('navigate', targetPage);
            },
            showAddButton : function () {
                // if user didn't add loans keep showing the "add Loans" button
                SALT.trigger('show:addLoansbutton');
            }
        }),

        EnrollmentStatusView: Backbone.View.extend({
            initialize: function () {
                this.model.on('change:EnrollmentStatus', this.render, this);
                this.render();
            },
            render: function () {
                renderTemplate('EnrollmentStatus', this.model.toJSON(), this);
            }
        }),

        FeaturedContent: Backbone.View.extend({
            initialize: function () {
                //Attach event listener to call this view's render method when the slide changes
                SALT.on('slide:change', this.render, this);
            },
            render: function (data) {
                renderTemplate('FeaturedContent', data, this);
            }
        }),

        FindPin: Backbone.View.extend({
            events: {
                'click #startOver': 'startOverClicked'
            },
            initialize: function () {
                $('#threeColumn').hide();
                getDataAndRender('FindPin', this);
            }
        }),

        HiddenFieldsView: Backbone.View.extend({
            initialize: function () {
                SALT.on('slide:change', this.render, this);
            },
            render: function (data) {
                renderTemplate('HiddenFields', data, this);
            }
        }),

        IncomeBasedForm: Backbone.View.extend({
            events: {
                'click #IBRForm': 'validateForm',
                'valid #repaymentHHIInWidget': 'saveIBRinputs'
            },
            initialize: function () {
                this.render();
                this.$el.undelegate();
            },
            render: function () {
                renderTemplate('IncomeBasedForm', {}, this);
            },
            validateForm: function (e) {
                e.preventDefault();
                this.$el.foundation();
                $('#repaymentHHIInWidget').one('invalid', function () {
                    e.preventDefault();
                    e.stopPropagation();
                });
                $('#repaymentHHIInWidget').trigger('submit');
            },
            saveIBRinputs: function () {
                var monthlyIncome = $('#monthlyIncomeInWidget').val();
                var householdSize = parseInt($('#numberOfPeopleInWidget').val(), 10);
                var stateOfResidence = $('#stateOfResidenceInWidget').val();
                var filingStatus = $('#filingStatusInWidget').val();
                SALT.trigger('vlcWT:fire', 'start');
                SALT.trigger('set:progressModel', {FamilySize: householdSize, AdjustedGrossIncome: monthlyIncome, TaxFilingStatus: filingStatus, StateOfResidence: stateOfResidence });
                $('#IncomeBasedForm').html('');
            }
        }),

        InSchoolYNView: Backbone.View.extend({
            events: {
                'click .inschool': 'setEnrollmentCurrent',
                'click .outofschool': 'setEnrollmentOut'
            },
            initialize: function () {
                getDataAndRender('InSchoolYN', this);
            },
            setEnrollmentCurrent: function () {
                SALT.trigger('set:progressModel', { EnrollmentStatus: 'Enrolled', RepaymentStatus: 'N/A'});
            },
            setEnrollmentOut: function () {
                SALT.trigger('set:progressModel', { EnrollmentStatus : 'Out Of School'});
            }
        }),

        InRepaymentView: Backbone.View.extend({
            events: {
                'click .current': 'setCurrent',
                'click .notYet' : 'setGrace'
            },
            initialize: function () {
                getDataAndRender('InRepayment', this);
            },
            setCurrent: function () {
                SALT.trigger('set:progressModel', { RepaymentStatus : 'Current'});
            },
            setGrace: function () {
                SALT.trigger('set:progressModel', { RepaymentStatus : 'Grace'});
            }
        }),

        LastAttendedView: Backbone.View.extend({
            events: {
                'click .saveDate': 'saveDate'
            },
            initialize: function () {
                getDataAndRender('LastAttended', this);
                SALT.trigger('vlcWT:fire', 'start');
            },
            saveDate: function (e) {
                SALT.trigger('vlcWT:fire', 'complete');
                var userSelectedDate = utilities.extractDropdownDate();
                var dateSixMonthsAgo = new Date.now().addMonths(-6);
                var LastAttendedDate = new Date(userSelectedDate.month + '/' + userSelectedDate.day + '/' + userSelectedDate.year);
                var theDate = utilities.prepareDate(userSelectedDate.day, userSelectedDate.month, userSelectedDate.year);
                SALT.trigger('set:progressModel', {GraduationDate: theDate});

                if (LastAttendedDate > dateSixMonthsAgo) {
                    //Date is within 6 months, prevent default behavior, and route to new destination
                    e.preventDefault();
                    e.stopPropagation();
                    SALT.trigger('navigate', 'DoYouKnowHowMuch');
                }
            }
        }),

        LastPaid270View: Backbone.View.extend({
            events: {
                'click .yesAnswer': 'yesAnswer',
                'click .noAnswer': 'noAnswer'
            },
            initialize: function () {
                getDataAndRender('LastPaid270', this);
            },
            yesAnswer: function () {
                SALT.trigger('set:progressModel', {RepaymentStatus: 'Default'});
            },
            noAnswer: function () {
                SALT.trigger('set:progressModel', {RepaymentStatus: 'Delinquent'});
            }
        }),

        LoanForgivenessView : Backbone.View.extend({
            events: {
                'click .scroll-top' : 'scrollTopView'
            },

            initialize: function () {
                getWhatYouveLearnedData(this.model);
                this.model.on('change', this.render, this);
                this.render();
            },

            render: function () {
                renderTemplate('LoanForgiveness', this.model.toJSON(), this);
            },

            scrollTopView: function () {
                Utility.topScroll();
            }
        }),

        LoanDefaultView : Backbone.View.extend({
            events: {
                'click .scroll-top' : 'scrollTopView'
            },

            initialize: function () {
                getWhatYouveLearnedData(this.model);
                this.model.on('change', this.render, this);
                this.render();
            },

            render: function () {
                renderTemplate('LoanDefault', this.model.toJSON(), this);
            },

            scrollTopView: function () {
                Utility.topScroll();
            }
        }),

        LoanImportView: Backbone.View.extend({
            events: {
                'click #notrightnow': 'exitLoanImport',
                'click #startOver': 'startOverClicked'
            },
            initialize: function () {
                $('#threeColumn').hide();
                getDataAndRender('LoanImport', this);
            },
            exitLoanImport: function () {
                this.$el.html('');
                $('#threeColumn').show();
            },
            startOverClicked: function () {
                SALT.trigger('navigate', '');
            }
        }),

        LoanRelatedInfo: Backbone.View.extend({
            initialize: function () {
                this.model.on('change:GraduationDate', this.render, this);
                this.model.on('change:EnrollmentStatus', this.render, this);
                this.render();
            },
            render: function () {
                renderTemplate('LoanRelatedInfoWidget', this.model.toJSON(), this);
                var $gradLink = $('#gradLink');
                if (this.model.get('EnrollmentStatus') === 'Out Of School') {
                    $gradLink.attr('href', '#LastAttended');
                } else {
                    $gradLink.attr('href', '#WhenWillYouGrad');
                }
            }
        }),

        LoanUploadView: Backbone.View.extend({
            events: {
                'click #startOver': 'startOverClicked'
            },
            initialize: function () {
                $('#threeColumn').hide();
                SALT.on('loanimport:upload:success', this.setLoanData, this);
                SALT.once('vlc:template:rendered', this.attachEvents);
                getDataAndRender('LoanUpload', this, true);
                utilities.openNSLDSwindow('https://www.nslds.ed.gov/nslds_SA/public/SaPrivacyConfirmation.do', 'nslds', 820, 420);
            },
            setLoanData: function () {
                SALT.trigger('getLoansAndSetModels');
                this.$el.html('');
                $('#threeColumn').show();
                SALT.trigger('navigate', 'ManageablePaymentYN');
            },
            attachEvents: function () {
                myDataUpload.init();
            },
            startOverClicked: function () {
                SALT.trigger('navigate', '');
            }
        }),

        ManageablePaymentYNView: Backbone.View.extend({
            initialize: function () {
                SALT.once('vlc:template:rendered', this.setUiFields);
                SALT.trigger('ManageablePaymentYNView:initialize', this);
                getDataAndRender('ManageablePaymentYN', this, true);

                //If NOT in mobile, Check if the "My Loans At A Glance Accordion" is open, if not, open it (SWD-4443)
                if (window.innerWidth > 767 && !$('#js-myloans-accordion').hasClass('active')) {
                    $('#js-myloans-accordion-btn').click();
                }
            },
            setUiFields: function () {
                SALT.trigger('ManageablePaymentYNView:setUiFields');
            }
        }),

        MonthlyPaymentView: Backbone.View.extend({
            initialize: function () {
                SALT.on('progressModel:fetch:success', this.render, this);
                this.model.on('change', this.render, this);
                this.render();
            },

            render: function () {
                renderTemplate('MonthlyPayment', this.model.toJSON(), this);
            }
        }),

        ProgressWidget: Backbone.View.extend({
            events: {
                'change #planSelect': 'selectPlan'
            },
            initialize: function () {
                //Attach listener, when model changes, progress widget will re render
                SALT.on('progressModel:fetch:success', this.render, this);
                this.model.on('change', this.render, this);
            },
            render: function () {
                renderTemplate('ProgressWidget', this.model.toJSON(), this);

                //show the proper disclaimer
                var disclaimerPlan = this.model.get('Plan').replace(/\s/g, '').toLowerCase();
                $('#widget-disclaimers').children().hide();
                $('#' + 'disclaimer-' + disclaimerPlan).show();

            },
            selectPlan: function (e) {
                var plan = $(e.target).val();
                SALT.trigger('set:progressModel', { Plan : plan});

                if (plan === 'Income Based') {
                    SALT.trigger('show:ibrForm');
                } else {
                    $('#IncomeBasedForm').html('');
                }
            }
        }),

        QuestionHeadline: Backbone.View.extend({
            initialize: function () {
            //Attach event listener to call this view's render method when the slide changes
                SALT.on('slide:change', this.render, this);
            },
            render: function (data) {
                renderTemplate('QuestionHeadline', data, this);
            }
        }),

        PaymentPlansView : Backbone.View.extend({
            events: {
                'click .scroll-top' : 'scrollTopView'
            },
            initialize: function () {
                getWhatYouveLearnedData(this.model);
                this.model.on('change', this.render, this);
                this.render();
            },

            render: function () {
                renderTemplate('PaymentPlans', this.model.toJSON(), this);
            },

            scrollTopView: function () {
                Utility.topScroll();
            }
        }),

        PostponePaymentView : Backbone.View.extend({
            events: {
                'click .scroll-top' : 'scrollTopView'
            },
            initialize: function () {
                getWhatYouveLearnedData(this.model);
                this.model.on('change', this.render, this);
                this.render();
            },
            render: function () {
                renderTemplate('PostponePayment', this.model.toJSON(), this);
            },

            scrollTopView: function () {
                Utility.topScroll();
            }
        }),

        WhenWillYouGradView: Backbone.View.extend({
            events: {
                'click .saveDate': 'saveDate'
            },
            initialize: function () {
                getDataAndRender('WhenWillYouGrad', this);
                SALT.trigger('vlcWT:fire', 'start');
            },
            saveDate: function () {
                SALT.trigger('vlcWT:fire', 'complete');
                var userSelectedDate = utilities.extractDropdownDate();
                var theDate = utilities.prepareDate(userSelectedDate.day, userSelectedDate.month, userSelectedDate.year);
                SALT.trigger('set:progressModel', {GraduationDate: theDate});
            }
        }),

        VLCBody: Backbone.View.extend({
            initialize: function () {
                SALT.on('slide:multiplechoice', this.render, this);
            },
            render: function (data) {
                renderTemplate('VLCBody', data, this);
            }
        })

    };

    SALT.on('show:ibrForm', function () {
        var incomeFormView = new views.IncomeBasedForm({
            el: '#IncomeBasedForm'
        });
    });

    return views;

});
