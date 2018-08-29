/* jshint maxstatements: 50 */
define([
    'jquery',
    'dust',
    'underscore',
    'salt',
    'configuration',
    'jquery.cookie'
], function ($, dust, _, SALT, Configuration) {
    //See the following for the structure of a sort comparison function
    //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/sort
    function lastModifiedComparer(a, b) {
        //a, the current item is more recent, dont swap
        if (a.attributes.LastModified < b.attributes.LastModified) {
            return 1;
        }
        //the item we are comparing with, b, is more recent, swap the order
        if (a.attributes.LastModified > b.attributes.LastModified) {
            return -1;
        }
        return 0;
    }

    function buildEndecaIgnoreString(tags) {
        //Changes the format we get from configuration: "budgeting,banking,credit,taxes,insurance"
        //into the format the Endeca "Nr" parameter expects: 'NOT(Tags:budgeting),NOT(Tags:banking),NOT(Tags:credit),NOT(Tags:taxes),NOT(Tags:insurance),'
        return tags.split(',').map(function (tag) {
            return 'NOT(Tags:' + tag + '),';
        }).join('');
    }

    var terms = [],
        utility = {
        SOURCE_IMPORTED_REP_NAV : 1,
        SOURCE_SELF_REPORTED_REP_NAV : 2,
        SOURCE_IMPORTED_KWYO : 3,
        SOURCE_SELF_REPORTED_KWYO : 4,
        itemsAlreadyRendered : {
            MySALT: {},
            RepayStudentDebt: {},
            MasterMoney: {},
            PayForSchool: {},
            FindAJob: {}
        },
        paramToGoalInfo : {
            MM: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.MasterMoney,
                url: '',
                name: 'MasterMoney',
                tags: buildEndecaIgnoreString(Configuration.goalTags.MasterMoney),
                paramName: 'MM'
            },
            RSD: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.RepayStudentDebt,
                url: '',
                name: 'RepayStudentDebt',
                tags: buildEndecaIgnoreString(Configuration.goalTags.RepayStudentDebt),
                paramName: 'RSD'
            },
            FJ: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.FindAJob,
                url: '',
                name: 'FindAJob',
                tags: buildEndecaIgnoreString(Configuration.goalTags.FindAJob),
                paramName: 'FJ'
            },
            PS: {
                baseUrl: Configuration.apiEndpointBases.GenericEndeca + Configuration.apiEndpointBases.PayForSchool,
                url: '',
                name: 'PayForSchool',
                tags: buildEndecaIgnoreString(Configuration.goalTags.PayForSchool),
                paramName: 'PS'
            }
        },
        //////////////////////////////////////////////////////////////////////////
        //  WCF Data Formatting Utilities
        //////////////////////////////////////////////////////////////////////////
        convertFromJsonDate: function (input) {
            var convertedDate = input;

            if (input && typeof input === 'string' && input.indexOf('\/Date') !== -1) //if input is a JSON formatted Date string
            {
                var substringedDate = input.substring(6); //substringedDate= 1291548407008)/
                var parsedIntDate = parseInt(substringedDate, 10); //parsedIntDate= 1291548407008
                var dt = new Date(parsedIntDate);  // parsedIntDate passed to date constructor

                convertedDate = dt.getMonth() + 1 + '/' + dt.getDate() + '/' + dt.getFullYear();
            }

            return convertedDate;
        },
        formatDate: function (dateObj) {
            var monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
            return monthNames[dateObj.getMonth()] + ' ' + dateObj.getDate() + ', ' + dateObj.getFullYear();
        },

        /////////////////////////////////////////////////////////
        // getParameterByName
        // Parse URL QueryString for parameters (cmak)
        /////////////////////////////////////////////////////////
        getParameterByName: function (name) {
            name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
            var regexS = '[\\?&]' + name + '=([^&#]*)';
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.href.replace('&amp;', '&'));
            if (!results) {
                return '';
            } else {
                return decodeURIComponent(results[1].replace(/\+/g, ' '));
            }
        },
        getParameterByNameFromString: function (name, querystring) {
            if (querystring && querystring.indexOf('?') === -1) {
                querystring = '?' + querystring;
            }
            name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
            var regex = new RegExp('[\\?&]' + name + '=([^&#]*)'),
                results = regex.exec(querystring);
            return !results ? '' : decodeURIComponent(results[1]);
        },
        getParamStringFromURL: function (param, searchScope) {
            return param + '=' + this.getParameterByNameFromString(param, searchScope);
        },
        getLocationHash: function (linkString, isIE) {
            if (isIE && linkString.hash) {
                var hashString = linkString.hash,
                pathString = linkString.pathname;
                if (hashString.indexOf(pathString) === 1) {
                    hashString = hashString.replace(pathString, '');
                }
                return hashString.replace('#', '').replace('/', '');
            }
            return linkString.search;
        },

        getPathnameHash: function (linkString, isIE) {
            if (isIE && linkString.hash) {
                var a = document.createElement('a');
                a.href = linkString.hash.replace('#', '');
                return a.pathname;
            }
            return linkString.pathname;
        },
        getHostname: function (url) {
            var hostname;
            //find & remove protocol (http, ftp, etc.) and get hostname

            if (url.indexOf("://") > -1) {
                hostname = url.split('/')[2];
            }
            else {
                hostname = url.split('/')[0];
            }

            //find & remove port number
            hostname = hostname.split(':')[0];
            //find & remove "?"
            hostname = hostname.split('?')[0];

            return hostname;
        },
        getCopyrightYear: function () {
            var date = new Date();
            document.getElementById('copyrightyear').innerHTML = date.getFullYear();
        },

        convertDate: function (date) {
            var jsDate = utility.convertSALtoJSdate(date);
            var yyyy = jsDate.getFullYear().toString();
            var mm = (jsDate.getMonth() + 1).toString(); // getMonth() is zero-based
            var dd  = jsDate.getDate().toString();
            // return YYYY-MM-DD and make sure MM and DD are in the double digit format
            return yyyy + '-' + (mm[1] ? mm : '0' + mm[0]) + '-' + (dd[1] ? dd : '0' + dd[0]);
        },
        convertSALtoJSdate: function (dateString) {
            return new Date(parseInt(dateString.substr(6), 10));
        },

        deepLinkToCourse: function (coursePath) {
            //This implementation is based on Remote Learner's recommendation
            var courseLink = 'https://' + utility.getHostname(Configuration.mm101.url);
            if (coursePath) {
                courseLink = courseLink + coursePath.trim();
            }
            //update js-Courses-form action url, with Courses url
            $('#js-Courses-form').attr('action', Configuration.mm101.url);
            document.forms['js-Courses-form'].wantsurl.value = courseLink;
            document.forms['js-Courses-form'].submit();
        },

        // TODO: Find a better wat to scroll to top when needed isntead of using scrollToTop flag
        renderDustTemplate: function (tmplName, data, cbk, element, scrollToTop) {
            //If no callback was passed set it to a no-op
            if (!_.isFunction(cbk)) {
                cbk = function () {};
            }
            require(['Compiled/' + tmplName], function () {
                dust.render(tmplName, data, function (err, out) {
                    if (err) {
                        console.error('Rendering failed: ', err);
                    }
                    if (element) {
                        $(element).html(out);
                    }
                    if (scrollToTop) {
                        $(window).scrollTop(0);
                    }
                    cbk(err, out);
                });
            });
        },
        getLocationSearch: function () {
            if ($.browser.msie) {
                return utility.getLocationHash(location, true);
            } else {
                return utility.getLocationHash(location, false);
            }
        },
        getlocationPath: function () {
            if ($.browser.msie) {
                return utility.getPathnameHash(location, true);
            } else {
                return utility.getPathnameHash(location, false);
            }
        },
        handleUserConfiguration: function (apiUrl, userSegment) {
            if (userSegment) {
                apiUrl = this.updateQueryString(apiUrl, 'Endeca_user_segments', userSegment);
            }
            return apiUrl;
        },
        getDimensionString: function (dimsArray) {
            //On search page, definitions should always be part of the contents
            if (location.href.indexOf('searchCriteria') > -1) {
                dimsArray.push('159');
            }
            return dimsArray.join(',');
        },
        updateQueryString: function (url, param, paramVal) {
            if (typeof param === 'object') {
                url = this.updateQueryStringByObject(url, param);
            } else {
                url = this.updateQueryStringByString(url, param, paramVal);
            }
            return url;
        },
        updateQueryStringByString: function (currentURL, paramName, paramValue) {
            var url = currentURL || '';
            paramName += '=';
            if (url.indexOf(paramName) >= 0) {
                var prefix = url.substring(0, url.indexOf(paramName));
                var suffix = url.substring(url.indexOf(paramName));
                suffix = suffix.substring(suffix.indexOf('=') + 1);
                suffix = (suffix.indexOf('&') >= 0) ? suffix.substring(suffix.indexOf('&')) : '';
                url = prefix + paramName + paramValue + suffix;
            } else {
                if (url.indexOf('?') < 0) {
                    url += '?' + paramName + paramValue;
                } else {
                    url += '&' + paramName + paramValue;
                }
            }
            return url;
        },
        updateQueryStringByObject: function (url, paramObject) {
            var _this = this;
            _.each(_.keys(paramObject), function (paramName) {
                url = _this.updateQueryStringByString(url, paramName, paramObject[paramName]);
            });
            return url;
        },
        popMessage: function (element, time) {
            time = time || 2000;
            element.fadeIn();
            setTimeout(function () {
                element.fadeOut();
            }, time);
        },
        clearURLParams: function (url, paramArray) {
            var _this = this;
            _.each(paramArray, function (paramName) {
                url = _this.updateQueryStringByString(url, paramName, '');
            });
            return url;
        },
        SetSortBarParameterForURL: function (apiUrl, querystring) {
            apiUrl = this.updateQueryStringByObject(apiUrl, {
                'Ns': this.getParameterByNameFromString('Ns', querystring),
                'Dims': this.getParameterByNameFromString('Dims', querystring),
                'Type': this.getParameterByNameFromString('Type', querystring)
            });
            return apiUrl;
        },
        CheckAgeValidity: function (selectedYear) {
            selectedYear = parseInt(selectedYear, 10);
            if (new Date().getFullYear() - selectedYear <= 13) {
                return false;
            }
            return true;
        },
        waitForAsyncScript: function (name, callback) {
            var interval = 500,
                timeWaited = 0;
            function loadScripts() {
                if (timeWaited < 10000) {
                    //if window[name] is undefined we want to run this function again, but 0 will also be evaluated as falsy.
                    //So if the value is 0 that means this window[name] has been populated, we wanna trigger callback() in this case.
                    if (!window[name] && window[name] !== 0) {
                        setTimeout(function () {
                            timeWaited += interval;
                            loadScripts();
                        }, interval);
                    } else {
                        callback();
                    }
                } else {
                    console.error('Timed out on loading: ' + name);
                    return;
                }
            }
            loadScripts(interval);
        },
        setUrlToHideRecords: function (apiUrl, siteMember, excludeDefs) {
            var beginPart = 'AND(NOT(P_Primary_Key:',
                primaryKeysToExclude = utility.checkContentParticipation(siteMember),
                lastPart = excludeDefs ? '),NOT(ContentTypes:Definition))' : '))';

            if (primaryKeysToExclude.length > 0) {
                return this.updateQueryString(apiUrl, 'HideRecord', beginPart + primaryKeysToExclude.join('),NOT(P_Primary_Key:') + lastPart);
            } else {
                if (excludeDefs) {
                    apiUrl += '&HideRecord=NOT(ContentTypes:Definition)';
                }
                return apiUrl;
            }
        },
        isSubscribedToProduct: function (siteMember, productId) {
            var orgSubscribed = _.findWhere(siteMember.OrganizationProducts, {ProductID: productId, IsOrgProductActive: true});
            var memberSubscribed = _.findWhere(siteMember.Products, {RefProductID: productId, IsMemberProductActive: true});

            if (orgSubscribed || memberSubscribed) {
                return true;
            }
            return false;
        },
        isMemberSubscribedToProduct: function (siteMember, productId) {
            var memberSubscribed = _.findWhere(siteMember.Products, {RefProductID: productId, IsMemberProductActive: true});

            if (memberSubscribed) {
                return true;
            }
            return false;
        },
        isRoleActive: function (siteMember, roleId) {
            var roleActive = _.findWhere(siteMember.Roles, {RoleId: roleId, IsMemberRoleActive: true});

            if (roleActive) {
                return true;
            }
            return false;
        },
        checkContentParticipation: function (siteMember) {
            var primaryKeysToExclude = [];
            if (siteMember.IsAuthenticated === 'true') {
                primaryKeysToExclude = ['101-7416', '101-23826'];

                //if one organization praticipates they all get it.
                if (this.isSubscribedToProduct(siteMember, 4)) {
                    //If they are opted into ScholarshipSearch remove that primary key from the array to exclude
                    primaryKeysToExclude.splice($.inArray('101-7416', primaryKeysToExclude), 1);
                }
                if (this.isSubscribedToProduct(siteMember, 7)) {
                    //If they are opted into CCPTool remove that primary key from the array to exclude
                    primaryKeysToExclude.splice($.inArray('101-23826', primaryKeysToExclude), 1);
                }
            }
            return primaryKeysToExclude;
        },
        instantiateGlossaryTerms: function () {
            require(['configuration', 'jquery.zglossary'], function (Configuration) {
            // Build a JSON object with all definitions, to be used with the glossary jquery plugin.
                if (!terms.length) {
                    $.getJSON(Configuration.apiEndpointBases.GenericEndeca + 'glossary', function (data) {
                        _.each(data.secondaryContent[0].records, function (record) {
                            terms.push({
                                'term': record.attributes.term[0],
                                'definition': record.attributes.body[0]
                            });
                        });
                        $('.js-glossary-scope').glossary(terms);
                        SALT.trigger('glossary:fired');
                    });
                } else {
                    $('.js-glossary-scope').glossary(terms);
                    SALT.trigger('glossary:fired');
                }
            });
        },
        IsCurrentPage: function (page) {
            return (location.pathname.toLowerCase().indexOf(page ? page.toLowerCase() : '') === 0);
        },
        IsReferrerByCookie: function (ref) {
            if ($.cookie('referUrl')) {
                return ($.cookie('referUrl').indexOf(ref) !== -1);
            }
            return false;
        },
        IsReferrerByUrl: function (ref) {
            var referrer = document.referrer.toLowerCase();
            if (referrer) {
                return (referrer.indexOf(ref) >= 0);
            }
            return false;
        },
        trackLastVisitedURL: function () {
            if (this.IsReferrerByUrl('saltmoney') && !this.IsReferrerByUrl('logon') &&
                !this.IsReferrerByUrl('register') && !$.cookie('referUrl')) {
                $.cookie('referUrl', document.referrer, {path: '/', domain: '.saltmoney.org'});
            }
        },
        lastvisitedURL: function () {
            var url = $.cookie('referUrl');
            if (!url) {
                /* when empty return default */
                url = '/index.html';
            }

            /* SWD-7345: special events routing */
            return url;
        },
        getRerouteURL: function () {
            if (this.IsCurrentPage('/logon') || 
                this.IsCurrentPage('/register')) {
                return this.lastvisitedURL();
            } else {
                return this.getParameterByName('ReturnUrl');
            }
        },
        // checks the validity of deeply-nested object properties via dot
        // based on https://github.com/wilmoore/selectn
        checkNested: function (obj, query) {
            // normalize query to `.property` access (i.e. `a.b[0]` becomes `a.b.0`)
            query = query.replace(/\[(\d+)\]/g, '.$1');
            var parts = query.split('.'),
                len = parts.length;

            // iteratively save each segment's reference
            for (var i = 0; i < len; i++) {
                if (obj) {
                    obj = obj[parts[i]];
                }
            }
            return obj;
        },
        // convert string to title case, where each the first letter after a space is capitalized
        toTitleCase: function (toCase) {
            var titleCased;
            titleCased = toCase.toLowerCase().replace(/^(.)|\s(.)/g, function ($1) {
                return $1.toUpperCase();
            });

            return titleCased;
        },
        handleRegWallWT: function (action, removeCookie) {

            if ($('.js-reg-wall').is(':visible')) {
                if (!($.cookie('regWall'))) {
                    $.cookie('regWall', true);
                    action = 'Wall_Popup';
                }
            }

            if ($.cookie('regWall')) {
                SALT.publish('regWall', {
                    action: action
                });
            }
        },
        //updates target selector text from matching targetVal to new replaceVal text
        updateSelectorText: function (targetVal, replaceVal) {
            if (typeof targetVal === 'string' && typeof replaceVal === 'string') {
                $('.target-text').text(function () {
                    return $(this).text().replace(targetVal, replaceVal);
                });
            }
        },
        currencyComma: function (value) {
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        // If you update this, you should also update the function of the same name in saltUtils.js
        sortOrganizations: function (organizationsIn) {

            if (organizationsIn && (typeof organizationsIn === 'object')) {
                var sortOrgs = organizationsIn.sort(function (a, b) {
                    return b.EffectiveStartDate.localeCompare(a.EffectiveStartDate);
                });
                return sortOrgs;
            }
            else {
                return organizationsIn;
            }
        },
        // If you update this, you should also update the function of the same name in saltUtils.js
        returnBrandedOrganizations: function (organizationsIn) {
            if (organizationsIn && (typeof organizationsIn === 'object')) {

                var logoOrgs = organizationsIn.filter(function (el) {
                    return (el.Brand !== 'nologo');
                });
                return logoOrgs;
            }
            else {
                return organizationsIn;
            }
        },
        isScrolledIntoView: function ($elem) {
            var $window = $(window);

            var docViewTop = $window.scrollTop();
            var docViewBottom = docViewTop + $window.height();

            var elemTop = $elem.offset().top;
            var elemBottom = elemTop + $elem.height();

            return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
        },
        parseQuestionsAnswers: function (data) {
            // building a JSON object to pass to the backbone views to use when rendering dust
            var questionsAnswers = {},
                responses = {};
            // for each question in data...
            _.each(data.questions, function (question) {
                // from the questions object, build an object with QuestionID as the key, and the value is an object the contains:
                // Question = QuestionDescription(question text)
                // Answers = all available answers (options) for this question

                // If we've already concatenated the question and answer IDs, don't try to do it again.
                if (question.Answers && question.Answers.length > 0 && !isNaN(question.Answers[0].AnsID)) {
                    _.each(question.Answers, function (answer) {
                        answer.AnsID = question.QuestionID + '-' + answer.AnsID;
                    });
                }

                questionsAnswers[question.QuestionID] = { Question: question.QuestionDescription, Answers: question.Answers, QuestionID: question.QuestionID };
                // from the responses object, build an object with all user responses to the current question
                responses = _.where(this.responses, { QuestionExternalId: question.QuestionID });
                questionsAnswers[question.QuestionID].Responses = responses;
                // check if the question has a custom value stored, and add it to the question object at the base level for ease of access.
                var customValue = _.find(responses, function (response) {
                    return response.CustomValue;
                });
                questionsAnswers[question.QuestionID].otherAnswer = customValue ? customValue.CustomValue : null;
            }, data);
            return questionsAnswers;
        },
        setMemberResponses: function (formData) {
            var formAnswers = _.pairs(formData),
                profQandA = [];

            // pick the question we want to add to Sitemember.ProfialQsAndAs
            _.each(formAnswers, function (element) {
                // element is an array of size 2. [0] is the element's name, [1] is the element's value which is the unique ID of the answer.
                var currentValue = element[1];
                // element[0] (element ID) consists of 3 parts separated by "-":
                // 1) question type (multi answer, single asnwer)
                // 2) the string 'qid' by which we could tell if this question is in the RefProfileQuestion table or not
                // 3) the questions's unique ID in the RefProfileQuestion table.
                if (element[0].split('-')[1] === 'qid') {
                    // if an "other" question was asnwered the custom value is added to the element's value, read it, otherwise set it to null
                    var customVal = null;
                    if (element[0].split('-')[0] !== 'multi' || typeof currentValue === 'string') {
                        customVal = currentValue.split('-')[2] || null;
                        profQandA.push({ AnsExternalId: parseInt(currentValue.split('-')[1], 10), QuestionExternalId: parseInt(element[0].split('-')[2], 10), CustomValue: customVal });
                    } else {
                        _.each(currentValue, function (currentAnswerID) {
                            customVal = currentAnswerID.split('-')[2] || null;
                            profQandA.push({ AnsExternalId: parseInt(currentAnswerID.split('-')[1], 10), QuestionExternalId: parseInt(element[0].split('-')[2], 10), CustomValue: customVal });
                        });
                    }
                }
            });

            SALT.trigger('set:ProfileQandAs', profQandA);
            return profQandA;
        },
        prepareDashboardContent: function (json, goalRankResponses) {
            var potentialRecommendedTasks = {};
            var potentialLibraryTasks = {};
            var featuredTasks = {};
            var qAndARecords = {};
            var contentPrimarySelections = {};
            var contentSecondarySelections = {};
            var topicsSelectedPrimary = {};
            var topicsSelectedSecondary = {};

            var hasQAresponses = false;
            var hasTopicSelections = false;

            //We need to empty the already rendered objects for the MySALT section, as we might be rendering within an SPA session
            //And dont want to ignore duplicates from the previous sort/filter state
            utility.itemsAlreadyRendered.MySALT = {};

            json.goalRankResponses = goalRankResponses;

            //TODO, probably need a better data structure for this than simply adding properties to an object in order, we cant count on all browsers keeping that order
            //Add empty arrays for each goal, in the order the goals were ranked, so that when we combine these later, they are in order we will render them on the page
            _.each(goalRankResponses, function (response, ind, arr) {
                var goalWithNoWhitespace = response.AnsName.replace(new RegExp(' ', 'g'), '');
                qAndARecords[goalWithNoWhitespace] = [];
                //We need to empty the already rendered objects for the browse sections, as we might be rendering browse from an SPA session
                //And dont want to ignore duplicates from the previous sort/filter state
                utility.itemsAlreadyRendered[goalWithNoWhitespace] = {};
                potentialRecommendedTasks[goalWithNoWhitespace] = [];
                contentSecondarySelections[goalWithNoWhitespace] = [];
                contentPrimarySelections[goalWithNoWhitespace] = [];
                potentialLibraryTasks[goalWithNoWhitespace] = [];
                topicsSelectedPrimary[goalWithNoWhitespace] = [];
                topicsSelectedSecondary[goalWithNoWhitespace] = [];
            });

            //Populate featured tasks
            _.each(json.secondaryContent[0].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    featuredTasks[obj.title] = obj.records;
                }
            });

            //This cartridge contains organization specific featured task items.
            _.each(json.secondaryContent[7].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    //Put the org specific records first when combining
                    featuredTasks[obj.title] = obj.records.concat(featuredTasks[obj.title]);
                }
            });

            //Populate q+a tasks
            _.each(json.secondaryContent[1].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title] && obj.records.length) {
                    //The contents array would be empty and we wouldnt get into this function if there were no QA responses
                    //set the bool to be used in determining which scenario we are in
                    hasQAresponses = true;
                    //TODO implement a concat here rather than pushing just one record, in case any answers in the future link to multiple records rather than one
                    qAndARecords[obj.title].push(obj.records[0]);
                }
            });

            //Populate content selection secondary items
            _.each(json.secondaryContent[2].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    contentSecondarySelections[obj.title] = obj.records;
                }
            });

            //Populate content selection primary items
            _.each(json.secondaryContent[3].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title]) {
                    contentPrimarySelections[obj.title] = obj.records;
                }
            });

            //Populate topic selection primary items
            _.each(json.secondaryContent[4].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title] && obj.records.length) {
                    //The contents array would be empty and we wouldnt get into this function if there were no topics
                    //set the bool to be used in determining which scenario we are in
                    hasTopicSelections = true;
                    //TODO implement a concat here rather than pushing just one record, in case any answers in the future link to multiple records rather than one
                    topicsSelectedPrimary[obj.title].push(obj.records[0]);
                }
            });

            //Populate topic selection secondary items
            _.each(json.secondaryContent[5].contents, function (obj, ind, arr) {
                if (json.SiteMember.enabledGoals[obj.title] && obj.records.length) {
                    //TODO implement a concat here rather than pushing just one record, in case any answers in the future link to multiple records rather than one
                    topicsSelectedSecondary[obj.title].push(obj.records[0]);
                }
            });

            if (hasQAresponses && !hasTopicSelections) {
                //Loop through one of our goal rank ordered objects, using the property name parameter to combine the necessary cartridge data according to goal
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(qAndARecords[property]);
                    potentialLibraryTasks[property] = qAndARecords[property].concat(contentSecondarySelections[property]);
                });

                //Transform an object like { MasterMoney: [1,2], FindAJob: [3,4]} into a flat array like [1,2,3,4] so that our dust template can simply loop through the array and render records as tiles
                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
                //Add the ordered secondarySelections to the end of the recommended section
                //See the comment a couple lines above for what the .flatten & .values calls are doing
                potentialRecommendedTasks = potentialRecommendedTasks.concat(_.flatten(_.values(contentSecondarySelections)));
            } else if (!hasQAresponses && !hasTopicSelections) {
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(contentPrimarySelections[property]);
                    potentialLibraryTasks[property] = contentPrimarySelections[property].concat(contentSecondarySelections[property]);
                });
                //Transform an object like { MasterMoney: [1,2], FindAJob: [3,4]} into a flat array like [1,2,3,4] so that our dust template can simply loop through the array and render records as tiles
                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
                //Add the ordered secondarySelections to the end of the recommended section
                //See the comment a couple lines above for what the .flatten & .values calls are doing
                potentialRecommendedTasks = potentialRecommendedTasks.concat(_.flatten(_.values(contentSecondarySelections)));
            } else if (!hasQAresponses && hasTopicSelections) {
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(topicsSelectedPrimary[property]).concat(topicsSelectedSecondary[property]);

                    //For goals where the user didnt select any topics, use the contentPrimarySelections list
                    if (!topicsSelectedPrimary[property].length) {
                        potentialRecommendedTasks[property] = potentialRecommendedTasks[property].concat(contentPrimarySelections[property]);
                    }

                    potentialLibraryTasks[property] = topicsSelectedPrimary[property].concat(topicsSelectedSecondary[property]);
                });

                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
            } else {
                _.each(featuredTasks, function (goal, property, obj) {
                    potentialRecommendedTasks[property] = featuredTasks[property].concat(qAndARecords[property]).concat(topicsSelectedPrimary[property]).concat(topicsSelectedSecondary[property]);
                    potentialLibraryTasks[property] = qAndARecords[property].concat(topicsSelectedPrimary[property]).concat(topicsSelectedSecondary[property]);
                });

                potentialRecommendedTasks = _.flatten(_.values(potentialRecommendedTasks));
            }

            //Get rid of any added, complete, inProgress or duplicates items, they won't be shown on MySALT
            var incompletePotentialTasks = utility.removeItems(potentialRecommendedTasks);

            //Populate the two sections of the To-do/tasks tab (incomplete and complete are the two sections)
            json.openTasks = _.filter(json.mainContent[0].records, function (record) {
                return record.attributes.RefToDoStatusID === 1 || record.attributes.RefToDoStatusID === 4;
            });
            //Sort the open tasks by date, most recent at the top
            json.openTasks = json.openTasks.sort(lastModifiedComparer);
            //count the open todo tasks. If tasks more than 0 apply class active counter else null.
            json.openTodoCount = json.openTasks.length;
            json.openTodoCountClass = json.openTodoCount ? 'active-counter' : '';

            json.completedTasks = _.filter(json.mainContent[0].records, function (record) {
                return record.attributes.RefToDoStatusID === 2;
            });

            //Sort the completed tasks by date, most recent at the top
            json.completedTasks = json.completedTasks.sort(lastModifiedComparer);

            json.libraryTasks = potentialLibraryTasks;

            //Loop through the potential MySALT tasks, finding the index of the first item that is not in progress
            var firstMatchIndex = 0;
            _.find(incompletePotentialTasks, function (obj) {
                if (obj.attributes.RefToDoStatusID !== 4) {
                    return true;
                }
                firstMatchIndex++;
                return false;
            });
            //Use the index found above to splice that record out of the array and set it as the featured task
            json.featuredTask = incompletePotentialTasks.splice(firstMatchIndex, 1)[0];
            //The remaining "incompletePotentialTasks" object becomes the "Recommended" list on MySALT
            json.recommendedTasks = incompletePotentialTasks;

            //Populate the map object for the MySALT/Recommended page, which contains all of the primary keys already shown
            //so that we dont render duplicates when load more is clicked
            _.each(json.recommendedTasks, function (obj, ind, arr) {
                utility.itemsAlreadyRendered.MySALT[obj.attributes.P_Primary_Key[0]] = true;
            });
            if (json.featuredTask !== undefined) {
                utility.itemsAlreadyRendered.MySALT[json.featuredTask.attributes.P_Primary_Key[0]] = true;   
            }
            //Populate the map object for each of the browse page sections, which contains all of the primary keys already shown, and remove duplicates within a section
            _.each(json.libraryTasks, function (goal, goalName, libraryTasks) {
                //Using _.filter to filter out records that are duplicates
                goal = _.filter(goal, function (task) {
                    if (utility.itemsAlreadyRendered[goalName][task.attributes.P_Primary_Key[0]]) {
                        //We've already added this item to this goal/section, return false so that this copy of the record is not added to the list (we don't want duplicates)
                        return false;
                    } else {
                        //Not in the map yet, its a new item, add the primary key to the list of items we have already seen and return true so that we dont filter out this record
                        utility.itemsAlreadyRendered[goalName][task.attributes.P_Primary_Key[0]] = true;
                        return true;
                    }
                });
                //"goal" is a copy of the array that we want to be operating on, set the value to libraryTasks, which is a reference to the json.libraryTasks object
                libraryTasks[goalName] = goal;
            });
            return json;
        },
        removeItems: function (tasksToFilter) {
            //Map to keep track of the ids in our result set so we can get rid of duplicates
            var primaryKeyMap = {};

            return _.filter(tasksToFilter, function (task) {
                var notADuplicate = false;
                //console.log(task);
                if (!primaryKeyMap[task.attributes.P_Primary_Key[0]]) {
                    notADuplicate = true;
                    //Add it to the map so that we will ignore any future instances of this item
                    primaryKeyMap[task.attributes.P_Primary_Key[0]] = true;
                }
                //exclude Added, Complete and InProgress items
                var excludeStatusId = task.attributes.RefToDoStatusID === 1 || task.attributes.RefToDoStatusID === 2 || task.attributes.RefToDoStatusID === 4;
                return !excludeStatusId && notADuplicate;
            });
        },
        topScroll: function () {
            if (document.body.scrollHeight > 200) {
                $('html, body').animate({ scrollTop: 0 }, 500);
            }
        },
        lookupUserSegment: function (data, userSegmentValue, org) {
            //Default to using brand name as the path lookup in the data
            var keyPath = org.Brand;
            if (keyPath === 'nologo') {
                //If this org is branded no-logo it still may be using user segments.  Try OrgName as the key rather than brand.
                keyPath = org.OrganizationName;
            }
            if (data[keyPath]) {
                if (userSegmentValue) {
                    userSegmentValue += '|' + data[keyPath];
                } else {
                    userSegmentValue = 'UserSegment=' + data[keyPath];
                }
            }

            return userSegmentValue;
        },
        findUserSegment: function (responseObj) {
            $.getJSON('/Assets/Scripts/js/salt/userSegment.json')
                .done(function (json) {
                    var userSegmentValue;

                    _.each(responseObj.Organizations, function (org, ind, list) {
                        userSegmentValue = utility.lookupUserSegment(json, userSegmentValue, org);
                    });

                    if (userSegmentValue) {
                        document.cookie = userSegmentValue + '; PATH=/';
                    } else {
                        document.cookie = 'UserSegment=Default; PATH=/';
                    }
                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ', ' + error;
                });
        },
        getInitialOrder: function (obj) {
            var num = 1;
            $(obj).each(function () {
                //Set the value of the input to its position in the array, so the first label gets "1", second "2" and so on.
                $(this).find('input').val(num);

                //We need to be able to tie the ranking buttons with how they have been ranked for automated testing
                //Set the data-externalid property from the ranking button onto the ranking label
                var $goalButtonAtThisIndex = $('.js-goal-rank').eq(num - 1);
                if ($goalButtonAtThisIndex.length) {
                    $(this).find('input').attr('data-externalid', $goalButtonAtThisIndex.attr('data-externalid'));
                }
                num++;
            });
        }
    };
    return utility;
});
