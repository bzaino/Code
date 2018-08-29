/*global describe, it, chai, sinon */
define([
    'modules/VLC/VLCUtilities'
], function (VLCUtils) {

    var assert = chai.assert;

    describe('VLC Utility methods', function () {
        describe('Date Methods', function() {
            it('prepareDate should return a C# ready date', function () {
                var returnedDate = VLCUtils.prepareDate(1, 1, 2000);
                assert.equal('/Date(946702800000)/', returnedDate);
            });

            it('convertDate should return a js ready date from a c# date', function () {
                //converting C# date for 1/1/2000 into JS date
                var returnedDate = VLCUtils.convertDate('/Date(946702800000)/');
                //What JS date for 1/1/2000 should be
                var expectedDate = new Date('1/1/2000');
                assert.equal(0, expectedDate - returnedDate);
            });
        });

        describe('RegexTester Method', function () {
            var interestRegEx = '^(\\d{1,2})(\\.\\d{1,2})?$';
            var freeNumberRegEx = '^\\d+$';

            describe('Interest regex', function () {
                it('should accept 2 digits before and after decimal', function () {
                    assert.equal(true, VLCUtils.validateInput(9.99, interestRegEx));
                    assert.equal(false, VLCUtils.validateInput(9.999, interestRegEx));
                    assert.equal(false, VLCUtils.validateInput(9.991, interestRegEx));
                    assert.equal(true, VLCUtils.validateInput(9.2, interestRegEx));
                    assert.equal(true, VLCUtils.validateInput(10, interestRegEx));
                    assert.equal(false, VLCUtils.validateInput(100, interestRegEx));

                });

                it('should return false for improper input types', function () {
                    assert.equal(false, VLCUtils.validateInput('9.999 6.77 7.66', interestRegEx), '9.999, interestRegEx, fail');
                });

                it('should return true for trailing zero', function () {
                    assert.equal(true, VLCUtils.validateInput(9.990, interestRegEx));
                });


                it('should return false if letters used', function () {
                    assert.equal(false, VLCUtils.validateInput('there once was a carrot named sally....', interestRegEx), '9.990, interestRegEx');
                });
            });

            describe('Free Number regex', function () {
                it('should return false for improper data type', function () {
                    assert.equal(false, VLCUtils.validateInput('there once was a carrot named sally....', freeNumberRegEx), 'there once was a carrot named sally.... , freeNumberRegEx');
                });

                it('should return true for any numeric input', function () {
                    assert.equal(true, VLCUtils.validateInput(9000001, freeNumberRegEx), 'many digits');
                    assert.equal(true, VLCUtils.validateInput(111, freeNumberRegEx), 'few digits');
                    assert.equal(false, VLCUtils.validateInput('423roo', freeNumberRegEx), 'numbers then letters');
                });
            });
        });

    });

});
