var request = require('request');
var config = require('./config');

exports.determinePageAccess = function (req, context) {
    //Check the CM1 field 'isUnblocked' to determine if content is blocked or not
    //Also check querystring for 'unblocked' param
    if (context.content && !context.content.mainContent[0].record) {
        context.content.mainContent[0].record = context.content.mainContent[0].records[0];
    }

    if (context.content && context.content.mainContent[0].record && (context.content.mainContent[0].record.attributes.isUnblocked[0].trim() || req.query.unblocked)) {
        context.unblocked = true;
    } else {
        context.blockedContent = true;
    }
};

exports.cookieBaker = function (req, res) {
    //Slices req.headers.cookie and returns POJsO object with the cookie keys as keys of the object.
    var cookies = {};
    if (req.headers.cookie) {
        req.headers.cookie.split(';').forEach(function (cookie) {
            var parts = cookie.split('=');
            cookies[parts[0].trim()] = (parts[1] || '').trim();
        });
    }
    return cookies;
};

exports.requestBuilder = function (req) {
    return {
        hostAndPort: 'https://' + (req.header('host') ? req.header('host').split(':', 1)[0] : 'localhost') + (process.env.portSAL ? ':' + process.env.portSAL : ''),
        cookies: req.headers.cookie
    };
};

exports.getYearFromJsonDate = function (input) {
    var convertedYear = input;

    if (input && typeof input === 'string' && input.indexOf('\/Date') !== -1) //if input is a JSON formatted Date string
    {
        var substringedDate = input.substring(6); //substringedDate= 1291548407008)/
        var parsedIntDate = parseInt(substringedDate, 10); //parsedIntDate= 1291548407008
        var dt = new Date(parsedIntDate); // parsedIntDate passed to date constructor

        convertedYear = dt.getFullYear();
    }

    return convertedYear;
};

var makeServiceCall = function (callback, options) {
    //Makes GET request using the options object.
    //callback object is passed in and will be called when GET returns with data
    //Uses request wrapper plugin, see reference at https://github.com/mikeal/request
    if (!options.rejectUnauthorized) {
        options.rejectUnauthorized = false;
    }

    request.get(options, function (err, response, body) {
        if (err || response.headers['content-type'].indexOf('application/json') === -1) {
            console.log('error making the request to: ' + options.uri);
            console.log('error body:', body);
            return callback(null, null);
        }
        //Parse the returned JSON into a JS object
        var dataReturned = JSON.parse(body);

        // TODO: Remove the if statement below after resolving SWD-7090
        // Temporary code for PROD issue SWD-7090, until the root cause is discovered with the additional logging
        // if the request was to get Endeca data, and it came back with an error, retry once.
        if (dataReturned.mainContent && dataReturned.mainContent[0] && dataReturned.mainContent[0]['@error'] && !options.temporaryRetryAttempted) {
            console.log('Endeca data error making the request to: ' + options.uri);
            console.log('Retrying once');
            options.temporaryRetryAttempted = true;
            makeServiceCall(callback, options);
        } else {
            if (dataReturned.Authorization) {
                //We got a non-authorized response, pass back null rather than the dataReturned
                callback(null, null);
            } else {
                //We got back data from the server, pass it along to the callback
                callback(null, dataReturned);
            }
        }
    });
};

exports.makeServiceCall = makeServiceCall;

var generateCalls = function (options, callsToMake, asyncObj) {

    //This will iterate over the properties in the callsToMake obj and add them to the asyncObj by named key
    Object.getOwnPropertyNames(callsToMake).forEach(function (callName, idx, array) {
        asyncObj[callName] = function (callback) {
            makeServiceCall(callback, {
                uri: options.hostAndPort + callsToMake[callName],
                headers: {
                    'Cookie': options.cookies,
                    'Content-Type': 'application/json'
                },
                jar: false
            });
        };
    });

    return asyncObj;
};

exports.generateCalls = generateCalls;

exports.generateAsyncObj = function (options, callsToMake) {

    //All authenticated pages are currently making calls to individual
    //We push this call into the asyncArray object to start, and add any that were passed in with the forEach
    var asyncObj = {
        member: function (callback) {
            makeServiceCall(callback, {
                uri: options.hostAndPort + process.env.pathGetMember,
                headers: {
                    'Cookie': options.cookies,
                    'Content-Type': 'application/json'
                },
                jar: false
            });
        }
    };

    //add calls that were passed in with the forEach
    asyncObj = generateCalls(options, callsToMake, asyncObj);

    return asyncObj;
};

exports.generateAsyncObjUnauth = function (options, callsToMake) {
    //add calls that were passed in with the forEach
    var asyncObjUnauth = {};
    asyncObjUnauth = generateCalls(options, callsToMake, asyncObjUnauth);

    return asyncObjUnauth;
};

exports.removeParamsFromQueryString = function (url, paramsArray) {
    paramsArray.forEach(function (param) {
        var regex = new RegExp('\\?' + param + '=[^&]*&?', 'gi');
        url = url.replace(regex, '?');
        regex = new RegExp('\\&' + param + '=[^&]*&?', 'gi');
        url = url.replace(regex, '&');
        url = url.replace(/(\?|&)$/, '');
    });
    return url;
};

exports.isProductActive = function (context, productID) {
    var isProductActive = false;
    if (context.SiteMember.OrganizationProducts) {
        context.SiteMember.OrganizationProducts.forEach(function (obj) {
            //When we are on the product that we are looking for return the "active status"
            if (obj.ProductID === productID) {
                isProductActive = obj.IsOrgProductActive;
            }
        });
    }
    return isProductActive;
};

exports.isRoleActive = function (context, roleId) {
    var isRoleActive = false;
    if (context.SiteMember.Roles) {
        context.SiteMember.Roles.forEach(function (obj) {
            //When we are on the role that we are looking for return the "active status"
            if (obj.RoleId === roleId) {
                isRoleActive = obj.IsMemberRoleActive;
            }
        });
    }
    return isRoleActive;
};

// If you update this, you should also update the function of the same name in ASAUtilities.js
exports.sortOrganizations = function (organizationsIn) {

    if (organizationsIn && (typeof organizationsIn === 'object')) {

        var sortLogoOrgs = organizationsIn.sort(function (a, b) {
            return b.EffectiveStartDate.localeCompare(a.EffectiveStartDate);
        });
        return sortLogoOrgs;
    }
    else {
        return organizationsIn;
    }
};

exports.intersect = function (a, b) {
    var temp;
    // Make sure we loop over the shorter array so that we do not try to access an out of bounds index
    if (b.length > a.length) {
        temp = b;
        b = a;
        a = temp;
    }
    //Returns an array with all intersecting elements of the two arrays
    return a.filter(function (e) {
        if (b.indexOf(e) !== -1) {
            return true;
        }
    });
};

exports.isDetailPage = function (path) {
    if (path.indexOf('/content/media') === 0 || path.indexOf('/calculators') === 0) {
        return true;
    }
    return false;
};

exports.filterAndGroupUnigoChoicesData = function (answerData) {
    if (answerData && (typeof answerData === 'object')) {
        //filter answerData to only first 47 questions' choices
        var filteredAnswerData = answerData.filter(function (el) {
            return el.QuestionID < 48;
        });
        var groupedAnswerData = { };
        if (filteredAnswerData && filteredAnswerData.length) {
            //group each q's choices together, to fit with the render
            //function's expected format for the dropdowns initializations
            filteredAnswerData.forEach(function (item) {
                var list = groupedAnswerData[item.QuestionID];
                if (list) {
                    list.push(item);
                } else {
                    groupedAnswerData[item.QuestionID] = [item];
                }
            });
        }
        return groupedAnswerData;
    }
    else {
        return answerData;
    }
};
