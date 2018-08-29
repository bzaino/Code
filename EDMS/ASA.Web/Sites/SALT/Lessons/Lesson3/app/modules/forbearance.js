define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/forbearance",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel) {
  // Create a new module
  var Forbearance = app.module();

  Forbearance.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/Deferment/',
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
      referenceId: 'forbearance',
      name: 'Forbearance',
      forbearanceMonths: 6,
      modifiedYear: 10
        }
    , baseRepaymentModel.Model.prototype.defaults),
    
    checkForMonthly0Balance: function(monthNum){
      var currentYear = Math.ceil(monthNum / 12);
      this.set({'modifiedYears': currentYear});
    },
    getNumberOfYears: function(){
      //Forbearance repayment is standard 10 yr, plus user-selected $0 payment months
      return this.get('modifiedYears');
    }, // END getNumberOfYears
    getMonthlyPayment: function(monthCount){
      //User pays $0 during forbearanceMonths, standard 10yr repayment otherwise
      if (monthCount < this.get('forbearanceMonths')){
        return 0;
      } else {
        return this.get('monthlyPayment');        
      }
    },
    parse: function(resp,xhr){      
      if(resp) {
        if ($.isArray(resp)) {
          var index = -1;
          for (var i = 0; i < resp.length; i++) {
            if (resp[i].ForbearanceLength > 0) {
              index = i;
              break;
            }
          }
          if (index >= 0) {
            resp = resp[index];
          }
        }

        if (resp.ForbearanceLength != null) {
          resp.id = resp.DefermentId;
          resp.forbearanceMonths = resp.ForbearanceLength;
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
        ForbearanceLength: this.get('forbearanceMonths'),
        PlanType: this.get('referenceId')               
      }
      this.set(updatedProps,{silent: true});
    } 
  });

  
  // Attach the Views sub-module into this module.
  Forbearance.Views = Views;
  
  // Required, return the module for AMD compliance
  return Forbearance;

});