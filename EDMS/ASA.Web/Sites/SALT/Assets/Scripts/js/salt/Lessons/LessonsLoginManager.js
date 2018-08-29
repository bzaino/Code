define(function (require, exports, module) {
    var $ = require('jquery'),
        SALT = require('salt'),
        Utility = require('asa/ASAUtilities'),
        SiteMember = require('salt/models/SiteMember');
    require('jquery.cookie');


    SALT.LoginManager = {
        logonForm: null,
        errorField: null,
        registrationLink: null,
        returnURLOverride: null,

        init: function (config) {
            this.logonForm = $(config.logonFormName);
            this.errorField = $(config.errorFieldName);
            this.registrationLink = $(config.registrationLink);

            this.returnURLOverride = config.returnURLOverride || this.returnURLOverride;

            this.logonForm.submit(SALT.LoginManager.authenticate);
            this.registrationLink.click(SALT.LoginManager.registration);
            $('.cbox').remove();
            SiteMember.done(function (siteMember) {
                if (siteMember.IsAuthenticated === 'true') {
                    if (SiteMember.get('PrimaryEmailKey')) {
                        $('input[name="UserName"]').val(SiteMember.get('PrimaryEmailKey'));
                        $('input[name="Password"]').focus();
                    }
                }
            });
        },
        authenticate: function (event) {
            event.preventDefault();
            var loginForm = SALT.LoginManager.logonForm;
            var errorCode = SALT.LoginManager.errorField;
            errorCode.html('');

            var data = loginForm.serialize();
            var returnURLstring = '';
            var returnUrl = Utility.getParameterByName('ReturnUrl');
            var rememberMeVal = SALT.LoginManager.logonForm[0];

            if (SALT.LoginManager.returnURLOverride) {
                returnUrl = encodeURIComponent(SALT.LoginManager.returnURLOverride);
            }

            if (returnUrl !== '') {

                returnURLstring = '&returnUrl=' + returnUrl;
            // added logic to redirect to homepage when logon occurs on registration page
            } else {
                returnURLstring = '&returnUrl=' + location.pathname;
            }

            if (location.pathname === '/register/' || location.pathname.toLowerCase() === '/home/newpassword') {
                returnURLstring = '&returnUrl=' + location.host;
                // SWD-1663 -- Sitewide content search. If a user logs in from their search results ("searchCriteria"),
                // return them to the same location and query string they searched for. -khennings
            } else if (location.href.indexOf('searchCriteria') > 0) {
                returnURLstring = '&returnUrl=' + location.pathname + location.search;
            }

            data += returnURLstring;

            // standard spinner cannot be used- it prevents error fixing
            $('.loading').show();
            if (loginForm.attr('id') === 'loginForm') {
                $('#loginBtn span').text('Logging In...');

            } else {
                $('.loginBtn span').text('Logging In...');
            }


            $.ajax({
                type: 'POST',
                url: '/Account/LogOn',
                data: data,
                dataType: 'json'
            })
            .done(function (result) {
                if (result.Success && result.ReturnUrl) {
                    SiteMember.set('IsAuthenticated', 'true');
                    SALT.publish('siteMember:signin:complete', {
                        location: {
                            pathname: '/SignIn.html'
                        },
                        document: {
                            title: 'SignIn'
                        },
                        siteMember: result.Member
                    });
                    if (result.Member.IndividualId) {
                        document.cookie = 'IndividualId=' + result.Member.IndividualId + '; PATH=/; Domain=saltmoney.org;';
                    }
                    if ($.cookie('RememberMe')) {
                        $.removeCookie('RememberMe', { path: '/', domain: 'saltmoney.org' });
                        $.removeCookie('RememberMeEmail', { path: '/', domain: 'saltmoney.org' });
                        $.removeCookie('RememberMeUrl', { path: '/', domain: 'saltmoney.org' });
                    }
                    if (rememberMeVal.checkRememberMe.checked && result.Member.PrimaryEmailKey) {
                        var exdate = new Date();
                        exdate.setDate(exdate.getDate() + 730);
                        document.cookie = 'RememberMe=true; PATH=/; Expires=' + exdate.toString() + ';';
                        //webtrends call for Rememberme  // orig call //dcsMultiTrack('WT.z_actname', 'Remember Me', 'WT.z_acttype', 'save', 'WT.z_acttx', '1');
                        SALT.publish('LoginManager:RememberMe');
                    }
                    Utility.findUserSegment(result.Member);

                    // TODO: add analytics logger callback/promise pattern to guarantee
                    //       we don't navigate away from the page until all logging is complete
                    // increased to 1000 for lessons save on exit
                    setTimeout(function () {
                        if (SALT.LoginManager.repaymentNavigatorOverride) {
                            location.href = SALT.LoginManager.repaymentNavigatorOverride;
                            // Delete navigatorPath cookie if it exists
                            if ($.cookie('navigatorPath')) {
                                $.removeCookie('navigatorPath');
                            }
                        } else {
                            location.href = result.ReturnUrl;
                        }
                    }, 1000);
                } else {
                    $('.loading').hide();
                    if (loginForm.attr('id') === 'loginForm') {
                        $('#loginBtn span').text('Log in');
                    } else {
                        $('.loginBtn span').text('Log in');
                    }
                    errorCode.html(result.Message);
                }
            });

            return false;
        },
        findUserSegment: function (responseObj) {
            $.getJSON('/Assets/Scripts/js/salt/userSegment.json')
                .done(function (json) {
                    if (responseObj.Organizations.length < 1) {
                        console.log('you dont have any schools!');
                        document.cookie = 'UserSegment=Default; PATH=/';
                    } else {
                        for (var i = 0; i < responseObj.Organizations.length; i++) {
                            if (responseObj.PrimaryOrganizationKey === responseObj.Organizations[i].OrganizationId.toString()) {
                                var keyStr = responseObj.Organizations[i].Brand;
                                document.cookie = 'UserSegment=' + json[keyStr] + '; PATH=/';
                                console.log('UserSegment=' + json[keyStr] + '; PATH=/');
                                break;
                            }
                        }
                    }
                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ', ' + error;
                    console.log("JSON Request Failed: " + err);
                });

        },
        registration: function (event) {
            event.preventDefault();
            var url = this.href;
            if (SALT.LoginManager.returnURLOverride) {
                url += '?returnUrl=' + SALT.LoginManager.returnURLOverride;
            }

            location.href = url;
            return false;
        }

    };

    return SALT.LoginManager;
});
