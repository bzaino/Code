require([
    'jquery',
    'salt',
    'modules/overlays',
    'jquery.cookie'
], function ($, SALT) {
    $('#underage').click(function () {
        $.cookie('TooYoung', 'true', { path: '/' });
        SALT.trigger('TooYoung');
        SALT.trigger('open:customOverlay', 'UnderageMessage');

        // wait for 5 seconds. if it is not a sidebar form, close UnderageMessage overay
        if (!$('#sidebar-forms').length) {
            setTimeout(function () {
                SALT.trigger('close:customOverlay', 'UnderageMessage');
                $('.js-signup-link-text').css('color', '#dcddde');
            }, 5000);
        }
    });

    $('.js-back-to-reg').click(function () {
        //Close cofirm personal info windowshade, then open registration shade if there is one (there wont be on the registration page)
        SALT.trigger('close:customOverlay', 'confirmPersonalInfo');
        SALT.trigger('open:customOverlay', 'registrationOverlay');
    });
});
