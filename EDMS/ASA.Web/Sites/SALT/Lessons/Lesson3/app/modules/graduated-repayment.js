define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/graduated-repayment",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment",

  // Keeping the calculations separate since they are lengthy
  "lesson3/modules/graduated-payment-calculator"
],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel, graduatedPaymentCalculator) {

  // Create a new module
  var GraduatedRepayment = app.module();

  //Calculations for this model based on calculator www.asa.org/repay/calculators/graduated/default.aspx
  GraduatedRepayment.Model = baseRepaymentModel.Model.extend({
    localStorage: new Store("graduatedRepayment-backbone"),

    defaults: _.extend({
      referenceId: 'graduatedRepayment',
      name: 'Graduated Payment',
      monthlyPayment: [] 
    }, baseRepaymentModel.Model.prototype.defaults),
    
    initialize: function() {
      var _this = this;
      this.performCalculations();
      SALT.on('interestRate:changed', function (interestRate) {
        _this.setInterestRate(interestRate);
      });
    },

    getNumberOfYears: function() { 
      var years = Math.ceil(graduatedPaymentCalculator.getMonthsInRepayment() / 12);
      return years;
    },

    performCalculations: function(){
      this.calculateMonthlyPayment();
      this.calculateMonthlyBalancePoints();
    },

    calculateMonthlyPayment: function () {
      var principal = this.get('totalBalance');
      var rate = this.getMonthlyInterestRate() * 12;

      graduatedPaymentCalculator.setLoanAmount(principal);
      graduatedPaymentCalculator.setInterestRate(rate);
      graduatedPaymentCalculator.calculate();

      var monthlyPayment = graduatedPaymentCalculator.getMonthlyPayments();     
      this.set({'monthlyPayment': monthlyPayment });
    },

    calculateAveragePaymentYears1to2: function() {
      var monthlyPayments = this.get('monthlyPayment');
      return monthlyPayments[0];
    },

    calculateAveragePaymentYears3to5: function() {
      var monthlyPayments = this.get('monthlyPayment');

      if( monthlyPayments.length > 1 ) {
        if( typeof monthlyPayments[2] === "undefined" ) {
          return monthlyPayments[1];
        } else {
          return ((monthlyPayments[1] * 2) + monthlyPayments[2]) / 3;
        }
      } else {
        return "N/A";
      }
    },
    
    calculateAveragePaymentYears6to10: function(){
      var monthlyPayments = this.get('monthlyPayment');
      if( monthlyPayments.length > 2 ) {
        return (monthlyPayments[2] + ( monthlyPayments[3] * 2 ) + ( monthlyPayments[4] * 2 )) / 5;  
      } else {
        return "N/A";
      }
    },

    calculateAveragePaymentYears11to15: function(){
      return "N/A";
    },

    getMonthlyPayment: function(monthCount){
      var monthlyPayments = this.get('monthlyPayment');
      var paymentPeriod = Math.floor(monthCount / 24);

      // return the monthly payment amount; if we've exceded the array bounds, return the last payment amount (occurs sometimes when the final months loan balance is a few cents off 0)
      var paymentAmount = monthlyPayments[paymentPeriod] || monthlyPayments[monthlyPayments.length - 1];

      return parseFloat(paymentAmount.toFixed(2));
    }
  });

  
  // Attach the Views sub-module into this module.
  GraduatedRepayment.Views = Views;
  
  // Required, return the module for AMD compliance
  return GraduatedRepayment;

});
