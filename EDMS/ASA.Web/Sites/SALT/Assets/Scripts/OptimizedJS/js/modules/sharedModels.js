define([
    'backbone'
], function (Backbone) {
    return {
        Loan: Backbone.Model.extend({
            idAttribute: 'LoanSelfReportedEntryId',
            defaults: {
                DateAdded: '\/Date(1378930055733-0400)\/',
                OriginalLoanDate: '\/Date(1378930055733-0400)\/',
                ErrorList: [],
                LoanSelfReportedEntryId: null,
                RedirectURL: '',
                InterestRate: 4.45,
                LoanSource: 'Member',
                LoanStatusId: '',
                LoanTerm: '10',
                LoanTypeId: 'UE',
                OriginalLoanAmount: 0,
                PrincipalBalanceOutstandingAmount: 0,
                ReceivedYear: 0,
                MonthlyPaymentAmount: null,
                LoanName: 'FooName',
                LoanType: 'federal',
                TypeName: '',
                RecordSourceId: 2,
                ServicingOrganizationName: '',
                Color: '#18093e',
                LastModified: '\/Date(1111111111111-0400)\/'
            },
            urlRoot: '/api/SelfReportedService/restLoans'
        }),
        LoansForSAL: Backbone.Model.extend({
            defaults: {
                ErrorList: [],
                RedirectURL: '',
                Loans: []
            },
            url: '/api/SelfReportedService/SaveList'
        })
    };
});
