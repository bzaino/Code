/*global describe, it, chai, sinon */
define([
    'salt',
    'modules/Profile/OrganizationInformation'
], function (SALT, OrganizationInformation) {
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
            return { EnrollmentStatus: 'H', Organizations: [{ SchoolType: null}] };
        }
    };

    describe('Profile - SchoolInformation', function () {
        it('Should add enrollment status selected check to the context object', function () {
            var renderStub = sinon.stub(OrganizationInformation.prototype, 'render');
            var schoolInfo = new OrganizationInformation({ el: '#school-information-section', Questions: questionsAnswers, model: SiteMember });
            var context = schoolInfo.prepareData();
            assert.equal(context.H_Selected, 'selected');
            renderStub.restore();
        });
        it('Should return the primary organization', function () {
            var data = {
                'Organizations': [{
                    'OrganizationName': 'Boston College',
                    'IsPrimary': false
                },
                {
                    'OrganizationName': 'Zane State University',
                    'IsPrimary': true
                }]
            };

            var renderStub = sinon.stub(OrganizationInformation.prototype, 'render');
            var schoolInfo = new OrganizationInformation({ el: '#school-information-section', Questions: questionsAnswers, model: SiteMember });
            var currentOrganization = schoolInfo.selectCurrentOrganization(data);
            assert.equal(currentOrganization, 'Zane State University');
            renderStub.restore();
        });
        it('Should return the expected graduation year', function () {
            var data = {
                'Organizations': [{
                    'OrganizationName': 'Boston College',
                    'IsPrimary': false,
                    'ExpectedGraduationYear' : 2015
                },
                {
                    'OrganizationName': 'Zane State University',
                    'IsPrimary': true,
                    'ExpectedGraduationYear' : 2022
                }]
            };

            var renderStub = sinon.stub(OrganizationInformation.prototype, 'render');
            var schoolInfo = new OrganizationInformation({ el: '#school-information-section', Questions: questionsAnswers, model: SiteMember });
            var expectedGraduationDate = schoolInfo.selectExpectedGraduationYear(data);
            assert.equal(expectedGraduationDate, '2022');
            renderStub.restore();
        });
    });

});