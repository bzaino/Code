var transportation = transportation || {};

save = transportation;
var boxesArray = [];
var totalExpense;
var savedObject;
var transportationTypes = [];
var theType = '';
var errorMessage = 'Oops, be sure to select at least one.';
var expensesFullyLoaded = false;

var existingTransportation = [];
var transportationSum = 0;

transportation.global = {
    step: 4,
    utils: {
        init: function () {
            curriculum.global.viewport.animateViewport.normal();
            $("#content-container .content").show().css({ opacity: 0 });
            //include stylesheet & append class for this page`
            $('#content-container .content').attr('id', 'curriculum-transportation');
            //update page context
            curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on transportation", "Expenses");

            //trigger webtrends
            curriculum.global.tracking.trigger("lesson:step:start", {
                step: {
                    number: save.global.step
                }
            });

            transportation.global.utils.preloadData.init();
            transportation.global.utils.handleAddMoreButton();
            transportation.global.utils.handleRemoveButton();
            $('#total .value').text("$0");
        }, //END init
        handleTransportationChange: function (dropkick) {
            curriculum.global.viewport.getNewViewport();

            //if this has already been selected
            var $element = $(dropkick.data.select);
            if ($element.parents('.question-wrapper').hasClass('modified')) {
                previousRel = $element.parents('.question-wrapper').attr('rel');
                transportation.global.utils.removeTransportation(previousRel);
            }

            var theType = dropkick.value;

            if (theType == 'drive-a-car') {
                selectedTransportation = 'car';

                var theURL = 'includes/transportation-type-2.html';
                var thisIndex = $element.parents('.question-wrapper').index();

                where = $('#curriculum-transportation .question .question-wrapper').eq(thisIndex);

                $.ajax(theURL, {
                    cache: false,
                    async: false,
                    success: function (response) {
                        var html = response;
                        where.after(html).remove();

                        $('#curriculum-transportation .question .question-wrapper').eq(thisIndex).addClass('modified car').attr('rel', selectedTransportation).attr('data-display', 'Car').slideDown();

                        $('#curriculum-transportation .question .question-wrapper').eq(thisIndex).find('select').each(function (key, value) {
                            $(value).attr('name', theType + '-' + key);
                        });

                        //re-init all the stuff again
                        $('#curriculum-transportation input[type=checkbox]').styledInputs();

                        //style the transportation type dropdowns
                        $('#curriculum-transportation .dropdown-large.transportation').dropkick({
                            theme: 'large-transportation',
                            change: function () {
                                transportation.global.utils.handleTransportationChange(this);
                            }
                        });
                        //style the small .time dropdowns in car
                        $('#curriculum-transportation .dropdown.time').dropkick({
                            change: function () {
                                transportation.global.utils.updateExpense.init();
                            }
                        });

                        transportation.global.utils.cboxToggles($('#curriculum-transportation .question .question-wrapper').eq(thisIndex));
                        curriculum.global.utils.fixConflictingID('.question-wrapper');
                        transportation.global.utils.updateTransportationTypes(selectedTransportation);
                        transportation.global.utils.gatherAllBoxes.init();
                    }
                });

            } else if (theType == 'ride-a-bike') {

                selectedTransportation = 'bike';

                if ($element.parents('.question-wrapper').hasClass('car')) {

                    thisIndex = $element.parents('.question-wrapper').index();

                    theURL = 'includes/transportation-type-1.html';
                    where = $('#curriculum-transportation .question .question-wrapper').eq(thisIndex);

                    $.ajax(theURL, {
                        cache: false,
                        async: false,
                        success: function (response) {
                            var html = response;
                            where.after(html).remove();

                            //prep the transportation type dropdown to show what it needs to show

                            $('.dropdown-large.transportation option', $('#curriculum-transportation .question .question-wrapper').eq(thisIndex)).each(function (key, value) {

                                if ($(value).val() == 'ride-a-bike') {
                                    $(value).attr('selected', 'selected');
                                }
                            });

                            $('#curriculum-transportation .question .question-wrapper').eq(thisIndex).addClass('modified').attr('rel', selectedTransportation).attr('data-display', 'Bike');

                            $('#curriculum-transportation .question .question-wrapper').eq(thisIndex).find('select').each(function (key, value) {
                                $(value).attr('name', theType + '-' + key);
                            });

                            //re-init all the stuff again
                            $('#curriculum-transportation input[type=checkbox]').styledInputs();

                            //style the transportation type dropdowns
                            $('#curriculum-transportation .dropdown-large.transportation').dropkick({
                                theme: 'large-transportation',
                                change: function () {
                                    transportation.global.utils.handleTransportationChange(this);
                                }
                            });
                            //style the large .time dropdowns
                            $('#curriculum-transportation .dropdown-large.time').dropkick({
                                theme: 'large',
                                change: function () {
                                    transportation.global.utils.updateExpense.init();
                                }
                            });

                            curriculum.global.utils.fixConflictingID('.question-wrapper');
                            transportation.global.utils.updateTransportationTypes(selectedTransportation);
                            transportation.global.utils.gatherAllBoxes.init();
                        }
                    });


                } else {//if adding new html
                    $element.parents('.question-type-1').addClass('modified').attr('rel', selectedTransportation).removeClass('car').attr('data-display', 'Bike');
                    $element.parents('.question-type-1').find('select').each(function (key, value) {
                        $(value).attr('name', theType + '-' + key);
                    });
                    transportation.global.utils.updateTransportationTypes(selectedTransportation);
                    transportation.global.utils.calculateExpense.init();
                } // end of if bike

            } else if (theType == 'use-public-transit') {
                selectedTransportation = 'transit';

                if ($element.parents('.question-wrapper').hasClass('car')) {

                    thisIndex = $element.parents('.question-wrapper').index();

                    theURL = 'includes/transportation-type-1.html';
                    where = $('#curriculum-transportation .question .question-wrapper').eq(thisIndex);

                    $.ajax(theURL, {
                        cache: false,
                        async: false,
                        success: function (response) {
                            var html = response;
                            where.after(html).remove();
                            $('.dropdown-large.transportation option', $('#curriculum-transportation .question .question-wrapper').eq(thisIndex)).each(function (key, value) {

                                if ($(value).val() == 'use-public-transit') {
                                    $(value).attr('selected', 'selected');
                                }

                            });

                            $('#curriculum-transportation .question .question-wrapper').eq(thisIndex).addClass('modified').attr('rel', selectedTransportation).attr('data-display', 'Public Transit');

                            $('#curriculum-transportation .question .question-wrapper').eq(thisIndex).find('select').each(function (key, value) {
                                $(value).attr('name', theType + '-' + key);
                            });

                            //re-init all the stuff again
                            $('#curriculum-transportation input[type=checkbox]').styledInputs();
                            //$('#curriculum-transportation .dropdown-large').styledDropDowns({ddClass: 'dd-large', autoAdjust: true});

                            //style large transportation dropdown
                            $('#curriculum-transportation .dropdown-large.transportation').dropkick({
                                theme: 'large-transportation',
                                change: function () {
                                    transportation.global.utils.handleTransportationChange(this);
                                }
                            });

                            $('#curriculum-transportation .dropdown-large.time').dropkick({
                                theme: 'large',
                                change: function () {
                                    transportation.global.utils.updateExpense.init();
                                }
                            });

                            curriculum.global.utils.fixConflictingID('.question-wrapper');
                            transportation.global.utils.updateTransportationTypes(selectedTransportation);
                            transportation.global.utils.gatherAllBoxes.init();
                        }
                    });

                } else {//if adding new html
                    $element.parents('.question-type-1').addClass('modified').attr('rel', selectedTransportation).removeClass('car').attr('data-display', 'Public Transit');
                    $element.parents('.question-type-1').find('select').each(function (key, value) {
                        $(value).attr('name', theType + '-' + key);
                    });
                    transportation.global.utils.updateTransportationTypes(selectedTransportation);
                    transportation.global.utils.calculateExpense.init();

                } // end of if transit
            } //end if val is car / bike / transit
        }, //END handleTransportaionChange
        handleRemoveButton: function () {
            $('#curriculum-transportation a.btn-remove').die('click');
            $('#curriculum-transportation a.btn-remove').live('click', function (e) {
                e.preventDefault();
                var value = $(this).parents('.question-wrapper');

                transportation.global.utils.removeTransportation(value.attr('rel'));
                value.remove();

                transportation.global.utils.gatherAllBoxes.init();
                transportation.global.utils.calculateExpense.init();
                transportation.global.utils.updateExpense.init();

                transportation.global.utils.updateDropdownValues();

                transportation.global.utils.updateAddRemoveButtons();
                transportation.global.utils.updateGrammar();

                curriculum.global.viewport.getNewViewport();
            });

            transportation.global.utils.updateAddRemoveButtons();
        },
        updateGrammar: function () {
            $('.i:first').html('I');
        },
        updateAddRemoveButtons: function () {
            //toggle the display of the add/remove buttons
            if ($('.question-wrapper').length == 1) {
                $('.add-another').show();
                $('.btn-remove').hide();
            } else if ($('.question-wrapper').length == 2) {
                $('.add-another').show();
                $('.btn-remove').show();
            } else {
                $('.add-another').hide();
                $('.btn-remove').show();
            }
        },
        updateTransportationTypes: function (selectedTransportation, preloaded) {
            transportationTypes.push(selectedTransportation);
            if (!preloaded) {
                transportation.global.utils.updateExpense.init();
                transportation.global.utils.updateDropdownValues();
            }
        }, //END updateTransportationTypes
        handleAddMoreButton: function () {
            $('.add-another').click(function (e) {
                e.preventDefault();
                //only allow another to be added IF
                //the number of existing questions have been selected.
                //you cannot add another one if you have an empty selection waiting
                if ($('.question-wrapper').length == $('.question-wrapper.modified').length) {

                    theURL = 'includes/transportation-type-1.html';

                    $.ajax(theURL, {
                        cache: false,
                        async: false,
                        success: function (response) {
                            var html = response;

                            $('.question').append(html);
                            var newQuestionContainer = $('.question .question-wrapper:last').show();

                            //scan for existing transportation types
                            $.each(transportationTypes, function (key, value) {
                                //figure out what got added and then remove it from the select elements
                                if (value == 'car') {
                                    var thisIteration = $(newQuestionContainer).find('select.transportation').find('option[value="drive-a-car"]');
                                } else if (value == 'bike') {
                                    var thisIteration = $(newQuestionContainer).find('select.transportation').find('option[value="ride-a-bike"]');
                                } else if (value == 'transit') {
                                    var thisIteration = $(newQuestionContainer).find('select.transportation').find('option[value="use-public-transit"]');
                                }

                                if (!thisIteration.attr('selected')) {
                                    thisIteration.remove();
                                }
                            });

                            $(newQuestionContainer).find('select').each(function (key, value) {
                                $(value).attr('name', $(value).attr('name') + '-' + $('.question-wrapper').length + '-' + key);
                            });

                            //re-init all the plugins
                            $(newQuestionContainer).find('input[type=checkbox]').styledInputs();
                            //$(newQuestionContainer).find('.dropdown-large').styledDropDowns({ddClass: 'dd-large', autoAdjust: true});
                            $(newQuestionContainer).find('.dropdown-large.transportation').dropkick({
                                theme: 'large-transportation',
                                change: function () {
                                    transportation.global.utils.handleTransportationChange(this);
                                }
                            });
                            $(newQuestionContainer).find('.dropdown-large.time').dropkick({
                                theme: 'large',
                                change: function () {
                                    transportation.global.utils.updateExpense.init();
                                }
                            });
                            //transportation.global.utils.calculateExpense.init();
                            //transportation.global.utils.handleTransportationChange();

                            transportation.global.utils.updateAddRemoveButtons();

                            curriculum.global.viewport.getNewViewport();

                        }
                    });

                }


            });

            transportation.global.utils.updateAddRemoveButtons();
        }, //END handleAddMoreButton
        cboxToggles: function (scope) {
            //togle tip on and off on click

            scope.find('input[type=checkbox]').bind('propertychange change', function () {
                //keep checking which boxes are selected when the boxes are added or removed

                if ($(this).is(':checked')) {
                    //clear all of its related checkboxes of the error
                    $(this).parents('form').find('.box-wrapper .cbox').each(function () {
                        $(this).removeClass('error');
                    });
                    //bold the label
                    $(this).siblings('label').addClass('bold');
                    //show content
                    $(this).siblings('.tip').slideDown('normal', function () {
                        $('.details', $(this)).fadeIn('fast');
                        //set focus
                        $(this).find('.details input').focus();

                    });
                } else {
                    $(this).siblings('label').removeClass('bold');
                    //hide content
                    $(this).siblings('.tip').find('.details input').removeClass('error');
                    $(this).siblings('.tip').slideUp('normal', function () {
                        $('.details', $(this)).fadeOut('fast');
                    });

                }

                curriculum.global.viewport.getNewViewport();
                transportation.global.utils.gatherAllBoxes.init();
                transportation.global.utils.updateExpense.init();
            });
        }, //END cboxTogle
        calculateExpense: {
            init: function () {
                //manipulate user input text
                $('#curriculum-transportation .question .box-wrapper input[type=text], .question-type-1 input.value').die('keyup change blur');
                $('#curriculum-transportation .question .box-wrapper input[type=text], .question-type-1 input.value').live('keyup change blur', function (e) {
                    $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
                    transportation.global.utils.updateExpense.init();
                    if ($(this).hasClass('error')) {
                        cleanVal = curriculum.global.utils.cleanInput.init($(this).val());
                        if (curriculum.global.utils.isEmpty(cleanVal) || cleanVal == 0) {
                            $(this).addClass('error');
                        } else {
                            $(this).removeClass('error');
                        }
                    }
                    //highlight, only on keyup
                    if (e.type == 'keyup' || e.type == 'blur') {
                        $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
                    }


                });

                $('#curriculum-transportation select.time').die('change');
                $('#curriculum-transportation select.time').live('change', function () {
                    transportation.global.utils.updateExpense.init();
                });



            }
        }, // END handleTips
        updateExpense: {
            init: function () {
                /**
                this updates the local savedObject
                as well as the front-end display
                of what you are changing
                */

                //reset totalExpense and savedObject
                totalExpense = 0;
                savedObject = {};
                if (!userData.expenses.transportation) {
                    userData.expenses.transportation = {};
                }
                $('#total .value').text(totalExpense).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

                //insert the types of transportations into savedObject
                for (var g = 0; g < transportationTypes.length; g++) {
                    type = transportationTypes[g];
                    savedObject[type] = $.extend(savedObject[type], {});
                    savedObject[type].Name = type + "_" + g;

                    if (type == 'car') {
                        savedObject[type].displayName = 'Car';
                        savedObject[type].Complex = true;
                    }

                };

                //for each group of checkboxes in each question
                for (var h = 0; h < boxesArray.length; h++) {
                    for (var i = 0; i < boxesArray[h].length; i++) {
                        //format the input side
                        value = $(boxesArray[h][i]).siblings('.tip').find('input').val();
                        clean = curriculum.global.utils.cleanInput.init(value);

                        //get the rate at which the income is gathered
                        rate = boxesArray[h][i].siblings('.tip').find('select').val();

                        timed = curriculum.global.utils.determineRate.init(clean, rate);


                        //add each one up
                        totalExpense += timed;

                        //set the dom elements to show the correct expense
                        $('#total .value').text(totalExpense).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
                        //animate the graph

                        //update the object that we're keeping
                        mode = transportationTypes[h];
                        particular = boxesArray[h][i].attr('data-name');
                        var displayName = boxesArray[h][i].attr('data-display');


                        //create the the empty particular (payment, gas)
                        savedObject[mode][particular] = $.extend(savedObject[mode][particular], {});
                        //then insert the values from above
                        savedObject[mode][particular] = $.extend(savedObject[mode][particular], { displayName: displayName, value: clean, time: rate, newValue: null });
                    }
                }
                //end of checkboxes loop

                $('#curriculum-transportation .question-type-1').each(function (key1, value1) {
                    value = $(this).find('input.value').val();
                    clean = curriculum.global.utils.cleanInput.init(value);

                    rate = $(value1).find('select.time').val();
                    timed = curriculum.global.utils.determineRate.init(clean, rate);

                    totalExpense += timed;

                    //set the dom elements to show the correct expense
                    $('#total .value').text(totalExpense).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

                    rel = $(this).attr('rel');
                    displayName = $(this).attr('data-display');
                    if (typeof rel != 'undefined') {
                        savedObject[rel] = $.extend(savedObject[rel], { displayName: displayName, value: clean, time: rate, newValue: null });
                    }

                });
                //end of single input loop

                userData.expenseList.transportation = totalExpense;

                // prevent modifying expenses until data is fully loaded on form
                if (expensesFullyLoaded) {
                    var expenses = userData.expenses.transportation;
                    for (var key in expenses) {
                        if (expenses.hasOwnProperty(key) && curriculum.global.utils.server.isExpense(expenses[key])) {
                            if (savedObject[key] != undefined) {
                                // update any existing expenses with changes (true indicates deep copy)


                                var childExpenses = expenses[key];
                                for (var childKey in childExpenses) {
                                    if (expenses[key].hasOwnProperty(childKey) && curriculum.global.utils.server.isExpense(expenses[key][childKey])) {
                                        if (savedObject[key][childKey] != undefined) {
                                            // update any existing expenses with changes (true indicates deep copy)
                                            expenses[key][childKey] = $.extend(expenses[key][childKey], savedObject[key][childKey]);
                                            delete savedObject[key][childKey];
                                        } else {
                                            // delete any removed expenses
                                            delete expenses[key][childKey];
                                        }
                                    }
                                }
                                for (var newKey in savedObject[key]) {
                                    if (savedObject[key].hasOwnProperty(newKey)) {
                                        // add any new expenses
                                        expenses[key][newKey] = $.extend(expenses[key][newKey], savedObject[key][newKey]);
                                    }
                                }

                                expenses[key] = $.extend(expenses[key], savedObject[key]);

                                delete savedObject[key];
                            } else {
                                // delete any removed expenses
                                delete expenses[key];
                            }
                        }
                    }

                    for (var key in savedObject) {
                        if (savedObject.hasOwnProperty(key)) {
                            // add any new expenses
                            expenses[key] = $.extend(expenses[key], savedObject[key]);
                        }
                    }
                    userData.expenses.transportation = expenses;
                    curriculum.global.utils.animateGraph.refresh();
                }
            } //end init
        }, //END updateExpense
        gatherAllBoxes: {
            init: function () {


                boxesArray = [];
                //comepose an array with sub arrays for each .question element
                //so that each question can have its own set of checkboxes
                //for eavery question
                numZero = 0;
                $('#curriculum-transportation .question-wrapper').each(function (key1, value1) {
                    //for every input inside every question
                    boxesArray[key1] = [];
                    $('input', $(this)).each(function (key2, value2) {
                        //if the input is :checked
                        if ($(value2).is(':checked')) {
                            boxesArray[key1].push($(this));
                        }

                    });

                    //go through each array element and determine if it is empty
                    //if it is, then add  to the numZero variable

                    if (boxesArray[key1].length == 0) {
                        numZero++;
                    }
                    //if the number of empty arrays is equal to the value of numZero - it means all arrays are empty
                    if (numZero == boxesArray.length) {
                        $('#total .value').text("$0");
                    }


                });



            }
        }, //END gatherAllBoxes
        removeTransportation: function (what) {

            transportationTypes = $.grep(transportationTypes, function (v) {
                return v != what;
            });

            //transportation.global.utils.updateDropdownValues();
        },
        updateDropdownValues: function () {
            //scan for existing transportation types
            //clear out and reset <option> values for all select elements

            $('select.dropdown-large.transportation').empty().append('<option class="select-one" selected="selected" value="select-one">select one</option><option value="drive-a-car">drive a car</option><option value="ride-a-bike">ride a bike</option><option value="use-public-transit">use public transit</option>').removeClass('ddStyled');

            //loop through each question
            $('.question-wrapper').each(function (key, value) {
                var selection = $(this).attr('data-display');

                if (selection !== undefined) {

                    //reset the 'selected' <option>
                    if (selection == "Car") {
                        $(this).find('select.dropdown-large.transportation option[value="drive-a-car"]').attr('selected', 'selected');
                    } else if (selection == "Bike") {
                        $(this).find('select.dropdown-large.transportation option[value="ride-a-bike"]').attr('selected', 'selected');
                    } else if (selection == "Public Transit") {
                        $(this).find('select.dropdown-large.transportation option[value="use-public-transit"]').attr('selected', 'selected');
                    }
                }

                //for each question, loop through each already selected transportation
                $.each(transportationTypes, function (k, v) {

                    //and remove them from the <select> elements
                    if (v == 'car') {
                        if (!$(value).find('select.dropdown-large.transportation option[value="drive-a-car"]').attr('selected')) {

                            $(value).find('select.dropdown-large.transportation option[value="drive-a-car"]').remove();
                        }
                    } else if (v == 'bike') {
                        if (!$(value).find('select.dropdown-large.transportation option[value="ride-a-bike"]').attr('selected')) {

                            $(value).find('select.dropdown-large.transportation option[value="ride-a-bike"]').remove();
                        }
                    } else if (v == 'transit') {
                        if (!$(value).find('select.dropdown-large.transportation option[value="use-public-transit"]').attr('selected')) {

                            $(value).find('select.dropdown-large.transportation option[value="use-public-transit"]').remove();
                        }
                    }

                });

                //remve and re-run the dropdown plugin
                //$(this).find('select.dropdown-large.transportation').siblings('.dd-large').remove();
                $(this).find('select.dropdown-large.transportation').removeData('dropkick');
                $(this).find('select.dropdown-large.transportation').siblings('.dk_container').remove();
                //$(this).find('select.dropdown-large.transportation').styledDropDowns({ddClass: 'dd-large', autoAdjust: true});

                $(this).find('select.dropdown-large.transportation').dropkick({
                    theme: 'large-transportation',
                    change: function () {
                        transportation.global.utils.handleTransportationChange(this);
                    },
                    startSpeed: 0 // prevents the fade-in effect on repopulate
                });

            });





        }, //END updateDropdownValues()
        loadNewQuestion: function (whatToLoad, whereToLoad) {
            theURL = 'includes/transportation-type-1.html';

            $.ajax(theURL, {
                cache: false,
                async: false,
                success: function (response) {
                    var html = response;
                    $("#curriculum-transportation .question.transportation-container").append(html);
                    $('#curriculum-transportation .question-wrapper').show();

                    //after loading 1st quesiton. init all of the style plugins and calculation functions  
                    //$('#curriculum-transportation .dropdown-large').styledDropDowns({ddClass: 'dd-large', autoAdjust: true});
                    $('#curriculum-transportation .dropdown-large.transportation').dropkick({
                        theme: 'large-transportation',
                        change: function () {
                            transportation.global.utils.handleTransportationChange(this);
                        }
                    });
                    //$('#curriculum-transportation .dropdown').styledDropDowns();
                    $('#curriculum-transportation .dropdown-large.time').dropkick({
                        theme: 'large',
                        change: function () {
                            transportation.global.utils.updateExpense.init();
                        }
                    });
                    //transportation.global.utils.cboxToggles();
                    transportation.global.utils.calculateExpense.init();
                }
            });

        }, //END loadNewQuestion
        saveData: function () {
            userData.expenses.transportation.displayName = "Transportation";
            curriculum.global.utils.server.saveToServer(userData);
        }, //END saveData
        preloadData: {
            init: function () {
                if (userData.expenses.transportation != undefined) {

                    var index = 0;

                    //transportation.global.utils.cboxToggles();

                    var sortedTransporation = [];
                    $.each(userData.expenses.transportation, function (transportK, transportV) {
                        if (curriculum.global.utils.server.isExpense(transportV)) {
                            sortedTransporation.push({ key: transportK, value: transportV });
                        }
                    });
                    sortedTransporation.sort(function (a, b) {
                        var indexA = Number(a.value.Name.split('_')[1]);
                        var indexB = Number(b.value.Name.split('_')[1]);
                        return indexB < indexA;
                    });
                    $.each(sortedTransporation, function (index, value) {
                        transportK = value.key;
                        transportV = value.value;
                        //if (curriculum.global.utils.server.isExpense(transportV)) {
                        var transportationType = transportK;
                        //////////
                        if (transportationType == 'car') {
                            theType = 'drive-a-car';
                            theDisplayType = 'Car';

                            $.ajax('includes/transportation-type-2.html', {
                                cache: false,
                                async: false,
                                success: function (response) {
                                    var html = response;

                                    $('#curriculum-transportation .question').append(html);

                                    //update transportationTypes array
                                    transportation.global.utils.updateTransportationTypes(transportK, true);
                                    curriculum.global.utils.fixConflictingID('.question-wrapper');

                                    $('#curriculum-transportation input[type=checkbox]').styledInputs();

                                    var thisIndex = index;
                                    index++;

                                    var container = $('#curriculum-transportation .question .question-wrapper').eq(thisIndex);

                                    container.attr('name', transportationType);
                                    container.addClass('modified car').attr('rel', transportK).attr('data-display', theDisplayType);

                                    transportation.global.utils.cboxToggles(container);

                                    //loop through the subset of objects
                                    $.each(transportV, function (key, value) {
                                        if (curriculum.global.utils.server.isExpense(value)) {
                                            //checkbox

                                            //value
                                            container.find('.box-wrapper input[data-name=' + key + ']').siblings('.tip').find('.details input').val(value.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
                                            //select
                                            container.find('.box-wrapper input[data-name=' + key + ']').siblings('.tip').find('.details select').val(value.time);
                                            container.find('.box-wrapper input[data-name=' + key + ']').siblings('.tip').find('.details select').trigger('change');

                                            container.find('.box-wrapper input[data-name=' + key + ']').trigger('click');
                                            //container.find('.box-wrapper input[data-name=' + key + ']').siblings('.tip').find('.details .dd ul li.' + value.time + ' a').trigger('click');
                                        }
                                    });

                                    container.find('select').each(function (key, value) {
                                        $(value).attr('name', theType + '-' + key);
                                    });

                                    //dropkick setups
                                    container.find('select').removeData('dropkick');
                                    container.find('select').siblings('.dk_dropkick').remove();
                                    container.find('select.time').dropkick({
                                        change: function () {
                                            transportation.global.utils.updateExpense.init();
                                        }
                                    });

                                    container.find('select.transportation').dropkick({
                                        theme: 'large-transportation',
                                        change: function () {
                                            transportation.global.utils.handleTransportationChange(this);
                                        }
                                    });
                                }
                            });

                        }
                        //////////
                        /////////
                        if (transportationType != 'car') {

                            $.ajax('includes/transportation-type-1.html', {
                                cache: false,
                                async: false,
                                success: function (response) {
                                    var html = response;
                                    if (transportationType == 'bike') {
                                        theType = 'ride-a-bike';
                                        theTypeDisplay = 'Bike';
                                    } else if (transportationType == 'transit') {
                                        theType = 'use-public-transit';
                                        theTypeDisplay = 'Public Transit';
                                    }

                                    //attach to DOM
                                    $('#curriculum-transportation .question').append(html);

                                    transportation.global.utils.updateTransportationTypes(transportK, true);
                                    curriculum.global.utils.fixConflictingID('.question-wrapper');

                                    var singleIndex = index;
                                    index++;

                                    var container = $('#curriculum-transportation .question .question-wrapper').eq(singleIndex);

                                    container.addClass('modified').attr('rel', transportK).attr('data-display', theTypeDisplay);

                                    //transportation type
                                    //container.find('select.transportation option[value=' + theType + ']').attr('selected', 'selected');
                                    container.find('select.transportation').val(theType);

                                    //value
                                    container.find('input.value').val(transportV.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

                                    //time
                                    //set time
                                    container.find('select.time').val(transportV.time);

                                    //rename time <select> element
                                    //container.find('select.time').attr('name', theType + '-time');
                                    //container.find('select.transportation').attr('name', theType);
                                    container.find('select').each(function (key, value) {
                                        $(value).attr('name', theType + '-' + key);
                                    });

                                    //clear all of dropkick data then reset it
                                    container.find('select').removeData('dropkick');
                                    container.find('select').siblings('.dk_dropkick').remove();
                                    container.find('select.time').dropkick({
                                        theme: 'large',
                                        change: function () {
                                            transportation.global.utils.updateExpense.init();
                                        }
                                    });

                                    container.find('select.transportation').dropkick({
                                        theme: 'large-transportation',
                                        change: function () {
                                            transportation.global.utils.handleTransportationChange(this);
                                        }
                                    });

                                    //if its the first one
                                }
                            });

                        }
                        /////////
                        //}
                    });

                    expensesFullyLoaded = true;

                    // step data was preloaded; set tracking flag to true.
                    curriculum.global.tracking.preloaded = true;

                    transportation.global.utils.updateDropdownValues();
                    transportation.global.utils.updateGrammar();

                    //done looping and now just animate the content in
                    transportation.global.utils.calculateExpense.init();
                    $("#content-container .content").delay(350).animate({
                        opacity: 1
                    }, 1000, function () {
                        // Animation complete.
                        $('.question input[type=text]').change();
                        curriculum.global.utils.animateGraph.refresh();
                    });


                } else {
                    //first time visit to this page
                    //call the first question
                    transportation.global.utils.loadNewQuestion();
                                       
                    expensesFullyLoaded = true;

                    // step data was not preloaded; set tracking flag to false.
                    curriculum.global.tracking.preloaded = false;

                    transportation.global.utils.updateGrammar();

                    $("#content-container .content").delay(350).animate({
                        opacity: 1
                    }, 1000, function () {
                        // Animation complete.

                    });

                } //end if there is data to load

            }
        }, // END preloadData
        errors: function () {
            //$('#footer .error-msg').text(errorMessage).fadeOut();
            //check to see if any data is saved in savedObject
            if ($.isEmptyObject(userData.expenses.transportation)) {

                errorMessage = 'Oops, be sure to select at least one.';
                $('input[type=text]').addClass('error');

                $('#footer .error-msg').text(errorMessage).fadeIn();
            } else {
                //loop through question type 1
                $('.question-type-1').each(function (k, v) {

                    //dropdowns
                    if ($(this).find('#type-1-transportation-dropdown').val() == 'select-one') {
                        errorMessage = 'Oops, be sure to select at least one.';
                        $(this).addClass('error');
                    }
                    //values
                    currentElement = $(this).find('.value');
                    cleanVal = curriculum.global.utils.cleanInput.init(currentElement.val());
                    if (curriculum.global.utils.isEmpty(cleanVal) || cleanVal == 0) {
                        errorMessage = 'Oops, be sure to fill in all the fields';
                        currentElement.addClass('error');
                    } else {
                        currentElement.removeClass('error');
                    }
                });
                //loop through checkbox arrays
                $('.question-type-2').each(function (k, v) {
                    if ($(v).find(cboxElement + cboxCheckedSelector).length > 0) {
                        $('.question-type-2').eq(k).find(cboxElement).removeClass('error');
                        $('#footer .error-msg').fadeOut();
                    } else {
                        errorMessage = 'Oops, be sure to select at least one.';
                        $('.question-type-2').eq(k).find(cboxElement).addClass('error');
                        $('#footer .error-msg').fadeIn();
                    }
                });


                //check question type 2 for visible value fields
                $('.details input[type=text]:visible').each(function (k, v) {

                    cleanVal = curriculum.global.utils.cleanInput.init($(this).val());
                    if (curriculum.global.utils.isEmpty(cleanVal) || cleanVal == 0) {
                        errorMessage = 'Oops, be sure to fill in all the fields';
                        $(this).addClass('error');
                    } else {
                        $(this).removeClass('error');
                    }
                });


                //once its all done
                if ($('.error').length == 0) {
                    pagePass = true;
                    $('#footer .error-msg').fadeOut();
                    curriculum.global.utils.paginate.next();
                } else {
                    $('#footer .error-msg').text(errorMessage).fadeIn();

                }
            }




        } // END errors
    } // END: utils

};                                             // END expenses global var


$('document').ready(function(){
    // activate utils
    transportation.global.utils.init();
});