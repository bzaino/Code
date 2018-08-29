define([
  "lesson3/app",

  // Libs
  "backbone",
  'salt',
  
  // Views
  "lesson3/modules/repayment-options/income-sensitive-repayment",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment"

],

function(app, Backbone, SALT, Views, LocalStorage, baseRepaymentModel) {
  //This repayment model allows the user to specify percentage of monthly income as monthly payment, must be at least interest accrued
  //This lowered monthly payment runs for 5 years, then standard repayment on the remaining balance across 10 years, 15 year total period
  var IncomeSensitive = app.module();

  IncomeSensitive.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/LowerPayment/',
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
      referenceId: 'incomeSensitive',
      name: 'Income-sensitive',
      income: 0,
      incomePctPayment: 4, //Integer representing %age of income paid to loan
      modifiedYears: 15,
      monthlyPayment: [], //Array of 2 monthly payment amounts, each paid for 5 years for total of 10 years
      incomePercentageMaxMin: [25,4] //this is used when user is trying to modify the incomePctPayment value
    }, baseRepaymentModel.Model.prototype.defaults),
    initialize: function() {
      var _this = this;
      SALT.on('income:changed', function (income) {
        _this.setIncome(income);
      });
      SALT.on('interestRate:changed', function (interestRate) {
        _this.setInterestRate(interestRate);
      });
    },
    setIncome: function(income){
      //set the models income
      // console.log("modelName " + this.attributes.referenceId + " New Income: " + income);
      this.set('income', income);
    },
    getMonthlyPayment: function(monthCount){
      var index = 0;
      if(Math.floor(monthCount / 60) > 0){
        index = 1;
      }
      return this.get('monthlyPayment')[index];
    },
    getNumberOfYears: function(){
      return Math.ceil(this.get('modifiedYears'));
    }, 
    checkForMonthly0Balance: function(monthNum){
      var currentYear = Math.ceil(monthNum / 12);
      this.set({'modifiedYears': currentYear});
    },
    
    calculateMonthlyPayment: function() {
      var principal=this.get('totalBalance');
      var balance=this.get('totalBalance');
      var rate= this.getMonthlyInterestRate() * 12 * 100;
      var income= this.get('income') / 12; //monthly
      var incomePctPayment=this.get('incomePctPayment'); //as integer

      var numberOfPayments = 180;  //default number of standard and inc sens payments
      var remainingTerms = 120;  // default number of standard payments

      var r=rate/12/100;//Periodic Interest Rate
      var Pay1= r*principal; //Calculate interest only portion

      // Verify that incomePctPayment is being met
      if ( (Math.round((Pay1/income)*1000)/10) < incomePctPayment ) {
        Pay1= income * incomePctPayment / 100;
        balance = principal*(Math.pow((1+r),60))-(Pay1/r)*(Math.pow((1+r),60)-1);
        // too much money! loan paid within the 5 year period. calculate pay1 as a 5 year loan.
        if (balance <0 ) {
          Pay1= r*principal/(1-Math.pow((1+r),-60));
          balance = 0;
          numberOfPayments = 60;
        } 
      }
      var Pay2= r*balance/(1-Math.pow((1+r),-120)); //Calulate Standard portion

      // Final Case: if standard portion would be less then the user-selected payment amount, bring standard portion payment to match
      if (Pay2 < Pay1 && Pay2 !== 0) {
        // Calculate number of terms needed at continuing 4% payments
        remainingTerms = -Math.log(1-r*balance/Pay1) / Math.log(1 + r); 
        Pay2 = Pay1; // Set final payments equal to initial payments
        if (Math.round(remainingTerms) / remainingTerms < 1) {
          remainingTerms = Math.round(remainingTerms) + 1;
        }
        numberOfPayments = 60 + remainingTerms;
      }

      //Save collection of monthly payment amounts
      var monthlyPayments = [];
      monthlyPayments.push(Pay1);
      if(Pay2 !== 0){
        //Only push if its actually used; will be 0 if repayment completed within 5 years
        monthlyPayments.push(Pay2);
      }
      this.set({'monthlyPayment': monthlyPayments});
    },
    parse: function(resp,xhr){
          if(resp) {
            if ($.isArray(resp)) {
              var index = -1;
              for (var i = 0; i < resp.length; i++) {
                if (resp[i].IncomeSensitiveYearlyIncome > 0) {
                  index = i;
                  break;
                }
              }
              if (index >= 0) {
                resp = resp[index];
              }
            }

            if (resp.IncomeSensitiveYearlyIncome !== null && resp.IncomeSensitiveYearlyIncome > 0) {
              resp.id = resp.LowerPaymentId;
              resp.incomePctPayment = resp.IncomeSensitiveIncomePercentage*100;
              //Add loanTotal if found in app object
              if (typeof app.baseLoanTotal !== 'undefined' && app.baseLoanTotal > 0) {
                resp.totalBalance = app.baseLoanTotal;
              }
              SALT.trigger('incomePctPayment:changed', resp.incomePctPayment);
            }
            
            return resp;
          }
        },
    updateForServer: function() {
          var updatedProps = {
            IncomeSensitiveIncomePercentage: this.get('incomePctPayment')/100,
            PlanType: this.get('referenceId')            
          };
          this.set(updatedProps,{silent: true});
        } 

  });

  
  // Attach the Views sub-module into this module.
  IncomeSensitive.Views = Views;
  
  // Required, return the module for AMD compliance
  return IncomeSensitive;

});