  /*global describe, beforeEach,it, afterEach, chai, sinon, done*/
define([
	'jquery',
	'salt',
	'salt/registration'
], function ($, SALT, Registration) {
    var assert = chai.assert;
    describe('Registration', function () {
        describe('callSuccess', function () {
            var spy;
            var spySetErrorMsg;

            spy = sinon.spy(Registration, 'callSuccess');

            before(function () {
                SALT.closeOverlay = function () { };
                spySetErrorMsg = sinon.spy(Registration, 'setErrorMsg');
            });
            it('should set the api generic error msg in html appropriately', function () {
                spy.reset();
                var errorMsg = 'Our system is having trouble with your registration. Please check that everything\'s correct-especially your email and password-and then try again. If you still can\'t register, let Member Support know by calling 855.469.2724 (toll-free).';

                var data = {
                    ErrorList: [{
                        BusinessMessage: 'Error. Unable to complete your registration. Please check your information and try again.',
                        Code: 'GenericError',
                        DetailMessage: 'Error. Unable to complete your registration. Please check your information and try again.',
                        ServiceName: 'ASAMember Service'
                    }]
                };

                Registration.callSuccess(data);
                assert.ok(spySetErrorMsg.calledOnce);
                assert.equal(errorMsg, spySetErrorMsg.args[0]);
            });
            it('should set the DuplicateUserName error msg in html appropriately', function () {
                spy.reset();
                spySetErrorMsg.reset();
                var errorMsg = 'It looks like our system already has an account registered to this email address.';

                var data = {
                    ErrorList: [{
                        BusinessMessage: 'Error. It looks like you already have a SALT account. Visit saltmoney.org to login or to recover your password.',
                        Code: 'DuplicateUserName',
                        DetailMessage: 'Error. It looks like you already have a SALT account. Visit saltmoney.org to login or to recover your password.',
                        ServiceName: 'ASAMember Service'
                    }]
                };

                Registration.callSuccess(data);
                assert.ok(spySetErrorMsg.calledOnce);
                assert.equal(errorMsg, spySetErrorMsg.args[0]);
            });
        });
        describe('registerErrorCallback', function () {
            var spy;
            var spySetErrorMsg;

            spy = sinon.spy(Registration, 'registerErrorCallback');

            before(function () {
                SALT.registration.setErrorMsg = function () { };
                SALT.closeOverlay = function () { };
                spySetErrorMsg = sinon.spy(Registration, 'setErrorMsg');
            });
            it('should set the generic error msg error call back in html with appropriate message', function () {
                spy.reset();
                var errorMsg = 'Our system is having trouble with your registration. Please check that everything\'s correct-especially your email and password-and then try again. If you still can\'t register, let Member Support know by calling 855.469.2724 (toll-free).';

                Registration.registerErrorCallback();
                assert.ok(spySetErrorMsg.calledOnce);
                assert.equal(errorMsg, spySetErrorMsg.args[0]);
            });
        });
        describe('registerHandleQueryString', function () {
            it('should build the vars appropriately using the query string', function () {
                var href = '?t=2351263-374637-23423-234764237&oe=003639&br=00';
                var result = Registration.handleQueryString(href);
                assert.equal(result.t, '2351263-374637-23423-234764237');
                assert.equal(result.oe, '003639');
                assert.equal(result.br, '00');
            });
        });

        describe('Checkbox WebTrends(WTcheckboxRouter)', function () {
            it('Check that webtrebds calls are made when checkbox is clicked', function () {
                var webTrendsStub = sinon.stub(SALT.registration, 'checkboxWT');
                Registration.WTcheckboxRouter();
                assert.ok(webTrendsStub.calledTwice);
                webTrendsStub.restore();
            });
        });
    });
});
