require([
    'jquery',
    'salt',
    'foundation5'
], function ($, SALT) {
    $(document).ready(function () {

        var $featured = $('#featured');

        $featured.on('after-slide-change.fndtn.orbit', function (event, orbit) {
            SALT.publish('carousel:rotate', {
                carousel: {
                    name: $featured.data('name')
                }
            });
        });

        /*TODO Reimplement event for click of button
        SALT.publish('carousel:button:click', {
                    carousel: {
                        name: $featured.data('name'),
                        activeSlide: getActiveSlide()
                    }
                });
        $featuredWrapper.one('click', '.orbit-caption .carousel-btn', carouselButtonClick)
                        .one('tap',   '.orbit-caption .carousel-btn', carouselButtonClick);
        */
    });
});
