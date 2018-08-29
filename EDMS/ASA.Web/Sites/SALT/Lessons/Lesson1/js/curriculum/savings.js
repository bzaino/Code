var savings = savings || {};
var errorMessage = 'Oops, be sure to fill in all the fields';

save = savings;

savings.global = {
  step: 9,
  utils: {
    init: function(){
      curriculum.global.viewport.animateViewport.normal();
      // Append ID and class for this page
      $('#content-container .content').attr('id','curriculum-savings').addClass('simple');
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on savings", "Expenses", "Keep Going");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $("#content-container .content").show().css({opacity: 0});
      $('#total .value').text('$0');
      
      //bind events for value input
      $('input#savings-amount').bind('keyup focus blur', function(e){
        if($(this).hasClass('error')){
          if(!curriculum.global.utils.isEmpty($('input#savings-amount').val())){
            $(this).removeClass('error');
            $('#footer .error-msg').fadeOut();
          }
        }
        $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
          savings.global.utils.updateSavings.init();
        // Highlight, only on keyup
        if(e.type === 'keyup' || e.type === 'blur'){
          $('#total .value').stop(true,true).effect("highlight", {color: '#faff00'}, 1500);
        }
      });
      
      
      

      
      
      savings.global.utils.preloadData();
      
      $('#curriculum-savings .dropdown-large.time').dropkick({
        theme: 'large',
        change: function(){
          savings.global.utils.updateSavings.init();
        }
      });
            
    },
    updateSavings: {
      init: function(){
        
        // format the input side
        value = $('input#savings-amount').val();
        clean = curriculum.global.utils.cleanInput.init(value);

        // get the rate at which the savings are calculated
        rate = $('#curriculum-savings select').val();
        timed = curriculum.global.utils.determineRate.init(clean, rate);

        $('#total .value').text(timed).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        
        userData.expenses.savings = $.extend(userData.expenses.savings,{value: clean, time: rate, newValue: null});
        
        userData.expenseList.savings = timed;
        curriculum.global.utils.animateGraph.refresh();
      }
    }, // END updateSavings
    saveData: function(){
      userData.expenses.savings.displayName = "Savings";
      curriculum.global.utils.server.saveToServer(userData);

    }, //END saveData
    preloadData: function() {
      if(userData.expenses.savings != undefined){
        // step data was preloaded; set tracking flag to true.
        curriculum.global.tracking.preloaded = true;

        //value
        $('#savings-amount').val(userData.expenses.savings.value).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        //time
        $('.dropdown-large').val(userData.expenses.savings.time);
        $('.dropdown-large').trigger('change');
      } else {
        // step data was not preloaded; set tracking flag to false.
        curriculum.global.tracking.preloaded = false;
      }
      
      $("#content-container .content").delay(360).animate({opacity: 1}, 1000);
      savings.global.utils.updateSavings.init();

    }, // END preloadData
    errors: function(){
      $('#footer .error-msg').text(errorMessage).fadeOut();

      // savings value
      cleanGoal = curriculum.global.utils.cleanInput.init($('#savings-amount').val());
      if(curriculum.global.utils.isEmpty(cleanGoal) || cleanGoal == 0){
        $('#savings-amount').addClass('error');
      } else {
        $('#savings-amount').removeClass('error');
      }

      //once all validation is done, check to see if pagePass is true
      if($('.error').length == 0){
        pagePass = true;
        $('#footer .error-msg').fadeOut();
        curriculum.global.utils.paginate.next();
      } else{
        $('#footer .error-msg').fadeIn();
      }
      
    }
  } // END: utils

}; // END expenses global var


$('document').ready(function(){
  // activate utils
  savings.global.utils.init();
});
