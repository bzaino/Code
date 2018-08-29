/*global describe, it, chai, sinon */
define([
    'Tools/savingsCalculator'
], function (savingsCalculator) {

    var assert = chai.assert;
    var expect = chai.expect;

    describe('savingsCalculator', function() {
        describe('calculateInterest', function() {

            it('It calculates the interest based on the totals supplied', function () {
                
                var totals = {
                    currentAmount : 100,
                    totalInterest : 6.5,
                    interestRate: .05
                }

                var values = {
                    monthlyDeposit : 30
                }

                returnedtotal = savingsCalculator.global.utils.calculateInterest(totals, values);
                assert.equal(5, returnedtotal.currentInterest, 'Interest rate was different than expected');
                assert.equal(135, returnedtotal.currentAmount, 'Current amount was different than expected');
                assert.equal(11.5, returnedtotal.totalInterest, 'Total interest was different than expected');
            });
        });
            describe('setMonthlyPayment', function() {

            it('It calculates the monthly payment', function () {
                
                var values = {
                    schedule : 'week',
                    payment: 10,
                    monthlyDeposit: 20
                };
                var errorMessage = 'Monthly deposit was different than expected';
                pathToFunction = savingsCalculator.global.utils.setMonthlyPayment;

                returnedValues = pathToFunction(values);
                assert.equal(40, returnedValues.monthlyDeposit, errorMessage);

                values.schedule = 'two-weeks';
                returnedValues = pathToFunction(values);
                assert.equal(20, returnedValues.monthlyDeposit, errorMessage);

                values.schedule = 'month';
                returnedValues = pathToFunction(values);
                assert.equal(10, returnedValues.monthlyDeposit, errorMessage);

                values.schedule = 'three-months';
                returnedValues = pathToFunction(values);
                assert.equal(3.3333333333333335, returnedValues.monthlyDeposit, errorMessage);

                values.schedule = 'six-months';
                returnedValues = pathToFunction(values);
                assert.equal(1.6666666666666667, returnedValues.monthlyDeposit, errorMessage);

                values.schedule = 'year';
                returnedValues = pathToFunction(values);
                assert.equal(0.8333333333333334, returnedValues.monthlyDeposit, errorMessage);
             });
         });
                describe('reset', function() {

                it('Removes white space from inputs', function () {
                    var pathToFunction = savingsCalculator.global.utils.reset;
                    returnedValue = pathToFunction('');
                    
                    assert.equal(returnedValue, true);
                    
                    returnedValue = pathToFunction(' ');
                    assert.equal(returnedValue, true);
                    
                    returnedValue = pathToFunction('noSpaces');
                    assert.notEqual(returnedValue, true);
            });
        });
    });
});