define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/set-timeline",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel) {

  // Create a new module
  var SetTimeline = app.module();

  SetTimeline.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/FasterRepayment/',
    defaults: _.extend({
      referenceId: 'setTimeline',
      name: 'Set your own timeline'
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
    getNumberOfYears: function(){
      return  this.get('years');
    }, // END getNumberOfYears

    parse: function(resp,xhr) {
      if(resp) {

        if ($.isArray(resp)) {
          var index = -1;
          for (var i = 0; i < resp.length; i++) {
            if (resp[i].ShorterTimeline > 0) {
              index = i;
              break;
            }
          }
          if (index >= 0) {
            resp = resp[index];
          }
        }

        if(resp.ShorterTimeline != null){
          resp.years = resp.ShorterTimeline;
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
        ShorterTimeline: this.get('years'),
        PlanType: this.get('referenceId')    
      };
      this.set(updatedProps,{silent: true});
    }

  });

  
  // Attach the Views sub-module into this module.
  SetTimeline.Views = Views;
  
  // Required, return the module for AMD compliance
  return SetTimeline;

});
