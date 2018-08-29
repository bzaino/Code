define([
    'jquery',
    'backbone',
    'modules/LoanCollection',
    'modules/FinancialStatus/Views',
    'salt',
    'modules/FinancialStatus/LoanTypeMap',
    'underscore',
    'modules/VLC/VLCUtilities',
    'foundation5',
    'modules/myDataUpload'
], function ($, Backbone, collection, Views, SALT, typeMap, _, utilities) {
    var loans = new collection.LoansCollection(),
        gridView = new Views.Grid({collection: loans, el: '#grid-template'}),
        pieGraph = new Views.Pie({collection: loans, el: '#graph-template'}),
        addLoanView = new Views.AddLoan({collection: loans, el: '#add-loans-container'}),
        buttonMenu = new Views.ButtonMenu({ el: '#buttonMenu' }),
        totals = new Views.Totals({ collection: loans, el: '.js-totals-container' }),
        nextSteps = new Views.NextSteps({ collection: loans, el: '#nextSteps' }),
        federalAggregate = new Views.Totals({collection: loans, el: '#FederalAggregate'}),
        privateAggregate = new Views.Totals({collection: loans, el: '#PrivateAggregate'}),
        startPage,
        loanDetail;

    // overriding backbone collection's parse function to exclude Repayment Navigator loans
    loans.parse = collection.parseKWYOloans;

    function loanSelected(id) {
        var model = loans.get(id);
        loans.currentIndex = loans.indexOf(model);
        SALT.trigger('selected:index', loans.currentIndex);
        loanDetail = new Views.LoanDetail({ el: '#loanDetail', model: model });
    }

    function uploadSuccess() {
        $('#js-loanUpload').foundation('reveal', 'close');
        loans.fetch({reset: true});
    }

    $('#js-loanUpload').on('open', function () {
        utilities.openNSLDSwindow('https://www.nslds.ed.gov/nslds_SA/public/SaPrivacyConfirmation.do', 'nslds', 820, 420);
    });

    /* Attach application event listeners to mediate actions between components that shouldn't know about each other */
    SALT.on('loan:selected', loanSelected);
    SALT.on('loanimport:upload:success', uploadSuccess);

    loans.fetch({reset : true, success: function (model, resp, opts) {
        startPage = new Views.StartPage({ collection: loans, el: '#js-start'});
    }});
});
