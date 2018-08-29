define([], function () {

    // loan types
    var loanType = {
        FEDERAL: 'federal',
        PERKINS: 'perkins',
        PRIVATE: 'private'
    };

    //calculateStandardMonthlyPayment
    function calculateStandardMonthlyPayment(loansArray) { // T5.8.2  D18
        var total = 0;
        for (var i = 0; i < loansArray.length; i++) {
            var loan = loansArray[i];
            if (!isNaN(loan.MonthlyPaymentAmount)) {
                total += loan.MonthlyPaymentAmount;
            }
        }
        // QC 4005 - if monthly payment amount is less than 50, per fed regulations make min payment 50.
        var federalLoans = getLoansByType(loansArray, loanType.FEDERAL);
        if ((federalLoans && federalLoans.length !== 0) && total !== 0 && total < 50) {
            total = 50;
        }
        var payment = { initial : total };
        return payment;
    }

    // Federal Loan Extended Monthly Payment
    function calculateExtendedMonthlyPayment(loansArray) { // T5.9.5 D19
        var total = 0;
        var federalLoans = getLoansByType(loansArray, loanType.FEDERAL);

        for (var i = 0; i < federalLoans.length; i++) {
            var loan = federalLoans[i];
            if ((parseInt(loan.InterestRate, 10)) === 0) { //QC 6032, QC-6377
                total += (loan.OriginalLoanAmount) / (25 * 12);
            } else {
                total += ((loan.OriginalLoanAmount * ((loan.InterestRate / 100) / 12)) / (1 - Math.pow((1 + ((loan.InterestRate / 100) / 12)), -(25 * 12))));
            }

        }
        var payment = { initial : total };

        return payment;
    }

    // Federal Loan Graduated Monthly Payments
    // * Assuming paying interest only at the first 2 years + 8 Years of standard repayment
    function calculateGraduatedMonthlyPayment(loansArray) {

        var total = { initial : 0 };
        var federalLoans = getLoansByType(loansArray, loanType.FEDERAL);

        for (var i = 0; i < federalLoans.length; i++) {
            var loan = federalLoans[i];
             //QC 6032 If interest is 0% then there is no payment due the first 2 years.
            if ((parseInt(loan.InterestRate, 10)) === 0) { //QC-6377
                total.initial += 0;
            } else {
                total.initial += ((loan.OriginalLoanAmount * ((loan.InterestRate / 100) / 12)));
            }
        }
        return total;
    }

    // 2013 Federal Loan Income Based Monthly Payment
    function calculateIncomeBasedMonthlyPayment(loansArray, income, familySize, filingStatus, stateOfResidence) {
        var total = { initial : 0 };

        if (!income || !familySize || !filingStatus || !stateOfResidence) {
            return total;
        }
        //Calculate standard payment
        var standardPayment = calculateStandardMonthlyPayment(loansArray);
        var discretionaryIncome = 0;
        if (stateOfResidence === 'OTHER') {
            discretionaryIncome = 0.15 * (income - (11730 + (6480 * familySize))) / 12;
        } else if (stateOfResidence === 'AK') {
            discretionaryIncome = 0.15 * (income - (14670 + (8100 * familySize))) / 12;
        } else if (stateOfResidence === 'HI') {
            discretionaryIncome = 0.15 * (income - (13485 + (7455 * familySize))) / 12;
        }

        if (discretionaryIncome <= 0) {
            return total;
        } else if (discretionaryIncome > 0 && discretionaryIncome <= 5) {
            total = { initial : 5 };
            return total;
        } else if (discretionaryIncome > 5 && discretionaryIncome <= 10) {
            total = { initial : 10 };
            return total;
        } else {
            if (discretionaryIncome < standardPayment.initial) {
                //return the lesser of both standard repayment and discretionaryIncome
                total.initial = discretionaryIncome;
                return total;
            } else {
                return standardPayment;
            }
        }
    }

    // Filter loan by loan type, i.e. SALT.loanType.FEDERAL
    function getLoansByType(loansArray, type) {
        var loansByType = [];

        for (var i = 0; i < loansArray.length; i++) {
            var loan = loansArray[i];
            if (loan.LoanType === type) {
                loansByType.push(loan);
            }
        }

        return loansByType;
    }

    // Facade function to choose which calculation to call based on planType parameter
    function calculationChooser(planType, loanArray, model) {
        var income;

        if (planType === 'Standard') {
            return calculateStandardMonthlyPayment(loanArray);
        } else if (planType === 'Income Based') {
            income = parseInt(model.AdjustedGrossIncome, 10);
            return calculateIncomeBasedMonthlyPayment(loanArray, income, model.FamilySize, model.TaxFilingStatus, model.StateOfResidence);
        } else if (planType === 'Graduated') {
            return calculateGraduatedMonthlyPayment(loanArray);
        } else if (planType === 'Extended') {
            return calculateExtendedMonthlyPayment(loanArray);
        }
    }

    function calculateSimpleAmortizedPayment(interestRate, totalBalance, totalNumberOfPayments) {
        var paymentObj = {};

        var monthlyPayment = solveForMonthlyPaymentFormula(interestRate, totalBalance, totalNumberOfPayments);
        
        var modifiedMonthlyPayment = monthlyPayment >= 50 ? Math.round(monthlyPayment) : 50;
        paymentObj.monthly = modifiedMonthlyPayment;
        paymentObj.annual = modifiedMonthlyPayment * 12;
        paymentObj.totalInterest = (paymentObj.annual * (totalNumberOfPayments / 12)) - totalBalance;
        return paymentObj;
    }

    function solveForMonthlyPaymentFormula(interestRate, totalBalance, totalNumberOfPayments) {
        /**
        calculated using: http://www.myamortizationchart.com/articles/how-is-an-amortization-schedule-calculated/
        */
        var monthlyInterestRate  = interestRate / 12;
        var monthlyPaymentNumerator   = monthlyInterestRate * totalBalance * Math.pow(1 + monthlyInterestRate, totalNumberOfPayments);
        var monthlyPaymentDenominator = Math.pow(1 + monthlyInterestRate, totalNumberOfPayments) - 1;
        var newMonthlyPayment = parseFloat((monthlyPaymentNumerator / monthlyPaymentDenominator).toFixed(2));
        return newMonthlyPayment;
    }


    //Return only the calculationChooser as a public function
    return {calculationChooser : calculationChooser, calculateSimpleAmortizedPayment : calculateSimpleAmortizedPayment};
});
