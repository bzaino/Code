define([
    'salt',
    'salt/ForgotPasswordManager'
],
function (SALT, ForgotPasswordManager) {

    var assert = chai.assert;
    describe('ForgotPasswordManager', function () {
        describe('handleResponse', function () {
             it('Vefirifies success message is displayed when the result is success', function () {
                var result = ({
					Success: true
				});
                var successStub = sinon.stub(ForgotPasswordManager, 'showSuccessMsg');
                ForgotPasswordManager.handleResponse(result, '');
                assert.ok(successStub.calledOnce);
                successStub.restore();
             });

            it('Vefirifies error message is displayed', function () {
                var result = ({
                    Message:'Error! try Again'
                });

                var errorStub = sinon.stub(ForgotPasswordManager, 'showErrorMsg');
                ForgotPasswordManager.handleResponse(result, '');
                assert.ok(errorStub.calledOnce);
                assert.ok(errorStub.calledWith('Error! try Again'));
                errorStub.restore();
             });

        });
    });
});


