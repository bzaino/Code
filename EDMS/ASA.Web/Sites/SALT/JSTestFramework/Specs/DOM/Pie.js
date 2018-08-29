/*global before, sinon, after, describe, it, chai */
define([
    'jquery',
    'salt',
    'modules/FinancialStatus/Pie',
    'modules/LoanCollection',
    'highcharts4'
], function ($, SALT, Pie, LoanCollection) {

    var assert = chai.assert;

    /* WHAT I WANT TO TEST for PieGraph:
        -Has a method to serialize the necessary data from the view's collection
        -Creates a Highchart that is appended into the View's $el (TODO add case for no $el throwing)
        -Triggers salt 'slice:clicked' event when pie slice is clicked
        -redraw method calls prepare and set data
    */

    describe('Pie Graph', function () {

        before(function () {
            $('#fixtures').html($('<div id="graph-template"></div>'));
            this.pieGraph = new Pie({
                el: '#graph-template',
                id: '10',
                collection: new LoanCollection.LoansCollection([{
                    PrincipalBalanceOutstandingAmount: 500,
                    LoanName: 'LongerThan15Characters',
                    MonthlyPaymentAmount: 200,
                    TypeName: 'SomeFooType'
                }, {
                    PrincipalBalanceOutstandingAmount: 1500,
                    LoanName: 'Member1',
                    MonthlyPaymentAmount: 100
                }])
            });
        });

        after(function () {
            $('#graph-template').remove();
        });

        it('should create an array of objects with each object having a cid, a name, and a y value', function () {
            var preparedData = this.pieGraph.prepareGraphData();
            assert.equal(2, preparedData.length);
            assert.equal('LongerThan...', preparedData[0].name);
            assert.equal(this.pieGraph.collection.models[0].cid, preparedData[0].id);
            assert.equal(500, preparedData[0].y);
            assert.ok(preparedData[0].color, 'No color defined');
            assert.equal(200, preparedData[0].MonthlyPaymentAmount);
            assert.equal('SomeFooType', preparedData[0].TypeName);
            assert.equal('LongerThan15Characters', preparedData[0].LongName);
        });

        it('should create a highcharts chart in the view container upon initialization', function () {
            /* the .highcharts method returns a chart object when found on the selector passed
                ensure that a chart is being added to the view's container */
            assert.ok(this.pieGraph.$el.highcharts());
        });

        it('should trigger a salt event passing along id when pie slice clicked', function () {
            var eventSpy = sinon.spy();
            SALT.on('loan:selected', eventSpy);
            this.pieGraph.sliceClick();
            assert.ok(eventSpy.calledOnce);
            assert.ok(eventSpy.calledWith('10'));
        });

        it('should call prepare and set data', function () {
            /* stub the prepare data and setdata methods */
            var prepareDataStub = sinon.stub(this.pieGraph, 'prepareGraphData').returns('foo');
            var setDataStub = sinon.stub(this.pieGraph.chart.series[0], 'setData');

            this.pieGraph.drawPie();

            assert.ok(prepareDataStub.calledOnce);
            assert.ok(setDataStub.calledOnce);
            /* set data should be called with whatever prepare data returns */
            assert.ok(setDataStub.calledWith('foo'), 'was not called with proper arguments');

            /* Restore stubs in case this functionality is used elsewhere */
            this.pieGraph.prepareGraphData.restore();
            this.pieGraph.chart.series[0].setData.restore();
        });

        it('should call draw and add some event listeners', function () {
            // Stub out the draw method on our Pie instance
            var drawStub = sinon.stub(this.pieGraph, 'drawPie');

            // Make sure the view's collection doesnt have the events we expect to bind
            assert.notOk(this.pieGraph.collection._events.add);
            assert.notOk(this.pieGraph.collection._events.remove);
            assert.notOk(this.pieGraph.collection._events.change);

            //Call the method we are testing
            this.pieGraph.syncComplete();

            // Make sure draw was called
            assert.ok(drawStub.calledOnce);

            // Make sure we bound add/delete/change events to the view's collection
            assert.ok(this.pieGraph.collection._events.add.length);
            assert.ok(this.pieGraph.collection._events.remove.length);
            assert.ok(this.pieGraph.collection._events.change.length);
        });
    });
});
