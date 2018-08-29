/*global describe, before, it, chai, sinon */
define(['modules/LoanCollection'], function(collection) {

    var assert = chai.assert;

    describe('Loans Collection', function() {

        var responseMock, server, loans;
        before(function(done) {
            responseMock = {
                'ErrorList': [],
                'RedirectURL': '',
                'Loans': [{
                    'ErrorList': [],
                    'RedirectURL': '',
                    'DateAdded': '\/Date(1378930055733-0400)\/',
                    'IndividualId': '234dcb9d-3b20-f440-895d-0a9fe6f29632',
                    'InterestRate': 4.66,
                    'IsActive': false,
                    'LoanSelfReportedEntryId': '58672',
                    'LoanSource': 'Member',
                    'LoanStatusId': '',
                    'LoanTerm': 10,
                    'LoanTypeId': 'CL',
                    'MemberId': 1404240,
                    'OriginalLoanAmount': 17000.00,
                    'PrincipalBalanceOutstandingAmount': 17000.00,
                    'ReceivedYear': 0,
                    'RecordSourceId': 2,
                    'LastModified': '\/Date(1378930055733-0400)\/'
                }, {
                    'ErrorList': [],
                    'RedirectURL': '',
                    'DateAdded': '\/Date(1378930055733-0400)\/',
                    'IndividualId': '234dcb9d-3b20-f440-895d-0a9fe6f29632',
                    'InterestRate': 4.66,
                    'IsActive': false,
                    'LoanSelfReportedEntryId': '58673',
                    'LoanSource': 'Member',
                    'LoanStatusId': '',
                    'LoanTerm': 10,
                    'LoanTypeId': 'CL',
                    'MemberId': 1404240,
                    'OriginalLoanAmount': 17000.00,
                    'PrincipalBalanceOutstandingAmount': 17000.00,
                    'ReceivedYear': 0,
                    'RecordSourceId': 2,
                    'LastModified': '\/Date(1378930055733-0400)\/'
                }]
            };
            server = sinon.fakeServer.create();
            server.respondWith(
            [
            200, {
                'Content-type': 'application/json'
            },
            JSON.stringify(responseMock)]);
            loans = new collection.LoansCollection();

            done();
        });

        describe('Collection Defaults', function() {
            it('should have the right REST url', function() {
                assert.equal('/api/SelfReportedService/restLoans', loans.url);
            });
        });

        describe('Server Interactions', function() {

            it('should make request to the right place', function() {
                loans.fetch();
                assert.equal(1, server.requests.length);
                assert.equal('GET', server.requests[0].method);
                assert.equal(loans.url, server.requests[0].url);
            });

            it('should parse fetch properly', function() {
                loans.fetch();
                server.respond();
                assert.equal(responseMock.Loans.length, loans.length);
            });
        });

        describe('Loan model parsing', function() {
            it('should only detect that the parsed object returns only the loan objects inside (of which there are two), and ignore the errorlist or returnurl objects', function() {
                var response = loans.parse(responseMock);
                assert.isArray(response);
                assert.equal(2, response.length);
            });
        });
    });
});
