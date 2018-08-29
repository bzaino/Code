  /*global describe, beforeEach,it, afterEach, chai, sinon, done*/
define([
    'modules/CCP/CCPModels',
    'modules/CCP/LoanLimits'
], function(Models, LoanLimits) {
    var assert = chai.assert;
    var graduate = 0,
        freshman = 1,
        sophomore = 2,
        junior = 3,
        senior = 4,
        invalidGrade= 10;
    suite('College Cost Planner - Loan Limits file', function () {
        suite('getMaxYearsToFund', function(){
            test('Graduate max years to fund should equal 4', function(){
                var maxYearsToFund = LoanLimits.getMaxYearsToFund(graduate);
                assert.equal(4, maxYearsToFund);
            });
            test('Freshman max years to fund should equal 6', function(){
                var maxYearsToFund = LoanLimits.getMaxYearsToFund(freshman);
                assert.equal(6, maxYearsToFund);
            });
            test('Sophomore max years to fund should equal 5', function(){
                var maxYearsToFund = LoanLimits.getMaxYearsToFund(sophomore);
                assert.equal(5, maxYearsToFund);
            });
            test('Junior max years to fund should equal 4', function(){
                var maxYearsToFund = LoanLimits.getMaxYearsToFund(junior);
                assert.equal(4, maxYearsToFund);
            });
            test('Senior max years to fund should equal 3', function(){
                var maxYearsToFund = LoanLimits.getMaxYearsToFund(senior);
                assert.equal(3, maxYearsToFund);
            });
            test('Invalid grade level max years to fund should equal 0', function(){
                var maxYearsToFund = LoanLimits.getMaxYearsToFund(invalidGrade);
                assert.equal(0, maxYearsToFund);
            });
        });
        suite('getMaxFederalAmountByGradeLevelByYear', function(){
            suite('Grade Level - Graduate', function(){
                test('Amount for 1 year to fund should equal 20500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(graduate, 1);
                    assert.equal(20500, maxAmount);
                });
                test('Amount for 2 years to fund should equal 41000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(graduate, 2);
                    assert.equal(41000, maxAmount);
                });
                test('Amount for 3 years to fund should equal 61500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(graduate, 3);
                    assert.equal(61500, maxAmount);
                });
                test('Amount for 4 years to fund should equal 82000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(graduate, 4);
                    assert.equal(82000, maxAmount);
                });
                test('Amount for 5 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(graduate, 5);
                    assert.equal(0, maxAmount);
                });
                test('Aamount for 6 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(graduate, 6);
                    assert.equal(0, maxAmount);
                });
            });
            suite('Grade Level - Freshman', function(){
                test('Amount for 1 year to fund should equal 5500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(freshman, 1);
                    assert.equal(5500, maxAmount);
                });
                test('Amount for 2 years to fund should equal 12000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(freshman, 2);
                    assert.equal(12000, maxAmount);
                });
                test('Amount for 3 years to fund should equal 19500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(freshman, 3);
                    assert.equal(19500, maxAmount);
                });
                test('Amount for 4 years to fund should equal 27000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(freshman, 4);
                    assert.equal(27000, maxAmount);
                });
                test('Amount for 5 years to fund should equal 31000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(freshman, 5);
                    assert.equal(31000, maxAmount);
                });
                test('Aamount for 6 years to fund should equal 31000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(freshman, 6);
                    assert.equal(31000, maxAmount);
                });
            });
            suite('Grade Level - Sophomore', function(){
                test('Amount for 1 year to fund should equal 6500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(sophomore, 1);
                    assert.equal(6500, maxAmount);
                });
                test('Amount for 2 years to fund should equal 14000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(sophomore, 2);
                    assert.equal(14000, maxAmount);
                });
                test('Amount for 3 years to fund should equal 21500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(sophomore, 3);
                    assert.equal(21500, maxAmount);
                });
                test('Amount for 4 years to fund should equal 29000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(sophomore, 4);
                    assert.equal(29000, maxAmount);
                });
                test('Amount for 5 years to fund should equal 31000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(sophomore, 5);
                    assert.equal(31000, maxAmount);
                });
                test('Amount for 6 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(sophomore, 6);
                    assert.equal(0, maxAmount);
                });
            });
            suite('Grade Level - Junior', function(){
                test('Amount for 1 year to fund should equal 7500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(junior, 1);
                    assert.equal(7500, maxAmount);
                });
                test('Amount for 2 years to fund should equal 15000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(junior, 2);
                    assert.equal(15000, maxAmount);
                });
                test('Amount for 3 years to fund should equal 22500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(junior, 3);
                    assert.equal(22500, maxAmount);
                });
                test('Amount for 4 years to fund should equal 30000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(junior, 4);
                    assert.equal(30000, maxAmount);
                });
                test('Amount for 5 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(junior, 5);
                    assert.equal(0, maxAmount);
                });
                test('Amount for 6 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(junior, 6);
                    assert.equal(0, maxAmount);
                });
            });
            suite('Grade Level - Senior', function(){
                test('Amount for 1 year to fund should equal 7500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(senior, 1);
                    assert.equal(7500, maxAmount);
                });
                test('Amount for 2 years to fund should equal 15000', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(senior, 2);
                    assert.equal(15000, maxAmount);
                });
                test('Amount for 3 years to fund should equal 22500', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(senior, 3);
                    assert.equal(22500, maxAmount);
                });
                test('Amount for 4 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(senior, 4);
                    assert.equal(0, maxAmount);
                });
                test('Amount for 5 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(senior, 5);
                    assert.equal(0, maxAmount);
                });
                test('Amount for 6 years to fund should equal 0', function(){
                    var maxAmount = LoanLimits.getMaxFederalAmountByGradeLevelByYear(senior, 6);
                    assert.equal(0, maxAmount);
                });
            });
        });
        suite('calculateMaxLoanAmountValues', function(){
            var userInfo = new Models.UserInfoModel({});
            
            //setting userInfo will trigger the model methods to fire and calculate totals 12000
            test('Freshman with 2 years to fund with $18,500 plus interest still needed should equal $12,000', function(){
                userInfo.set ({costOfAttendancePerYear: 9250, gradeLevel: 1, yearsRemaining: 2})
                assert.equal(12000, userInfo.get('maxFsl'));
                assert.equal(6870, userInfo.get('maxFppl'));
                assert.equal(18870, userInfo.get('loansTotal'));
            });
            test('Sophomore with 4 years to fund with $8,400 plus interest still needed should equal $8,917', function(){
                userInfo.set ({costOfAttendancePerYear: 2100, gradeLevel: 2, yearsRemaining: 4})
                assert.equal(8917, userInfo.get('maxFsl'));
                assert.equal(0, userInfo.get('maxFppl'));
                assert.equal(8917, userInfo.get('loansTotal'));
            });
            test('Junior with 2 years to fund with $4,200 plus interest, grant for totla amount still needed should equal $0', function(){
                userInfo.set ({costOfAttendancePerYear: 2100, gradeLevel: 3, grantsPerYear:2143, yearsRemaining: 2})
                assert.equal(0, userInfo.get('maxFsl'));
                assert.equal(0, userInfo.get('maxFppl'));
                assert.equal(0, userInfo.get('loansTotal'));
            });
        });
        suite('calculateLoanDisplayTotals - steppers', function(){
            var userInfo = new Models.UserInfoModel({});
            var formCurrentValues = new Models.LoansModel({});
            suite('When loan type is FSL', function(){
                suite('remaining costs are $52,424 and maxFsl is $21,500', function(){
                    suite('and Form values are FSL - $12,424, FPPL - $7,000 & Other - $33000', function(){
                        //setting userInfo values here should not trigger model updates
                         test('And enter 50,000 into FSL, FSL should be $21,500, FPPL should be $0 and Other should be $30,924', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 52424, maxFsl: 21500});
                            formCurrentValues.set ({fsl: 12424, fppl: 7000, other: 33000});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 37576);
                            assert.equal(21500, displayValues.fsl, 'FSL should be 21500');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(30924, displayValues.other, 'Other should be 30924');
                        });
                    });
                    suite('and Form values are FSL - $12,424, FPPL - $7,000 & Other - $33000', function(){
                        //setting userInfo values here should not trigger model updates
                         test('And enter 112,424 into FSL, FSL should be $21,500, FPPL should be $0 and Other should be $30,924', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 52424, maxFsl: 21500});
                            formCurrentValues.set ({fsl: 12424, fppl: 7000, other: 33000});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 100000);
                            assert.equal(21500, displayValues.fsl, 'FSL should be 21500');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(30924, displayValues.other, 'Other should be 30924');
                        });
                    });
                    suite('and Form values are FSL - $12,424, FPPL - $7,000 & Other - $33000', function(){
                        //setting userInfo values here should not trigger model updates
                         test('And enter 21,000 into FSL, FSL should be $21,000, FPPL should be $0 and Other should be $31,424', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 52424, maxFsl: 21500});
                            formCurrentValues.set ({fsl: 12424, fppl: 7000, other: 33000});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 8576);
                            assert.equal(21000, displayValues.fsl, 'FSL should be 21000');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(31424, displayValues.other, 'Other should be 31424');
                        });
                    });
                    suite('and Form values are FSL - $12,424, FPPL - $7,000 & Other - $33000', function(){
                        //setting userInfo values here should not trigger model updates
                         test('And enter 21,500 into FSL, FSL should be $21,500, FPPL should be $0 and Other should be $30,924', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 52424, maxFsl: 21500});
                            formCurrentValues.set ({fsl: 12424, fppl: 7000, other: 33000});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 9076);
                            assert.equal(21500, displayValues.fsl, 'FSL should be 21500');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(30924, displayValues.other, 'Other should be 30924');
                        });
                    });
                });
                suite('remaining costs are $18,000 and maxFsl is $7,500', function(){
                    suite('and Form values are FSL - $7,400, FPPL - $10,600 & Other - $0', function(){
                        //setting userInfo values here should not trigger model updates
                         test('And increment by $100, FSL should be $7,500, FPPL should be $10,500 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 18000, maxFsl: 7500});
                            formCurrentValues.set ({fsl: 7400, fppl: 10600, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 100);
                            assert.equal(7500, displayValues.fsl, 'FSL should be 7500');
                            assert.equal(10500, displayValues.fppl, 'FPPL should be 10500');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $7,499, FPPL - $10,501 & Other - $0', function(){
                        test('And increment by $100, FSL should be $7,500, FPPL should be $10,500 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 18000, maxFsl: 7500});
                            formCurrentValues.set ({fsl: 7499, fppl: 10501, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 100);
                            assert.equal(7500, displayValues.fsl, 'FSL should be 7500');
                            assert.equal(10500, displayValues.fppl, 'FPPL should be 10500');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $7,499, FPPL - $9,501 & Other - $1000', function(){
                        test('And increment by $100, FSL should be $7,500, FPPL should be $9,500 and Other should be $1000', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 18000, maxFsl: 7500});
                            formCurrentValues.set ({fsl: 7499, fppl: 9501, other: 1000});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 100);
                            assert.equal(7500, displayValues.fsl, 'FSL should be 7500');
                            assert.equal(9500, displayValues.fppl, 'FPPL should be 9500');
                            assert.equal(1000, displayValues.other, 'Other should be 1000');
                        });
                    });
                    suite('and Form values are FSL - $13,878, FPPL - $0 & Other - $400', function(){
                         test('And increment by $100, FSL should be $13,978, FPPL should be $0 and Other should be $300', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 13878, fppl: 0, other: 400});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 100);
                            assert.equal(13978, displayValues.fsl, 'FSL should be 13978');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(300, displayValues.other, 'Other should be 300');
                        });
                    });
                });
                suite('remaining costs are $14,278 and maxFsl is $14,278', function(){
                    suite('and Form values are FSL - $14,278, FPPL - $0 & Other - $0', function(){
                        test('And increment by $100, FSL should be $14,278, FPPL should be $0 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14278, fppl: 0, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 100);
                            assert.equal(14278, displayValues.fsl, 'FSL should be 14278');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $14,178, FPPL - $0 & Other - $100', function(){
                        test('And increment by $100, FSL should be $14,278, FPPL should be $0 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14178, fppl: 0, other: 100});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, 100);
                            assert.equal(14278, displayValues.fsl, 'FSL should be 14278');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $14,278, FPPL - $0 & Other - $0', function(){
                        test('And decrement by $100, FSL should be $14,718, FPPL should be $100 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14278, fppl: 0, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, -100);
                            assert.equal(14178, displayValues.fsl, 'FSL should be 14178');
                            assert.equal(100, displayValues.fppl, 'FPPL should be 100');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $0, FPPL - $14,278 & Other - $0', function(){
                        test('And decrement by $100, FSL should be $0, FPPL should be $14,278 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 0, fppl: 14278, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fsl', userInfo, formCurrentValues, -100);
                            assert.equal(0, displayValues.fsl, 'FSL should be 0');
                            assert.equal(14278, displayValues.fppl, 'FPPL should be 14278');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                });
            });
            suite('When loan type is FPPL', function(){
                suite('remaining costs are $48,100 and maxFsl is $22,500', function(){
                    suite('and Form values are FSL - $0, FPPL - $0 & Other - $48100', function(){
                         test('And enter $50,000 in FPPL, FSL should be $0, FPPL should be $48,100 and Other should be $0', function (){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 48100, maxFsl: 22500});
                            formCurrentValues.set ({fsl: 0, fppl: 0, other: 48100});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, 50000);
                            assert.equal(0, displayValues.fsl, 'FSL should be 0');
                            assert.equal(48100, displayValues.fppl, 'FPPL should be 48100');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                });
                suite('remaining costs are $14,278 and maxFsl is $14,278', function(){
                    suite('and Form values are FSL - $10,378, FPPL - $0 & Other - $3900', function(){
                        test('And increment by $5000, FSL should be $10,278, FPPL should be $0 and Other should be $3900', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 10378, fppl: 0, other: 3900});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, -100);
                            assert.equal(10378, displayValues.fsl, 'FSL should be 10378');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(3900, displayValues.other, 'Other should be 3900');
                        });
                    });
                    suite('and Form values are FSL - $10,378, FPPL - $0 & Other - $3900', function(){
                        test('And decrement by $100, FSL should be $10,278, FPPL should be $0 and Other should be $3900', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 10378, fppl: 0, other: 3900});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, -100);
                            assert.equal(10378, displayValues.fsl, 'FSL should be 10378');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(3900, displayValues.other, 'Other should be 3900');
                        });
                    });
                    suite('and Form values are FSL - $14,178, FPPL - $100 & Other - $0', function(){
                        test('And decrement by $100, FSL should be $14,728, FPPL should be $100 and Other should be $100', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14178, fppl: 100, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, -100);
                            assert.equal(14178, displayValues.fsl, 'FSL should be 14178');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(100, displayValues.other, 'Other should be 100');
                        });
                    });
                    suite('and Form values are FSL - $14,178, FPPL - $100 & Other - $0', function(){
                        test('And increment by $100, FSL should be $14,078, FPPL should be $200 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14178, fppl: 100, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, 100);
                            assert.equal(14078, displayValues.fsl, 'FSL should be  14078');
                            assert.equal(200, displayValues.fppl, 'FPPL should be 200');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $14,128, FPPL - $100 & Other - $50', function(){
                        test('And increment by $100, FSL should be $14,078, FPPL should be $200 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14128, fppl: 100, other:50});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, 100);
                            assert.equal(14078, displayValues.fsl, 'FSL should be 14078');
                            assert.equal(200, displayValues.fppl, 'FPPL should be 200');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $14,078, FPPL - $100 & Other - $100', function(){
                        test('And increment by $100, FSL should be $14,178, FPPL should be $200 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14078, fppl: 100, other:100});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, 100);
                            assert.equal(14078, displayValues.fsl, 'FSL should be 14078');
                            assert.equal(200, displayValues.fppl, 'FPPL should be 200');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                    suite('and Form values are FSL - $78, FPPL - $14,200 & Other - $0', function(){
                        test('And increment by $100, FSL should be $0, FPPL should be $14,278 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 78, fppl: 14200, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, 100);
                            assert.equal(0, displayValues.fsl, 'FSL should be 0');
                            assert.equal(14278, displayValues.fppl, 'FPPL should be 14278');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                });
            });
            suite('When loan type is OTHER', function(){
                suite('remaining costs are $48,100 and maxFsl is $22,500', function(){
                    suite('and Form values are FSL - $22,500, FPPL - $25,600 & Other - $0', function(){
                         test('And enter $50,000 in Other, FSL should be $0, FPPL should be $0 and Other should be $48,100', function (){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 48100, maxFsl: 22500});
                            formCurrentValues.set ({fsl: 22500, fppl: 25600, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, 50000);
                            assert.equal(0, displayValues.fsl, 'FSL should be 0');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(48100, displayValues.other, 'Other should be 48100');
                        });
                    });
                    suite('and Form values are FSL - $0, FPPL - $0 & Other - $48,100', function(){
                         test('increment by $100, FSL should be $0, FPPL should be $0 and Other should be $48,100', function (){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 48100, maxFsl: 22500});
                            formCurrentValues.set ({fsl: 0, fppl: 0, other: 48100});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, 100);
                            assert.equal(0, displayValues.fsl, 'FSL should be 0');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(48100, displayValues.other, 'Other should be 48100');
                        });
                    });
                });
                suite('remaining costs are $14,278 and maxFsl is $14,278', function(){
                    suite('and Form values are FSL - $14,178, FPPL - $100 & Other - $0', function(){
                        test('and increment by $100, FSL should be $14,178, FPPL should be $0 and Other should be $100', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14178, fppl: 100, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, 100);
                            assert.equal(14178, displayValues.fsl, 'FSL should be 14178');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(100, displayValues.other, 'Other should be 100');
                        });
                    });
                    suite('and Form values are FSL - $14,128, FPPL - $50 & Other - $0', function(){
                        test('and increment by $100, FSL should be $14,178, FPPL should be $0 and Other should be $100', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14128, fppl: 50, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, 100);
                            assert.equal(14178, displayValues.fsl, 'FSL should be 14178');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(100, displayValues.other, 'Other should be 100');
                        });
                    }); 
                    suite('and Form values are FSL - $14,278, FPPL - $0 & Other - $0', function(){
                        test('and increment by $100, FSL should be $14,178, FPPL should be $0 and Other should be $100', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 14278, fppl: 0, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, 100);
                            assert.equal(14178, displayValues.fsl, 'FSL should be 14178');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(100, displayValues.other, 'Other should be 100');
                        });
                    });
                    suite('and Form values are FSL - $9,878, FPPL - $0 & Other - $4,400', function(){
                        test('and increment by $100, FSL should be $9,778, FPPL should be $0 and Other should be $4,500', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 9878, fppl: 0, other: 4400});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, 100);
                            assert.equal(9778, displayValues.fsl, 'FSL should be 9778');
                            assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                            assert.equal(4500, displayValues.other, 'Other should be 4500');
                        });
                    });
                    suite('and Form values are FSL - $13,928, FPPL - $100 & Other - $200', function(){
                        test('and decrement by $100, FSL should be $13,978, FPPL should be $200 and Other should be $100', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 13928, fppl: 100, other: 200});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, -100);
                            assert.equal(13928, displayValues.fsl, 'FSL should be 13928');
                            assert.equal(200, displayValues.fppl, 'FPPL should be 200');
                            assert.equal(100, displayValues.other, 'Other should be 100');
                        });
                    });
                    suite('and Form values are FSL - $11,078, FPPL - $3,310 & Other - $90', function(){
                        test('and decrement by $100, FSL should be $11,078, FPPL should be $3,400 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 11078, fppl: 3310, other: 90});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, -100);
                            assert.equal(11078, displayValues.fsl, 'FSL should be 11078');
                            assert.equal(3400, displayValues.fppl, 'FPPL should be 3400');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                });
            });
            suite('When enter value directly into FPPL input box', function(){
                suite('remaining costs are $14,278 and maxFsl is $14,278', function(){
                    suite('and Form values are FSL - $10,378, FPPL - $0 & Other - $3900', function(){
                        test('And input/increment by $5000, FSL should be $9,278, FPPL should be $5000 and Other should be $0', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                            formCurrentValues.set ({fsl: 10378, fppl: 0, other: 3900});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, 5000);
                            assert.equal(9278, displayValues.fsl, 'FSL should be 9278');
                            assert.equal(5000, displayValues.fppl, 'FPPL should be 5000');
                            assert.equal(0, displayValues.other, 'Other should be 0');
                        });
                    });
                });
                suite('remaining costs are $65,553 and maxFsl is $22,500', function(){
                    suite('and Form values are FSL - $22,500, FPPL - $43,053 & Other - $0', function(){
                        test('And input $5,000 which is a decrement by $38,053, FSL should be $22,500, FPPL should be $5000 and Other should be $38,053', function(){
                            //costOfAttendanceLessAdjustmentsTotal is remaining costs
                            userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 65553, maxFsl: 65553});
                            formCurrentValues.set ({fsl: 22500, fppl: 43053, other: 0});

                            var displayValues = LoanLimits.calculateLoanDisplayTotals('fppl', userInfo, formCurrentValues, -38053);
                            assert.equal(22500, displayValues.fsl, 'FSL should be 22500');
                            assert.equal(5000, displayValues.fppl, 'FPPL should be 5000');
                            assert.equal(38053, displayValues.other, 'Other should be 38053');
                        });
                    });
                });
            });
            suite('When enter value directly into OTHER input box', function(){
                suite('and Form values are FSL - $11,078, FPPL - $3,200 & Other - $0', function(){
                    test('And input $3,500 which is an increment by $3,500, FSL should be $10,778, FPPL should be $0 and Other should be $3500', function(){
                        //costOfAttendanceLessAdjustmentsTotal is remaining costs
                        userInfo.set ({costOfAttendanceLessAdjustmentsTotal: 14278, maxFsl: 14278});
                        formCurrentValues.set ({fsl: 11078, fppl: 3200, other: 0});

                        var displayValues = LoanLimits.calculateLoanDisplayTotals('other', userInfo, formCurrentValues, 3500);
                        assert.equal(10778, displayValues.fsl, 'FSL should be 10778');
                        assert.equal(0, displayValues.fppl, 'FPPL should be 0');
                        assert.equal(3500, displayValues.other, 'Other should be 3500');
                    });
                });
            });
        });
        suite('Determine YourPlan Message to display', function(){
            var userInfo = new Models.UserInfoModel({});
            var totalLoans = new Models.LoansModel({});
            suite('When FSL loans are more heavily used', function(){
                test('Message to display should be FSL', function(){
                    userInfo.set ({grantsTotal: 200, monthlyInstallmentsTotal: 500});
                    totalLoans.set({fsl: 600, fppl: 0, other: 0});
                    var yourPlanMsg = LoanLimits.determineYourPlanMessage(userInfo, totalLoans);
                    assert.equal('FSL', yourPlanMsg);
                });
            });
            suite('When FPPL loans are more heavily used', function(){
                test('Message to display should be PlusLoan', function(){
                    userInfo.set ({grantsTotal: 200, monthlyInstallmentsTotal: 500});
                    totalLoans.set({fsl: 600, fppl: 700, other: 50});
                    var yourPlanMsg = LoanLimits.determineYourPlanMessage(userInfo, totalLoans);
                    assert.equal('PlusLoan', yourPlanMsg);
                });
            });
            suite('When Other loans are more heavily used', function(){
                test('Message to display should be PrivateLoan', function(){
                    userInfo.set ({grantsTotal: 200, monthlyInstallmentsTotal: 500});
                    totalLoans.set({fsl: 600, fppl: 700, other: 5000});
                    var yourPlanMsg = LoanLimits.determineYourPlanMessage(userInfo, totalLoans);
                    assert.equal('PrivateLoan', yourPlanMsg);
                });
            });
            suite('When Grants and Scholarships are more heavily used', function(){
                test('Message to display should be Grants', function(){
                    userInfo.set ({grantsTotal: 20000, monthlyInstallmentsTotal: 500});
                    totalLoans.set({fsl: 600, fppl: 700, other: 5000});
                    var yourPlanMsg = LoanLimits.determineYourPlanMessage(userInfo, totalLoans);
                    assert.equal('Grants', yourPlanMsg);
                });
            });
            suite('When Monthly Installments are more heavily used', function(){
                test('Message to display should be Grants', function(){
                    userInfo.set ({grantsTotal: 20000, monthlyInstallmentsTotal: 50000});
                    totalLoans.set({fsl: 600, fppl: 700, other: 5000});
                    var yourPlanMsg = LoanLimits.determineYourPlanMessage(userInfo, totalLoans);
                    assert.equal('Grants', yourPlanMsg);
                });
            });
            suite('When Graduate and FPPL loans are more heavily used', function(){
                test('Message to display should be GradPlus', function(){
                    userInfo.set ({gradeLevel: 0, grantsTotal: 200, monthlyInstallmentsTotal: 500});
                    totalLoans.set({fsl: 600, fppl: 5000, other: 700});
                    var yourPlanMsg = LoanLimits.determineYourPlanMessage(userInfo, totalLoans);
                    assert.equal('GradPlusLoan', yourPlanMsg);
                });
            });
        });
    });
});
