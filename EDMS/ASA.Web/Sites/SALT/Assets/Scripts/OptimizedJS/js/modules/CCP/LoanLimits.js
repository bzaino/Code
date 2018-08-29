// jshint maxstatements:27
define([], function () {
    var gradeLevelIndex = [
            [0], //graduate
            [1], //freshman
            [2], //sophomore
            [3], //junior
            [4]  //senior
        ];

    //maximum years to Fund by gradelevel
    var maxYearsToFund = [
            [4], //graduate
            [6], //freshman
            [5], //sophomore
            [4], //junior
            [3]  //senior
        ];

    //maximum Federal amounts by gradelevel by year
    var graduateRow0 = [20500, 41000, 61500, 82000, 0, 0],
        freshmanRow1 = [5500, 12000, 19500, 27000, 31000, 31000],
        sophomoreRow2 = [6500, 14000, 21500, 29000, 31000, 0],
        juniorRow3 = [7500, 15000, 22500, 30000, 0, 0],
        seniorRow4 = [7500, 15000, 22500, 0, 0, 0];

    var maxFederalAmountByGradeLevelByYear = [graduateRow0, freshmanRow1, sophomoreRow2, juniorRow3, seniorRow4];

    function getGradeLevelIndex(gradeLevel) {
        return parseInt(gradeLevelIndex[gradeLevel], 10) || 0;
    }
    function getMaxYearsToFund(gradeLevel) {
        return parseInt(maxYearsToFund[gradeLevel], 10) || 0;
    }
    function getMaxFederalAmountByGradeLevelByYear(gradeLevel, yearsToFund) {
        var maxAmount = 0;
        var gradeLevelIndex = getGradeLevelIndex(gradeLevel);

        maxAmount = parseInt(maxFederalAmountByGradeLevelByYear[gradeLevelIndex][(yearsToFund - 1)], 10);
        return maxAmount;
    }
    function calculateMaxLoanAmountValues(userInfo) {
        setMaxFslAmount(userInfo);
        setMaxFpplAmount(userInfo);
    }
    function setMaxFslAmount(userInfo) {
        var maxLoanAmount = getMaxFederalAmountByGradeLevelByYear(userInfo.get('gradeLevel'), userInfo.get('yearsRemaining'));
        var unMetNeedAmount = userInfo.get('costOfAttendanceLessAdjustmentsTotal');
        if (unMetNeedAmount <= 0) {
            userInfo.set('maxFsl', 0);
        } else if (maxLoanAmount > unMetNeedAmount) {
            userInfo.set('maxFsl', unMetNeedAmount);
        } else {
            userInfo.set('maxFsl', maxLoanAmount);
        }
    }
    function setMaxFpplAmount(userInfo) {
        var maxFslAmount = userInfo.get('maxFsl');
        var unMetNeedAmount = userInfo.get('costOfAttendanceLessAdjustmentsTotal');
        var remainingAmount = unMetNeedAmount - maxFslAmount;
        if (remainingAmount > 0) {
            userInfo.set('maxFppl', remainingAmount);
        } else {
            userInfo.set('maxFppl', 0);
        }
    }

    function setTotalsFromFsl(userInfo, formValues, increment, newValues) {
        var maxFslAmount = userInfo.get('maxFsl');
        var incrementValue = parseInt(increment, 10);
        var unMetNeedAmount = userInfo.get('costOfAttendanceLessAdjustmentsTotal');

        if (formValues.get('fsl') + incrementValue < maxFslAmount) {
            newValues.fsl = formValues.get('fsl') + incrementValue;
            if (newValues.fsl < 0) {
                incrementValue = incrementValue + newValues.fsl;
                newValues.fsl = 0;
            }
            newValues.fppl = unMetNeedAmount - newValues.fsl - formValues.get('other');
            if (newValues.fppl < 0) {
                newValues.other = formValues.get('other') + newValues.fppl;
                newValues.fppl = 0;
            }
            else {
                newValues.other = formValues.get('other');
            }
        } else {
            var tempFppl = formValues.get('fppl') - (incrementValue - formValues.get('fsl'));
            if (tempFppl < 0) {
                newValues.fppl = 0;
                newValues.other = formValues.get('other') + tempFppl;

                if (newValues.other < 0) {
                    newValues.fsl = maxFslAmount;
                    newValues.other = unMetNeedAmount - maxFslAmount;
                } else {
                    newValues.fsl = unMetNeedAmount - newValues.fppl - newValues.other;
                    if (newValues.fsl > maxFslAmount) {
                        newValues.fsl = maxFslAmount;
                        newValues.other = unMetNeedAmount - maxFslAmount - newValues.fppl;
                    }
                }
            }
            else {
                newValues.fsl = maxFslAmount;
                newValues.fppl = unMetNeedAmount - maxFslAmount - formValues.get('other') < 0 ? 0 : unMetNeedAmount - maxFslAmount - formValues.get('other');
                newValues.other = unMetNeedAmount - maxFslAmount - newValues.fppl;
            }
        }
    }
    function setTotalsFromFppl(userInfo, formValues, increment, newValues) {
        var maxFslAmount = userInfo.get('maxFsl');
        var incrementValue = parseInt(increment, 10);
        var unMetNeedAmount = userInfo.get('costOfAttendanceLessAdjustmentsTotal');

        if (incrementValue > 0) { //increase-plus
            if (formValues.get('other') === 0 || formValues.get('other') >= incrementValue) {
                if (formValues.get('other') > 0) {
                    newValues.other = formValues.get('other') - incrementValue;
                    if (newValues.other < 0) {
                        incrementValue = incrementValue + newValues.other;
                        newValues.other = 0;
                    }
                }

                newValues.fppl = formValues.get('fppl') + incrementValue;
                newValues.fsl = unMetNeedAmount - newValues.other - newValues.fppl;

                if (newValues.fsl < 0) {
                    newValues.fppl = newValues.fppl + newValues.fsl;
                    newValues.fsl = 0;
                }
            } else {
                newValues.other = 0;
                newValues.fppl = formValues.get('fppl') + incrementValue > unMetNeedAmount ? unMetNeedAmount : formValues.get('fppl') + incrementValue;
                newValues.fsl = unMetNeedAmount - newValues.fppl;
            }
        } else {
            newValues.fsl = formValues.get('fsl');
            newValues.fppl = formValues.get('fppl') + incrementValue;
            if (newValues.fppl < 0) {
                incrementValue = incrementValue - newValues.fppl;
                newValues.fppl = 0;
            }
            newValues.other = formValues.get('other') - incrementValue;
        }
    }
    function setTotalsFromOther(userInfo, formValues, increment, newValues) {
        var maxFslAmount = userInfo.get('maxFsl');
        var incrementValue = parseInt(increment, 10);
        var unMetNeedAmount = userInfo.get('costOfAttendanceLessAdjustmentsTotal');

        if (incrementValue > 0) { //increase-other/private
            if (formValues.get('fppl') === 0 || formValues.get('fppl') >= incrementValue) {
                if (formValues.get('fppl') === 0) {
                    newValues.fppl = 0;
                    newValues.fsl = formValues.get('other') + incrementValue > unMetNeedAmount ? 0 : formValues.get('fsl') - incrementValue;
                } else {
                    newValues.fppl = formValues.get('fppl') - incrementValue;
                    newValues.fsl = formValues.get('fsl');
                }

                newValues.other = unMetNeedAmount - newValues.fsl - newValues.fppl;
            } else {
                newValues.fppl = 0;
                newValues.other = formValues.get('other') + incrementValue > unMetNeedAmount ? unMetNeedAmount : formValues.get('other') + incrementValue;
                newValues.fsl = unMetNeedAmount - newValues.other;
            }
        } else {
            newValues.fsl = formValues.get('fsl');
            newValues.other = formValues.get('other') + incrementValue;
            if (newValues.other < 0) {
                incrementValue = incrementValue - newValues.other;
                newValues.other = 0;
            }
            newValues.fppl = formValues.get('fppl') - incrementValue;
        }
    }
    function calculateLoanDisplayTotals(loanType, userInfo, formValues, increment) {

        var newValues = {
            fsl: 0,
            fppl: 0,
            other: 0
        };

        if (loanType === 'fsl') {
            setTotalsFromFsl(userInfo, formValues, increment, newValues);
        }

        if (loanType === 'fppl') {
            setTotalsFromFppl(userInfo, formValues, increment, newValues);
        }

        if (loanType === 'other') {
            setTotalsFromOther(userInfo, formValues, increment, newValues);
        }

        return newValues;
    }
    function determineYourPlanMessage(userInfo, totalLoansModel) {
        var mostDependentTypeToDisplay = 'undefined';
        var highestLoanAmount = 0;

        var fsl = totalLoansModel.get('fsl');
        var fppl = totalLoansModel.get('fppl');
        var other = totalLoansModel.get('other');
        var grants = userInfo.get('grantsTotal');
        var monthlyInstallments = userInfo.get('monthlyInstallmentsTotal');

        if (fsl > 0 && (fsl > fppl && fsl > other)) {
            mostDependentTypeToDisplay = 'FSL';
            highestLoanAmount = fsl;
        }
        if (fppl > 0 && (fppl > fsl && fppl > other)) {
            mostDependentTypeToDisplay = userInfo.get('gradeLevel') === 0 ? 'GradPlusLoan' : 'PlusLoan';
            highestLoanAmount = fppl;
        }
        if (other > 0 && (other > fsl && other > fppl)) {
            mostDependentTypeToDisplay = 'PrivateLoan';
            highestLoanAmount = other;
        }

        if ((grants > 0 && (grants > highestLoanAmount)) || (monthlyInstallments > 0 && (monthlyInstallments > highestLoanAmount))) {
            mostDependentTypeToDisplay = 'Grants';
        }

        return mostDependentTypeToDisplay;
    }

    //Return only the functions needed to be public functions
    return {
        calculateMaxLoanAmountValues : calculateMaxLoanAmountValues,
        getMaxFederalAmountByGradeLevelByYear: getMaxFederalAmountByGradeLevelByYear,
        getMaxYearsToFund: getMaxYearsToFund,
        calculateLoanDisplayTotals: calculateLoanDisplayTotals,
        determineYourPlanMessage: determineYourPlanMessage
    };
});
