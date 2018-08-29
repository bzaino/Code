define([
	'jquery',
	'salt',
	'underscore',
	'backbone',
	'modules/CCP/LoanLimits'
], function ($, SALT, _, Backbone, LoanLimits) {

	var models = {
		LoansModel : Backbone.Model.extend({
			defaults: {
				fsl: 0,
				fppl: 0,
				other: 0
			}
		}),
		UserInfoModel : Backbone.Model.extend({
			defaults: {
				maxFsl: 0, //Federal Student Loan
				maxFppl: 0, //Federal Plus Parent loan
				gradeLevel: 1, //freshman
				yearsRemaining : 1,
				costOfAttendancePerYear : 0,
				costOfAttendanceTotal: 0,
				costOfAttendanceLessAdjustmentsTotal: 0,
				grantsPerYear: 0,
				grantsTotal: 0,
				plannedContributionsTotal: 0,
				monthlyInstallmentPerMonth: 0,
				monthlyInstallmentsTotal: 0,
				loansTotal: 0,
                interestTotal: 0,
                showLoansInGraph: false,
                setLoanDefaults: true
			},
			initialize: function () {
				this.on('change:yearsRemaining', this.adjustModel);
				this.on('change:costOfAttendancePerYear', this.adjustModel);
				this.on('change:grantsPerYear', this.adjustModel);
				this.on('change:plannedContributionsTotal', this.adjustModel);
				this.on('change:monthlyInstallmentPerMonth', this.adjustModel);
                SALT.on('change:showLoansInGraph:true', this.setShowLoansInGraphTrue, this);
			},
			adjustModel: function (model) {
				model.set('grantsTotal', model.get('grantsPerYear') * model.get('yearsRemaining'));
				model.set('monthlyInstallmentsTotal', parseInt(model.get('monthlyInstallmentPerMonth') * model.get('yearsRemaining') * 12, 10) || 0);
				//Using compound interest of increasing regularly increasing principal formula: cost * (((1 + rate) ^ years - 1) / rate)
				var unroundedTotal = model.get('costOfAttendancePerYear') * ((Math.pow(1.04, model.get('yearsRemaining')) - 1) / 0.04);
				var total = parseInt(unroundedTotal, 10) || 0;
				model.set('costOfAttendanceTotal', total);

				unroundedTotal = unroundedTotal - (model.get('grantsTotal') + model.get('plannedContributionsTotal') + model.get('monthlyInstallmentsTotal'));
				total = parseInt(unroundedTotal, 10) || 0;
				model.set('costOfAttendanceLessAdjustmentsTotal', total);

				LoanLimits.calculateMaxLoanAmountValues(model);

				if (model.get('setLoanDefaults')) {
					model.set('loansTotal', model.get('maxFsl') + model.get('maxFppl'));
				}

				SALT.trigger('update:headerBoxTotals', model);
				SALT.trigger('change:costOfAttendanceLessAdjustmentsTotal', model);
				SALT.trigger('change:loanTotalsMsg', model);
				SALT.trigger('change:yourPlanMsg');
				if (model.get('setLoanDefaults')) {
					SALT.trigger('reset:loanTotals', model);
				}
				
				SALT.trigger('update:resultsPage', model);
				SALT.trigger('update:graph', model);
			},
            setShowLoansInGraphTrue: function () {
                if (!this.get('showLoansInGraph')) {
                    this.set('showLoansInGraph', true);
                    SALT.trigger('update:graph', this);
                }
            }
		}),
	};

	return models;
});
