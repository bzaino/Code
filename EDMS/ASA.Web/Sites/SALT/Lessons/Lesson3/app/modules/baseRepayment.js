define([
  "lesson3/app",

  // Libs
  "backbone",
  'salt'
],

function(app, Backbone, SALT) {

  // Create a new module
  var BaseRepayment = app.module();

  BaseRepayment.Model = Backbone.Model.extend({
    localStorage: {},
    url: 'localstorage:',
    defaults: {
      /**
        These defaults are specific to each repayment.
      */
      totalBalance  : 10000,
      years         : 10,
      interestRate  : 0.0386,
      monthlyPayment: 0,
      monthlyRepaymentPoints : [],
      yearlyRepaymentPoints : [],
      gracePeriod: '6 Months',
      deferment: '36 Months',
      forbearance: '36 Months'
    },

    initialize: function() {
      var _this = this;
      this.performCalculations();
      SALT.on('interestRate:changed', function (interestRate) {
        _this.setInterestRate(interestRate);
      });
    },
    performCalculations: function(){
      //Run all calculations based on currently-set attributes
      this.calculateMonthlyPayment();
      this.calculateMonthlyBalancePoints();
      this.calculateYearlyDataPoints();
    },
    calculateMonthlyPayment: function (){
      //This method uses the baseRepayment values for calculation, not any child-type overrides
      //Needed so we can reference original payment amount in betterInterest 
      var monthlyInterestRate    = this.get('interestRate') / 12;
      var years = this.get('years');
      var totalNumberOfPayments  = years * 12;

      var newMonthlyPayment = this.solveForMonthlyPaymentFormula(monthlyInterestRate, this.get('totalBalance'), totalNumberOfPayments);
      this.set({'monthlyPayment': newMonthlyPayment});
    },
    solveForMonthlyPaymentFormula: function(monthlyInterestRate, totalBalance, totalNumberOfPayments){
      /**
        calculated using: http://www.myamortizationchart.com/articles/how-is-an-amortization-schedule-calculated/
      */
      var monthlyPaymentNumerator   = monthlyInterestRate * totalBalance * Math.pow(1+ monthlyInterestRate, totalNumberOfPayments);
      var monthlyPaymentDenominator = Math.pow(1 + monthlyInterestRate, totalNumberOfPayments) - 1;
      var newMonthlyPayment = parseFloat((monthlyPaymentNumerator / monthlyPaymentDenominator).toFixed(2));
      return newMonthlyPayment;    
    },
    //monthCount value used to fetch appropriate amount for given month, for repayment options that have varying monthtly payment
    //monthCount is 0-based
    getMonthlyPayment: function(monthCount){
      return this.get('monthlyPayment');
    }, 
    getNumberOfYears: function(){
      //Always round up to next integer
      return Math.ceil(this.get('years'));
    }, // END getNumberOfYears
    getMonthlyInterestRate: function(monthCount){
      //This method is here to support repayment options with dynamic interest rates based on the month of the calculation
      //Returning -1 from here represents a special case, where the balance for the given month doesn't change and 
      //no interest accrues. For Example, year 1 of Income-based
      return this.get('interestRate') / 12;
    }, // END getMonthlyInterestRate()
    setInterestRate: function(interestRate){
      //set the BaseRepayment model interestRate
      // console.log("modelName " + this.attributes.referenceId + " New InterestRate: " + interestRate);
      return this.set('interestRate', interestRate);
    },
    calculateMonthlyBalancePoints: function(){
      var currentBalance = this.get('totalBalance');
      var monthlyRepaymentPoints  = []; //Represents collection of data for monthly payment calculation
      var interestAccrued = 0;
      var years  = this.getNumberOfYears();

      //Insert intial repayment point to show the initial balance
      //monthlyRepaymentPoints.push({principalPayment: 0,monthlyInterest: 0,currentBalance: currentBalance, monthlyPayment: monthlyPayment, interestAccrued: 0});

      var newBalance = 0, principalPayment = 0, monthlyInterest = 0;
      var currentBalanceRounded = currentBalance;
      var monthCount = -1;

      //These two properties are pulled within loop, in order to support dynamic values
      var monthlyPayment = 0;
      var monthlyInterestRate = 0;
      //Calculate monthly payments until balance reaches 0
      while(currentBalanceRounded > 0) {
        monthCount = monthCount + 1;
        monthlyPayment = this.getMonthlyPayment(monthCount);
        monthlyInterestRate = this.getMonthlyInterestRate(monthCount);
        
        if(monthlyInterestRate === -1){
          monthlyInterestRate = 0;
          monthlyInterest= newBalance * monthlyInterestRate;
          //monthlyInterest = Math.round(monthlyInterest * 100) / 100;
          newBalance = currentBalance;
        } else {
          monthlyInterest = currentBalance * Math.pow(( 1 + monthlyInterestRate), 1) - currentBalance;
          //monthlyInterest = Math.round(monthlyInterest * 100) / 100;
          newBalance = currentBalance + monthlyInterest - monthlyPayment;
        }

        principalPayment = Math.round((currentBalance - newBalance) * 100) / 100;
        interestAccrued = interestAccrued + monthlyInterest;
        currentBalance = newBalance;

        currentBalanceRounded = Math.round(currentBalance * 100) / 100;
        //check if value is negative, set to 0 if so
        if(currentBalanceRounded < 0){
          currentBalanceRounded = 0;
        }
        //Add each month's to our repayment array
        monthlyRepaymentPoints.push({principalPayment: principalPayment,monthlyInterest: monthlyInterest,currentBalance: currentBalanceRounded, monthlyPayment: monthlyPayment, interestAccrued: interestAccrued});
      }

      this.checkForMonthly0Balance(monthCount);

      this.set({'monthlyRepaymentPoints': monthlyRepaymentPoints});
      this.calculateYearlyDataPoints();
    },
    checkForMonthly0Balance: function(monthNum){ 
       //monthNum is 0-indexed
       //Do nothing in base implementation
    },
    calculateYearlyDataPoints: function() {
      //Requires that calculateMonthlyBalancePoints be run before this when attributes changed
      var monthlyRepaymentPoints = this.get('monthlyRepaymentPoints');
      var yearlyRepaymentDataPoints = [];
       
       
       //Push initial state of payment
      yearlyRepaymentDataPoints.push(monthlyRepaymentPoints[0]);
       
       
      for(i=1; i < monthlyRepaymentPoints.length; i++) {
        var remainder = (i % 12) / 100;
        if ((remainder === 0) || (i === monthlyRepaymentPoints.length -1)) {
          //Push data points every 12 months, as well as last month if mid year
          yearlyRepaymentDataPoints.push(monthlyRepaymentPoints[i]);
        }
      }
     
      this.set({'yearlyRepaymentPoints': yearlyRepaymentDataPoints});
     
    }, // END calculateYearlyDataPoints()
    resetToDefaults: function(){
      //This method is used by child types to reset custom values to defaults
      //Do nothing in base, since it only uses defaults
    },
    setTotalBalance: function(balance){
      this.set({'totalBalance': balance});
      
      this.performCalculations();
    },
    calculateAveragePaymentYears1to2: function(){
      var averagePayment;
      var paymentSum = 0;
      var repaymentPoints = this.get('monthlyRepaymentPoints');
      var monthCount = 0; //Saves number of months iterated over, for denominator value, in case payment ended early
      
      for(var i = 0; i < repaymentPoints.length; i++){
        if(typeof repaymentPoints[i] !== 'undefined'){
          paymentSum = paymentSum + repaymentPoints[i].monthlyPayment;
          monthCount++;
        }
      }
      
      averagePayment = paymentSum / monthCount;
      
      if(averagePayment <= 0 || isNaN(averagePayment)){
        averagePayment = 'N/A';
      }
      
      return averagePayment;
    },
    calculateAveragePaymentYears3to5: function(){
      var averagePayment;
      var paymentSum = 0;
      var repaymentPoints = this.get('monthlyRepaymentPoints');
      var monthCount = 0; //Saves number of months iterated over, for denominator value, in case payment ended early
      
      for(var i = 25; i < repaymentPoints.length; i++){
        if(typeof repaymentPoints[i] !== 'undefined'){
          paymentSum = paymentSum + repaymentPoints[i].monthlyPayment;
          monthCount++;
        }
      }
      
      averagePayment = paymentSum / monthCount;
      
      if(averagePayment <= 0 || isNaN(averagePayment)){
        averagePayment = 'N/A';
      }
      
      return averagePayment;
    },
    calculateAveragePaymentYears6to10: function(){
      var averagePayment;
      var paymentSum = 0;
      var repaymentPoints = this.get('monthlyRepaymentPoints');
      var monthCount = 0; //Saves number of months iterated over, for denominator value, in case payment ended early
      
      for(var i = 61; i < repaymentPoints.length; i++){
        if(typeof repaymentPoints[i] !== 'undefined'){
          paymentSum = paymentSum + repaymentPoints[i].monthlyPayment;
          monthCount++;
        }
      }
      
      averagePayment = paymentSum / monthCount;
      
      if(averagePayment <= 0 || isNaN(averagePayment)){
        averagePayment = 'N/A';
      }
      
      return averagePayment;
    },
    calculateAveragePaymentYears11to15: function(){
      var averagePayment;
      var paymentSum = 0;
      var repaymentPoints = this.get('monthlyRepaymentPoints');
      var monthCount = 0; //Saves number of months iterated over, for denominator value, in case payment ended early 
      
      for(var i = 121; i < repaymentPoints.length; i++){
        if(typeof repaymentPoints[i] !== 'undefined'){
          paymentSum = paymentSum + repaymentPoints[i].monthlyPayment;
          monthCount++;
        }
        
      }
      
      averagePayment = paymentSum / monthCount;
      
      if(averagePayment <= 0 || isNaN(averagePayment)){
        averagePayment = 'N/A';
      }
      
      return averagePayment;
    }
  });

  // Required, return the module for AMD compliance
  return BaseRepayment;

});
