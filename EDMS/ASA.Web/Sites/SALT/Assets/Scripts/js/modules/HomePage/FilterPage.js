/* jshint maxstatements: 29 */
define([
    'require',
    'jquery',
    'salt',
    'salt/SortControlBar',
    'salt/models/SiteMember',
    'configuration',
    'asa/ASAUtilities',
    'underscore',
    'modules/HomePage/Views',
    'modules/Onboarding',
    'modules/ReportingStatus',
    'jquery.cookie',
    'salt/SortControlBarDelegate'
], function (require, $, SALT, SortControlBar, SiteMember, Configuration, Utility, _, pageHelpers, onboarding, reportingStatus) {
    var currentlySelectedGoal = '',
        tags = '',
        numberOfTilesPerPage = 8,
        infiniteScrollObject = {},
        lastScrollPosition = 0,
        $footerContainer = $('.footer-container'),
        useTodoTileDesign = $.cookie('useTodoTileDesign');

    function shimArgumentsObj(argsObj, numberOfFetches) {
        //When we are fetching for multiple goals the "arguments" object is an array of arrays
        //When we are fetching for a single goal, "arguments" is a flat array
        //Shim the single fetch case to have the same structure so that our "each" loop can operate on ajaxResult[0] no matter which case.
        var results;
        if (numberOfFetches === 1) {
            results = [argsObj];
        } else {
            results = argsObj;
        }
        return results;
    }

    function decorateGoalURLs(goals, goalFilter, querystring) {
        var goalString = '';
        //We need to loop through the goals (they should be sorted by ranking)
        //For each goal, add the tags from the one(s) before it to the filter param so that they will be excluded from the results, so that we do not get duplicates across sections
        _.each(goals, function (goal, goalName, arr) {
            var goalMap = _.findWhere(Utility.paramToGoalInfo, {name: goalName});
            //We only want to decorate the urls if we have no goalFilter, or if we have a filter and the goal we are on is in the filter.
            if (!goalFilter || (goalFilter && goalFilter.indexOf(goalMap.paramName) > -1)) {
                //Add the querystring to the base url to start building the url to be fetched
                goalMap.url = goalMap.baseUrl + '&' + querystring;
                //Having an empty filter param messes up what endeca returns, so we only want to have an Nr param for the last three goals
                if (goalString) {
                    //The NOT clasuses within the wrapping AND clause cannot end in a comma so we need to get rid of the last character
                    goalMap.url += '&Nr=AND(' + goalString.substring(0, goalString.length - 1) + ')';
                }
                //Add the tags for the goal to the goalString, so that they will be added to the next goal filter param
                goalString += goalMap.tags;
            }
        });

    }

    var filterPage = {
        renderFilterPage: function (querystring, isDashboard) {
            var _this = this,
                apiUrl,
                template,
                scrollToTop,
                jiveURL,
                searchTerm,
                searchType,
                $mainContent = $('.js-main-content'),
                $sideBarContent = $('#js-sidebar-content'),
                isSearchPage = (querystring && querystring.indexOf('searchCriteria') > -1);

            // if infiniteScrollObject.origin !== document.URL, delete session stored tiles.
            if (infiniteScrollObject.origin !== document.URL) {
                infiniteScrollObject = {};
                // to make sure we only load previous tiles with the browser's back button
                infiniteScrollObject.origin = document.URL;
                scrollToTop = true;
            }
            SiteMember.done(function (siteMember) {
                searchTerm = unescape(Utility.getParameterByNameFromString('searchCriteria', querystring).replace(/[+]/g, ' ').trim());
                searchType = unescape(Utility.getParameterByNameFromString('ntk', querystring).replace(/[+]/g, ' ').trim());

                //Put the search term text into the header's search box
                if ($('.js-simple-header').length) {
                    $('#searchCriteria').val(searchTerm);
                }
                if (querystring && querystring.indexOf('searchCriteria') > -1) {
                    //SWD-7621: when searching by tags, make first letter of each tag upper case
                    if (searchType === 'Tags') {
                        searchTerm = Utility.toTitleCase(searchTerm);
                    }
                    apiUrl = _this.buildSearchUrl(querystring, siteMember);
                    template = 'searchResultsBody';
                } else {
                    apiUrl = _this.buildFilterUrl(querystring, siteMember);
                    template = 'partial_homepage_body';
                }
                _this.updatePageTitle(querystring);
                /*Since we are doing partially render, the infiniteScroll could still be attached from previous page,
                so unbind scroll first whenever a new content gets rendered*/
                $(window).unbind('scroll', this.throttledScroll);

                $.getJSON(apiUrl).done(function (json) {
                    json = SortControlBar.populateDropdown(json);
                    json = SortControlBar.checkNoBannerTreatment(json, querystring);
                    pageHelpers.addSchoolToContext(json);
                    json.tagsForGoal = tags[currentlySelectedGoal];
                    json.useTodoTileDesign = useTodoTileDesign;
                    json.isDashboard = isDashboard || '';
                    if (template === 'searchResultsBody') {
                        document.title = searchTerm + ' - Search Results - SALT';

                        //if it's search page, set param so that 4 goals and featured options will be hidden
                        json.hideGoals = true;
                        require(['salt/analytics/webtrends-config'], function (webtrendsConfig) {
                            var numSearchResults = json.mainContent[0].contents[0].totalNumRecs,
                                searchKey = '',
                                contentGroupType = '',
                                searchAuthorName = '',
                                searchCrumbs = json.secondaryContent[1].searchCrumbs,
                                searchTerm = searchCrumbs.length ? searchCrumbs[0].terms : '',
                                searchPageNum = Math.ceil(json.mainContent[0].contents[0].firstRecNum / json.mainContent[0].contents[0].recsPerPage),
                                WTTags = webtrendsConfig.tags;

                            if (searchCrumbs && Utility.checkNested(json, 'secondaryContent[1].searchCrumbs[0]')) {
                                searchKey = searchCrumbs[0].key;
                                if (searchKey === 'Tags') {
                                    contentGroupType = searchTerm;
                                }
                                else if (searchKey === 'author-name') {
                                    searchAuthorName = searchTerm;
                                }
                            }

                            var pageServerCallIdentifier = webtrendsConfig.SERVER_CALL_IDENTIFIERS.SALT_CUSTOM_EVENT,
                                pageWTTransaction = '';

                            if (siteMember.DashboardEnabled) {
                                pageServerCallIdentifier = webtrendsConfig.SERVER_CALL_IDENTIFIERS.NAMED_ANCHOR,
                                pageWTTransaction = 'SALT3';
                            }
                            dcsMultiTrack(
                                WTTags.SITE,                    'salt',
                                WTTags.EXTERNAL_VISITOR_ID, $('meta[name="WT.dcsvid"]').attr('content'),
                                WTTags.DCS_AUTHENTICATED_USERNAME, $('meta[name="DCS.dcsaut"]').attr('content'),
                                WTTags.CONTENT_GROUP,           'Search',
                                WTTags.CONTENT_SUBGROUP,        'Search Results',
                                WTTags.ONSITE_SEARCH_WORD,      searchTerm,
                                WTTags.NUM_SEARCH_RESULTS,      numSearchResults,
                                WTTags.SEARCH_PAGE_NUMBER,      searchPageNum,
                                WTTags.SERVER_CALL_IDENTIFIER,  pageServerCallIdentifier,
                                WTTags.CG_TYPE,                 contentGroupType,
                                WTTags.AUTHOR_NAME,             searchAuthorName,
                                WTTags.ACTIVITY_TRANSACTION,    pageWTTransaction
                            );
                        });
                    }
                    json.configuration = json.configuration || {};
                    json.configuration.mm101B = Configuration.mm101.url;
                    //only load the school logo for authenticated users on search results page and if there is not already a logo on the sidebar
                    Utility.renderDustTemplate(template, json, function (err, out) {
                        infiniteScrollObject.savedQueryString = infiniteScrollObject.scrollQuerystring || querystring;
                        //assign configuration value so scroll will be able to access it
                        infiniteScrollObject.configuration = {};
                        infiniteScrollObject.configuration.mm101B = json.configuration.mm101B;

                        // if number is avaiable use if else keep default of 8
                        if (json.mainContent[0].contents[0].recsPerPage) {
                            numberOfTilesPerPage = json.mainContent[0].contents[0].recsPerPage;
                        }
                        var newFilterHTML = $.parseHTML(out);
                        //508 Compliance, SWD-6602, reorder the tabbing structure.Start
                        $(newFilterHTML).find('.js-tabindex-3').attr('tabindex', '3');
                        //508 Compliance, end

                        $mainContent.html(newFilterHTML);
                        _this.renderSidebarCarousel(json);

                        pageHelpers.reTriggerInlineScripts(newFilterHTML);

                        /*Only attach infiniteScroll when it's not featured page
                        On Featured page, we want to render only 8 static records.*/
                        if (json.sortControlBarLeftVal !== 'Featured') {
                            $(window).unbind('scroll', this.throttledScroll);
                            // if we are on the search page, and the first fetch was for a definition, and have no tiles, call renderInfiniteScroll to load content tiles
                            if (isSearchPage && !$('.js-tileContainer').length && $('.js-definition-result').length) {
                                _this.renderInfiniteScroll(infiniteScrollObject);
                            } else if ($('.js-tileContainer').length === 1) {
                                //When we only have 1 record, we need to call render again, rather than just attach, so that we can fill the first row with a second record
                                //Otherwise the user would only see one tile, and need to start scrolling to see more.
                                infiniteScrollObject.savedQueryString += '&No=1';
                                _this.renderInfiniteScroll(infiniteScrollObject);
                            }
                            else {
                                _this.attachInfiniteScroll();
                            }
                        }

                        //determine if ReportingId is needed and adjust SaltCourse Urls.
                        var showReportIdQuestion = reportingStatus.checkReportIdStatus(siteMember);
                        reportingStatus.initOverlay(showReportIdQuestion);

                        SALT.trigger('populateMultiSelect:needed');
                        $('#' + currentlySelectedGoal).addClass('active-user-goal');
                        $(document).foundation();
                        SALT.trigger('SPA:PageViewed', json);
                        //hide reg wall on filter pages.
                        $('.js-reg-wall').hide();
                        //determine if SALT Tour should be displayed.
                        pageHelpers.openSaltTourOverlay();
                    }, null, scrollToTop);
                });
            });
        },
        updatePageTitle: function (querystring) {
            //Page title requirements per SWD-4770
            var typeParam = Utility.getParameterByNameFromString('Type', querystring),
                tagParam  = Utility.getParameterByNameFromString('Tag', querystring),
                tagToUse = tagParam ? '- ' + Utility.toTitleCase(tagParam): '';

            if (typeParam && typeParam !== 'All') {
                //Transform to add spaces between words e.g. 'RepayStudentDebt' --> 'Repay Student Debt'
                document.title = typeParam.replace(/[A-Z]/g, ' ' + '$&').trim() + tagToUse.trim() + ' - Salt';
            } else {
                switch (Utility.getParameterByNameFromString('Dims', querystring)) {
                case '43':
                    document.title = 'Forms - Salt';
                    break;
                case '46':
                    document.title = 'Tools - Salt';
                    break;
                case '44':
                    document.title = 'Lessons - Salt';
                    break;
                default:
                    document.title = 'Salt: Education Unlocked. Dreams Unlimited.';
                }
            }
        },
        buildDashboardUrl: function (siteMember) {
            var apiUrl = Configuration.apiEndpointBases.GenericEndeca + '/DashboardPrototypeMK',
                userSegment = $.cookie('UserSegment');

            //Set UserSegment to apiURL.
            apiUrl = Utility.handleUserConfiguration(apiUrl, userSegment);
            /*Set apiurl for those tiles need to be hidden*/
            apiUrl = Utility.setUrlToHideRecords(apiUrl, siteMember);

            return apiUrl;
        },
        buildFilterUrl: function (querystring, siteMember) {
            querystring = querystring ? querystring : '';
            var apiUrl = Configuration.apiEndpointBases.AuthHome,
                typeVal = '',
                userSegment = '',
                saltSuggestObject = '',
                currentTag = Utility.getParameterByNameFromString('Tag', querystring);
            typeVal = Utility.getParameterByNameFromString('Type', querystring);
            var goalRegex = /^(MasterMoney|FindAJob|PayForSchool|RepayStudentDebt)$/,
                personaRegex = /^(AssociatesDegree|BachelorsDegree|GraduateSchool|AlumniPostSchool)$/;
            if (goalRegex.test(typeVal)) {
                apiUrl += '/' + typeVal;
                saltSuggestObject = this.getSaltSuggestSegment(typeVal, currentTag, currentlySelectedGoal);
                currentlySelectedGoal = typeVal;
                userSegment = saltSuggestObject.userSegment;
            } else if (!typeVal || personaRegex.test(typeVal)) {
                apiUrl += '/Personalized';
                currentlySelectedGoal = '';
                userSegment = $.cookie('UserClass');
            } else {
                currentlySelectedGoal = '';
                userSegment = $.cookie('UserSegment');
            }
            /*Base URL*/
            apiUrl += '/_/N-14';
            /*pass param object to update apiURL*/
            apiUrl = Utility.updateQueryString(apiUrl, {
                'No': Utility.getParameterByNameFromString('No', querystring),
                'Ntk': 'Tags',
                'Ntx': 'mode+matchany',
                'Ntt': saltSuggestObject.nttVal ? saltSuggestObject.nttVal : ''
            });
            //Set UserSegment to apiURL.
            apiUrl = Utility.handleUserConfiguration(apiUrl, userSegment);
            /*Set Sort Bart Parameter to base api url*/
            apiUrl = Utility.SetSortBarParameterForURL(apiUrl, querystring);
            /*Replace /GetSearch by GetMedia if there is any*/
            apiUrl = apiUrl.replace('/GetSearch', '/GetMedia');
            /*Set apiurl for those tiles need to be hidden*/
            apiUrl = Utility.setUrlToHideRecords(apiUrl, siteMember);

            return apiUrl;
        },
        buildSearchUrl: function (querystring, siteMember, excludeDefs) {
            /*The base url*/
            var apiUrl = Configuration.apiEndpointBases.SearchPage,
                userSegment = '',
                matchMode = 'mode+matchany',
                recordSearchKey = Utility.getParameterByNameFromString('ntk', querystring) || 'ContentSearch';
            if (recordSearchKey === 'Tags' || recordSearchKey === 'author-name') {
                matchMode = 'mode+matchall';
            }

            apiUrl = Utility.updateQueryString(apiUrl, {
                'Ntk': recordSearchKey,
                'Ntx': matchMode,
                'Ntt': Utility.getParameterByNameFromString('searchCriteria', querystring)
            });

            if (!Utility.getParameterByNameFromString('No', apiUrl)) {
                if (Utility.getParameterByNameFromString('No', querystring)) {
                    apiUrl = Utility.updateQueryString(apiUrl, 'No', Utility.getParameterByNameFromString('No', querystring));
                } else {
                    apiUrl = Utility.updateQueryString(apiUrl, 'No', '0');
                }
            }

            if ($.cookie('UserSegment')) {
                userSegment = $.cookie('UserSegment');
            }

            apiUrl = Utility.handleUserConfiguration(apiUrl, userSegment);
            /*Set Sort Bart Parameter to base api url*/
            apiUrl = Utility.SetSortBarParameterForURL(apiUrl, querystring);
            /*Set apiurl for those tiles need to be hidden*/
            apiUrl = Utility.setUrlToHideRecords(apiUrl, siteMember, excludeDefs);
            return apiUrl;
        },
        attachInfiniteScroll: function () {
            /*infinite scroll needs to stop when it reaches the end*/
            if (!$('.js-no-records').length) {
                $(window).scroll(this.throttledScroll);
            }
        },
        infiniteScrollHandler: function () {
            if ($(window).scrollTop() > lastScrollPosition) {
                $footerContainer.removeClass('bottom-fixed');
                // check if #scroll-waypoint is at 120% from the top of the viewport
                if ($('#scroll-waypoint').offset().top - $(window).scrollTop() < window.innerHeight + (20 * (window.innerHeight / 100))) {
                    $('.js-profile-saved-img').last().show();
                    $(window).unbind('scroll', this.throttledScroll);
                    if (infiniteScrollObject.savedQueryString && infiniteScrollObject.savedQueryString.substring(0, 1) !== '?') {
                        infiniteScrollObject.savedQueryString = '?' + infiniteScrollObject.savedQueryString;
                    }
                    if (!Utility.getParameterByNameFromString('No', infiniteScrollObject.savedQueryString)) {
                        infiniteScrollObject.savedQueryString = Utility.updateQueryString(infiniteScrollObject.savedQueryString, 'No', 0);
                    }
                    var pageIndex = Utility.getParamStringFromURL('No', infiniteScrollObject.savedQueryString);
                    var incrementedPageIndex = pageIndex.replace(pageIndex.split('=')[1], parseInt(pageIndex.split('=')[1], 10) + numberOfTilesPerPage);
                    infiniteScrollObject.savedQueryString = infiniteScrollObject.savedQueryString.replace(pageIndex, incrementedPageIndex);
                    this.renderInfiniteScroll(infiniteScrollObject);
                }
            } else {
                $footerContainer.addClass('bottom-fixed');
            }
            lastScrollPosition = $(window).scrollTop();
        },
        renderInfiniteScroll: function (scrollObject) {
            var _this = this,
                querystring = scrollObject.savedQueryString,
                apiUrl,
                template = 'tiles';
            SiteMember.done(function (siteMember) {
                if (querystring.indexOf('searchCriteria') > -1) {
                    apiUrl = _this.buildSearchUrl(querystring, siteMember, true);
                } else {
                    apiUrl = _this.buildFilterUrl(querystring, siteMember);
                }
                $.getJSON(apiUrl).done(function (json) {
                    json = SortControlBar.populateDropdown(json);
                    json.useTodoTileDesign = useTodoTileDesign;

                    json.configuration = scrollObject.configuration;

                    pageHelpers.addSchoolToContext(json);
                    Utility.renderDustTemplate(template, json, function (err, out) {
                        var parsedHTML = $.parseHTML(out);
                        //508 Compliance, SWD-6602, reorder the tabbing structure. start
                        $(parsedHTML).find('.js-tabindex-3').attr('tabindex', '3');
                        //508 Compliance end
                        pageHelpers.reTriggerInlineScripts(parsedHTML);
                        $('.js-profile-saved-img').hide();
                        $('.js-home-tiles').append(parsedHTML);

                        //determine if ReportingId is needed and adjust SaltCourse Urls.
                        var showReportIdQuestion = reportingStatus.checkReportIdStatus(siteMember);
                        reportingStatus.initOverlay(showReportIdQuestion);

                        infiniteScrollObject.scrollQuerystring = querystring;

                        _this.attachInfiniteScroll();
                        SALT.trigger('populateMultiSelect:needed');
                        $(document).foundation();
                    }, null);
                });
            });
        },
        getSaltSuggestSegment: function (typeVal, currentTag) {
            var toReturn = {};
            /*Only make the ajax call when tags don't exist*/
            if (!tags) {
                $.ajax({
                    type: 'GET',
                    async: false,
                    url: '/Assets/Scripts/js/salt/TagsForGoals.json',
                    contentType: 'application/json; charset=utf-8'
                }).done(function (json) {
                        tags = json;
                    });
            }
            if (!currentTag) {
                toReturn.userSegment = 'SaltSuggests-default';
                toReturn.nttVal = '"' + tags[typeVal].join('"+"') + '"';
            } else {
                toReturn.userSegment = 'SaltSuggests-' + currentTag.replace(/\s+/g, '-');
                toReturn.nttVal = '"' + currentTag + '"';
            }
            return toReturn;
        },
        renderSidebarCarousel: function (json) {
            var carouselObject = _.where(json.secondaryContent, {name: 'Carousel'});

            if (Utility.checkNested(carouselObject[0], 'records[0].attributes.P_Primary_Key[0]')) {
                require(['modules/Carousel'], function (render) {
                    _.each(carouselObject[0].records, function (el, ind, arr) {
                        render(el.attributes.P_Primary_Key[0]);
                    });
                });
            }
        },
        renderDashboard: function (querystring) {
            var _this = this;

            SiteMember.done(function (siteMember) {
                var apiUrl = _this.buildDashboardUrl(siteMember);
                $.getJSON(apiUrl).done(function (json) {
                    pageHelpers.addSchoolToContext(json);
                    json.configuration = json.configuration || {};
                    json.configuration.mm101B = Configuration.mm101.url;
                    json.configuration.orgAdmin = siteMember.OrgAdmin ? "Admin" : "";
                    json = Utility.prepareDashboardContent(json, siteMember.goalRankResponses);
                    Utility.renderDustTemplate('partial_dashboard', json, function (err, out) {
                        console.log(err);
                        filterPage.buildBrowseContent(querystring, json, siteMember);
                        //Parse html strips out scripts, call 'reTriggerInlineScripts' to re-trigger inline scripts
                        var newFilterHTML = $.parseHTML(out);
                        //508 Compliance, SWD-6602, reorder the tabbing structure.Start
                        $(newFilterHTML).find('.js-tabindex-3').attr('tabindex', '3');
                        //508 Compliance, end
                        $('.js-main-content').html(newFilterHTML);
                        //The parsehtml call above strips out scripts from the rendered dust html
                        //Make sure to re-trigger scripts so we gets ratings, video, etc
                        pageHelpers.reTriggerInlineScripts(newFilterHTML);
                        SALT.trigger('SPA:PageViewed', json);
                        //We need to re-initialize foundation because of the tabs that we've inserted
                        $(document).foundation();
                        // 508 compliance, keeps the tab shell (which we control) tabbable, not the tab link
                        $('.tabs').on('toggled', function (e, tab) {
                            $('.js-dash-nav').prop('tabindex', '2');
                            tab.add(tab.children()).prop('tabindex', '-1');
                        });
                        $('.js-dash-nav a').prop('tabindex', '-1');
                    }, null, true);
                });
            });
        },
        renderOnboarding: function (querystring) {
            SiteMember.done(function (siteMember) {
                var context = {},
                    showOverlay = false;
                if (siteMember.goalRankResponses.length) {
                    context.previouslyRankedGoals = true;
                    context.goalRankResponses = siteMember.goalRankResponses;
                } else {
                    showOverlay = true;
                }
                //Add the question and answer data, as well as any responses, in order to render the onboarding questions with prefilled data if there is any
                context.ProfileQandAs = Utility.parseQuestionsAnswers({ questions: siteMember.ProfileQAs, responses: _.last(siteMember.ProfileQAs).Responses });
                context.SiteMember = siteMember;
                //enabledGoalCount was not getting recognized in onboarding.dust. This has been added back in this step.
                var enabledGoalCount = 0;
                for (var goal in siteMember.enabledGoals) {
                    if (siteMember.enabledGoals[goal]) {
                        enabledGoalCount++;
                    }
                }
                context.enabledGoalCount = enabledGoalCount;
                Utility.renderDustTemplate('Modules/Onboarding', context, function (err, out) {
                    SALT.publish('onboarding:action:taken', {
                        step_num: '1'
                    });
                    require(['libs/Sortable.min'], function (Sortable) {
                        Sortable.create(document.getElementById('sortable-goals'), {
                            onUpdate: function (evt) {
                                SALT.trigger('goalrank:updated');
                            },
                        });
                        Utility.getInitialOrder('#goal-rank-num li');
                        SALT.trigger('goalrank:rendered');
                    });
                    SALT.trigger('SPA:PageViewed', context);
                    if (showOverlay) {
                        $(document).foundation('reflow');
                        $(document).on('opened.fndtn.reveal', '.js-instr[data-reveal]', function (e) {
                            $(e.target).find('a').first().focus();
                        });
                        SALT.openOverlay('#onboardingHelpOverlay1');
                    } else {
                        $('#goal-rank-num input').first().focus();
                    }
                    document.cookie = 'NeedsOnboarding = true; PATH=/; expires=Mon, 9 Oct 2012 20:47:11 UTC;';
                }, '.js-main-content');
            });
        },
        buildBrowseContent: function (querystring, json, siteMember) {
            var dimMap = {
                41: 'Article',
                42: 'Video',
                43: 'Form',
                44: 'Lesson',
                45: 'Infographic',
                46: 'Tool',
                157: 'Comic',
                303: 'Ebook',
                304: 'Course'
            };
            //Sometimes the querystring will be "null", change it empty string so that we aren't adding null to the url's we fetch
            if (!querystring) {
                querystring = '';
            }

            //Check if there is a querystring parameter to filter the goals the user wants to see
            var goalFilter = Utility.getParameterByName('Goals');

            decorateGoalURLs(json.libraryTasks, goalFilter, querystring);

            //Now that the base urls have been assigned to paramToGoalInfo, add any organization opt-out records to the querystring
            _.each(Utility.paramToGoalInfo, function (obj, ind, list) {
                obj.url = Utility.setUrlToHideRecords(obj.url, json.SiteMember);
            });

            if (goalFilter) {
                //We need to determine which goals are not included in the goalFilter parameter.
                //For the excluded goals, we need to empty the contents of the array in json.libraryTasks, so that section wont be shown
                //And so we dont try to evaluate any of those records for the other sort/filter params.
                _.each(Utility.paramToGoalInfo, function (goalObj, paramName, obj) {
                    if (goalFilter.indexOf(paramName) === -1) {
                        json.libraryTasks[goalObj.name] = [];
                    }
                });
            }

            if (Utility.getParameterByName('Ns')) {
                var goalsToFetch = [];

                //If we have a goal filter, only fetch data for the goals they want to see
                if (goalFilter) {
                    //The param will look something like "MM,RSD,FJ", split it at the commas to get an array of the goals to be fetched
                    var goals = goalFilter.split(',');
                    _.each(goals, function (goal, ind, goalsArr) {
                        //Use the map object to translate the param value, to a url to be fetched, push this url onto the array of urls to be fetched
                        goalsToFetch.push($.getJSON(Utility.paramToGoalInfo[goal].url + '&No=0'));
                    });
                } else {
                    //No goal filter, we want to request data for all of the goals
                    goalsToFetch = [$.getJSON(Utility.paramToGoalInfo.RSD.url + '&No=0'), $.getJSON(Utility.paramToGoalInfo.MM.url + '&No=0'), $.getJSON(Utility.paramToGoalInfo.FJ.url + '&No=0'), $.getJSON(Utility.paramToGoalInfo.PS.url + '&No=0')];
                }
                $.when.apply($, goalsToFetch).done(function () {
                    var results = shimArgumentsObj(arguments, goalsToFetch.length);
                    //Populate the libraryTasks object for each goal that we made an ajax request for
                    _.each(results, function (ajaxResult, ind, arr) {
                        json.libraryTasks[ajaxResult[0].name] = ajaxResult[0].mainContent[0].records;
                        //Update the itemsAlreadyRendered object we keep, so that we dont render duplicates within a section when load more is clicked
                        _.each(ajaxResult[0].mainContent[0].records, function (obj, index, list) {
                            Utility.itemsAlreadyRendered[ajaxResult[0].name][obj.attributes.P_Primary_Key] = true;
                        });
                    });

                    filterPage.renderBrowse(json, siteMember);
                });
            } else {
                //Check if we have a content type/language filter in the URL
                var filter = Utility.getParameterByName('Dims');
                if (filter) {
                    //Object to hold the filtered libraryTasks object, so that we are not modifying the object we are looping through
                    var tasks = {};
                    var spanishFilterPresent = filter.indexOf('104') > -1;
                    //The filter param comes in the form of a comma-separated list, e.g. "41,42,47"
                    var filterArray = filter.split(',');

                    //For each "goal" loop through the candidate records and filter out any that don't match
                    _.each(json.libraryTasks, function (potentialTasks, goalName, libraryTasks) {
                        //Populate the tasks object with the result of the filter
                        tasks[goalName] = _.filter(potentialTasks, function (record) {
                            var matched = false;
                            //Short circuit out and return false for this record if we have a spanish filter present, and this item does not have spanish available
                            //no need to check if it matches the other content types in the filter if it doesnt match the spanish filter
                            if (spanishFilterPresent && record.attributes.Language.indexOf("Spanish") === -1) {
                                return false;
                            }
                            //Have to check each content type in the filter to see if it matches the record's content type.
                            _.each(filterArray, function (dimValue, ind, arr) {
                                if (record.attributes.ContentTypes[0] === dimMap[dimValue]) {
                                    matched = true;
                                }
                            });
                            return matched;
                        });
                    });
                    //Now that we are done looping through library tasks, reset this object to be the filtered result
                    json.libraryTasks = tasks;
                }
                //Our curated content might serve less than 3 records in a section.
                //In this case(s) we need to fetch from that goal's backfill to fill out content for that section
                var backfillFetch = [];

                //Loop through each goal, if there are less than 3 records in that goal's potential set, and the goal is in the goal filter or there is no filter (all goals showing)...
                //add the goal to the array of urls to fetch
                _.each(Utility.paramToGoalInfo, function (goalObj, paramName, obj) {
                    if (json.SiteMember.enabledGoals[goalObj.name]) {
                        if (json.libraryTasks[goalObj.name].length < 3 && ((goalFilter && goalFilter.indexOf(paramName) !== -1) || !goalFilter)) {
                            //Use the url for the goal, adding the popularity sort, as we are on backfill for "Recommended" which is sorted by popularity
                            backfillFetch.push($.getJSON(goalObj.url + '&Ns=P_Hits_Last_30_Days|1&No=0'));
                        }
                    }
                });

                $.when.apply($, backfillFetch).done(function () {
                    var results = shimArgumentsObj(arguments, backfillFetch.length);
                    _.each(results, function (ajaxResult, ind, resultArr) {
                        //Update the itemsAlreadyRendered object we keep, so that we dont render duplicates within a section when load more is clicked
                        _.each(ajaxResult[0].mainContent[0].records, function (record, index, list) {
                            if (!Utility.itemsAlreadyRendered[ajaxResult[0].name][record.attributes.P_Primary_Key]) {
                                //This record has not been shown in this section yet, add it to the array we are going to render and mark it as already rendered in itemsAreadyRendered
                                json.libraryTasks[ajaxResult[0].name].push(record);
                                Utility.itemsAlreadyRendered[ajaxResult[0].name][record.attributes.P_Primary_Key] = true;
                            }
                        });
                    });
                    filterPage.renderBrowse(json, siteMember);
                });
            }
        },
        renderBrowse: function (json, siteMember) {
            //Loop through library tasks, checking if all goals are empty
            var hasContent = false;
            _.each(json.libraryTasks, function (obj, ind, arr) { 
                if (obj.length) { 
                    hasContent = true; 
                }
            });
            json.hasContent = hasContent;
            
            // we need selected goals in browsepageContent.dust to ensure that when we unselect goals in the sort filter
            // all the selected goals appear in the result. The selected goals which have no results appear with a message no results found.
            // we get a list of selected goals. 
            var goalSelected = Utility.getParameterByName('Goals');

            //By default there is no goals parameter in the URL, which means all goals are filter-enabled
            //Set the goals selected to all of the abbreviations. 
            if (!goalSelected) {
                goalSelected = 'MM,RSD,FJ,PS';
            }
            //The goalSelected will look something like "MM,RSD,FJ", split it at the commas to get an array of the goals to be fetched
            var enabledGoalArray = goalSelected.split(',');
            // when the page is loaded goalRankResponses is assigned the property filterEnabled. So when the loop below runs it already has filterEnabled = true
            // we need to delete the property and add the property again
            _.each(json.goalRankResponses, function (prop) {
                if (prop.hasOwnProperty('filterEnabled')) {
                    delete prop.filterEnabled;
                }
            }),
      
            // a filterEnabled value is added to the goalRankResponse which is true when goals are selected.
            // for every enabledGoalArray we compare the seleted goals. The selected goal is in param and goalRankResponses does not have param value 
            // We use paramToGoalInfo to get the name value and compare the nameWithNoSpaces and if it is true add the filterEnabled true value.  
            _.each(enabledGoalArray, function (goalSel) {
                _.each(json.goalRankResponses, function (goalList) {
                    if (goalList.nameWithNoSpaces === Utility.paramToGoalInfo[goalSel].name) {
                        goalList.filterEnabled = true;
                    }
                });
            }),

            Utility.renderDustTemplate('Modules/BrowsePageContent', json, function (err, out) {
                var newFilterHTML = $.parseHTML(out);
                $('#Library').html(newFilterHTML);
                //The parsehtml call above strips out scripts from the rendered dust html
                //Make sure to re-trigger scripts so we gets ratings, video, etc
                pageHelpers.reTriggerInlineScripts(newFilterHTML);
                //determine if ReportingId is needed and adjust SaltCourse Urls.
                var showReportIdQuestion = reportingStatus.checkReportIdStatus(siteMember);
                reportingStatus.initOverlay(showReportIdQuestion);
                SALT.trigger('populateMultiSelect:needed');
                $(document).foundation('reflow');
                $(newFilterHTML).find('.js-tabindex-3').attr('tabindex', '3');
                // add scroll handlers to sticky Browse sections
                filterPage.stickyHeaders();
            });
        },
        // handles the Goal subheaders in browse section that stick to the top
        stickyHeaders : function () {
            var $window = $(window),
                $stickies = $('#Library h2[data-magellan-destination]'),
                $findWhatYouNeed = $('#browse-fixed-header'),
                $tabsContent = $('.tabs-content'),
                $magellanSubNav = $('#Library .magellan-container[data-magellan-expedition] .sub-nav'),
                $magellanEmptySubNav = $('#Library .magellan-container[data-magellan-expedition] .js-browse-no-content');

            var updateWidth = function () {
                // since these are fixed, explicit widths must be given which means
                // that we cannot just inherit from the percentage widths of foundation
                var widthOffsetToallowRightBorderToShow = 12;
                if ($stickies.length && $magellanSubNav.length) {
                    $magellanSubNav.width($tabsContent.outerWidth() - widthOffsetToallowRightBorderToShow);
                } else if ($magellanEmptySubNav.length) {
                    $magellanEmptySubNav.width($tabsContent.outerWidth() - widthOffsetToallowRightBorderToShow);
                }
                widthOffsetToallowRightBorderToShow = 2;
                $findWhatYouNeed.outerWidth($tabsContent.outerWidth() - widthOffsetToallowRightBorderToShow);
            };

            $window.off('resize', updateWidth).on('resize', updateWidth);
            $(document).foundation('reflow');
            updateWidth();

            //We want the dashboard tab buttons and browse header to become sticky only when they are at the top of the screen
            //The threshold variables account for the fact that we dont want these elements to start being sticky when they are scrolled to the very top of the page
            //We want the tab buttons to start being sticky when the top of the tabs reaches the bottom of the nav bar
            //The browse header should become sticky when it reaches the bottom of the tab buttons.
            var saltHeaderHeight = $('.js-simple-header').outerHeight(),
                dashNavHeight = $('.js-dash-nav').outerHeight(),
                $tabs = $('.tabs'),
                tabsStickyThreshold = $tabs.offset().top - saltHeaderHeight,
                $browseTabHeader = $('#browse-fixed-header'),
                browseHeaderStickyThreshold = $browseTabHeader.offset().top - (saltHeaderHeight + dashNavHeight),
                $browseGoalHeader = $('#Library .magellan-container[data-magellan-expedition]');

            $window.scroll(function () {
                var currentScrollPosition = $window.scrollTop();
                $tabs.add($browseTabHeader).add($browseGoalHeader).toggleClass('sticky', currentScrollPosition > tabsStickyThreshold);
            });
        }
    };

    // throttledScroll will be a throttled function to be used as scroll events' handler.
    filterPage.throttledScroll = _.throttle(filterPage.infiniteScrollHandler.bind(filterPage), 100);
    // unbind any throttledScroll event on every SPA page view in case we were listening to any
    SALT.on('need:navigation', function () {
        $(window).unbind('scroll', filterPage.throttledScroll);
    }, filterPage);

    $(document.body).on('click', '.js-tileContainer', function () {
        infiniteScrollObject.scrollPosition = $(document).scrollTop();
    });

    return filterPage;
});
