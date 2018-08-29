/**
 * External SALT RequireJS Configuration
 ***************************************
 * Enables usage of require in scripts loaded outside of main.js.
 *
 * Usage:
 * - Include before <script data-main="path/to/main" src="path/to/require.js"> </script>.
 *
 * Notes:
 * - Paths followed by the comment // AMD are already AMD-aware, therefore do not need
 *   entries in the shim section.
 */

/*jshint unused:false */
requirejs.config({
    waitSeconds: 0,

    packages: [
        {
            'name': 'salt',
            'main': 'salt'
        },
        {
            'name': 'salt/global',
            'main': 'global'
        }
    ],

    paths: {
        /**
         * RequireJS Plugins
         */
        'promise': 'requirejs-promise', // AMD

        /**
         * Registration.js Path
         */
        'registration': '../js/salt/registration',

        /**
         * Lessons
         */
        'lesson1': '../../../lessons/Lesson1/js',

        'lesson2': '../../../lessons/Lesson2/app',
        'lesson2/plugins': '../../../lessons/Lesson2/assets/js/plugins',
        'lesson2/libs': '../../../lessons/Lesson2/assets/js/libs',

        'lesson3': '../../../lessons/Lesson3/app',
        'lesson3/plugins': '../../../lessons/Lesson3/assets/js/plugins',
        'lesson3/libs': '../../../lessons/Lesson3/assets/js/libs',

        /**
         * Datejs
         */
        'datejs': 'libs/datejs/date',

        /**
         * Dust template library and any helpers
         */
        'dust-core': 'libs/dust/dust-core',
        'dust': 'libs/dust/dust-full',
        'dust-helpers': 'libs/dust/dust-helpers',

        /**
         * Compiled DUST templates
         THESE PATHS ARE RELATIVE TO OPTIMIZEDJS FOLDER
         */
        'Compiled': '../../../templates/compiled/',
   
        /* Paths to compiled templates for JSI tool */
        'JSISchoolDropDown': '../../../templates/compiled/JSISchoolDropDown',
        'SalaryEstimatorResults': '../../../templates/compiled/SalaryEstimatorResults',

        /**
         * Underscore + Backbone
         * (Not using jrburke's AMD versions, as they're quite far behind. Shims required.)
         */
        'underscore': 'libs/documentcloud/underscore',
        'backbone': 'libs/documentcloud/backbone',

        /**
         * Backbone Marionette. (Using AMD version.)
         */
        'backbone.marionette': 'libs/marionettejs/backbone.marionette', // AMD
        'backbone.wreqr': 'libs/marionettejs/backbone.wreqr', // AMD

        /**
         * Brightcove
         */
        'bc': '//players.brightcove.net/1894469414001/SJLISQXFb_default/index.min', // AMD

        /**
         * Highcharts
         */
        'highcharts': 'libs/highcharts/highcharts',
        'highcharts4': 'libs/highcharts/highcharts4',
        'highcharts4-3d': 'libs/highcharts/highcharts4-3d',

        /**
         * jQuery
         */
        'jquery': 'libs/jquery/jquery',

        'history': 'libs/history',

        /**
         * Foundation 5
         */
        'foundation5': 'libs/foundation5/foundation.plugins',
        'foundation.lib': 'libs/foundation5/foundation',

        /**
         * jQuery UI & jQuery Plugins
         */
        'jquery.ui':               'libs/jquery/jquery.ui.min',
        'jquery.ui.widget':        'libs/jquery/plugins/jquery.ui.widget',
        'jquery.cookie':           'libs/jquery/plugins/jquery.cookie',            // AMD
        'jquery.dotdotdot':        'libs/jquery/plugins/jquery.dotdotdot',
        'jquery.fileupload':       'libs/jquery/plugins/jquery.fileupload',        // AMD
        'jquery.iframe-transport': 'libs/jquery/plugins/jquery.iframe-transport',  // AMD
        'jquery.client':           'libs/jquery/plugins/jquery.client',
        'jquery.colorbox':         'libs/jquery/plugins/jquery.colorbox',
        'jquery.commify':          'libs/jquery/plugins/jquery.commify',
        'jquery.dropkick':         'libs/jquery/plugins/jquery.dropkick',
        'jquery.formatCurrency':   'libs/jquery/plugins/jquery.formatCurrency',
        'jquery.hoverIntent':      'libs/jquery/plugins/jquery.hoverIntent',
        'jquery.json':             'libs/jquery/plugins/jquery.json',
        'jquery.tools':            'libs/jquery/plugins/jquery.tools',
        'jquery.tools-126':        'libs/jquery/plugins/jquery.tools-126',
        'jquery.validate':         'libs/jquery/plugins/jquery.validate',
        'jquery.waypoints':        'libs/jquery/plugins/jquery.waypoints',
        'jquery.serializeObject':  'libs/jquery/plugins/jquery.serializeObject',
        'jquery.autocomplete':     'libs/jquery/plugins/jquery.autocomplete',
        'jquery.rwdImageMaps':     'libs/jquery/plugins/jquery.rwdImageMaps',
        'jquery.zglossary':        'libs/jquery/plugins/jquery.zglossary',

        /**
         *   Lesson-specific custom plugins
         */
        'jquery.styledInputs': 'libs/jquery/plugins/jquery.styledInputs',
        'jquery.styledDropDowns': 'libs/jquery/plugins/jquery.styledDropDowns',

        /**
         * Foundation jQuery Plugins
         */
        'jquery.offcanvas':                   'libs/foundation/jquery.offcanvas',

        /*
        * Un auth home page carousel plugin
        */
        'owl.carousel':                       'libs/owl.carousel',
        /*
        * youtube iframe_api
        */
        'iframe_api':                       'libs/iframe_api',
        'googleSWF':                        'https://ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject'
    },

    shim: {

        /**
         * Youtube Iframe
         */
        'iframe_api': {
            exports: 'YT'
        },
        'googleSWF': {
            exports: 'swfobject'
        },
        /**
         * Owl carousel
         */
        'owl.carousel': {
            deps: ['jquery']
        },
        /**
         * Datejs
         */
        'datejs': {
            exports: 'Date'
        },

        /**
         * Dust template library and any helpers
         */
        'dust': {
            exports: 'dust'
        },
        'dust-core': {
            exports: 'dust'
        },

        /**
         * Underscore + Backbone
         */
        'underscore': {
            exports: '_'
        },

        'backbone': {
            deps: ['jquery', 'underscore'],
            exports: 'Backbone'
        },

        /**
         * Lessons
         */
        'lesson2/plugins/backbone.layoutmanager': {
            deps: ['backbone']
        },
        'lesson2/plugins/backbone-asa-persistence': {
            deps: ['backbone']
        },
        'lesson2/plugins/asa-plugins': {
            deps: ['jquery']
        },

        'lesson3/plugins/backbone.layoutmanager': {
            deps: ['backbone']
        },
        'lesson3/plugins/backbone-asa-persistence': {
            deps: ['backbone']
        },
        'lesson3/plugins/asa-plugins': {
            deps: ['jquery']
        },

        'dust-helpers': {
            deps: ['dust']
        },
        /**
         * Highcharts Version 2 for Lessons
         */
        'highcharts': {
            exports: 'Highcharts'
        },

        /**
         * Highcharts Version 4
         */
        'highcharts4': {
            exports: 'Highcharts',
            deps: ['jquery']
        },

        /**
         * Highcharts Version 4 with 3d library
         */
        'highcharts4-3d': {
            deps: ['jquery', 'highcharts4']
        },

        /**
         * Webtrends
         */
        'wtoptimize': {
            exports: 'WTOptimize'
        },

        /**
         * jQuery
         */
        'jquery': {
            exports: 'jQuery'
        },

        /**
         * jQuery UI & jQuery Plugins
         */
        'jquery.ui': {
            deps: ['jquery']
        },
        'jquery.ui.widget': {
            deps: ['jquery']
        },
        'jquery.client': {
            deps: ['jquery']
        },
        'jquery.colorbox': {
            deps: ['jquery']
        },
        'jquery.commify': {
            deps: ['jquery']
        },
        'jquery.dotdotdot': {
            deps: ['jquery']
        },
        'jquery.dropkick': {
            deps: ['jquery']
        },
        'jquery.formatCurrency': {
            deps: ['jquery']
        },
        'jquery.hoverIntent': {
            deps: ['jquery']
        },
        'jquery.json': {
            deps: ['jquery']
        },
        'jquery.tools': {
            deps: ['jquery']
        },
        'jquery.tools-126': {
            deps: ['jquery']
        },
        'jquery.validate': {
            deps: ['jquery']
        },
        'jquery.serializeObject': {
            deps: ['jquery']
        },

        /**
         *   Lesson-specific custom plugins
         */
        'jquery.styledDropDowns': {
            deps: ['jquery']
        },
        'jquery.styledInputs': {
            deps: ['jquery']
        },

        'jquery.autocomplete': {
            deps: ['jquery']
        },

        'jquery.rwdImageMaps': {
            deps: ['jquery']
        },

        'jquery.zglossary': {
            deps: ['jquery']
        },
        /**
         * Foundation 5
         */
        'foundation5': {
            deps: ['jquery', 'foundation.lib']
        },
        'foundation.lib': {
            deps: ['jquery']
        },

        /**
         * Foundation 3 jQuery Plugins
         */
        'jquery.offcanvas': {
            deps: ['jquery']
        }
    }
});
