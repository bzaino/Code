define([
    'jquery',
    'salt',
    'asa/ASAUtilities',
    'modules/HomePage/Views',
    'owl.carousel'
], function ($, SALT, Utility, pageHelpers) {
    var owlObject,
        unauthHome = {
        renderUnAuthHome: function (data, persona) {

            //TODO put this into exp mgr into all 8 personas so that it can be changed by a content editor
            document.title = 'Salt: Education Unlocked. Dreams Unlimited.';

            var _this = this;

            data.configuration = data.configuration || {};

            Utility.renderDustTemplate('HomePage/HomeDefault', data, function (err, out) {
                var parsedHTML = $.parseHTML(out);

                $('#rendered-content').html(parsedHTML).addClass('homepage js-SPA-enabled js-main-content');

                //Remove any fixed positioning the footer might have, in case the user came from an infinite scroll page, which has a bottom fixed footer.
                $('.footer-container').removeClass('bottom-fixed');

                _this.initializeOwlCarousel();
                _this.slideEventsBinder();

                //determine if SALT Tour should be displayed.
                pageHelpers.openSaltTourOverlay();
                pageHelpers.reTriggerInlineScripts('#rendered-content');
                data.mainContent[0].record = {attributes: {page_description: data.metaDescription}};
                SALT.trigger('SPA:PageViewed', data);
                //if it's a full reload on un auth home page, the updateMeta function in application.js won't be trigger, so in this case, load metaDescription Manually.
                $('meta[name=description]').attr('content', data.metaDescription);
            }, null, true);
        },
        slideMovedCallback: function (owl) {
            var $currentSection = $('section[itemIndex="' + owl.data('owlCarousel').currentItem + '"]'),
                persona = $currentSection.attr('persona');
            /*use history pushState to change the url without reloading.*/
            if (persona === 'Home') {
                persona = '';
            }
            history.pushState('', '', '?Type=' + persona);
            //add active link to the current selection
            $currentSection.find('a[href*="' + persona + '"]').addClass('is-active-link');
        },
        triggerNavigate: function (goToPersona, animationNeeded) {
            /*get target page index from persona, if persona doesn't exist, go to home.*/
            var goToIndex = goToPersona ? $('section[persona="' + goToPersona + '"]').attr('itemIndex') : 0;
            /*use plugin method goTo to animate to the target page.*/
            if (animationNeeded) {
                owlObject.goTo(parseInt(goToIndex, 10));
            } else {
                owlObject.jumpTo(parseInt(goToIndex, 10));
            }

        },
        navigateOnClicks: function (e) {
            e.preventDefault();
            var $target = $(e.currentTarget),
                goToPersona = Utility.getParameterByNameFromString('Type', $target.attr('href'));
            //check whether it's unauth home page or not.
            //if it's not unauth home, we don't need to do navigateslide event.
            if ($('.js-homecarousel-container').length) {
                unauthHome.triggerNavigate(goToPersona, true);
            }
            var activityType = {
                    '': 'Home',
                    'AssociatesDegree': 'Associates Degree',
                    'BachelorsDegree': 'Bachelors Degree',
                    'GraduateSchool': 'Graduate School',
                    'AlumniPostSchool': 'Alumni Post School',
                    'MasterMoney': 'Master Money',
                    'RepayStudentDebt': 'Repay Student Debt',
                    'PayForSchool': 'Pay For School',
                    'FindAJob': 'Find A Job'
                };
            //if the target doesn't have class persona-btn, that means it's Nav links in the middle of the pages.
            if ($target.hasClass('js-persona-btn')) {
                SALT.publish('unauthpersona:button:click', {'activity_name': 'UA_HP_User_Goals', 'activity_type': activityType[goToPersona]});
            } else {
                SALT.publish('unauthpersona:button:click', {'activity_name': 'Service_Nav_User_Goals', 'activity_type': activityType[goToPersona]});
            }
        },
        triggerSlide: function (e) {
            e.preventDefault();
            var direction = $(e.currentTarget).attr('title');
            if (direction === 'previous') {
                owlObject.prev();
            } else {
                owlObject.next();
            }
        },
        slideEventsBinder: function () {
            $('.js-arrow').unbind('click').click(this.triggerSlide);
            $('.home-link').unbind('click').click(this.navigateOnClicks);
        },
        initializeOwlCarousel: function () {
            //508 Compliance, SWD-6602, reorder the tabbing structure.
            $('.js-tabindex-3:visible').attr('tabindex', '3');

            //We cant add .owl-carousel to the carousel container to begin with, because the owl css styles these items to not show until the js has been initialized
            //In order to show the banner image rather than blank content, we add .owl-carousel right before we are ready to init the
            $('.js-homecarousel-container').addClass('owl-carousel');
            /*init the carousel plugin*/
            var $owlElement = $('.owl-carousel');
            $owlElement.owlCarousel({
                pagination: false,
                slideSpeed: 300,
                singleItem: true,
                afterMove: this.slideMovedCallback,
                afterInit: function () {
                    $('.js-carousel-slider').show();
                }
            });
            owlObject = $owlElement.data('owlCarousel');
            var goToPersona = Utility.getParameterByNameFromString('Type', Utility.getLocationSearch());
            this.triggerNavigate(goToPersona, false);

            //Listen to owl events:
            //setup to trigger mousedown event for Webtrends heat maps
            //this was a bug with the way the owl carousel handles the event.
            //It stopped it from being sent along so Webtrends heat map event did not fire.
            $('.js-homecarousel-container').on('mousedown.owl.carousel', function (event) {
                Utility.waitForAsyncScript('WebtrendsHeatMap', function () {
                    WebtrendsHeatMap.hm_clickEvent(Webtrends.dcss.dcsobj_0, event);
                });
            });
            $(document).foundation();
        }
    };
    return unauthHome;
});
