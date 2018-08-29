var graph = graph || {};
//ALL NUMBERS ON THIS PAGE ARE STANDARDIZED TO MONTHLY VALUES

/////////////////////////////////////////////////////////////////////
/// These variables are required for the graph get drawn properly ///
/////////////////////////////////////////////////////////////////////

var goalAmount              = 1500; // How much do you want to save by initMonthsToGoal?
var initCashFlow            = 100; // Carried over from lesson
var initMonthsToGoal        = 7; // How many months are planned to reach goal
var initSavingsMonthlyGoal  = Math.ceil(goalAmount / initMonthsToGoal); // How much you need to save per month to reach goal on time

/////////////////////////////////////////////////////////////////////


var validSavings; //Stores if there is savings greater than $1 (boolean)


var newMonthsToGoal = 0; //how many months to go out past the initSavingsMonthlyGoal if cashflow does not allow user to meet initSavingsMonthlyGoal
var useExtra = false;

////////////////////////////////////////////////////////////
/// These variables are copies of the initial values     ///
/// and will be the ones that are changed with modifiers ///
////////////////////////////////////////////////////////////

var graphMonthsToGoal = initMonthsToGoal;
var graphCashFlow = initCashFlow;


////////////////////////////////////////////////////////////


// Higchartarts-related
var chart;
// chart object
var chartInfoDisplay;

var months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
//var monthsFull = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]; //not used for Lesson 1
var years = [];
var date = new Date();
var year = date.getFullYear();
var monthOffset = date.getMonth() + 1;
var newPoints; //


// use current month as offset for x axis labels
// Arrays to hold higchartharts points

var extraPoints = new Array();





//goal left
//var editedCashflow = 0;
//var moreMoney = 0;

graph.global = {
    utils: {
        init: function(){
          var resizeTimer = 1;
          graph.global.utils.graphInit();
          $(window).resize(function(){
            if (resizeTimer){
              clearTimeout(resizeTimer);
              resizeTimer = setTimeout(graph.global.utils.drawGraph, 200);
            }
          });  
            
            

            /*
$('#curriculum-goal .restore-graph').click(function(){
              graph.global.utils.clearChartData();
              graphCashFlow = initCashFlow;
              graphSavingsMonthlyGoal = initSavingsMonthlyGoal;
              graphMonthsToGoal = initMonthsToGoal;
              $('#goal-container').find("input:radio:checked").prop('checked',false);
              $('.radio').remove();
              $('input[type=radio]').removeClass('inputStyles').styledInputs();
              boost = 0;
              graph.global.utils.drawGraph();
            });
*/

        }, // END utils.init();
        modifyGraph: function(modifiedCashFlow, moreMoney, wait){
          /////////////////////////////////////////////////////////////////////////////
          // This function takes in modified values and redraws the graph            //
          // All paremeters must be passed to it,                                    //
          // Pass a zero value if you dont want to change that value                 //
          /////////////////////////////////////////////////////////////////////////////
          //console.log('modifyGraph()');
          
          
          
          
          if(modifiedCashFlow > 0){
            graphCashFlow = modifiedCashFlow;
          } else{
            graphCashFlow = initCashFlow;
          }
          
          if(moreMoney){
            graphCashFlow = graphCashFlow + moreMoney;
          } 
          
          
          if(wait){
            newMonthsToGoal = Math.ceil(goalAmount / graphCashFlow);
            graphMonthsToGoal = newMonthsToGoal;
          } else{
            graphMonthsToGoal = initMonthsToGoal;
          }
          
          //Draw the graph
          graph.global.utils.drawGraph();
        }, // END modifyGraph()
        graphInit: function() {
        

            // Initialize chart look and feel
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'graph',
                    type: 'area',
                    marginRight: 65,
                    marginLeft: 144
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
                          
                          if(newPoints > 30){
                            inc = years[this.value / 12];
                          } else{
                            var index = (this.value + monthOffset) % 12;
                             inc = months[index];
                          }
                            
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
                              var n = str.replace(/\-/gi, "\-\$");
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
                    enabled: false,
                    crosshairs: true,
                    useHTML: true,
                    backgroundColor: '#333',
                    borderWidth: 0,
                    style: {
                      color: '#ffffff'
                    },
                    positioner: function (boxWidth, boxHeight, point) {
                      return { x: point.plotX + 50, y: 140 };
                      //return { x: this.chart.plotLeft, y: this.chart.plotTop - 40 };
                    },
                    formatter: function(){
                      var tip;

                      if(this.points[1]){
                        tip = '<div style="color: #ffffff;text-align: center; width: 150px; padding: 10px;position: relative;">' + monthsFull[(this.x + monthOffset) % 12] + '<p>Extra cash:  $' + this.points[0].y.toFixed(0) + '</p><p>Cashflow:  $' + this.points[1].y.toFixed(0) + '</p><p>Outside Var:  $' + cashFlowCondition + '</p><div style="width: 17px; height: 17px; background: #333; position: absolute; left: 75px; bottom: -17px;" class="wing"></div></div>';  
                      } else {
                        tip = '<div style="color: #ffffff;text-align: center; width: 150px; padding: 10px;position: relative;">' + monthsFull[(this.x + monthOffset) % 12] + '<p>Cashflow:  $' + this.points[0].y.toFixed(0) + '</p><p>Outside Var:  $' + cashFlowCondition + '</p><div style="width: 17px; height: 17px; background: #333; position: absolute; left: 75px; bottom: -17px;" class="wing"></div></div>';  
                      }
                      
                      
                      return tip;
                    },
                    /*
headerFormat: '<div style="color: #ffffff;text-align: center; padding-top: 10px;">Title</div><table style="color: #ffffff; font-size: 12px; width: 200px; padding: 10px">',
                    pointFormat: '<tr><td>{series.name}: </td>' + '<td style="text-align: right"><b>{point.y}</b></td></tr>',
                    footerFormat: '</table> <div style="color: #ffffff;text-align: center; padding-top: 10px;">Balance: </div>',
*/
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
            
            //draw the graph
            //after specifying all graph settings
            graph.global.utils.drawGraph();
        }, // END graphInit()
        //END graphInit()
        drawLabels: function() {
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
              if (point.x == graphMonthsToGoal) {
                  //console.log('draw OG goal');
                  // Draw label at Original goal
                  drawGoalLabel(point, false);
              } else {
                  //console.log('draw new goal');
                  // Draw label at new GoalPlotPoint time
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
                yPos = point.plotY - imageHeight + 40; //was + 15

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
                  pointValue += goalAmount;
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

          newMonthsToGoal = 0;
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
        /*
          var headerCopy;
          extraSavings = graphCashFlow - initSavingsMonthlyGoal;

          if (boost) {
            headerCopy = "It looks like you'll reach your goal <strong>on time</strong>";
            $('#goalPageMessage span').html(headerCopy);
          } else if(cashFlowCondition == -1) {
            // negative cashflow
            //curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column-negative.html');
            console.log('cashflowCondition: ' + cashFlowCondition);
            headerCopy = "It looks like you need to make <strong>$" + $.commify(Math.ceil(initSavingsMonthlyGoal - graphCashFlow)) + " more</strong> per month";
            $('#goalPageMessage span').html(headerCopy);
          } else if (cashFlowCondition == 0) {
            // positive cashflow, but not reaching goal
            //curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column-negative.html');
            console.log('cashflowCondition: ' + cashFlowCondition);
            headerCopy = "It looks like it will take an <strong>extra " + $.commify(parseInt(newMonthsToGoal - initMonthsToGoal)) + " months</strong> to reach your goal";
            $('#goalPageMessage span').html(headerCopy);
          } else if (cashFlowCondition == 1) {
            // positive cashflow, exceeding goal
            // curriculum.global.utils.paginate.navigateLeftColumn('includes/goal-left-column.html');
            console.log('cashflowCondition: ' + cashFlowCondition);
            
            
              console.log('extraSavings: ' + extraSavings);
           if (extraSavings < 1) {
              headerCopy = "It looks like you'll reach your goal <strong>on time</strong>";
            } else if (graphMonthsToGoal < initMonthsToGoal){
              headerCopy = "It looks like you'll reach your goal <strong>" + parseInt(initMonthsToGoal - graphMonthsToGoal) + " months sooner</strong>";
            } else {
              headerCopy = "It looks like you will reach your goal with <strong>$" + $.commify(Math.ceil(graphCashFlow - initSavingsMonthlyGoal)) + "</strong> extra every month";
            }
            
          }

          //console.log('newTotalExpenses: ' + newTotalExpenses);
          //console.log('$ to goal: ' +  $.commify(Math.ceil(initSavingsMonthlyGoal - graphCashFlow)));


          */
          //$('#content-left').find('.expenses').text( Math.ceil(initSavingsMonthlyGoal - graphCashFlow));
        }, //END displayHeaderCopy()

        drawGraph: function() {
            graph.global.utils.clearChartData();

            // Holder for points array
            var points;

            // Determine what the slope of our graph is
            // If cash flow is negative
            if (graphCashFlow <= 0) {
              console.log('graphCashFlow: NEGATIVE');
              cashFlowCondition = -1;
              points = calcPoints(graphCashFlow, graphMonthsToGoal);
            } else if(graphCashFlow > 0 && graphCashFlow < initSavingsMonthlyGoal) {
              console.log('graphCashFlow: POSITIVE UNDER');
              cashFlowCondition = 0;
              newMonthsToGoal = Math.ceil(goalAmount / graphCashFlow);
              points = calcPoints(graphCashFlow, newMonthsToGoal);
            } else if (graphCashFlow >= initSavingsMonthlyGoal) {
              console.log('graphCashFlow: POSITIVE OVER');
              cashFlowCondition = 1;
              //determine if you have any extra money
              //true or false
              validSavings = (graphCashFlow - initSavingsMonthlyGoal > 1);
              
              // If we should put our extra money towards the goal
              if (useExtra == true) {
                  points = calcPoints(graphCashFlow, newMonthsToGoal);
              } else {
                // Save our extra money
                points = calcPoints(initSavingsMonthlyGoal, graphMonthsToGoal);
                if (validSavings){
                  console.log('valid savings');
                  // Get points for extra income over time
                  extraPoints = calcPoints(graphCashFlow - initSavingsMonthlyGoal, graphMonthsToGoal);
                }
              };
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

            // Add labels (needs to happen after chart is redrawn to use the right points)
            graph.global.utils.drawLabels();


            // Generate graph points
            function calcPoints(slope, numMonths) {
              var _points = new Array();
              for (var i = 0; i <= numMonths; i++) {
                  _points[i] = (slope * i);
              };
              newPoints = _points.length;
              for(var i = 0; i < newPoints; i ++){
                years.push(year++);
              }
              
              
              // If useExtra is true, we need to add dummy points
              if (useExtra == true) {
                  for (var i = _points.length; i <= graphMonthsToGoal; i++) {
                      //_points[i] = null;
                  };
              };


              return _points;
            };


        }, // END drawGraph()

        errors: function() {
            //hardCoded
        }, //END errors

        saveData: function() {
          //hardCoded
        } // END saveData()
    },// END: utils

    modifiers: {
    /*
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
      */
    }//END modifiers
};// END expenses global var
$('document').ready(function(){
    // activate utils
    graph.global.utils.init();
});
