/* jshint maxstatements: 46, maxdepth: 6 */
var async = require('async');
var saltUtils = require('./saltUtils');
var goalQuestionIDs = [17, 18, 19, 20];

exports.handler = function (req, res, endecaPath, salCalls, unauthSalCalls, needsAuthorization, cbk) {
    var context = {},
    options = saltUtils.requestBuilder(req),
        bakedCookies = saltUtils.cookieBaker(req, res),
        constructedServiceCalls = saltUtils.generateAsyncObj(options, salCalls),
        constructedUnauthServiceCalls = saltUtils.generateAsyncObjUnauth(options, unauthSalCalls);
    if (process.env.ProdIntercept === 'true') {
        context.ProdIntercept = true;
    }
    //See https://github.com/caolan/async#parallel for usage of async.parallel
    //The last function will be a success callback that waits until all of the functions in the async.parallel have returned to fire
    async.parallel({
        endeca: function (callback) {
            if (endecaPath) {
                //This function runs the http call to the SAL to get Endeca data at the path designated
                saltUtils.makeServiceCall(callback, {
                    uri: options.hostAndPort + endecaPath,
                    jar: false,
                    headers: {
                        'Cookie': options.cookies,
                        'Content-Type': 'application/json'
                    }
                });
            } else {
                callback(null, null);
            }
        },
        serviceCalls: function (callback) {
            if (bakedCookies.IndividualId && bakedCookies.MemberId) {
                //IndividualId cookie was found, so we are logged in, make the service call(s) if any from the ones passed in.
                //They will be run in parallel using another async.parallel
                context.individualIDCookie = true;
                async.parallel(constructedServiceCalls,

                function (err, results) {
                    //All logged-in dependent service calls have returned, signal done and pass in results
                    callback(null, results);
                });
            } else {
                callback(null, null);
            }
        },
        unauthServiceCalls: function (callback) {
            if (Object.getOwnPropertyNames(unauthSalCalls).length > 0) {
                async.parallel(constructedUnauthServiceCalls,

                function (err, results) {
                    //All Unauthenticated service calls have returned, signal done and pass in results
                    callback(null, results);
                });
            } else {
                //No calls to be made, signal this function is done and pass null
                callback(null, null);
            }
        }
    }, function (err, results) {
        //Results will be an object, of arguments passed in by the parallel functions, with the keys being the named functions above.
        //As such, the objext will look like {endeca: {}, serviceCalls : {}}
        //Add endeca and or member data to the context objext and call the callback to signal you are done.
        //Pass results obj as well as context obj to callback so that individual page handlers have access to service call data
        context.content = results.endeca;
        
        // we currently use this for Facebook pixel to see if we should render the tracking script
        context.isProd = process.env.Environment === 'Prod';        

        //If there is an Endeca error in the content object or mainContent object, don't try to execute anything else, redirect to the error page.
        if (context.content && context.content['@error']) {
            console.error('Endeca content returned an error:\n' + context.content['@error']);
            return res.redirect('/errorPage.html');
        }
        else if (context.content && context.content.mainContent && context.content.mainContent[0] && context.content.mainContent[0]['@error']) {
            console.error('Endeca mainContent returned an error:\n' + context.content.mainContent[0]['@error']);
            return res.redirect('/errorPage.html');
        }

        //Check that the serviceCalls object exists, and that has a non-falsy 'member' property
        //before trying to add authenticated data to the context object
        if (results.serviceCalls) {
            if (results.serviceCalls.member) {
                context.configuration = context.configuration || {};
                //We got back authenticated data, add it to the context object
                context.SiteMember = results.serviceCalls.member;
                context.SiteMember.ShowMM101 = false;
                context.SiteMember.ShowAdminPortal = false;

                if (context.SiteMember.DashboardEnabled) {
                    //Set a flag on the data denoting whether the member has activated "Browse" and "Search" products
                    context.SiteMember.BrowseDisabled = !saltUtils.isProductActive(context, 12);
                    context.SiteMember.SearchDisabled = !saltUtils.isProductActive(context, 13);

                    //Some of our client side features need to know how many goals are enabled
                    //Calculate that, and set the result as a property on SiteMember
                    var enabledGoals = {};
                    var enabledGoalCount = 0;
                    enabledGoals.RepayStudentDebt = saltUtils.isProductActive(context, 8);
                    enabledGoals.FindAJob = saltUtils.isProductActive(context, 9);
                    enabledGoals.PayForSchool = saltUtils.isProductActive(context, 10);
                    enabledGoals.MasterMoney = saltUtils.isProductActive(context, 11);

                    for (var property in enabledGoals) {
                        if (enabledGoals.hasOwnProperty(property) && enabledGoals[property] === true) {
                            enabledGoalCount++;
                        }
                    }

                    context.SiteMember.enabledGoalCount = enabledGoalCount;

                    //Filter the responses down to answers to the goal ranking questions
                    var goalRankResponses = [],
                    responses = context.SiteMember.ProfileQAs[context.SiteMember.ProfileQAs.length - 1].Responses;
                    for (var i = 0; i < responses.length; i++) {
                        if (goalQuestionIDs.indexOf(responses[i].QuestionExternalId) > -1) {
                            //Populate the nameWithNoSpaces property before pushing the response onto the goalRankResponses
                            responses[i].nameWithNoSpaces = responses[i].AnsName.replace(/[\W_]+/g, '');
                            goalRankResponses.push(responses[i]);
                        }
                    }

                    context.goalRankResponses = goalRankResponses;
                }

                var mm101ProductID = parseInt(process.env.myMoney101ProductID, 10);
                var mm101bool = saltUtils.isProductActive(context, mm101ProductID);
                if (mm101bool) {
                    context.configuration.mm101B = process.env.myMoney101UrlNew;
                    context.SiteMember.ShowMM101 = true;
                }

                var adminPortalbool = saltUtils.isRoleActive(context, process.env.adminPortalRoleID);
                if (adminPortalbool) {
                    context.configuration.adminPortalUrl = process.env.adminPortalUrl;
                    context.SiteMember.ShowAdminPortal = true;
                }
            }
        } else {
            if (needsAuthorization) {
                //Service call data came back empty (unauth), and this page needs authorization
                //Redirect to homepage with proper ReturnUrl
                //Return when calling redirect so that we dont continue to execute this handler now that we know we are sending a 302
                return res.redirect('/index.html?ReturnUrl=' + encodeURIComponent(req.path));
            }
        }
        cbk(context, results);
    });
};
