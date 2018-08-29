define([
    'jquery',
    'asa/ASAUtilities',
    'salt',
    'underscore',
    'salt/models/SiteMember'
], function ($, Utility, SALT, _, SiteMember) {

    var previouslyChecked,
        showRecommended;

    function toggleRecommendedVisibilty() {
        if (showRecommended) {
            $('.js-recommended-sort').show();
        } else {
            $('.js-recommended-sort').hide();
        }
    }

    $(function () {

        $(document.body).on('click', '#drop-middle a', function (e) {
            e.preventDefault();
            var navState = '',
            nsVal = $(this).attr('id');
            /*appending ns value to the current url*/
            navState = Utility.updateQueryString(Utility.getlocationPath() + Utility.getLocationSearch(), 'Ns', nsVal);
            /*append the page number No to the url if No doesn't exist*/
            if (navState.indexOf('No=') === -1) {
                navState = Utility.updateQueryString(navState, 'No', '0');
            }
            /*if it's featured page, and user clicks on By Usefullness or Popularity, it should turn featured into All Content.*/
            if (!Utility.getParameterByNameFromString('Type', navState) && (nsVal === 'P_Rating|1' || nsVal === 'P_Hits_Last_30_Days|1')) {
                navState = Utility.updateQueryString(navState, 'Type', 'All');
            }
            SALT.trigger('need:navigation', navState);
        });

        // show/hide the "By Recommended" sorting option based on profile answers
        $(document.body).on('mousedown touchstart', '#js-sorting-menu', function (e) {
            SiteMember.done(function (siteMember) {
                if (siteMember.IsAuthenticated === 'true') {
                    if (!previouslyChecked) {
                        $.getJSON('/api/SearchService/DetermineByRecommendedVisibility/' + siteMember.MembershipId, function (shouldBeVisible) {
                            previouslyChecked = true;
                            showRecommended = shouldBeVisible;
                            toggleRecommendedVisibilty();
                        });
                    } else {
                        toggleRecommendedVisibilty();
                    }
                }
            });
        });

        $(document.body).on('click', '#clearAllBtn', function (e) {
            e.preventDefault();
            var selectBtn = $(this);
            $('.js-content-type-option').attr('checked', false).trigger('change');
            selectBtn.attr('data-selected', 'off');
        });

        $(document.body).on('click', '#select-all-types', function (e) {
            e.preventDefault();
            $('.js-content-type-option').attr('checked', true).trigger('change');
        });

        $(document.body).on('change', '.js-content-type-option', function (e) {
            e.preventDefault();
            if ($('.js-content-type-option:checked').length === 0) {
                $('#typesApplyBtn').attr('disabled', true);
            } else {
                $('#typesApplyBtn').attr('disabled', false);
            }
        });

        $(document.body).on('click', '#typesApplyBtn', function (e) {
            e.preventDefault();
            var $checkedContentTypeInputs;
            var url;

            //Using this selector to differentiate the "new" sort/filter panel from the old
            //There are checkboxes on the new panel that we dont want to include in the content type param
            if ($('.js-sortfilter').length) {
                //New selector for content types and spanish checkoxes that only exists in the new sort/filter html
                $checkedContentTypeInputs = $('.js-dims-param:checked');
                var $checkedUserGoals = $('.js-sortfilter-usergoal:checked');
                var userGoalParamValues = $.map($checkedUserGoals, function (element) {
                    return $(element).attr('data-goal-param-value');
                });
                url = Utility.updateQueryString(Utility.getlocationPath() + Utility.getLocationSearch(), 'Goals', userGoalParamValues.join(','));

                //Check which sort the user has seleted and add it to the Ns param
                var sortValue = $('input:radio[name=sortby]:checked').val();
                url = Utility.updateQueryString(url, 'Ns', sortValue);
            } else {
                $checkedContentTypeInputs = $('#drop-content-types input:checked');
            }
            var selectedDimensions = $.map($checkedContentTypeInputs, function (element) {
                return $(element).val();
            });

            //If we have already started building a url because we are on dashboard we want to update that string, not the location path/location search
            if (url) {
                url = Utility.updateQueryString(url, 'Dims', Utility.getDimensionString(selectedDimensions));
            } else {
                url = Utility.updateQueryString(Utility.getlocationPath() + Utility.getLocationSearch(), 'Dims', Utility.getDimensionString(selectedDimensions));
            }

            /*if it's featured page, and user clicks on apply button to filter by content types, it should turn featured into All Content.*/
            if (!Utility.getParameterByNameFromString('Type', url)) {
                url = Utility.updateQueryString(url, 'Type', 'All');
            }
            //When we are on the dashboard page we need to make sure to append the library/browse tab's hash,
            //This is so that the dropdown plugin knows to make the library/browse tab active, otherwise MySALT will show as it is the first/default tab
            if ($('.js-dashboard-container').length) {
                url += '#fndtn-Library';
            }
            //Often, the user will have changed the options in the sort/filter panel, which will trigger a url change, and a re-rendering of the panel as closed
            //Sometimes they will not have changed any options, which means the url won't change and wont trigger a re-rendering.  For these cases we need to make sure we are closing the panel.
            //Since the panel has to close in all cases anyway, Im not putting a check for if anything changed as the overhead for that would be larger than calling close for the cases where we will be re-rendering anywhere
            Foundation.libs.dropdown.close($('#drop-content-types'));
            //Trigger a url change now that we have built up the url variable
            SALT.trigger('need:navigation', url);
            
            //Fire WT, SWD-9008 Sort&Filter "Apply" button, Event
            SALT.publish('dashboard:action:taken', {
                activity_name: 'Sort By',
                activity_type: 'Apply'
            });
        });

        $(document.body).on('click', '.user-goal', function (e) {
            e.preventDefault();
            var url = Utility.updateQueryString(Utility.getlocationPath() + Utility.getLocationSearch(), {'Type': $(this).attr('goal'), 'Tag': ''});
            SALT.trigger('need:navigation', url);
        });

        $(document.body).on('click', '.SiteWide', function (e) {
            e.preventDefault();
            var siteWideID = $(this).attr('id'),
            url = Utility.getlocationPath() + Utility.getLocationSearch();
            url = Utility.updateQueryString(url, 'Type', siteWideID);
            //Check if the click was the View All option, in which case do not clear the Ns param, b/c sort should still be By Popularity for View All - SWD 7558
            if ($(e.currentTarget).attr('id') === 'All') {
                url = Utility.clearURLParams(url, ['Dims', 'Tag']);
            } else {
                /*get rid of any other selection if site wide clicked*/
                url = Utility.clearURLParams(url, ['Dims', 'Tag', 'Ns']);
            }
            SALT.trigger('need:navigation', url);
        });

        $(document.body).on('click', '.tag-tray-link', function (e) {
            e.preventDefault();
            //update url with tag val if one of the tags clicked.
            var tagVal = $(this).text(),
                url = Utility.updateQueryString(Utility.getlocationPath() + Utility.getLocationSearch(), 'Tag', tagVal);
            SALT.trigger('need:navigation', url);
        });

        function prePopulateCheckboxes(paramName, checkboxSelector) {
            var paramString = Utility.getParameterByNameFromString(paramName, Utility.getLocationSearch());
            var paramArray = paramString ? paramString.split(',') : [];

            if (paramArray.length === 0) {
                $(checkboxSelector).attr('checked', true);
            } else {
                _.each(paramArray, function (currentElement) {
                    $('#content-type-' + currentElement).attr('checked', true);
                });
            }
        }
        SALT.on('populateMultiSelect:needed', function () {
            prePopulateCheckboxes('Dims', '.js-content-type-option');
            //If we are using the new sort/filter panel we need to populate the Sort radio buttons and Goal Checkboxes
            if ($('.js-sortfilter').length) {
                prePopulateCheckboxes('Goals', '.js-sortfilter-usergoal');

                var sortParam = Utility.getParameterByNameFromString('Ns', Utility.getLocationSearch());
                if (!sortParam) {
                    $('#js-sort-Recommended').attr('checked', true);
                } else if (sortParam === 'P_Rating|1') {
                    $('#js-sort-Rating').attr('checked', true);
                } else {
                    $('#js-sort-Popularity').attr('checked', true);
                }
            }
        });

        //508 Compliance, SWD-6602, reorder tabbing structure, open drop down on tab, start.
        //keyup event for sort control bar.
        $(document.body).on('keyup', '.js-tab-marker', function (e) {
            // event.which normalizes event.keyCode and event.charCode, recommended by Jquery.
            // 9 means it's tab key
            if (e.which === 9 && !$(e.currentTarget).siblings('.f-dropdown').hasClass('open')) {
                $(e.currentTarget).click();
            }
        });
        //in order to close the multi selelct content type drop down, bind a key down on the last element of that drop down, and close it manually.
        $(document.body).on('keydown', '#drop-content-types .filter-option:last-child', function (e) {
            if (e.which === 9 && !e.shiftKey && $('#drop-content-types').hasClass('open')) {
                $('#selectedType').click();
            }
        });
        //508 Compliance, SWD-6602, reorder tabbing structure, open drop down on tab, end.
    });
});
