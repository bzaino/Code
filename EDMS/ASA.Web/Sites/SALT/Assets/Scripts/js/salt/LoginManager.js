/* jshint maxstatements: 32 */
define([
    'jquery',
    'salt',
    'asa/ASAUtilities',
    'salt/models/SiteMember',
    'modules/ReportingStatus',
    'configuration',
    'underscore',
    'modules/ASALocalStore',
    'jquery.cookie'
], function ($, SALT, Utility, SiteMember, ReportingStatus, Configuration, _, asaLocalStore) {
    SALT.LoginManager = {
        isInitialized: false,
        logonForm: null,
        errorField: null,
        returnURLOverride: null,
        init: function (config) {

            this.logonForm = $(config.logonFormName);
            this.errorField = $(config.errorFieldName);

            this.returnURLOverride = config.returnURLOverride || this.returnURLOverride;
            //This init function can get called many times.  Make sure we aren't firing the login start webtrends repeatedly
            if (!this.isInitialized) {
                //Use "one" because the login form can be opened and closed multiple times, we dont want to fire multiple events for this
                $('#UserName').one('focus', function () {
                    SALT.publish('LoginManager:Start');
                });
                this.isInitialized = true;
            }

            $('#loginForm, #loginSideForm').on('valid', function () {
                SALT.LoginManager.authenticate();
            });

            SiteMember.done(function (siteMember) {
                if (siteMember.IsAuthenticated === 'true') {
                    if (SiteMember.get('PrimaryEmailKey')) {
                        $('input[name="UserName"]').val(SiteMember.get('PrimaryEmailKey'));
                        $('input[name="Password"]').focus();
                    }
                } else if ($.cookie('RememberMe')) {
                    $('input[name="UserName"]').val($.cookie('RememberMeEmail'));
                    $('input[name="Password"]').focus();
                }
            });

        },
        removeTourFromUrl: function () {
            //remove tour from URL so SALT tour video will not replay when logging in
            var locationSearch = Utility.getLocationSearch();
            locationSearch = locationSearch.replace('tour=true', '');
            locationSearch = locationSearch.replace('?&', '?');
            if (locationSearch === '?') {
                locationSearch = '';
            }
            return locationSearch;
        },
        authenticate: function () {
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

            if (location.pathname === '/register/') {
                returnURLstring = '&returnUrl=' + location.host;
            } else if (location.pathname.toLowerCase() === '/home/newpassword') {
                returnURLstring = '&returnUrl=/home';
                // SWD-1663 -- Sitewide content  search. If a user logs in from their search results ("searchCriteria"),
                // return them to the same location and query string they searched for. -khennings
            } else if (location.href.indexOf('searchCriteria') > 0) {
                returnURLstring = '&returnUrl=' + location.pathname + location.search;
            } else if (location.pathname.indexOf('index.html') > -1 || location.href.indexOf('Type=') > -1) {
                //remove tour from URL so SALT tour video will not replay when logging in
                var updatedURL = SALT.LoginManager.removeTourFromUrl();
                if (updatedURL.indexOf('?ReturnUrl=/') === 0) {
                    returnURLstring = '&returnUrl=/' + updatedURL.substr(12, updatedURL.length);
                } else {
                    returnURLstring = '&returnUrl=/home' + updatedURL;
                }
            }

            if (rememberMeVal.checkRememberMe.checked && $.cookie('RememberMe') && $.cookie('RememberMeUrl')) {
                returnURLstring = '&returnUrl=' + encodeURIComponent($.cookie('RememberMeUrl'));
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
                    //If the user is dashboard enabled, set a cookie that is used by the front-end to render the new todo-tile style
                    if (result.Member.DashboardEnabled) {
                        document.cookie = 'useTodoTileDesign = true; path=/';
                    }
                    var profileQandAresponses = _.last(result.Member.ProfileQAs).Responses;
                    _.each(profileQandAresponses, function (response) {
                        //Set specific cookie for the TopRankedGoal response, otherwise set using Question and Answer external ids
                        if (response.QuestionExternalId === 17) {
                            document.cookie = 'TopGoal=' + 'Dashboard-TopGoal-' + response.AnsName + '; PATH=/';
                        } else {
                            document.cookie = 'ans-' + response.QuestionExternalId + '-' + response.AnsExternalId + '=' + 'Dashboard-QA-' + response.QuestionExternalId + '-' + response.AnsName + '; PATH=/';
                        }
                    });
                    SiteMember.set(_.extend(result.Member, {IsAuthenticated: true}));
                    SALT.publish('siteMember:signin:complete', {
                        location: {
                            pathname: '/SignIn.html'
                        },
                        document: {
                            title: 'SignIn'
                        },
                        siteMember: result.Member
                    });

                    Utility.handleRegWallWT('Wall_Login_Complete');
                    $.removeCookie('regWall');

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
                        document.cookie = 'RememberMe=true; PATH=/; Domain=saltmoney.org; Expires=' + exdate.toString() + ';';
                        document.cookie = 'RememberMeEmail=' + result.Member.PrimaryEmailKey + '; PATH=/; Domain=saltmoney.org; Expires=' + exdate.toString() + ';';
                        //webtrends call for Rememberme // orig call //dcsMultiTrack('WT.z_actname', 'Remember Me', 'WT.z_acttype', 'save', 'WT.z_acttx', '1');
                        SALT.publish('LoginManager:RememberMe');
                    }

                    //remove any previously cached courses data
                    if (asaLocalStore.getLocalStorage('coursesCompletion')) {
                        asaLocalStore.removeLocalStorage('coursesCompletion');
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
                            var locName = Utility.getParameterByName('ReturnUrl'),
                            validLink = ['/home#fndtn-MySALT', '/home#fndtn-Todos', '/home#fndtn-Library'];
                            //directing the pages with dashboard link to the correct pages. 
                            if (validLink.indexOf(locName) !== -1) {
                                location.href = location.origin + locName;
                            } else if (Utility.getParameterByNameFromString('ReturnUrl', result.ReturnUrl)) {
                                location.href = Utility.getParameterByNameFromString('ReturnUrl', result.ReturnUrl);
                            } else if (Utility.getHostname(document.referrer) === Utility.getHostname(Configuration.mm101.url)) {
                                /*SWD-9522, user came for authentication, re route back while triggering SSO */
                                location.href = Configuration.mm101.url;
                            } else if (SALT.LoginManager.returnURLOverride && Utility.IsCurrentPage('/logon')) {
                                /* SWD-7217: fix index routing to home page with url querystring*/
                                location.href = SALT.LoginManager.returnURLOverride.replace('index.html', 'home');
                            } else {
                                location.href = result.ReturnUrl;
                            }
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
        }
    };

    return SALT.LoginManager;
});
