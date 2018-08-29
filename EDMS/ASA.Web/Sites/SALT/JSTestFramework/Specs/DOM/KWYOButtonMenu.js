/*global describe, beforeEach, before, after, it, chai, sinon */
define([
	'jquery',
	'modules/FinancialStatus/ButtonMenu',
	'salt'
], function ($, ButtonMenu, SALT) {

    var assert = chai.assert;
    /* What I want to test:
        -View
            -Events mapped for showing: addloanform and flipGrid
            -showAddLoanForm triggers salt event for "show:loanForm"
    */

    var buttonMenu = new ButtonMenu();

    describe('ButtonMenu', function () {
        describe('Add Loan button', function () {
            it('should have UI events mapped for add loan form', function () {
                assert.equal('showAddLoanForm', buttonMenu.events['click .js-addMore']);
            });

            it('should publish a WT event and trigger a show:loanForm application event', function () {
                var saltTriggerStub = sinon.stub(SALT, 'trigger'),
                    saltPublishStub = sinon.stub(SALT, 'publish');

                /* Trigger each of the methods in turn, checking that it is calling the spy once for each */
                buttonMenu.showAddLoanForm();

                //Make sure stubs were called once each
                assert.ok(saltTriggerStub.calledOnce);
                assert.ok(saltPublishStub.calledOnce);

                //Make sure they were called with the correct arguments
                assert.ok(saltTriggerStub.calledWith('show:loanForm'), 'incorrect app event published');
                assert.ok(saltPublishStub.calledWith('KWYO:addloans:start'), 'incorrect wt event published');

                //Restore stubbed behavior
                saltTriggerStub.restore();
                saltPublishStub.restore();
            });
        });

        /* I WANT TO TEST:
            - That there is an event mapped to clicking the grid flip button
            - That the event runs jquery code that changes the DOM positioning on the correct DOM elements.

         */

        before(function () {
            /* Needed only for the Grid Flip tests.
            Set up DOM elements for testing */
            $('body').append($('<div class="js-flipTest"><div class="js-graphSwitch js-elementSwitch"></div><div class="js-gridSwitch js-elementSwitch"></div></div>'));
            /* turn off the jquery effect. The delay for animations sometimes causes a false fa */
            $.fx.off = true;
        });

        after(function () {
            /* remove testing div */
            $('.js-flipTest').remove();
            /* turn jQuery effects back on */
            $.fx.off = false;
        });

        describe('Grid Flip button', function () {
            it('should have UI event mapped for flipping the position of the chart and grid', function () {
                assert.equal('flipGridEvent', buttonMenu.events['click .js-flipGrid']);
            });

            it('should change the DOM structure when the event runs', function () {
                /* run the flipGrid event */
                buttonMenu.flipGridEvent();
                /* check/assert that gridSwitch is now first element*/
                assert.strictEqual(($('.js-gridSwitch').index()), 0);
            });

            it('should trigger a SALT publish with the proper arguments when called', function () {
                var publishStub = sinon.stub(SALT, 'publish');

                buttonMenu.flipGridEvent();

                assert.ok(publishStub.calledOnce);
                assert.ok(publishStub.calledWith('KWYO:switchView'));

                publishStub.restore();
            });
        });

        describe('Glossary Button', function () {
            it('should trigger a SALT publish with proper arguments when called', function () {
                var publishStub = sinon.stub(SALT, 'publish');

                buttonMenu.glossaryClicked();

                assert.ok(publishStub.calledOnce);
                assert.ok(publishStub.calledWith('KWYO:glossary'));

                publishStub.restore();
            });
        });

        describe('Import Button', function () {
            it('should trigger a SALT publish with proper arguments when called', function () {
                var publishStub = sinon.stub(SALT, 'publish');

                buttonMenu.importClicked();

                assert.ok(publishStub.calledOnce);
                assert.ok(publishStub.calledWith('KWYO:import:nav'));

                publishStub.restore();
            });
        });
    });
});
