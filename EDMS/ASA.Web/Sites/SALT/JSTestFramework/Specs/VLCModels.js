/*global describe, beforeEach, it, chai, sinon */
define(['modules/VLC/VLCModels'], function(models) {

    var assert = chai.assert;
    describe('Repayment Navigator Models', function() {


        describe('Progress Model', function() {
            var progress = new models.ProgressModel({});

            it('should set attributes to defaults on model creation', function() {
                assert.equal(0, progress.get('MemberID'));
                assert.equal('XX,XXX', progress.get('EstimatedLoanBalance'));
                assert.equal('XX', progress.get('initialMonthlyPayment'));
                assert.equal('N/A', progress.get('EnrollmentStatus'));
                assert.equal('mm/DD/YYYY', progress.get('GraduationDate'));
                assert.equal(null, progress.get('RepaymentStatus'));
                assert.equal(null, progress.get('MostRecentPaymentDate'));
                assert.equal('N/A', progress.get('DaysSinceLastPayment'));
                assert.equal(0, progress.get('AdjustedGrossIncome'));
                assert.equal(0, progress.get('FamilySize'));
                assert.equal(0, progress.get('AmountBorrowed'));
                assert.equal('Standard', progress.get('Plan'));
                assert.equal(null, progress.get('FilingStatus'));
                assert.equal(null, progress.get('StateOfResidence'));
            });
            it('should have correct REST url', function() {
                assert.equal('/api/ASAMemberService/Profile', progress.url);
            });

        });



        describe('Response Model', function() {
            var response = new models.ResponseModel({});

            it('should set attributes to defaults on model creation', function() {
                assert.equal(0, response.get('MemberID'));
                assert.equal('', response.get('Question').QuestionID);
                assert.equal('', response.get('Question').QuestionVersion);
                assert.equal('', response.get('Question').QuestionText);
                assert.equal('', response.get('ResponseText'));
            });

            it('should have correct REST url', function() {
                assert.equal('/api/SurveyService/AddVlcResponse', response.url);
            });

        });

        describe('What Youve Learned Model', function() {
            var learned = new models.WhatYouveLearnedModel({});

            it('should set attributes to defaults on model creation', function() {
                assert.equal('Go Now', learned.get('RepaymentStandard'));
                assert.equal('Go Now', learned.get('RepaymentExtended'));
                assert.equal('Go Now', learned.get('RepaymentGraduated'));
                assert.equal('Go Now', learned.get('IncomeDriven'));
                assert.equal('Go Now', learned.get('Consolidation'));
                assert.equal('Go Now', learned.get('DefermentInSchool'));
                assert.equal('Go Now', learned.get('DefermentEducationRelated'));
                assert.equal('Go Now', learned.get('DefermentSummerBridge'));
                assert.equal('Go Now', learned.get('DefermentPerkins'));
                assert.equal('Go Now', learned.get('DefermentRehabilitation'));
                assert.equal('Go Now', learned.get('DefermentPlusLoan'));
                assert.equal('Go Now', learned.get('DefermentUnemployment'));
                assert.equal('Go Now', learned.get('DefermentEconomic'));
                assert.equal('Go Now', learned.get('DefermentMilitary'));
                assert.equal('Go Now', learned.get('DefermentPeaceCorps'));
                assert.equal('Go Now', learned.get('DefermentPostActiveDuty'));
                assert.equal('Go Now', learned.get('ForgivenessEducator'));
                assert.equal('Go Now', learned.get('ForgivenessHealthcare'));
                assert.equal('Go Now', learned.get('ForgivenessMilitary'));
                assert.equal('Go Now', learned.get('ForgivenessPublicServant'));
                assert.equal('Go Now', learned.get('ForgivenessSTEM'));
                assert.equal('Go Now', learned.get('LifeEvent'));
                assert.equal('Go Now', learned.get('Default'));
                assert.equal('Go Now', learned.get('Delinquency'));
            });

        });
    });
});
