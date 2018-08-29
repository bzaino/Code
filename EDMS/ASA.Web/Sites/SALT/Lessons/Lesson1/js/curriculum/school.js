var school = school || {};
var boxesArray = [];
var savedObject = {};
var totalSchool = 0;
var chosenForm;
var expensesFullyLoaded = false;

save = school;
school.global = {
  step: 7,
  utils: {
    init: function () {
      curriculum.global.viewport.animateViewport.normal();
      //include stylesheet & append class for this page
      $('#content-container .content').attr('id', 'curriculum-school');
      //update page context
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "total spent for school", "Expenses");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $("#content-container .content").show().css({ opacity: 0 });

      $('#total .value').text("$0");

      // activate global plugins
      school.global.utils.handleTips.init();
      school.global.utils.preloadData.init();
      school.global.utils.plugins.init();

    },
    plugins: {
      // init all plugins
      init: function () {


        $('#curriculum-school .dropdown').dropkick({
          change: function () {
            school.global.utils.updateSchool.init();
          }
        });
        $('#curriculum-school input[type=checkbox]').styledInputs();
      }
    }, // END: plugins
    handleTips: {
      init: function () {
        //togle tip on and off on click
        $('#curriculum-school .question .box-wrapper input[type=checkbox]').bind('propertychange change', function () {
          //keep checking which boxes are selected when the boxes are added or removed
          school.global.utils.gatherAllBoxes.init();

          if ($(this).is(':checked')) {
            //bold the label
            $(this).siblings('label').addClass('bold');
            //show content
            $(this).siblings('.tip').fadeIn('normal', function () {
              $('.details', $(this)).fadeIn('fast');
              //set focus
              $(this).find('.details input').focus();
            });

          } else {
            $(this).siblings('label').removeClass('bold');
            //hide content
            $(this).siblings('.tip').find('.details input').removeClass('error');
            $(this).siblings('.tip').fadeOut('normal', function () {
              $('.details', $(this)).fadeOut('fast');
            });
          }

          curriculum.global.viewport.getNewViewport();
        });

        //manipulate user input text
        $('#curriculum-school .question .box-wrapper input[type=text]').bind('keyup change blur', function (e) {
          if ($(this).hasClass('error')) {
            cleanVal = curriculum.global.utils.cleanInput.init($(this).val());
            if (curriculum.global.utils.isEmpty(cleanVal) || cleanVal == 0) {
              $(this).addClass('error');
            } else {
              $(this).removeClass('error');
            }
          }
          $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
          school.global.utils.updateSchool.init();

          //highlight, only on keyup
          if (e.type == 'keyup' || e.type == 'blur') {
            $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
          }
        });



      }
    }, // END handleTips

    gatherAllBoxes: {
      init: function () {
        boxesArray = [];
        //compose the array from scratch every time this is run
        $('#curriculum-school input[type=checkbox]').each(function () {
          if ($(this).is(':checked')) {
            boxesArray.push($(this));
          }
        });

        if (boxesArray.length > 0) {
          $(cboxElement).removeClass('error');
          $('#footer .error-msg').fadeOut();
        } else {
          pagePass = false;
        }

        //update the school
        school.global.utils.updateSchool.init();
      }
    }, //END gatherAllBoxes

    updateSchool: {
      init: function () {
        /**
        loop through the checked boxes
        sanitize the data and replace the values in DOM	
        */

        totalSchool = 0;
        savedObject = {};

        for (i = 0; i < boxesArray.length; i++) {
          //format the input side
          value = boxesArray[i].siblings('.tip').find('input').val();
          clean = curriculum.global.utils.cleanInput.init(value);

          //get the rate at which the school is gathered
          rate = boxesArray[i].siblings('.tip').find('select').val();
          timed = curriculum.global.utils.determineRate.init(clean, rate);

          displayName = boxesArray[i].attr('data-display');
          //add each one up
          totalSchool += timed;

          //set the dom elements to show it
          $('#total .value').text(totalSchool).toNumber().formatCurrency({ roundToDecimalPlace: 0 });


          //animate the graph
          userData.expenseList.school = totalSchool;

          //update the object that we're keeping
          theName = boxesArray[i].attr('id');
          savedObject[theName] = $.extend(savedObject[theName], { displayName: displayName, value: clean, time: rate, newValue: null });

        }

        // prevent modifying expenses until data is fully loaded on form
        if (expensesFullyLoaded) {
          var expenses = userData.expenses.school.cost;
          for (var key in expenses) {
            if (expenses.hasOwnProperty(key) && curriculum.global.utils.server.isExpense(expenses[key])) {
              if (savedObject[key] != undefined) {
                // update any existing expenses with changes
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
          userData.expenses.school.cost = expenses;
          curriculum.global.utils.animateGraph.refresh();
        }
      } //end init
    }, // END udateSchool

    saveData: function () {
      userData.expenses.school.displayName = "School";
      curriculum.global.utils.server.saveToServer(userData);
    }, //END saveData
    preloadData: {
      init: function () {
        var selectedSchool;
        if (userData.expenses.school != undefined) {

          selectedSchool = userData.expenses.school.selected;

          $('#curriculum-school .dropdown-large.school').val(selectedSchool);
          $('#curriculum-school .dropdown-large.school').change();


          p = userData.expenses.school.cost;

          for (var key in p) {
            if (p.hasOwnProperty(key)) {

              checkbox = $('#curriculum-school #' + key);
              value = $(checkbox).siblings('.tip').find('input').val(p[key].value);
              time = $(checkbox).siblings('.tip').find('select');
              checkbox.trigger('click');
              time.val(p[key].time);
              time.trigger('change');
            }
          }

          // indicates that all expenses have been loaded into the form, so we avoid trying to re-calculate as each value is populated
          expensesFullyLoaded = true;

          // step data was preloaded; set tracking flag to true.
          curriculum.global.tracking.preloaded = true;

          $('#curriculum-school .dropdown-large').siblings('.dd-large').find('ul li[data-name=select-one]').remove();
          //recalculate everything
          $('#curriculum-school .question .box-wrapper input[type=text]').change();
          $('input[type=text]:first').focus();

        } else {
          userData.expenses.school = {};
          userData.expenses.school.cost = {};
          expensesFullyLoaded = true;

          // step data was not preloaded; set tracking flag to false.
          curriculum.global.tracking.preloaded = false;
        }

        $("#content-container .content").delay(360).animate({ opacity: 1 }, 1000);
      }
    }, // END preloadData
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

      if ($('select.school').val() == 'select-one') {
        $('select.school').addClass('error');

      }
      //if there are selected boxes
      if (boxesArray.length > 0) {
        $.each(boxesArray, function (k, v) {
          element = v.siblings('.tip').find('.details input');
          cleanVal = curriculum.global.utils.cleanInput.init(v.siblings('.tip').find('.details input').val());
          if (curriculum.global.utils.isEmpty(cleanVal) || cleanVal == 0) {
            element.addClass('error');
          } else {
            element.removeClass('error');
          }
        });
      } else {
        pagePass = false;
        $(cboxElement).addClass('error');
        $('#footer .error-msg').fadeIn();
      }

      //once all validation is done, check to see if pagePass is true
      if ($('.error').length == 0) {
        pagePass = true;
        $('#footer .error-msg').fadeOut();
        curriculum.global.utils.paginate.next();
      } else {
        $('#footer .error-msg').fadeIn();

      }
    }
  } //END utils

};    // END school global var


$('document').ready(function(){
	// activate utils
	school.global.utils.init();
});
