define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/hardship-deferment",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel) {
  // Create a new module
  var HardShipDeferment = app.module();

  HardShipDeferment.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/Deferment/',
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
      referenceId: 'hardShipDeferment',
      name: 'Unemployment deferment',
      defermentMonths: 6,
      modifiedYear: 10,
      additionalMonthlyPayment: 0
        }
    , baseRepaymentModel.Model.prototype.defaults),
    
    checkForMonthly0Balance: function(monthNum){
      var currentYear = Math.ceil(monthNum / 12);
      this.set({'modifiedYears': currentYear});
    },
    getNumberOfYears: function(){
      //Deferment repayment is standard 10 yr, plus user-selected $0 payment months
      return this.get('modifiedYears');
    }, // END getNumberOfYears
    getMonthlyPayment: function(monthCount){
      //User pays $0 during defermentMonths, standard 10yr repayment otherwise
      if (monthCount < this.get('defermentMonths')){
        return 0;
      } else {
        return this.get('monthlyPayment') + this.get('additionalMonthlyPayment');        
      }
    },
    getMonthlyInterestRate: function(monthCount){
      //During hardship deferment months we don't accrue interest on subsidized loans
      if(monthCount >= this.get('defermentMonths')){
        return this.get('interestRate') / 12;
      } else {
        return -1;
      }
    }, // END getMonthlyInterestRate()
    parse: function(resp,xhr){      
      if(resp) {
        if ($.isArray(resp)) {
          var index = -1;
          for (var i = 0; i < resp.length; i++) {
            if (resp[i].HardshipDefermentLength > 0) {
              index = i;
              break;
            }
          }
          if (index >= 0) {
            resp = resp[index];
          } else { return; }
        }

        if (resp.HardshipDefermentLength != null) {
          resp.id = resp.DefermentId;
          resp.defermentMonths = resp.HardshipDefermentLength;
          resp.additionalMonthlyPayment = resp.HardshipDefermentExtraAmount;
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
        HardshipDefermentLength: this.get('defermentMonths'),  
        HardshipDefermentExtraAmount: this.get('additionalMonthlyPayment'),
        PlanType: this.get('referenceId')            
      };
      this.set(updatedProps,{silent: true});
    } 

  });

  
  // Attach the Views sub-module into this module.
  HardShipDeferment.Views = Views;
  
  // Required, return the module for AMD compliance
  return HardShipDeferment;

});