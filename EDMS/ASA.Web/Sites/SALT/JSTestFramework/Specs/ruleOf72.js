/*global describe, it, chai, sinon */
define([
    'Tools/ruleOf72'
], function (ruleOf72) {

    var assert = chai.assert;
    var expect = chai.expect;

    describe('ruleOf72', function () {

        it('Calculate years taken to double your money', function () {
            var retVal = ruleOf72.global.utils.calculateRule(5);
            assert.equal(14.4, retVal);
            retVal = ruleOf72.global.utils.calculateRule('65464');
            assert.equal(0, retVal);
        });
        it('Check if the value is a number', function () {
            var retVal = ruleOf72.global.utils.isNumber(550);
            assert.ok(retVal);
            retVal = ruleOf72.global.utils.isNumber('ASA');
            assert.notOk(retVal);
        });
    });
});
