define([
    'jquery',
    'asa/ASAUtilities'
], function ($, Utility) {
    var reachOutModule = {
        init: function ($htmlToRebind) {
            if ($htmlToRebind.find('.js-reachout-module').length) {
                //remove duplicate reachout header
                var $reachOutHeader = $('.js-reachout-header');
                if ($reachOutHeader.length === 2) {
                    $reachOutHeader.eq(1).remove();
                }
                //determine online/offline
                Utility.waitForAsyncScript('bLHNOnline', function () {
                    var $liveChatOnline = $('.js-livechat-online'),
                        $liveChatOffline = $('.js-livechat-offline');
                    if (window.bLHNOnline) {
                        $liveChatOnline.removeClass('hide');
                        $liveChatOffline.addClass('hide');
                    } else {
                        $liveChatOnline.addClass('hide');
                        $liveChatOffline.removeClass('hide');
                    }
                });
            }
        }
    };
    return reachOutModule;
});
