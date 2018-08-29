/*global getParameterByName */
require([
    'require',
    'underscore',
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'asa/ASAUtilities',
    'modules/overlays',
    'foundation5'
], function (req, _, $, SALT, SiteMember, utility) {

    SiteMember.done(function (siteMember) {
        if (siteMember.IsAuthenticated === 'true') {
            if (siteMember.DashboardEnabled) {
                require(['modules/Dashboard', 'modules/ProgressWidgets']);

                //Set the cookie to take the user to onboarding on their next visit to /home if they dont yet have goal ranking responses foo
                if (!siteMember.goalRankResponses.length) {
                    document.cookie = 'NeedsOnboarding = true; path=/';
                }
                var redraw = function () {
                    if ($(window).width() > 1025) {
                        //f-dropdown adds inline styling to #js-sidebar as it opens - which persists if the browser is resized larger.
                        //By removing the inline style, the div will revert to the proper css class for it's media query.
                        $('#js-sidebar').removeAttr('style');
                    }
                };
                var debouncedRedraw = _.debounce(redraw, 400);
                $(window).on('resize', debouncedRedraw);
            }
            if (siteMember.OnboardingEnabled) {
                require(['modules/Onboarding']);
            }

            $(document.body).click(function () {
                SALT.trigger('sessionTimeOut:reset');
            });
        }
    });
    $(function () {
        //pass all query strings for mobile devices on logon and signup page.
        var getPathNameFromString = function (string) {
            return string.substring(string.indexOf('/'), string.indexOf('?') === -1 ? string.length : string.indexOf('?'));
        };
        $(document.body).on('click', '.registration-link', function (e) {
            var queryString,
                href;
            e.preventDefault();
            queryString = utility.getLocationSearch();
            href = $(this).attr('href');

            if (queryString) {
                window.location.href = getPathNameFromString(href) + queryString;
            } else {
                window.location.href = getPathNameFromString(href);
            }
        });

        var searchOpenedYet = false;
        $(document.body).on('click keydown', '.js-search-magn', function (e) {
            if (e.type === 'click' || e.which === 13) {
                e.preventDefault();

                //If this is the first time opening the search bar on the page fire a SALT event
                if (!searchOpenedYet) {
                    SALT.trigger('search:opened:firsttime');
                    searchOpenedYet = true;
                }

                $('.js-search-form').toggleClass('hide').find(':input:visible:enabled:first').focus();
                $('.js-search').toggleClass('hide');
                $('.js-header').toggleClass('search-magn-active');
            }
        });
        // functionality for x button to clear text and shutoff autocomplete
        $(document.body).on('click keydown', '.js-search-x', function (e) {
            if (e.type === 'click' || e.which === 13) {
                e.preventDefault();
                e.stopPropagation();
                $('#searchCriteria').attr('value', '').focus().autocomplete('close');
                $('.autocomplete-suggestions').hide();
            }
        });
        /*to close the search box when clicked anywhere on the page */
        $(document.body).on('click', function (e) {
            if (!($(e.target).is('#search-btn, .js-search-label, #searchForm .search'))) {
                $('.js-search-form').addClass('hide');
                $('.js-search').addClass('hide');
                $('.js-header').removeClass('search-magn-active');
            }
        });
        //on click of menu-btn and profile-btn
        //if the .js-search-form does not have class hide then add class hide
        $(document.body).on('click', '.salt-header .dd-opener', function (e) {
            if (!$('.js-search-form').hasClass('hide')) {
                $('.js-search-form').addClass('hide');
                $('.js-search').addClass('hide');
            }
        });
        $(document.body).on('click', '.js-scroll-top-button', function (e) {
            e.preventDefault();
            $('html, body').animate({scrollTop: 0}, 300);
        });

        $(document.body).on('click', '.no-accordion a, .dropdown li>a', function (e) {
            Foundation.libs.dropdown.close($('.left-menu'));
        });

        $(document.body).on('keydown', '.glossaryTerm', function (e) {
            if (e.which === 9) {
                $('#glossaryClose').click();
            }
        });

        $(document.body).on('click', '.left-menu li:not(.menu-copyright) a, .left-menu .no-accordion a', function (e) {
            var itemText = e.target.text.toString();
            SALT.publish('dashboard:action:taken', {'activity_name': 'Main Menu', 'activity_type': itemText});
        });
        $(document.body).on('click keydown', '.js-f-skip-link', function (e) {
            if (e.type === 'click' || e.which === 13) {
                $('.js-footer-container').focus();
            }
        });
        $(document.body).on('click keydown', '.js-rr-skip-link', function (e) {
            if (e.type === 'click' || e.which === 13) {
                $('#js-sidebar').focus();
            }
        });
        $(document).on('click', '.liveChatLink, .js-live-chat-now-btn', function (e) {
            e.preventDefault();
            $('#aLHNBTN').click();
            var actName = $(this).next('.js-livechat-WT-act').val();
            //If we didnt find a "js-livechat-WT-act" value, check the data-wt-value field on the input itself
            //Newer links will have this field, as it allows for more flexibility in the structure of the html (the .next call enforces that js-livechat-wt-act be directly after the input, which is bad)
            actName = actName ? actName : $(this).attr('data-wt-value');
            if (actName) {
                SALT.publish('Standard:Action:Start', { 'activity_name': actName});
            }
        });
        //the dropdowns on dashboard do not close when clicked on right rail
        //the data-dropdown-content a foundation element is impacting this. It is required for the right rail to close onsmall screens. So data-dropdown-content is being removed for screen sizes bigger than 640.
        $(window).on('resize', function () {
            if ($(window).width() > 1025) {
                $('#js-sidebar').attr('data-dropdown-content', null);
            } else {
                $('#js-sidebar').attr('data-dropdown-content', '');
            }
        });
        SiteMember.done(function (siteMember) {
            if (siteMember.IsAuthenticated === 'true') {
                if (!$.cookie('IndividualId')) {
                    //In remembered state the links need re authorisation
                    $('.MM101 a').attr('href', '/index.html?ReturnUrl=' + $('.MM101 a').attr('href'));
                    $('.SC a').attr('href', '/index.html?ReturnUrl=' + $('.SC a').attr('href') + '/login.jspa');
                }
            }
        });

        // stop the site search form from submitting if blank, aka 0 characters -khennings
        $('#searchForm').on('submit', function (e) {
            var searchValue = $('#searchCriteria').val().length;
            if (searchValue === 0) {
                e.preventDefault();
            } else {
                return;
            }
        });

        // stop the site search form from submitting if blank, aka 0 characters -khennings
        $('#mobileSearchForm').on('submit', function (e) {
            var searchValue = $('#mobileSubmit').val().length;
            if (searchValue === 0) {
                e.preventDefault();
            } else {
                return;
            }
        });

        req(['salt/analytics/webtrends']);


        $(document.body).on('change', '.js-profileQA-container input:checkbox, #js-tag-selection-container input:checkbox', function () {
            var $this = $(this),
                thisName = $this.attr('name');
            // if the current selection is "None", uncheck all other options
            if ($this.hasClass('js-none') && $this.is(':checked')) {
                $('input[name=' + thisName + ']').removeAttr('checked');
                $this.attr('checked', '');
                // hide the "other" input field and clear it.
                $this.closest('.js-question-container').find('.other-ans').hide().find('.js-other-input').val('');
                //prevent unchecking the only checked checkbox
            } else if (!$('input[name=' + thisName + ']').is(':checked') && !$this.hasClass('js-single-check')) {
                $this.attr('checked', '');
                // user clicked on "other" answer, show/hide the text field
            } else if ($this.hasClass('js-other')) {
                if ($this.is(':checked')) {
                    $this.siblings('.other-ans').show();
                    // Uncheck none answers.
                    $('input[name=' + thisName + '].js-none').removeAttr('checked');
                } else {
                    // hide the "other" input field and clear it.
                    $this.siblings('.other-ans').hide().find('.js-other-input').val('');
                }
                // else if the user selects an aption that is not "none", uncheck "none"
            } else {
                $('input[name=' + thisName + '].js-none').removeAttr('checked');
            }
        });
        // when a user modifies an "other" input field, concat the the value to the value of corresponding input field
        $(document.body).on('change', '.js-other-input', function () {
            var otherFieldValue = document.getElementById(this.id.substr(this.id.indexOf('ans'))),
                customVal = otherFieldValue.value.split('-')[2];
            // if we previousely didn't have a custom value stored
            if (!customVal) {
                otherFieldValue.value += '-' + this.value;
            } else {
                // if we have a previous custom value, delete it
                var customValIndex = otherFieldValue.value.indexOf(customVal);
                otherFieldValue.value = otherFieldValue.value.substring(0, customValIndex - 1);
                // if we have a new custom value to replace the old one, use it, other wise just delete it
                if (this.value) {
                    otherFieldValue.value += '-' + this.value;
                }
            }
        });

        //Open rel="external" links in new window

        $("a[rel*='external']").live('click', function () {
            var externalLinkMessage = "You are leaving the Salt\xAE site and will be directed to a third-party site. Please read the Privacy Policy and Terms and Conditions of that site.";
            if (confirm(externalLinkMessage)) {
                window.open(this.href);
            }
            return false;
        });
    });
});
