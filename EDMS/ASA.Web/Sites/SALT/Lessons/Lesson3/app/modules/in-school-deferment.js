define([
  "lesson3/app",

  // Libs
  "backbone",
  
  // Views
  "lesson3/modules/repayment-options/in-school-deferment",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, Views, LocalStorage, baseRepaymentModel) {
  //This repayment option allows the user to select 0-12 months of deferment
  //During that time $0 monthly payment and interest DOES NOT accrue
  //Standard 10yr term repayment begins once deferment period is over
  var InSchoolDeferment = app.module();

  InSchoolDeferment.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/Deferment/',
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
      referenceId: 'inSchoolDeferment',
      name: 'In-school deferment',
      defermentMonths: 6,
      modifiedYears: 10
    }, baseRepaymentModel.Model.prototype.defaults),
    
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
      var monthlyPayment = this.get('monthlyPayment');
      if (monthCount < this.get('defermentMonths')){
        monthlyPayment = 0;
      } else {
        if (monthlyPayment < 50) {
          monthlyPayment = 50;
        }
      }
      return monthlyPayment;
    },
    getMonthlyInterestRate: function(monthCount){
      //During in-school deferment months we don't accrue interest
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
            if (resp[i].DefermentLength > 0) {
              index = i;
              break;
            }
          }
          if (index >= 0) {
            resp = resp[index];
          } else { return; }
        }

        if (resp.DefermentLength != null) {
          resp.id = resp.DefermentId;
          resp.defermentMonths = resp.DefermentLength;
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
        DefermentLength: this.get('defermentMonths'),
        PlanType: this.get('referenceId')  
      };
      this.set(updatedProps,{silent: true});
    } 

  });

  
  // Attach the Views sub-module into this module.
  InSchoolDeferment.Views = Views;
  
  // Required, return the module for AMD compliance
  return InSchoolDeferment;

});