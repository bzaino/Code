var technology = technology || {};

save = technology;

technology.global = {
  step: 13,
  utils: {
    init: function () {
      curriculum.global.viewport.animateViewport.normal();
      // Append ID and class for this page
      $('#content-container .content').attr('id', 'curriculum-technology').addClass('simple');
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on technology", "Expenses", "Keep Going");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $("#content-container .content").show().css({ opacity: 0 });
      $('#total .value').text('$0');

      //bind events for value input
      $('input#technology-amount').bind('keyup change blur', function (e) {
        if ($(this).hasClass('error')) {
          if (!curriculum.global.utils.isEmpty($('#technology-amount').val())) {
            $(this).removeClass('error');
            $('#footer .error-msg').fadeOut();
          }
        }
        $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        technology.global.utils.updateTechnology.init();
        // Highlight, only on keyup
        if (e.type === 'keyup' || e.type === 'blur') {
          $('#total .value').stop(true, true).effect("highlight", { color: '#faff00' }, 1500);
        }
      })



      technology.global.utils.preloadData();

      $('#curriculum-technology .dropdown-large.time').dropkick({
        theme: 'large',
        change: function () {
          technology.global.utils.updateTechnology.init();
        }
      });



    },
    updateTechnology: {
      init: function () {

        // format the input side
        value = $('input#technology-amount').val();
        clean = curriculum.global.utils.cleanInput.init(value);

        // get the rate at which the technology are calculated
        rate = $('#curriculum-technology select').val();
        timed = curriculum.global.utils.determineRate.init(clean, rate);

        $('#total .value').text(timed).toNumber().formatCurrency({ roundToDecimalPlace: 0 });

        userData.expenses.technology = $.extend(userData.expenses.technology, { value: clean, time: rate, newValue: null });

        userData.expenseList.technology = timed;
        curriculum.global.utils.animateGraph.refresh();
      }
    }, // END updateTechnology
    saveData: function () {
      userData.expenses.technology.displayName = "Phone & Technology";
      curriculum.global.utils.server.saveToServer(userData);
    }, //END saveData
    preloadData: function () {
      if (userData.expenses.technology != undefined) {
        // step data was preloaded; set tracking flag to true.
        curriculum.global.tracking.preloaded = true;

        //value
        $('#technology-amount').val(userData.expenses.technology.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        //time
        $('.dropdown-large').val(userData.expenses.technology.time);
        $('.dropdown-large').trigger('change');
      }

      $("#content-container .content").delay(360).animate({ opacity: 1 }, 1000);
      technology.global.utils.updateTechnology.init();

    }, // END preloadData
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

      // savings value
      cleanGoal = curriculum.global.utils.cleanInput.init($('#technology-amount').val());
      if (curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal == 0) {
        $('#technology-amount').addClass('error');
      } else {
        $('#technology-amount').removeClass('error');
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
  technology.global.utils.init();
});
