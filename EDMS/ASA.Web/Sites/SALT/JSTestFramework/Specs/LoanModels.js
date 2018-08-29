/*global describe, it, beforeEach, chai, sinon */
define([
    'modules/sharedModels'
], function(models) {

    /* default to assert style, replace this if you prefer 'should' or 'expect'. */
    var assert = chai.assert;

    describe('Loan Model', function() {

        var loan;

        beforeEach(function(done) {
            loan = new models.Loan({});
            done();
        });

        it('should use the correct ID attribute', function() {
            /*  The db field for id should map to the backbone model id
                See docs for idAttribute http://backbonejs.org/#Model-idAttribute
            */
            var idModel = new models.Loan({
                LoanSelfReportedEntryId: 5
            });

            assert.equal(5, idModel.id);
        });

        it('should set attributes to defaults on model creation', function() {
            assert.equal('Member', loan.get('LoanSource'));
            assert.equal(4.45, loan.get('InterestRate'));
            assert.equal('', loan.get('LoanStatusId'));
            assert.equal('10', loan.get('LoanTerm'));
            assert.equal('UE', loan.get('LoanTypeId'));
            assert.equal(0, loan.get('OriginalLoanAmount'));
            assert.equal(0, loan.get('PrincipalBalanceOutstandingAmount'));
            assert.equal(0, loan.get('ReceivedYear'));
            assert.equal(2, loan.get('RecordSourceId'));
            assert.equal('', loan.get('TypeName'));
            assert.equal('federal', loan.get('LoanType'));
            assert.equal('FooName', loan.get('LoanName'));
            assert.equal(null, loan.get('MonthlyPaymentAmount'));
            assert.equal(null, loan.get('LoanSelfReportedEntryId'));
            assert.equal('\/Date(1378930055733-0400)\/', loan.get('DateAdded'));
            assert.equal('\/Date(1378930055733-0400)\/', loan.get('OriginalLoanDate'));
        });

        it('should have correct REST url', function() {
            assert.equal('/api/SelfReportedService/restLoans', loan.urlRoot);
        });

        it('should fire a change event', function() {
            var spy = sinon.spy();
            loan.bind('change', spy);
            loan.set('OriginalLoanAmount', 100);
            assert.equal(spy.calledOnce, true);
        });

    });

    describe('Loans For SAL model', function() {
        var sal = new models.LoansForSAL({});

        it('should set attributes to defaults on model creation', function() {
            assert.lengthOf(sal.get('ErrorList'), 0);
            assert.equal('', sal.get('RedirectURL'));
            assert.lengthOf(sal.get('Loans'), 0);
        });

        it('should have correct REST url', function() {
            assert.equal('/api/SelfReportedService/SaveList', sal.url);
        });

    });
});
