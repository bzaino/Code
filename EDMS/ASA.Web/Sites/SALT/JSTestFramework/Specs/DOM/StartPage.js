/*global describe, before, after, it, chai, sinon */
define([
    'modules/FinancialStatus/StartPage',
    'salt'
], function (StartPage, SALT) {

    /* default to assert style, replace this if you prefer 'should' or 'expect'. */
    var assert = chai.assert;

    $('#fixtures').html($('<div id="js-start" class="reveal-modal" data-reveal><a class="js-close">Close Button</a></div>'));
        var foundationStub = sinon.stub($.fn, 'foundation');
        var startPage = new StartPage({ el: '#js-start' });
        foundationStub.restore();

    /* I WANT TO TEST:
            - Foundation is called to open the overlay on initialize
            - There is a click event to close the overlay mapped
            - Foundation is called to close the overlay when the "close" event runs
        });
    */

    describe('Start Page View', function () {

        it('Should call foundation to open the overlay on its initialize method', function () {
            /* create a stub for foundation */
            var foundationStub = sinon.stub(startPage.$el, 'foundation').returns('');
            /* run the initialize method */
            startPage.initialize();
            /* assert the stub ran once when method was called */
            assert.ok(foundationStub.calledOnce);
            /* assert it was called with the correct parameters */
            assert.ok(foundationStub.calledWith('reveal', 'open'));
            /* Restore foundation for any future tests that may depend on it */
            startPage.$el.foundation.restore();
        });

        it('Should have a click event mapped to class js-close', function () {
            assert.ok(startPage.events['click .js-close']);
            assert.equal('close', startPage.events['click .js-close']);
        });

        it('Should call foundation to close the overlay and publish a WT event when the "close" event runs', function () {
            /* create a new stub for foundation */
            var foundationStub = sinon.stub(startPage.$el, 'foundation').returns('');
            var saltPublishStub = sinon.stub(SALT, 'publish');
            /* run the "close" method */
            startPage.close();
            /* assert the stubs were called when method was called */
            assert.ok(foundationStub.calledOnce);
            assert.ok(saltPublishStub.calledOnce);
            /* assert they were called with the correct parameters */
            assert.ok(foundationStub.calledWith('reveal', 'close'));
            assert.ok(saltPublishStub.calledWith('KWYO:seeloans'));
            /* Restore stubs for any future tests that may depend on them */
            foundationStub.restore();
            saltPublishStub.restore();
        });


        it('should start joyride and attach click handlers', function () {
            var foundationStub = sinon.stub($.fn, 'foundation');
            var clickStub = sinon.stub($.fn, 'click');
            var saltPublishStub = sinon.stub(SALT, 'publish');

            startPage.startTour();

            /* assert they were called with the correct parameters */
            assert.ok(foundationStub.calledTwice);
            assert.ok(foundationStub.calledWith('joyride', 'start'));
            assert.ok(clickStub.calledTwice);
            assert.ok(saltPublishStub.calledOnce);
            assert.ok(saltPublishStub.calledWith('KWYO:tour:start'));

            /* Restore stubs for any future tests that may depend on them */
            foundationStub.restore();
            clickStub.restore();
            saltPublishStub.restore();
        });

        it('should prevent Foundation from going to the next Joyride stop when background is clicked', function () {
            var testFalse = startPage.stopJoyrideEvent();
            assert.isFalse(testFalse, 'the function returned false');
        });

        it('should close the overlay and scroll to the top', function () {
            var foundationStub = sinon.stub(startPage.$el, 'foundation');
            var scrollStub = sinon.stub($.fn, 'animate');

            startPage.tourClose();

            /* assert they were called with the correct parameters */
            assert.ok(foundationStub.calledOnce);
            assert.ok(foundationStub.calledWith('reveal', 'close'));
            assert.ok(scrollStub.calledOnce);
            assert.ok(scrollStub.calledWith({ scrollTop: 0 }, 300));

            /* Restore stubs for any future tests that may depend on them */
            foundationStub.restore();
            scrollStub.restore();
        });
    });

    describe('Upload Button', function () {
        it('should trigger a SALT publish with proper arguments when called', function () {
            var publishStub = sinon.stub(SALT, 'publish');

            startPage.uploadLoan();

            assert.ok(publishStub.calledOnce);
            assert.ok(publishStub.calledWith('KWYO:importLoans'));

            publishStub.restore();
        });
    });

    //clean up after tests run
    $('#js-start').remove();
});
