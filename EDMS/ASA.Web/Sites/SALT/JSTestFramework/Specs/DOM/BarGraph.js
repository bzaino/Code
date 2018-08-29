/*global before, after, describe, it, chai */
define([
    'jquery',
    'salt',
    'modules/FinancialStatus/BarGraph',
    'modules/sharedModels',
    'highcharts4'
], function($, SALT, BarGraph, models) {

    var assert = chai.assert;

    /* WHAT I WANT TO TEST for BarGraph:

        -Creates a Highchart when view is created
        -Serializes principal properly
        -Sets chart series data properly
    */
    before(function () {
        $('body').append($('<div id="bargraph-template"></div>'));
        this.barGraph = new BarGraph({el: '#bargraph-template', model: new models.Loan()});
    });

    after(function () {
        $('#bargraph-template').remove();
    });

    describe('BarGraph', function() {
        it('should create a highcharts chart in the view container upon initialization', function () {
            /* the .highcharts method returns a chart object when found on the selector passed
                ensure that a chart is being added to the view's $el */
            assert.ok(this.barGraph.$el.highcharts());
        });

        it('should return an array of principal values', function () {
            this.barGraph.model.set('OriginalLoanAmount', 500);
            var principalArr = this.barGraph.preparePrincipal();
            assert.deepEqual([500, 500, 500], principalArr);
        });

        it('should set the view\'s chart data correctly when modelChanged is called', function () {

            /* Make sure bar's value is what we think it is before attempting to change it */
            assert.equal(500, this.barGraph.chart.series[1].data[0].y);

            /* Value to change to */
            var newValue = 2000;

            /*  Add silent option to keep any backbone events from being triggered.
                We added a listener for model changes in the views initializer that we don't want triggered
                This allows us to programatically trigger the method we are attempting to test
            */
            this.barGraph.model.set('OriginalLoanAmount', newValue, {silent: true});

            /* Call method we are testing */
            this.barGraph.modelChanged();

            /* Make sure it changed the bar's value to the amount we set the model to */
            assert.equal(newValue, this.barGraph.chart.series[1].data[0].y);
        });
    });
});
