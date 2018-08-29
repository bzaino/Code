/*global describe, beforeEach, it, chai, sinon */
define([
    'salt',
    'modules/CCP/CCPModels'
], function(SALT, models) {

    var assert = chai.assert;
    describe('College Cost Planner Models', function() {
        describe('Loans Model', function() {
            var loanModel = new models.LoansModel({});
            it('should set attributes to defaults on model creation', function() {
                assert.equal(0, loanModel.get('fsl'));
                assert.equal(0, loanModel.get('fppl'));
                assert.equal(0, loanModel.get('other'));
            });
        });
        describe('UserInfo Model', function() {
            var userInfo = new models.UserInfoModel({});
            it('should set attributes to defaults on model creation', function() {
                assert.equal(0, userInfo.get('maxFsl'));
                assert.equal(0, userInfo.get('maxFppl'));
                assert.equal(1, userInfo.get('gradeLevel'));
                assert.equal(1, userInfo.get('yearsRemaining'));
                assert.equal(0, userInfo.get('costOfAttendanceTotal'));
                assert.equal(0, userInfo.get('costOfAttendanceLessAdjustmentsTotal'));
                assert.equal(0, userInfo.get('grantsPerYear'));
                assert.equal(0, userInfo.get('grantsTotal'));
                assert.equal(0, userInfo.get('plannedContributionsTotal'));
                assert.equal(0, userInfo.get('monthlyInstallmentPerMonth'));
                assert.equal(0, userInfo.get('monthlyInstallmentsTotal'));
                assert.equal(0, userInfo.get('loansTotal'));
                assert.equal(true, userInfo.get('setLoanDefaults'));
                assert.equal(false, userInfo.get('showLoansInGraph'));
            });
            it('should set showLoansInGraph attribute to true', function() {
                var saltSpy = sinon.spy();
                SALT.bind('change:showLoansInGraph:true', saltSpy);
                SALT.trigger('change:showLoansInGraph:true');
                assert.ok(saltSpy.calledOnce);
            });
        });
    });
});
