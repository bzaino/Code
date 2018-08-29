var entertainment = entertainment || {};

save = entertainment;

entertainment.global = {
  step: 11,
  utils: {
    init: function () {
      curriculum.global.viewport.animateViewport.normal();
      // Append ID and class for this page
      $('#content-container .content').attr('id', 'curriculum-entertainment').addClass('simple');
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on entertainment", "Expenses", "Keep Going");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $("#content-container .content").show().css({ opacity: 0 });
      $('#total .value').text('$0');

      //bind events for value input
      $('input#entertainment-amount').bind('keyup change blur', function (e) {
        if ($(this).hasClass('error')) {
          if (!curriculum.global.utils.isEmpty($('#entertainment-amount').val())) {
            $(this).removeClass('error');
            $('#footer .error-msg').fadeOut();
          }
        }
        $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        entertainment.global.utils.updateEntertainment.init();
        // Highlight, only on keyup
        if (e.type === 'keyup' || e.type === 'blur') {
          $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
        }
      });



      entertainment.global.utils.preloadData();

      $('#curriculum-entertainment .dropdown-large.time').dropkick({
        theme: 'large',
        change: function () {
          entertainment.global.utils.updateEntertainment.init();
        }
      });

    },
    updateEntertainment: {
      init: function () {

        // format the input side
        value = $('input#entertainment-amount').val();
        clean = curriculum.global.utils.cleanInput.init(value);

        // get the rate at which the entertainment are calculated
        rate = $('#curriculum-entertainment select').val();
        timed = curriculum.global.utils.determineRate.init(clean, rate);

        $('#total .value').text(timed).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

        userData.expenses.entertainment = $.extend(userData.expenses.entertainment, { value: clean, time: rate, newValue: null });

        userData.expenseList.entertainment = timed;
        curriculum.global.utils.animateGraph.refresh();
      }
    }, // END updateEntertainment
    saveData: function () {
      userData.expenses.entertainment.displayName = "Entertainment";
      curriculum.global.utils.server.saveToServer(userData);

    }, //END saveData
    preloadData: function () {
      if (userData.expenses.entertainment != undefined) {
        // step data was preloaded; set tracking flag to true.
        curriculum.global.tracking.preloaded = true;

        //value
        $('#entertainment-amount').val(userData.expenses.entertainment.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        //time
        $('.dropdown-large').val(userData.expenses.entertainment.time);
        $('.dropdown-large').trigger('change');
      } else {
        // step data was not preloaded; set tracking flag to false.
        curriculum.global.tracking.preloaded = false;
      }

      $("#content-container .content").delay(360).animate({ opacity: 1 }, 1000);
      entertainment.global.utils.updateEntertainment.init();

    }, // END preloadData
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

      // savings value
      cleanGoal = curriculum.global.utils.cleanInput.init($('#entertainment-amount').val());
      if (curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal == 0) {
        $('#entertainment-amount').addClass('error');
      } else {
        $('#entertainment-amount').removeClass('error');
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
  entertainment.global.utils.init();
});
