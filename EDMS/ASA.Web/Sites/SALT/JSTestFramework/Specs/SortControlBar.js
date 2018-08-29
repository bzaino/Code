/*global describe, it, chai, sinon */
define([
    'salt/SortControlBar'
], function (SortControlBar) {
    var assert = chai.assert;
    describe('SortControlBar functions test', function () {
        describe('checkNoBannerTreatment', function () {
            var mockData = {},
                mockDataOriginal = {};
            beforeEach(function () {
                mockData = {
                    sortControlBarRightOptions:
                        [
                            {
                                'Articles': '41'
                            }, {
                                'Videos': '42'
                            }, {
                                'Lessons': '44'
                            }, {
                                'Forms': '43'
                            }, {
                                'Tools': '46'
                            }, {
                                'Infographics': '45'
                            }, {
                                'Comics': '157'
                            }, {
                                'eBooks': '303'
                            }
                        ]
                    },
                    mockDataOriginal = {
                        sortControlBarRightOptions:
                            [
                                {
                                    'Articles': '41'
                                }, {
                                    'Videos': '42'
                                }, {
                                    'Lessons': '44'
                                }, {
                                    'Forms': '43'
                                }, {
                                    'Tools': '46'
                                }, {
                                    'Infographics': '45'
                                }, {
                                    'Comics': '157'
                                }, {
                                    'eBooks': '303'
                                }
                            ]
                        };
                });
            afterEach( function () {
                assert.deepEqual(mockDataOriginal.sortControlBarRightOptions, mockData.sortControlBarRightOptions);
            });
            it('No "NoBanner" parameter in querystring, json returned should be whatever was passed in', function () {
                var queryString = '?Dims=43';
                mockData = SortControlBar.checkNoBannerTreatment(mockData, queryString);
                assert.deepEqual(mockDataOriginal, mockData);
            });
            it('"NoBanner" parameter in querystring, DIMs array length does not equal 1, json returned should equal what was passed plus: noBannerTreatment = true, hideGoals =true', function () {
                var queryString = '?NoBanner=true&Dims=43,46';
                mockData = SortControlBar.checkNoBannerTreatment(mockData, queryString);
                assert.isTrue(mockData.noBannerTreatment);
                assert.isTrue(mockData.hideGoals);
                assert.isUndefined(mockData.showContentTypeInHeader);
            });
            it('"NoBanner" parameter in header, DIMs length = 1, json should include showContentTypeInHeader and, selectedContentType', function () {
                var queryString = '?NoBanner=true&Dims=43';
                mockData = SortControlBar.checkNoBannerTreatment(mockData, queryString);
                assert.isTrue(mockData.noBannerTreatment);
                assert.isTrue(mockData.hideGoals);
                assert.isTrue(mockData.showContentTypeInHeader);
                assert.equal('Forms', mockData.selectedContentType);
            });
            it('"NoBanner" parameter in header, DIMs length =1 but dimsArray[0] is falsy, json should contain what was passed plus noBannerTreatment and hideGoals', function () {
                var queryString = '?NoBanner=true&Dims=';
                mockData = SortControlBar.checkNoBannerTreatment(mockData, queryString);
                assert.isTrue(mockData.noBannerTreatment);
                assert.isTrue(mockData.hideGoals);
                assert.isUndefined(mockData.showContentTypeInHeader);
            });
        });
    });
});
