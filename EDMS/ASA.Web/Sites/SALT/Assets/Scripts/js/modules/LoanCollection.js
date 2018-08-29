define([
    'jquery',
    'salt',
    'backbone',
    'modules/sharedModels',
    'modules/FinancialStatus/LoanTypeMap'
], function ($, SALT, Backbone, models, typeMap) {

    var LoansCollection = Backbone.Collection.extend({
        currentIndex: 0,
        model: models.Loan,
        hasNext: function () {
            return (this.currentIndex < (this.models.length - 1));
        },
        hasPrev: function () {
            return (this.currentIndex !== 0);
        },
        next: function () {
            this.currentIndex++;
            return this.at(this.currentIndex);
        },
        prev: function () {
            this.currentIndex--;
            return this.at(this.currentIndex);
        },
        url: '/api/SelfReportedService/restLoans',
        parse: function (response, options) {
            return response.Loans;
        }
    });

    return {
        LoansCollection : LoansCollection,
        parseKWYOloans: function (response, options) {
            //Ignore any loans returned by the SAL that have the User Estimated LoanType, as it means it came from RepaymentNavigator
            var KWYOLoans = [];
            _.each(response.Loans, function (currentModel) {
                currentModel.LoanType = typeMap[currentModel.LoanTypeId].loanType;
                currentModel.TypeName = typeMap[currentModel.LoanTypeId].typeName;
                currentModel.PrincipalBalanceOutstandingAmount = currentModel.PrincipalBalanceOutstandingAmount.toFixed(2);
                currentModel.MonthlyPaymentAmount = currentModel.MonthlyPaymentAmount.toFixed(2);
                currentModel.OriginalLoanAmount = currentModel.OriginalLoanAmount.toFixed(2);
                if (currentModel.LoanTypeId !== 'UE') {
                    KWYOLoans.push(currentModel);
                }
            });
            return KWYOLoans;
        }
    };

});
