define([
    'jquery',
    'salt',
    'underscore',
    'backbone',
    'modules/VLC/VLCViews',
    'configuration'
], function ($, SALT, _, Backbone, views, Configuration) {

    /*This function handles the quirk that all of the VLC slides are 8 columns of main body
    And 4 columns of sidebar, except the 3 Loan Import, Find Pin, and Loan Upload slides
    Adds handles to inspect any changes in slides an adjust the html accordingly
    */
    //TODO this pattern needs improvement or more documentation
    //TODO try using view.remove instead of clearing out the loanimporthtml???
    function loanImportViewHandler() {
        this.once('route', function (route, router, params) {
            this.once('route', function (route, router, params) {
                if (route !== 'LoanImportRoute' && route !== 'LoanUploadRoute' && route !== 'FindPinRoute') {
                    $('#loanImportContainer').html('');
                    $('#threeColumn').show();
                }
            });
        });
    }

    var VLCRouter = Backbone.Router.extend({
        /* define the route and function maps for this router */
        //Custom routes (i.e. questions that are not MC, will have specific handlers)
        initialize: function () {
            var self = this;
            SALT.on('navigate', function (route) {
                self.navigate(route, {
                    trigger: true
                });
            });
        },
        routes: {
            '': 'changeSlide',
            'LastAttended': 'LastAttendedRoute',
            'DoYouKnowHowMuch': 'DoYouKnowHowMuchRoute',
            'WhenWillYouGrad': 'WhenWillYouGradRoute',
            'LastPaid270': 'LastPaid270Route',
            'ManageablePaymentYN': 'ManageablePaymentYNRoute',
            'InRepayment': 'InRepaymentRoute',
            'InSchoolYN': 'InSchoolYNRoute',
            'LoanImport': 'LoanImportRoute',
            'LoanUpload': 'LoanUploadRoute',
            'FindPin': 'FindPinRoute',
            ':query': 'changeSlide'
        },

        LastAttendedRoute: function () {
            var view = new views.LastAttendedView({ el: '#VLCBody' });
        },

        InRepaymentRoute: function () {
            var view = new views.InRepaymentView({ el: '#VLCBody' });
        },

        InSchoolYNRoute: function () {
            var view = new views.InSchoolYNView({ el: '#VLCBody' });
        },

        DoYouKnowHowMuchRoute: function () {
            var view = new views.DoYouKnowHowMuchView({ el: '#VLCBody' });
        },

        WhenWillYouGradRoute: function () {
            var view = new views.WhenWillYouGradView({ el: '#VLCBody' });
        },

        LastPaid270Route: function () {
            var view = new views.LastPaid270View({ el: '#VLCBody' });
        },

        ManageablePaymentYNRoute: function () {
            var view = new views.ManageablePaymentYNView({ el: '#VLCBody' });
        },

        LoanImportRoute: function () {
            var view = new views.LoanImportView({ el: '#loanImportContainer' });
            loanImportViewHandler.call(this);
        },

        LoanUploadRoute: function () {
            var view = new views.LoanUploadView({ el: '#loanImportContainer' });
            loanImportViewHandler.call(this);
        },

        FindPinRoute: function () {
            var view = new views.FindPin({ el: '#loanImportContainer' });
            loanImportViewHandler.call(this);
        },

        changeSlide: function (query) {
            //If there is no query, we are at the default route, which means we should be querying the 'Menu' page
            if (!query) {
                query = 'Menu';
                SALT.trigger('trigger:dcsMultiTrack');
            }

            $.getJSON(Configuration.apiEndpointBases.GenericEndeca + 'VLC/' + query)
                .done(function (json) {
                    SALT.trigger('slide:change', json);
                    SALT.trigger('slide:multiplechoice', json);
                })
                .fail(function (jqxhr, textStatus, error) {
                    console.log('JSON Request Failed--- TextStatus:' + textStatus + ' -- Error: ' + error);
                });
        }
    });

    return VLCRouter;
});
