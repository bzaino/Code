/*global describe, it, chai, sinon */
define([
    'dust',
    'modules/SaltDustHelpers'
], function (dust) {

    /* default to assert style, replace this if you prefer 'should' or 'expect'. */
    var assert = chai.assert;
    describe('SaltDustHelpers filter', function () {
        describe('Currency comma filter', function () {
            it('should return a number less than 1000 without change', function () {
                var returnedVal = dust.filters.currencyComma(500);
                assert.equal(500, returnedVal);
            });

            it('should return comma formatted number if greater than 1000', function () {
                var returnedVal = dust.filters.currencyComma(1000);
                assert.equal('1,000', returnedVal);
            });

            it('should return multiple commas if greater than 999999', function () {
                var returnedVal = dust.filters.currencyComma(1000000);
                assert.equal('1,000,000', returnedVal);
            });
        });
        describe('Lowercase filter', function () {
            it('should return SALT in lowercase letters', function () {
                var returnedVal = dust.filters.lc('SALT');
                assert.equal('salt', returnedVal);
            });
        });
        describe('String trim filter', function () {
            it('should return SALT with spaces trimmed', function () {
                var returnedVal = dust.filters.trim(' SALT ');
                assert.equal('SALT', returnedVal);
            });
        });
        describe('First Flat Tag filter', function () {
            it('should return first flat tag in delimited list', function () {
                var flatTags = "banking | loans | bankruptcy";
                var returnedVal = dust.filters.returnFirstFlatTag(flatTags);
                assert.equal('banking', returnedVal);
            });
        });
        describe('Second Flat Tag filter', function () {
            it('should return second flat tag in delimited list', function () {
                var flatTags = "banking | loans | bankruptcy";
                var returnedVal = dust.filters.returnSecondFlatTag(flatTags);
                assert.equal('loans', returnedVal);
            });
        });

    });
});
