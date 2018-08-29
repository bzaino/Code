define([
    'jquery',
    'backbone',
    'underscore',
    'salt',
    'highcharts4',
    'asa/ASAUtilities',
], function ($, Backbone, _, SALT, Highcharts, asaUtilities) {
    var Pie = Backbone.View.extend({
        initialize: function () {
            var _this = this;
            this.$el.highcharts({
                credits: {
                    enabled: false
                },
                chart: {
                    backgroundColor: 'transparent',
                    type: 'pie'
                },
                tooltip: {
                    headerFormat: '',
                    pointFormat: 'Type: <b>{point.TypeName}</b><br/>Name: <b>{point.LongName}</b><br/>Balance: <b>${point.balance}</b><br/>Monthly Payment: <b>${point.MonthlyPaymentAmount}</b>',
                    style: {
                        fontFamily: 'Arial, Sans-Serif'
                    },
                    //white, non-transparent background for tooltips
                    backgroundColor: 'rgba(255,255,255,1)' 
                },
                plotOptions: {
                    series: {                        
                        point: {
                            events: {
                                click: _this.sliceClick
                            }
                        }
                    },
                    pie: {
                        innerSize: '65%', // Creating the inner circle of the piechart to create a donut pie chart
                        allowPointSelect: true,
                        cursor: 'pointer',
                        borderWidth: 1, // Change this to 1 to put white border within piechart.
                        dataLabels: {
                            enabled: false,
                            color: '#636466',
                            style: {
                                fontFamily: 'Arial, Sans-Serif',
                                fontWeight: '700',
                                fontSize: '10px' 
                            },
                            distance: 5,
                            connectorPadding: 0
                        },
                        size: '100%',
                    }
                },
                title: {
                    text: '',
                },
                subtitle: {
                    text: '',
                    useHTML: true, //Allow to use html/css to customize subtitle.
                    align: 'center',                //For aligning and centering and
                    verticalAlign: 'middle',        //the sub-title inside pie-chart.
                    y: -45         //Move sub-title up a bit 
                },

                series: [{
                    data: this.prepareGraphData()
                }]
            });
            
            /* Get chart object and assign it to a property on the view */
            this.chart = this.$el.highcharts(); 

            SALT.on('selected:index', this.sliceAnimation, this);

            /* Watch for first data retrieval */
            this.collection.once('sync', this.syncComplete, this);

        },
        sliceAnimation: function (index) {
            var _this = this;
            _.each(this.chart.series[0].data, function (el, ind, arr) {
                if (ind !== index) {
                    if (_this.chart.series[0].data[ind].sliced) {
                        _this.chart.series[0].data[ind].slice();
                    }
                } else {
                    _this.chart.series[0].data[ind].slice();
                }
            });
        },
        sliceClick: function () {
            /* "this" refers to the highcharts slice object, the "id" property should have been set when graph was initialized */
            SALT.trigger('loan:selected', this.id);
        },
        // function to generate data for Highchart to use to create the graph
        prepareGraphData: function () {
            /* The color should be dependent on the type of loan, currently mapping against RecordSourceId */
            //var to hold sum of total loan
            var loanTotal = 0;
            //and total monthly payment  
            var monthlyTotal = 0;
            //counter for generating new shaded federal colors    
            var federalCounter = 0;
            //counter for generating new shaded private colors  
            var privateCounter = 0;
            
            // Initial SALT's colors of federal and private loans
            // these are not 1-to-1 with our brand colors to get a wider range 
            var colors = {           
                'federal': ['#001E4A', '#75DF1A', '#e69d00'],  
                'private': ['#00A3E1', '#9F5CC0', '#F5416C'] 
            };

            var _this = this;
            var collectionLength = this.collection.length;

            // prep colors for pie chart
            if (collectionLength) {
                var numFederalLoans = _this.collection.models.reduce(function (total, model) {
                    return model.get('LoanType') === 'federal' ? total + 1 : total;
                }, 0);

                var numPrivateLoans = collectionLength - numFederalLoans;

                // change this to give a bigger range of each of the individual colors
                var maxChange = 0.5,
                    federalIncrement = maxChange / (numFederalLoans / colors['federal'].length),
                    privateIncrement = maxChange / (numPrivateLoans / colors['private'].length);
            }
            // default state if there are no loans
            if (!collectionLength) {
                return [{
                    name: 'No Loans Yet',
                    y: 10,
                    color: '#636466'
                }];
            // fill the pie with colors
            } else {
                return this.collection.map(function (model) {
                    var newColor = '';
                    //Begin assigning pie color and generate new shaded colors after 3 initial colors are used.
                    if (model.get('LoanType') === 'federal') {
                        //newColor is used for highchart to create pie's colors.
                        newColor = colors[model.get('LoanType')][federalCounter];     
                        //Increment to set newColor to the next color.                           
                        federalCounter++;
                        //Begin replacing 3 current colors with 3 new shaded colors when the current 3 colors are used (when counter === array.length).                                                                       
                        if (federalCounter === colors['federal'].length) {  
                            // increase/decrease brightness for salt blue, growth green, and gold respectively
                            colors['federal'].splice(0, 3,                          
                                //Brighten or darken the first color (@salt-blue).               
                                _this.generateColor(colors['federal'][0], federalIncrement),
                                //Brighten or darken the second color (@growth-green).                 
                                _this.generateColor(colors['federal'][1], 0 - federalIncrement),
                                //Brighten or darken the third color (@gold).                  
                                _this.generateColor(colors['federal'][2], federalIncrement));                 
                            federalCounter = 0;
                        }
                    } else {
                        newColor = colors[model.get('LoanType')][privateCounter];
                        privateCounter++;
                        if (privateCounter === colors['private'].length) {
                            // increase/decrease brightness for light blue, purple, and pink respectively
                            colors['private'].splice(0, 3, 
                                _this.generateColor(colors['private'][0], privateIncrement), 
                                _this.generateColor(colors['private'][1], 0 - privateIncrement), 
                                _this.generateColor(colors['private'][2], 0 - privateIncrement));
                            privateCounter = 0;
                        }
                    } 
                    
                    //We do not want to trigger a 'change' event for this model set, which would cause the pie to rerender itself recursively
                    model.set({ Color: newColor}, { silent: true });
                    SALT.trigger('loan:updated', model);

                    
                    //Calculating total loan and total monthly payment
                    loanTotal = loanTotal + Number(model.get('PrincipalBalanceOutstandingAmount'));
                    monthlyTotal = monthlyTotal + Number(model.get('MonthlyPaymentAmount'));
                    
                    //calling setSubtitle to create the sub-title 
                    _this.setSubTitle(loanTotal.toFixed(2), monthlyTotal.toFixed(2));
                    
                    var obj = {
                        //If loan name is > 15 characters, show the first 10 with an ellipse
                        name: model.get('LoanName').length > 15 ? model.get('LoanName').substr(0, 10) + '...' : model.get('LoanName'),
                        id: model.cid,
                        y: parseFloat(model.get('PrincipalBalanceOutstandingAmount'), 10),
                        color: newColor,
                        balance: asaUtilities.currencyComma(model.get('PrincipalBalanceOutstandingAmount')),
                        MonthlyPaymentAmount: asaUtilities.currencyComma(model.get('MonthlyPaymentAmount')),
                        TypeName: model.get('TypeName'),
                        LongName: model.get('LoanName')
                    };

                    return obj;
                });
            }
        },
        syncComplete: function () {
            //Draw pie for the first time
            this.drawPie();
            //Now that we have our loans from 'sync', bind to any further add/remove/change events to redraw the chart
            this.collection.on('add remove change reset', this.drawPie, this);
        },
        drawPie: function () {
            this.chart.series[0].setData(this.prepareGraphData());
        },
        /**
          * @desc create shaded colors.
          * @param {double} percentage - decimal value between -1.0 to 1.0 to control the shading. Positive will brighten toward white while negative will darken toward black.
          * @param {string} color - Hex or RGB value of a color to be shaded.
          * @return {string} - Hex or RGB value of the shaded color.
          * NOTE: Here's the doc: http://stackoverflow.com/questions/5560248/programmatically-lighten-or-darken-a-hex-color-or-rgb-and-blend-colors
        */
        shadeColor: function (percentage, color) {
            var f = '';
            var n = percentage < 0 ? percentage * -1 : percentage, u = Math.round, w = parseInt;
            if (color.length > 7) {
                f = color.split(","), t = (color ? color : percetnage < 0 ? "rgb(0,0,0)" : "rgb(255,255,255)").split(","), R = w(f[0].slice(4)), G = w(f[1]), B = w(f[2]);
                return "rgb(" + (u((w(t[0].slice(4)) - R) * n) + R) + "," + (u((w(t[1]) - G) * n) + G) + "," + (u((w(t[2]) - B) * n) + B) + ")";
            } else {
                f = w(color.slice(1), 16), t = w((color ? color : p < 0 ? "#000000" : "#FFFFFF").slice(1), 16), R1 = f >> 16, G1 = f >> 8 & 0x00FF, B1 = f & 0x0000FF;
                return "#" + (0x1000000 + (u(((t >> 16) - R1) * n) + R1) * 0x10000 + (u(((t >> 8 & 0x00FF) - G1) * n) + G1) * 0x100 + (u(((t & 0x0000FF) - B1) * n) + B1)).toString(16).slice(1);
            }
        },
        /**
          * @desc create new shaded color by calling the shadeBlend func above and control brightness with Highchart function
          * @param {string} color - Hex or RGB value of a color to be used for shading
          * @param {double} brightness - value between -1 to 1 to control whether the color is darkned or lightened
          * @return {string} - the RGB of the shaded color. 
        */
        generateColor: function (color, brightness) {               
            var shadedColor = this.shadeColor(0, color);
            return Highcharts.Color(shadedColor).brighten(brightness).get('rgb');
        },
        /**  
          * @desc change pie's sub-title to output total loan, total monthy.
          * @param {double} federalTotal - total amount of loans.
          * @param {double} monthlyTotal - total amount of monthly payments.
          * @return void - Use Highchart setTitle function to set the subtitle that displays calculated federalTotal and monthlyTotal and customize with inline css and _KWYO.scss 
        */
        setSubTitle: function (federalTotal, monthlyTotal) {  
            //"asaUtilities.currencyComma" add the comma at the thousands                                
            var fedTotal = asaUtilities.currencyComma(federalTotal);        
            var monTotal = asaUtilities.currencyComma(monthlyTotal);
            //Highchart function to add subtitle, first param is null mean it's for subtitle not title    
            this.chart.setTitle(null, {text:                            
                '<span align="center" style="color:#003765; display: block;">TOTAL OWED</span>' +
                '<span align="center" style="color:#d00b39; display: block;">$' + fedTotal + '</span> <br/>' + 
                '<span align="center" style="color:#003765; display: block;">Monthly Payment</span>' +  
                '<span align="center" style="color:#d00b39; display: block;">$' + monTotal + '</span>'
            });
        }
    });

    return Pie;
}); 
