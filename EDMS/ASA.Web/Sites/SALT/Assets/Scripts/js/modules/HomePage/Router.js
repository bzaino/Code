define([
    'salt',
    'underscore',
    'backbone',
    'asa/ASAUtilities',
    'configuration',
    'jquery.cookie',
    'asa/ASAWebService'
], function (SALT, _, Backbone, Utility, configuration) {

    var HomeRouter = Backbone.Router.extend({
        initialize: function () {
            var _this = this;
            this.initialRouteFired = false;
            this.route(/^content\/search\?([^?]*?)?$/, 'searchPage');
            SALT.on('need:navigation', function (url) {
                _this.navigate(url, {
                    trigger: true
                });
                //if the content type drop down box is open, close it.
                if ($('#drop-content-types').hasClass('open')) {
                    $('#selectedType').click();
                }
            });
            this.on('route', this.spaCleanUp);
        },
        routes: {
            'home?*querystring': 'filterPage',
            'home(?*querystring)': 'filterPage',
            'home/': 'filterPage'
        },
        filterPage: function (querystring) {
            querystring = this.handleSpanishQuerystring(querystring);
            require(['modules/HomePage/FilterPage', 'salt/models/SiteMember'], function (filterPage, SiteMember) {
                SiteMember.done(function (siteMember) {
                    if (siteMember.OnboardingEnabled && $.cookie('NeedsOnboarding')  || querystring && querystring.indexOf('onboarding=true') > -1) {                    
                        filterPage.renderOnboarding(querystring);
                    } else if (siteMember.DashboardEnabled && $.cookie('IndividualId')) {
                        filterPage.renderDashboard(querystring);
                    } else {
                        filterPage.renderFilterPage(querystring);
                    }
                });
            });
        },
        searchPage: function (querystring) {
            querystring = this.handleSpanishQuerystring(querystring);
            //Backbone is going to return the unencoded match, we need to encode it, as functions downstream expect a properly formed url
            /*strip out <> from querystring*/
            querystring = querystring.replace(/</g, '').replace(/>/g, '');
            querystring = encodeURI(querystring);
            require(['modules/HomePage/FilterPage'], function (filterPage) {
                filterPage.renderFilterPage(querystring);
            });
        },
        spaCleanUp: function (route, params) {
            //If we are moving from a search page to another SPA page we should clear the search term text from the search box
            $('#searchCriteria').val('');

            //Empty any sidebar carousels, they are page specific, we dont want them hanging around from page to page
            $('#sidebar-carousels').empty();

            if (this.initialRouteFired) {
                Utility.waitForAsyncScript('QSI', function () {
                    QSI.API.unload();
                    QSI.API.load();
                });
            } else {
                this.initialRouteFired = true;
            }
        },
        handleSpanishQuerystring: function (querystring) {
            if (Utility.getParameterByName('Lang').toLowerCase() === 'spanish') {
                var dimsString = Utility.getParameterByNameFromString('Dims', querystring),
                    dimsArray = dimsString ? dimsString.split(',') : [],
                    selectedDims,
                    selectorArray = _.each(dimsArray, function (element) {
                        return element;
                    });
                // if we have any dimesions in the URL already, then add spanish to them. Otherwise, us all content types.
                if (selectorArray.length === 0) {
                    // TODO Find a way to get all content types' dval numbers from the object in SortControlBar.js
                    selectedDims = '104,41,42,43,44,45,46,157,303';
                } else {
                    selectedDims = '104,' + dimsString;
                }
                querystring = Utility.updateQueryString('?' + querystring, 'Dims', selectedDims);
                // update the URL for the drop down to set the default values without triggering a route
                this.navigate(location.pathname + querystring, {trigger: false, replace: false});
            }
            return querystring;
        }
    });

    return HomeRouter;
});
