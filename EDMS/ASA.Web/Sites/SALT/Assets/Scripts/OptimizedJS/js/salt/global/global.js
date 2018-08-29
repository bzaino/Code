/*!
 * SALT.global
 */
define([
    'jquery',
    'salt',
    'configuration',
    'asa/ASAWebService',
    'jquery.cookie'
], function ($, SALT, configuration) {

    SALT.global = {
        utils: {
            init: function () {
                SALT.global.utils.formsAuthTimeout.setTimeoutIntervals();

                SALT.on('sessionTimeOut:reset', function () {
                    SALT.global.utils.formsAuthTimeout.cleartimeOut();
                });
            },

            formsAuthTimeout: {
                alertTimeoutInterval: 0,
                redirectTimeoutInterval: 0,
                aspxauthKeepAliveInterval: 0,
                visibilityEventObject: (function () {
                    var visibility = {};
                    if (typeof document.hidden !== 'undefined') { // Opera 12.10 and Firefox 18 and later support
                        visibility.hidden = 'hidden';
                        visibility.visibilityChange = 'visibilitychange';
                    } else if (typeof document.mozHidden !== 'undefined') {
                        visibility.hidden = 'mozHidden';
                        visibility.visibilityChange = 'mozvisibilitychange';
                    } else if (typeof document.msHidden !== 'undefined') {
                        visibility.hidden = 'msHidden';
                        visibility.visibilityChange = 'msvisibilitychange';
                    } else if (typeof document.webkitHidden !== 'undefined') {
                        visibility.hidden = 'webkitHidden';
                        visibility.visibilityChange = 'webkitvisibilitychange';
                    }
                    return visibility;
                }()),

                setTimeoutIntervals: function () {
                    var _this = this;
                    // only need to sync session timeout if the user is logged in.
                    require(['salt/models/SiteMember'], function (SiteMember) {
                        SiteMember.done(function (siteMember) {
                            if (siteMember.IsAuthenticated === 'true') {
                                $(document).on(_this.visibilityEventObject.visibilityChange, function () {
                                    if (!document[_this.visibilityEventObject.hidden] && $.cookie('IndividualId')) {
                                        var currentTime = new Date().getTime(),
                                            sessionExpirationTime = parseInt($.cookie('SessionTimeOut'), 10);
                                        if (currentTime >= sessionExpirationTime) {
                                            _this.sessionTimeoutRedirect();
                                        } else if (sessionExpirationTime > currentTime && sessionExpirationTime - currentTime < (3 * 60 * 1000)) {
                                            _this.sessionTimeoutNotification();
                                        }
                                    }
                                });

                                // set timers for session timeout alert and session timeout redirect
                                _this.cleartimeOut();
                            }
                        });
                    });
                },

                cleartimeOut: function () {
                    if ($.cookie('IndividualId')) {
                        clearInterval(this.alertTimeoutInterval);
                        clearInterval(this.redirectTimeoutInterval);
                        this.alertTimeoutInterval = setInterval(this.sessionTimeoutNotification, (configuration.apiEndpointBases.FormsAuthTimeoutValue - 3) * 60 * 1000); // timeout -3 minutes
                        this.redirectTimeoutInterval = setInterval(this.sessionTimeoutRedirect, (configuration.apiEndpointBases.FormsAuthTimeoutValue) * 60 * 1000); // timeout
                        if (!$.cookie('RememberMe') && this.aspxauthKeepAliveInterval === 0) {
                            //call keepAlive to generate/reset new aspxauth cookie and then setup interval
                            this.sessionAspxauthKeepAlive();
                            this.aspxauthKeepAliveInterval = setInterval(this.sessionAspxauthKeepAlive, (configuration.apiEndpointBases.FormsAuthTimeoutValue / 2) * 60 * 1000);
                        }
                        var date = new Date();
                        date = date.setMinutes(date.getMinutes() + configuration.apiEndpointBases.FormsAuthTimeoutValue);
                        document.cookie = 'SessionTimeOut=' + date + '; PATH=/;';
                    }
                },

                sessionAspxauthKeepAlive: function () {
                    $.ajax({
                        type: 'GET',
                        url: '/api/ASAMemberService/Beat',
                        cache: false,
                        contentType: 'application/json',
                    });
                },

                sessionTimeoutNotification: function () {
                    var d = new Date();
                    var alertOpened = d.getTime();
                    if ($.cookie('IndividualId') && !$.cookie('RememberMe')) {
                        alert('Your session will expire in 3 minutes, please save any progress.');
                    }
                    //If the user left the alert box open for more than a minute trigger the sessionTimeout
                    var dAfter = new Date();
                    if (dAfter.getTime() - alertOpened > (60 * 1000)) {
                        SALT.global.utils.formsAuthTimeout.sessionTimeoutRedirect();
                    }
                },

                sessionTimeoutRedirect: function () {
                    if ($.cookie('RememberMe')) {
                        //expire cookie in twelve hours
                        var exdate = new Date();
                        exdate.setTime(exdate.getTime() + 12 * 3600 * 1000);
                        var returnLocation = window.location.pathname + window.location.search + window.location.hash;
                        returnLocation = returnLocation.replace(/&/g, '%26');
                        document.cookie = 'RememberMeUrl=' + returnLocation + '; PATH=/; Domain=saltmoney.org; Expires=' + exdate.toString() + ';';
                    } else {
                        //clear/stop aspxauth keep-alive
                        clearInterval(this.aspxauthKeepAliveInterval);
                    }
                    window.location = '/Account/LogOff';
                }
            }

        },

        federalLoanTypeIds: ['CL', 'D1', 'D2', 'D3', 'D4', 'D5', 'GB', 'HL', 'PL', 'SF', 'SL', 'SU', 'SX'],
        perkinsLoanTypeIds: ['PU'],
        otherLoanTypeIds: ['IN', 'PC', 'PR', 'ST', 'NA']
    }; // END SALT global var

    $(document).ready(function () {

        // activate utils
        SALT.global.utils.init();
    });

    return SALT.global;
});
