var income = income || {};
var boxesArray = [];
var errorMessage = 'Oops, be sure to select at least one.';
var incomesFullyLoaded = false;

save = income;
income.global = {
  step: 2,
  utils: {
    init: function () {
      //include stylesheet & append class for this page
      curriculum.global.viewport.animateViewport.normal();
      $('head').append('<link rel="stylesheet" href="css/curriculum/income.css" type="text/css" />');
      $('#content-container .content').attr('id', 'curriculum-income');
      //update page context
      curriculum.global.utils.paginate.updateContext("2", "Your Income", "total monthly income", "Expenses");
      
      //trigger webtrends
      curriculum.global.tracking.trigger("lesson:step:start", {
        step: {
          number: save.global.step
        }
      });

      $('#total .value').text("$0");

      income.global.utils.handleTips.init();
      income.global.utils.preloadData.init();

      var path = 'income.html';
      for (i = 0; i < pages.length; i++) {
        if (pages[i].path === path) {
          curriculum.global.utils.paginate.currentPagePosition = i;
          break;
        }
      }

      // activate global plugins
      income.global.utils.plugins.init();
      curriculum.global.utils.animateGraph.refresh();
    },

    plugins: {
      // init all plugins
      init: function () {

        $('#curriculum-income .dropdown').dropkick({
          change: function () {
            income.global.utils.updateIncome.init();
          }
        });
        $('#curriculum-income input[type=checkbox]').styledInputs();
      }
    }, // END: plugins

    handleTips: {
      init: function () {
        //togle tip on and off on click
        $('#curriculum-income .question .box-wrapper input[type=checkbox]').on('propertychange change', function () {
          //keep checking which boxes are selected when the boxes are added or removed
          income.global.utils.gatherAllBoxes.init();
          if (incomesFullyLoaded) {
            //update the income
            income.global.utils.updateIncome.init();
          }

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
        $('#curriculum-income .question .box-wrapper input[type=text]').bind('keyup change blur', function (e) {
          if ($(this).hasClass('error')) {
            cleanVal = curriculum.global.utils.cleanInput.init($(this).val());

            if (curriculum.global.utils.isEmpty(cleanVal) || cleanVal == 0) {
              $(this).addClass('error');
            } else {
              $(this).removeClass('error');
            }
          }
          $(this).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
          income.global.utils.updateIncome.init();



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
        $('#curriculum-income input[type=checkbox]').each(function () {
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

    updateIncome: {
      init: function () {
        /**
        loop through the checked boxes
        sanitize the data and replace the values in DOM	
        */

        var currentIncomes = {};

        for (var i = 0; i < boxesArray.length; i++) {
          //format the value input 
          value = boxesArray[i].siblings('.tip').find('input').val();
          var clean = curriculum.global.utils.cleanInput.init(value);

          //get the rate at which the income is gathered
          rate = boxesArray[i].siblings('.tip').find('select').val();

          //update the object that we're keeping
          theName = boxesArray[i].attr('id');
          currentIncomes[theName] = $.extend(currentIncomes[theName], { value: clean, time: rate, newValue: null }); //SERVER CHANGE: Extend
        }

        for (var key in userData.income) {
          if (userData.income.hasOwnProperty(key)) {
            if (currentIncomes[key] != undefined) {
              // update any existing incomes with changes
              userData.income[key] = $.extend(userData.income[key], currentIncomes[key]);
              delete currentIncomes[key];
            } else {
              // delete any removed incomes
              delete userData.income[key];
            }
          }
        }

        for (var key in currentIncomes) {
          if (currentIncomes.hasOwnProperty(key)) {
            // add any new incomes
            userData.income[key] = $.extend(userData.income[key], currentIncomes[key]);
          }
        }

        var totalIncome = curriculum.global.utils.totalIncome.value();
        $('#total .value').text(totalIncome).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        //animate the graph
        curriculum.global.utils.animateGraph.refresh();

        if (boxesArray.length === 0) {
          $('.pre').show();
          $('.post').hide();
        }
      } //end init
    }, // END udateIncome	
    saveData: function () {
      curriculum.global.utils.server.saveToServer(userData);
    }, //END saveData
    preloadData: {
      init: function () {
        if (userData.income) {

          p = userData.income;
          for (var key in p) {
            if (p.hasOwnProperty(key)) {

              checkbox = $('#' + key);

              value = $(checkbox).siblings('.tip').find('input').val(p[key].value);
              time = $(checkbox).siblings('.tip').find('select');
              time.val(p[key].time);

              checkbox = $('#' + key).trigger('click');

              time.removeData('dropkick');
              time.siblings('.dk_container').remove();
              time.dropkick({
                change: function () {
                  income.global.utils.updateIncome.init();
                }
              });
            }
          }

          // indicates that all incomes have been loaded, so we can avoid trying to re-calculate as each value is populated
          incomesFullyLoaded = true;

          // step data was preloaded; set tracking flag to true.
          curriculum.global.tracking.preloaded = true;

          //trigger a change so that the styled dropdowns get an update
          $('.question .box-wrapper input[type=text]').change();
          $('input[type=text]:first').focus();
        } else {
          userData.income = {};

          incomesFullyLoaded = true;

          // step data was not preloaded; set tracking flag to false.
          curriculum.global.tracking.preloaded = false;
        }

        //$("#content-container .content").delay(660).animate({opacity: 1}, 1000);
      }
    }, // END preloadData
    errors: function () {
      $('#footer .error-msg').text(errorMessage).fadeOut();

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

};     // END income global var


$('document').ready(function(){
	// activate utils
	income.global.utils.init();
});
