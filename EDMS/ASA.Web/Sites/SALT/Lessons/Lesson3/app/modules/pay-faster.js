define([
  "lesson3/app",

// Libs
  "backbone",

// Views
  "lesson3/modules/repayment-options/pay-faster",

// Plugins
  "lesson3/plugins/backbone-asa-persistence",

//Base repayment model
  "lesson3/modules/baseRepayment"

],

function (app, Backbone, Views, LocalStorage, baseRepaymentModel) {
  // Create a new module
  var PayFaster = app.module();

  PayFaster.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/FasterRepayment/',
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
      referenceId: "payFaster",
      name: 'Pay more each month',
      additionalMonthlyPayment: 50,
      modifiedYears: 10
    }
    , baseRepaymentModel.Model.prototype.defaults),

    getMonthlyPayment: function(monthCount){
      var monthlyPayment = this.get('monthlyPayment');
      if (monthlyPayment < 50) {
        monthlyPayment = 50;
      }    

      return monthlyPayment + this.get('additionalMonthlyPayment');
    },
    getNumberOfYears: function () {
      return this.get('modifiedYears');
    },
    checkForMonthly0Balance: function (monthNum) {
      var currentYear = Math.ceil(monthNum / 12);
      this.set({ 'modifiedYears': currentYear });
    },
    resetToDefaults: function () {
      this.set({ 'modifiedYears': this.get('years') });
    },
    parse: function (resp, xhr) {

      if (resp) {
        if ($.isArray(resp)) {
          var index = -1;
          for (var i = 0; i < resp.length; i++) {
            if (resp[i].AdditionalMonthlyPayment > 0) {
              index = i;
              break;
            }
          }
          if (index >= 0) {
            resp = resp[index];
          }
        }

        if (resp.AdditionalMonthlyPayment != null) {
          resp.additionalMonthlyPayment = resp.AdditionalMonthlyPayment;
          resp.id = resp.FasterRepaymentId;
          //Add loanTotal if found in app object
          if (typeof app.baseLoanTotal !== 'undefined' && app.baseLoanTotal > 0) {
            resp.totalBalance = app.baseLoanTotal;
          }
        }

        return resp;
      }

    },
    updateForServer: function () {
      var updatedProps = {
        AdditionalMonthlyPayment: this.get('additionalMonthlyPayment'),
        PlanType:this.get('referenceId')
      };
      this.set(updatedProps, { silent: true });
    }

  });


  // Attach the Views sub-module into this module.
  PayFaster.Views = Views;

  // Required, return the module for AMD compliance
  return PayFaster;

});
