/*global suite, setup, chai, test, mocha */
define([
	'modules/paymentCalculator'
], function(paymentCalculator) {

	var assert = chai.assert;

	suite('Loan Payment Calculator', function () {
		suite('Monthly payment for interest: 4.45%', function(){
			var planType;
			var loansArray;
			var model;
			setup(function(){
				planType = 'Standard';
				loansArray = [{
					InterestRate: 4.45,
					LoanSource: 'Member',
					LoanStatusId: '',
					LoanTerm: '10',
					LoanType: 'federal',
					LoanTypeId: 'CL',
                    MonthlyPaymentAmount: 41.0,
					OriginalLoanAmount: 15000,
					PrincipalBalanceOutstandingAmount: 15000,
					ReceivedYear: 0,
					RecordSourceId: 2
				}];
                loansArray2 = [{
                    InterestRate: 4.45,
                    LoanSource: 'Member',
                    LoanStatusId: '',
                    LoanTerm: '10',
                    LoanType: 'federal',
                    LoanTypeId: 'CL',
                    MonthlyPaymentAmount: 1.0,
                    OriginalLoanAmount: 15000,
                    PrincipalBalanceOutstandingAmount: 15000,
                    ReceivedYear: 0,
                    RecordSourceId: 2
                }];
				model = {
					AmountBorrowed: 4000,
					DaysSinceLastPayment: 'N/A',
					EnrollmentStatus: null,
					EstimatedLoanBalance: null,
					GraduationDate: null,
					AdjustedGrossIncome: 2000,
					MemberID: 1405706,
					MostRecentPaymentDate: null,
					FamilySize: 2,
					Plan: 'Income Based',
					RepaymentStatus: null,
					finalMonthlyPayment: 50,
					initialMonthlyPayment: 50
				};
			});
			// Standard Plan Tests
			suite('Plan: Standard', function(){
				test('Loan amount: $4000 should equal $50', function(){
					loansArray[0].OriginalLoanAmount = 4000;
					var payments = paymentCalculator.calculationChooser(planType, loansArray);
					assert.equal(50, Math.floor(payments.initial));
				});
				test('Loan amount: $15000 should equal $50', function(){
					var payments = paymentCalculator.calculationChooser(planType, loansArray);
					assert.equal(50, Math.floor(payments.initial));
				});
                test('Loan amount: $30000 should equal $50', function(){
                    var twoloans = loansArray.concat(loansArray2);
                    var payments = paymentCalculator.calculationChooser(planType, twoloans);
                    assert.equal(50, Math.floor(payments.initial));
                });
				test('Loan amount: $30000 should equal $51', function(){
					loansArray2[0].MonthlyPaymentAmount = 10;
                    var twoloans = loansArray.concat(loansArray2);
                    var payments = paymentCalculator.calculationChooser(planType, twoloans);
                    assert.equal(51, Math.floor(payments.initial));
                });
                test('Loan amount: $30000 should equal $150', function(){
                    loansArray2[0].MonthlyPaymentAmount = 109;
                    var twoloans = loansArray.concat(loansArray2);
                    var payments = paymentCalculator.calculationChooser(planType, twoloans);
                    assert.equal(150, Math.floor(payments.initial));
                });
			});
			// Graduated Plan Tests
			suite('Plan: Graduated', function(){
				test('Loan amount: $4000, initial should equal $15', function(){
					loansArray[0].OriginalLoanAmount = 4000;
					planType = 'Graduated';
					var payments = paymentCalculator.calculationChooser(planType, loansArray);
					assert.equal(14, Math.floor(payments.initial));
				});
				test('Loan amount: $15000, initial should equal $58', function(){
					loansArray[0].OriginalLoanAmount = 15000;
					planType = 'Graduated';
					var payments = paymentCalculator.calculationChooser(planType, loansArray);
					assert.equal(55, Math.floor(payments.initial));
				});
			});
			// Extended Plan Tests
			suite('Plan: Extended', function(){
				test('Loan amount: $4000, initial should equal $22', function(){
					loansArray[0].OriginalLoanAmount = 4000;
					planType = 'Extended';
					var payments = paymentCalculator.calculationChooser(planType, loansArray);
					assert.equal(22, Math.floor(payments.initial));
				});
				test('Loan amount: $15000, initial should equal $84', function(){
					loansArray[0].OriginalLoanAmount = 15000;
					planType = 'Extended';
					var payments = paymentCalculator.calculationChooser(planType, loansArray);
					assert.equal(82, Math.floor(payments.initial));
				});
			});

			// Income Based Plan Tests
			suite('Plan: Income Based', function(){
				test('Loan amount: $4000, Monthly income: $1200, Household count: 1, initial should equal $0', function(){
					loansArray[0].OriginalLoanAmount = 4000;
					planType = 'Income Based';
					model.AdjustedGrossIncome = 14400;
					model.FamilySize = 1;
					model.TaxFilingStatus = 'S';
					model.StateOfResidence = 'OTHER';
					var payments = paymentCalculator.calculationChooser(planType, loansArray, model);
					assert.equal(0, Math.floor(payments.initial));
				});
				test('Loan amount: $4000, Monthly income: $1500, Household count: 1, initial should equal $10', function(){
					loansArray[0].OriginalLoanAmount = 4000;
					planType = 'Income Based';
					model.AdjustedGrossIncome = 19000;
					model.FamilySize = 1;
					model.TaxFilingStatus = 'S';
					model.StateOfResidence = 'OTHER';
					var payments = paymentCalculator.calculationChooser(planType, loansArray, model);
					assert.equal(10, Math.floor(payments.initial));
				});
				test('Loan amount: $15000, , Monthly income: $2040, Household count: 2, initial should equal $0', function(){
					loansArray[0].OriginalLoanAmount = 15000;
					planType = 'Income Based';
					model.AdjustedGrossIncome = 24480;
					model.FamilySize = 2;
					model.TaxFilingStatus = 'S';
					model.StateOfResidence = 'OTHER';
					var payments = paymentCalculator.calculationChooser(planType, loansArray, model);
					assert.equal(0, Math.floor(payments.initial));
				});
                test('Loan amount: $15000, , Monthly income: $5000, Household count: 3, initial should equal $50', function(){
                    var twoloans = loansArray.concat(loansArray2);
                    planType = 'Income Based';
                    model.AdjustedGrossIncome = 60000;
                    model.FamilySize = 3;
                    model.TaxFilingStatus = 'S';
                    model.StateOfResidence = 'OTHER';
                    var payments = paymentCalculator.calculationChooser(planType, twoloans, model);
                    assert.equal(50, Math.floor(payments.initial));
                });
				test('Loan amount: $15000, , Monthly income: $2040, Household count: 2, initial should equal $5', function(){
					loansArray[0].OriginalLoanAmount = 15000;
					planType = 'Income Based';
					model.AdjustedGrossIncome = 22000;
					model.FamilySize = 1;
					model.TaxFilingStatus = 'S';
					model.StateOfResidence = 'HI';
					var payments = paymentCalculator.calculationChooser(planType, loansArray, model);
					assert.equal(13, Math.floor(payments.initial));
				});
            });
		});
	});
	suite('CCP Loan Payment Calculator', function () {
		var loanAmount, 
		interestRate,
		undergradRate = '.0445',
		gradRate = '.06',
		plusLoanRate = '.07',
		privateLoanRate = '.1',
		totalNumberOfPayments;
		suite('Federal Student Loans, Undergrad: Interest Rate: 4.45% ', function(){
			test('Loan amount: $4000 should equal $50', function(){
				loanAmount = 4000;
				interestRate = undergradRate;
				totalNumberOfPayments = 120;
				var payments = paymentCalculator.calculateSimpleAmortizedPayment(interestRate, loanAmount, totalNumberOfPayments);
				assert.equal(50, Math.round(payments.monthly));
			});
			test('Loan amount: $15000 should equal $150', function(){
				loanAmount = 15000;
				interestRate = undergradRate;
				totalNumberOfPayments = 120;
				var payments = paymentCalculator.calculateSimpleAmortizedPayment(interestRate, loanAmount, totalNumberOfPayments);
				assert.equal(155, Math.round(payments.monthly));
			});
		});
		suite('Federal Student Loans, Graduate Student: Interest Rate: 6% ', function(){
			test('Loan amount: $15000 should equal $161', function(){
				loanAmount = 15000;
				interestRate = gradRate;
				totalNumberOfPayments = 120;
				var payments = paymentCalculator.calculateSimpleAmortizedPayment(interestRate, loanAmount, totalNumberOfPayments);
				assert.equal(167, Math.round(payments.monthly));
			});
		});		
		suite('Federal Parent PLUS Loans: Interest Rate: 7% ', function(){
			test('Loan amount: $15000 should equal $169', function(){
				loanAmount = 15000;
				interestRate = plusLoanRate;
				totalNumberOfPayments = 120;
				var payments = paymentCalculator.calculateSimpleAmortizedPayment(interestRate, loanAmount, totalNumberOfPayments);
				assert.equal(174, Math.round(payments.monthly));
			});
		});
		suite('Private Student Loans: Interest Rate: 10% ', function(){
			test('Loan amount: $15000 should equal $198', function(){
				loanAmount = 15000;
				interestRate = privateLoanRate;
				totalNumberOfPayments = 120;
				var payments = paymentCalculator.calculateSimpleAmortizedPayment(interestRate, loanAmount, totalNumberOfPayments);
				assert.equal(198, Math.round(payments.monthly));
			});
		});		
	});
});