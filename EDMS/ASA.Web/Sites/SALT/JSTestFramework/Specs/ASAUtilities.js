/*global describe, it, chai, sinon */
define([
    'asa/ASAUtilities',
    'jquery.cookie'
], function (Utility) {

    var assert = chai.assert;

    describe('ASAUtilities', function () {
        var date = '/Date(1388811600000-0500)/';
        var testlocation = {
            hash: '#savemore',
            href: 'https://www.saltmoney.org/JSTestFramework#savemore',
            pathname: '/content/moneycoach/0/N-32'
        };
        describe('convertDate', function () {
            it('should convert date from back-end to Date in YYYY-MM-DD format', function () {
                var convertedDate = Utility.convertDate(date);
                assert.equal('2014-01-04', convertedDate);
            });
        });

        describe('convertFromJsonDate', function () {
            it('should convert date from back-end to M/D/YYYY format', function () {
                var convertedFromJsonDate = Utility.convertFromJsonDate(date);
                assert.equal('1/4/2014', convertedFromJsonDate);
            });
        });

        describe('getLocationHash', function () {
            it('should get hash location', function () {
                var convertedlocationHashIE = Utility.getLocationHash(testlocation, true);
                assert.equal('savemore', convertedlocationHashIE, 'Only works for IE. IsIE set to True');

            });
            it('should not get location', function () {
                var convertedlocationHash = Utility.getLocationHash(testlocation, false);
                assert.notEqual('savemore', convertedlocationHash, 'Only works for IE. IsIE set to false');

            });
        });

        describe('getPathnameHash', function () {
            it('should get pathname', function () {
                var convertedFpathNameHash = Utility.getPathnameHash(testlocation, true);
                assert.equal(-1, convertedFpathNameHash.indexOf('#'));
            });
        });

        describe('getParameterByNameFromString', function () {
            it('should get value of the string from the URL', function () {
                var nameValue = Utility.getParameterByNameFromString('view', 'https://www.saltmoney.org/index.html?CT=N-14&view=List&Ns=P_Rating|1');
                assert.equal('List', nameValue);
            });
        });

        describe('getParameterByNameFromString', function () {
            it('should get value of the string from the URL', function () {
                var nameValue = Utility.getParameterByNameFromString('view', 'https://www.saltmoney.org/index.html?CT=N-14&view=List&Ns=P_Rating|1');
                assert.equal('List', nameValue);
            });
        });

        describe('handleUserConfiguration', function () {
            it('using UserSegment cookie, to update api query with param key Endeca_user_segments, ? or & will be handled', function () {
                var apiURL = 'https://xxx.saltmoney.org/testingSegment';
                $.cookie('UserSegment', 'School-Harvard');
                apiURL = Utility.handleUserConfiguration(apiURL, $.cookie('UserSegment'));
                assert.equal('https://xxx.saltmoney.org/testingSegment?Endeca_user_segments=School-Harvard', apiURL);
            });
            it('using UserSegment cookie, to update api query with param key Endeca_user_segments, ? or & will be handled', function () {
                var apiURL = 'https://xxx.saltmoney.org/testingSegment?foo=bar';
                $.cookie('UserClass', 'UserClass-Alumni');
                apiURL = Utility.handleUserConfiguration(apiURL, $.cookie('UserClass'));
                assert.equal('https://xxx.saltmoney.org/testingSegment?foo=bar&Endeca_user_segments=UserClass-Alumni', apiURL);
            });
            it('testing apiURL will stay the same if userSegment is empty or undefined', function () {
                var apiURL = 'https://xxx.saltmoney.org/testingSegment',
                    userSegment = '';
                apiURL = Utility.handleUserConfiguration(apiURL, userSegment);
                assert.equal('https://xxx.saltmoney.org/testingSegment', apiURL);
            });
        });
        describe('getParamStringFromURL', function () {
            it('should get the parameter value for Ns', function () {
                var nameValue = Utility.getParamStringFromURL('Ns', 'https://www.saltmoney.org/index.html?CT=N-14&view=List&Ns=P_Rating|1');
                assert.equal('Ns=P_Rating|1', nameValue);
            });
        });
        describe('checkContentParticipation', function () {
            var siteMember;
            beforeEach(function () {
                siteMember = {
                    IsAuthenticated: 'true',
                    OrganizationProducts: [{
                        IsOrgProductActive: true,
                        ProductID: 3
                    }, {
                        IsOrgProductActive: true,
                        ProductID: 4
                    }, {
                        IsOrgProductActive: true,
                        ProductID: 7
                    }]
                };
            });
            it('should return an empty array when the user is opted into products three, four and seven or when the user is not authenticated', function () {
                var primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, []);
                siteMember.IsAuthenticated = false;
                var primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, []);
            });
            it('should return the primary key for the scholarship search tool when product four is not active or is not in the list of products', function () {
                //Set the product to inactive and check that we get the correct response
                siteMember.OrganizationProducts[1].IsOrgProductActive = false;
                var primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, ['101-7416']);
                //Set the prodcut to be a different product entirely (simulating the product not being in the list)
                siteMember.OrganizationProducts[1].IsOrgProductActive = true;
                siteMember.OrganizationProducts[1].ProductID = 2;
                primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, ['101-7416']);
            });
            it('should return the primary key for the college cost planner tool when product seven is not active or is not in the list of products', function () {
                //Set the product to inactive and check that we get the correct response
                siteMember.OrganizationProducts[2].IsOrgProductActive = false;
                var primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, ['101-23826']);
                //Set the prodcut to be a different product entirely (simulating the product not being in the list)
                siteMember.OrganizationProducts[2].IsOrgProductActive = true;
                siteMember.OrganizationProducts[2].ProductID = 2;
                primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, ['101-23826']);
            });
            it('should return the primary keys for scholarship search, and college cost planner when product three, four or seven are not active or are not in the list of products', function () {
                //Set the product to inactive and check that we get the correct response
                siteMember.OrganizationProducts[0].IsOrgProductActive = false;
                siteMember.OrganizationProducts[1].IsOrgProductActive = false;
                siteMember.OrganizationProducts[2].IsOrgProductActive = false;
                var primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, ['101-7416', '101-23826']);
                //Set the prodcut to be a different product entirely (simulating the products not being in the list)
                siteMember.OrganizationProducts[0].IsOrgProductActive = true;
                siteMember.OrganizationProducts[0].ProductID = 2;
                siteMember.OrganizationProducts[1].IsOrgProductActive = true;
                siteMember.OrganizationProducts[1].ProductID = 5;
                siteMember.OrganizationProducts[2].IsOrgProductActive = true;
                siteMember.OrganizationProducts[2].ProductID = 6;
                primaryKeysToExclude = Utility.checkContentParticipation(siteMember);
                assert.deepEqual(primaryKeysToExclude, ['101-7416', '101-23826']);
            });
        });

        describe('lookupUserSegment', function () {
            var segmentData = {
                "antioch-university": "Region-New-England",
                "asa-university-sample": "Region-New-England",
                "american-river-college": "ID-American-River-College",
                "some nologo school": "some segment name",
                "nologo": "Default"
            }

            var brandedOrg = {
                Brand: 'asa-university-sample',
                OrganizationName: 'ASA University'
            };

            var nologoOrgWithSegment = {
                Brand: 'nologo',
                OrganizationName: 'some nologo school'
            }

            it('branded org should return segment name', function () {
                var segment = Utility.lookupUserSegment(segmentData, '', brandedOrg);
                assert.equal('UserSegment=Region-New-England', segment);
            });

            it('branded org should return | delimited segment name when segment name passed in', function () {
                var segment = Utility.lookupUserSegment(segmentData, 'UserSegment=fooo', brandedOrg);
                assert.equal('UserSegment=fooo|Region-New-England', segment);
            });

            it('nologo org that uses segments should return segment name', function () {
                var segment = Utility.lookupUserSegment(segmentData, '', nologoOrgWithSegment);
                assert.equal('UserSegment=some segment name', segment);
            });

            it('nologo org that uses segments should return | delimited segment name when segment name passed in', function () {
                var segment = Utility.lookupUserSegment(segmentData, 'UserSegment=fooo', nologoOrgWithSegment);
                assert.equal('UserSegment=fooo|some segment name', segment);
            });
        });
    });
});
