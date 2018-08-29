/*!
 * SALT / Analytics Abstraction Layer / Webtrends API Wrapper and Analytics Framework Event Logger
 */
define([
    'jquery',
    'underscore',
    'salt/analytics/webtrends-config',
    'salt',
    'salt/models/SiteMember',
    'salt/analytics',
    'asa/ASAUtilities',
    'json!salt/analytics/webtrends/maps/main.json'
], function ($, _, WTConfig, SALT, SiteMember, Analytics, Utility, MainMap) {

    // BEGIN: Constants
    var LOGGER_ID          = 'webtrends',
        LOGGER_NAME        = 'Webtrends Event Tracker',
        LOGGER_DESCRIPTION = 'Logs select application events to the Webtrends Data Collection Server',
        Webtrends = window.Webtrends;
    // END: Constants
    if (Webtrends) {
        var dcs = Webtrends.dcss['dcsobj_' + 0],
        _wtReady = $.Deferred();

        var WT = Analytics.registerLogger({
                id:          LOGGER_ID,
                name:        LOGGER_NAME,
                description: LOGGER_DESCRIPTION,

                map: [
                    MainMap
                ],

                // Values accessible via JavaScript
                tags:                  WTConfig.tags,
                serverCallIdentifiers: WTConfig.SERVER_CALL_IDENTIFIERS,
                userStateIdentifiers:  WTConfig.USER_STATE_IDENTIFIERS,

                // Values accessible via Dust template
                context: {
                    tags:                  WTConfig.tags,
                    serverCallIdentifiers: WTConfig.SERVER_CALL_IDENTIFIERS,
                    userStateIdentifiers:  WTConfig.USER_STATE_IDENTIFIERS
                },

                getTagValue: function (tags, tagName) {
                    var tagValue;
                    var tagIndex = WTConfig.getTagIndex(tags, tagName);

                    if (tagIndex !== -1) {
                        tagValue = tags[tagIndex + 1];
                    }

                    return tagValue;
                },

                meta: function (name, value) {
                    var $meta = $('meta[name=' + name.replace(/\./g, '\\.') + ']');

                    if ($meta.length === 0) {
                        $meta = $('<meta name="' + name + '" />').appendTo('head');
                    }

                    $meta.attr('content', value);

                    return $meta;
                },

                // updates meta tags, and calls track(). Accepts an array of elements.
                updateMeta: function (parsedHTML) {
                    var tags = [],
                        name = '';

                    parsedHTML.forEach(function (element, index, array) {
                        name = element.getAttribute('name');
                        WTConfig.setTag(tags, name, element.getAttribute('content'));
                        $('meta[name=' + name.replace(/\./g, '\\.') + ']').remove();
                        $('head').append(element);
                    });

                    this.track(tags);
                },

                // END: Helper methods

                // accepts an array of tag names/values or an exploded list of tag arguments
                track: function () {
                    var self = this,
                        args = arguments,
                        tags;

                    if (args.length === 1 && typeof args[0] !== 'string') {
                        tags = args[0];
                    } else {
                        tags = [];
                        for (var i = 0; i < args.length; i++) {
                            tags.push(args[i]);
                        }
                    }

                    _wtReady.done(_.bind(function () {
                        var title = tags.indexOf(self.tags.PAGE_TITLE) === -1 ? document.title : tags[tags.indexOf(self.tags.PAGE_TITLE) + 1],
                            uriStem = tags.indexOf(self.tags.DCS_URI_STEM) === -1 ? document.location.pathname : tags[tags.indexOf(self.tags.PAGE_TITLE) + 1];
                        WTConfig.setTag(tags, self.tags.SITE, 'salt');
                        WTConfig.setTag(tags, self.tags.DCS_URI_STEM, uriStem);
                        WTConfig.setTag(tags, self.tags.EVENT_SOURCE, document.location.hostname + document.location.pathname);
                        WTConfig.setTag(tags, self.tags.DCS_URI_QUERY, Utility.getLocationSearch());
                        WTConfig.setTag(tags, self.tags.DCS_REFERRER, document.referrer);
                        WTConfig.setTag(tags, self.tags.PAGE_TITLE, title);

                        // Make sure normally empty tags are cleared if they're not explicitly defined.
                        for (var i = 0; i < WTConfig.NORMALLY_EMPTY_TAGS.length; i++) {
                            var normallyEmptyTag = WTConfig.NORMALLY_EMPTY_TAGS[i];

                            if (WTConfig.getTagIndex(tags, normallyEmptyTag) === -1) {
                                WTConfig.setTag(tags, normallyEmptyTag, '');
                            }
                        }

                        window.dcsMultiTrack.apply(window, tags);

                        // cleanup Webtrends DCS tags any time an activity has completed
                        if (self.getTagValue(tags, self.tags.ACTIVITY_TYPE) === 'complete') {
                            dcs.dcsCleanUp();
                        }
                    }, this)).fail(function () {
                        console.log(self.name + ': Webtrends prerequisites failed. Not calling dcsMultiTrack(' + tags.join(', ') + ').');
                    });
                },

                initialize: function () {
                    var self = this;

                    var _dcsCreateImage = dcs.dcsCreateImage;
                    dcs.dcsCreateImage = function (dcsSrc) {
                        _dcsCreateImage.call(dcs, dcsSrc);
                    };

                    // Set global info about user (logged in or otherwise)
                    SiteMember.done(function (siteMember) {
                        // Only proceed with population and template use if user is authenticated.
                        if (siteMember.IsAuthenticated === 'true') {
                            //Populate Webtrends field from 'siteMember' object
                            if (siteMember.Source !== null) {
                                if (siteMember.Source === 'Self-registered no match') {
                                    self.meta(self.tags.STATE, self.userStateIdentifiers.LOGGED_IN_UNMATCHED);
                                } else {
                                    self.meta(self.tags.STATE, self.userStateIdentifiers.LOGGED_IN_MATCHED);
                                }
                            }

                            if (siteMember.MembershipId !== null) {
                                $.each(WTConfig.ID_TAGS, function (i, tagName) {
                                    self.meta(tagName, siteMember.MembershipId);
                                });
                            }
                        } else {
                            self.meta(self.tags.STATE, self.userStateIdentifiers.LOGGED_OUT);
                        }

                        //Resolve promise for WT, marking as ready to start firing events, now that we have member data
                        _wtReady.resolve();
                    });
                }
            });

        // Legacy support
        if (!!SALT.global) {
            SALT.global.WT = WT;
        }

        return WT;
    }
});
