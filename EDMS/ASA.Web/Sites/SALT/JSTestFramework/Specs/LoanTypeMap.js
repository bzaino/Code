/*global describe, it, chai, sinon */
define([
    'modules/FinancialStatus/LoanTypeMap'
], function(map) {

    /* default to assert style, replace this if you prefer 'should' or 'expect'. */
    var assert = chai.assert;

    /*  What I want to test:
            -Hash object where keys are 2 digit codes, value is either federal or private
            -Correct string value is returned (According to Winnie)
    */
    describe('Loan Type Map', function() {
        it('should return the right loan name and type name based on LoanTypeID', function(){
            //Test a federal type
            var returnedType = map['D5'];
            assert.strictEqual('federal', returnedType.loanType);
            assert.strictEqual('Direct Consolidated Unsubsidized', returnedType.typeName);

            //Test a private type
            returnedType = map['PR'];
            assert.strictEqual('private', returnedType.loanType);
            assert.strictEqual('Private Student Loan', returnedType.typeName);

            //Test a Federal type
            returnedType = map['DU'];
            assert.strictEqual('federal', returnedType.loanType);
            assert.strictEqual('National Defense Loan (Perkins)', returnedType.typeName);

            //Test a Federal type
            returnedType = map['D4'];
            assert.strictEqual('federal', returnedType.loanType);
            assert.strictEqual('Direct PLUS Parent', returnedType.typeName);

            //Test a Federal type
            returnedType = map['NU'];
            assert.strictEqual('federal', returnedType.loanType);
            assert.strictEqual('National Direct Student Loan (Perkins)', returnedType.typeName);

            //Test a Federal type
            returnedType = map['PL'];
            assert.strictEqual('federal', returnedType.loanType);
            assert.strictEqual('FFEL PLUS Parent', returnedType.typeName);
        });
    });
});
