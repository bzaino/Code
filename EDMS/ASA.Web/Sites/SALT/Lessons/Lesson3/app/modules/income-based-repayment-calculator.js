// Adapted from: http://studentaid.ed.gov/repay-loans/understand/plans/income-based/calculator
// see: http://studentaid.ed.gov/sites/all/themes/ise/js/ibr-calculator.js
define(function () {
 
  var qualifies = false,
      interestRate = 3.86,
      dependents = 1,
      principal = 30000,
      income = 24000,
      state = 'other',
      paymentPerMonth = 0;

  function getQualifies() {
    return qualifies;
  }

  function getMonthlyPayment() {
    return paymentPerMonth;
  }

  function setInterestRate(newInterestRate) {
    interestRate = newInterestRate;
  }

  function setIncome(newIncome) {
    income = newIncome;
  }

  function setState(newState) {
    state = newState;
  }

  function setPrincipal(newPrincipal) {
    principal = newPrincipal;
  }

  function setDependents(newDependents) {
    dependents = newDependents;
  }

  function setQualifies(newQualifies) {
    qualifies = newQualifies;
  }

  function calculate(){
    log("Start Calculating");
    var payment = getTotalAnnualPayment();
    log("Annual Payment: " + payment);

    var povertyGuideline = getPovertyAmount(getDependents(), getState());

    log("Poverty: " + povertyGuideline);

    var povertyAmount = povertyGuideline * 1.5;
    log("PovertyAmt: " + povertyAmount);

    var totalIncome = getTotalIncome();
    log("Total Income: " + totalIncome);

    var discretionaryIncome = totalIncome - povertyAmount;
    log("Discretionary Income: " + discretionaryIncome);
    
    //round to 2 decimal places.
    var monthlyPayment = roundNumber(((discretionaryIncome * 0.15)/12), 2);
    if (monthlyPayment <= 0){
      monthlyPayment = 0;
    } else {
      if (monthlyPayment <= 5){
        monthlyPayment = 5;
      } else {
        if (monthlyPayment > 5 && monthlyPayment <= 10) {
          monthlyPayment = 10;
        }
      }
    }
    log("Monthly Payment: " + monthlyPayment);

    var qualify;

    log("Yearly Payment: " + (monthlyPayment * 12) + " discretionary: " + (discretionaryIncome * 0.15));
    
    if( (payment * 12) > (discretionaryIncome * 0.15) ) {
      qualify = true;
    } else {
      qualify = false;
    } 

    log("Qualify: " + qualify);
    showResults(qualify, monthlyPayment);
  }

  function showResults(qualify, monthlyPayment){
    log("Qualify: " + qualify + " Payment: " + monthlyPayment);
    qualifies = qualify;
    if(qualify){
      var ratio = getIncomeRatio();
      //round to 2 decimal places.
      var yourPayment = roundNumber((monthlyPayment * ratio), 2);
      
      paymentPerMonth = yourPayment;

      log( "your payment: " + yourPayment );
    } else {
      log( "the no qualify payment: " + getTotalAnnualPayment() );
    }   
  }

  function roundNumber(number, places){
    return Math.round(number*Math.pow(10, places))/Math.pow(10, places);
  }

  function getDependents(){
    return parseInt(dependents, 10);
  }

  function getState(){
    return state;
  }

  function getIncomeRatio(){
    return 1.0;
  }

  function getTotalAnnualPayment(){
    return getUserAnnualPayment();
  }

  function getTotalIncome(){
    return income;
  }

  function getUserLoanAmount() {
    return principal;
  }

  function getUserAnnualPayment(){
    var amount = getUserLoanAmount();
    return getAnnualPayment(amount, interestRate);
  }

  function getAnnualPayment(loanAmount, interestRate){
    log('raw: "' + loanAmount + '" "' + interestRate + '"');
    
    if(isNaN(loanAmount)) { loanAmount = 0.0; }
    loanAmount = parseFloat(loanAmount);

    if(isNaN(interestRate)) { interestRate = 3.86; }
    interestRate = parseFloat(interestRate);

    if(interestRate > 1.0){
      interestRate = interestRate / 100;
    }

    log("Amt: " + loanAmount + " Rate: " + interestRate);

    var amount = loanAmount;
    
    log("Greater Amt: " + amount);

    if(amount === 0){ return 0; }

    var moIntRate = interestRate / 12;
    var defaultTerm = 120; // 10 years
    var payment = 0;

    if( moIntRate === 0 ){
      payment = amount / defaultTerm; 
    } else {
      payment = amount * moIntRate / (1-(1/Math.pow(1 + moIntRate, defaultTerm)));
    }

    log("Annual Payment: " + payment);

    return payment;
  }

  function getPovertyAmount(dependents, state){
    // HHS Poverty Guidelines for 2017
    //taken from https://aspe.hhs.gov/poverty-guidelines
    var povertyGuidelines = [];
    povertyGuidelines['other'] = [];
    povertyGuidelines['other']['1'] = 12140;
    povertyGuidelines['other']['2'] = 16460;
    povertyGuidelines['other']['3'] = 20780;
    povertyGuidelines['other']['4'] = 25100;
    povertyGuidelines['other']['5'] = 29420;
    povertyGuidelines['other']['6'] = 33740;
    povertyGuidelines['other']['7'] = 38060;
    povertyGuidelines['other']['8'] = 42380;
    povertyGuidelines['other']['extra'] = 4320 ;
    povertyGuidelines['AK'] = [];
    povertyGuidelines['AK']['1'] = 15180;
    povertyGuidelines['AK']['2'] = 20580;
    povertyGuidelines['AK']['3'] = 25980;
    povertyGuidelines['AK']['4'] = 31380;
    povertyGuidelines['AK']['5'] = 36780;
    povertyGuidelines['AK']['6'] = 42180;
    povertyGuidelines['AK']['7'] = 47580;
    povertyGuidelines['AK']['8'] = 52980;
    povertyGuidelines['AK']['extra'] = 5400 ;
    povertyGuidelines['HI'] = [];
    povertyGuidelines['HI']['1'] = 13960;
    povertyGuidelines['HI']['2'] = 18930;
    povertyGuidelines['HI']['3'] = 23900;
    povertyGuidelines['HI']['4'] = 28870;
    povertyGuidelines['HI']['5'] = 33840;
    povertyGuidelines['HI']['6'] = 38810;
    povertyGuidelines['HI']['7'] = 43780;
    povertyGuidelines['HI']['8'] = 48750;
    povertyGuidelines['HI']['extra'] = 4970 ;

    var amount = 0;
    if(dependents > 8){
      var addSize = dependents - 8;
      amount = povertyGuidelines[state]['8'];
      var extra = addSize * povertyGuidelines[state].extra;

      amount += extra;
    } else {
      amount = povertyGuidelines[state][dependents.toString()];
    }
    return amount;  
  }

  function log(msg){
    console.log(msg);
  }

  function formatCurrency(num) {
    log("Format Currency: " + num + " parsed: " + parseFloat(num).toFixed(2));
    num = isNaN(num) || num === '' || num === null ? 0.00 : num;
    return '$' + parseFloat(num).toFixed(2);
  }

  function getPercent(txtField){
    var reg = /^(\d{0,2}(?:\.\d*)?%?$)/;
    var val =  $('input:text[name=' + txtField + ']').val();
     
    if($.trim(val) === ''){
      return -1;
    }
    if(!reg.test(val)){
      return -1;
    }

    val = val.replace('%', '');
    val = parseFloat(val);
    if(val < 0){ val = abs(val); }
     
    return val;
  }

  return {
    calculate: calculate,
    setInterestRate: setInterestRate,
    setPrincipal: setPrincipal,
    setIncome: setIncome,
    setDependents: setDependents,
    setState: setState,
    qualifies: getQualifies,
    setQualifies: setQualifies,
    getMonthlyPayment: getMonthlyPayment
  };

});