define([
    'backbone',
    'modules/HomePage/Router',
    'salt',
    'asa/ASAUtilities',
    'salt/analytics/webtrends'
], function (Backbone, Router, SALT, Utility, wtLogger) {
    var HomeRouter = new Router(),
        noRefresh;
    //Enable pushstate
    Backbone.history.start({ pushState: true, hashChange: false, silent: false });

    $('body').on('click', '.js-SPA', function (e) {
        e.preventDefault();
        noRefresh = true;
        SALT.trigger('need:navigation', $(e.currentTarget).attr('href'));
    });

    //Add class to signify that this is an SPA page.  Some client-side features need to distinguish between the SPA/Non-SPA
    $('#rendered-content').addClass('js-SPA-enabled');

    // listen for single page app page views
    SALT.on('SPA:PageViewed', function (context) {
        if (noRefresh) {
            context.content = context.content ? context.content : context;
            Utility.renderDustTemplate('WebtrendsTags', context, function (err, out) {
                wtLogger.updateMeta($.parseHTML(out));
            });
        }
    });
});
