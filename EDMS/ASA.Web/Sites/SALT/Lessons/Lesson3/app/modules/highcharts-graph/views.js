define([
  "lesson3/app",

  // Libs
  "backbone",
  "lesson3/plugins/asa-plugins"
],

function(app, Backbone, asaPlugins) {
  var Views = {};
  var dataModel; // holds the model that is passed in with updateGraph() call
  var viewsHolder = {};
  var balanceChart;
  var paymentChart;
  var chartInfoDisplay; // Renderer for add'l display data such as markers and labels
  var date = new Date();
  var monthOffset = date.getMonth() + 1;
  var yearlyRepaymentDataPoints = []; //Stores collection of repaymentData for the points that are being graphed

  var months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
  var monthsFull = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]; //not used for Lesson 1
  var years = [];
  var maxXAxisTickersDisplay = 12;

  var oneTimePaymentMarkerLabel_ImageSrc = "/Assets/images/lessons/lesson3/step5/ExtraPayment_markerLabel.png";
  var oneTimePaymentMarkerLabel_ImageWidth = 101;
  var oneTimePaymentMarkerLabel_ImageHeight = 32;

  var oneTimeExpenseMarkerLabel_ImageSrc = "/Assets/images/lessons/lesson3/step5/OneTimePurchase_markerLabel.png";
  var oneTimeExpenseMarkerLabel_ImageWidth = 127;
  var oneTimeExpenseMarkerLabel_ImageHeight = 32;
  //Vertical offset so marker labels don't overlaps\
  var markerLabelOverlapOffset = 35;

  /**
    default initial variables for graph
  */
  var cardBalance         = 23000;
  var interestRate        = 0.0;
  var monthlyPaid         = 400;

  //start modifiers
  var displayYears            = 11;
  var increaseMonthlyPaid     = 0;
  var modifiedInterestRate    = 0;

  var proxyInterest;

  //step 2 content view
  Views.HighChartsGraph = Backbone.View.extend({
    template: 'content/highcharts-graph',
    className: 'high-charts',
    beforeRender: function() {

    },

    initialize: function(options) {
     //save this view out so that sibling views (graphModifiers) can access it
      viewsHolder.content = this;

      /**
        bind all incoming triggered events
      */
      this.bind('updateGraph', this.updateGraph);
      this.bind('getMonthlyPayment', this.calculateMonthlyPayment);
      this.bind('updateHeaderCopy', this.updateHeaderCopy);
    },

    events: {
      'click .loan-balance': 'showBalanceGraph',
      'click .monthly-payment': 'showPaymentGraph',
      'click #show-graphs': 'showGraphs',
      'click #show-list': 'showList'
    },

    updateYearsArray: function(){
      years = [];

      for(i = 0; i < dataModel.getNumberOfYears() + 1; i ++){
        years.push(date.getFullYear() + i);
      }
    }, // END updateYearsArray()

    updateHeaderCopy: function(copy){
      $('#graph-header .heading').html(copy);
    },

    //Currently handles populating yearlyRepaymentDataPoints for graph points
    //as well as building and setting balance points for highcharts to draw
    //TODO Refactor out highcharts collection building and setting
    updateGraph: function(repaymentModel) {
      dataModel = repaymentModel;

      this.updateYearsArray();
      yearlyRepaymentDataPoints = dataModel.get('yearlyRepaymentPoints');

      this.initBalanceGraph();
      this.initPaymentGraph();

      //draw balance chart
      this.populateBalanceChart();

      //draw payment chart
      this.populatePaymentChart();

      //populate list view data
      this.populateListViewData();
    },

    populatePaymentChart: function(){
      var yearlyPaymentPoints = []; //Stores values for highchart to draw the standard payments
      var yearlyModifiedPaymentPoints = []; //Stores values for highchart to draw the modified payments

      var standardRepaymentPoints = app.router.repaymentModels.standardRepayment.get('yearlyRepaymentPoints');
      var standardRepaymentLength = standardRepaymentPoints.length; // figure out the length of standard repayment points
      var currentRepaymentDiff = dataModel.getNumberOfYears() + 1 - standardRepaymentLength; // get the difference between standard and this current repayment option

      /**
        Standard repayment only calculates 10 years
        there may be a point where the paymentChart needs to show beyond that. so we need to fill standardRepaymentPoints with null values for monthlyPayment so that it does not break
      */
      for(j = 0; j < currentRepaymentDiff; j ++){
        standardRepaymentPoints.push({monthlyPayment: null});
      }

      for(i=0; i < standardRepaymentPoints.length; i++) {
        yearlyPaymentPoints.push(standardRepaymentPoints[i].monthlyPayment);
      }
      for(i=0; i < yearlyRepaymentDataPoints.length; i++) {
        yearlyModifiedPaymentPoints.push(yearlyRepaymentDataPoints[i].monthlyPayment);
      }

      //set standard repayment points
      paymentChart.series[1].setData(yearlyPaymentPoints, false);
      paymentChart.series[0].setData(yearlyModifiedPaymentPoints, false);

      if (yearlyPaymentPoints[0] != yearlyModifiedPaymentPoints[0]) {

      }

      paymentChart.redraw();
    },

    populateBalanceChart: function() {
      var yearlyBalancePoints = []; //Stores values for highchart to draw

      for(i=0; i < yearlyRepaymentDataPoints.length; i++) {
        yearlyBalancePoints.push(yearlyRepaymentDataPoints[i].currentBalance);
      }

      balanceChart.series[0].setData(yearlyBalancePoints, false);
      balanceChart.redraw();
    },

    populateListViewData: function() {
      var baseListViewElement = $('#list-view-data');
      var yearlyRepaymentPoints = dataModel.get('yearlyRepaymentPoints');
      var year0 = yearlyRepaymentPoints[0];
      var yearLast = yearlyRepaymentPoints[yearlyRepaymentPoints.length - 1];

      var principal = dataModel.get('totalBalance');

      //default totalInterestPaid to 0 if not defined in yearLast repaymentPoint
      var totalInterestPaid = typeof yearLast.interestAccrued === 'undefined' ? 0 : yearLast.interestAccrued;
      var totalAmountPaid = principal + totalInterestPaid;

      var gracePeriod = dataModel.get('gracePeriod');
      var deferment = dataModel.get('deferment');
      var forbearance = dataModel.get('forbearance');

      baseListViewElement.find('#total-repaid').text($.commify(totalAmountPaid.toFixed(2), {prefix: '$'}));

      var repaymentPeriod = dataModel.getNumberOfYears();
      baseListViewElement.find('#repayment-period').text(dataModel.getNumberOfYears());

      baseListViewElement.find('#interest-paid').text($.commify(totalInterestPaid.toFixed(2), {prefix: '$'}));

      baseListViewElement.find('#monthly-payment').text($.commify( (year0.monthlyPayment).toFixed(2), {prefix: '$'}));

      baseListViewElement.find('#grace-period').text(gracePeriod);

      baseListViewElement.find('#deferment').text(deferment);

      baseListViewElement.find('#forbearance').text(forbearance);
    },

    showGraphs: function(){
      app.wt.trigger('lesson:step' + app.router.currentStep + ':showGraphs', {
        user: app.user.get('UserId'),
        time: new Date()
      });

      $('#view-selector .selected').removeClass('selected');
      $('#view-selector #show-graphs').addClass('selected');

      $('#graphs-view').show();
      $('#list-view').hide();

      $(window).trigger('resize');

      return false;
    },

    showList: function(){
      app.wt.trigger('lesson:step' + app.router.currentStep + ':showList', {
        user: app.user.get('UserId'),
        time: new Date()
      });

      $('#view-selector .selected').removeClass('selected');
      $('#view-selector #show-list').addClass('selected');

      $('#graphs-view').hide();
      $('#list-view').show().removeClass('hidden');

      return false;
    },

    showBalanceGraph: function(event){
      // app.wt.trigger('lesson:step' + app.router.currentStep + ':showBalanceGraph', {
      //   user: app.user.get('UserId'),
      //   time: new Date()
      // });
      app.wt.trigger('lesson:graph:optionChange', {
        graph: {
          option: 'loan-balance'
        },
        step: {
          number: app.router.currentStep
        }
      });

      $('#balance-graph-container').show();
      $('#payment-graph-container').hide();

      $('#graph-nav .active').removeClass('active');
      $('#graph-nav .loan-balance').addClass('active');

      $(window).trigger('resize');
      return false;
    },

    showPaymentGraph: function(event){
      // app.wt.trigger('lesson:step' + app.router.currentStep + ':showPaymentGraph', {
      //   user: app.user.get('UserId'),
      //   time: new Date()
      // });
      app.wt.trigger('lesson:graph:optionChange', {
        graph: {
          option: 'monthly-payment'
        },
        step: {
          number: app.router.currentStep
        }
      });

      $('#payment-graph-container').show();
      $('#balance-graph-container').hide();

      $('#graph-nav .active').removeClass('active');
      $('#graph-nav .monthly-payment').addClass('active');

      $(window).trigger('resize');

      return false;
    },

    afterRender: function(){
      //TODO:
      //determines which graph to show first
      $('#payment-graph-container').hide();
      $('#graph-nav .loan-balance').addClass('active');

      if(dataModel.get('hideAdjustedKey')){
        $('#key .red').hide();
      }
    },

    drawGraphDataOverlays: function() {
      clearChartDataOverlays();
      init();

      function init(){
        drawLineMarkers();
      }

      function clearChartDataOverlays(){
        // Clear existing labels
        if (chartInfoDisplay !== undefined) {
          // Destroy existing labels group (since I can't seem to destroy indiv elements)
          chartInfoDisplay.destroy();
        }
        // Initialize a new renderer group for the labels
        chartInfoDisplay = balanceChart.renderer.g().add();
        chartInfoDisplay.toFront();
      }

      function drawLineMarkers(){
        //Show One time payment marker, if exists
        if(addOneTimePayment){
          drawOneTimePaymentMarker();
        }

        if(oneTimeExpenses && !ignoreOneTimeExpenses){
          var expense;
          for(var i = 0; i < oneTimeExpenses.length; i++){
            drawOneTimeExpenseMarker(i);
          }
        }
      }

      function drawOneTimePaymentMarker() {
        var oneTimePaymentPoint = balanceChart.series[1].data[oneTimePayment.month];
        drawLineMarker(oneTimePaymentPoint);
        drawLineMarkerLabel(oneTimePaymentPoint, oneTimePaymentMarkerLabel_ImageSrc, oneTimePaymentMarkerLabel_ImageWidth, oneTimePaymentMarkerLabel_ImageHeight);
      }

      //We need to know index so that marker label can be set at correct height, to guarantee no overlap of labels
      function drawOneTimeExpenseMarker(oneTimeExpenseIndex){
        var oneTimeExpensePoint = balanceChart.series[1].data[oneTimeExpenses[oneTimeExpenseIndex].month];
        drawLineMarker(oneTimeExpensePoint);
        drawLineMarkerLabel(oneTimeExpensePoint,
          oneTimeExpenseMarkerLabel_ImageSrc,
          oneTimeExpenseMarkerLabel_ImageWidth,
          oneTimeExpenseMarkerLabel_ImageHeight,
          oneTimeExpenseIndex + 1); //Index is +1 because one-time payment is 0 offset
      }

      function drawLineMarker(point) {
        // Add the vertical marker line at the X position for the passed-in point
        //balanceChart.renderer.rect: x, y, width, height, corner radius(?)

        balanceChart.renderer.rect(
        point.plotX + balanceChart.plotLeft,
        balanceChart.plotTop,
        2, balanceChart.plotHeight
        ).attr({
          fill: 'black',
          zIndex: 5
        }).add(chartInfoDisplay);
      }

      function drawLineMarkerLabel(point, imageSrc, imageWidth, imageHeight, offsetCount) {
        offsetCount = typeof offsetCount !== 'undefined' ? offsetCount : 0;

        var xPos;
        var yPos;

        // subtract height of .png
        yPos = balanceChart.plotHeight + balanceChart.plotTop - imageHeight;
        xPos = point.plotX + balanceChart.plotLeft - imageWidth,

        // Add the label
        // image: source, x, y, width, height
        balanceChart.renderer.image(
        imageSrc,
        xPos,
        yPos - (offsetCount * markerLabelOverlapOffset),
        imageWidth,
        imageHeight
        ).attr({
            zIndex: 6
          }).add(chartInfoDisplay);
      }
    },

    initBalanceGraph: function(){
      var newPoints;

      // Initialize balanceChart look and feel
      balanceChart = new Highcharts.Chart({
        chart: {
          renderTo: 'graph',
          type: 'area',
          marginTop: 140,
          marginRight: 75,
          marginLeft: 144,
          backgroundColor:'rgba(255, 255, 255, 0.1)'
          //zoomType: 'x'
        },

        credits: {
          enabled: false
        },

        exporting: {
          enabled: false
        },

        title: {
          text: ' '
        },

        legend: {
          enabled: false,
          borderWidth: 0
        },

        xAxis: {
          title: {
            text: ''
          },

          tickInterval: 1,

          labels: {
            formatter: function() {
              var inc;

              if (typeof this.value !== 'undefined') {
                var numberOfPoints = this.chart.series[0].xIncrement;

                //We need to calculate which markers to not show if too many present on the graph
                //displayTickCount used to calculate which should be skipped
                //Add on top of 1 for modulus math below
                var displayTickCount = 1 + Math.floor( numberOfPoints / maxXAxisTickersDisplay);

                if (this.value % displayTickCount === 0) {
                  var shortYear;
                  if (years[this.value]) {
                    shortYear = years[this.value].toString().substring(2);
                    inc = months[monthOffset % 12] + " '" + shortYear;
                  } else {
                    inc = "";
                  }
                } else {
                  inc = "";
                }

                return inc;
              }
            }
          },

          gridLineDashStyle: 'Dot',
          gridLineColor: '#bfbfbf',
          gridLineWidth: 1
        },

        yAxis: {
          title: {
            text: ''
          },

          labels: {
            formatter: function() {
              if (this.value < 0) {
                //if number is negative change the placement of the minus sign to before the dollar sign
                var num = this.value;
                var str = num.toString();
                var n = str.replace(/\-/gi, "-$");

                return n;
              } else {
                return '$' + this.value;
              }
            }
          },

          gridLineDashStyle: 'Dot',
          gridLineColor: '#bfbfbf'
        },

        tooltip: {
          enabled: true,
          crosshairs: true,
          useHTML: true,
          /* backgroundColor:'rgba(255, 255, 255, 0.1)', */
          backgroundColor: '#272525',
          shadow: false,
          borderWidth: 0,

          style: {
            padding: '0px',
            color: '#ffffff'
          },

          positioner: function(boxWidth, boxHeight, point) {
            var y = point.plotY - 5;

            //dont let the plotY get down to 0 to prevent cut off
            if (point.plotY < 20 ) {
              y = 20;
            }

            return { x: point.plotX + 50, y: y };
            //return { x: point.plotX + 50, y: point.plotY - 35 };
            //return { x: this.balanceChart.plotLeft, y: this.balanceChart.plotTop - 40 };
          },

          formatter: function() {
            var tip;

            tip = '<div style="background: #272525; border-radius: 3px; color: #ffffff; text-align: center; width: 172px; padding: 10px; position: relative; z-index: 8000; margin-left: -10px;">' + monthsFull[monthOffset % 12] + ' ' + years[Math.floor(this.x)] +'<p>Monthly payment: ' + $.commify( (yearlyRepaymentDataPoints[this.points[0].key].monthlyPayment).toFixed(2), { prefix:'$' }) + '</p><p>Interest accrued: ' + $.commify(Math.round(yearlyRepaymentDataPoints[this.points[0].key].interestAccrued), { prefix:'$' }) + '</p><p>Balance: ' + $.commify( (yearlyRepaymentDataPoints[this.points[0].key].currentBalance).toFixed(2), { prefix:'$' }) + '</p><div style="width: 17px; height: 17px; position: absolute; left: 95px; bottom: -11px;" class="wing"><img src="/Assets/images/lessons/lesson3/step5/highcharts-tooltip-carrot.png" /></div></div>';

            return tip;
          },

          shared: true
        },

        plotOptions: {
          area: {
            stacking: 'normal',

            marker: {
              enabled: true,
              symbol: 'circle',
              radius: 2
            }
          },

          series: {
            pointPadding: 0,
            groupPadding: 0
          }
        },

        series: [
          {
            //Series 0
            name: 'Balance',
            color: '#99f3c6',
            shadow: false,
            lineWidth: 3
          }
        ]
      });

      // Initialize a new renderer group for the markers and labels
      chartInfoDisplay = balanceChart.renderer.g().add();
      chartInfoDisplay.toFront();
    }, // END initBalanceGraph

    initPaymentGraph: function() {
      var newPoints;

      // Initialize chart look and feel
      paymentChart = new Highcharts.Chart({
        chart: {
          renderTo: 'payment-graph',
          type: 'line',
          marginTop: 140,
          marginRight: 75,
          marginLeft: 144,
          backgroundColor:'rgba(255, 255, 255, 0.1)'
          //zoomType: 'x'
        },

        credits: {
          enabled: false
        },

        exporting: {
          enabled: false
        },

        title: {
          text: ' '
        },

        legend: {
          enabled: false,
          borderWidth: 0
        },

        xAxis: {
          title: {
            text: ''
          },

          tickInterval: 1,

          labels: {
            formatter: function() {
              var inc;

              /*
              if(balancePoints.length > 30){
                inc = years[(this.value + monthOffset) / 12];
              } else {
                var index = (this.value + monthOffset) % 12;
                inc = months[index];
              }
              */

              inc = years[this.value];

              return inc;
            }
          },

          gridLineDashStyle: 'Dot',
          gridLineColor: '#bfbfbf',
          gridLineWidth: 1
        },

        yAxis: {
          title: {
            text: ''
          },

          labels: {
            formatter: function() {
              if (this.value < 0) {
                //if number is negative change the placement of the minus sign to before the dollar sign
                var num = this.value;
                var str = num.toString();
                var n = str.replace(/\-/gi, "-$");

                return n;
              } else {
                return '$' + this.value;
              }
            }
          },

          gridLineDashStyle: 'Dot',
          gridLineColor: '#bfbfbf'
        },

        tooltip: {
          enabled: true,
          crosshairs: true,
          useHTML: true,
          /* backgroundColor:'rgba(255, 255, 255, 0.1)', */
          backgroundColor: '#272525',
          shadow: false,
          borderWidth: 0,

          style: {
            padding: '0px',
            color: '#ffffff'
          },

          positioner: function(boxWidth, boxHeight, point) {
            var y = point.plotY - 5;

            //dont let the plotY get down to 0 to prevent cut off
            if (point.plotY < 20 ) {
              y = 20;
            }

            return { x: point.plotX + 50, y: y };
            //return { x: point.plotX + 50, y: point.plotY - 35 };
            //return { x: this.chart.plotLeft, y: this.chart.plotTop - 40 };
          },

          formatter: function() {
            var tip;

            //tip = '<div style="background: #272525; border-radius: 3px; color: #ffffff;text-align: center; width: 150px; padding: 10px;position: relative; z-index: 8000;">' + monthsFull[monthOffset] + ' ' + years[Math.floor(this.x)] +'<p>Payment applied: ' + $.commify(yearlyRepaymentDataPoints[this.points[0].key], { prefix:'$' }) + '</p><p>Monthly Interest: ' + $.commify(thisMonthsInterest, { prefix:'$' }) + '</p><p>Balance: ' + $.commify(thisMonthsBalance, { prefix:'$' }) + '</p><div style="width: 17px; height: 17px; position: absolute; left: 81px; bottom: -12px;" class="wing"><img src="/Assets/images/lessons/lesson3/step5/highcharts-tooltip-carrot.png" /></div></div>';

            return tip;
          },

          shared: true
        },

        plotOptions: {
          area: {
            stacking: 'normal',

            marker: {
              enabled: true,
              symbol: 'circle',
              radius: 2
            }
          },

          series: {
            pointPadding: 0,
            groupPadding: 0
          }
        },

        series: [
          {
            //Adjusted payment
            name: 'adjusted',
            color: '#ff0099',
            shadow: false,
            lineWidth: 3
          },

          {
            //Standard payment
            name: 'payment',
            color: '#05e1f5',
            shadow: false,
            lineWidth: 3
          }
        ]
      });
    } // END initPaymentGraph
  });

  return Views;
});
