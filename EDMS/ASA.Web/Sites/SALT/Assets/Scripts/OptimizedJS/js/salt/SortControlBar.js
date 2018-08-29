define([
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'underscore'
], function (Utility, SiteMember, _) {
    var SortControlBar = {
        populateDropdown: function (json) {
            var showCourses = Utility.isSubscribedToProduct(SiteMember.attributes, 2);
            var rightBarOptions = [
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
                ],
                middleBarOptionsMap = {
                    'Endeca.stratify': 'By Recommended',
                    'page_title': 'By Relevance',
                    'P_Rating': 'By Usefulness',
                    'P_Hits_Last_30_Days': 'By Popularity'
                },
                leftBarOptionsMap = {
                    'MasterMoney': 'Master Money',
                    'RepayStudentDebt': 'Repay Student Debt',
                    'PayForSchool': 'Pay For School',
                    'FindAJob': 'Find A Job',
                    'AssociatesDegree': 'Featured',
                    'BachelorsDegree': 'Featured',
                    'GraduateSchool': 'Featured',
                    'AlumniPostSchool': 'Featured',
                    '': 'Featured',
                    'All': 'All Items'
                },
            dimsValFromURL = Utility.getParameterByNameFromString('Dims', Utility.getLocationSearch()),
            nsObject = '',
            typeVal = Utility.getParameterByNameFromString('Type', Utility.getLocationSearch());
            
            if (showCourses) {
                rightBarOptions.push({'Courses': '304'});
            }

            /*Checking json to grab the sortkey.*/
            nsObject = _.find(Object.keys(middleBarOptionsMap), function (element) {
                    var endecaSortKey = Utility.checkNested(json, 'endeca:assemblerRequestInformation.endeca:sortKey[0]');
                    if (endecaSortKey) {
                        return endecaSortKey.indexOf(element) !== -1;
                    } else {
                        //return true so that it won't throw error when something wrong with endeca.
                        return true;
                    }
                });

            /*Set sort control bar left val for search page*/
            if (Utility.getlocationPath().indexOf('content/search') > -1) {
                typeVal = 'All';
            }
            json.sortControlBarLeftVal = leftBarOptionsMap[typeVal];
            json.sortControlBarMiddleVal = middleBarOptionsMap[nsObject];
            json.sortControlBarRightOptions = rightBarOptions;

            //SALT Suggests treatment should be shown in the first two tiles when user has selected both a user goal and 'By Relevance' -- SWD-6148
            if (nsObject === 'page_title' && typeVal !== '' && typeVal !== 'All') {
                if (Utility.checkNested(json, 'mainContent[0].contents[0].records[1]') && json.mainContent[0].contents[0].firstRecNum === 1) {
                    json.mainContent[0].contents[0].records[0].SaltSuggests = json.mainContent[0].contents[0].records[1].SaltSuggests = true;
                }
            }
            return json;
        },
        //Check NoBanner parameter, if NoBanner is true, it means that the request is from resource tab,
        //therefor we set signal into json to hide banner image and Goals CTAs.
        checkNoBannerTreatment: function (json, queryString) {
            if (Utility.getParameterByNameFromString('NoBanner', queryString)) {
                json.noBannerTreatment = true;
                json.hideGoals = true;
                var dimsArray = Utility.getParameterByNameFromString('Dims', queryString).split(',');
                if (dimsArray.length === 1 && dimsArray[0]) {
                    json.showContentTypeInHeader = true;
                    var rightOptionsJsonObject = {};
                    _.each(json.sortControlBarRightOptions, function (element) {
                        rightOptionsJsonObject = _.extend(rightOptionsJsonObject, _.invert(element));
                    });
                    json.selectedContentType = rightOptionsJsonObject[dimsArray[0]];
                }
            }
            return json;
        }
    };
    return SortControlBar;
});
