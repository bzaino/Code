define([
    'lesson2/app',
    'lesson2/modules/header',
    'lesson2/modules/bar-graph',
    'lesson2/modules/graph-modifiers',
    'lesson2/modules/footer',

    // Libs
    'backbone',
    'lesson2/plugins/asa-plugins',
    'asa/asaWebService'
],

function (app, header, barGraph, graphModifiers, footer, Backbone, asaPlugins) {


  var Views = {};

  var viewsHolder = {};
  var chart;
  // Renderer for add'l display data such as markers and labels
  var chartInfoDisplay;

  var balancePoints = [];
  var interestPoints = [];  //Stores the values used for graph display
  var calculatedInterestPoints = [];  //Stores the monthly values, for use by the header
  var calculatedInterestTotal = 0;  //Stores the total interest accumulated
  var appliedPayments = [];

  var months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
  var monthsFull = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]; //not used for Lesson 1

  // Array containing the next 10 years for use with the graph
  var years = (function getNextTenYears() {
    var NUM_YEARS = 10,
        nextTenYears = [],
        now = new Date(),
        currentYear = now.getFullYear();

    for (var i = 0; i < NUM_YEARS; i++) {
      nextTenYears.push(currentYear + i);
    }

    return nextTenYears;
  })();

  var date = new Date();
  var monthOffset = (date.getMonth() + 1) % 12;

  var oneTimePaymentMarkerLabel_ImageSrc = "/assets/images/lessons/lesson2/step5/ExtraPayment_markerLabel.png";
  var oneTimePaymentMarkerLabel_ImageWidth = 101;
  var oneTimePaymentMarkerLabel_ImageHeight = 32;

  var oneTimeExpenseMarkerLabel_ImageSrc = "/assets/images/lessons/lesson2/step5/OneTimePurchase_markerLabel.png";
  var oneTimeExpenseMarkerLabel_ImageWidth = 127;
  var oneTimeExpenseMarkerLabel_ImageHeight = 32;

  //Vertical offset so marker labels don't overlaps\
  var markerLabelOverlapOffset = 35;

  /**
  default initial variables for graph
  */

  var cardBalance = 0;
  var interestRate = .065;
  var recurringExpenses = 300;
  var monthlyPaid = 100;

  var oneTimeExpenses = [
      { value: 500, month: 2 }, { value: 500, month: 6 }, { value: 500, month: 10 }
  ];

  var defaultInterestRate = .065;

  //start modifiers
  var displayYears = 1;
  var increaseMonthlyPaid = 0;
  var ignoreRecurringExpenses = false;
  var addOneTimePayment = false;
  var ignoreOneTimeExpenses = false;
  var modifiedInterestRate = 0;
  var useModifiedInterest = false;

  var oneTimePayment = { value: 0, month: 1 + monthOffset };

  var proxyInterest;


  //step 5 content view
  Views.step5Content = Backbone.View.extend({
    template: 'content/step5',
    className: 'step5-content',
    beforeRender: function () {

    },
    initialize: function (options) {

      /**
      import initial variables from steps 1 - 4 models
      */
      //only import if the models are there
      //otherwise use defaults values listed above
      if (typeof options.models.step1 !== 'undefined') {

        cardBalance = options.models.step1.get('balance');
        interestRate = options.models.step1.get('interestRate') * .01;

        monthlyPaid = options.models.step4.get('payment');

        recurringExpenses = options.models.step2.get('collectionSum');

        oneTimeExpenses = [];
        options.collections.oneTimeExpense.each(function (element, index, list) {
          oneTimeExpenses.push({ value: element.get('value'), month: element.get('month') });
        });


      }

      this.graphModifiersModel = options.graphModifiersModel;


      //save this view out so that sibling views (graphModifiers) can access it
      viewsHolder.content = this;

      //bind all incoming events from graphModifiers view
      this.bind('handleIncreasePayment', this.handleIncreasePayment);
      this.bind('handleIncreasePaymentValueChange', this.handleIncreasePaymentValueChange);

      this.bind('handleIgnoreRecurringExpenses', this.handleIgnoreRecurringExpenses);

      this.bind('handleIgnoreOneTimeExpenses', this.handleIgnoreOneTimeExpenses);

      this.bind('handleExtraPayment', this.handleExtraPayment);
      this.bind('handleExtraPaymentValueChange', this.handleExtraPaymentValueChange);
      this.bind('handleExtraPaymentMonthChange', this.handleExtraPaymentMonthChange);
      this.bind('handleExtraPaymentValueKeyUp', this.handleExtraPaymentValueKeyUp);

      this.bind('handleChangeInterest', this.handleChangeInterest);
      this.bind('handleModifiedInterest', this.handleModifiedInterest);
      this.bind('handleModifiedInterestChange', this.handleModifiedInterestChange);

      this.bind('updateGraph', this.updateGraph);

      this.activeStep = app.router.activeStep;
      this.preloaded = false;

      app.wt.trigger('lesson:step:start', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        step: {
          number:    this.activeStep,
          preloaded: this.preloaded
        }
      });
    },

    events: {
      'click #time-scale a': 'handleYearChange'

    },

    handleIncreasePayment: function (event) {
      var element = $(event.target);
      var value = element.parents('.section').find('#increase-payment-value').val();
      var cleanValue = Number(value.replace(/[^0-9\.-]+/g, "")); // UÂ©nescaped '-'.
      var checked = false;
      if (element.is(':checked')) {
        checked = true;
        increaseMonthlyPaid = cleanValue;
      } else {
        increaseMonthlyPaid = 0;
      }

      this.graphModifiersModel.set({ increaseMonthlyPayment: increaseMonthlyPaid }, { silent: true });
      this.graphModifiersModel.set({ increaseMonthlyPaymentChecked: checked }, { silent: true });

      this.updateGraph();
    },
    handleIncreasePaymentValueChange: function (event) {
      var element = $(event.target);
      var cleanValue = Number(element.val().replace(/[^0-9\.-]+/g, "")); // Unescaped '-'.

      increaseMonthlyPaid = cleanValue;

      //      this.model.set({ increaseMonthlyPaid: increaseMonthlyPaid });
      this.graphModifiersModel.set({ increaseMonthlyPayment: increaseMonthlyPaid }, { silent: true });
    },

    handleIgnoreRecurringExpenses: function (event) {
      var element = $(event.target);

      if (element.is(':checked')) {
        ignoreRecurringExpenses = true;
      } else {
        ignoreRecurringExpenses = false;
      }

      //      this.model.set({ ignoreRecurringExpenses: ignoreRecurringExpenses },{ silent: true });
      this.graphModifiersModel.set({ cashForRecurringExpenses: ignoreRecurringExpenses }, { silent: true });
      this.updateGraph();
    },

    handleIgnoreOneTimeExpenses: function (event) {
      var element = $(event.target);

      if (element.is(':checked')) {
        ignoreOneTimeExpenses = true;
      } else {
        ignoreOneTimeExpenses = false;
      }

      //this.model.set({ ignoreOneTimeExpenses: ignoreOneTimeExpenses });
      this.graphModifiersModel.set({ cashForOneTimePurchases: ignoreOneTimeExpenses }, { silent: true });
      this.updateGraph();
    },

    handleExtraPayment: function (event) {
      var element = $(event.target);
      var value = element.parents('.section').find('#extra-payment-value').val();
      var cleanValue = Number(value.replace(/[^0-9\.-]+/g, "")); // Unescaped '-'.
      var checked = false;

      if (element.is(':checked')) {
        checked = true;
        addOneTimePayment = true;
        if (parseInt(cleanValue)) {
          oneTimePayment.value = cleanValue;
        }

      } else {
        checked = false;
        addOneTimePayment = false;
        oneTimePayment.value = 0;
      }

      //      this.model.set({ addOneTimePayment: addOneTimePayment });
      //      this.model.set({ oneTimePaymentValue: oneTimePayment.value });
      this.graphModifiersModel.set({ extraPaymentAmount: oneTimePayment.value }, { silent: true });
      this.graphModifiersModel.set({ extraPaymentChecked: checked }, { silent: true });

      this.updateGraph();
    },
    handleExtraPaymentValueChange: function (event) {
      var element = $(event.target);

      oneTimePayment.value = element.val();
      //      this.model.set({ oneTimePaymentValue: oneTimePayment.value });
      this.graphModifiersModel.set({ extraPaymentAmount: oneTimePayment.value }, { silent: true });
    },
    handleExtraPaymentValueKeyUp: function (event) {
      var element = $(event.target);
      var cleanValue = element.val().replace(/[^0-9\.-]+/g, ""); // Unescaped '-'.

      oneTimePayment.value = cleanValue;
      //      this.model.set({ oneTimePaymentValue: oneTimePayment.value });
      this.graphModifiersModel.set({ extraPaymentAmount: oneTimePayment.value }, { silent: true });
    },
    handleExtraPaymentMonthChange: function (month) {

      oneTimePayment.month = parseInt(month);
      //      this.model.set({ oneTimePaymentMonth: oneTimePayment.month });
      this.graphModifiersModel.set({ extraPaymentMonth: oneTimePayment.month }, { silent: true });
    },
    //toggle checkbox
    handleChangeInterest: function (event) {
      var element = $('#change-interest');
      var interestInput = element.parents('.section').find('#modified-interest');
      var value = interestInput.val();
      var cleanValue = value.replace(/[^0-9\.-]+/g, "");
      if (isNaN(cleanValue) || cleanValue == "" || cleanValue <= 0) {
        // Set the default interest rate if invalid
        cleanValue = (defaultInterestRate * 100);
      }

      if (element.is(':checked')) {
        useModifiedInterest = true;
      } else {
        useModifiedInterest = false;
        //When un-selecting custom interest we need to reset interest to 0
        cleanValue = 0;
      }

      interestInput.val(cleanValue + "%");
      modifiedInterestRate = parseFloat(cleanValue) * .01;

      //      this.model.set({ modifiedInterestRate: modifiedInterestRate });
      this.graphModifiersModel.set({ lowerInterestRateChecked: useModifiedInterest }, { silent: true });
      this.graphModifiersModel.set({ lowerInterestRate: modifiedInterestRate }, { silent: true });
      this.updateGraph();
    },
    handleModifiedInterestChange: function (event) {
      var element = $(event.target);
      var value = element.val();
      var cleanValue = value.replace(/[^0-9\.-]+/g, "");
      if (Number(cleanValue)) {
        if (cleanValue > 100) {
          element.val(100);
          cleanValue = 100;
        } else if (cleanValue < 0) {
          element.val(0);
          cleanValue = 0;
        }

        modifiedInterestRate = parseFloat(cleanValue) * .01;
        //        this.model.set({ modifiedInterestRate: modifiedInterestRate });
        this.graphModifiersModel.set({ lowerInterestRate: modifiedInterestRate }, { silent: true });
        this.updateGraph();
      }
    },
    handleModifiedInterest: function (event) {
      var element = $(event.target);
      var value = element.parents('.section').find('#modified-interest').val();
      var cleanValue = value.replace(/[^0-9\.-]+/g, "");

      modifiedInterestRate = parseFloat(cleanValue) * .01;
      //      this.model.set({ modifiedInterestRate: modifiedInterestRate });
      this.graphModifiersModel.set({ lowerInterestRate: modifiedInterestRate }, { silent: true });
      this.updateGraph();
    },
    handleYearChange: function (event) {
      var newScope = $(event.target).attr('data-years');
      $('#time-scale .selected').removeClass('selected');

      $(event.target).parent().addClass('selected');

      displayYears = newScope;

      app.wt.trigger('lesson:graph:optionChange', {
        user: Backbone.Asa.User.userGuid,
        actionName: 'whatif',
        graph: {
          option: displayYears + ' years'
        }
      });

      this.updateGraph();

      return false;

    },

    afterRender: function () {
      var self = this;

      setTimeout(function () {
        self.updateGraph();
      }, 200);
    },

    drawGraphDataOverlays: function () {
      clearChartDataOverlays();
      init();

      function init() {
        drawLineMarkers();
      };

      function clearChartDataOverlays() {

        // Clear existing labels
        if (chartInfoDisplay != undefined) {
          // Destroy existing labels group (since I can't seem to destroy indiv elements)
          chartInfoDisplay.destroy();
        }

        // Initialize a new renderer group for the labels
        if (chart) {
          chartInfoDisplay = chart.renderer.g().add();
          chartInfoDisplay.toFront();
        }
      }

      function drawLineMarkers() {
        //Show One time payment marker, if exists
        if (addOneTimePayment) {
          drawOneTimePaymentMarker();
        }

        if (oneTimeExpenses && !ignoreOneTimeExpenses) {
          var expense;
          for (var i = 0; i < oneTimeExpenses.length; i++) {
            drawOneTimeExpenseMarker(i);
          }
        }
      };

      function drawOneTimePaymentMarker() {
        var monthIndex = (11 - (monthOffset - oneTimePayment.month)) % 12;
        var oneTimePaymentPoint = chart.series[1].data[monthIndex];
        drawLineMarker(oneTimePaymentPoint);
        drawLineMarkerLabel(oneTimePaymentPoint, oneTimePaymentMarkerLabel_ImageSrc, oneTimePaymentMarkerLabel_ImageWidth, oneTimePaymentMarkerLabel_ImageHeight);
      }

      //We need to know index so that marker label can be set at correct height, to guarantee no overlap of labels
      function drawOneTimeExpenseMarker(oneTimeExpenseIndex) {
        var monthIndex = (11 - (monthOffset - oneTimeExpenses[oneTimeExpenseIndex].month)) % 12;
        var oneTimeExpensePoint = chart.series[1].data[monthIndex];
        drawLineMarker(oneTimeExpensePoint);
        drawLineMarkerLabel(oneTimeExpensePoint,
          oneTimeExpenseMarkerLabel_ImageSrc,
          oneTimeExpenseMarkerLabel_ImageWidth,
          oneTimeExpenseMarkerLabel_ImageHeight,
          oneTimeExpenseIndex + 1); //Index is +1 because one-time payment is 0 offset
      }

      function drawLineMarker(point) {
        // Add the vertical marker line at the X position for the passed-in point
        //chart.renderer.rect: x, y, width, height, corner radius(?)

        chart.renderer.rect(
        point.plotX + chart.plotLeft,
        chart.plotTop,
        2, chart.plotHeight
        ).attr({
          fill: 'black',
          zIndex: 5
        }).add(chartInfoDisplay);
      };

      function drawLineMarkerLabel(point, imageSrc, imageWidth, imageHeight, offsetCount) {

        offsetCount = typeof offsetCount !== 'undefined' ? offsetCount : 0;

        var xPos;
        var yPos;

        // subtract height of .png
        yPos = chart.plotHeight + chart.plotTop - imageHeight;
        xPos = point.plotX + chart.plotLeft - imageWidth,

        // Add the label
        // image: source, x, y, width, height
        chart.renderer.image(
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
    updateGraph: function () {
      this.calculatePoints();
      this.drawGraphDataOverlays();
    },
    updateGraphHeader: function () {
      var balanceLength = balancePoints.length - 1;
      var interestLength = interestPoints.length - 1;


      $('#graph-header .months').text(displayYears * 12);
      $('#graph-header .interest').text( Math.round(calculatedInterestTotal) ).formatCurrency({ roundToDecimalPlace: 0 });
      $('#graph-header .balance').text( balancePoints[balanceLength] + Math.round(interestPoints[interestLength]) ).formatCurrency({ roundToDecimalPlace: 2 });


    },
    calculatePoints: function () {
      //FROM SPEC SHEET, ORDER OF MONTHLY CALCULATIONS
      //Apply interest to previous balance
      //Apply monthly charges (recurring and one-time)
      //Apply payments to balance, interest
      //Due to Highcharts, the values passed into arrays need to be running totals, not monthly values


      //reset the points arrays
      balancePoints = [];
      interestPoints = [];
      calculatedInterestPoints = [];
      appliedPayments = [];

      var interestTotal = 0; //Stores running total of interest, for graph display
      calculatedInterestTotal = 0;
      var paymentTotal = 0;  //Stores running total of payments

      var workingBalance = 0; //Used for calculations within a given month, not involved with interest
      var monthlyInterest = 0; //Interest-only for a given month. Could be negative, to represent months where payment > cost of interest
      var monthlyBalancePlusInterest = 0; //Total card balance for a given month
      var monthlyBalanceAfterPayment = 0; //Total card balance for a given month
      var previousBalancePlusInterest = 0; //Stores the calculation of previous balance plus interest applied
      var totalPaymentForMonth = 0; //Total amount paid in a given month
      var totalCostsForMonth = 0; //Total of expenses (recurring, one-time) in a given month
      var endOfMonthBalance = cardBalance; //Stores final balance (Principal + interest - payments) for a month, carried over to the next month to base calculations off it
      var graphBalance = 0; //Stores balance value minus interest total, for showing in graph

      var proxyOneTimePay = 0;

      for (var i = 0; i < displayYears * 12.2; i++) {
        //Prepare some values used in calculation
        if (modifiedInterestRate != 0) {
          proxyInterest = modifiedInterestRate;
        } else {
          proxyInterest = interestRate;
        }

        proxyOneTimePay = 0;
        if (i < 12){ // only apply one time payments during the first year
          var oneTimePaymentMonth = (12 - (monthOffset - oneTimePayment.month)) % 12;
          if (typeof oneTimePayment !== 'undefined' && oneTimePaymentMonth == (i + 1) % 12) {
            proxyOneTimePay = oneTimePayment.value;
          }
        }

        totalPaymentForMonth = monthlyPaid + increaseMonthlyPaid + proxyOneTimePay;

        //1. Apply monthly charges (recurring and one-time)
        totalCostsForMonth = 0;
        //Apply recurring expenses
        if (ignoreRecurringExpenses === false) {
          totalCostsForMonth = totalCostsForMonth + recurringExpenses;
        }

        //Apply one time expenses, if applicable for the given month and only during the first year
        if (ignoreOneTimeExpenses === false && i < 12) {
          for (var j = 0; j < oneTimeExpenses.length; j++) {
            var oneTimeExpenseMonth = (12 - (monthOffset - oneTimeExpenses[j].month)) % 12;
            if (oneTimeExpenseMonth == (i + 1) % 12) {
              totalCostsForMonth = totalCostsForMonth + oneTimeExpenses[j].value;
            }
          }
        }

        //Apply interest to balance
        //We don't do calculations over time, since every month will have a new starting balance due to recurring / one-time expenses

        //Interest is accrued on previous principal
        previousBalancePlusInterest = endOfMonthBalance * (1 + (proxyInterest / 12));
        monthlyInterest = Math.round((previousBalancePlusInterest - endOfMonthBalance) * 100) / 100;

        //Add new costs
        monthlyBalancePlusInterest = previousBalancePlusInterest + totalCostsForMonth;

        //Apply payments to balance

        //ENSURE TOTAL BALANCE DOESNT GO BELOW 0
        if ((monthlyBalancePlusInterest + interestTotal)- totalPaymentForMonth <= 0) {
          totalPaymentForMonth = monthlyBalancePlusInterest + interestTotal;
        }

        endOfMonthBalance = Math.round((monthlyBalancePlusInterest - totalPaymentForMonth) * 100) / 100;
        paymentTotal = paymentTotal + totalPaymentForMonth;

        calculatedInterestTotal = calculatedInterestTotal + monthlyInterest;
        interestTotal = interestTotal + monthlyInterest;
        if (monthlyInterest + totalCostsForMonth - totalPaymentForMonth < 0) {
          //Graph has negative slope, 0 graph display
          interestTotal = 0;
        }

        //We need to account for interestTotal increase in the graph, so subtract it from endOfMonthBalance
        graphBalance = endOfMonthBalance - interestTotal;
        //don't allow balance to go negative
        if (endOfMonthBalance < 0) {
          endOfMonthBalance = graphBalance = 0;
        }

        //Save monthly data into corresponding arrays
        balancePoints.push(graphBalance);
        interestPoints.push(interestTotal);
        calculatedInterestPoints.push(monthlyInterest);
        appliedPayments.push(paymentTotal);
      } // END points loop

      // update chart data and redraw
      if (chart) {
        chart.series[1].setData(balancePoints, false);
        chart.series[0].setData(interestPoints, false);

        chart.redraw();
      }
      this.updateGraphHeader();
    }, // END calculatePoints()
    initGraph: function () {
      var newPoints;

      // Initialize chart look and feel
      chart = new Highcharts.Chart({
        chart: {
          renderTo: 'graph',
          type: 'area',
          marginTop: 140,
          marginRight: 75,
          marginLeft: 144,
          backgroundColor: 'rgba(255, 255, 255, 0.1)'
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
            formatter: function () {
              var inc;

              if (balancePoints.length > 30) {
                inc = years[(this.value + monthOffset) / 12];
              } else {
                var index = (this.value + monthOffset) % 12;
                inc = months[index];
              }

              return inc;
            }
          },
          gridLineDashStyle: 'Dot',
          gridLineColor: '#bfbfbf',
          gridLineWidth: 1
        }, //END xAxis
        yAxis: {
          title: {
            text: ''
          },
          labels: {
            formatter: function () {
              if (this.value < 0) {
                //if number is negative change the placement of the minus sign to before the dollar sign
                var num = this.value;
                var str = num.toString();
                var n = str.replace(/\-/gi, "\-\$");
                return n;
              } else {
                return '$' + this.value;
              }
            }
          },
          gridLineDashStyle: 'Dot',
          gridLineColor: '#bfbfbf'
        }, //END yAxis
        tooltip: {
          enabled: true,
          crosshairs: true,
          useHTML: true,
          /* backgroundColor:'rgba(255, 255, 255, 0.1)', */
          backgroundColor: '#272525',
          shadow: false,
          shared: true,
          borderWidth: 0,
          style: {
            padding: '0px',
            color: '#ffffff'
          },
          positioner: function (boxWidth, boxHeight, point) {
            var y = point.plotY - 5;
            //dont let the plotY get down to 0 to prevent cut off
            if (point.plotY < 20) {
              y = 20;
            }

            return { x: point.plotX + 50, y: y };
          },
          formatter: function () {
            var tip;

            var thisMonthsInterest = calculatedInterestPoints[this.points[0].key];
            var thisMonthsBalance = Math.round(balancePoints[this.points[0].key] + this.points[0].y);

            tip = '<div style="background: #272525; border-radius: 3px; color: #ffffff;text-align: center; width: 150px; padding: 10px;position: relative; z-index: 8000;">' + monthsFull[(this.x + monthOffset) % 12] + ' ' + years[Math.floor((this.x + monthOffset) / 12)] + '<p>Payment applied: ' + $.commify(Math.round(appliedPayments[this.points[0].key]), { prefix: '$' }) + '</p><p>Monthly Interest: ' + $.commify(Math.round(thisMonthsInterest), { prefix: '$' }) + '</p><p>Balance: ' + $.commify(thisMonthsBalance, { prefix: '$' }) + '</p><div style="width: 17px; height: 17px; position: absolute; left: 81px; bottom: -12px;" class="wing"><img src="/assets/images/lessons/lesson2/step5/highcharts-tooltip-carrot.png" /></div></div>';
            return tip;
          }
        }, //END tooltip
        plotOptions: {
          area: {
            stacking: 'normal',
            marker: {
              enabled: true,
              crosshairs: true,
              useHTML: true,
              /* backgroundColor:'rgba(255, 255, 255, 0.1)', */
              backgroundColor: '#272525',
              shadow: false,
              shared: true,
              borderWidth: 0,
              style: {
                padding: '0px',
                color: '#ffffff'
              },
              positioner: function (boxWidth, boxHeight, point) {
                var y = point.plotY - 5;
                //dont let the plotY get down to 0 to prevent cut off
                if (point.plotY < 20) {
                  y = 20;
                }


                return { x: point.plotX + 50, y: y };
                //return { x: point.plotX + 50, y: point.plotY - 35 };
                //return { x: this.chart.plotLeft, y: this.chart.plotTop - 40 };
              },
              formatter: function () {
                var tip;

                var thisMonthsInterest = calculatedInterestPoints[this.points[0].key];
                var thisMonthsBalance = Math.round(balancePoints[this.points[0].key] + this.points[0].y);

                tip = '<div style="background: #272525; border-radius: 3px; color: #ffffff;text-align: center; width: 150px; padding: 10px;position: relative; z-index: 8000;">' + monthsFull[(this.x + monthOffset) % 12] + ' ' + years[Math.floor((this.x + monthOffset) / 12)] +'<p>Payment applied: ' + $.commify(appliedPayments[this.points[0].key], { prefix:'$' }) + '</p><p>Monthly Interest: ' + $.commify(Math.round(thisMonthsInterest), { prefix:'$' }) + '</p><p>Balance: ' + $.commify(thisMonthsBalance, { prefix:'$' }) + '</p><div style="width: 17px; height: 17px; position: absolute; left: 81px; bottom: -12px;" class="wing"><img src="/assets/images/lessons/lesson2/step5/highcharts-tooltip-carrot.png" /></div></div>';
                return tip;
              }
            }
          },
          series: {
            pointPadding: 0,
            groupPadding: 0

          }

        },
        series: [
          {
            //Series 1
            name: 'Interest',
            color: '#9bf3fb',
            shadow: false,
            lineWidth: 3,
            marker: {
              radius: 1
            }
          },
          {
            //Series 0
            name: 'Principal',
            color: '#99f3c6',
            shadow: false,
            lineWidth: 3,
            marker: {
              radius: 1
            }
          }
        ] //END series
      }); //END chart

      // Initialize a new renderer group for the markers and labels
      chartInfoDisplay = chart.renderer.g().add();
      chartInfoDisplay.toFront();

    }
  });

  //Header view
  Views.step5Header = Backbone.View.extend({
    initialize: function () {
      var hdr = new header.Model({ currentStep: 5, stepName: 'Real cost over time' });
      this.insertView(new header.Views.Header({ model: hdr }));
    }
  });

  Views.step5BarGraph = Backbone.View.extend({
    initialize: function (options) {
      this.insertView(new barGraph.Views.BarGraph({
        //pass in global events obj
        model: options.barGraph,
        viewsHolder: viewsHolder
      }));
    }
  });

  Views.step5GraphModifiers = Backbone.View.extend({
    initialize: function (options) {
      //this.contentView = options.viewsHolder.content;
      this.insertView(new graphModifiers.Views.GraphModifiers({
        model: options.graphModifiersModel,
        viewsHolder: viewsHolder
      }));

    },
    afterRender: function () {
    }
  });

  //footer view
  Views.step5Footer = Backbone.View.extend({
    initialize: function (options) {
      this.parentView = options.model;
      this.graphModifiersModel = options.graphModifiersModel;

      var ftr = new footer.Model({ amount: 0, description: '', subtitle: '', nextButton: "I'm done", showSum: false });
      this.insertView(new footer.Views.Footer({ model: ftr }));
    },

    events: {
      'click .right-button': 'handleNextButton',
      'click .left-button': 'handlePrevButton'
    },

    handleNextButton: function (e) {
      e.preventDefault();

      var step5valid = true;

      parentViewSaveComplete = false;
      graphModifiersSaveComplete = false;
      userSaveComplete = false;

      var checkAllSavesComplete = function () {
        if (parentViewSaveComplete && graphModifiersSaveComplete && userSaveComplete) {
          // See if user is logged in vs. anonymous
          if( Backbone.Asa.User.individualId == null || typeof Backbone.Asa.User.individualId === "undefined" ) {
            app.router.showLoginPrompt($(e.target).attr("href"));
          } else {
            window.location.href = $(e.target).attr("href");
          }
        }
      };

      this.parentView.save(null, {
        success: function (model, response) {
          parentViewSaveComplete = true;
          checkAllSavesComplete();
        },
        error: function () {
          parentViewSaveComplete = true;
          checkAllSavesComplete();
        }
      });
      this.graphModifiersModel.save(null, {
        success: function (model, response) {
          graphModifiersSaveComplete = true;
          checkAllSavesComplete();
        },
        error: function () {
          graphModifiersSaveComplete = true;
          checkAllSavesComplete();
        }
      });

      if (app.user.get('Lesson2Step') < 5) {
        app.user.set('Lesson2Step', 5);
      }

      app.user.save(null, {
        success: function (model, response) {
          userSaveComplete = true;
          checkAllSavesComplete();
        },
        error: function () {
          userSaveComplete = true;
          checkAllSavesComplete();
        }
      });

      app.wt.trigger('lesson:overall:complete', {
        user: Backbone.Asa.User.userGuid,
        time: new Date(),
        totalTime: ((Date.now() - Backbone.Asa.User.startTime) / 60000),
        step: {
          number:    app.router.activeStep+1,
        }
      });

        SALT.trigger('content:todo:completed');
        app.router.globalNavigation(null, 'next');
    },

    handlePrevButton: function (e) {
      e.preventDefault();

      app.router.globalNavigation(null, 'prev');
      return false;
    }
  });

  Views.Step5ValidationLogic = {
    error: function (model, error) {

    },
    success: function (model, error) {

    }
  };

  return Views;

});
