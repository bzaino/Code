  /*global describe, beforeEach,it, afterEach, chai, sinon, done*/
define([
    'jquery',
    'salt',
    'modules/reportingstatus'
], function ($, SALT, ReportingStatus) {
    var assert = chai.assert;
    describe('ReportingStatus', function () {
        describe('Test validateStatus method', function () {
            it('should vaildate enrollment status G is valid', function () {
                var enrollmentStatus = 'G';
                var result = ReportingStatus.validateStatus(enrollmentStatus);
                assert.equal(result, true);
            });
            it('should vaildate enrollment status N is valid', function () {
                var enrollmentStatus = 'N';
                var result = ReportingStatus.validateStatus(enrollmentStatus);
                assert.equal(result, true);
            });
            it('should vaildate enrollment status Q is valid', function () {
                var enrollmentStatus = 'Q';
                var result = ReportingStatus.validateStatus(enrollmentStatus);
                assert.equal(result, true);
            });
            it('should vaildate enrollment status W is valid', function () {
                var enrollmentStatus = 'W';
                var result = ReportingStatus.validateStatus(enrollmentStatus);
                assert.equal(result, true);
            });
            it('should vaildate enrollment status X is valid', function () {
                var enrollmentStatus = 'X';
                var result = ReportingStatus.validateStatus(enrollmentStatus);
                assert.equal(result, true);
            });
            it('should vaildate enrollment status D is invalid', function () {
                var enrollmentStatus = 'D';
                var result = ReportingStatus.validateStatus(enrollmentStatus);
                assert.equal(result, false);
            });
        });
    });
});
