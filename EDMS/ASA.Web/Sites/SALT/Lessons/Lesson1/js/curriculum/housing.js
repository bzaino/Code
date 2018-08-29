var housing = housing || {};
var boxesArray = [];
var savedObject = {};
var totalHousing = 0;
var chosenForm;
var errorMessage = 'Oops, be sure to select at least one.';
var expensesFullyLoaded = false;

save = housing;
housing.global = {
  step: 5,
  utils: {
    init: function () {
      curriculum.global.viewport.animateViewport.normal();
      //include stylesheet & append class for this page
      $('#content-container .content').attr('id', 'curriculum-housing');
      //update page context
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on housing", "Expenses");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $("#content-container .content").show().css({ opacity: 0 });

      $('#total .value').text("$0");

      //hide the two forms on load
      $('#curriculum-housing form.own').hide();
      $('#curriculum-housing form.rent').hide();

      housing.global.utils.handleTips.init();
      housing.global.utils.preloadData.init();



      //housing.global.utils.handleHousingChange();




    },

    plugins: {
      // init all plugins
      init: function () {
        $('#curriculum-housing .dropdown-large.housing').dropkick({
          theme: 'large',
          change: function () {
            housing.global.utils.handleHousingChange(this.value, $(this.data.select));
          }
        });
        $('#curriculum-housing .dropdown').dropkick({
          change: function () {
            housing.global.utils.updateHousing.init();
          }
        });
        $('#curriculum-housing input[type=checkbox]').styledInputs();
      }
    }, // END: plugins
    handleHousingChange: function (value, element) {
      //$('.dropdown-large.housing').change(function(){

      $(element).removeClass('error');
      $(cboxElement).removeClass('error');


      if (value == 'own') {
        $('#curriculum-housing form.own').fadeIn();
        $('#curriculum-housing form.rent').fadeOut();
        chosenForm = 'own';
      } else if (value == 'rent') {
        $('#curriculum-housing form.rent').fadeIn();
        $('#curriculum-housing form.own').fadeOut();
        chosenForm = 'rent';
      }


      userData.expenses.housing.selected = chosenForm;
      $('#curriculum-housing .dd-large').find('li[data-name=select-one]').remove();

      housing.global.utils.updateHousing.init();
      curriculum.global.viewport.getNewViewport();
      // });

    }, // END handleHousingChange()
    handleTips: {
      init: function () {
        //togle tip on and off on click
        $('#curriculum-housing .question .box-wrapper input[type=checkbox]').bind('propertychange change', function () {

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
          housing.global.utils.updateHousing.init();
        });

        //manipulate user input text
        $('#curriculum-housing .question .box-wrapper input[type=text]').bind('keyup change blur', function (e) {
          if ($(this).hasClass('error')) {
            cleanVal = curriculum.global.utils.cleanInput.init($(this).val());
            if (curriculum.global.utils.isEmpty(cleanVal) || cleanVal == 0) {
              $(this).addClass('error');
            } else {
              $(this).removeClass('error');
            }
          }
          $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
          housing.global.utils.updateHousing.init();

          //highlight, only on keyup
          if (e.type == 'keyup' || e.type == 'blur') {
            $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
          }

        });

        $('#curriculum-housing select').die();
        $('#curriculum-housing select').live('change', function () {
          housing.global.utils.updateHousing.init();
        });

      }
    }, // END handleTips

    gatherAllBoxes: {
      init: function () {
        boxesArray = [];
        //compose the array from scratch every time this is run
        $('#curriculum-housing form.' + chosenForm + ' input[type=checkbox]').each(function () {
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

      }
    }, //END gatherAllBoxes

    updateHousing: {
      init: function () {
        housing.global.utils.gatherAllBoxes.init();
        /**
        loop through the checked boxes
        sanitize the data and replace the values in DOM	
        */

        totalHousing = 0;
        savedObject = {};

        for (i = 0; i < boxesArray.length; i++) {
          //format the input side
          value = boxesArray[i].siblings('.tip').find('input').val();
          clean = curriculum.global.utils.cleanInput.init(value);

          //get the rate at which the housing is gathered
          rate = boxesArray[i].siblings('.tip').find('select').val();
          timed = curriculum.global.utils.determineRate.init(clean, rate);

          //add each one up
          totalHousing += timed;


          //set the dom elements to show it
          $('#total .value').text(totalHousing).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

          //update the object that we're keeping
          theName = boxesArray[i].attr('id');
          displayName = boxesArray[i].attr('data-display');

          savedObject[theName] = $.extend(savedObject[theName], { displayName: displayName, value: clean, time: rate, newValue: null });

          userData.expenseList.housing = totalHousing;  
        }

        // prevent modifying expenses until data is fully loaded on form
        if (expensesFullyLoaded) {
          var expenses = userData.expenses.housing.cost;
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
          userData.expenses.housing.cost = expenses;
          curriculum.global.utils.animateGraph.refresh();
        }
      } //end init
    }, // END udateHousing

    saveData: function () {
      userData.expenses.housing.displayName = "Housing";
      curriculum.global.utils.server.saveToServer(userData);
    }, //END saveData
    preloadData: {
      init: function () {
        var selectedHousing;
        if (userData.expenses.housing != undefined) {
          selectedHousing = userData.expenses.housing.selected;

          chosenForm = selectedHousing;
          $('#curriculum-housing .dropdown-large.housing').val(selectedHousing);

          housing.global.utils.handleHousingChange(selectedHousing, $('#curriculum-housing .dropdown-large.housing'));


          p = userData.expenses.housing.cost;
          for (var key in p) {
            if (p.hasOwnProperty(key)) {
              checkbox = $('#curriculum-housing form.' + selectedHousing + ' #' + key);
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

          //trigger a change so that the styled dropdowns get an update
          housing.global.utils.updateHousing.init();

          //recalculate everything
          $('.question .box-wrapper input[type=text]').change();
          $('input[type=text]:first').focus();
          // activate global plugins
          housing.global.utils.plugins.init();
        } else {
          userData.expenses.housing = {};
          userData.expenses.housing.cost = {};
          expensesFullyLoaded = true;

          // step data was not preloaded; set tracking flag to false.
          curriculum.global.tracking.preloaded = false;

          // activate global plugins
          housing.global.utils.plugins.init();
          $('#curriculum-housing form').hide();

        }

        $("#content-container .content").delay(360).animate({ opacity: 1 }, 1000);
      }
    }, // END preloadData
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

      if ($('select.housing').val() == 'select-one') {
        $('select.housing').addClass('error');
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

};       // END housing global var


$('document').ready(function(){
	// activate utils
	housing.global.utils.init();
	
});
