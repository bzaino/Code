var positiveGoal = positiveGoal || {};

//var expensesCutGoal = initSavingsMonthlyGoal - initCashFlow;

positiveGoal.global = {
  utils: {
    init: function () {
      $('#content-left .reset-changes').click(function () {
        graph.global.utils.clearChartData();
        //hide and uncheck hidden modifiers
        $('#content-left').find("input:checkbox:checked").prop('checked', false);
        $('input[type=checkbox]').siblings('.cbox').removeClass('checked');
        $('.hidden-modifiers').css('display', 'none');

        //reset copy for modifiers
        $('#more-money-value').val('$');
        $('#content-left .big').val('ex: babysitting');

        //reset popUp expense valuespositiveGoal
        //graphCashFlow = editCashflow.global.utils.getNewCashflow(initCashFlow,moreMoney);
        $('#expenses-overlay .reset-changes').trigger('click');
        
        useExtra = false;
        editedCashFlow = 0;
        graphCashFlow = initCashFlow;
        waitAsLong = 0;
        moreMoney = 0;
        graph.global.utils.drawGraph();
        editCashflow.global.utils.startGraph();
      });

      $('#more-money-value').bind('change keyup',function(){
        $(this).formatCurrency({ roundToDecimalPlace: 0 });
      });


    },

    plugins: function () {
      // Styled inputs
      $('input[type=checkbox]').styledInputs();
    }, // END: plugins

    checkBoxes: function () {

      //show hide modifiers if checkboxes checked
      $('input[type=checkbox]').unbind('propertychange');
      $('input[type=checkbox]').unbind('change');
      $('input[type=checkbox]').bind('propertychange change', function () {
        if ($(this).is(':checked')) {
          $(this).siblings('.hidden-modifiers').slideDown('fast');
          //revert graph to modifier state if values were entered previously
          //Make More Money
          if ($(this).is('#more-money')) {
            var moreMoney = curriculum.global.utils.cleanInput.init($('#more-money-value').val());
            if (moreMoney > 0) {
              $('.more-money').trigger('click');
            }
          }
        } else {
          $(this).siblings('.hidden-modifiers').slideUp('fast');
        }
      });
    },//END: checkBoxes

    //cut expenses
    cutExpenses: function() {
      //click back button
      $('#expenses-overlay').find('.back').click(function(e){
        e.preventDefault();
        //if finish cutting has been clicked already
        if($('#expenses-overlay').find('.done').hasClass('click')){
          editCashflow.global.utils.populateEditValueProperty();
        } else {
          editCashflow.global.utils.populateNewValueProperty();
        }
       
        positiveGoal.global.utils.cleanUpEditCashflow();
        editCashflow.global.utils.startGraph();

        curriculum.global.tracking.trigger("lesson:graph:optionChange", {
          graph: {
            option: 'expenses-overlay-back'
          },
          step: {
            number: save.global.step
          }
        });

        close();
      });

      //click done button
      $('#expenses-overlay').find('.done').bind('click', function(e){
        e.preventDefault();
        $(this).one().addClass('click');
        editCashflow.global.utils.populateEditValueProperty();
        
        editedCashFlow = editCashflow.global.utils.getNewCashflow();
        graph.global.utils.drawGraph();

        curriculum.global.tracking.trigger("lesson:graph:optionChange", {
          graph: {
            option: 'expenses-overlay-done'
          },
          step: {
            number: save.global.step
          }
        });

        close();
      });

      //reset modal values to originals
      $('#expenses-overlay').find('.reset-changes').click(function(e){
        e.preventDefault();
        editCashflow.global.utils.populateNewValueProperty();
        $('#expenses-overlay').find('.done').removeClass('click');
        positiveGoal.global.utils.cleanUpEditCashflow();
        editCashflow.global.utils.startGraph();
        graph.global.utils.drawGraph();
        
        curriculum.global.tracking.trigger("lesson:graph:optionChange", {
          graph: {
            option: 'expenses-overlay-reset-changes'
          },
          step: {
            number: save.global.step
          }
        });
      });

      //close modal window
      function close() {
        $('#expenses-overlay-container').fadeOut('fast');
      }



    },//END cutExpenses();
    cleanUpEditCashflow: function(){
      $('#expenses-overlay').find('ul li').remove();
      $('#curriculum-cashflow ').find('.bar').remove();
    }, // END cleanUpEditCashflow
    graphModifiers: function () {

      //positive outcome reach goal faster
      $('#goal-faster').bind('propertychange change', function () {
        
        if( $('#goal-faster').is(':checked') ){
          useExtra = true;
          curriculum.global.tracking.trigger("lesson:graph:optionChange", {
            graph: {
              option: $(this).attr("id")
            },
            step: {
              number: save.global.step
            }
          });
        } else {
          useExtra = false;
        }
        graph.global.utils.drawGraph();
        
      });

      //cut expenses
      $('#cut-expenses').bind('propertychange change', function () {
        positiveGoal.global.utils.cleanUpEditCashflow();
        editCashflow.global.utils.startGraph();
        positiveGoal.global.utils.updateCopy();
        
        if ($(this).is(':checked')) {
          editedCashFlow = editCashflow.global.utils.getNewCashflow();
          graph.global.utils.drawGraph();
          curriculum.global.tracking.trigger("lesson:graph:optionChange", {
            graph: {
              option: $(this).attr("id")
            },
            step: {
              number: save.global.step
            }
          });
        } else {
          editedCashFlow = 0;
          graph.global.utils.drawGraph();
        }
        
      });

      //negative outcome earn more money/mon to reach goal
      $('#more-money').bind('propertychange change', function () {
        if ($(this).is(':checked')) {
          moreMoneySet();
          curriculum.global.tracking.trigger("lesson:graph:optionChange", {
            graph: {
              option: $(this).attr("id")
            },
            step: {
              number: save.global.step
            }
          });
        } else {
          moreMoneyReset();
        }
        
        positiveGoal.global.utils.updateCopy();
      });

      $('.more-money').click(function(e){
          e.preventDefault();
          moreMoneySet();
          editedCashflow = editCashflow.global.utils.getNewCashflow();
          positiveGoal.global.utils.updateCopy();
          curriculum.global.tracking.trigger("lesson:graph:optionChange", {
            graph: {
              option: 'more-money-update-graph'
            },
            step: {
              number: save.global.step
            }
          });
      });

      $('.open-expenses').click(function(e){
        e.preventDefault();
        positiveGoal.global.utils.cleanUpEditCashflow();
        editCashflow.global.utils.startGraph();
        $('#expenses-overlay-container').fadeIn('fast');
        curriculum.global.tracking.trigger("lesson:graph:optionChange", {
          graph: {
            option: 'open-expenses-overlay'
          },
          step: {
            number: save.global.step
          }
        });
      });


      function moreMoneySet() {
        moreMoney = curriculum.global.utils.cleanInput.init($('#more-money-value').val());
        graph.global.utils.drawGraph();
      }

      function moreMoneyReset() {
        moreMoney = 0;
        graph.global.utils.drawGraph();
      }


      $('#reconsider-goal').bind('propertychange change', function () {
        if ($(this).is(':checked')) {
          if(graphCashFlow > 1){
            waitAsLong = 1;
          }
          //newMonthsToGoal = Math.ceil(goalAmount / graphCashFlow);
          
          curriculum.global.tracking.trigger("lesson:graph:optionChange", {
            graph: {
              option: $(this).attr("id")
            },
            step: {
              number: save.global.step
            }
          });
          //console.log('newmonthstogoal: ' + newMonthsToGoal);
        } else {
          //newMonthsToGoal = Math.ceil(goalAmount / graphCashFlow);
          
          waitAsLong = 0;
          //console.log('UNCHECKED');
        }
        graph.global.utils.drawGraph();
        positiveGoal.global.utils.updateCopy();
      });


      //Set-up checkbox copy
      //set initial months to goal on load
//asdf
//      $('#content-left span.months').html((Math.ceil(initSavingsTarget / graphCashFlow) - graphMonthsToGoal) + ' more months');

      //set copy for expenses modifier
      var expenseAmount = initSavingsMonthlyGoal - initCashFlow;
      $('#content-left .expenses').html('$' + expenseAmount);
    },

    //copy for change goal date
    updateCopy: function() {
      //Update last checkbox copy to reflect goal date
/*

      if (graphCashFlow <= 0) {
        $('#content-left .new-goal').html('It looks like you won\'t reach your goal on time. Increase your income or cut your expenses to reach your goal.');
      } else if (newMonthsToGoal == 0) {
        if (extraSavings < 1) {
          $('#content-left .new-goal').html('It looks like you\'ll reach your goal on time!');
        } else if (graphMonthsToGoal < initMonthsToGoal) {
          $('#content-left .new-goal').html('It looks like you\'ll reach your goal ' + parseInt(initMonthsToGoal - graphMonthsToGoal) + ' months sooner!');
        } else {
          $('#content-left .new-goal').html('It looks like you will reach your goal with $' + $.commify(Math.ceil(graphCashFlow - initSavingsMonthlyGoal)) + ' extra every month!');
        }
      } else {
        $('#content-left .new-goal').html('I\'m willing to save for ' + parseInt(newMonthsToGoal - initMonthsToGoal) + ' more months to reach my goal.');
      }
*/
    }
  } //END utils
}; // END goal-left global var
$('document').ready(function () {
  // activate utils
  positiveGoal.global.utils.init();
  positiveGoal.global.utils.checkBoxes();
  positiveGoal.global.utils.plugins();
  positiveGoal.global.utils.cutExpenses();
  positiveGoal.global.utils.graphModifiers();
});

