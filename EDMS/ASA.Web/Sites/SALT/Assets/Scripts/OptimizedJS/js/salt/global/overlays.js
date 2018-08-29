/*!
 * SALT.global.overlays
 */

define(function (require, exports, module) {
    var $    = require('jquery'),
        SALT = require('salt'),
        WT   = require('salt/analytics/webtrends-config');

    require('salt/global');

    $.extend(SALT.global, {
        overlays: {
            init: function () {
                var WTTags = WT.tags;

                this.types = {
                    'LogOn': {
                        href: 'Account/LogOn',
                        WTTags: [
                            WTTags.PAGE_TITLE, 'Member Activation | Login', // TODO: Automate
                            WTTags.CONTENT_GROUP, 'Member',
                            WTTags.CONTENT_SUBGROUP, 'Login'
                        ]
                    }
                };
            },

            /*************************************************************************************
            UsagePath: SALT.global.overlays.populateWithContent
            Note: element will default to: #emptyDivToAddContentTo unless otherwise defined
            See cshtml files of overlays (logon etc) for example usage
            *************************************************************************************/
            populateWithContent: function (path, element) {

                $.get(path, {}, function (data) {
                    if (!element) {
                        element = '#emptyDivToAddContentTo';
                    }
                    $(element).html($(data).find('#ASASnippet'));
                    $.fn.colorbox.prep($(element).contents());
                });
            },

            /**************************************
            -the function below waits for the colorbox lifecycle to complete for the second time (2nd time initiated by colorbox.prep in contentpopulation function)
            -pass in a function of initialize events for the overlay to run
            See cshtml files of overlays for example usage
            **************************************************/
            executeInitForContentPopulation: function (func) {
                $(document).one('cbox_complete', function () {
                    $(document).one('cbox_complete', function () {

                        func();
                        var flag = true;

                        if (SALT.Utility.currentBrowser.isSafari || SALT.Utility.currentBrowser.isIE6_9) {
                            flag = false;
                        }
                        if (flag) {
                            $('#ASASnippet').find("input[type!='hidden']:first").focus();
                            $(document).one('cbox_cleanup', function () {
                                $('#ASASnippet').find("input[type!='hidden']").blur();

                            });
                        }
                    });
                });

            },

            open: function (overlayTypeConfig) {
                var overlayTypeName, overlayType, overlayHref;

                if (!this.types) {
                    this.init();
                }

                if (typeof overlayTypeConfig === "object") {
                    overlayTypeName = overlayTypeConfig.name;
                    overlayType = this.types[overlayTypeName];

                    if (overlayTypeConfig.anchor) {
                        overlayHref = overlayTypeConfig.anchor.href;
                    } else if (overlayTypeConfig.href) {
                        overlayHref = overlayTypeConfig.href;
                    }
                } else {
                    overlayTypeName = overlayTypeConfig;
                    overlayType = this.types[overlayTypeName];
                    overlayHref = overlayType.href;
                }

                if (typeof overlayType !== 'undefined') {
                    WT.setTag(overlayType.WTTags, 'DCS.dcsuri',                   '/' + overlayType.href);
                    WT.setTag(overlayType.WTTags, WT.tags.SERVER_CALL_IDENTIFIER, WT.SERVER_CALL_IDENTIFIERS.STANDARD_PAGE_VIEW);

                    // if the anchor is specified, we're assuming colorbox creation has already been done; just initiate WT tracking.
                    if (typeof overlayTypeConfig === 'object' && overlayTypeConfig.anchor) {
                        dcsMultiTrack(overlayType.WTTags);
                    } else {
                        $.fn.colorbox({
                            href: overlayHref,
                            opacity: 0.5,
                            onLoad: function () {
                                dcsMultiTrack(overlayType.WTTags);
                            }
                        });
                    }
                }
            }
        }
    });

    return SALT.global.overlays;
});
