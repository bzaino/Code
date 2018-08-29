/*global describe, before, after, it, chai */
define([
    'jquery',
    'salt/pschecker'
], function ($) {

    var assert = chai.assert;

    describe('Password Requirements', function () {
        describe('Strength Checker', function () {
            $('#fixtures').html(
                '<form id="test-form">' +
                    '<div class="password-container row">' +
                        '<div class="meter-txt">' +
                            '<span class="meter-txt"></span>' +
                            '<input id="password-field" class="strong-password meter" type="password"/>' +
                        '</div>' +
                    '</div>' +
                '</form>'
            ).pschecker();
            var $meter = $('#password-field'),
                $meterText = $('.meter-txt');
            it('psChecker: test a strong password meter -lowercase, uppercase, numbers, special characters', function () {
                //set the password value, and trigger blur
                $meter.val('111aaaA!').blur();
                //check for expected classname strength value
                assert.ok($meter.hasClass('strong'));
                assert.ok($meterText.hasClass('strong'));
            });
            it('psChecker: test a medium password meter - lowercase, uppercase and special characters', function () {
                //set the password value, and trigger blur
                $meter.val('aaaaaaA!').blur();
                //check for expected classname strength value
                assert.ok($meter.hasClass('medium'));
                assert.ok($meterText.hasClass('medium'));
            });
            it('psChecker: test a weak password meter -lowercase only', function () {
                //set the password value, and trigger blur
                $meter.val('1111111').blur();
                //check for expected classname strength value
                assert.ok($meter.hasClass('weak'));
                assert.ok($meterText.hasClass('weak'));
            });
            it('should remove any previous strength indicators when a reset event is triggered on a form', function () {
                //set the password value, and trigger blur
                $meter.val('1111111').blur();
                // test that we have previous indicators
                assert.ok($meter.is('.weak, .medium, .strong'));
                assert.ok($meterText.is('.weak, .medium, .strong'));
                // trigger form reset
                $('#test-form').trigger('reset');
                // test that all previous indicators were removed
                assert.ok($meter.not('.weak, .medium, .strong'));
                assert.ok($meterText.not('.weak, .medium, .strong'));
            });
            it('should not validate passwords that are shorter than minimum password length (currently 8)', function () {
                //set the password value, and trigger blur
                $meter.val('aaaa').blur();
                // test that password strength was not evaluated
                assert.ok($meter.not('.weak, .medium, .strong'));
                assert.ok($meterText.not('.weak, .medium, .strong'));
            });
            it('should validate to medium if passwords contain numbers, special characters and lower characters only', function () {
                //set the password value, and trigger blur
                $meter.val('12345678@w').blur();
                // test that password strength evaluated to medium
                assert.ok($meter.hasClass('medium'));
                assert.ok($meterText.hasClass('medium'));
            });
        });
    });
});
