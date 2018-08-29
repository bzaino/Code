/* jshint maxstatements: 28 */
define([
    'salt',
    'salt/models/SiteMember',
    'jquery',
    'modules/ContentInteractionModels',
    'asa/ASAUtilities',
    'configuration',
    'asa/ASAWebService',
    'jquery.cookie'
], function (SALT, SiteMember, $, Interactions, utility, configuration) {

    $(document.body).on('click', '.js-manage-rank', function () {
        //If we are on the goal rank page we dont want to set the NeedsOnboarding cookie, as we'll end up back on onboarding instead of the homepage when they finish Q+A and tag selection
        var href = window.location.href;
        var subStrHref = href.substr(href.lastIndexOf(".org") + 4);

        if (!$('#js-rank-container').is(':visible')) {
            document.cookie = 'NeedsOnboarding = true; path=/';

            if ($('.js-SPA-enabled').length) {
                //We are not actually   making use of the onboarding qurystring parameter, just needs to be different than /home, in order to trigger the backbone router if we are already on /home
                SALT.trigger('need:navigation', '/home?onboarding=true');
            } else {
                window.location.href = '/home?onboarding=true';
            }
            Foundation.libs.dropdown.close($('.js-header-dropdown'));

            setProfileModCookie(subStrHref);
        }
    });

    $(document.body).on('click', '.js-back-out', function (e) {
        window.location.href = $.cookie('ProfileModBack');
        $.removeCookie('ProfileModBack', { path: '/' });
    });

    $(document.body).on('click', '.js-tab-link', function (e) {
        e.preventDefault();
        //Simulate a click of the MySALT or Browse tab buttons, depending on the data-destination attribute of the clicked link
        //Foundation tabs are not linkable in a way that wont re-render the entire dashboard content
        //We just want to switch from todolist to MySALT, this avoids making an API call and re-rendering the page for no reason
        $('#js-' + $(this).attr('data-destination')).click();
    });

    $(document.body).on('click keydown', '.js-dash-nav', function (e) {
        if (e.which === 13) {
            $(this).find('a').click();
        }
        if (e.type === 'click') {
            Foundation.libs.dropdown.close($('[data-dropdown-content]'));
            //Dont scroll if the tab clicked is already active
            if (!$(this).hasClass('active')) {
                //Scrolling down on one of the tabs can make the tab and browseHeader "sticky"
                //Remove these classes before calculating the offset of the browse header bar as we want to calcuate the top pre-stickiness
                //see the stickyHeaders function for where we do the calculations (the toggled handler in that function runs after this one)
                $('.tabs').removeClass('sticky');
                $('#browse-fixed-header').removeClass('sticky');
                //Scroll to the top of the tab when we are switching tabs
                $(window).scrollTop(0);
                // this is to reset the Magellan (used in the sticky headers) because switching Foundation tabs
                // makes the Foundation Magellan think the bottom section header is above the threshold to be active
                // SWD-8831
                if ($(this).is('#js-library-tab')) {
                    $('.browse-arrivals').removeClass('sticky-top');
                }
            }
            var tabText = $(e.target).text();
            //Fire WT, SWD-9006 Navigation tabs subhead, Event
            SALT.publish('dashboard:action:taken', {
                activity_name: 'Nav-Sub Head',
                activity_type: tabText
            });
        }

        
    });

    $(document.body).on('keydown', '.js-featured-todo-toggle', function (e) {
        if (e.which === 13) {
            $(this).children(':visible').click();
        }
    });

    $(document.body).on('click', '.js-wt-todoheader', function (e) {
        var $targetTileHeader = $(e.currentTarget),
            subName = '',
            contentTitle = $targetTileHeader.attr('title'),
            contentType = $targetTileHeader.closest('.js-todoContainer').attr('data-content-type'),
            contentId = $targetTileHeader.closest('.js-todoContainer').attr('data-primary-key');

        var tabId = $('.tabs-content > .content.active').attr('id');

        if ($targetTileHeader.hasClass('js-featuredContent')) {
            subName = 'Hero';
        }
        else if (tabId === 'Todos') {
            subName = 'To Do List';
        }
        else if (tabId === 'MySALT') {
            subName = 'Dashboard';
        }

        if (contentType === 'Course') {
            SALT.trigger('content:todo:inProgress', {contentId: contentId});
        }

        //Fire WT, to do tile header or image link clicked, Event
        SALT.publish('dashboard:action:taken', {
            activity_name: 'Tasks:' + contentTitle,
            activity_sub_name: subName,
            activity_type: contentType,
            content_id: contentId
        });
    });


    function setProfileModCookie(subStrHref) {
        $.cookie('ProfileModBack', subStrHref, {path: '/'});
    }

    function sendTaskMenuWebtrends($clickedButton) {
        SALT.publish('dashboard:action:taken', {
            'activity_name': 'Tasks:' + $clickedButton.closest('.js-todoContainer').attr('data-content-title'),
            'activity_sub_name' : 'Task Menu',
            'activity_type' : $clickedButton.text(),
            'content_id' : $clickedButton.closest('.js-todoContainer').attr('data-primary-key')
        });
    }

    function TodoModel(memberID, contentID, statusID, typeID) {
        this.MemberID = memberID;
        this.ContentID = contentID;
        this.CreatedBy = null;
        this.CreatedDate = '/Date(1453830187537-0500)/';
        this.MemberToDoListID = 10;
        this.ModifiedBy = null;
        this.ModifiedDate = null;
        this.RefToDoStatusID = statusID;
        this.RefToDoTypeID = typeID;
    }

    function buildTodoModel($clickedButton, memberID, targetStatus, toDoTypeID) {
        var contentID = $clickedButton.closest('.js-todoContainer').attr('data-primary-key');
        //if not in todo task menu, check if the context within eBook landing page to grab contentID
        if (!contentID && $clickedButton.text() === 'Download') {
            contentID = $clickedButton.closest('section').find('.js-todoContainer').attr('data-primary-key');
        }
        if (!toDoTypeID) {
            //Look for content type text, if there is none, it is an offline task, so we need to make sure refToDoTypeID is set to 3
            if ($clickedButton.closest('.row').find('.js-offline-todo').length) {
                toDoTypeID = 3;
            }
        }
        return new TodoModel(memberID, contentID, targetStatus, toDoTypeID);
    }

    SALT.on('content:todo:completed', function (data) {
        SiteMember.done(function (siteMember) {
            var modelToSend = new TodoModel(parseInt(siteMember.MembershipId, 10), data.contentId, 2, 1);
            SALT.services.upsertTodo(function (successful) {
                if (successful) {
                    //Fire WT, task completion, Event
                    SALT.publish('dashboard:action:taken', {
                        activity_name: 'Tasks:' + data.contentTitle,
                        activity_sub_name: 'Complete',
                        activity_type: data.contentType,
                        content_id: data.contentId
                    });
                }
            }, JSON.stringify(modelToSend));
        });
    });

    SALT.on('content:todo:inProgress', function (data) {
        SiteMember.done(function (siteMember) {
            var modelToSend = new TodoModel(parseInt(siteMember.MembershipId, 10), data.contentId, 4, 1);
            SALT.services.upsertTodo(null, JSON.stringify(modelToSend));
        });
    });


    SiteMember.done(function (siteMember) {
        function fetchLoadMore($currentSectionContainer, $clickedLoadMoreBtn) {
            //Load more sections can have independent lengths for how many records should be fetched when load more is clicked
            //Grab the value for how many records to load from the current section
            var howManyRecordsToLoad = parseInt($currentSectionContainer.attr('data-loadMore-amount'), 10);
            //We may already have records ready to be shown in the DOM rather than needing to fetch them
            //Select for any of these records and show them
            var $hiddenRecords = $currentSectionContainer.find('.js-todoContainer:hidden:lt(' + howManyRecordsToLoad + ')');
            $hiddenRecords.show();
            //If the amount of hidden records was less than the amount we want to show we need to fetch some new ones from the backend.
            if ($hiddenRecords.length < howManyRecordsToLoad) {
                //Get the section's mapKey, to be used in looking up duplicates or determining if the load more click came from the todos tab
                var sectionMapKey = $currentSectionContainer.attr('data-mapKey');
                //We load all of the todos into the page, and hide all but the first three
                //No need to call to endeca to fetch more records like we do for other pages
                if (sectionMapKey === 'Todos') {
                    //We are on todos, and have less records hidden than we want to show
                    //this means its the last "page" of results and we need to hide the load more button
                    $clickedLoadMoreBtn.hide();
                } else {
                    //Figure out how many of the records we are about to fetch should be hidden
                    //We fetch 10 at a time, but might not need to show them all if we already have records ready to be shown
                    //This number is 0 indexed, hence the -1 at the end
                    var numNewRecordsNeeded = howManyRecordsToLoad - $hiddenRecords.length - 1;
                    //Find out what index we are searching for for the current section
                    var currentIndex = parseInt($currentSectionContainer.attr('data-currentIndex'), 10);
                    //Grab the section/goal name, to be used to pick which goal url we should be fetching data for
                    var sectionGoalName = '';
                    if (sectionMapKey === 'MySALT') {
                        //Use the data-current-goal property to build which data-goal- attribute we are on, e.g. data-goal-1
                        sectionGoalName = $currentSectionContainer.attr('data-goal-' + $currentSectionContainer.attr('data-current-goal'));
                    } else {
                        //Browse page, use the sectionMapKey which is the goal name
                        sectionGoalName = sectionMapKey;
                    }
                    //Sometimes the result set we are fetching will have items that are duplicates with the curriculum content
                    //We should fetch slightly more items than we actually want to load to acount for this.
                    var howManyRecordsToLoadWithBuffer = howManyRecordsToLoad + 5;
                    //The backfill should be sorted by Popularity when we are on MySALT OR when we are on browse with no param ("by recommended" radio button chosen)
                    //If we are on browse and have an Ns param like P_Rating|1 we will just pass that along when we append utility.getLocationSearch() to urlToFetch
                    var sortingParam = '&';
                    if (sectionMapKey === 'MySALT' || (sectionMapKey !== 'MySALT' && !utility.getParameterByNameFromString('Ns', utility.getLocationSearch()))) {
                        sortingParam = '&Ns=P_Hits_Last_30_Days|1&';
                    }
                    var urlToFetch;
                    //If we are on the browse tab, we want our url to include the Nr param to filter out tags for the goals ahead of the one we are clicking load more for
                    if (sectionMapKey !== 'MySALT') {
                        //Use the "sectionGoalName" variable to find the correct object in the paramToGoalInfo object
                        goalObj = _.findWhere(utility.paramToGoalInfo, {name: sectionGoalName});
                        urlToFetch = goalObj.url + '&No=' + currentIndex + '&Nrpp=' + howManyRecordsToLoadWithBuffer + sortingParam;
                    } else {
                        //If we are elsewhere (MySALT), build a url consisting of the generic endeca base, the goal name for the current section, the index we want to fetch, set the Number of records to return
                        urlToFetch = configuration.apiEndpointBases.GenericEndeca + configuration.apiEndpointBases[sectionGoalName] + '&No=' + currentIndex + '&Nrpp=' + howManyRecordsToLoadWithBuffer + sortingParam;
                    }
                    urlToFetch = utility.setUrlToHideRecords(urlToFetch, siteMember);

                    $.getJSON(urlToFetch).done(function (data) {
                        var numRecordsReturned = data.mainContent[0].records.length;

                        data.useTodoTileDesign = true;
                        //Add the sitememebr to the context because the todo dust template relies on several member properties to render the todoutilitybar in the proper state
                        data.SiteMember = siteMember;
                        //The dust template we are about to render needs to put the goal name into the "section-identifier" variable
                        //This variable gets used to build a unique string value to be used in the data-dropdown attribute, to build the TodoUtilityBar
                        //We need to alias "name" because there is also a name property in the records object we are looping through when we actually want to write out the goal name
                        data.goalName = data.name;
                        //Set the section map key as a property on the context, to be used in generating a unique string for the data-dropdown attribute
                        data.sectionMapKey = sectionMapKey;
                        //Use the "itemsAlreadyRendered" map to remove any duplicates from result set we are about to render
                        //Remove added:1, completes:2 and inProgress:4 if the load more button was clicked on the MySALT section
                        data.mainContent[0].records = _.filter(data.mainContent[0].records, function (record, ind, arr) {
                            if (utility.itemsAlreadyRendered[sectionMapKey][record.attributes.P_Primary_Key[0]] || (sectionMapKey === 'MySALT' && (record.attributes.RefToDoStatusID === 1 || record.attributes.RefToDoStatusID === 2 || record.attributes.RefToDoStatusID === 4))) {
                                return false;
                            }
                            //Add this key to the map so that we won't include it again
                            utility.itemsAlreadyRendered[sectionMapKey][record.attributes.P_Primary_Key[0]] = true;
                            return true;
                        });
                        $currentSectionContainer.attr('data-currentIndex', currentIndex + numRecordsReturned);
                        utility.renderDustTemplate('Modules/MainContentResultsListRenderer', data, function (err, out) {
                            var $templateOutput = $(out);
                            //Hide any records beyond the amount we want to show on a load more click, e.g. if we want to show 10 and have 12, hide the last two
                            $templateOutput.filter('.js-todoContainer:gt(' + numNewRecordsNeeded + ')').hide();
                            //Add the records to the DOM before the load more button
                            $clickedLoadMoreBtn.before($templateOutput);
                            // TODO make this line work so keyboard navigation continues to the correct tile
                            $templateOutput.first().find('.js-todo-header').focus();
                            //If the amount of records returned by the SAL/Endeca is less than the amount we were trying to fetch, we are on the last "page" of records, and should hide the load more button
                            if (numRecordsReturned < howManyRecordsToLoadWithBuffer) {
                                //If we are on MySALT, we need to change to the next goal in the ranking if we are not on the last goal
                                if (sectionMapKey === 'MySALT' && $currentSectionContainer.attr('data-current-goal') !== (siteMember.goalRankResponses.length - 1).toString()) {
                                    //Find out which goal index we are on and increment it by 1
                                    var currentGoalIndex = parseInt($currentSectionContainer.attr('data-current-goal'), 10);
                                    $currentSectionContainer.attr('data-current-goal', ++currentGoalIndex);
                                    //Set the record index back to 0 so that we fetch the first records from the new goal
                                    $currentSectionContainer.attr('data-currentIndex', 0);
                                    fetchLoadMore($currentSectionContainer, $clickedLoadMoreBtn);
                                } else if (numRecordsReturned <= numNewRecordsNeeded) {
                                    //The above check is necessary because sometimes we will have more records returned than we needed, e.g. the last page of records was 11, but we only needed 4 to fill out the current load more of 10 records
                                    //In this case we want  the button to still be shown, so that the user can click and see the final 7 records
                                    $clickedLoadMoreBtn.hide();
                                }
                            }
                        }, null);
                    });
                }
            }
        }

        $(document.body).on('click keydown', '.js-load-more', function (e) {
            if (e.type === 'click' || e.which === 13) {
                //Fire WT, browse sortFilterApply clicked, Event
                var sectionTitle = $(e.target).closest('.js-load-more-container').attr('data-mapkey');
                if (sectionTitle === 'Todos') {
                    sectionTitle = $(e.target).closest('.js-load-more-container').attr('data-subSection');
                }
                SALT.publish('dashboard:action:taken', {
                    activity_name: 'More',
                    activity_type: sectionTitle
                });
                //Store references to the load more button container that was clicked
                var $clickedLoadMoreBtn = $(this).closest('.js-load-more-btn-container');
                //Store the current goal section container for that load more button
                var $currentSectionContainer = $(this).closest('.js-load-more-container');
                fetchLoadMore($currentSectionContainer, $clickedLoadMoreBtn);
            }
        });

        $(document.body).on('click', '.js-add-todo', function (e) {
            //Some of the buttons arent really links, so we need to prevent default on the click behavior so that we dont get auto-scrolled to the top of the page
            e.preventDefault();
            var $clickedButton = $(this);
            sendTaskMenuWebtrends($clickedButton);

            //Create a model to send to the server, should have status = 1 (Added), TODO change this to using text rather than the ID???
            var modelToSend = buildTodoModel($clickedButton, parseInt(siteMember.MembershipId, 10), 1, 2);
            //POST the model to the SAL, update the button to have remove functionality and styling
            SALT.services.upsertTodo(function (data) {
                //Hide the "Add" button and show the "Adding" loading spinner
                $clickedButton.hide().siblings('.js-adding-spinner').show();
                setTimeout(function () {
                    //Hide the loading spinner now that its been 2 seconds
                    $clickedButton.siblings('.js-adding-spinner').hide();
                    //Find any other tiles for the same record on this page
                    $.each($('[data-primary-key="' + $clickedButton.closest('.js-todoContainer').attr('data-primary-key') + '"]'), function () {
                        //hide the add button and show the remove button
                        $(this).find('.js-add-todo').hide().next().show();
                        //Set the styling to "Added" for any matches
                        $(this).find('.js-status-indicator').text('Added');
                        $(this).find('.js-todo-utility-bar').addClass('state-1').removeClass('state-');
                    });
                    if (location.pathname === '/home') {
                        //Since we dont have the json data for this record available, we need to clone the tile and add it to the todos section
                        //This comes with some drawbacks, namely that we get repeated ids and as a result, duplicate bindings on the cloned tile
                        //We need to update the three important attrs to a new unique value:
                        //      -The 'data-dropdown' attribute on the button to open the menu
                        //      -The 'dropdown' property set on the data object of the button to open the menu
                        //      -The id of the ul element which holds the menu itself
                        var $clickedTile = $clickedButton.closest('.js-todoContainer');
                        var $newlyAddedTodo;
                        //We cant simply clone the tile if we are adding the "featured" task from the MySALT section
                        //This tile has a different styling entirely from normal tiles
                        //We have a hidden normally styled tile right next to the featured task in the dom, grab that for cloning instead.
                        //Also make sure to show this tile after prepending it
                        if ($clickedTile.attr('id') === 'js-featuredTask') {
                            $newlyAddedTodo = $clickedTile.next().clone().prependTo('#js-open-container').show();
                        } else {
                            $newlyAddedTodo = $clickedTile.clone().prependTo('#js-open-container');
                        }
                        var uniqueID = 'OpenTodos-' + $newlyAddedTodo.attr('data-primary-key');
                        $newlyAddedTodo.find('.js-todo-bar-items-wrapper').attr('id', uniqueID);

                        //In case its the first todo being added to the list we need to hide the "empty list" message
                        $('#js-emptylist-message').hide();

                        //Increment the open todo counter by one
                        var $todoCountElement = $('#js-todo-count'),
                            openTodoCount = parseInt($todoCountElement.text(), 10);
                        $todoCountElement.text(++openTodoCount);
                        $todoCountElement.addClass('active-counter');
                    }
                }, 2000);
            }, JSON.stringify(modelToSend));
        });

        $(document.body).on('click', '.js-remove-todo', function (e) {
            //The remove todo buttons are <a> but arent really links, so we need to prevent default on the click behavior so that we dont get auto-scrolled to the top of the page
            e.preventDefault();
            var $clickedButton = $(this);
            sendTaskMenuWebtrends($clickedButton);

            //Create a model to send to the server, should have status = 3 (cancelled)
            var modelToSend = buildTodoModel($clickedButton, parseInt(siteMember.MembershipId, 10), 3);
            //POST the model to the SAL
            SALT.services.upsertTodo(function (data) {
                //Hide the remove button and show the loading spinner for 2 seconds
                $clickedButton.hide().siblings('.js-removing-spinner').show();
                setTimeout(function () {
                    //Hide the loading spinner, close the tile manager
                    $clickedButton.siblings('.js-removing-spinner').hide();

                    //Decrement the open todo counter by one
                    var openTodoCount = parseInt($('#js-todo-count').text(), 10);
                    $('#js-todo-count').text(--openTodoCount);

                    //There might be multiple tiles for the same record on the dashboard page (can be in each of the 3 tabs)
                    //Loop through the tiles present for the selected record, setting the styling back to un-added if the tile is on MySALT or Browse
                    //Deleting the tile if its on the todo list open section
                    $.each($('[data-primary-key="' + $clickedButton.closest('.js-todoContainer').attr('data-primary-key') + '"]'), function () {
                        $(this).find('.js-remove-todo').hide().prev().show();
                        $(this).find('.js-status-indicator').text('');
                        $(this).find('.js-todo-utility-bar').removeClass('state-1 state-4');

                        //Only delete the item if its actually in the todo list, we dont want to delete tiles from MySALT or Library when "remove" is clicked
                        if (location.pathname === '/home' && $.contains($('#js-open-todos')[0], $(this)[0])) {
                            $(this).closest('.js-load-more-container').find('.js-todoContainer:hidden').first().show();
                            $(this).closest('.js-todoContainer').remove();
                        }
                        //If there are no todos left in the open todo list, show the empty list message
                        if (!$('#js-open-todos .js-todoContainer').length) {
                            $('#js-todo-count').removeClass('active-counter');
                            $('#js-emptylist-message').show();
                            //Hide the load more button in case it was present now that there are 0 todos
                            $('#js-open-todos .js-load-more-btn-container').hide();
                        }
                    });
                }, 2000);

            }, JSON.stringify(modelToSend));
        });

        $(document.body).on('click', '.js-eBook-body a:contains("Download")', function () {
            SALT.trigger('content:todo:completed', {
                contentId: $('.js-todoContainer').attr('data-primary-key'),
                contentType: $('.js-todoContainer').attr('data-content-type'),
                contentTitle: $('.js-todoContainer').attr('data-content-title')
            });
        });
        // on the dashboard making the text Add to-do / Manage to-do clickable
        //adding event on clicking the text
        //stopPropagination to prevent the default close of dropdown
        $(document.body).on('click', '.js-status-indicator', function (e) {
            e.stopPropagation();
            $(e.currentTarget).siblings('.js-todo-menu-open-btn').click();
        });
    });
});
