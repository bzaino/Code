define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/standard-repayment",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel) {
  // Create a new module
  var StandardRepayment = app.module();

  StandardRepayment.Model = baseRepaymentModel.Model.extend({
    localStorage: new Store("standardRepayment-backbone"),
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
        referenceId: "standardRepayment", 
        name: "Standard repayment",
        hideAdjustedKey: true
      }
    , baseRepaymentModel.Model.prototype.defaults),

    getMonthlyPayment: function(monthCount){
      var monthlyPayment = this.get('monthlyPayment');
      if (monthlyPayment < 50) {
        return 50;
      } else {
        return monthlyPayment;
      }    
    } ,
    checkForMonthly0Balance: function (monthNum) {
      var currentYear = Math.ceil(monthNum / 12);
      this.set({ 'years': currentYear });
    },
    resetToDefaults: function () {
      this.set({ 'years': 10 });
    }


  });

  
  // Attach the Views sub-module into this module.
  StandardRepayment.Views = Views;
  
  // Required, return the module for AMD compliance
  return StandardRepayment;

});
