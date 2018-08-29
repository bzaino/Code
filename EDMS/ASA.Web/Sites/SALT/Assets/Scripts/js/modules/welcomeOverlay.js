require([
    'jquery',
    'underscore',
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'foundation5',
    'jquery.cookie'
], function ($, _, Utility, SiteMember) {
    $(document).ready(function () {
        //We only want to check about presenting the welcome overlay if the user is not opted into the onboarding process.
        SiteMember.done(function (siteMember) {
            if (!siteMember.OnboardingEnabled) {
                checkCookie();
            }
        });
        hideOrganizationProducts(SiteMember);
    });
    function checkCookie() {
        var welcomeCookie = $.cookie('ShowWelcome');
        if (welcomeCookie) {
            presentWelcomeOverlay();
        }
    }

    function hideOrganizationProducts(SiteMember) {
        SiteMember.done(function (siteMember) {
            if (siteMember.IsAuthenticated === 'true') {
                var jobSearchFlag = false,
                    scholarshipSearchFlag = false;
                jobSearchFlag = Utility.isSubscribedToProduct(siteMember, 3);
                scholarshipSearchFlag = Utility.isSubscribedToProduct(siteMember, 4);
                var $jobSearchContainer = $('.js-jobsearch-container'),
                    $scholarshipSearchContainer = $('.js-scholarship-container');
                if (jobSearchFlag && scholarshipSearchFlag) {
                    $scholarshipSearchContainer.hide();
                } else {
                    if (!jobSearchFlag) {
                        $jobSearchContainer.hide();
                    }
                    if (!scholarshipSearchFlag) {
                        $scholarshipSearchContainer.hide();
                    }
                }
            }
        });
    }

    function presentWelcomeOverlay() {
        document.cookie = 'ShowWelcome = true; expires=Mon, 9 Oct 2012 20:47:11 UTC;';
        //In IE .foundation() has not always been called at this point, so we need to make sure foundation has been init'd
        $(document).foundation();
        $('#welcomeO ').foundation('reveal', 'open');
    }
});
