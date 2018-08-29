// Adapted from: http://studentaid.ed.gov/repay-loans/understand/plans/standard/comparison-calculator
// see: http://studentaid.ed.gov/sites/all/themes/ise/js/calculator.js
define(function () {

  var interestRate = 0.0386,
      monthlyPayments = [],
      totalInterestPaid = 0,
      loanAmount = 0,
      loanTerm = 120,
      monthsInRepayment = loanTerm;

  function getMonthsInRepayment() {
    return monthsInRepayment;
  }

  function getTotalInterestPaid() {
    return parseFloat(totalInterestPaid);
  }

  function getMonthlyPayments() {
    return monthlyPayments;
  }

  function calculate() {
    return calculateGraduated(loanTerm);
  }

  function setInterestRate(newInterestRate) {
    interestRate = newInterestRate;
  }

  function setLoanAmount(newLoanAmount) {
    loanAmount = newLoanAmount;
  }

  function getInterestRate() {
    return interestRate;
  }

  function getLoanAmount() {
    return loanAmount;
  }

  function formatCurrency(num) {
    num = isNaN(num) || num === '' || num === null ? 0.00 : num;
    return '$' + parseFloat(num).toFixed(2);
  }

  // The initial monthly payment cannot be less than $25, nor less than 50% of the
  // 10 year monthly payment, nor less than one month of interest, or BI
  function calInitGraduatedPayment(loanAmount, monthlyInterest) {
    var payment = calculatePayment(loanAmount, monthlyInterest, 120);
    var interestOnly = loanAmount * monthlyInterest;

    return Math.max(25, (payment * 0.5), interestOnly);
  }

  function calculatePayment(loanAmount, monthlyInterest, termMonths) {
    var payment = (loanAmount * monthlyInterest) / (1 -  Math.pow((monthlyInterest + 1), -termMonths));
    return payment;
  }

  //copied from www.myfedloan.org/scripts/repayment-calculators.js 2015-03-26
  /**
   * Graduated Repayment [revised - working copy 11/14/2012]
   * @description : Graudated repayment calculation
   * @parameter(aArguments) : Array of parameters for calculation
   * @parameter(sType) : String to toggle "extended graduated" vs. "graduated" repayment
   */

  function calculateGraduated(loanTerm){
    var iTotalPaymentAmount = 0, 
    iLevels = 5;
      
    var interestRate = getInterestRate(),
        monthlyRate = interestRate / 12;

    var standardMonthlyPayment = calculatePayment(loanAmount, monthlyRate, loanTerm);
    standardMonthlyPayment = standardMonthlyPayment <= 50 ? 50: standardMonthlyPayment;
    
    var principalBalance = getLoanAmount(),
        tempPrincipalBalance = 0, 
        interestPerMonth = 0, 
        incrementPercent = 1.2437673130193905817174515235457, 
        initialPayment = Math.max(5, standardMonthlyPayment / 2, principalBalance * monthlyRate),
        holdInitialPayment = initialPayment,
        firstPayment = initialPayment, 
        secondPayment = firstPayment * incrementPercent, 
        thirdPayment = secondPayment * incrementPercent,
        fourthPayment = thirdPayment * incrementPercent, 
        fifthPayment = fourthPayment * incrementPercent,
        
        incrementFactor = 0, 
        prevIncrementFactor = 0, 
        threeTimesPayment = initialPayment * 3, 
        amountChange = fifthPayment, 
        lastGradPaymentAmount = fifthPayment, 
        
        populateLevels = function () {
            var payments = new Array(firstPayment, secondPayment, thirdPayment, fourthPayment, fifthPayment);
            return payments;
          },
        
        i = 1;
        
    while (i < 99) {
          
      var levels = populateLevels();
      
      tempPrincipalBalance = principalBalance;
      
      // Step through levels;
      for (var j = 0; j < iLevels; j++) {
        var month = 1;
        
        // Step through payments per level;
        while (month <= 24) {
          interestPerMonth = monthlyRate * tempPrincipalBalance;
          tempPrincipalBalance = tempPrincipalBalance + interestPerMonth - levels[j];
          month += 1;
        }
      }
          
      if (parseInt(tempPrincipalBalance, 10) <= 5 && parseInt(tempPrincipalBalance, 10) >= -5) {
        break;
      }
      
      if (parseInt(tempPrincipalBalance, 10) > 5) {
        incrementFactor = 1;
      } else if (parseInt(tempPrincipalBalance, 10) < -5) {
        incrementFactor = -1;
      }
          
      if (((incrementFactor > 0) && (prevIncrementFactor > 0)) || ((incrementFactor < 0) && (prevIncrementFactor < 0))) {
          // Do nothing;
      } else {
        amountChange = amountChange / 2;
      }
          
      prevIncrementFactor = incrementFactor;
      lastGradPaymentAmount = lastGradPaymentAmount + (amountChange * incrementFactor);
      
      var power = 1 / 4;
      var base = lastGradPaymentAmount / firstPayment;
      
      incrementPercent = Math.pow(base, power);
      firstPayment = initialPayment * incrementPercent;
      secondPayment = firstPayment * incrementPercent;
      thirdPayment = secondPayment * incrementPercent;
      fourthPayment = thirdPayment * incrementPercent;
      fifthPayment = fourthPayment * incrementPercent;
      
      threeTimesPayment = firstPayment * 3;
      
      if (threeTimesPayment > fifthPayment) {
        initialPayment -= 1;
      } else {
        initialPayment += 1;
      }
      
      i = i + 1;
      
    }

    console.log('NewCalc-firstPayment', firstPayment);
    console.log('NewCalc-secondPayment', secondPayment);
    console.log('NewCalc-thirdPayment', thirdPayment);
    console.log('NewCalc-fourthPayment', fourthPayment);
    console.log('NewCalc-fifthPayment', fifthPayment);
    
    if (firstPayment === 0) {
      firstPayment = holdInitialPayment;
    }
    // SWD-7733 own-your-student-loans lesson system hangs when enter balance=$1000
    // The second-fifth payments should always have a minimum payment
    if (secondPayment === 0) {
      secondPayment = holdInitialPayment;
    }
    if (thirdPayment === 0) {
      thirdPayment = holdInitialPayment;
    }
    if (fourthPayment === 0) {
      fourthPayment = holdInitialPayment;
    }
    if (fifthPayment === 0) {
      fifthPayment = holdInitialPayment;
    }

    var results = [];
    results[0]= firstPayment;
    results[1]= secondPayment;
    results[2]= thirdPayment;
    results[3]= fourthPayment;
    results[4]= fifthPayment;
    monthlyPayments = results;

    if( parseInt(firstPayment, 10) == parseInt(25, 10) ){
      var dRealTerm = (Math.log(firstPayment) - Math.log (firstPayment - (loanAmount * monthlyRate))) / Math.log (monthlyRate + 1);

      if(Math.ceil(dRealTerm) <= loanTerm) {
        loanTerm = Math.ceil(dRealTerm);
        monthsInRepayment = parseInt(loanTerm, 10);
      }
    }

    console.log('NewCalc-monthlyPayments[0]', monthlyPayments[0]);
    console.log('NewCalc-monthlyPayments[1]', monthlyPayments[1]);
    console.log('NewCalc-monthlyPayments[2]', monthlyPayments[2]);
    console.log('NewCalc-monthlyPayments[3]', monthlyPayments[3]);
    console.log('NewCalc-monthlyPayments[4]', monthlyPayments[4]);

    iTotalPaymentAmount += firstPayment;

    return iTotalPaymentAmount;
  }

  function roundNumber(number, places){
    return Math.round(number*Math.pow(10, places))/Math.pow(10, places);
  }

  return {
    calculate: calculate,
    setInterestRate: setInterestRate,
    setLoanAmount: setLoanAmount,
    getLoanAmount: getLoanAmount,
    getTotalInterestPaid: getTotalInterestPaid,
    getMonthlyPayments: getMonthlyPayments,
    getMonthsInRepayment: getMonthsInRepayment
  };
});
