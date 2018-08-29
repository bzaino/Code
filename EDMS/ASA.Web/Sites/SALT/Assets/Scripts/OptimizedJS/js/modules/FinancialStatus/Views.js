define([
    'modules/FinancialStatus/Grid',
    'modules/FinancialStatus/AddLoan',
    'modules/FinancialStatus/Pie',
    'modules/FinancialStatus/ButtonMenu',
    'modules/FinancialStatus/LoanDetail',
    'modules/FinancialStatus/Totals',
    'modules/FinancialStatus/StartPage',
    'modules/FinancialStatus/NextSteps'
], function (Grid, AddLoan, Pie, ButtonMenu, LoanDetail, Totals, StartPage, NextSteps) {

    /* Create facade object to hold all the different views so that the application can pull in this file,
        rather than all of the views individually */
    return {
        Grid : Grid,
        AddLoan : AddLoan,
        Pie : Pie,
        ButtonMenu : ButtonMenu,
        LoanDetail : LoanDetail,
        Totals : Totals,
        StartPage: StartPage,
        NextSteps: NextSteps
    };

});
