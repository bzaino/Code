/*global describe, it, chai, sinon */
define([
    'modules/FinancialStatus/Grid',
    'backbone',
    'salt'
], function (Grid, Backbone, SALT) {

    var assert = chai.assert;

    /* What I want to test for the loan grid
            Click events mapped
            Render calls renderSingle for each item in the collection
            A method that triggers a salt event to open detail page
    */

    var grid = new Grid({ collection: new Backbone.Collection({})});


    describe('Grid View', function () {
        /*
        it('should throw an error if a gridview is initialized without a collection', function () {
        });


        it('should add an event listener for add after the initial render', function () {
            // Initialize the view and make sure the add event listeren isnt present

            // FIXME: this test needs to be looked at
            var grid = new Grid({ collection: new Backbone.Collection([{foo: 'bar'}])});
            assert.notOk(grid.collection._events.add);

            grid.render();

            // Check that the views collection is now listening for add events after rendering
            assert.ok(grid.collection._events.add);
        });
        */

        it('should add an event listener for sync when view is initialized', function () {
            assert.ok(grid.collection._events.sync);
        });

        it('should have a click listener to open a detail', function () {
            assert.ok(grid.events['click .js-loanDetailLink'], 'No event mapped');
            assert.equal(grid.events['click .js-loanDetailLink'], 'openDetail', 'Event mapped to the wrong method');
        });

        it('should render for each item in the collection', function () {
            /* Initialize view with a collection with two items */
            var grid = new Grid({ collection: new Backbone.Collection([{foo: 'bar'}, {foo: 'baz'}])});

            var renderSingleStub = sinon.stub(grid, 'renderSingle').returns('');

            grid.render();

            /* Check that render single was called twice */
            assert.ok(renderSingleStub.calledTwice);

            /* restore the stubbed behavior for future tests that may depend on it */
            grid.renderSingle.restore();
        });

        it('should call a salt event with proper arguments', function () {
            var saltEventStub = sinon.stub(SALT, 'trigger').returns('');

            //setup a fake path to ID (this would normally come from the click event)
            var e = { target: { attributes : [ {value: 5}]}};
            var attrStub = sinon.stub($.fn, 'attr').returns(5);

            //Call the method under test
            var idUsed = grid.openDetail(e);

            // Should fire one salt event
            assert.ok(saltEventStub.calledOnce, 'no salt event was fired');
            assert.ok(attrStub.called, 'jquery.attr wasnt called');
            // Should pass correct event name and id
            assert.ok(saltEventStub.calledWith('loan:selected', 5));
            // Open detail should return the id
            assert.equal(5, idUsed, 'wrong id returned');

            /* restore the stubbed behavior for future tests that may depend on it */
            SALT.trigger.restore();
            $.fn.attr.restore();
        });

        it('should remove the element that has an id matching the model to be removed', function () {
            $('body').append('<div id="5"></div>');

            var modelWithId = new Backbone.Model({ id: 5 });

            //Make sure there is an element with id 5
            assert.ok($('#5'));
            grid.removeSingle(modelWithId);
            //Make sure the element has been removed
            assert.deepEqual(0, $('#5').length);
        });

        /*
        it('should call updateSingle function when loan:updated is triggered', function () {

            // two more stubs because updateGrid is still being excuted
            //var removeStub = sinon.stub(Grid.prototype, 'removeSingle');
            //var renderStub = sinon.stub(Grid.prototype, 'renderSingle');
            //var serializeStub = sinon.stub(Grid.prototype, 'serializeModel');
            //var updateStub = sinon.stub(Grid.prototype, 'updateSingle');
            //var grid2 = new Grid({ collection: new Backbone.Collection({})});
            var grid2 = sinon.createStubInstance(Grid);

            SALT.trigger('loan:updated');
            //assert.ok(updateStub.calledOnce);
            //updateStub.restore();
            //removeStub.restore();
            //renderStub.restore();
            //serializeStub.restore();
        });
        */
    });
});
