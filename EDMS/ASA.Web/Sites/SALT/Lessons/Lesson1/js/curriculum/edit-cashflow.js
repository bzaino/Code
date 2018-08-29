var editCashflow = editCashflow || {};
var currentCashflowElement;
var maxWidth = 600;
var maxCost = 0;
var segmentPaddingPixelCount = 4;
var factor;
var timed;
var initialWidth = 0;
var parent;
var element;
var minSegmentWidth = 60;
var newTotalExpenses = 0;
var barDynamicResizeCalculations = {};
//vars used for loop
var pixelWidthDiff = 0;
var pulledStaticPixelDiff = 0;
var pulledDynamicPixelWidth = 0;

//pass back to global
save = editCashflow;

//TODO Document overall methodology used to calculate width of each segment for a category (Dynamic vs static widths)
editCashflow.global = {
    step: 16,
    utils: {

        init: function () {
            editCashflow.global.utils.populateNewValueProperty();

            // preloaded flag is irrelevant here; set tracking flag to false.
            curriculum.global.tracking.preloaded = false;
        },

        //For each expense, set newValue and editValue property from value
        populateNewValueProperty: function () {
            $.each(userData.expenses, function (categoryK, categoryV) {
                if (categoryV.Complex === true) {
                    $.each(userData.expenses[categoryK], function (expenseK, expenseV) {
                        //Filter out properties so we're only looking at child objects that aren't User, aka expenses
                        if (typeof expenseV === 'object' && expenseV !== null && expenseK !== "User") {
                            if (expenseV.Complex === true || expenseK === 'cost') {
                                //Utilities, Housing, School, Transportation nested sub expense (Car)
                                $.each(expenseV, function (k2, v2) {
                                    //Filter out properties so we're only looking at child objects that aren't User, aka expenses.
                                    if (typeof v2 === 'object' && v2 !== null && k2 != "User") {
                                        //console.log('updating: ' + 'userData.expenses.' + categoryK + '.' + expenseK + [k2]);
                                        userData.expenses[categoryK][expenseK][k2].newValue = userData.expenses[categoryK][expenseK][k2].value;
                                        userData.expenses[categoryK][expenseK][k2].editValue = userData.expenses[categoryK][expenseK][k2].value;
                                    }
                                });
                            } else { //Not complex
                                //Health, Optionals, Transportation simple sub expenses(bike, public transit)
                                userData.expenses[categoryK][expenseK].newValue = userData.expenses[categoryK][expenseK].value;
                                userData.expenses[categoryK][expenseK].editValue = userData.expenses[categoryK][expenseK].value;
                            }
                        }
                    });
                } else { //Simple segment (Eating, Entertainment, Groceries, Savings, Technology)
                    userData.expenses[categoryK].newValue = userData.expenses[categoryK].value;
                    userData.expenses[categoryK].editValue = userData.expenses[categoryK].value;
                }

            });
        },

        //For each expense, set editValue property from newValue if user clicked finish cutting before
        populateEditValueProperty: function () {
            $.each(userData.expenses, function (categoryK, categoryV) {
                if (categoryV.Complex === true) {
                    $.each(userData.expenses[categoryK], function (expenseK, expenseV) {
                        //Filter out properties so we're only looking at child objects that aren't User, aka expenses
                        if (typeof expenseV === 'object' && expenseV !== null && expenseK != "User") {
                            if (expenseV.Complex === true || expenseK === 'cost') {
                                //Utilities, Housing, School, Transportation nested sub expense (Car)
                                $.each(expenseV, function (k2, v2) {
                                    //Filter out properties so we're only looking at child objects that aren't User, aka expenses.
                                    if (typeof v2 === 'object' && v2 !== null && k2 != "User") {
                                        //console.log('updating: ' + 'userData.expenses.' + categoryK + '.' + expenseK + [k2]);
                                        userData.expenses[categoryK][expenseK][k2].editValue = userData.expenses[categoryK][expenseK][k2].value;
                                    }
                                });
                            } else { //Not complex
                                //Health, Optionals, Transportation simple sub expenses(bike, public transit)
                                userData.expenses[categoryK][expenseK].editValue = userData.expenses[categoryK][expenseK].value;
                            }
                        }
                    });
                } else { //Simple segment (Eating, Entertainment, Groceries, Savings, Technology)
                    userData.expenses[categoryK].editValue = userData.expenses[categoryK].value;
                }

            });
        },

        startGraph: function () {
            var graphContainer = $('#graph-container'),
          overlay = $('#expenses-overlay #curriculum-cashflow');

            //clear the graph out
            overlay.empty();

            //Calculate pixel widths for all expenses
            $.each(userData.expenses, function (categoryK, categoryV) {
                barDynamicResizeCalculations[categoryK] = {};
                barDynamicResizeCalculations[categoryK].dynamicPixelWidth = 0;
                barDynamicResizeCalculations[categoryK].staticPixelDiff = 0;
                //barDynamicResizeCalculations[categoryK].staticBlockCount = 0;
                factor = userData.nonCreditExpenseList[categoryK] / maxWidth;

                if (categoryV.Complex === true) {
                    $.each(userData.expenses[categoryK], function (expenseK, expenseV) {
                        //Filter out properties so we're only looking at child objects that aren't User, aka expenses
                        if (typeof expenseV === 'object' && expenseV !== null && expenseK != "User") {
                            if (expenseV.CreditExpense) {
                              return;
                            }
                            if (expenseV.Complex === true || expenseK === 'cost') {
                                //Utilities, Housing, School, Transportation nested sub expense (Car)
                                barDynamicResizeCalculations[categoryK].dynamicPixelWidth = 0;
                                $.each(expenseV, function (k2, v2) {
                                    //Filter out properties so we're only looking at child objects that aren't User, aka expenses.
                                    if (typeof v2 === 'object' && v2 !== null && k2 != "User") {
                                        if (v2.CreditExpense) {
                                          return;
                                        }
                                      
                                        timed = curriculum.global.utils.determineRate.init(v2.newValue, v2.time);
                                        initialWidth = timed / factor;
                                        userData.expenses[categoryK][expenseK][k2].pixelWidth = (Math.floor(initialWidth) - segmentPaddingPixelCount); //This  last subtraction accounts for right margin and borders for each segment
                                        //if segment is smaller than minSegmentWidth set it to minSegmentWidth
                                        if (userData.expenses[categoryK][expenseK][k2].pixelWidth <= minSegmentWidth) {
                                            pixelWidthDiff = minSegmentWidth - userData.expenses[categoryK][expenseK][k2].pixelWidth;
                                            barDynamicResizeCalculations[categoryK].staticPixelDiff += pixelWidthDiff;
                                            //barDynamicResizeCalculations[categoryK].staticBlockCount += 1;
                                            userData.expenses[categoryK][expenseK][k2].pixelWidth = minSegmentWidth;
                                        } else {
                                            //otherwise add the segment size to the dynamic Segment Pixel count
                                            barDynamicResizeCalculations[categoryK].dynamicPixelWidth += userData.expenses[categoryK][expenseK][k2].pixelWidth;
                                        }
                                    }
                                });
                            } else {
                                //this is like a bike or transit
                                timed = curriculum.global.utils.determineRate.init(expenseV.newValue, expenseV.time);
                                initialWidth = timed / factor;

                                userData.expenses[categoryK][expenseK].pixelWidth = Math.floor(initialWidth) - segmentPaddingPixelCount; //This  last subtraction accounts for right margin and borders for each segment

                                //if segment is smaller than minSegmentWidth set it to minSegmentWidth
                                if (userData.expenses[categoryK][expenseK].pixelWidth <= minSegmentWidth) {
                                    pixelWidthDiff = minSegmentWidth - userData.expenses[categoryK][expenseK].pixelWidth;
                                    barDynamicResizeCalculations[categoryK].staticPixelDiff += pixelWidthDiff;
                                    //barDynamicResizeCalculations[categoryK].staticBlockCount += 1;
                                    userData.expenses[categoryK][expenseK].pixelWidth = minSegmentWidth;
                                } else {
                                    //otherwise add the segment size to the dynamic Segment Pixel count
                                    barDynamicResizeCalculations[categoryK].dynamicPixelWidth += userData.expenses[categoryK][expenseK].pixelWidth;
                                }
                            }
                        }

                    });

                }

            });

            //used to find factor
            $.each(barDynamicResizeCalculations, function (categoryK, categoryV) {
                var newDynamicPixelWidth = barDynamicResizeCalculations[categoryK].dynamicPixelWidth - barDynamicResizeCalculations[categoryK].staticPixelDiff;
                //console.log('newDynamicPixelWidth=' + newDynamicPixelWidth + '; barDynamicResizeCalculations[categoryK].dynamicPixelWidth=' + barDynamicResizeCalculations[categoryK].dynamicPixelWidth);
                barDynamicResizeCalculations[categoryK].factor = newDynamicPixelWidth / barDynamicResizeCalculations[categoryK].dynamicPixelWidth;
            });

            $.each(userData.expenses, function (categoryK, categoryV) {

                var hideCategory = false;
                if (categoryK == 'transportation' || categoryK == 'housing') {
                  hideCategory = true;
                  $.each(userData.expenses[categoryK], function(expenseK, expenseV) {
                    //Filter out properties so we're only looking at child objects that aren't User, aka expenses
                    if (typeof expenseV === 'object' && expenseV !== null && expenseK != "User") {
                      if (expenseK == 'car' || expenseK == 'cost') {
                        $.each(expenseV, function (k2, v2) {
                          if (typeof v2 === 'object' && v2 !== null && k2 != "User") {
                            if (!v2.CreditExpense) {
                              hideCategory = false;
                            }
                          }
                        });
                      } else if (!expenseV.CreditExpense) {
                        hideCategory = false;
                      }
                    }
                  });
                }

                if (categoryV.CreditExpense || (categoryV.Name === "credit" && categoryV.Value === 0) || hideCategory) {
                  return;
                }
                
                //insert .bar element
                //populate li
                $('#expenses-overlay').find('.title ul').append('<li class="' + categoryK + '"><a class="name" href="' + categoryK + '">' + categoryK + '</a></li>');

                //category total
                $(overlay).append('<div class="bar clearfix" rel="' + categoryK + '"><div class="total-info"><div class="value"></div> <div class="name" data-name="' + categoryK + '">&nbsp;spent on ' + categoryK + '</div></div><div class="quick-tip"><h5>Quick Tips</h5><p>By taking public transit, walking or riding your bike just 2 days a week, you can save up to $80 on gas every month.</p></div></div>');

                if (categoryV.Complex === true) {
                    $.each(userData.expenses[categoryK], function (expenseK, expenseV) {
                        //Filter out properties so we're only looking at child objects that aren't User, aka expenses
                        if (typeof expenseV === 'object' && expenseV !== null && expenseK != "User") {
                            if (expenseV.CreditExpense) {
                              return;
                            }
                            if (expenseV.Complex === true || expenseK === 'cost') {
                                //Utilities, Housing, School, Transportation nested sub expense (Car)
                                $.each(expenseV, function (k2, v2) {
                                    //Filter out properties so we're only looking at child objects that aren't User, aka expenses.
                                    if (typeof v2 === 'object' && v2 !== null && k2 != "User") {
                                        if (v2.CreditExpense) {
                                          return;
                                        }
                                        if (v2.pixelWidth > minSegmentWidth) {
                                            //if formerly dynamic
                                            v2.pixelWidth *= barDynamicResizeCalculations[categoryK].factor;
                                        }

                                        timed = Math.floor(curriculum.global.utils.determineRate.init(v2.newValue, v2.time));
                                        parent = $(overlay).find('.bar[rel=' + categoryK + ']');
                                        //format expenseK
                                        splitExpenseK = expenseK.split('_');
                                        if (splitExpenseK[0] == 'cost') {
                                            splitExpenseK[0] = '';
                                        } else {
                                            splitExpenseK[0] = splitExpenseK[0] + '-';
                                        }

                                        segment = $('<div class="segment" data-name="' + expenseK + '/' + k2 + '" ><div class="tip clearfix"><div class="name" data-name="' + splitExpenseK[0] + k2 + '">' + splitExpenseK[0] + k2 + ' / </div><div class="label"></div><div class="x">x</div><div class="wing"></div></div></div>');

                                        var tip = splitExpenseK[0] + k2;
                                        var tipLength = tip.length;

                                        $(parent).prepend(segment);

                                        editCashflow.global.utils.createNewBar(parent, segment, v2.pixelWidth, timed, tipLength);
                                    }
                                });
                            } else { //Not complex
                                //Health, Optionals, Transportation simple sub expenses(bike, public transit)
                                if (expenseV.pixelWidth > minSegmentWidth) {
                                    //if formerly dynamic
                                    expenseV.pixelWidth *= barDynamicResizeCalculations[categoryK].factor;
                                }

                                timed = Math.floor(curriculum.global.utils.determineRate.init(expenseV.newValue, expenseV.time));
                                parent = $(overlay).find('.bar[rel=' + categoryK + ']');

                                splitExpenseK = expenseK.split('_');

                                segment = $('<div class="segment" data-name="' + expenseK + '" ><div class="tip clearfix"><div class="name" data-name="' + splitExpenseK[0] + '">' + splitExpenseK[0] + ' / </div><div class="label"></div><div class="x">x</div><div class="wing"></div></div></div>');

                                var tip = splitExpenseK[0] + splitExpenseK[0];
                                var tipLength = tip.length;

                                $(parent).prepend(segment);

                                editCashflow.global.utils.createNewBar(parent, segment, expenseV.pixelWidth, timed, tipLength);
                            }
                        }
                    });
                } else { //Simple segment
                    timed = Math.floor(curriculum.global.utils.determineRate.init(categoryV.newValue, categoryV.time));
                    parent = $(overlay).find('.bar[rel=' + categoryK + ']');

                    segment = $('<div class="segment" data-name="' + categoryK + '" ><div class="tip clearfix"><div class="name" data-name="' + categoryK + '">' + categoryK + ' / </div><div class="label"></div><div class="x">x</div><div class="wing"></div></div></div>');

                    var tip = categoryK;
                    var tipLength = categoryK.length;

                    $(parent).prepend(segment);

                    editCashflow.global.utils.createNewBar(parent, segment, maxWidth, timed, tipLength);
                }
            });

            //show/hide expense categories
            $('#expenses-overlay').find('ul li:first a').addClass('current');
            $('#curriculum-cashflow').find('.bar').css('display', 'none');
            $('#curriculum-cashflow').find('.bar:first').css('display', 'block');

            //click expense links and show appropriate expenses
            $('#expenses-overlay').find('ul li a').click(function (e) {
                e.preventDefault();

                //get link href attribute to match bar segment with
                var expenseName = $(this).attr('href');

                //remove current from all links and add it to only the clicked link
                $('#expenses-overlay').find('ul li a').removeClass('current');
                $(this).addClass('current');

                //show correlated bar and hide other bars
                $('#curriculum-cashflow ').find('.bar').css('display', 'none');
                $('.bar[rel=' + expenseName + ']').css('display', 'block');
            });


        }, //END startGraph

        wipeExpenseNewValues: function () {
            //Wipe any existing newValue info from userData
            $.each(userData.expenses, function (categoryK, categoryV) {
                if (categoryV.Complex === true) {
                    $.each(userData.expenses[categoryK], function (expenseK, expenseV) {
                        //Filter out properties so we're only looking at child objects that aren't User, aka expenses
                        if (typeof expenseV === 'object' && expenseV !== null && expenseK != "User") {
                            if (expenseV.Complex === true || expenseK === 'cost') {
                                //Utilities, Housing, School, Transportation nested sub expense (Car)
                                $.each(expenseV, function (k2, v2) {
                                    //Filter out properties so we're only looking at child objects that aren't User, aka expenses.
                                    if (typeof v2 === 'object' && v2 !== null && k2 != "User") {
                                        userData.expenses[categoryK][expenseK][k2].newValue = null;
                                    }
                                });
                            } else { //Not complex
                                //Health, Optionals, Transportation simple sub expenses(bike, public transit)
                                userData.expenses[categoryK][expenseK].newValue = null;
                            }
                        }
                    });
                } else { //Simple segment (Eating, Entertainment, Groceries, Savings, Technology)
                    userData.expenses[categoryK].newValue = null;
                }
            });
        }, //END wipeExpenseNewValues

        createNewBar: function (parent, element, initialWidth, timed, tipLength) {
            var tipInnerContentWidth = 0;
            var timedLength = timed.toString().length + 6;
            tipLength = (tipLength * 3) + (timedLength * 2) + 100;

            //round initialWidth
            initialWidth = Math.round(initialWidth);
            if (timed == 0) {
                element.remove();
            }
            $(element).find('.tip .label').html(timed).formatCurrency({
                roundToDecimalPlace: 0
            });

            $(element).width(initialWidth);
            editCashflow.global.utils.gatherCategoryTotal(parent);

            $(element).find('.tip').each(function () {

                //timed = number
                newWidth = (initialWidth / 2) + 100;
                $(element).addClass('set');

                $(element).find('.tip').css({ width: tipLength });

            });

            $(element).resizable({
                handles: 'e',
                minHeight: 65,
                maxHeight: 65,
                minWidth: 26,
                maxWidth: initialWidth,

                // on resize
                resize: function (event, ui) {
                    var dataName = element.attr('data-name');
                    newWidth = ui.size.width;
                    newValue = Math.round((newWidth / initialWidth) * timed);
                    element.find('.tip .label').html('$' + newValue).formatCurrency({
                        roundToDecimalPlace: 0
                    });

                    //handle the size and remove
                    if (ui.size.width <= 30) {
                        element.find('.tip').addClass('delete');
                        element.find('.tip .name').html('Remove?');
                        element.find('.tip .label').hide();
                    } else {
                        element.find('.tip').removeClass('delete');
                        element.find('.tip .label').show();
                        element.find('.tip .name').html(element.find('.tip .name').attr('data-name') + ' / ');
                    }

                    editCashflow.global.utils.gatherCategoryTotal(parent);
                    editCashflow.global.utils.saveUpdatedValue(parent, dataName, newValue);
                },
                //start
                start: function (event, ui) {
                    resizing = true;
                    element.find('.ui-resizable-handle').addClass('down');
                    element.find('.tip').addClass('dragDown');
                },

                //stop
                stop: function (event, ui) {
                    resizing = true;
                    element.find('.ui-resizable-handle').removeClass('down');
                    element.find('.tip').removeClass('dragDown');

                    curriculum.global.tracking.trigger("lesson:graph:optionChange", {
                        graph: {
                            option: 'expenses-overlay-bar-resize-' + $(parent).attr('rel')
                        },
                        step: {
                            number: save.global.step
                        }
                    });
                }
            });

            //handle delete button
            $(element).find('.tip .x').click(function () {

                $(this).parents('.segment').remove();

                editCashflow.global.utils.gatherCategoryTotal(parent);

                //delete from userData.expenses
                thisCategory = $(parent).attr('rel');
                thisSegment = $(element).attr('data-name');
                thisSegmentArr = thisSegment.split('/');

                var expenseToZero;

                //Set segments newValue to zero
                if (userData.expenses[thisCategory].value == 0 || userData.expenses[thisCategory].value == undefined) {
                    if (thisSegmentArr.length == 2) {
                        //Nested Expense
                        expenseToZero = userData.expenses[thisCategory][thisSegmentArr[0]][thisSegmentArr[1]];
                        expenseToZero.newValue = 0;
                    } else if (thisSegmentArr.length == 1) {
                        //Complex expense
                        expenseToZero = userData.expenses[thisCategory][thisSegment];
                        expenseToZero.newValue = 0;
                    };
                } else {
                    //if this is a simple expense
                    expenseToZero = userData.expenses[thisCategory];
                    expenseToZero.newValue = 0;
                };
            });

        }, //END createNewBar

        gatherCategoryTotal: function (parent) {
            categoryTotal = 0;

            if ($('.segment', parent).length != 0) {
                $('.segment', parent).each(function (k, v) {
                    categoryTotal += curriculum.global.utils.cleanInput.init($(v).find('.tip .label').text());

                });

                $(parent).find('.total-info .value').text(categoryTotal).formatCurrency({
                    roundToDecimalPlace: 0
                });

            } else {
                $(parent).find('.total-info .value').text('0');
            }
            editCashflow.global.utils.updateCashflow();

        }, //END gatherCategoryTotal()

        updateCashflow: function () {
            //get all new total costs
            newTotalExpenses = 0;
            $('.bar').each(function (k, v) {
                cleaned = curriculum.global.utils.cleanInput.init($(v).find('.total-info .value').text());
                newTotalExpenses += cleaned;
            });

            editCashflow.global.utils.getNewCashflow();


        }, // END updateCashflow()

        getNewCashflow: function () {
            totalNonCreditExpenses = curriculum.global.utils.totalNonCreditExpenses.value();
          
            var savings = (totalIncome - newTotalExpenses) - initSavingsMonthlyGoal;

            var expensesCutGoal = (initSavingsMonthlyGoal - graphCashFlow) - (totalNonCreditExpenses - newTotalExpenses); //how much you need to cut to reach goal on time
            var newExpenseCashflow = (initCashFlow) + (totalNonCreditExpenses - newTotalExpenses); //

            //graph.global.utils.displayHeaderCopy();






            if (savings < 0) {
                $('.overlay-content .title .positive').hide();
                $('.overlay-content .title .negative').show();
            } else {
                $('.overlay-content .title .negative').hide();
                $('.overlay-content .title .positive').show();
            }

            $('.overlay-content .title .value').text(savings).formatCurrency({
                roundToDecimalPlace: 0,
                negativeFormat: '%s%n'
            });

            return newExpenseCashflow;
        }, //END getNewCashflow()

        saveData: function () {
            
            var curriculumUtils = curriculum.global.utils;

            for ( var i in userData.expenses ) {
                if( userData.expenses[i].Complex) {
                    if( userData.expenses[i].cost ) { //Utilities, Housing, School
                        for (var k in userData.expenses[i].cost ) {
                            if( userData.expenses[i].cost[k].newValue === 0 || userData.expenses[i].cost[k].value === null ) {
                                delete userData.expenses[i].cost[k];
                            } else if( userData.expenses[i].cost[k].value !== userData.expenses[i].cost[k].newValue ) {
                                userData.expenses[i].cost[k].value = userData.expenses[i].cost[k].newValue;
                            }
                        }

                        if( $.isEmptyObject(userData.expenses[i].cost) ) {
                            delete userData.expenses[i];
                        }
                    } else { //Health, Other, Transportation
                        var numExpenses = 0;

                        for (var k in userData.expenses[i] ) {
                            if( curriculumUtils.server.isExpense( userData.expenses[i][k] ) ) {
                                numExpenses++;

                                if( userData.expenses[i][k].Complex ) { //Transportation.car
                                    var numSubExpense = 0;
                                    
                                    for (var l in userData.expenses[i][k] ) {
                                        if( curriculumUtils.server.isExpense( userData.expenses[i][k][l] ) ) {
                                            numSubExpense++;
                                            
                                            if( userData.expenses[i][k][l].newValue === 0 || userData.expenses[i][k][l].value === null ) {
                                                delete userData.expenses[i][k][l];
                                                numSubExpense--;
                                            } else if( userData.expenses[i][k][l].value !== userData.expenses[i][k][l].newValue ) {
                                                userData.expenses[i][k][l].value = userData.expenses[i][k][l].newValue;
                                            }
                                        }
                                    }
                                    
                                    if( numSubExpense === 0 ) {
                                        delete userData.expenses[i][k];
                                        numExpenses--;
                                    }
                                } else { //Transportation(bike,transit),Health, Other
                                    if( userData.expenses[i][k].newValue === 0 || userData.expenses[i][k].value === null ) {
                                        delete userData.expenses[i][k];
                                        numExpenses--;
                                    } else if( userData.expenses[i][k].value !== userData.expenses[i][k].newValue ) {
                                        userData.expenses[i][k].value = userData.expenses[i][k].newValue;
                                    }
                                }
                                
                            } 
                        }

                        if( numExpenses === 0 ) {
                            delete userData.expenses[i];
                        }
                    }
                } else { //Simple segment (Eating, Entertainment, Groceries, Savings, Technology)
                    if( userData.expenses[i].newValue === 0 || userData.expenses[i].value === null ) {
                        delete userData.expenses[i];
                    } else if( userData.expenses[i].value !== userData.expenses[i].newValue ) {
                        userData.expenses[i].value = userData.expenses[i].newValue;
                    }
                }
            }
        
           curriculum.global.utils.server.saveToServer(userData);

        },

        //This method is used to update data persisted in newValue field when modifying expenses
        saveUpdatedValue: function (parent, dataName, newValue) {
            var parentName = parent.attr('rel');

            //Determine where we need to save this data
            if (parentName == dataName) {
                //This is a simple expense
                userData.expenses[parentName].newValue = newValue;
            } else if (dataName.indexOf('/') < 0) {
                //This is a complex expense
                userData.expenses[parentName][dataName].newValue = newValue;
            } else {
                //This is a nested complex expense
                //Split string for expense and sub-expense names;
                var dataNameArray = dataName.split('/');
                userData.expenses[parentName][dataNameArray[0]][dataNameArray[1]].newValue = newValue;
            }
        },
        errors: function () {
            pagePass = true;
            curriculum.global.utils.paginate.next();
        }
    } // END: utils

};               // END cashflow global var


$('document').ready(function () {
  // activate utils
  editCashflow.global.utils.init();

});
