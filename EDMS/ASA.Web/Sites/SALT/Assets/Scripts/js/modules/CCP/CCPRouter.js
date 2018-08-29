define([
    'jquery',
    'salt',
    'underscore',
    'backbone',
    'configuration',
    'asa/ASAUtilities',
    'modules/CCP/CCPModels'
], function ($, SALT, _, Backbone, Configuration, Utility, Models) {
    var CCPRouter = Backbone.Router.extend({
        initialize: function () {
            var self = this;
            SALT.on('navigate', function (route) {
                self.navigate(route, {
                    trigger: true
                });
            });
        },
        routes: { 
            'R-101-23826': 'Start',
            'CostOfAttendance': 'CostOfAttendanceRoute',
            'GrantsAndScholarships': 'GrantsAndScholarshipsRoute',
            'PlannedContributions': 'PlannedContributionsRoute',
            'MonthlyInstallments': 'MonthlyInstallmentsRoute',
            'StudentLoans': 'StudentLoansRoute',
            'YourPlan': 'YourPlanRoute'
        },
        Start: function () {
            $('.js-ccp-form').find('[data-index]').hide();
            $('#js-ccp-chart').hide();
            $('.js-hide').show();
            $('.js-ccp-form').find("[data-index ='" + 1 + "']").show();
            //Hide mobile flyout from first page
            $('#js-CCPMobileFlyOutButton').addClass('hiddenitem');
            if (Backbone.History.started === false) {
                Backbone.history.start({pushState: true});
            }
            Utility.topScroll();
        },    
        CostOfAttendanceRoute: function () {
            this.showHideSections(2);
        },
        GrantsAndScholarshipsRoute: function () {
            this.showHideSections(3);

        },
        MonthlyInstallmentsRoute: function () {
            this.showHideSections(5);
        },
        PlannedContributionsRoute: function () {
            this.showHideSections(4);
        },
        StudentLoansRoute: function () {
            SALT.trigger('change:showLoansInGraph:true');
            this.showHideSections(6);
        },
        YourPlanRoute: function () {
            this.showHideSections(7);
            SALT.trigger('change:showLoansInGraph:true');
            SALT.trigger('change:yourPlanMsg');
            $('.js-CCPMessageContainer').show();
        },
        showHideSections: function (nextIndex) {
            $('.js-hide').hide();
            $('.js-ccp-form').find('[data-index]').hide();
            $('.js-ccp-form').find("[data-index ='" + nextIndex + "']").show();
            $('.js-CCPMessageContainer').hide();
            //force tool-tip to close when moving from page to page.
            $(document).trigger('click');
            SALT.trigger('show:graph');
            Utility.topScroll();
        }

    });
    return CCPRouter;
});
