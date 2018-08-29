var groceries = groceries || {};

save = groceries;

groceries.global = {
  step: 10,
  utils: {
    init: function () {
      curriculum.global.viewport.animateViewport.normal();
      // Append ID and class for this page
      $('#content-container .content').attr('id', 'curriculum-groceries').addClass('simple');
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on groceries", "Expenses", "Keep Going");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $("#content-container .content").show().css({ opacity: 0 });
      $('#total .value').text('$0');

      //bind events for value input
      $('input#groceries-amount').bind('keyup change blur', function (e) {
        if ($(this).hasClass('error')) {
          if (!curriculum.global.utils.isEmpty($('#groceries-amount').val())) {
            $(this).removeClass('error');
            $('#footer .error-msg').fadeOut();
          }
        }
        $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        groceries.global.utils.updateGroceries.init();
        // Highlight, only on keyup
        if (e.type === 'keyup' || e.type === 'blur') {
          $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
        }
      });





      groceries.global.utils.preloadData();
      $('#curriculum-groceries .dropdown-large.time').dropkick({
        theme: 'large',
        change: function () {
          groceries.global.utils.updateGroceries.init();
        }
      });

    },
    updateGroceries: {
      init: function () {

        // format the input side
        value = $('input#groceries-amount').val();
        clean = curriculum.global.utils.cleanInput.init(value);

        // get the rate at which the groceries are calculated
        rate = $('#curriculum-groceries select').val();
        timed = curriculum.global.utils.determineRate.init(clean, rate);

        $('#total .value').text(timed).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

        userData.expenses.groceries = $.extend(userData.expenses.groceries, { value: clean, time: rate, newValue: null });

        userData.expenseList.groceries = timed;
        curriculum.global.utils.animateGraph.refresh();
      }
    }, // END updateGroceries
    saveData: function () {
      userData.expenses.groceries.displayName = "Groceries";
      curriculum.global.utils.server.saveToServer(userData);
    }, //END saveData
    preloadData: function () {
      if (userData.expenses.groceries != undefined) {
        // step data was preloaded; set tracking flag to true.
        curriculum.global.tracking.preloaded = true;

        //value
        $('#groceries-amount').val(userData.expenses.groceries.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        //time
        $('.dropdown-large').val(userData.expenses.groceries.time);
        $('.dropdown-large').trigger('change');
      } else {
        // step data was not preloaded; set tracking flag to false.
        curriculum.global.tracking.preloaded = false;
      }

      $("#content-container .content").delay(360).animate({ opacity: 1 }, 1000);
      groceries.global.utils.updateGroceries.init();
    }, // END preloadData
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

      // savings value
      cleanGoal = curriculum.global.utils.cleanInput.init($('#groceries-amount').val());
      if (curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal == 0) {
        $('#groceries-amount').addClass('error');
      } else {
        $('#groceries-amount').removeClass('error');
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

  } // END: utils

};  // END expenses global var

$('document').ready(function(){
  // activate utils
  groceries.global.utils.init();
});
