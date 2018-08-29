var EMPTY_GUID = "00000000-0000-0000-0000-000000000000";
require([
// Application.
  "lesson3/app",

// Main Router.
  "lesson3/router",

  "salt",
  "salt/models/SiteMember",
  "salt/analytics/webtrends",

  "salt/global!",
  "salt/Lessons/LessonsLoginManager"
],

function (app, Router, SALT, SiteMember, WT) {

    var $html = $("html");
    var uA = navigator.userAgent;
    if($.browser.msie === true || uA.indexOf('Trident') != -1 ){
         $html.addClass("ie");
    }
    if ($.browser.webkit === true) {
        $html.addClass("webkit");
    }

    var loadLesson = function () {

        //push meta tags at this point to ensure meta tags set for webtrends
        var metaTags = $('meta'),
            meta = [];

        // page:view event
        // Add attributes' names and values of every meta tag to page:view context
        $.each(metaTags, function (i, metaTag) {
            var attrs = {};

            $.each(metaTag.attributes, function (i, attr) {
                attrs[attr.name] = attr.value;
            });

            //forcing webTrends tags from META tags
            if (attrs.name === 'WT.z_state')
                Backbone.Asa.User.state = attrs.content;
            if (attrs.name === 'WT.dcsvid')
                Backbone.Asa.User.siteMemberId = attrs.content;

            meta.push(attrs);
        });

        // Define your master router on the application namespace and trigger all
        // navigation from this instance.
        app.router = new Router();

        // Trigger the initial route and enable HTML5 History API support, set the
        // root folder to '/' by default.  Change in app.js.
        Backbone.history.start({ pushState: true, root: app.root });

        // All navigation that is relative should be passed through the navigate
        // method, to be processed by the router. If the link has a `data-bypass`
        // attribute, bypass the delegation completely.
        $(document).on('click', 'a[href]:not([data-bypass])', function (evt) {
            if (!$(this).parents(".addthis_toolbox")) {
                // Get the absolute anchor href.
                var href = { prop: $(this).prop('href'), attr: $(this).attr('href') };
                // Get the absolute root.
                var root = location.protocol + '//' + location.host + app.root;

                // Ensure the root is part of the anchor href, meaning it's relative.
                if (href.prop.slice(0, root.length) === root) {
                    // Stop the default event to ensure the link will not cause a page
                    // refresh.
                    evt.preventDefault();

                    // `Backbone.history.navigate` is sufficient for all Routers and will
                    // trigger the correct events. The Router's internal `navigate` method
                    // calls this anyways.  The fragment is sliced from the root.
                    Backbone.history.navigate(href.attr, true);
                }
            }
        });
        SALT.trigger('content:todo:inProgress');
        $(document.body).click(function () {
                SALT.trigger('sessionTimeOut:reset');
        });
    };

    //setup here so they are avaiable right away
    SALT.on('content:todo:completed', function() {
        SiteMember.done(function (siteMember) {
            var todoObject = { MemberID: siteMember.MembershipId, ContentID: '101-4479', RefToDoStatusID: 2, RefToDoTypeID: 1 };
            SALT.services.upsertTodo(function () {}, JSON.stringify(todoObject));
        });
    });
    SALT.on('content:todo:inProgress', function() {
        SiteMember.done(function (siteMember) {
            var todoObject = { MemberID: siteMember.MembershipId, ContentID: '101-4479', RefToDoStatusID: 4, RefToDoTypeID: 1 };
            SALT.services.upsertTodo(function () {}, JSON.stringify(todoObject));
        });
    });

    var userId = Backbone.Asa.readCookie('UserGuid');
    var individualId = Backbone.Asa.readCookie('IndividualId');

    if (SiteMember) {
        SiteMember.done(function (siteMember) {
            if (siteMember.IsAuthenticated === 'true') {
                if (!individualId) {
                    //If siteMember.IndividualId is populated, the user is recognized as an ASA User
                    var href = location.protocol + '//' + location.host + '/index.html?ReturnUrl=/lesson3/';
                    window.location = href;
                    return false;
                } else {
                    Backbone.Asa.User.individualId = individualId;
                }
            }
            loadLesson();
        }).fail(function () {
                loadLesson();
            });
    } else if (userId && individualId) {
            Backbone.Asa.User.userGuid = userId;
            Backbone.Asa.User.individualId = individualId;
            loadLesson();
    } else if (userId) {
            // We already know the UserId
            Backbone.Asa.User.userGuid = userId;
            loadLesson();
    } else {
        // Try to find the UserId
        // if (individualId) {
        //Backbone.Asa.User.individualId = individualId;
        loadLesson();
        // }
    }
});
