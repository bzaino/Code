/*global describe, before, after, it, chai */
define([
    'jquery',
    'salt',
    'salt/schoolLookup'
], function ($, SALT) {

    var assert = chai.assert;

    describe('DOM: School Lookup test', function () {
        describe('toggleErrorMessage show/hide', function () {
            it('should toogle error message not visible then visible', function () {
                var testableHTML = '<input type="text" id="schoolFilter" value=""/><small class="error" id="school-name-error">School name not found in list.</small>';
                $('#fixtures').html(testableHTML);

                SALT.schoolLookup.config.itemFoundInList = true;
                SALT.schoolLookup.toggleErrorMessage();
                assert.isFalse($('#school-name-error').is(':visible'), 'error message should not be visible');

                $('#schoolFilter').val('non-existent school');
                SALT.schoolLookup.config.itemFoundInList = false;
                SALT.schoolLookup.toggleErrorMessage();
                assert.isTrue($('#school-name-error').is(':visible'), 'error message should be visible');
            });
        });
        describe('populateLookupValues', function () {
            it('should set config values based on item passed in/selected', function () {
                var testableHTML = '<div id=schoolFilter></div><div id=OECode></div><div id=OEBranch></div><ul id="schoolsContainerList"><li><a fullschoolname="Art Institute of Boston" schoolid="008174" branch="00" href="#" class="schoolName">Art Institute of Boston</a></li>';

                SALT.schoolLookup.config.itemFoundInList = false;

                $('#fixtures').html('');
                $('#fixtures').html(testableHTML);

                var $dropDownItem = $('ul#schoolsContainerList > li > a').filter(function () {
                    return $(this).text().toLowerCase() === 'art institute of boston';
                });

                SALT.schoolLookup.populateLookupValues($dropDownItem);

                assert.isTrue(SALT.schoolLookup.config.itemFoundInList, 'itemFoundInList should be true', 'Counter should be zero');
                assert.equal($(SALT.schoolLookup.config.oeCodeField).val(), '008174', 'oeCode incorrect value');
                assert.equal($(SALT.schoolLookup.config.oeBranchField).val(), '00', 'oeBranchCode incorrect value');
                assert.equal($(SALT.schoolLookup.config.inputField).val(), 'Art Institute of Boston', 'inputField incorrect value');
            });
        });
    });

    $('#fixtures').html('');
});
