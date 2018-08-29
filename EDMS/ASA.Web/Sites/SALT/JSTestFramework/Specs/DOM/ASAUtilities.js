/*global describe, it, chai, sinon */
define([
    'jquery',
    'asa/ASAUtilities'
], function ($, Utility) {

    var assert = chai.assert;

    describe('ASAUtilities DOM', function () {

        describe('toggleVisibility', function () {
            it('should toggle visibility of an element by its id', function () {
                $('#fixtures').append('<div id="test-id" style ="display: none">test</div>');
                var $testElement = $('#test-id');
                Utility.toggleVisibility($testElement.attr('id'));
                assert.isTrue($testElement.is(':visible'));
                Utility.toggleVisibility($testElement.attr('id'));
                assert.isFalse($testElement.is(':visible'));
                $('#fixtures').html('');
            });
        });
    });


});