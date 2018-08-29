/*global describe, it, chai, sinon */
define([
    'salt',
    'modules/VLC/VLCViews'
], function (SALT, views) {

    var assert = chai.assert;

    describe('VLC Views', function () {

        describe('ApplicationView', function () {

            var applicationView;

            before(function () {
                applicationView = new views.ApplicationView();
            });

            describe('Instantiation', function () {
                it('should have a div as the root element', function () {
                    assert.equal('div', applicationView.el.nodeName.toLowerCase());
                });
            });
            describe('saveAnswers', function () {
                it('should trigger a SALT trigger with proper arguments when called', function () {
                    var saltEventStub = sinon.stub(SALT, 'trigger');

                    applicationView.saveAnswers();

                    assert.ok(saltEventStub.calledOnce);
                    assert.ok(saltEventStub.calledWith('saveAnswers:called'));
                    saltEventStub.restore();
                });
            });
            describe('startOverClicked', function () {
                it('should trigger a SALT trigger with proper arguments when called', function () {
                    var saltEventStub = sinon.stub(SALT, 'trigger');

                    applicationView.startOverClicked();

                    assert.ok(saltEventStub.calledOnce);
                    assert.ok(saltEventStub.calledWith('navigate'));
                    saltEventStub.restore();
                });
            });
        });

        describe('ManageablePaymentYNView setUiFields', function () {
            var manageablePaymentYNView;

            before(function () {
                manageablePaymentYNView = new views.ManageablePaymentYNView();
            });

            describe('setUiFields', function () {
                it('should trigger a SALT trigger with proper arguments when called', function () {
                    var saltEventStub = sinon.stub(SALT, 'trigger');

                    manageablePaymentYNView.setUiFields();

                    assert.ok(saltEventStub.calledOnce);
                    assert.ok(saltEventStub.calledWith('ManageablePaymentYNView:setUiFields'));
                    saltEventStub.restore();
                });
            });
        });
    });
});