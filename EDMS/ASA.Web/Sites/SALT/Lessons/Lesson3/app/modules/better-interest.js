define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/better-interest",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel) {

  // Create a new module
  var BetterInterest = app.module();

  BetterInterest.Model = baseRepaymentModel.Model.extend({
    localStorage: new Store("betterInterest-backbone"),
    url: app.serverUrl + '/Lesson3/FasterRepayment/',
    defaults: _.extend({
        referenceId: 'betterInterest',
        name: 'Earn a better interest rate',
        modifiedInterestRate: .0386
      },
    baseRepaymentModel.Model.prototype.defaults),
    
      getMonthlyPayment: function(monthCount){
        var monthlyPayment = this.get('monthlyPayment');
        if (monthlyPayment < 50) {
          return 50;
        } else {
          return monthlyPayment;
        }
      },
      getMonthlyInterestRate: function(){
        return this.get('modifiedInterestRate') / 12;
      }, // END getMonthlyInterestRate()
      calculateMonthlyPayment: function (){
        //This method uses the baseRepayment values for calculation, not any child-type overrides
        //Needed so we can reference original payment amount in betterInterest 
        var monthlyInterestRate    = this.getMonthlyInterestRate();
        var years = this.get('years');
        var totalNumberOfPayments  = years * 12;

        var newMonthlyPayment = this.solveForMonthlyPaymentFormula(monthlyInterestRate, this.get('totalBalance'), totalNumberOfPayments);
        this.set({'monthlyPayment': newMonthlyPayment});
      },
//    getNumberOfYears: function(){
//      //http://www.amortizationer.com/amortization-formula.html 
//      var Rvalue = 1 + (this.getMonthlyInterestRate());
//      var Avalue = this.get('totalBalance');
//      var Pvalue = this.getMonthlyPayment();
//      
////      N = -ln [ 1 - A ( R - 1 ) / P ] / ln[ R ]
//      var numberOfPaymentsNumerator = -Math.log( 1 - ((Avalue * (Rvalue - 1)) / Pvalue));
//      var numberOfPaymentsDenominator = Math.log( Rvalue );
//      var numberOfPayments = Math.floor(numberOfPaymentsNumerator / numberOfPaymentsDenominator);

//      return Math.ceil(numberOfPayments / 12);
//    },
      parse: function(resp,xhr){
        if (resp) {
          if ($.isArray(resp)) {
            var index = -1;
            for (var i = 0; i < resp.length; i++) {
              if (resp[i].LowerInterestRate > 0) {
                index = i;
                break;
              }
            }
            if (index >= 0) {
              resp = resp[index];
            }
          }

          if(resp.LowerInterestRate !== null){
            resp.modifiedInterestRate = resp.LowerInterestRate;
            resp.id = resp.FasterRepaymentId;
            //Add loanTotal if found in app object
            if (typeof app.baseLoanTotal !== 'undefined' && app.baseLoanTotal > 0) {
              resp.totalBalance = app.baseLoanTotal;
            }
          }
          return resp;
        }
      },
      updateForServer: function() {
        var updatedProps = {
          LowerInterestRate: this.get('modifiedInterestRate'),
          PlanType:this.get('referenceId')
        };
        this.set(updatedProps,{silent: true});
      }
    });
  
  // Attach the Views sub-module into this module.
  BetterInterest.Views = Views;
  
  // Required, return the module for AMD compliance
  return BetterInterest;
});