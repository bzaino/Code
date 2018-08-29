var helpers = helpers || {};

helpers.global = {
    utils: {
        init: function () {
            // activate global plugins
            //helpers.global.utils.plugins.init();
            helpers.global.utils.equalHeightColumns();
            helpers.global.utils.binds();

        },
        /**
         * all Binds
         *
         * @Function
         * @public
         */
        binds: function () {
            var e = setTimeout(function () {
                $(window).resize(helpers.global.utils.equalHeightColumns);
            }, 500);

        },
        plugins: {
            // instantiate 
            init: function () {}
        },
        // END: plugins
        /**
         * Makes sure both columns are equal height
         *
         * @Function
         * @public
         */
        equalHeightColumns: function () {
            if ($('.equal').length > 0 && $('.tool-wrap').width() > 768) {
                //clear height so you can reapply on resize
                $('div.equal').css({
                    'minHeight': 'inherit'
                });
                var tallest = 0,
                    equal = $('div.equal');
                equal.each(function () {
                    if ($(this).outerHeight() > tallest) {
                        tallest = $(this).outerHeight(true);
                    }
                });
                equal.css({
                    'min-height': tallest
                });

            }
        },
        /**
         * Replaces any special characters in string
         *
         * @Function
         * @public
         */
        replaceSpecialCharacters: function (value) {
            console.log('replace');
            if (value !== '') {
                return Number(value.replace(/[A-Za-z$\-]/g, ''));
            } else {
                return 0;
            }
        },
        createGUID: function () {
            var d = new Date().getTime();
            var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = (d + Math.random() * 16) % 16 | 0;

                d = Math.floor(d / 16);
                return (c === 'x' ? r : (r & 0x7 | 0x8)).toString(16);
            });
            return uuid;
        }

    },
    // END: utils
    cachedSelectors: {}

}; // END home global var

$(document).ready(function () {
    // activate utils
    helpers.global.utils.init();
});