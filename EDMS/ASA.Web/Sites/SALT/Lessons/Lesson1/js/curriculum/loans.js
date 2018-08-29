var loans = loans || {};
var savedObject = {};
var loansTotal = 0;
var gatherCredit = false;
var gatherLoans  = false;
save = loans;


loans.global = {
  utils: {
    init: function(){

      // Append stylesheet with IE support
      if (document.createStyleSheet) {
        document.createStyleSheet('css/curriculum/loans.css');
      } else {
        $('head').append('<link rel="stylesheet" href="css/curriculum/loans.css" type="text/css" />');
      }

      // Append ID for this page
      $('#content-container .content').attr('id','curriculum-loans');

      // Update page context
      curriculum.global.utils.paginate.updateContext("3", "Your Expenses", "spent on loans", "Other Expenses", "Keep Going");
    
      $("#content-container .content").show().css({opacity: 1})
      $('#total .value').text('$0');
      
      //bind events for value input
      $('#curriculum-loans input.large').bind('keyup change blur', function(e){
        $(this).formatCurrency({ roundToDecimalPlace: 0 });
          loans.global.utils.updateLoans.init();
        // Highlight, only on keyup
        if(e.type === 'keyup' || e.type === 'blur'){
          $('#total .value').stop(true,true).effect("highlight", {color: '#faff00'}, 1500);
        }
      });      
      
      
      
      $('#credit-form').hide();
      $('#loans-form').hide();
      
      if(userData.income != undefined && userData.income.credit != undefined){
        gatherCredit = true;
        $('#credit-form').show();
      }
      if(userData.income != undefined && userData.income.loans != undefined){
        gatherLoans = true;
        $('#loans-form').show();
      }
      
      $('#curriculum-loans .dropdown-large.time').dropkick({
        theme: 'large',
        change: function(){
          loans.global.utils.updateLoans.init();
        }
      });
      loans.global.utils.preloadData();
      
    },
    updateLoans: {
      init: function(){
        //reset everything 1st
        loansTotal = 0;
        savedObject = {};
        if(userData.expenseList != undefined){
           delete userData.expenseList.credit;
           delete userData.expenseList.loans;
        }
        //////Credit
        if(gatherCredit){
          creditValue = $('#credit').val();
          creditClean = curriculum.global.utils.cleanInput.init(creditValue);
          creditRate  = $('#curriculum-loans select.credit-time').val();
          creditTimed = curriculum.global.utils.determineRate.init(creditClean, creditRate);
          
          savedObject.credit = {value: creditClean, time: creditRate, newValue: null};
          
          userData.expenseList.credit = creditTimed;
          
          loansTotal += creditTimed
        }
        //////loans
        if(gatherLoans){
          loansValue = $('#loans').val();
          loansClean = curriculum.global.utils.cleanInput.init(loansValue);
          loansRate  = $('#curriculum-loans select.loans-time').val();
          loansTimed = curriculum.global.utils.determineRate.init(loansClean, loansRate);
          
          savedObject.loans = {value: loansClean, time: loansRate, newValue: null};
          
          userData.expenseList.loans = loansTimed;
          
          loansTotal += loansTimed
        }
        
        
        $('#total .value').text(loansTotal).toNumber().formatCurrency({ roundToDecimalPlace: 0 });
        curriculum.global.utils.animateGraph.refresh();
      }
    }, // END updateLoans
    saveData: function(){
      
      if(userData.income != undefined && userData.income.credit != undefined){
        if(savedObject.credit.value > 0){
          userData.expenses.credit = savedObject.credit;
        } else {
          delete userData.expenses.credit
        }
      }
      
      if(userData.income != undefined && userData.income.loans != undefined){
        if(savedObject.loans.value > 0){
          userData.expenses.loans = savedObject.loans;
        } else {
          delete userData.expenses.loans
        }
      }
      
    }, //END saveData
    preloadData: function() {
      //credit

      if(userData.expenses.credit != undefined){

        //value
        $('#curriculum-loans #credit').val(userData.expenses.credit.value);
        //time
        $('#curriculum-loans select.credit-time').val(userData.expenses.credit.time);
        $('#curriculum-loans select.credit-time').change();
      }
      
      //loans
      if(userData.expenses.loans != undefined){

        //value
        $('#curriculum-loans #loans').val(userData.expenses.loans.value);
        //time
        $('#curriculum-loans select.loans-time').val(userData.expenses.loans.time);
        $('#curriculum-loans select.loans-time').change();
      } 
      
      $('#curriculum-loans input[type=text]').change();
      $('#curriculum-loans input[type=text]:first').focus();
      loans.global.utils.updateLoans.init();
      $("#content-container .content").delay(360).animate({opacity: 1}, 1000);
      
    }, // END preloadData
		errors: function(){
  	 pagePass = true;
  	 curriculum.global.utils.paginate.next();
  	}

  } // END: utils

}; // END expenses global var

$('document').ready(function(){
  // activate utils
  loans.global.utils.init();
});
