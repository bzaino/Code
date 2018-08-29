define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/extended-repayment",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel) {
  // Create a new module
  var ExtendedRepayment = app.module();

  ExtendedRepayment.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/LowerPayment/',
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
      referenceId: "extendedRepayment",
      name: 'Extended',
      modifiedYears: 10
    }
    , baseRepaymentModel.Model.prototype.defaults),
    
    calculateMonthlyPayment: function (){
      //Override here to use custom year value
      var monthlyInterestRate = this.get('interestRate') / 12;
      var years = this.getNumberOfYears();
      var totalNumberOfPayments  = years * 12;

      var newMonthlyPayment = this.solveForMonthlyPaymentFormula(monthlyInterestRate, this.get('totalBalance'), totalNumberOfPayments);
      this.set({'monthlyPayment': newMonthlyPayment});      
    },
    getNumberOfYears: function(){
      return Math.ceil(this.get('modifiedYears'));
    },
    parse: function(resp,xhr){      
      if(resp) {
       

        if (resp.ExtendedLength > 0) {
          resp.id = resp.LowerPaymentId;
          resp.modifiedYears = resp.ExtendedLength;
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
        ExtendedLength: this.get('modifiedYears'),
        PlanType: this.get('referenceId')           
      };
      this.set(updatedProps,{silent: true});
    } 
  });

  
  // Attach the Views sub-module into this module.
  ExtendedRepayment.Views = Views;
  
  // Required, return the module for AMD compliance
  return ExtendedRepayment;

});