/*global describe, before, it, chai, sinon */
define([
    'modules/FinancialStatus/Totals',
    'modules/LoanCollection',
    'modules/sharedModels'
], function(Totals, LoanCollection, Models) {

    /* default to assert style, replace this if you prefer 'should' or 'expect'. */
    var assert = chai.assert;

    /*  What Im testing
        View that calculates and displays sum of Loan Balances and monthly payments
    */


    describe('Totals View', function () {
        before(function() {
            this.totals = new Totals({ collection: new LoanCollection.LoansCollection([ new Models.Loan({ PrincipalBalanceOutstandingAmount: 500, MonthlyPaymentAmount: 250 }), new Models.Loan({ PrincipalBalanceOutstandingAmount: 1500, MonthlyPaymentAmount: 1000 }) ]) });
        });

        it('Should return sum of collection`s balances', function () {
            var balancesSum = this.totals.calculate('PrincipalBalanceOutstandingAmount');
            assert.equal(2000, balancesSum.total);
        });

        it('Should return sum of collection`s monthly payments', function () {
            var monthlyPayment = this.totals.calculate('MonthlyPaymentAmount');
            assert.equal(1250, monthlyPayment.total);
        });

        it('Should return sum of collection`s monthly payments rounded to two decimal places', function () {
            var monthlyPayment = this.totals.calculate('MonthlyPaymentAmount');
            assert.strictEqual('1250.00', monthlyPayment.total);
            assert.notEqual('1250', monthlyPayment.total);
        });

        it('should call render and add a collection listener for add and remove events', function () {
            /* handleSync should call render, so we need to stub it to avoid DOM interaction */
            var renderStub = sinon.stub(this.totals, 'render').returns('');

            /* Make sure collection isnt listening to add or remove events */
            assert.notOk(this.totals.collection._events.add);
            assert.notOk(this.totals.collection._events.remove);

            this.totals.handleSync();

            /* Make sure render was called */
            assert.ok(renderStub.calledOnce);
            /* Check that collection is now listening for add and remove events */
            assert.ok(this.totals.collection._events.add);
            assert.ok(this.totals.collection._events.remove);
        });
    });
});
