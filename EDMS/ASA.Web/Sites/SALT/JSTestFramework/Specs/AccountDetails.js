/*global describe, it, chai, sinon */
define([
    'salt',
    'modules/Profile/AccountDetails'
], function (SALT, AccountDetails) {
    var assert = chai.assert;

    var questionsAnswers = {
        'AnsExternalId': 2,
        'AnsName': 'Bachelor',
        'QuestionExternalId': 10,
        'QuestionName': 'Highest Degree Completed',
        'RefAnsID': 54,
        'RefQuestionID': 10
    };

    var SiteMember = {
        toJSON: function () {
            return { ContactFrequency: 0,
                    EnrollmentStatus: 'H',
                    PrimaryEmailKey: 'testEmail@asa.org',
                    Schools: [{SchoolType: null}],
                    Emails: [{'EmailAddress' : 'testEmail@asa.org', 'IsPrimary' : true}]
                };
        },
        get: function () {
            return {PrimaryEmailKey: 'testEmail@asa.org', Emails: [{'EmailAddress' : 'oldEmail@asa.org', 'IsPrimary' : true}] };
        },
        set: function () {
            return {PrimaryEmailKey: 'testEmail@asa.org', Emails: [{'EmailAddress' : 'testEmail@asa.org', 'IsPrimary' : true}] };
        }
    };

    describe('Profile - AccountDetails', function () {
        it('Should add ContactFrequency selected check to the context object when prepareData called', function () {
            var renderStub = sinon.stub(AccountDetails.prototype, 'render');
            var accountDetailsInfo = new AccountDetails({ el: '#account-details-section', Questions: questionsAnswers, model: SiteMember});
            var context = accountDetailsInfo.prepareData();
            assert.equal(context.ContactFrequencyCheck, 'checked');
            renderStub.restore();
        });
        it('Should add CurrentEmail to the context object when prepareData called', function () {
            var renderStub = sinon.stub(AccountDetails.prototype, 'render');
            var accountDetailsInfo = new AccountDetails({ el: '#account-details-section', Questions: questionsAnswers, model: SiteMember});
            var context = accountDetailsInfo.prepareData();
            assert.equal(context.CurrentEmail, 'testEmail@asa.org');
            renderStub.restore();
        });
        it('Should return true from updateEmailAllowed email addresses are different', function () {
            var renderStub = sinon.stub(AccountDetails.prototype, 'render');
            var accountDetailsInfo = new AccountDetails({ el: '#account-details-section', Questions: questionsAnswers, model: SiteMember});
            var updateAllowed = accountDetailsInfo.updateEmailAllowed({'EmailAddress' : 'oldEmail@asa.org'});
            assert.isTrue(updateAllowed);
            renderStub.restore();
        });
        it('Should return false from updateEmailAllowed email addresses are the same', function () {
            var renderStub = sinon.stub(AccountDetails.prototype, 'render');
            var accountDetailsInfo = new AccountDetails({ el: '#account-details-section', Questions: questionsAnswers, model: SiteMember});
            var updateAllowed = accountDetailsInfo.updateEmailAllowed({'EmailAddress' : 'testEmail@asa.org'});
            assert.isFalse(updateAllowed);
            renderStub.restore();
        });
        it('Should return the primary email', function () {
            var data = {
                'Emails': [{
                    'EmailAddress': 'nonprimaryEmail@asa.org',
                    'IsPrimary': false
                },
                {
                    'EmailAddress': 'primaryEmail@asa.org',
                    'IsPrimary': true
                }]
            };
            var renderStub = sinon.stub(AccountDetails.prototype, 'render');
            var accountDetailsInfo = new AccountDetails({ el: '#account-details-section', Questions: questionsAnswers, model: SiteMember});
            var currentEmail = accountDetailsInfo.selectCurrentEmail(data);
            assert.equal(currentEmail, 'primaryEmail@asa.org');
            renderStub.restore();
        });
    });
});