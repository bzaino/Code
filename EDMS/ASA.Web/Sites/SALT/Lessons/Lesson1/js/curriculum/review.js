var review = review || {};

var noPassError;
//var totalExpenses;
var totalIncome;
var totalCredit = 0;
var ccBoxesArray = [];
var ccBalance;

returnToReview = false;

//pass back to global
save = review;

review.global = {
    step: 15,
    utils: {
        init: function () {
            LazyLoad.css('css/curriculum/review.css', function () {
                curriculum.global.viewport.animateViewport.wide();
            });

            // Append ID and class for this page
            $('#content-container .content').attr('id', 'curriculum-review').removeClass('simple');

            // Update page context
            $('#total .value').text("$0");
            curriculum.global.utils.paginate.updateContext("4", "Review", "Monthly Cashflow", "Edit cashflow", "Keep Going");

            //trigger webtrends
            curriculum.global.tracking.trigger("lesson:step:start", {
                step: {
                  number: save.global.step
                }
            });

            review.global.utils.startData.expenses();
            review.global.utils.startData.income();

            //reset currentPagePosition
            //currentPagePosition = $.inArray('review.html',pages);

            $('#edit-income').click(function (e) {
                e.preventDefault();
                //currentPagePosition = 1;
                curriculum.global.utils.paginate.currentPagePosition = 1;
                returnToReview = true;
                curriculum.global.utils.paginate.navigateSpecific('income.html');
            });


        }, // END init()

        gatherBoxesCC: function () {
          userData.creditExpenses = [];
          $('div.expenses input[type="checkbox"]').each(function () {
            var currentID = $(this).attr('id');

            var expense;

            var parentName = $(this).parent().parent().data('name');
            var parentExpense = userData.expenses[parentName];
            if (parentName == 'transportation') {
              var parent = $(this).parent().parent().data('parent');
              parentExpense = parentExpense[parent];
            }

            if ($(this).hasClass('sub')) {
              var name = $(this).parent().data('name');

              var carIndex = name.indexOf("car.");
              var costIndex = name.indexOf("cost.");

              if (carIndex != -1) {
                name = name.substring(carIndex + 4);
                expense = parentExpense[name];
              } else if (costIndex != -1) {
                name = name.substring(costIndex + 5);
                expense = parentExpense.cost[name];
              } else {
                expense = parentExpense[name];
              }

              if ($(this).is(':checked')) {
                expense.CreditExpense = true;
                userData.creditExpenses.push(currentID);
              } else {
                expense.CreditExpense = false;
              }
            } else {
              expense = parentExpense;

              if ($(this).is(':checked')) {
                var hasChildren = $(this).parent().parent().hasClass('hasChildren');

                if (hasChildren) {
                  var uncheckedChildren = $(this).parent().parent().find('input[type="checkbox"]:not(:checked).sub');
                  if (uncheckedChildren.length == 0) {
                    expense.CreditExpense = true;
                    userData.creditExpenses.push(currentID);
                  }
                } else {
                  expense.CreditExpense = true;
                  userData.creditExpenses.push(currentID);
                }
              } else {
                expense.CreditExpense = false;
              }
            }
          });
        }, // END gatherBoxesCC

        cleanUpEditArea: function () {
            $('div.expenses .table').addClass('default');
            $('div.expenses .dim').removeClass('dim');
            $('dl.editing').find('.remove').removeClass('remove');
            $('dl.editing').find('.bottom').remove();
            $('dl.editing').removeClass('editing');
        }, // END cleanUpEditArea()

        showEditCredit: function () {
            review.global.utils.cleanUpEditArea();
            this.calcuateSubLevelCreditExpenses();
            this.calculateTopLevelCreditExpenses();

            $('div.expenses dl dd.listing').removeClass('hidden');

            $('#cc-tip-init').closest('.table').addClass('cc-edit');
            $('#cc-tip-init').closest('.tip').fadeOut('fast');

            $('div.expenses input[type="checkbox"]').removeAttr('disabled');
            $('div.expenses .value input[type="text"]').attr('disabled', 'disabled');
            $('div.expenses dl dd.listing').fadeIn('fast');

            $('.interim, .cc-details').show();

            if (userData.expenses.credit != undefined) {
                $('.cc-expenses-total').val(userData.expenses.credit.value).formatCurrency({ roundToDecimalPlace: 0 });
                //HACK Dropkick isn't properly displaying the correct dropdown selection to the user, though the .val() result is correct
                //http://stackoverflow.com/questions/9857771/issues-setting-value-label-using-dropkick-javascript
                $('#cc-dropkick-time').val(userData.expenses.credit.time);
                $(".cc-details .dk_label").text(userData.expenses.credit.time);
                $(".cc-details .dk_options_inner li").each(function(){
                  $(this).removeAttr('class');
                  if ($(this).text() == userData.expenses.credit.time){
                     $(this).attr('class', 'dk_option_current');
                  }
                });

            }


            //hide credit if it exists
            $('dl.listing[data-name="credit"]').hide();
            //hide add new item
            $('.expenses .edit-back').hide();
        },

        getTotalCreditExpenses: function() {
            var totalVal = 0;
            $("#curriculum-review .expenses input:checked").each(function() {
                var $inputParent = $(this).parent();

                if( $inputParent.prop("tagName") !== "SPAN" && !($inputParent.prop("tagName") === "DT" && $inputParent.parent().hasClass("hasChildren")) ) {
                    totalVal += parseFloat($(this).parent().find("input").attr("data-value").replace("$", "").replace(",", ""));
                }
            });
            return totalVal;
        },

        //This method returns the number of expenses marked as credit. This includes top-level expenses with no children, and any children expenses.
        //Parents with credit children are not counted
        getTotalCreditExpenseCount: function() {
          var expenseCount = 0;
          var inactiveExpenseArray = $("#curriculum-review").find(".inactive");
          var expenseElement;

          for(var i = 0; i < inactiveExpenseArray.length; i++) {
            expenseElement = $(inactiveExpenseArray[i]);

             //Check if this is a parent expense with children, dont count
            if (expenseElement.prop("tagName") === "DT" && expenseElement.parent("dl").hasClass("hasChildren")) {
              // continue;
            } else if( expenseElement.prop("tagName") === "DT" || expenseElement.prop("tagName") === "DD" ) {
                expenseCount++;
            }
          }

          return expenseCount;
        },

        dataEvents: function () {
            var self = this;

            ////////////child checkboxes
            $('div.expenses input[type="checkbox"].sub').die('change propertychange');
            $('div.expenses input[type="checkbox"].sub').live('change propertychange', function (e) {
                $(this).parents('dl').find('.cbox').removeClass('partial-select');
                //loop to see if all of this element's siblings are checked

                siblingCount = $(this).parents('dl').find('dd').length;

                var i = 0;
                $(this).parents('dl').find('dd').each(function (key, value) {
                    if ($(this).find('input').is(':checked')) {
                        i++;
                    }
                });

                if (i <= 0) { //if none of the children are selected
                    //clear out the parent box
                    $(this).parents('dl').find('.cbox:first').removeClass('partial-select');
                    $('div.expenses dl dt input[type="checkbox"]').die('change');
                    $(this).parents('dl').find('dt input').prop("checked", false).trigger('change');
                } else if (i == siblingCount) { // if all of the children are selected
                    //check off the parent box
                    $('div.expenses dl dt input[type="checkbox"]').die('change');
                    $(this).parents('dl').find('dt input').prop("checked", true).trigger('change');
                    $(this).parents('dl').find('.cbox:first').removeClass('partial-select');
                } else { //if only some of the children are selected
                    //make the parent partial but unselected
                    $('div.expenses dl dt input[type="checkbox"]').die('change');
                    $(this).parents('dl').find('dt input').prop("checked", false).trigger('change');
                    $(this).parents('dl').find('.cbox:first').addClass('partial-select');
                }

                totalCredit = self.getTotalCreditExpenses();

                review.global.utils.dataEvents();

                //update the total sum in DOM
                $('strong.cc-total-value').text(totalCredit).formatCurrency({ roundToDecimalPlace: 0 });

            });

            //parent checkboxes
            $('div.expenses dl dt input[type="checkbox"]').die('change propertychange');
            $('div.expenses dl dt input[type="checkbox"]').live('change propertychange', function (e) {
                $(this).siblings('.cbox').removeClass('partial-select');

                if (!$(this).is(':checked')) {
                    $(this).parents('dl').find('dd input').each(function () {
                        $('div.expenses input[type="checkbox"].sub').die('change');
                        $(this).prop('checked', false).trigger('change');
                    });
                } else {
                    $(this).parents('dl').find('dd input').each(function () {
                        $('div.expenses input[type="checkbox"].sub').die('change');
                        $(this).prop('checked', true).trigger('change');
                    });
                }

                review.global.utils.dataEvents();

                totalCredit = self.getTotalCreditExpenses();

                //update the total sum in DOM
                $('strong.cc-total-value').text(totalCredit).formatCurrency({ roundToDecimalPlace: 0 });
            });

            /**
            Inline editing events:
            */
            //////activate inline editing
            $('#curriculum-review div.expenses dl dt a.edit').click(function (e) {
                e.preventDefault();
                //only allow editing to happen once per line at a time

                review.global.utils.cleanUpEditArea();
                //clear out the quesiton hover
                $('.question-hover').fadeOut();

                $('div.expenses .table').removeClass('default');

                if (!$(this).parents('dl').hasClass('editing')) {
                    var parent = $(this).attr('data-parent');
                    //declare this editable
                    $(this).parents('dl').addClass('editing');
                    //insert editing controls
                    //exclude the credit card expense
                    if ($(this).parents('dl').attr('data-name') != 'credit') {
                        var controls = $(this).parents('dl').append('<dd class="bottom"><a href="#" class="btn-back">Go back to ' + parent + '</a> <span class="right"><a href="#" class="cancel">Cancel</a> <a href="#" class="save">Save</a></span></dd>');
                    } else {
                        var controls = $(this).parents('dl').append('<dd class="bottom"><span class="right"><a href="#" class="cancel">Cancel</a> <a href="#" class="save">Save</a></span></dd>');
                    }


                    //dropdown re-population
                    //when in editing mode the dropdowns need to read the original time entry not monthly
                    var parent = $(this).parents('dl');

                    if ($(parent).hasClass('hasChildren')) {
                        //take away any hidden classes on the children
                        $('div.expenses .editing').find('dd.hidden').removeClass('hidden');

                        //set time for all child dd elements
                        $(parent).find('dd.listing').each(function (key, value) {

                            var actualTime = $(this).find('select').attr('data-time');
                            $(this).find('select').val(actualTime).trigger('change');


                            //remove the existing dropdowns and replace them after changing the value
                            $(this).find('select').siblings('.dk_container').remove();
                            $(this).find('select').removeData('dropkick');
                            $(this).find('select').dropkick({
                                change: function () {
                                    review.global.utils.calculateSpecificMonthlyTotal();
                                }
                            });
                        });
                    } else {
                        //set time for dt
                        //elemetns without children also need their monthly values re-calculated based on the original time entry.
                        //remove the existing dropdowns and replace them after changing the value
                        $(parent).find('dt select').val($(parent).find('dt select').attr('data-time')).trigger('change');
                        $(parent).find('dt select').siblings('.dk_container').remove();
                        $(parent).find('dt select').removeData('dropkick');
                        $(parent).find('dt select').dropkick({
                            change: function () {
                                review.global.utils.calculateSpecificMonthlyTotal();
                            }
                        });

                        //make the amount field re-editable
                        $(parent).find('dt .amount').removeAttr('disabled');
                        var dataValue = $(parent).find('dt .amount').attr('data-value');
                        if (dataValue) {
                            $(parent).find('dt .amount').val(dataValue).formatCurrency({ roundToDecimalPlace: 0 });
                        }
                    }

                    //////


                    //////

                    //go back button
                    $(parent).find('.bottom .btn-back').click(function (e) {
                        e.preventDefault();
                        var dataName = $(this).parents('dl').attr('data-name').split('.');
                        var path = 'expenses/' + dataName[0] + '.html';
                        var navigate;

                        for (i = 0; i < pages.length; i++) {
                            if (pages[i].path === path) {
                                curriculum.global.utils.paginate.currentPagePosition = i;
                                navigate = pages[i].path;
                                break;
                            }
                        }

                        $("#content-container .content").fadeOut(pageTransitionDuration, function () {
                            curriculum.global.utils.paginate.navigateSpecific(navigate);
                        });
                    });


                    //cancel buttons
                    $(controls).find('.cancel').click(function (e) {
                        e.preventDefault();
                        var parent = $(this).parents('dl');
                        var controls = $(this).parents('.bottom');

                        review.global.utils.cleanUpEditArea();

                        review.global.utils.startData.expenses(false);
                    });


                    //save buttons
                    //saves the inline editing changes
                    $(controls).find('.save').click(function (e) {
                        e.preventDefault();
                        var parent = $(this).parents('dl');
                        var parentName = $(parent).attr('data-name');
                        var dataName;
                        var expenseListEntryTotal = 0; //Stores monthly value of expense entry being saved
                        var controls = $(this).parents('.bottom');

                        //find out IF this parent hasChildren
                        if ($(parent).hasClass('hasChildren')) {
                            //determine IF there are any remaining children
                            if ($(parent).find('dd.listing').not('.remove').not("inactive").length) {
                                $(parent).find('dd.listing').not(".inactive").each(function () {
                                    dataName = $(this).attr('data-name').split('.');

                                    if ($(this).hasClass('remove')) {
                                        //remove EACH element with a .remove class
                                        if (dataName[1]) {
                                            delete userData.expenses[parentName][dataName[0]][dataName[1]];
                                            //console.log('deleting: ' + dataName[1]);
                                        } else {
                                            delete userData.expenses[parentName][dataName[0]];
                                            //console.log('deleting: ' + dataName[0]);
                                        }
                                    } else {
                                        //save the value for each non removed child
                                        var clean = curriculum.global.utils.cleanInput.init($(this).find('.amount').val());
                                        var time = $(this).find('select').val();

                                        if (dataName[1]) {
                                            userData.expenses[parentName][dataName[0]][dataName[1]].value = clean;
                                            userData.expenses[parentName][dataName[0]][dataName[1]].time = time;
                                        } else {
                                            userData.expenses[parentName][dataName[0]].value = clean;
                                            userData.expenses[parentName][dataName[0]].time = time;
                                        }
                                        //Add value to running total
                                        expenseListEntryTotal = expenseListEntryTotal + curriculum.global.utils.determineRate.init(clean, time);
                                    }
                                });
                            } else {
                                //All children expenses are marked for deletion, so we need to delete its parent as well
                                removeExpenseEntry(parent, parentName, $(parent).attr('data-parent'));
                            }
                        } else { // parent does not have children
                            //determine whether to remove or save this single value
                            dataName = $(this).parents('dl').attr('data-name');
                            var dataParent = $(this).parents('dl').attr('data-parent');
                            if (dataName !== dataParent) {
                                dataName = dataName + '.' + dataParent;
                            }
                            dataName = dataName.split('.');

                            if ($(parent).find('dt').hasClass('remove')) {
                                //Remove expense entry from userData expense and expenseList objects
                                removeExpenseEntry(parent, dataName[0], dataName[1]);
                            } else if( ! $(parent).find('dt').hasClass('inactive') ) {
                                var clean = curriculum.global.utils.cleanInput.init($(parent).find('dt .amount').val());
                                var time = $(parent).find('dt select').val();
                                if (dataName[1]) {
                                    userData.expenses[dataName[0]][dataName[1]].value = clean;
                                    userData.expenses[dataName[0]][dataName[1]].time = time;

                                } else {
                                    userData.expenses[dataName[0]].value = clean;
                                    userData.expenses[dataName[0]].time = time;
                                }
                                expenseListEntryTotal = expenseListEntryTotal + curriculum.global.utils.determineRate.init(clean, time);
                            }
                        }

                        //save the expenseList value for this expense entry.
                        //For transportation child expenses we need to ensure that total value of siblings is also added in
                        //Its possible that transportation expense has been completely removed, so need to test for that as well
                        if (parentName === 'transportation' && typeof userData.expenseList.transportation !== 'undefined') {
                            var transportationTotal = expenseListEntryTotal;
                            var transportationSubExpenseName = $(this).parents('dl').attr('data-parent');
                            $.each(userData.expenses.transportation, function (expenseK, expenseV) {
                                //Look at child expenses, but not the expense being edited
                                if (typeof expenseV === 'object' && expenseV !== null && expenseK !== "User" && expenseK !== transportationSubExpenseName) {
                                    if (expenseV.Complex === true) { // Transportation nested sub expense (Car)
                                        $.each(expenseV, function (k2, v2) {
                                            //Filter out properties so we're only looking at expenses
                                            if (typeof v2 === 'object' && v2 !== null && k2 != "User") {
                                                transportationTotal = transportationTotal + curriculum.global.utils.determineRate.init(v2.value, v2.time);
                                            }
                                        });
                                    } else { //simple sub expenses(bike, public transit)
                                        transportationTotal = transportationTotal + curriculum.global.utils.determineRate.init(expenseV.value, expenseV.time);
                                    }
                                }
                            });
                            userData.expenseList[parentName] = transportationTotal;
                        } else {
                            //For any non-transportation expense entries
                            userData.expenseList[parentName] = expenseListEntryTotal;
                        }

                        review.global.utils.cleanUpEditArea();
                        review.global.utils.startData.expenses(false);
                    });


                    //input field manipulation
                    if ($(this).parents('dl').hasClass('hasChildren')) {
                        $('div.expenses dl.editing dd .amount').bind('change keyup blur', function (e) {
                            $(this).formatCurrency({ roundToDecimalPlace: 0 });

                            review.global.utils.calculateSpecificMonthlyTotal();
                        });

                        //run the calculations above when the corresponding select element is changed
                        /*
                        $('div.expenses dl.editing dd select').change(function(){
                        $(this).parent().find('.amount').trigger('keyup');
                        review.global.utils.calculateSpecificMonthlyTotal();
                        });
                        */

                    } else {
                        $('div.expenses dl.editing dt .amount').bind('change keyup blur', function (e) {
                            $(this).formatCurrency({ roundToDecimalPlace: 0 });
                        });
                    }



                    //make all others dimmed
                    $('div.expenses dl.listing').not('.editing').addClass('dim');

                }

            });

            //This function handles removing an expense entry, including those with chidlren
            //PARAMETERS
            //expenseEntryDLElement: DL DOM element that belongs to the expense type
            //expenseType: name of expense type to remove from user's expense object
            //subExpense: For transporation, name of sub expense; otherwise ignored
            function removeExpenseEntry(expenseEntryDLElement, expenseType, subExpense) {
                //Hide from DOM
                expenseEntryDLElement.find('dt').addClass('remove');

                //For transportation, need to check if there are any addition child expenses. If not, remove it as well
                if (expenseType === 'transportation') {
                    delete userData.expenses[expenseType][subExpense];

                    //Look through transportation objec for any other child expenses
                    var childExpenseFound = false;
                    $.each(userData.expenses.transportation, function (expenseK, expenseV) {
                        if (typeof expenseV === 'object' && expenseV !== null && expenseK !== "User") {
                            childExpenseFound = true;
                            return false;
                        }
                    });
                    if (!childExpenseFound) {
                        //If not child expenses found, remove transportation
                        delete userData.expenses.transportation;
                        delete userData.expenseList.transportation;
                    }
                } else {
                    //Not transportation, remove expense
                    delete userData.expenses[expenseType];
                    delete userData.expenseList[expenseType];
                }
            };

            //remove buttons
            $('div.expenses dl.listing label').click(function (e) {
                //this marks child expenses for removal
                if ($(this).parents('dl').hasClass('editing')) {
                    if ($(this).parents('dl').hasClass('hasChildren')) {
                        $(this).parents('dd').addClass('remove');
                    } else {
                        //in this condition, it marks the 'simple' cost for removal
                        $(this).parents('dt').addClass('remove');
                    }
                }

                review.global.utils.calculateSpecificMonthlyTotal();
            });


            //credit question hover event
            $('a.question').mouseenter(function () {
                if ($('.table.default')) {
                    $('.question-hover').stop(true, true).fadeIn();
                }
            }).click(function(e) {
                e.preventDefault();
            });

            $('.question-hover').mouseleave(function () {
                $(this).stop(true, true).fadeOut();
            });

        }, //END dataEvents

        calcuateSubLevelCreditExpenses: function() {
            $(".sub").each(function() {
                var montlyValue = Math.round($(this).data("value")),
                    $parent = $(this).parent();

                $parent.find(".amount").val( montlyValue ).formatCurrency({ roundToDecimalPlace: 0 });
            });
        },

        calculateTopLevelCreditExpenses: function() {
            $(".expenses dl.listing").each(function() {
                var $this = $(this),
                    topLevelTotal = 0,
                    $topLevel = $this.find("dt .amount"),
                    $lineItems = $this.find("dd .amount");

                $lineItems.each(function() {
                    topLevelTotal += parseFloat($(this).val().replace(",", "").replace("$",""));
                });

                if( topLevelTotal > 0 ) {
                    $topLevel.val( topLevelTotal ).formatCurrency({ roundToDecimalPlace: 0 });
                }
            });
        },

        calculateSpecificMonthlyTotal: function() {
            var thisExpenseTotal = 0;

            //re calculate monthly total for this expense based on child values
            //the correct group is chosen via the .editing class
            //be sure to ignore the items marked for removal
            $('dl.editing dd.listing').not('.remove').not(".inactive").each(function (key, value) {
                var clean = curriculum.global.utils.cleanInput.init($(this).find('.amount').val());
                var time = $(this).find('select').val();
                //as we add up the 'thisExpenseTotal' value we need to remember to
                //convert these values to a monthly number as the total is alwasy represented as a per month value
                timed = curriculum.global.utils.determineRate.init(clean, time);
                thisExpenseTotal += timed;
            });

            //Update value in DOM
            $('dl.editing').find('dt .amount').val(thisExpenseTotal).formatCurrency({ roundToDecimalPlace: 0 });
        },

        startData: {
            expenses: function (omitCC) {
                $('.expenses .edit-back').show();

                curriculum.global.utils.animateGraph.refresh();

                //only show the credit card edit box on initial load of this slide
                //DISABLED
                if (omitCC != false && !( userData.expenses.credit && userData.expenses.credit.Value > 0 ) ) {
                    $('#cc-tip-init').show();
                }

                //////////
                var parents = '';
                var children = '';
                var j = 0;
                //loop through parents
                $.each(userData.expenses, function (categoryK, categoryV) {
                    //this is main / basic expenses loop
                    if (curriculum.global.utils.server.isExpense(categoryV) ) {
                        if (categoryK == 'transportation') {
                            $.each(categoryV, function (key, value) {
                                if (curriculum.global.utils.server.isExpense(value)) {
                                    if (!value.Complex) {
                                        //this field is a simple transportation type
                                        timed = curriculum.global.utils.determineRate.init(value.value, value.time);
                                        parents += '<dl class="listing clearfix" data-parent="' + key + '" data-name="transportation"><dt class="listing clearfix"><input type="checkbox" disabled="false" data-value="' + timed + '" id="' + categoryK + '-' + key + '" /><label for="' + categoryK + '-' + key + '">' + value.displayName + '</label><a href="#" class="edit" data-parent="' + value.displayName + '">edit</a><span class="value"><select class="time dropdown" tabindex="' + j + '" name="' + categoryK + key + '" data-time="' + value.time + '"><option value="week">week</option><option selected="selected" value="month">month</option><option value="semester">semester</option><option value="year">year</option></select><span class="dash">/ </span> <input type="text" class="amount" disabled="disabled" data-value="' + value.value + '" value="' + timed + '" /> </span></dt></dl>';
                                    } else {
                                        var sum = 0;
                                        $.each(value, function (key, value) {
                                            if (curriculum.global.utils.server.isExpense(value)) {
                                                if (!value.Complex) {
                                                    sum += curriculum.global.utils.determineRate.init(value.value, value.time);
                                                }
                                            }
                                        });
                                        parents += '<dl class="listing clearfix" data-parent="' + key + '" data-name="transportation"><dt class="listing clearfix"><input type="checkbox" disabled="false" data-value="' + sum + '" id="' + categoryK + '-' + key + '" /><label for="' + categoryK + '-' + key + '">' + value.displayName + '</label><a href="#" class="edit" data-parent="' + value.displayName + '">edit</a><span class="value"><select class="time dropdown" tabindex="' + j + '" name="' + categoryK + key + '" data-time="' + value.time + '"><option value="week">week</option><option selected="selected" value="month">month</option><option value="semester">semester</option><option value="year">year</option></select><span class="dash">/ </span> <input type="text" class="amount" disabled="disabled" data-value="' + value.value + '" value="' + sum + '" /></span></dt></dl>';
                                    }
                                }
                            });
                        } else {
                            timed = curriculum.global.utils.determineRate.init(categoryV.value, categoryV.time);

                            if (userData.expenseList[categoryK] > 0) {
                                if (categoryK == 'credit') {
                                    parents += '<dl class="listing clearfix" data-parent="' + categoryK + '" data-name="' + categoryK + '"><dt class="clearfix listing"><label for="' + categoryK + '">' + categoryV.displayName + '</label><a href="#" class="question">' + 0 + '</a><div class="question-hover">You\'ve indicated you have a balance of <strong><span class="balance">$100</span> + interest</strong> left on your card each month. This amount is not included in your monthly cash-out.<div class="wing"></div></div><a href="#" data-parent="' + categoryV.displayName + '" class="edit-credit">edit</a><span class="value"> <select tabindex="' + j + '" name="' + categoryK + '" class="time dropdown" data-time="' + categoryV.time + '"><option value="week">week</option><option selected="selected" value="month">month</option><option value="semester">semester</option><option value="year">year</option></select><span class="dash">/ </span> <input type="text" class="amount" disabled="disabled" value="' + userData.expenseList[categoryK] + '"/></span></dt></dl>';
                                } else {
                                    parents += '<dl class="listing clearfix" data-parent="' + categoryK + '" data-name="' + categoryK + '"><dt class="clearfix listing"><input type="checkbox" disabled="false" data-value="' + timed + '" id="' + categoryK + '" /><label for="' + categoryK + '">' + categoryV.displayName + '</label><a href="#" data-parent="' + categoryV.displayName + '" class="edit">edit</a><span class="value"><select tabindex="' + j + '" name="' + categoryK + '" class="time dropdown" data-time="' + categoryV.time + '"><option value="week">week</option><option selected="selected" value="month">month</option><option value="semester">semester</option><option value="year">year</option></select><span class="dash">/ </span><input type="text" data-value="' + categoryV.value + '" class="amount" disabled="disabled" value="' + userData.expenseList[categoryK] + '" /></span></dt></dl>';
                                }
                            } else if (this.CreditExpense) {
                                //Need to add the simple expense into DOM, but hide it from regular view since its on credit. Needs to be present for editing in credit card entry state

                            }
                        }
                        j++;
                    }
                });

                //insert into DOM
                $('div.expenses .table .tbody .data').html(parents);


                //loop through children
                $.each(userData.expenses, function (categoryK, categoryV) {

                    if (curriculum.global.utils.server.isExpense(categoryV) ) {
                        if (categoryV.value == 0) {

                            //this is a complex object which requires more looping
                            $.each(categoryV, function (key, value) {

                                if (curriculum.global.utils.server.isExpense(value) || key == 'cost') {

                                    //simple transportation types
                                    thisParent = 'dl[data-parent="' + categoryK + '"]';
                                    transportationParent = 'dl[data-parent="' + key + '"]';
                                    timed = curriculum.global.utils.determineRate.init(value.value, value.time);

                                    if (key == 'cost') {
                                        $.each(value, function (k, v) {
                                            timed = curriculum.global.utils.determineRate.init(v.value, v.time);
                                            $(thisParent).append('<dd class="listing" data-name="' + key + '.' + k + '"><input type="checkbox" disabled="false" data-value="' + timed + '" class="sub" id="' + categoryK + '-' + k + '" /><label for="' + categoryK + '-' + k + '">' + v.displayName + '</label><span class="value"><select tabindex="' + j + '" name="' + categoryK + k + '" class="time dropdown" data-time="' + v.time + '"><option value="week">week</option><option selected="selected" value="month">month</option><option value="semester">semester</option><option value="year">year</option></select><span class="dash">/ </span> <input type="text" class="amount" value="' + v.value + '" /></span></dd>');
                                        });
                                    } else if (categoryK == 'transportation' && value.value == 0) {
                                        //this only covers car
                                        carTotal = 0;
                                        $.each(value, function (k, v) {
                                            if (curriculum.global.utils.server.isExpense(v)) {
                                                timed = curriculum.global.utils.determineRate.init(v.value, v.time);
                                                $(transportationParent).append('<dd class="listing" data-name="' + key + '.' + k + '"><input type="checkbox" disabled="false" data-value="' + timed + '" class="sub" id="' + categoryK + '-' + k + '" /><label for="' + categoryK + '-' + k + '">' + v.displayName + '</label><span class="value"><select tabindex="' + j + '" name="' + categoryK + k + '" class="time dropdown" data-time="' + v.time + '"><option value="week">week</option><option selected="selected" value="month">month</option><option value="semester">semester</option><option value="year">year</option></select><span class="dash">/ </span> <input type="text" class="amount" value="' + v.value + '" /></span></dd>');
                                                carTotal += timed;
                                            }
                                        });

                                        carParent = 'dl.' + key;
                                        $(carParent).find('span.amount:first').text(carTotal).formatCurrency({ roundToDecimalPlace: 0 });
                                    } else if (key == 'selected' || key == 'displayName') {
                                    } else {
                                        thisChild = '<dd class="listing" data-name="' + key + '"><input type="checkbox" disabled="false" class="sub" data-value="' + timed + '" id="' + categoryK + '-' + key + '" /><label for="' + categoryK + '-' + key + '">' + value.displayName + '</label><span class="value"><select tabindex="' + j + '" name="' + categoryK + key + '" class="time dropdown" data-time="' + value.time + '"><option value="week">week</option><option selected="selected" value="month">month</option><option value="semester">semester</option><option value="year">year</option></select><span class="dash">/ </span> <input type="text" class="amount"  value="' + value.value + '" /></span></dd>';

                                        $(thisParent).append(thisChild);
                                    }
                                }
                            });
                        }
                        j++;
                    }
                });



                $('div.expenses .table .tbody .data input[type=checkbox]').styledInputs();
                //$('#curriculum-review .dropdown').styledDropDowns();
                $('#curriculum-review .dropdown').dropkick();
                $('div.expenses .table dl:first').find('dt').addClass('first');

                ///////


                //loop through the checked options and check them
                $('div.expenses dl dt input[type="checkbox"]').each(function (k, v) {
                    if ($(this).parent().siblings().length) {
                        $(this).addClass('omit');
                        $(this).parents('dl').addClass('hasChildren');
                    }
                });

                review.global.utils.dataEvents();
                totalCredit = 0;


                //loop through userData.creditExpenses
                //which are the previously selected checkboxes for creditcard
                if (userData.creditExpenses != undefined) {
                    $.each(userData.creditExpenses, function (key, value) {
                        if (!$('#' + value).hasClass('omit')) {
                            $('#' + value).removeAttr('disabled');

                            if ($.browser.msie) {
                                $('#' + value).prop("checked", true).trigger('propertychange');
                            } else {
                                $('#' + value).prop("checked", true).trigger('change');
                            }

                            $('#' + value).attr('disabled', 'true');
                        }
                    });
                }

                if (userData.expenses.credit != undefined) {
                    ccBalance = totalCredit - curriculum.global.utils.determineRate.init(userData.expenses.credit.value, userData.expenses.credit.time);
                    $('.question-hover .balance').text(ccBalance).formatCurrency({ roundToDecimalPlace: 0 });
                }

                // This is where Money Out gets set initially
                $('div.expenses .table .thead .month-total').text(totalExpenses).formatCurrency({ roundToDecimalPlace: 0 });
                $('.value .amount').formatCurrency({ roundToDecimalPlace: 0 });

                $('.cc-expenses-total').bind('change keyup blur', function () {
                    $(this).toNumber();
                    $(this).formatCurrency({ roundToDecimalPlace: 0 });
                });


                // yes buttons
                $('button#cc-yes').click(function (e) {
                    e.preventDefault();

                    curriculum.global.tracking.trigger("lesson:step:editCredit", {
                      step: {
                        number: 4
                      }
                    });
                    review.global.utils.showEditCredit();
                });

                //edit credit button
                $('.expenses .edit-credit').click(function (e) {
                    e.preventDefault();
                    review.global.utils.showEditCredit();
                });

                //save buttons
                $('.interim .cc-save').click(function () {
                    var $creditCardExpenseTotal = $('.cc-expenses-total'),
                        cleanCCTotal = curriculum.global.utils.cleanInput.init($creditCardExpenseTotal.val()),
                        ERROR_CLASS = "error";

                    if( $("#curriculum-review .expenses input:checked").length === 0 ) {
                        cleanCCTotal = 0;
                        $creditCardExpenseTotal.val(0);
                    }

                    review.global.utils.gatherBoxesCC();
                    // Ensure the user enters a credit card value if they want to save.
                    if( cleanCCTotal > 0 || $(".cc-total-value").text() == "$0" || userData.creditExpenses.length == 0) {
                        $creditCardExpenseTotal.removeClass(ERROR_CLASS);

                        $('div.expenses input[type="checkbox"]').attr('disabled', 'true');
                        $('div.expenses dl dd.listing').addClass('hidden');
                        $(this).closest('.table').removeClass('cc-edit');

                        //ccTotal = totalCredit;

                        userData.expenses.credit = $.extend(userData.expenses.credit, { displayName: 'Credit Card', value: cleanCCTotal, time: $('#cc-dropkick-time').val(), omit: true });
                        userData.expenseList.credit = curriculum.global.utils.determineRate.init(cleanCCTotal, $('#cc-dropkick-time').val());
                        review.global.utils.startData.expenses(false);

                        $('.interim, .cc-details').hide();
                        $('.question-hover').fadeIn().delay(2000).fadeOut();

                        review.global.utils.startData.setInactiveExpenses();
                    } else {
                        $creditCardExpenseTotal.addClass(ERROR_CLASS);
                        $creditCardExpenseTotal.focus();
                    }
                });

                // close/cancel buttons
                $('.interim .cc-cancel').click(function () {
                    var $creditCardExpenseTotal = $('.cc-expenses-total'),
                        ERROR_CLASS = "error";

                    $creditCardExpenseTotal.removeClass(ERROR_CLASS);

                    $('.expenses .edit-back').show();
                    $('dl.listing[data-name="credit"]').show();
                    $('.interim, .cc-details').fadeOut('fast');

                    $(this).closest('.table').removeClass('cc-edit');
                    $('div.expenses input[type="checkbox"]').attr('disabled', 'true');
                    $('div.expenses dl dd.listing').addClass('hidden');

                    review.global.utils.startData.expenses(false);
                });


                review.global.utils.startData.setInactiveExpenses();
                review.global.utils.startData.recalculateMoneyOutLineItems();

                $(".question").html(review.global.utils.getTotalCreditExpenseCount());


                var totalIncome = curriculum.global.utils.totalIncome.value();

                var moneyOutValue = 0;
                $(".data dt").not(".inactive").find(".amount").each(function() {
                    moneyOutValue += parseInt($(this).val().replace(",", "").replace("$", ""));
                });

                var totalExpenses = moneyOutValue;  // Math.round(curriculum.global.utils.totalExpenses.value());
                var cashflow = totalIncome - totalExpenses;

                $('div.expenses .table .thead .month-total').text(totalExpenses).formatCurrency({ roundToDecimalPlace: 0 });


                //set cashflow
                if (cashflow < 0) {
                    $('#total .value').addClass('negative');
                    $('#total .value').removeClass('positive');
                } else {
                    $('#total .value').addClass('positive');
                    $('#total .value').removeClass('negative');
                }
                $('#total .value').text(cashflow).formatCurrency({ roundToDecimalPlace: 0, negativeFormat: '-%s%n' });

                $('#edit-expenses').click(function (e) {
                    e.preventDefault();
                    //currentPagePosition = 2;
                    curriculum.global.utils.paginate.currentPagePosition = 2;
                    curriculum.global.utils.paginate.navigateSpecific('expenses.html');

                });
                ///////

            }, // END expenses()

            recalculateMoneyOutLineItems: function() {
                $("dl.listing").each(function() {
                    var $this = $(this);

                    if( $this.data("name") !== "credit" ) {
                        var lineItemTotal = 0,
                            $children = $this.find("dd.listing");

                        $children.not(".inactive").each(function() {
                           lineItemTotal += $(this).find("input").data("value");
                        });

                        if( $children.length > 0 ) {
                            $this.find("dt .amount").val(lineItemTotal).formatCurrency({ roundToDecimalPlace: 0 });;
                        }
                    }
                });
            },

            setInactiveExpenses: function() {
                $("#curriculum-review .data").find("input:checked").each(function() {
                    $(this).parent().addClass("inactive");
                });
            },

            income: function () {
                incomeContainer = $('div.income .table .tbody');

                ////
                $.each(userData.income, function (incomeK, incomeV) {
                    if (incomeV.time == 'semester') {
                        timeFix = 'sem.';
                    } else {
                        timeFix = incomeV.time;
                    }
                    row = '<div class="tr clearfix"><div class="th">' + incomeK + '</div><div class="td month">' + timeFix + '</div><div class="td slash">/</div><div class="td month-total in">' + incomeV.value + '</div></div>';
                    incomeContainer.append(row);
                });

                //////
                totalIncome = Math.round(curriculum.global.utils.totalIncome.value());
                $('div.income .table .thead .month-total').text(totalIncome).formatCurrency({ roundToDecimalPlace: 0 });
                $('.month-total').formatCurrency({ roundToDecimalPlace: 0 });
            } // END income()
        }, //END startData()
        saveData: function () {
            curriculum.global.utils.server.saveToServer(userData);
        }, // END saveData
        errors: function () {
            pagePass = true;
            curriculum.global.utils.paginate.next();
        }
    } // END: utils
}; // END review global var


$('document').ready(function(){
    review.global.utils.init();
});
