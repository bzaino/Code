/*global describe, it, chai*/
define([], function() {
    var assert = chai.assert;
    var interestRegEx = '^(\\d{1,2})(\\.\\d{1,2})?$';
    var freeNumberRegEx = '^\\d+$';

    function validateInput(input, reg) {
        var regex = new RegExp(reg);
        if (!regex.test(input)) {
            return false;
        } else {
            return true;
        }
    }
    describe('VLC Input validation', function() {
        describe('Interest regex', function() {
            it('should accept 2 digits before and after decimal', function() {
                assert.equal(true, validateInput(9.99, interestRegEx));
                assert.equal(false, validateInput(9.999, interestRegEx));
                assert.equal(false, validateInput(9.991, interestRegEx));
                assert.equal(true, validateInput(9.2, interestRegEx));
                assert.equal(true, validateInput(10, interestRegEx));
                assert.equal(false, validateInput(100, interestRegEx));

            });

            it('should return false for improper input types', function() {
                assert.equal(false, validateInput('9.999 6.77 7.66', interestRegEx), '9.999, interestRegEx, fail');
            });

            it('should return true for trailing zero', function() {
                assert.equal(true, validateInput(9.990, interestRegEx));
            });


            it('should return false if letters used', function() {
                assert.equal(false, validateInput('there once was a carrot named sally....', interestRegEx), '9.990, interestRegEx');
            });
        });

        describe('Free Number regex', function() {
            it('should return false for improper data type', function() {
                assert.equal(false, validateInput('there once was a carrot named sally....', freeNumberRegEx), 'there once was a carrot named sally.... , freeNumberRegEx');
            });

            it('should return true for any numeric input', function() {
                assert.equal(true, validateInput(9000001, freeNumberRegEx), 'many digits');
                assert.equal(true, validateInput(111, freeNumberRegEx), 'few digits');
                assert.equal(false, validateInput('423roo', freeNumberRegEx), 'numbers then letters');
            });
        });
    });
});