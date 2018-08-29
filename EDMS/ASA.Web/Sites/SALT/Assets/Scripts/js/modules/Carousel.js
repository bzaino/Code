define([
    'jquery',
    'salt',
    'asa/ASAUtilities',
    'configuration',
    'underscore',
    'owl.carousel'
], function ($, SALT, utility, configuration, _) {

    function initialize($el) {
        $el.owlCarousel({
            pagination: true,
            slideSpeed: 300,
            singleItem: true,
            navigation: true,
            tabbable: true,
            removeEmptyContent: true
        }).on('carousel:paginated', function (e, paginationDirection) {
            SALT.publish('Standard:Action:Generic', {
                activity_name: 'Carousel ' + paginationDirection + ' Button',
                activity_transaction: '1'
            });
        }).on('mousedown.owl.carousel', function (event) {
            //Listen to owl events:
            //setup to trigger mousedown event for Webtrends heat maps
            //this was a bug with the way the owl carousel handles the event.
            //It stopped it from being sent along so Webtrends heat map event did not fire.
            utility.waitForAsyncScript('WebtrendsHeatMap', function () {
                WebtrendsHeatMap.hm_clickEvent(Webtrends.dcss.dcsobj_0, event);
            });
        });
    }
    var render = function (carouselRecord) {
        var $carouselHolder = $('.js-carousel-holder-' + carouselRecord);
        $.getJSON(configuration.apiEndpointBases.GenericEndeca + 'media/_/R-' + carouselRecord, function (data) {
            utility.renderDustTemplate('Modules/CarouselSlides', data, function (err, out) {
                // don't re-render, and re-initialize if we are loading a carousel with the same class twice (ex. english/spanish content)
                if ($carouselHolder.is(':empty')) {
                    $carouselHolder.html(out);
                    // if the carousel has no body, and no author, remove the element that contains them.
                    _.each($carouselHolder.find('.js-carousel-nullable'), function (element) {
                        var $nullableElement = $(element);
                        if (!$nullableElement.has('img').length && !$nullableElement.text().trim()) {
                            $nullableElement.remove();
                        }
                    });
                    initialize($('.js-carousel-' + carouselRecord));
                }
                // 508 purposes, owl makes these tabindex 2 by default
                $('.owl-carousel .navigation-btn, .owl-carousel a').prop('tabindex', 'null');
            });
        });
    };

    $(function () {
        $(document.body).on('click', '.carousel-holder a', function (e) {
            SALT.publish('Standard:Action:Generic', { 'activity_name': 'Carousel Clicked Link', 'activity_transaction': '1'});
            //Account for direct links, setTimeout here because if we navigate to other pages immediately, the webtrends call above won't be able to finish.
            //Timeout 600 to fix the ipad and iphone bug. If it's lower than 600, the webtrends above won't be able to finish before naviagte to other pages.
            if (!$(this).attr('target') || $(this).attr('target').indexOf('blank') === -1) {
                var link = $(this).attr('href');
                e.preventDefault();
                setTimeout(function () {
                    location.href = link;
                }, 600);
            }
        });
    });

    return render;
});
