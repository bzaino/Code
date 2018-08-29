/*global describe, it, chai, sinon */
define([
    'jquery',
    'backbone',
    'modules/FinancialStatus/NextSteps'
], function($, Backbone, NextSteps) {

    /* default to assert style, replace this if you prefer 'should' or 'expect'. */
    var assert = chai.assert;

    var nextSteps = new NextSteps({ el: '#foo', collection: new Backbone.Collection() });

    describe('Next Steps Widget', function() {
        it('should hide if there are no loans in the collection', function(){
            //Arrange
            var addClassStub = sinon.stub($.fn, 'addClass');

            //Act
            nextSteps.decideShow();

            //Assert
            assert.ok(addClassStub.calledOnce);

            addClassStub.restore();
        });

        it('should show if there are any loans in the collection', function () {
            //Arrange - Add a model to the empty collection, and stub remove class
            nextSteps.collection.add(new Backbone.Model({}));
            var removeClassStub = sinon.stub($.fn, 'removeClass');

            //Act
            nextSteps.decideShow();

            //Assert
            assert.ok(removeClassStub.calledOnce);

            removeClassStub.restore();
        });
    });
});