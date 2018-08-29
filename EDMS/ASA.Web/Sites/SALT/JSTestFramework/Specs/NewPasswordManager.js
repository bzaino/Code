  /*global describe, beforeEach,it, afterEach, chai, sinon, done*/
define([
	'jquery',
	'salt',
	'salt/NewPasswordManager'
], function ($, SALT, NewPasswordManager) {
	var assert = chai.assert;
	describe('NewPasswordManager', function() {
		describe('handleResponse', function() {
			var spy;
			var spyShowSuccessMsg;
			var spySetErrorMsg;

			spy = sinon.spy(NewPasswordManager, 'handleResponse');

			before(function () {
				SALT.NewPasswordManager.showSuccessMsg = function(){};
				spyShowSuccessMsg = sinon.spy(NewPasswordManager, 'showSuccessMsg');
			});
			it('should show success msg when result is success', function(){
				var result = ({
					Success: true
				});

				NewPasswordManager.handleResponse(result);
				assert.ok(spyShowSuccessMsg.calledOnce);
			});
			before(function () {
				SALT.NewPasswordManager.setErrorMsg = function(){};
				spySetErrorMsg = sinon.spy(NewPasswordManager, 'setErrorMsg');
			});
			it('should set error msg in html with correct message when result is false', function(){
				spy.reset();
				var errorMsg = 'We are sorry. We were unable to process your request. Please contact Customer Service for assistance.';
				var result = ({
					Success: false
				});

				NewPasswordManager.handleResponse(result);
				assert.ok(spySetErrorMsg.calledOnce);
				assert.equal(errorMsg, spySetErrorMsg.args[0]);
			});
		});
	});
});
