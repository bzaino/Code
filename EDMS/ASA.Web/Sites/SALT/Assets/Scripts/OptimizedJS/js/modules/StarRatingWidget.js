define([
    'jquery'
], function($) {

    function highlightStars (starsCollection, rating, stars) {
        //Highlight selected stars.
        for (var index = 0; index < rating; index++) {
            starsCollection[index].addClass('ratings-full');
        }
        //Unhighlight unselected stars.
        for (var index = rating; index < stars; index++) {
            starsCollection[index].removeClass('ratings-full');
        }
    }

    $.fn.ratings = function(stars, initialRating, editable) {

        //Save  the jQuery object for later use.
        var $elements = this;

        //Go through each object in the selector and create a ratings control.
        return this.each(function() {

            //Make sure intialRating is set.
            if (!initialRating) {
                initialRating = 0;
            }

            //Save the current element for later use.
            var containerElement = this;

            //grab the jQuery object for the current container div
            var $container = $(this);

            //Make sure container is empty before adding anything into it
            $container.empty();

            //Create an array of stars so they can be referenced again.
            var starsCollection = [];

            //Save the initial rating.
            containerElement.rating = initialRating;

            //Set the container div's overflow to auto.  This ensure it will grow to
            //hold all of its children.
            $container.css('overflow', 'auto');

            //create each star
            for (var starIdx = 0; starIdx < stars; starIdx++) {

                //Create an <li> to hold the star.
                var starElement = document.createElement('li');

                //Get a jQuery object for this star.
                var $star = $(starElement);

                //Store the rating that represents this star.
                starElement.rating = starIdx + 1;

                //Add the style.
                $star.addClass('ratings-star');

                //Add the full css class if the star is beneath the initial rating.
                if (starIdx < initialRating) {
                    $star.addClass('ratings-full');
                }

                //add the star to the container
                $container.append($star);
                starsCollection.push($star);

                if (editable) {
                    // when a star has been pressed (by touch or click), fire the 'ratingchanged' event handler, pass the rating through as the data argument.
                    $star.on('mousedown touchstart', function () {
                        $elements.trigger('ratingchanged', {
                            rating: this.rating
                        });
                        containerElement.rating = this.rating;
                    });
                    // when the mouse leaves the stars, or touch screen release, highlight the appropriate stars
                    $star.on('mouseleave touchend', function () {
                        highlightStars(starsCollection, containerElement.rating, stars);
                    });

                    $star.mouseenter(function() {
                        highlightStars(starsCollection, this.rating, stars);
                    });
                }
            }
        });
    };
});
