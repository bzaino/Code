/*global describe, afterEach, beforeEach, it, chai, sinon */
define([
    'modules/FinancialStatus/AddLoan',
    'salt',
    'backbone'
], function (AddLoan, SALT, Backbone) {

    var assert = chai.assert;

    $('#fixtures').html($('<div id="baz" style="display:none"></div>'));
    var foundationStub = sinon.stub($.fn, 'foundation');
    var addLoan = new AddLoan({el: '#baz', collection: new Backbone.Collection()});
    foundationStub.restore();


    /* What I want to test:
            -When 'show:loanForm' salt event is triggered, view should show its $el
    */
    describe('AddLoan', function () {
        afterEach(function () {
            $('#baz').remove();
        });

        it('should show the proper form after choosing form the drop down', function () {
            $('#fixtures').append('<form class="addLoanForm" id="addLoanForm" style="display:none"><a class="button alert" id="cancel" style="display:none">Cancel</a></form><div id="loan-forms"><form id="cc-form" style="display:none"></form><form id="car-form" style="display:none"></form></div>');

            var form = 'cc-form';
            /* Check that the form is style="display:none" */
            assert.notOk($('#' + form).is(':visible'));

            addLoan.showSpecificForm(form);

            /* Check that the right form is visible */
            assert.ok($('#' + form).is(':visible'));
        });

        it('should trigger a reveal open', function () {
            /* Stub out any calls to foundation */
            var foundationStub = sinon.stub(addLoan.$el, 'foundation').returns('');
            addLoan.showLoanForm();

            /* Make sure foundation was called and that it was passed arguments to open th4e overlay */
            assert.ok(foundationStub.calledOnce);
            assert.ok(foundationStub.calledWith('reveal', 'open'));

            /* Restore foundation for any future tests that may depend on it */
            addLoan.$el.foundation.restore();
        });

        /*
        TODO: Fix this test
        it('should capture form name and call show', function () {
            // Setup event data structure.  We the method we are testing depends upon e.currentTarget.value 
            var evt = { currentTarget: { value: 'foo', selectedIndex: [] } };

            var showFormStub = sinon.stub(addLoan, 'showSpecificForm').returns('');
            addLoan.captureFormName(evt);

            // Check that it was called, and that it was called with correct argument 
            assert.ok(showFormStub.calledOnce);
            assert.ok(showFormStub.calledWith('foo'));

            // restore the stubbed behavior for future tests that may depend on it
            addLoan.showSpecificForm.restore();
        });
        */


        it('should restore the default form when close method is called', function () {
            /* Check that the form is style="display:none" before calling the close method which should show it */
            assert.notOk($('#addLoanForm').is(':visible'));

            addLoan.closeForm();

            /* Make sure it is visible now */
            assert.ok($('#addLoanForm').is(':visible'));
        });

        it('should add model, save to server and close the form', function () {
            /* Setup stubs for functionality we dont want called */
            var e = { currentTarget: {parentNode: {checkValidity: function () {
                return true;
            }}} };
            var serializeStub = sinon.stub(addLoan, 'serializeData').returns({});
            var closeFormStub = sinon.stub(addLoan, 'closeForm').returns('');
            var collectionCreateStub = sinon.stub(addLoan.collection, 'create').returns('');

            addLoan.addManualLoan(e);

            /* Check that all stubs were called */
            assert.ok(serializeStub.calledOnce);
            assert.ok(serializeStub.calledWith(e));
            assert.ok(closeFormStub.calledOnce);
            assert.ok(collectionCreateStub.calledOnce);
            /* There should be a property on the object serializeData returns with RecordSource 4 */
            assert.ok(collectionCreateStub.calledWithMatch({ RecordSourceId : 4 }, { wait: true, parse: false }));

            /* Restore the stubs in case any future tests depend on the stubbed functionality */
            addLoan.serializeData.restore();
            addLoan.closeForm.restore();
            addLoan.collection.create.restore();
        });

        it('should call a SALT publish with the proper string as an argument', function () {
            var model = new Backbone.Model({ LoanTypeId: 'CC', LoanType: 'foo' });
            var publishStub = sinon.stub(SALT, 'publish');

            addLoan.trackCompleted(model);

            assert.ok(publishStub.calledOnce);
            assert.ok(publishStub.calledWith('KWYO:addloans:complete', { debtType: 'Credit Card Debt'}));

            publishStub.restore();
        });
    });
});
