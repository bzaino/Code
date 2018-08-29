/* jshint maxstatements: 28 */
define([
    'require',
    'jquery',
    'salt',
    'underscore',
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'modules/overlays',
    'jquery.dotdotdot'
], function (require, $, SALT, _, Utility, SiteMember) {

    var saltTourLoaded = false;

    var ContentView = {
        addSchoolToContext: function (context) {
            SiteMember.done(function (siteMember) {
                //TODO REMEMBER ME STATE?????
                if (siteMember.IsAuthenticated === 'true') {
                    //Add sitemember to context since we are logged in
                    context.SiteMember = siteMember;
                    context.configuration = context.configuration || {};
                    context.SiteMember.Organizations = Utility.sortOrganizations(context.SiteMember.Organizations);
                    var logoOrgs = Utility.returnBrandedOrganizations(context.SiteMember.Organizations);
                    if (logoOrgs && logoOrgs.length > 0) {
                        context.configuration.CurrentSchool = logoOrgs[0].OrganizationName;
                        context.configuration.CurrentSchoolBrand = logoOrgs[0].Brand;
                    }
                    else {
                    //Use for each loop to determine which of the objects in the Organization Array is the current organization.  We do this by comparing the OrganizationId property, to PrimaryOrganizationKey
                        _.each(siteMember.Organizations, function (obj, ind, arr) {
                            if (obj.OrganizationId.toString() === siteMember.PrimaryOrganizationKey) {
                                context.configuration.CurrentSchool = obj.OrganizationName;
                                context.configuration.CurrentSchoolBrand = obj.Brand;
                            }
                        });
                    }
                }
            });
        },
        reTriggerInlineScripts: function (parsedHTML) {
            if (!window.HTMLPictureElement) {
                //retrigger picturefill after images render on client side.
                picturefill();
            }
            require(['modules/ReachOut'], function (reachOutModule) {
                reachOutModule.init($(parsedHTML));
            });
            $(parsedHTML).find('.tileText, .tileHeader').dotdotdot({
                wrap: 'word',
                watch: 'window',
                ellipsis: '... ',
                height: null
            });
            $(parsedHTML).find('.js-video-tile, .js-tile-review-container, .overlaySelector, .youTubeSelector').each(function (ind, el) {
                if ($(this).hasClass('js-tile-review-container')) {
                    var $currentElement = $(this),
                        $tileRating = $('.js-tile-rating', $currentElement);
                    if ($tileRating.length) {
                        require(['modules/StarRatingWidget'], function () {
                            $currentElement.ratings(5, Math.round(Number($tileRating.val())), false);
                        });
                    }
                } else if ($(this).hasClass('youTubeSelector')) {
                    require(['modules/ASAYouTubeVideo'], function (uTube) {
                        uTube.initialize();
                    });
                } else if ($(this).hasClass('overlaySelector')) {
                    if (!saltTourLoaded) {
                        saltTourLoaded = true;
                    }
                }
            });
        },
        openSaltTourOverlay: function () {
            //if location hash is #tour, we want to automatically open salt tour video.
            if (Utility.getParameterByNameFromString('tour', location.search)) {
                if (saltTourLoaded === false) {
                    saltTourLoaded = true;
                }
                SALT.openOverlay('#js-reveal-video-overlay');
            }
        }
    };
    return ContentView;
});
