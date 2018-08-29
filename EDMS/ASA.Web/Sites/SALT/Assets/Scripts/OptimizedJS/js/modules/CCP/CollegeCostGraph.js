define([
    'jquery',
    'backbone',
    'salt',
    'asa/ASAUtilities',
    'highcharts4',
    'highcharts4-3d'
], function ($, Backbone, SALT, utility, Highcharts) {
    var graph = Backbone.View.extend({
        initialize: function (inputs) {
            this.$el.highcharts({
                chart: {
                    type: 'column',
                    options3d: {
                        enabled: true,
                        alpha: 15,
                        beta: 15,
                        depth: 50
                    },
                    width: 300
                },
                //column colors, used in order listed
                colors: ['#9f5cc0', '#00a3e1', '#939598', '#5bc500', '#ffb81c', '#f5416c'],
                tooltip: {
                    formatter: function () {
                        var tip = this.series.name + ': <b>$' + utility.currencyComma(this.y) + '</b><br/>';
                        return tip;
                    }
                },
                legend: {
                    itemHoverStyle: {
                        cursor: 'default' //change so hand icon no displayed
                    },
                    itemDistance: 250 //force legend names to be on seperate rows
                },
                plotOptions: {
                    column: {
                        stacking: 'normal',
                        depth: 40
                    },
                    series: {
                        events: {
                            legendItemClick: function () {
                                return false; // <== returning false will cancel the default action
                            }
                        }
                    }
                },
                title: {
                    text: null
                },
                xAxis: {
                    categories: ['Cost / Funding']
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'In Dollars'
                    },
                },
                credits: {
                    enabled: false   //not display HighCharts LOGO
                },
                exporting: {
                    enabled: false
                },
                series: [{ //dummy data to set columns/stacks
                    name: 'Cost Of Attendance',
                    data: [0.00],
                    stack: '0'
                }, {
                    name: 'Grants And Scholarships',
                    data: [0.00],
                    stack: '1'
                }, {
                    name: 'Planned Contributions',
                    data: [0.00],
                    stack: '1'
                }, {
                    name: 'Monthly Installments',
                    data: [0.00],
                    stack: '1'
                }, {
                    name: 'Loans',
                    data: [0.00],
                    stack: '1'
                }, {
                    name: 'Loan Interest',
                    data: [0.00],
                    stack: '1'
                }]
            });
        
            /* Get chart object and assign it to a property on the view */
            this.chart = this.$el.highcharts();

            /* Watch for data changes */
            SALT.on('sync:graph', this.syncComplete, this);
        },
        syncComplete: function () {
            console.log(this.model);
            this.drawGraph();
        },
        drawGraph: function () {
            this.chart.series[0].setData([this.model.get('costOfAttendanceTotal')], false); //Cost of Attendance
            this.chart.series[1].setData([this.model.get('grantsTotal')], false); //Grants And Scholarships
            this.chart.series[2].setData([this.model.get('plannedContributionsTotal')], false); //Planned Contributions
            this.chart.series[3].setData([this.model.get('monthlyInstallmentsTotal')], false); //Monthly Installments
            if (this.model.get('showLoansInGraph')) {
                this.chart.series[4].setData([this.model.get('loansTotal')], false); //Loans
                this.chart.series[5].setData([this.model.get('interestTotal')], true); //Loan Interest

            } else {
                this.chart.series[4].setData([0], false); //Loans
                this.chart.series[5].setData([0], true); //Loan Interest
            }
        }
    });

    return graph;
});
