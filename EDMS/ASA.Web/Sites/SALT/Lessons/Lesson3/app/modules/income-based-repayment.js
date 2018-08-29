define([
  "lesson3/app",

  // Libs
  "backbone",
  'salt',
  
  // Views
  "lesson3/modules/repayment-options/income-based-repayment",
  
  // Plugins
  "lesson3/plugins/backbone-asa-persistence",
  
  //Base repayment model
  "lesson3/modules/baseRepayment",

  // Keeping the calculations separate since they are lengthy
  "lesson3/modules/income-based-repayment-calculator"
],

function(app, Backbone, SALT, Views, LocalStorage, baseRepaymentModel, ibrCalculator) {
  //This repayment option starts with 1 year of reduced payments,
  //followed by standard repayment over 10 year term
  var IncomeBased = app.module();

  IncomeBased.Model = baseRepaymentModel.Model.extend({
    url: app.serverUrl + '/Lesson3/LowerPayment/',
    //We extend defaults from base class, allows us to inherit defaults set there
    defaults: _.extend({
      referenceId: 'incomeBased',
      name: 'Income-based',
      income: 0,
      dependents: 1,
      state: 'other',
      //Array of 2 monthly payment amounts, first for low payment period
      monthlyPayment: []
    }, baseRepaymentModel.Model.prototype.defaults),
    initialize: function() {
      var _this = this;
      SALT.on('state:changed', function (state) {
        _this.setState(state);
      });
      SALT.on('dependents:changed', function (dependents) {
        _this.setDependents(dependents);
      });
      SALT.on('income:changed', function (income) {
        _this.setIncome(income);
      });
      SALT.on('interestRate:changed', function (interestRate) {
        _this.setInterestRate(interestRate);
      });
    },
    setDependents: function(dependents){
      //set the model dependents
      // console.log("modelName " + this.attributes.referenceId + " New Dependents: " + dependents);
      return this.set('dependents', dependents);
    },
    setIncome: function(income){
      //set the model income
      // console.log("modelName " + this.attributes.referenceId + " New Income: " + income);
      return this.set('income', income);
    },
    setState: function(state){
      //set the model state
      // console.log("modelName " + this.attributes.referenceId + " New State: " + state);
      return this.set('state', state);
    },
    getNumberOfYears: function(){
      //Always round up to next integer
      return 11;
    }, // END getNumberOfYears
    
    getMonthlyInterestRate: function(monthCount){
      //During first year we don't accrue interest
      if(monthCount >= 12){
        return this.get('interestRate') / 12;
      } else {
        return -1;
      }
    }, // END getMonthlyInterestRate()
    calculateMonthlyPayment: function () {

      ibrCalculator.setPrincipal(this.get('totalBalance'));
      ibrCalculator.setIncome(this.get('income'));
      ibrCalculator.setDependents(this.get('dependents'));
      ibrCalculator.setState(this.get('state'));
      ibrCalculator.setInterestRate(this.get('interestRate'));
      ibrCalculator.calculate();

      var monthlyPayments = [];
      
      var monthlyInterestRate    = this.get('interestRate') / 12;
      var totalNumberOfPayments  = 120; //10yr term
      var standardPayment = this.solveForMonthlyPaymentFormula(monthlyInterestRate, this.get('totalBalance'), totalNumberOfPayments);

      //First year has custom lowered payment if qualifies, otherwise its standard repayment
      if (ibrCalculator.qualifies()){
        var loweredPayment = ibrCalculator.getMonthlyPayment();
        //If calculation for lowered payment is higher than standard, user doesnt qualify
        if (loweredPayment > standardPayment) {
          ibrCalculator.setQualifies(false);
        } else {
          monthlyPayments.push(ibrCalculator.getMonthlyPayment());
        }
      } 
      //If no first year payment set we need to add in standard amount
      if (monthlyPayments.length === 0) {
        monthlyPayments.push(standardPayment);
      }
      monthlyPayments.push(standardPayment);

      this.set({'monthlyPayment': monthlyPayments});
    },
    getMonthlyPayment: function(monthCount){
      //We have 2 monthly payments. First is for 1 year, other is for following 10 years
      if(monthCount >= 12){
        return this.get('monthlyPayment')[1];
      } else {
        return this.get('monthlyPayment')[0];
      }
    },
    getHeaderCopy: function(){
      //Custom function for IBR repayment, so we can present Do Not Qualify message if needed
      var headerCopy;

      if(ibrCalculator.qualifies()){
        headerCopy = "Based on your income and family size, you'll pay about <strong>" + $.commify(this.getMonthlyPayment(0), { prefix:'$' }) + "</strong> every month for the first year.";
      } else {
        headerCopy = "It appears you don't qualify for the Income Based Repayment Plan. You might want to consider Graduated or Extended Repayment Plans to lower your monthly payment.";
      }
      return headerCopy;
    },
    parse: function(resp,xhr){
      if (resp) {
        if ($.isArray(resp)) {
          var index = -1;
          for (var i = 0; i < resp.length; i++) {
            if (resp[i].IncomeBasedYearlyIncome > 0) {
              index = i;
              break;
            }
          }
          if (index >= 0) {
            resp = resp[index];
          }
        }

        if (resp.IncomeBasedYearlyIncome !== null && resp.IncomeSensitiveYearlyIncome > 0) {
          resp.id = resp.LowerPaymentId;
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
        PlanType: this.get('referenceId')
      };
      this.set(updatedProps,{silent: true});
    }

  });

  
  // Attach the Views sub-module into this module.
  IncomeBased.Views = Views;
  
  // Required, return the module for AMD compliance
  return IncomeBased;

});