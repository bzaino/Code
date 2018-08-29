/* jshint maxstatements: 28 */
var controller = require('./controller'),
    saltUtils = require('./saltUtils'),
    serverWidgets = require('./serverWidgets');

exports.glossary = function (req, res) {
    var str = req.params[2].toLowerCase();

    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathGenericEndeca + str.replace('.html', '');

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        //JSON paths are different than a normal detail page because a record spotlight was used rather than a product detail
        //Shim to normal path so that dust templates do not need to account for both
        if (context.content && !context.content.mainContent[0].record) {
            context.content.mainContent[0].record = context.content.mainContent[0].records[0];
        }

        context.content.secondaryContent[0].records.sort(function (a, b) {
            if (a.attributes.letter > b.attributes.letter) {
                return 1;
            }
            if (a.attributes.letter < b.attributes.letter) {
                return -1;
            }
            if (a.attributes.term > b.attributes.term) {
                return 1;
            }
            if (a.attributes.term < b.attributes.term) {
                return -1;
            }
            // a must be equal to b
            return 0;
        });
        res.render('Pages/glossary.dust', context);
    });
};

/**** DETAIL PAGE HANDLERS START ****/
exports.articleDetail = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathDetail + req.params[2];

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false,  function (context, results) {

        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
            if (context.content  && context.content.mainContent[0].record.attributes.RefToDoStatusID !== 2) {
                context.content.endOfIncompleteContent = true;
            }
        }
        context.PageContentType = 'Article';
        try {
            context.configuration = context.configuration || {};
            if (req.query.s) {
                context.configuration.isSpanish = true;
            }
            //Not logged in add blocked/unblocked state
            if (!context.SiteMember) {
                saltUtils.determinePageAccess(req, context);
            }
            res.render('Pages/CommonDetailPage.dust', context);

        } catch (e) {
            context.blockedContent = true;
            console.log(e.message);
            res.render('Pages/CommonDetailPage.dust', context);
        }
    });
};

exports.sharedDetailPage = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath;

    //Videos have a distinct sub page from the "media" endpoint in endeca.  We want requests for this content type to go to media/Video/PrimaryKey rather than media/PrimaryKey
    if (req.params[1] === 'Video') {
        endecaPath = process.env.pathDetail + req.params[1] + '/' + req.params[2];
    } else {
        endecaPath = process.env.pathDetail + req.params[2];
    }

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        } else {
            saltUtils.determinePageAccess(req, context);
        }
        context.PageContentType = req.params[1];
        //Comics actually use the same body template as infographics so we need to shim the value to 'Infographic'
        if (context.PageContentType === 'Comic') {
            context.PageContentType = 'Infographic';
        }
        //There is no dedicated ebookBody template, it uses the tool template, which provides the html field necessary to output the download buttons for the ebook
        if (context.PageContentType === 'eBook') {
            context.PageContentType = 'Tool';
        }

        if (context.SiteMember && context.SiteMember.DashboardEnabled && context.PageContentType === 'Infographic') {
            context.content.mainContent[0].record.attributes.RefToDoStatusID = 2;
            context.content.mainContent[0].record.attributes.RefToDoTypeID = 1;
            context.content.mainContent[0].record.attributes.ToDoComplete = true;
        }
        res.render('Pages/CommonDetailPage.dust', context);
    });
};

exports.toolDetail = function (req, res) {
    var bakedCookies = saltUtils.cookieBaker(req, res),
        serviceCalls = {},
        needsAuth = false,
        unauthServiceCalls = {},
        userSegment = bakedCookies.UserSegment ? bakedCookies.UserSegment : '',
        // make sure the "R" in "R-101" in always uppercase
        requestPath = req.params[2].indexOf('r-101') > -1 ? req.params[2].replace('r-101', 'R-101') : req.params[2],
        endecaPath = process.env.pathDetail + requestPath + '?Endeca_user_segments=' + userSegment;

    //this is KWYO, add call to SAL for loan check.
    if (req.path.indexOf('R-101-13584') > -1) {
        serviceCalls = {KWYOLoan : '/api/SelfReportedService/kwyoLoans'};
    }

    //SWD-8629: if tool is CCP, require user to be authorized to see it
    if (req.path.indexOf('college-cost-planner') > -1) {
        needsAuth = true;
        endecaPath = process.env.pathCCP + requestPath + '?Endeca_user_segments=' + userSegment;
    }

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, needsAuth, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);

            //this is KWYO, should we skip landing page? if user has kwyo loans, yes
            if (results.serviceCalls && results.serviceCalls.KWYOLoan) {
                if (results.serviceCalls.KWYOLoan > 0) {
                    res.redirect('/KnowWhatYouOwe/');
                }
            }
        } else {
            saltUtils.determinePageAccess(req, context);
        }
        context.PageContentType = 'Tool';
        res.render('Pages/CommonDetailPage.dust', context);
    });
};

exports.JSIEstimator = function (req, res) {
    var bakedCookies = saltUtils.cookieBaker(req, res),
        serviceCalls = {},
        unauthServiceCalls = {
            JSIQuizMajors : '/api/SurveyService/JSI/Majors',
            JSIQuizStates : '/api/SurveyService/JSI/States'
        },
        userSegment = bakedCookies.UserSegment ? bakedCookies.UserSegment : '',
        endecaPath = process.env.pathDetail + req.params[3] + '?Endeca_user_segments=' + userSegment;

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        context.PageContentType = 'Tool';
        if (results.unauthServiceCalls && results.unauthServiceCalls.JSIQuizMajors && results.unauthServiceCalls.JSIQuizStates) {
            if (context.SiteMember) {
                context = serverWidgets.schoolInfo(context);
            } else {
                saltUtils.determinePageAccess(req, context);
            }
            context.JSIQuizMajors = results.unauthServiceCalls.JSIQuizMajors;
            //remove unwanted states codes
            results.unauthServiceCalls.JSIQuizStates.JSIQuizList = serverWidgets.filterContent(results.unauthServiceCalls.JSIQuizStates.JSIQuizList, 'StateCd', 'PW MP MH FM AS');
            context.JSIQuizStates = results.unauthServiceCalls.JSIQuizStates;

            res.render('Pages/CommonDetailPage.dust', context);
        } else {
            saltUtils.determinePageAccess(req, context);
            res.render('Pages/CommonDetailPage.dust', context);
        }
    });
};

exports.calcXMLDetail = function (req, res) {

    var serviceCalls = {},
    unauthServiceCalls = {},
    endecaPath = process.env.pathGenericEndeca + 'iFrames/' + req.params[0];
    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            //JSON paths are different than a normal detail page because a record spotlight was used rather than a product detail
            //Shim to normal path so that dust templates do not need to account for both
            if (context.content && !context.content.mainContent[0].record) {
                context.content.mainContent[0].record = context.content.mainContent[0].records[0];
            }
            context = serverWidgets.schoolInfo(context);
        } else {
            saltUtils.determinePageAccess(req, context);
        }
        context.PageContentType = 'iFrame';
        res.render('Pages/CommonDetailPage.dust', context);
    });
};
exports.seniorEducationResource = function (req, res) {
    var bakedCookies = saltUtils.cookieBaker(req, res),
        serviceCalls = {},
        unauthServiceCalls = {
            states : '/api/SurveyService/JSI/States'
        },
        userSegment = bakedCookies.UserSegment ? bakedCookies.UserSegment : '',
        endecaPath = process.env.pathDetail + req.params[3] + '?Endeca_user_segments=' + userSegment;

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        context.PageContentType = 'Tool';
        if (results.unauthServiceCalls && results.unauthServiceCalls.states && context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
            //remove content with states codes
            results.unauthServiceCalls.states.JSIQuizList = serverWidgets.filterContent(results.unauthServiceCalls.states.JSIQuizList, 'StateCd', 'PW MP MH FM AS GU PR');
            context.JSISelectStates = results.unauthServiceCalls.states;
            res.render('Pages/CommonDetailPage.dust', context);
        } else {
            saltUtils.determinePageAccess(req, context);
            res.render('Pages/CommonDetailPage.dust', context);
        }
    });
};
/**** DETAIL PAGE HANDLERS END ****/

/**** SINGLETON PAGE HANDLERS START ****/

exports.home = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = '';
    if (req.path.indexOf('/index.html') > -1) {
        endecaPath = process.env.pathGenericEndeca + 'CombinedUnauthHome';
    } else {
        endecaPath = process.env.pathGenericEndeca + 'AuthHome';
    }

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            //User is logged in, but is trying to access index.html, re-route them to /home
            if (req.path === '/index.html' && !req.query.ReturnUrl) {
                var url = '/home';
                if (req._parsedUrl.search) {
                    url += req._parsedUrl.search;
                }
                res.redirect(url);
            // link to todo / browse when the user is already logged in is redirected to the right page   
            } else if (req.path === '/index.html') {
                var urlNew;
                if (req._parsedUrl.search) {
                    var UrlVal = req._parsedUrl.search.slice(req._parsedUrl.search.indexOf('ReturnUrl=') + 10);
                    var UrlValDec = decodeURIComponent(UrlVal);   
                    urlNew = UrlValDec;
                }
                res.redirect(urlNew);
            } else {
                //Classify user to one of segments
                var userClassification = serverWidgets.ClassifyUser(context.SiteMember);
                //set a user classification cookie to be picked up by client side js for appropriate content rendering
                res.cookie('UserClass', userClassification, { Expires: 'session', httpOnly: false, secure: true });
                context = serverWidgets.schoolInfo(context);
                //Set a property if we are trying to serve the onboarding page for the "home" dust template to use
                if (req.query.onboarding) {
                    context.isOnboarding = true;
                }
                res.render('Pages/home.dust', context);
            }
        } else {
            res.cookie('UserClass', 'UserClass-Default', { Expires: 'session', httpOnly: false, secure: true });
            if (req.path === '/home' || req.path === '/home/') {
                res.render('Pages/home.dust', context);
            } else {
                if (!context.content) {
                    //Endeca returned an error or never responded, we don't have the data to build the homepage html, redirect to the error page
                    return res.redirect('/errorPage.html');
                }
                //render home as usual
                var persona = req.query.Type;
                if (persona) {
                    context.content.persona = persona;
                } else {
                    context.content.persona = 'Home';
                }
                res.render('Pages/newhome.dust', context);
            }
        }
    });
};


exports.repaymentNavigator = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathGenericEndeca + 'VLC/Menu';

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, true, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        res.render('VLC.dust', context);
    });
};

exports.knowWhatYouOwe = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathGenericEndeca + 'KWYO';

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, true, function (context, results) {
        res.render('KWYO/KnowWhatYouOwe.dust', context);
    });
};

exports.registration = function (req, res) {
    var oe = req.query.oe ? req.query.oe : req.query.Oe,
        br = req.query.br ? req.query.br : req.query.Br,
        token = req.query.t ? req.query.t : req.query.T,
        extOrgId = req.query.orgid ? req.query.orgid : '',
        serviceCalls = {},
        unauthServiceCalls = ((oe && br) || extOrgId) ?
            { orgData: process.env.MemberService + '/PreRegistration?oe=' + oe + '&br=' + br + '&extOrgId=' + extOrgId} : {},
        endecaPath = process.env.pathStandAlone + 'SchoolActivation';
    if (token) {
        //set expiration date to 30 days. SWD-7617
        res.cookie('ActivationToken', token, { httpOnly: false, expires: new Date(Date.now() + 2592000000) });
    }

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        context.isRegistrationPage = true;
        if (unauthServiceCalls.orgData && results && results.unauthServiceCalls.orgData) {
            context.configuration = context.configuration || {};
            context.configuration.CurrentSchool = results.unauthServiceCalls.orgData.OrgName;
            context.configuration.CurrentSchoolBrand = results.unauthServiceCalls.orgData.OrgLogo;
            context.configuration.IsSchool = results.unauthServiceCalls.orgData.IsSchool;
            context.configuration.OECode = oe ? oe : results.unauthServiceCalls.orgData.OeCode;
            context.configuration.OEBranch = br ? br : results.unauthServiceCalls.orgData.OeBranch;
            context.configuration.ExtOrgId = extOrgId ? extOrgId : results.unauthServiceCalls.orgData.ExtOrgId;
            context.configuration.OrgId = results.unauthServiceCalls.orgData.OrgId;
        }

        //for meta description population, because in WebtrendsTags.dust, it's using detail page structure, so modify the context to fit the structure.
        context.content.mainContent = [{record: {attributes: {page_description: context.content.metaDescription}}}];
        res.render('Pages/registration.dust', context);
    });
};

exports.manageprofile = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathGenericEndeca + 'profile';
    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, true, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        res.render('Profile/manageprofile.dust', context);
    });
};

exports.specialevent = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathSepecialEvent + req.params[0].toLowerCase();

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        if (context.content && context.content.metaKeywords === 'sidebarReg') {
            context.sidebarReg = true;
        }
        res.render('Pages/specialevent.dust', context);
    });
};

exports.customlanding = function (req, res) {
    var bakedCookies = saltUtils.cookieBaker(req, res),
        serviceCalls = {},
        unauthServiceCalls = {},
        userSegment = bakedCookies.UserSegment ? bakedCookies.UserSegment : '',
        endecaPath = process.env.pathCustomLanding + req.params[0] + '?Endeca_user_segments=' + userSegment;

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        //for meta description population, because in WebtrendsTags.dust, it's using detail page structure, so modify the context to fit the structure.
        context.content.mainContent = [{record: {attributes: {page_description: context.content.metaDescription}}}];
        res.render('Pages/customlanding.dust', context);
    });
};

exports.press = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathPress;

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        res.render('Pages/press.dust', context);
    });
};

exports.search = function (req, res) {
    var endecaPath = process.env.pathGenericEndeca + 'SearchResults';
    controller.handler(req, res, endecaPath, {}, {}, false, function (context, results) {
        res.render('Pages/searchresults.dust', context);
    });
};

exports.resetpassword = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {
            resetPasswordMemberModel: '/api/ASAMemberService/ForgotPassword?token=' + req.query.token
        },

        endecaPath = process.env.pathGenericEndeca + 'CombinedUnauthHome';

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (results.unauthServiceCalls && results.unauthServiceCalls.resetPasswordMemberModel) {
            context.configuration = {};
            if (results.unauthServiceCalls.resetPasswordMemberModel.ErrorList.length === 0) {
                context.resetPasswordMemberModel = results.unauthServiceCalls.resetPasswordMemberModel;
                context.token = req.query.token;
                context.configuration.resetPassword = true;
            } else {
                if (results.unauthServiceCalls.resetPasswordMemberModel.ErrorList[0].DetailMessage === 'Ticket Expired') {
                    // The ticket expired - allow the member to generate another ticket to change password
                    context.configuration.resetExpired = true;
                }
            }
        }
        context.content.persona = 'Home';

        res.render('Pages/newhome.dust', context);
    });
};

exports.root = function (req, res) {
    var str = req.params[2].toLowerCase();

    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathGenericEndeca + str.replace('.html', '');

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        res.render('Pages/miscellaneous.dust', context);
    });
};
exports.contact = function (req, res) {
    var str = req.params[2].toLowerCase();
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathGenericEndeca + str.replace('.html', '');

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        res.render('Pages/contact.dust', context);
    });
};

exports.schoollogo = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = '';
    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        if (context.SiteMember) {
            context = serverWidgets.schoolInfo(context);
        }
        res.render('SchoolLogo.dust', context);
    });
};

exports.whyusesalt = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathStandAlone + 'WhyUseSALT';

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        res.render('Pages/whyusesalt.dust', context);
    });
};

exports.logon = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathSortPage + 'logon';

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        context.hideLoginOverlay = true;
        context.isRegistrationPage = true;
        res.render('Pages/logOnPage.dust', context);
    });
};

exports.retrieveAnswerValues = function (req, res) {
    try {
        serverWidgets.retrieveAnswerValues(function (results) {
            //in case object is undefined or null, end the response with no data.
            if (results) {
                res.setHeader('Content-Type', 'application/json');
                res.send(JSON.stringify(results));
            } else {
                res.end();
            }
        });
    } catch (e) {
        console.log(e.message);
        res.redirect('/errorPage.html');
    }
};

exports.detectIntent = function (req, res) {
    if (req.body.eventName || req.body.queries) {
        console.log("request = " + JSON.stringify(req.body));
        serverWidgets.queryDialogflow(req)
            .then(function (responses) {
                if (responses[0]) {
                    serverWidgets.logToChatbase(responses[0], req.body.sessionId.toString(), req.headers.cookie);
                    res.json(responses[0]);
                } else {
                    console.error('No dialogflow error, but no response object.');
                    res.status(500).end();
                }
            })
            .catch(function (err) {
                console.error(err);
                res.status(500).end();
            });
    } else {
        // Request payload is missing proper params
        console.error('Received request with bad params.  No eventName or queries present.  Request body: ');
        console.error(req.body);
        res.status(400).end();
    }
};

exports.blog = function (req, res) {
    var serviceCalls = {},
        unauthServiceCalls = {},
        endecaPath = process.env.pathGenericEndeca + 'blog?Ns=sys_contentpostdate|1&Ntk=Tags&Ntx=mode+matchany&Ntt=&Dims=305&No=0';

    controller.handler(req, res, endecaPath, serviceCalls, unauthServiceCalls, false, function (context, results) {
        res.render('Pages/blog.dust', context);
    });
};
/**** SINGLETON PAGE HANDLERS END ****/
