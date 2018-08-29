var goal = goal || {};
//ALL NUMBERS ON THIS PAGE ARE STANDARDIZED TO MONTHLY VALUES
//Pull data from result of Edit your Cashflow
var goalAmount = userData.goal.value;//How much do you want to save by initMonthsToGoal?

// Initial values
//var initCashFlow = cashflow + (totalExpenses - newTotalExpenses);//income - expenses
var initCashFlow = cashflow;
var initMonthsToGoal = parseInt(userData.goal.time);//How many months are planned to reach goal
var initSavingsMonthlyGoal = goalCalculated;//How much do you need to save per month to reach goal?
var initSavingsTarget = userData.goal.value;//Total amount to save for
//These variables are loaded with values for graph. Initially pulled from variables above, but
//are needed to account for modifiers being applied
var graphCashFlow = 0;
var graphMonthsToGoal = 0;
var graphSavingsMonthlyGoal = 0;
var cashFlowCondition=-1;
var validSavings; //Stores if there is savings greater than $1
var boost = 0;// if they add a one-time boost to income

//new monthly income
var newIncome = 0;
var newCashFlow = 0;
var newMonthsToGoal = 0;
var useExtra = false;
goalPageLoaded = true;
// Higchartarts-related
var chart;
// chart object
var chartInfoDisplay;
// chart chartInfoDisplay group
var months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
var date = new Date();
var monthOffset = date.getMonth() + 1;
// use current month as offset for x axis labels
// Arrays to hold higchartharts points
var pointSeries = new Array();
var extraPoints = new Array();

//extra savings for left content
var extraSavings = 0;

// Var to check if graph has been run
var init = false;


//goal left
var editedCashflow = 0;
var moreMoney = 0;
var modifiedGraphMonthsToGoal = 0;

save = goal;
var noPassError = "Yo! you need to complete the form yo!";
goal.global = {
    step: 1,
    utils: {
        init: function() {

            $("#content-container .content").show().css({
                opacity: 1
            });
            curriculum.global.viewport.animateViewport.inverse();

            // Append ID for this page
            $('#content-container .content').attr('id', 'curriculum-goal');

            // Append stylesheet with IE support
            if (document.createStyleSheet) {
              document.createStyleSheet('css/curriculum/goal.css');
            } else {
              $('head').append('<link rel="stylesheet" href="css/curriculum/goal.css" type="text/css" />');
            }

            // Update page context
            curriculum.global.utils.paginate.updateContext("5", "Reach Your Goal Faster", "", "", "I’m Done");

            // Styled inputs
            $('input[type=radio]').styledInputs();

            goal.global.utils.graphInit();
            goal.global.modifiers.init();

            $('#curriculum-goal .restore-graph').click(function(){
              goal.global.utils.clearChartData();
              graphCashFlow = initCashFlow;
              graphSavingsMonthlyGoal = initSavingsMonthlyGoal;
              graphMonthsToGoal = initMonthsToGoal;
              $('#goal-container').find("input:radio:checked").prop('checked',false);
              $('.radio').remove();
              $('input[type=radio]').removeClass('inputStyles').styledInputs();
              boost = 0;
              goal.global.utils.drawGraph();
            });

          //hide unecessary copy in footer
          $('#total').hide();
          $('#up-next').hide();
          $('#restore').hide();

					//include sidebar
					if (cashFlowCondition == -1) {
						curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column-negative.html');
					} else if (cashFlowCondition == 0) {
						curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column-negative.html');
					} else if (cashFlowCondition == 1) {
            var goalAmountMonthly = goalAmount / initMonthsToGoal;
            //check if break exactly even
            if(cashflow == goalAmountMonthly) {
					  	curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column-equal.html');
            }
            else {
					  	curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column.html');
            }
					}

        },
        // Add labels
        graphInit: function() {

            // Initialize chart
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'graph',
                    type: 'area',
                    marginRight: 65,
                    marginLeft: 144
                },
                credits: {
                    enabled: false
                },
                exporting: {
                    enabled: false
                },
                title: {
                    text: ''
                },
                legend: {
                    enabled: false
                },
                xAxis: {
                    title: {
                        text: null
                    },
                    tickInterval: 1,
                    labels: {
                        formatter: function() {
                            var index = (this.value + monthOffset) % 12;
                            return months[index];
                        }
                    },
                    gridLineDashStyle: 'Dot',
                    gridLineColor: '#bfbfbf',
                    gridLineWidth: 1
                },
                yAxis: {
                    title: {
                        text: null
                    },
                    labels: {
                        formatter: function() {
                            if (this.value < 0) {
                              //if number is negative change the placement of the minus sign to before the dollar sign
                              var num = this.value;
                              var str = num.toString();
                              var n = str.replace(/\-/gi, "\-\$");
                              return n;
                            }
                            else {
                              return '$' + this.value;
                            }
                        }
                    },
                    gridLineDashStyle: 'Dot',
                    gridLineColor: '#bfbfbf'
                },
                tooltip: {
                    enabled: false
                },
                plotOptions: {
                    area: {
                        stacking: 'normal',
                        marker: {
                            enabled: true,
                            symbol: 'circle',
                            radius: 2,
                            states: {
                                hover: {
                                    enabled: true
                                }
                            }
                        }
                    },
                    series: {
                        pointPadding: 0,
                        groupPadding: 0,
                        states: {
                            hover: {
                                enabled: false
                            }
                        }
                    }
                },
                series: [
                {
                    //Series 2
                    name: 'Extra Cashflow',
                    color: '#9bf3fb',
                    shadow: false,
                    lineWidth: 3
                },
                {
                    //Series 1
                    name: 'Negative Cashflow',
                    color: '#ff99d6',
                    shadow: false,
                    lineWidth: 3
                },
                {
                    //Series 0
                    name: 'Positive Cashflow',
                    color: '#99f3c6',
                    shadow: false,
                    lineWidth: 3
                }
                ]
            });

            // Set defaults for scenario if not provided
            graphCashFlow = initCashFlow;
            graphSavingsMonthlyGoal = initSavingsMonthlyGoal;
            //graphMonthsToGoal = initMonthsToGoal;
            graphMonthsToGoal = goal.global.utils.getGraphMonthsToGoal();

            goal.global.utils.drawGraph();
        },
        getGraphMonthsToGoal: function(){
          if(modifiedGraphMonthsToGoal == 0) {
            graphMonthsToGoal = initMonthsToGoal;
          } else {
            if(modifiedGraphMonthsToGoal < 0){
              graphMonthsToGoal = initMonthsToGoal;
            } else{
              graphMonthsToGoal = modifiedGraphMonthsToGoal;
            }
          }

          //console.log('graphMonthsToGoal: ' + graphMonthsToGoal);
          return graphMonthsToGoal;
        }, //END getGraphMonthsToGoal
        //END graphInit()
        drawLabels: function() {
            graphMonthsToGoal = goal.global.utils.getGraphMonthsToGoal();
            init();
            function init(){
              // Labels for initial and new goal time
              addChartInfoDisplay(graphMonthsToGoal);
              //if newMonthsToGoal is NOT zero then user is not going to meet their goal
              if (newMonthsToGoal !== 0) {
                  addChartInfoDisplay(newMonthsToGoal);
              }
            }; //END init

            // Create all the markers/labels for the chart
            function addChartInfoDisplay(seriesIndex) {

              var point;
              var extrainitCashFlowPoint; //For positive cashflow above goal

              if (cashFlowCondition == -1) {

                  point = chart.series[1].data[seriesIndex];
              } else if (cashFlowCondition == 0) {
                  point = chart.series[2].data[seriesIndex];
              } else if (cashFlowCondition == 1) {
                  point = chart.series[2].data[seriesIndex];
                  if (validSavings){
                    extraPoint = chart.series[0].data[seriesIndex];
                  }
              }

              //Vertical line marking x-axis for point
              drawLineMarker(point);

              //////////goal markers
              if (point.x == initMonthsToGoal) {
                  // Original goal
                  drawGoalLabel(point, false);
              } else {
                  // new GoalPlotPoint time
                  drawGoalLabel(point, true);
              }

              //////////value markers
              drawPointValueLabel(point);
              if (cashFlowCondition == 1 && validSavings) {

                drawPointValueLabel(extraPoint, true);
              }

            }; //END addChartInfoDisplay

            function drawLineMarker(point) {
                // Add the marker line
                //chart.renderer.rect: x, y, width, height, corner radius(?)

                chart.renderer.rect(
                point.plotX + chart.plotLeft,
                chart.plotTop,
                2, chart.plotHeight
                ).attr({
                    fill: 'black',
                    zIndex: 5
                }).add(chartInfoDisplay);
            }; //END drawLineMarker

            function drawGoalLabel(point, projected) {
                var imageSrc;
                var xPos;
                var yPos;
                var imageWidth;
                var imageHeight = 32;

                if (projected) {
                    imageSrc = '/Assets/images/lessons/lesson1/curriculum/actual.png';
                    imageWidth = 144;
                    yPos = chart.plotHeight + chart.plotTop - imageHeight;
                    // subtract height of .png
                    yPos -= 35;
                    // position above to avoid overlapping labels
                } else {
                    imageSrc = '/Assets/images/lessons/lesson1/curriculum/goal.png';
                    imageWidth = 127;
                    yPos = chart.plotHeight + chart.plotTop - imageHeight;
                    // subtract height of .png
                }

                xPos = point.plotX + chart.plotLeft - imageWidth,
                // subtract width of .png

                // Add the label
                // image: source, x, y, width, height
                chart.renderer.image(
                imageSrc,
                xPos,
                yPos,
                imageWidth,
                imageHeight
                ).attr({
                    zIndex: 5
                }).add(chartInfoDisplay);

            }; //END drawGoalLabel

            //addInitialGoalAmount is boolean used to tell valueMarker that it should add initSavingsTarget to value
            function drawPointValueLabel(point, addInitSavingsTarget) {
                var imageSrc;
                var xPos;
                var yPos;
                var imageWidth;
                var imageHeight = 32;
                var factor;
                //pixels per character
                var characterCount;
                var rectWidth;
                var pointValue = 0;

                imageSrc = '/Assets/images/lessons/lesson1/curriculum/value-marker.png';
                imageWidth = 16;
                imageHeight = 19;

                xPos = point.plotX + chart.plotLeft - (imageWidth / 2);
                yPos = point.plotY - imageHeight + 15;

                //Add marker image over graph point
                chart.renderer.image(
                imageSrc,
                xPos,
                yPos,
                imageWidth,
                imageHeight
                ).attr({
                    zIndex: 6
                }).add(chartInfoDisplay);

                //Get the string to display
                pointValue = getPointValueLabelString(point, addInitSavingsTarget);

                //draw text container
                //calculate offset for x-position based on character count
                factor = 8;
                characterCount = pointValue.length;
                rectWidth = 10 + (characterCount * factor);
                xOffset = (rectWidth / 2) - (imageWidth / 2);

                chart.renderer.rect(
                xPos - xOffset,
                yPos - imageHeight + 1,
                rectWidth,
                20
                ).attr({
                    fill: '#fff0de',
                    zIndex: 6
                }).add(chartInfoDisplay);

                //value markers text
                chart.renderer.text(
                pointValue,
                xPos - xOffset + 5,
                yPos - imageHeight + 15
                ).attr({
                    zIndex: 7
                }).add(chartInfoDisplay);
            }; //END drawPointValueLabel

            function getPointValueLabelString(point, addInitSavingsTarget) {
                var pointValue = 0;

                //calculate point value display in order to build label container
                if (point.x == graphMonthsToGoal) {
                    pointValue = point.series.processedYData[graphMonthsToGoal];
                } else {
                    pointValue = point.series.processedYData[newMonthsToGoal];
                }
                if (addInitSavingsTarget) {
                  pointValue += initSavingsTarget;
                }

                //Format value for display
                if (parseInt(pointValue) < 0) {
                    pointValue = $.commify(pointValue.toFixed(0), {prefix: '-$'});
                } else {
                    pointValue = $.commify(pointValue.toFixed(0), {prefix: '$'});
                }
                return pointValue;
            }; //END getPointValueLabelString



        },//END drawLabels()

        clearChartData: function() {

          //Wipe data series values
          chart.series[0].setData([], false);
          chart.series[1].setData([], false);
          chart.series[2].setData([], false);

          newIncome = 0;
          newCashFlow = 0;
          newMonthsToGoal = 0;
          useExtra = false;
          extraPoints = [];

          // Clear existing labels
          if (chartInfoDisplay != undefined) {
              // Destroy existing labels group (since I can't seem to destroy indiv elements)
              chartInfoDisplay.destroy();
          }
          // Initialize a new renderer group for the labels
          chartInfoDisplay = chart.renderer.g().add();
          chartInfoDisplay.toFront();

        },//END clearChartData

        displayHeaderCopy: function() {
          var headerCopy;
          extraSavings = graphCashFlow - initSavingsMonthlyGoal;

          if (boost) {
            headerCopy = "It looks like you'll reach your goal <strong>on time</strong>";
            $('#goalPageMessage span').html(headerCopy);
          } else if(cashFlowCondition == -1) {
            // negative cashflow
            //curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column-negative.html');
            headerCopy = "It looks like you need to make <strong>$" + $.commify(Math.ceil(initSavingsMonthlyGoal - graphCashFlow)) + " more</strong> per month";
            $('#goalPageMessage span').html(headerCopy);
          } else if (cashFlowCondition == 0) {
            // positive cashflow, but not reaching goal
            //curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column-negative.html');
            headerCopy = "It looks like it will take an <strong>extra " + $.commify(parseInt(newMonthsToGoal - initMonthsToGoal)) + " months</strong> to reach your goal";
            $('#goalPageMessage span').html(headerCopy);
          } else if (cashFlowCondition == 1) {
            // positive cashflow, exceeding goal
            // curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column.html');
           if (extraSavings < 1){
              headerCopy = "It looks like you'll reach your goal <strong>on time</strong>";
            } else if (graphMonthsToGoal < initMonthsToGoal){
              headerCopy = "It looks like you'll reach your goal <strong>" + parseInt(initMonthsToGoal - graphMonthsToGoal) + " months sooner</strong>";
            } else {
              headerCopy = "It looks like you will reach your goal with <strong>$" + $.commify(Math.ceil(graphCashFlow - initSavingsMonthlyGoal)) + "</strong> extra every month";
            }
            $('#goalPageMessage span').html(headerCopy);
          }

          //console.log('newTotalExpenses: ' + newTotalExpenses);
          //console.log('$ to goal: ' +  $.commify(Math.ceil(initSavingsMonthlyGoal - graphCashFlow)));



          $('#content-left').find('.expenses').text( Math.ceil(initSavingsMonthlyGoal - graphCashFlow)).formatCurrency({
            roundToDecimalPlace: 0,
            negativeFormat: '%s%n'
          });
        }, //END displayHeaderCopy()

        drawGraph: function() {
            goal.global.utils.clearChartData();
            graphMonthsToGoal = goal.global.utils.getGraphMonthsToGoal();

            //calculate graphCashFlow based on the modifiers
            if(editedCashflow !== 0){
              graphCashFlow = editedCashflow + moreMoney;
              //console.log('editedCashflow '+ editedCashflow +'moreMoney' + moreMoney );
            } else {
              graphCashFlow = initCashFlow + moreMoney;
              //console.log('init '+ initCashFlow +'moreMoney '+ moreMoney);
            }




            // Holder for points array
            var points;

            // Determine what the slope of our graph is
            // If cash flow is negative
            if (graphCashFlow <= 0) {
              //console.log('graphCashFlow: NEGATIVE');
              cashFlowCondition = -1;
              points = calcPoints(graphCashFlow, graphMonthsToGoal);
              // Update title
              // Set graph options
              if (init == false) {};
            } else if (boost){
              //console.log('boost is being used');
              cashFlowCondition = 0;
              points = calcPoints(graphCashFlow, initMonthsToGoal);
            } else if(graphCashFlow > 0 && graphCashFlow < graphSavingsMonthlyGoal) {
              //console.log('graphCashFlow: POSITIVE UNDER');
              // If cash flow is positive but below the target savings goal
              cashFlowCondition = 0;
              newMonthsToGoal = Math.ceil(goalAmount / graphCashFlow);
              //newMonthsToGoal = goal.global.utils.getGraphMonthsToGoal();

              points = calcPoints(graphCashFlow, newMonthsToGoal);

              // Update title

            } else if (graphCashFlow >= graphSavingsMonthlyGoal) {
              //console.log('graphCashFlow: POSITIVE OVER');
              // If cash flow is positive and equal or above the target savings goal
              cashFlowCondition = 1;
              validSavings = (graphCashFlow - graphSavingsMonthlyGoal > 1);
              // If we should put our extra money towards the goal
              if (useExtra == true) {
                  points = calcPoints(graphCashFlow, newMonthsToGoal);
              } else {
                // Save our extra money
                points = calcPoints(graphSavingsMonthlyGoal, graphMonthsToGoal);
                if (validSavings){
                  // Get points for extra income over time
                  extraPoints = calcPoints(graphCashFlow - graphSavingsMonthlyGoal, graphMonthsToGoal);
                }
                // Update title
              }
              // Set graph options
              if (init == false) {};
            };


            // Set the data points & redraw chart, show appropriate modifier

            if (cashFlowCondition == -1) {
              // negative cashflow drawGraph
							chart.series[1].setData(points, false);
            } else if (cashFlowCondition == 0) {
              // positive cashflow, but not reaching goal
              chart.series[2].setData(points, false);
            } else if (cashFlowCondition == 1) {
              // positive cashflow, exceeding goal
              chart.series[2].setData(points, false);
              if (validSavings){
                chart.series[0].setData(extraPoints, false);
              }
            }
            chart.redraw();
            goal.global.utils.displayHeaderCopy();

            // Add labels (needs to happen after chart is redrawn to use the right points)
            goal.global.utils.drawLabels();

            if (init == false) {
                init = true;
            };

            // Generate graph points
            // I'll need to add conditionals if we need to ignore certain things
            function calcPoints(slope, numMonths) {
              var _points = new Array();
              for (var i = 0; i <= numMonths; i++) {
                  _points[i] = (slope * i);
              };
              // If we're adding a boost
              if (boost) {
                  for (var i = 0; i <= numMonths; i++) {
                    _points[i] += boost;
                  };
              };
              // If useExtra is true, we need to add dummy points
              if (useExtra == true) {
                  for (var i = _points.length; i <= initMonthsToGoal; i++) {
                      _points[i] = null;
                  };
              };


              return _points;
            };


        }, // END drawGraph()

        errors: function() {
            pagePass = true;
            curriculum.global.utils.paginate.next();
        }, //END errors

        saveData: function() {
          userData.newTotalIncome = totalIncome + (graphCashFlow - initCashFlow);
          window.location = "../summary.html";
        } // END saveData()
    },// END: utils

    modifiers: {
        init: function() {
          goal.global.modifiers.createNegativeModifierHandlers();
          goal.global.modifiers.createPositiveModifierHandlers();
          goal.global.modifiers.createExtraCashModifierHandlers();
          goal.global.modifiers.displayModifier();
        }, //END init()

        displayModifier: function(){

          //Hide any shown modifiers
          $('.graphModifier').hide();

          if (cashFlowCondition == -1) {
              goal.global.modifiers.displayNegativeModifier();
          } else if (cashFlowCondition == 0) {

              goal.global.modifiers.displayPositiveModifier();
          } else if (cashFlowCondition == 1) {

              goal.global.modifiers.displayExtraCashModifier();
          }
        },// END displayModifier()

        displayNegativeModifier: function() {

          var modifierMonthsToGoal;
          var modifierSavingsTarget;
          var cashFlowOffset;
          //Modifier 1 - calculate needed cashflow to reach goal on time
          modifierSavingsTarget = Math.ceil(initSavingsTarget / initMonthsToGoal);
          cashFlowOffset = Math.ceil(initSavingsMonthlyGoal - initCashFlow);
          $('#negative-modifier1-amount').text('$' + $.commify(cashFlowOffset));
          $('#negative-modifier1-amount').attr('data-value', modifierSavingsTarget);

          //Modifier 2 - 1.5x original goal month, calculate needed cashflow
          modifierMonthsToGoal = Math.ceil(initMonthsToGoal * 1.5);
          modifierSavingsTarget = Math.ceil(initSavingsTarget / modifierMonthsToGoal);
          cashFlowOffset = modifierSavingsTarget - initCashFlow;

          $('#negative-modifier2-amount').text('$' + cashFlowOffset);
          $('#negative-modifier2-amount').attr('data-value', modifierSavingsTarget);
          $('#negative-modifier2-months').text(modifierMonthsToGoal);
          $('#negative-modifier2-months').attr('data-value', modifierMonthsToGoal);

          $('#negative-container').show();
        },  //END displayNegativeModifier

        displayPositiveModifier: function() {

          var totalSavingsAtGoalTime = 0;
          var modifierSavingsTarget = 0;
          var cashFlowOffset = 0;

          //Modifier 1 - calculate initial boost needed to reach goal on time

          totalSavingsAtGoalTime = initMonthsToGoal * initCashFlow;
          //boosthere
          //boost = Math.ceil(initSavingsTarget - totalSavingsAtGoalTime);
          $('#reached-goal-modifier1-amount').text('$' + boost);
          $('#reached-goal-modifier1-amount').attr('data-value', boost);

          //Modifier 2 - calculate needed cashflow to reach goal on time
          modifierSavingsTarget = Math.ceil(initSavingsTarget / initMonthsToGoal);
          cashFlowOffset = Math.ceil(initSavingsMonthlyGoal - initCashFlow);
          $('#reached-goal-modifier2-amount').text('$' + cashFlowOffset);
          $('#reached-goal-modifier2-amount').attr('data-value', modifierSavingsTarget);

          //Modifier 3 - user submitted cashflow
          //No data needed

          $('#reached-goal-container').show();
        },
        displayExtraCashModifier: function() {
          //Modifier - Apply extra cash towards goal


          $('#extra-cash-container').show();

        },
        createNegativeModifierHandlers: function() {

          $('#negative-modifier1').change(function() {
            if ($('#negative-modifier1').is(':checked')) {
              graphCashFlow = parseInt($('#negative-modifier1-amount').attr('data-value'));
              graphMonthsToGoal = initMonthsToGoal;
              graphSavingsMonthlyGoal = graphCashFlow;
              newMonthsToGoal = 0;
              goal.global.utils.drawGraph();
            }
          });
          $('#negative-modifier2').change(function() {
            if ($('#negative-modifier2').is(':checked')) {
              graphCashFlow = parseInt($('#negative-modifier2-amount').attr('data-value'));
              graphMonthsToGoal = parseInt($('#negative-modifier2-months').attr('data-value'));
              graphSavingsMonthlyGoal = graphCashFlow;
              newMonthsToGoal = 0;
              goal.global.utils.drawGraph();
            }
          });
        },
        createPositiveModifierHandlers: function() {
          $('#reached-goal-modifier1').change(function() {
            if ($('#reached-goal-modifier1').is(':checked')) {

              graphCashFlow = initCashFlow;
              graphMonthsToGoal = initMonthsToGoal;
              graphSavingsMonthlyGoal = initSavingsMonthlyGoal;
              boost = parseInt($('#reached-goal-modifier1-amount').attr('data-value'));
              goal.global.utils.drawGraph();
            }
          });
          $('#reached-goal-modifier2').change(function() {
            if ($('#reached-goal-modifier2').is(':checked')) {

              graphCashFlow = parseInt($('#reached-goal-modifier2-amount').attr('data-value'));
              graphMonthsToGoal = initMonthsToGoal;
              graphSavingsMonthlyGoal = graphCashFlow;
              boost = 0;

              goal.global.utils.drawGraph();
            }
          });
          $('#reached-goal-modifier3').change(function() {
            if ($('#reached-goal-modifier3').is(':checked')) {

              graphCashFlow = parseInt($('#negative-modifier2-amount').attr('data-value'));
              graphMonthsToGoal = parseInt($('#negative-modifier2-months').attr('data-value'));
              graphSavingsMonthlyGoal = graphCashFlow;
              boost = 0;
              goal.global.utils.drawGraph();
            }
          });

        },
        createExtraCashModifierHandlers: function() {

          $('#extra-cash-modifier1').change(function() {

            if ($('#extra-cash-modifier1').is(':checked')) {

              graphMonthsToGoal = Math.ceil(initSavingsTarget / graphCashFlow);
              graphSavingsMonthlyGoal = graphCashFlow;
              goal.global.utils.drawGraph();
            }
          });
        }

    }//END modifiers
};// END expenses global var
$('document').ready(function(){
  
    // activate utils  
    goal.global.utils.init();
});
