define([], function () {
    /* create variables for the strings to make it easier to change */
    var fed = 'federal',
        priv = 'private';
    var map = {
        CL: {loanType: fed, typeName: 'FFEL Consolidated'},
        DU: {loanType: fed, typeName: 'National Defense Loan (Perkins)'},
        D0: {loanType: fed, typeName: 'Direct Stafford Subsidized (SULA ELIGIBLE)'},
        D1: {loanType: fed, typeName: 'Direct Stafford Subsidized'},
        D2: {loanType: fed, typeName: 'Direct Stafford Unsubsidized'},
        D3: {loanType: fed, typeName: 'Direct PLUS Graduate'},
        D4: {loanType: fed, typeName: 'Direct PLUS Parent'},
        D5: {loanType: fed, typeName: 'Direct Consolidated Unsubsidized'},
        D6: {loanType: fed, typeName: 'Direct Consolidated Subsidized'},
        D7: {loanType: fed, typeName: 'Direct PLUS Consolidated'},
        D8: {loanType: fed, typeName: 'Direct Unsubsidized (TEACH)'},
        D9: {loanType: fed, typeName: 'Direct Consolidated Subsidized (SULA ELIGIBLE)'},
        EU: {loanType: fed, typeName: 'Perkins Expanded Lending'},
        FI: {loanType: fed, typeName: 'Federal Insured (FISL)'},
        GB: {loanType: fed, typeName: 'FFEL PLUS Graduate'},
        IC: {loanType: fed, typeName: 'Income Contingent (ICL)'},
        NU: {loanType: fed, typeName: 'National Direct Student Loan (Perkins)'},
        PL: {loanType: fed, typeName: 'FFEL PLUS Parent'},
        PU: {loanType: fed, typeName: 'Federal Perkins'},
        RF: {loanType: fed, typeName: 'FFEL Refinanced'},
        SF: {loanType: fed, typeName: 'FFEL Stafford Subsidized'},
        SL: {loanType: fed, typeName: 'FFEL Supplemental Loan (SLS)'},
        SU: {loanType: fed, typeName: 'FFEL Stafford Unsubsidized'},
        AL: {loanType: priv, typeName: 'Car Loan', trackingString: 'Car Debt'},
        CC: {loanType: priv, typeName: 'Credit Card', trackingString: 'Credit Card Debt'},
        MG: {loanType: priv, typeName: 'Mortgage', trackingString: 'Mortgage Debt'},
        OT: {loanType: priv, typeName: 'Other', trackingString: 'Other Debt'},
        PR: {loanType: priv, typeName: 'Private Student Loan', trackingString: 'Private Student Loan Debt'},
        FD: {loanType: fed, typeName: 'Federal Student Loan', trackingString: 'Fed Student Loan Debt'},
        UE: {loanType: fed, typeName: 'User Estimated'},
        IS: {loanType: priv, typeName: 'State/Institutional Student Loan', trackingString: 'State Debt'},
        HP: {loanType: priv, typeName: 'Healthcare Professionals Student Loan', trackingString: 'HP Debt'},
        SN: {loanType: fed, typeName: 'FFEL Stafford Non-Subsidized'}
    };

    return map;
});