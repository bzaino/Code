/* ------------------------------------------------------------------------
    Class: ASAYouTubeVideo
    Author: Rob Mendes and mlasell
------------------------------------------------------------------------- */
define([
    'jquery',
    'iframe_api',
    'googleSWF',
    'asa/ASAUtilities'
],  function ($, YT, swfobject, asaUtilities) {
    var asaYouTube = {
        gSettings : '',
        playerObject : {},
        initialize: function (parmSettings) {
            asaYouTube.gSettings = $.extend({
                markupTemplate: '<iframe id="ytplayer" type="text/html" style="height:100%; width:100%" src ="{path}" frameborder="0" allowfullscreen >',
                playerId: 'js-overlay-player',
                utubeParam: '?enablejsapi=1&amp;playerapiid=Ytplayer&amp;cc_load_policy=1&amp;autoplay=',
                autoPlay: true,
                vidUrl: ''
            }, parmSettings);
            asaYouTube.addContent();
        },
        insertYouTubeIFramePlayer: function (YouTubeId) {
            var params = { allowScriptAccess: 'always', allowfullscreen: 'true' },
            atts = { id: asaYouTube.gSettings.playerId, autoplay: asaYouTube.gSettings.autoPlay ? 1 : 0, wmode: 'transparent'};
            YouTubeId += asaYouTube.gSettings.autoPlay ? asaYouTube.gSettings.utubeParam + '1' :  asaYouTube.gSettings.utubeParam + '0';
			swfobject.embedSWF('https://www.youtube.com/v/' + YouTubeId + '&version=3', asaYouTube.gSettings.playerId, '100%', '100%', '8', null, null, params, atts);

        },
        //The API will call asaYouTube.function when the video player is ready.
        onYouTubePlayerReady: function (event) {
            asaYouTube.playerObject = document.getElementById(asaYouTube.gSettings.playerId);
        },
        getYouTubeId: function (url) {
            var movieid = asaUtilities.getParameterByNameFromString('v', url.trim());
            // youtu.be link
            if (movieid === '') {
                movieid = url.split('youtu.be/');
                movieid = movieid[1];
                if (movieid.indexOf('?') > 0) {
                    movieid = movieid.substr(0, movieid.indexOf('?')); // Strip anything after the ?
                }
                if (movieid.indexOf('&') > 0) {
                    movieid = movieid.substr(0, movieid.indexOf('&')); // Strip anything after the &
                }
                if (movieid.indexOf('/') > 0) {
                    movieid = movieid.substr(0, movieid.indexOf('/')); // Strip anything after the /
                }
            }
            return movieid;
        },
        isMobilePhone: function () {
            //0 = non mobile, 1= tablet 2=phone
            if (navigator.userAgent.match(/iPhone/i) || navigator.userAgent.match(/iPad/i) || navigator.userAgent.match(/Android/i)) {
                //check for tablet
                if (screen.width > 380 && !navigator.userAgent.match(/iPhone/i)) {
                    return 1;
                } else {
                    return 2;
                }
            }
            return 0;
        },
        addContent: function () {
            var youTubeId, embedCode;
            if (asaYouTube.gSettings.vidUrl) {
                //if param was passed take it as the video ide
                youTubeId = asaYouTube.getYouTubeId(asaYouTube.gSettings.vidUrl);
            }
            else {
                //otherwise use the url
                youTubeId = asaYouTube.getYouTubeId($('#playerURL').attr('href'));
            }
            //use on when in mobile mode
            //0 = non-mobile, 1 = tablet, 2 = phone
            if (asaYouTube.isMobilePhone() === 2) {
                youTubeId += asaYouTube.gSettings.utubeParam + '0&playsinline=1&fs=1version=2&modestbranding=0';
                embedCode = asaYouTube.gSettings.markupTemplate.replace(/{path}/g, 'https://www.youtube.com/embed/' + youTubeId);
                //add content to template
                $('#' + asaYouTube.gSettings.playerId).html(embedCode);
            } else if (asaYouTube.isMobilePhone() === 1) {
                youTubeId += asaYouTube.gSettings.utubeParam + '1&amp;fs=1&amp;version=3&amp;modestbranding=0';
                embedCode = asaYouTube.gSettings.markupTemplate.replace(/{path}/g, 'https://www.youtube.com/embed/' + youTubeId);
                //add content to template
                $('#' + asaYouTube.gSettings.playerId).html(embedCode);
            } else {
                $('.show-vid-overlay').click(function () {
                    asaYouTube.insertYouTubeIFramePlayer(youTubeId);
                });
            }
        }
    };
    return asaYouTube;
});
