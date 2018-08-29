var eating = eating || {};

save = eating;

eating.global = {
  step: 12,
  utils: {
    init: function () {
      curriculum.global.viewport.animateViewport.normal();
      // Append ID and class for this page
      $('#content-container .content').attr('id', 'curriculum-eating').addClass('simple');
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on eating", "Expenses", "Keep Going");

      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $("#content-container .content").show().css({ opacity: 0 });
      $('#total .value').text('$0');

      //bind events for value input
      $('input#eating-amount').bind('keyup change blur', function (e) {
        if ($(this).hasClass('error')) {
          if (!curriculum.global.utils.isEmpty($('#eating-amount').val())) {
            $(this).removeClass('error');
            $('#footer .error-msg').fadeOut();
          }
        }
        $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        eating.global.utils.updateEatingOut.init();
        // Highlight, only on keyup
        if (e.type === 'keyup' || e.type === 'blur') {
          $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
        }
      });


      eating.global.utils.preloadData();

      $('#curriculum-eating .dropdown-large.time').dropkick({
        theme: 'large',
        change: function () {
          eating.global.utils.updateEatingOut.init();
        }
      });

    },
    updateEatingOut: {
      init: function () {

        // format the input side
        value = $('input#eating-amount').val();
        clean = curriculum.global.utils.cleanInput.init(value);

        // get the rate at which the eating are calculated
        rate = $('#curriculum-eating select').val();
        timed = curriculum.global.utils.determineRate.init(clean, rate);

        $('#total .value').text(timed).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

        userData.expenses.eating = $.extend(userData.expenses.eating, { value: clean, time: rate, newValue: null });

        userData.expenseList.eating = timed;
        curriculum.global.utils.animateGraph.refresh();
      }
    }, // END updateEatingOut
    saveData: function () {
      userData.expenses.eating.displayName = "Eating";
      curriculum.global.utils.server.saveToServer(userData);
    }, //END saveData
    preloadData: function () {
      if (userData.expenses.eating != undefined) {
        // step data was preloaded; set tracking flag to true.
        curriculum.global.tracking.preloaded = true;

        //value
        $('#eating-amount').val(userData.expenses.eating.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        //time
        $('.dropdown-large').val(userData.expenses.eating.time);
        $('.dropdown-large').trigger('change');
      } else {
        // step data was not preloaded; set tracking flag to false.
        curriculum.global.tracking.preloaded = false;
      }

      $("#content-container .content").delay(360).animate({ opacity: 1 }, 1000);
      eating.global.utils.updateEatingOut.init();

    }, // END preloadData
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

      // savings value
      cleanGoal = curriculum.global.utils.cleanInput.init($('#eating-amount').val());
      if (curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal == 0) {
        $('#eating-amount').addClass('error');
      } else {
        $('#eating-amount').removeClass('error');
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
  eating.global.utils.init();
});
