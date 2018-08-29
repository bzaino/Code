/* global dcsMultiTrack */
require([
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'salt/analytics/webtrends-config',
    'underscore',
    'salt/LoginManager',
    'salt/analytics/webtrends',
    'foundation5',
    'jquery.cookie'
], function ($, SALT, SiteMember, Utility, WT, _) {


    function setFocus(element) {
        var elSetFoucus = function () {
            $(element).focus();
        };
        setTimeout(elSetFoucus, 350);
    }

    //Open overlay opens a reveal overlay and Publishes a overlay opened event with page specific data
    //  it takes 3 arguments
    //      -a css selector in string form e.g. '#foo' or '#bar'
    //      -a string with the overlay name to be passed to webtrends
    //      -a string with the url of the current page to be passed to webtrends
    SALT.openOverlay = function (selector, overlayName, currentPage) {
        $(selector).foundation('reveal', 'open');
    };

    SALT.on('open:customOverlay', function (overlay) {
        openWindowShade(overlay);
    });

    SALT.on('close:customOverlay', function (overlay) {
        closeWindowShade(overlay);
    });

    //Open overlay closes a reveal overlay
    //  it takes 1 argument
    //      -a css selector in string form e.g. '#foo' or '#bar'
    SALT.closeOverlay = function (selector) {
        $(selector).foundation('reveal', 'close');
    };
    
    $(document.body).on('click', '.js-reveal-close', function () {
        $(this).closest('.reveal-modal').foundation('reveal', 'close');
    });

    $('body').on('click', '.loginButton, .js-reg-button', function (e) {
        e.preventDefault();
        if (!$(e.currentTarget).is('[data-window-shade]')) {
            if ($(e.currentTarget).hasClass('loginButton')) {
                $(this).attr('data-window-shade', 'loginOverlay');
            } else {
                $(this).attr('data-window-shade', 'registrationOverlay');
            }
            $(this).trigger('click');
        }
    });

    $(document.body).on('click', '[data-window-shade]', function (e) {
        e.preventDefault();
        var windowshadeToOpen = $(this).attr('data-window-shade');
        toggleNoMenuHeader(windowshadeToOpen);
        if (windowshadeToOpen === 'registrationOverlay') {
            if ($.cookie('TooYoung')) {
                return;
            }
            if ($(window).height() < 750) {
                window.location = '/register' + window.location.search;
                return;
            }
        }
        if (windowshadeToOpen === 'loginOverlay') {
            if ($(window).height() < 400 || window.location.pathname === '/register') {
                window.location = '/logon';
                return;
            }
        }
        openWindowShade(windowshadeToOpen);
    });

    $(document.body).on('click keydown', '.window-shade-close', function (e) {
        if (e.type === 'click' || e.keyCode === 13) {
            var $windowShadeToClose = $(this).closest('.custom-overlay'),
                focusOnAfterSel = $(this).attr('data-focus-sel');
            closeWindowShade($windowShadeToClose.attr('id'));
            if (focusOnAfterSel) {
                $(focusOnAfterSel).focus();
            }
        }
    });

    function toggleNoMenuHeader(windowshadeToOpen) {
        if (windowshadeToOpen === 'registrationOverlay') {
            $('.js-registration-link').hide();
            $('.js-login-link').show();
        }
        else if (windowshadeToOpen === 'loginOverlay') {
            $('.js-login-link').hide();
            $('.js-registration-link').show();
        }
    }

    function windowShadeClosedCleanUp($windowShade) {
        $windowShade.find('form').trigger('reset');
        if ($windowShade.attr('id') === 'registrationOverlay') {
            SALT.trigger('resetControlsToDefault');
        }

        $('label[class="error"]').empty();
        $('ul.main-nav li a').removeClass('inactive');

        //Clean up server errors
        $('.js-server-error-text').text('');
        $('#email-reused').addClass('hidden');
    }

    function windowShadeOpenedCleanUp($windowShade) {
        if ($windowShade.attr('id') === 'registrationOverlay') {
            SALT.trigger('prePopulateSchool');
            SALT.trigger('showRadioButtons');
            SALT.publish('Registration:Start');
            Utility.handleRegWallWT('Wall_Signup_Start');
        }
        else if ($windowShade.attr('id') === 'loginOverlay') {
            Utility.handleRegWallWT('Wall_Login_Start');
            //need to reinitialize LoginManger
            Utility.trackLastVisitedURL();
            var returnUrl = Utility.getRerouteURL();
            loginOverlayHandler(returnUrl);
        }
        $windowShade.find('form').find(':input:visible:enabled:first').focus();
        $('ul.main-nav li a').addClass('inactive');
    }

    var loginOverlayHandler = function (returnUrl) {
        if (!returnUrl) {
            returnUrl = location.pathname + location.search;
        }
        SALT.LoginManager.init({
            logonFormName: '#loginForm',
            errorFieldName: '#errorCode',
            returnURLOverride: returnUrl
        });
    };

    $(document).ready(function () {
        if (!$.cookie('IndividualId')) {
            //stop after validation
            Utility.trackLastVisitedURL();
            var returnUrl = Utility.getRerouteURL();
            loginOverlayHandler(returnUrl);
            //third condition below for SWD-6480
            if (returnUrl &&  //there's return url
                window.location.href.indexOf("/register") === -1) {
                openWindowShade('loginOverlay');
            }
        }
    });

    function openWindowShade(windowShade) {
        var $windowShade = $('#' + windowShade);
        if ($windowShade.is(':hidden')) {
            var currentlyOpenWindowShadeID = $('.custom-overlay:visible').attr('id');
            //make sure all other window shades are closed in order that their forms be reset
            //We dont want forms to be reset when the confirm personal info is about to be opened, check for this case before closing
            if (currentlyOpenWindowShadeID && windowShade !== 'confirmPersonalInfo') {
                closeWindowShade(currentlyOpenWindowShadeID);
            } else {
                //ConfirmPersonalInfo case, just slide up the registration form rather than fully closing and restting the form
                $('.custom-overlay').slideUp('slow');
            }
            // open the window shade
            $windowShade.slideDown('slow', function () {
                //clean up after opening the window shade
                windowShadeOpenedCleanUp($windowShade);
            });
        }
    }

    function closeWindowShade(windowShade) {
        var $windowShade = $('#' + windowShade);
        $windowShade.slideUp('slow', function () {
            //clean up after closing the window shade
            windowShadeClosedCleanUp($windowShade);
        });
    }

});
