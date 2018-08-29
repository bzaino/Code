define([
	'jquery',
	'salt',
	'underscore',
	'backbone',
    'modules/sharedModels'
], function ($, SALT, _, Backbone, sharedModels) {

	var models = {
		Loan : sharedModels.Loan,
		LoansForSAL : sharedModels.LoansForSAL,
		ProgressModel : Backbone.Model.extend({
			defaults: {
				MemberID: 0,
				EstimatedLoanBalance: 'XX,XXX',
				initialMonthlyPayment: 'XX',
				EnrollmentStatus: 'N/A',
				GraduationDate: 'mm/DD/YYYY',
				RepaymentStatus: null,
				MostRecentPaymentDate: null,
				DaysSinceLastPayment: 'N/A',
				AdjustedGrossIncome: 0,
				FamilySize: 0,
				AmountBorrowed: 0,
				Plan: 'Standard',
				TaxFilingStatus: null,
				StateOfResidence: null
			},
			initialize: function () {
				var self = this;
				SALT.on('set:progressModel', function (options) {
					self.set(options);
				});

				this.on('change:AmountBorrowed', this.calculate);
				this.on('change:InterestRate', this.calculate);
				this.on('change:Plan', this.calculate);
				this.on('change:AdjustedGrossIncome', this.calculate);
				this.on('change:FamilySize', this.calculate);
				this.on('change:FilingStatus', this.calculate);
				this.on('change:StateOfResidence', this.calculate);

				this.fetch({ success: function () {
					SALT.trigger('progressModel:fetch:success');
					//After we have set all the model properties we got back from the fetch
					//Attach a listener, any change to the model, call the saveModel function
					self.on('change', self.saveModel);
				}});
			},
			calculate: function (model, value, options) {
				SALT.trigger('calculation:needed', this);
			},
			saveModel: function (model, options) {
				//The progress model changed, save to the back end
				this.save();
			},
			url: '/api/ASAMemberService/Profile'
		}),
		ResponseModel : Backbone.Model.extend({
			defaults: {
				MemberID: 0,
				Question: {
					QuestionID: '',
					QuestionVersion: '',
					QuestionText: ''
				},
				ResponseText: ''
			},
			url: '/api/SurveyService/AddVlcResponse'
		}),
		WhatYouveLearnedModel : Backbone.Model.extend({
			defaults: {
				//repayment options
				RepaymentStandard: 'Go Now',
				RepaymentExtended: 'Go Now',
				RepaymentGraduated: 'Go Now',
				IncomeDriven: 'Go Now',
				Consolidation: 'Go Now',

				// deferment options
				DefermentInSchool: 'Go Now',
				DefermentEducationRelated: 'Go Now',
				DefermentSummerBridge: 'Go Now',
				DefermentPerkins: 'Go Now',
				DefermentRehabilitation: 'Go Now',
				DefermentPlusLoan: 'Go Now',
				DefermentUnemployment: 'Go Now',
				DefermentEconomic: 'Go Now',
				DefermentMilitary: 'Go Now',
				DefermentPeaceCorps: 'Go Now',
				DefermentPostActiveDuty: 'Go Now',

				// forgiveness options
				ForgivenessEducator: 'Go Now',
				ForgivenessHealthcare: 'Go Now',
				ForgivenessMilitary: 'Go Now',
				ForgivenessPublicServant: 'Go Now',
				ForgivenessSTEM: 'Go Now',
				LifeEvent: 'Go Now',

				// default and delinquency options
				Default: 'Go Now',
				Delinquency: 'Go Now'
			}
		})
	};

	return models;

});
