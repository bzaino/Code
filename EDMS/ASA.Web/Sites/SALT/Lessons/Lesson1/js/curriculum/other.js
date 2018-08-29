var other = other || {};
var expensesArray = [];
var otherCount = 0;
var errorMessage;
save = other;
other.global = {
    step: 14,
    utils: {
        init: function () {


            LazyLoad.css('css/curriculum/other.css', function () {
                curriculum.global.viewport.animateViewport.normal();
            });

            // Append ID and class for this page
            $('#content-container .content').attr('id', 'curriculum-other').removeClass('simple');

            // Update page context
            $('#total .value').text("$0");
            curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on other", "Summary", "Keep Going");

            //trigger webtrends
            curriculum.global.tracking.trigger("lesson:step:start", {
                step: {
                    number: save.global.step
                }
            });

            // Manipulate user input text
            $('#curriculum-other input[type=text]').die();
            $('#curriculum-other input[type=text]').live('keyup change blur', function (e) {

                // Highlight, only on keyup
                if (e.type === 'keyup' || e.type === 'blur') {
                    $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
                }

                if ($(this).hasClass('value')) {
                    $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
                }
                if ($(this).hasClass('other-description')) {
                    var cleanedValue = curriculum.global.utils.cleanInput.freeFormNoPerctg($(this).val());
                    $(this).val(cleanedValue);
                    if (cleanedValue && $(this).hasClass('error')) {
                        $(this).removeClass('error');
                        $('#footer .error-msg').fadeOut();
                    }
                }
                other.global.utils.updateOther.init();

            });



            // Add another expense button
            $('a.add-another').click(function (e) {

                e.preventDefault();

                otherCount++;

                $.ajax('includes/add-another.html', {
                    cache: false,
                    success: function (response) {

                        var html = response;

                        $('#curriculum-other .question').append(html);

                        current = $('#curriculum-other .question .other-expense');
                        currentIndex = $('#curriculum-other .question .other-expense').index();

                        //rename the name attribute for each new <select> element
                        //unique names are required by the dropkick() plugin
                        current.eq(currentIndex).find('select').attr('name', 'time' + currentIndex);


                        // Update dropdown
                        current.eq(currentIndex).find('select').removeData('dropkick').addClass('thisGuy');
                        current.eq(currentIndex).find('.dk_container').remove();
                        current.eq(currentIndex).find('select').dropkick({
                            theme: 'large',
                            change: function () {
                                other.global.utils.updateOther.init();
                            }
                        });

                        current.eq(currentIndex).attr('rel', 'other_' + otherCount);
                        expensesArray.push(current.eq(currentIndex).attr('rel'));
                    }
                });

            });

            $('#curriculum-other .btn-remove').die();
            $('#curriculum-other .btn-remove').live('click', function (e) {
                e.preventDefault();

                rel = $(this).parents('.other-expense');
                rel.fadeOut(function () {
                    $(this).remove();
                });

                other.global.utils.removeOtherExpense(rel.attr('rel'));
            });


            other.global.utils.preloadData();
            // Update dropdown
            //$('#curriculum-other .dropdown-element').styledDropDowns({ddClass: 'dd-large', autoAdjust: true});


            //comment them out because it's been handled by the other function at line 42
            // var rgx = /|[!@#$^&*()=+;{}\:',."<>|?~_-]/;
            // $('.other-description').die('keyup');
            // $('.other-description').live('keyup', function () {


            //   if (rgx.test($(this).val())) {
            //     $(this).addClass('error');
            //     errorMessage = 'Oops, be sure to use only letters when describing an expense.';
            //   } else {
            //     $('#footer .error-msg').fadeOut();
            //     $(this).removeClass('error');
            //   }

            //   //$(this).val($(this).val().replace(/\d|[!@#$%^&*()=+;{}\:'"<>|?]/, ''));

            // });


        }, // END init
        removeOtherExpense: function (what) {

            expensesArray = $.grep(expensesArray, function (v) {
                return v != what;
            });
            other.global.utils.updateOther.init();

        },
        updateOther: {
            init: function () {
                /**
                loop through the added expenses
                sanitize the data and replace the values in DOM
                */

                var totalExpenses = 0;
                savedObject = {};

                for (i = 0; i < expensesArray.length; i++) {
                    var thisIteration = $('.other-expense[rel="' + expensesArray[i] + '"]');

                    // format the input side
                    var value = thisIteration.find('input.value').val();
                    var clean = curriculum.global.utils.cleanInput.init(value);

                    var description = $('input.other-description', thisIteration).val();
                    description = description.replace(/ /g, "-");

                    var displayName = thisIteration.find('input.other-description').val();

                    //get the rate at which the income is gathered
                    var rate = thisIteration.find('select').val();
                    var timed = curriculum.global.utils.determineRate.init(clean, rate);




                    if (!curriculum.global.utils.isEmpty(value) && curriculum.global.utils.isEmpty(displayName)) {
                        thisIteration.find('input.other-description').addClass('error');
                        errorMessage = 'Oops. Please enter a name for your expense';
                    } else {

                        // add each one up
                        totalExpenses += timed;


                        var currentOther = expensesArray[i];


                        if (clean != 0) {
                            savedObject[description] = { displayName: displayName, name: description, value: clean, time: rate, newValue: null };
                        }

                        $('#total .value').text(totalExpenses).formatCurrency({ roundToDecimalPlace: 0 });
                        userData.expenseList.other = totalExpenses;
                        curriculum.global.utils.animateGraph.refresh();

                    }


                }
            } // END updateOther.init()
        }, // END updateOther

        saveData: function () {
            if (!$.isEmptyObject(savedObject)) {
                var other = userData.expenses.other || {};
                other.displayName = 'Other';

                for (var key in other) {
                    if (other.hasOwnProperty(key) && curriculum.global.utils.server.isExpense(other[key])) {
                        if (savedObject[key] != undefined) {
                            // update any existing incomes with changes
                            other[key] = $.extend(other[key], savedObject[key]);
                            delete savedObject[key];
                        } else {
                            // delete any removed incomes
                            delete other[key];
                        }
                    }
                }

                for (var key in savedObject) {
                    if (savedObject.hasOwnProperty(key)) {
                        // add any new incomes
                        other[key] = $.extend(other[key], savedObject[key]);
                    }
                }

                userData.expenses.other = other;
            } else {
                delete userData.expenses.other;
            }

            curriculum.global.utils.server.saveToServer(userData);
        }, //END saveData
        preloadData: function () {
            isEmpty = $.isEmptyObject(userData.expenses.other);
            savedObject = {};

            if (userData.expenses.other != undefined && isEmpty == false) {
                $.each(userData.expenses.other, function (k, v) {
                    if (curriculum.global.utils.server.isExpense(v)) {

                        v.name = v.Name; //SERVER CHANGE: assign name property for other expenses

                        $.ajax('includes/add-another.html', {
                            cache: false,
                            success: function (response) {
                                var html = response;
                                $('#curriculum-other .question').append(html);

                                current = $('#curriculum-other .question .other-expense');
                                currentIndex = $('#curriculum-other .question .other-expense').index();

                                //value
                                theName = v.name.replace(/-/g, " ");
                                current.eq(currentIndex).find('input.other-description').val(theName);
                                current.eq(currentIndex).find('input.value').val(v.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
                                //time

                                current.eq(currentIndex).find('select.time').val(v.time);
                                // Update dropdown
                                current.eq(currentIndex).find('select').removeData('dropkick').attr('name', 'time_' + otherCount);
                                current.eq(currentIndex).find('.dk_container').remove();
                                current.eq(currentIndex).find('select').dropkick({
                                    theme: 'large',
                                    change: function () {
                                        other.global.utils.updateOther.init();
                                    }
                                });



                                //current.eq(currentIndex).find('select.time option[value=' + v.time + ']').attr('selected','selected');

                                current.eq(currentIndex).attr('rel', v.name);
                                expensesArray.push(current.eq(currentIndex).attr('rel'));

                                curriculum.global.utils.animateGraph.refresh();
                                other.global.utils.updateOther.init();


                                otherCount++;
                            }
                        });

                    } //end if nested loop
                }); //END Each loop

                // step data was preloaded; set tracking flag to true.
                curriculum.global.tracking.preloaded = true;
            } else {
                userData.expenses.other = {};

                // step data was not preloaded; set tracking flag to false.
                curriculum.global.tracking.preloaded = false;

                //trigger the first click to load the first 'other' expense
                $('a.add-another').trigger('click');
            }


        }, // END preloadData
        errors: function () {
            if ($('.error').length == 0) {
                pagePass = true;
                $('#footer .error-msg').fadeOut();
                curriculum.global.utils.paginate.next();
            } else {
                $('#footer .error-msg').text(errorMessage).fadeOut();
                $('#footer .error-msg').fadeIn();
            }

        }

    } // END: utils

};            // END expenses global var


$('document').ready(function(){
  // activate utils
  other.global.utils.init();
});
