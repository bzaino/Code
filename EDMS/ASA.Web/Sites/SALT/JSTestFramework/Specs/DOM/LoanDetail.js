/*global describe, it, chai, sinon */
define([
    'jquery',
    'modules/FinancialStatus/LoanDetail',
    'modules/sharedModels',
    'salt',
    'modules/LoanCollection'
], function ($, LoanDetail, Models, SALT, Collection) {

    var assert = chai.assert;

    /*  What I want to test:
            Calling close:
                -Removes the view
                -Closes the overlay
            Calling delete:
                -Calls view's close method
                -Triggers SALT 'remove:loan' event
                -Triggers that event passing across a model
            Calling chooseTemplate:
                -Sets template property based on the type of the currently selected model
    */
    // stubbing updatePolyfill function because we dont load the polyfill library for unit testing

    $.fn.updatePolyfill = function () {};
    var fooModel = new Models.Loan();
    var collection = new Collection.LoansCollection([fooModel]);
    // stubbing the initialize function of Loan Detail to be only this one line.
    var initializeStub = sinon.stub(LoanDetail.prototype, 'initialize', function () {
        this.model.once('change', this.modelChanged);
    });

    describe('Loan Detail View', function () {
        it('should close on the overlay and remove the view', function(){
            var loanDetail = new LoanDetail({ model : fooModel});
            var foundationStub = sinon.stub(loanDetail.$el, 'foundation').returns('');
            var viewRemoveStub = sinon.stub(loanDetail, 'undelegateEvents').returns('');

            loanDetail.close();

            assert.ok(foundationStub.calledOnce);
            assert.ok(foundationStub.calledWith('reveal', 'close'));
            assert.ok(viewRemoveStub.calledOnce);

            /* Restore stubs in case this functionality is used elsewhere */
            loanDetail.undelegateEvents.restore();
        });

        describe('Delete a Loan', function () {
            it('should call close', function () {
                var loanDetail = new LoanDetail({ model : fooModel});
                var closeStub = sinon.stub(loanDetail, 'close').returns('');

                loanDetail.deleteLoan();

                assert.ok(closeStub.calledOnce);
                loanDetail.close.restore();
            });


            it('should trigger a destroy', function () {
                var loanDetail = new LoanDetail({ model : fooModel});
                //Keep real events from being fired
                var saltEventStub = sinon.stub(loanDetail.model, 'destroy').returns('');

                loanDetail.deleteLoan();

                //Make sure event was fire with expected arguments
                assert.ok(saltEventStub.calledOnce);
                assert.ok(saltEventStub.calledWith({ wait:true }));

                //Restore stubs in case this functionality is used elsewhere
                loanDetail.model.destroy.restore();
            });

        });

        describe('Updating Grid after loan update', function () {
            it( 'model Change even should call modelChanged', function () {
                var stub = sinon.stub(LoanDetail.prototype, 'modelChanged'),
                    testModel = new Models.Loan(),
                    collection = new Collection.LoansCollection([testModel]),
                    detailView = new LoanDetail({ model : testModel});
                detailView.model.set({PrincipalBalanceOutstandingAmount: 4000});
                assert.ok(stub.calledOnce);
                // test that the updated model was passed
                assert.equal(4000, stub.firstCall.args[0].attributes.PrincipalBalanceOutstandingAmount);
                stub.restore();
            });

            it( 'Calling modelChanged should trigger SALT loan:updated event', function () {
                var loanDetail = new LoanDetail({ model : fooModel});
                var stub = sinon.stub(SALT, 'trigger');
                loanDetail.modelChanged();
                assert.ok(stub.calledWith('loan:updated'));
                stub.restore();
            });
        });

        it('should choose template based on Loan Type', function () {
            var loanDetail = new LoanDetail({ model : fooModel});
            var saltTriggerStub = sinon.stub(SALT, 'trigger');

            /* Private loans use LoanTypeId to create a template name */
            loanDetail.model.set({ LoanTypeId: 'PR' });
            var template = loanDetail.chooseTemplate();
            assert.equal('loan_allTypes', template);

            /* Manual Federal from KWYO */
            loanDetail.model.set({ LoanType: 'federal', LoanTypeId: 'FD'});
            template = loanDetail.chooseTemplate();
            assert.equal('loan_FederalManual', template);

            /* should choose loan_CC for loan type CC */
            loanDetail.model.set({ LoanTypeId: 'CC' });
            template = loanDetail.chooseTemplate();
            assert.equal('loan_CC', template);

            /* Federal and manual */
            loanDetail.model.set({ LoanTypeId: 'CL', RecordSourceId: 2 });
            template = loanDetail.chooseTemplate();
            assert.equal('loan_allTypes', template);

            /* Federal and Imported */
            loanDetail.model.set({ LoanTypeId: 'CL', RecordSourceId: 1 });
            template = loanDetail.chooseTemplate();
            assert.equal('loan_FederalImported', template);

            /* Federal and Imported from KWYO */
            loanDetail.model.set({ LoanTypeId: 'CL', RecordSourceId: 3 });
            template = loanDetail.chooseTemplate();
            assert.equal('loan_FederalImported', template);

            //restore salt trigger stub
            saltTriggerStub.restore();
        });

        it('should return the loanTypeId for a non-imported loan', function () {
            var loanDetail = new LoanDetail({ model : fooModel});
            //Setup a model with a non-imported source
            loanDetail.model.set({ RecordSourceId: 2, LoanTypeId: 'ZZ' });
            var id = loanDetail.idChooser();
            assert.equal('ZZ', id);
        });

        it('should return the "IMP" for an imported loan', function () {
            var loanDetail = new LoanDetail({ model : fooModel});
            //Setup a model with an imported source
            loanDetail.model.set({ RecordSourceId: 3, LoanTypeId: 'ZZ' });
            var id = loanDetail.idChooser();
            assert.equal('IMP', id);

            //Check the other imported source
            loanDetail.model.set({ RecordSourceId: 1 });
            id = loanDetail.idChooser();
            assert.equal('IMP', id);
        });
    });
});

