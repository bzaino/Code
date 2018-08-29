var request = require('request'),
    saltUtils = require('./saltUtils'),
    dialogflow = require('dialogflow'),
    _ = require('../Assets/Scripts/js/libs/documentcloud/underscore'),
    chatbase = require('@google/chatbase')
            .setApiKey(process.env.ChatbaseKey);

var PRIMARY_FALLBACK = 'Default Fallback Intent';
var SECONDARY_FALLBACK = 'second-fallback';


exports.schoolInfo = function (context) {
    //Check for or initialize context.configuration object
    context.configuration = context.configuration || {};

    context.SiteMember.Organizations = saltUtils.sortOrganizations(context.SiteMember.Organizations);

    //Use for each loop to determine which of the objects in the Organization Array is the current organization.  We do this by comparing the OrganizationId property, to PrimaryOrganizationKey
    context.SiteMember.Organizations.forEach(function (obj, ind, arr) {
        if (obj.OrganizationId.toString() === context.SiteMember.PrimaryOrganizationKey) {
            //We are on the current school object, add relevant properties to Dust ContextBlock
            //TODO refactor this to pass whole school object across rather than just selected properties
            context.configuration.CurrentSchool = obj.OrganizationName;
            context.configuration.CurrentSchoolBrand = obj.Brand;
        }
    });
    return context;
};

exports.filterContent = function (collection, propertyID, filterValues) {
    //Check traversable collection object, such arrays, for obj[propertyID] having value equal anything
    //found in [space delimited] FilterValues string and removes it from the collection.
    //**** CAUTION propertyID is case sensitve.
    if (collection && propertyID && filterValues) {
        filterValues = filterValues.toLowerCase();
        collection = collection.filter(function (obj) {
            if (!obj[propertyID] || filterValues.indexOf(obj[propertyID].toLowerCase()) === -1) {
                //either not a valid property identifier or this is not one
                //of the values we want to filter out. Return object unchanged
                return obj;
            }
        });
    }
    return collection;
};

var fetchRetryCount = 0;

function sendXDomainRequest(options, successCallBack, timeOutCallBack) {
    request(options, function (error, response, body) {
        if (error && error.code === 'ETIMEDOUT') {
            console.log('Cross Domain API Call To:', options.url, 'Timed Out!');            
            //if timeout is within the first RequestRetryCount attempts, timeOutCallBack to retry
            if (fetchRetryCount < parseInt(process.env.RequestRetryCount, 10)) {
                timeOutCallBack(error);
            } else {
                //handle error gracefully.
                console.log('Timed Out! giveup for the moment To try again later!');
                successCallBack(null, '');
            }
            fetchRetryCount += 1;
        }
        else if (!error && response.statusCode === 200 &&
            response.headers['content-type'].indexOf('application/json') !== -1) {
            console.log('Cross Domain API Call To:', options.url, ' Succeeded!');
            //clean up data
            if (typeof body === 'string') {
                var escaped = body.replace(/^throw [^;]*;|^</, '');
                try {
                    body = JSON.parse(escaped);
                } catch (e) {
                    console.log('Cross Domain API Call To:', options.url, 'Returned malformed data:', body);
                    console.log('JSON parsing Exception:', e.message);
                    //handle error gracefully.
                    body = '';
                }
            }
            successCallBack(null, body);
        } else {
            //handle error gracefully.
            console.log('Cross Domain API Call To:', options.url, 'Error:', error);
            console.log('Error handled gracefully');
            successCallBack(null, '');
        }
    });
}

function determineProxyNeeded(options) {
    if (process.env.ProxyNeed === 'true') {
        var proxyURL = process.env.ProxyURL,
            urlHttpPart = process.env.ProxyURL.substring(0, proxyURL.indexOf('proxy')),
            urlPart = process.env.ProxyURL.substring(proxyURL.indexOf('proxy'));
        options.proxy = urlHttpPart + process.env.ProxyUser + ':' + process.env.ProxyPass + '@' + urlPart;
        options.tunnel = process.env.ProxyTunnel;
    }
    return options;
}

var answerData = {}, retryTimerId;
//Fetches answer data from unigo, retrying after a minute when we get a timeout error
function fetchAnswerData() {
    var arrayOfFuncs = [],
        options = {
            method: 'GET',
            headers: { 'content-type': 'application/json' },
            timeout: parseInt(process.env.ScholarshipsTimeout, 10),
            json: true,
            url: process.env.ScholarshipsURL + 'allchoices?auth=' + process.env.ScholarshipsAuthToken
        };

    options = determineProxyNeeded(options);

    function successCallBack(err, results) {
        //Fill or refresh the cache only if returned results has data
        if (results && results.length) {
            //Fill the cache with filtered and grouped choices from results
            answerData = saltUtils.filterAndGroupUnigoChoicesData(results);
        }
    }

    function timeOutCallBack(error) {
        console.log('This attempt to fetch Unigo answer data failed, waiting one minute and trying again');
        setTimeout(fetchAnswerData, 60000);
    }
    sendXDomainRequest(options, successCallBack, timeOutCallBack);
}

function isCachedAnswerData() {
    //if answerData is empty
    if ((Object.keys(answerData).length === 0 && answerData.constructor === Object)) {
        return false;
    }
    return true;
}

function retryFetchAnswerData(isFirstAttempt) {
    //reset fetchRetryCount
    fetchRetryCount = 0;
    //if answerData is empty
    if (!isCachedAnswerData()) {
        console.log('There is no cached Unigo answer data, trying to retrieve ...');
        if (isFirstAttempt) {
            console.log('retryFetchAnswerData in 5 mins from the application initialization');  
        } else {
            console.log('retryFetchAnswerData after 1 hour from last attempt');
        }
        //A note from NodeJS doc: When delay is larger than 2147483647 or less than 1, the delay will be set to 1
        //we want to try immediately, thus 1 millisecond
        setTimeout(fetchAnswerData, 1);
    } else {
        console.log('retryFetchAnswerData - answerData cached');
        //if there was previously setInterval timer but answerData is already cached
        if (retryTimerId) {
            console.log('retryFetchAnswerData - answerData cached cancelling setInterval for retryFetchAnswerData');
            //cancel subsequent attempts to retrieve answerData
            clearInterval(retryTimerId);
            //reset retryTimerId so that it won't try to clearInterval since it is already cancelled
            retryTimerId = null;
        }
    }
}

function buildSendChatbaseMsg(msgText, intentName, sessionId, isUserMsg) {

    var msg = chatbase.newMessage()
        .setUserId(sessionId)
        .setMessage(msgText)
        .setIntent(intentName)
        .setAsHandled();

    if (isUserMsg) {
        msg.setAsTypeUser();
    }
    else {
        msg.setAsTypeAgent();
    }

    if (intentName === PRIMARY_FALLBACK || intentName === SECONDARY_FALLBACK) {
        msg.setAsNotHandled();
    }

    msg.send()
        .then(function (msg) {
            console.log('Chatbase response: ');
            console.log(msg.getCreateResponse());
        })
        .catch(function (err) {
            console.error('Error communicating with chatbase: ');
            console.error(err);
        });
}

//Fetch the data when the app starts
fetchAnswerData();
//Wait 5 minutes when the app starts, and retry fetching the data if it failed at initialization
setTimeout(retryFetchAnswerData, 300000, true);
//retry every 1 hour until answerData is cached once
retryTimerId = setInterval(retryFetchAnswerData, 3600000, false);
//Refresh the data every 24 hours
setInterval(fetchAnswerData, parseInt(process.env.unigoAnswerFetchInterval, 10));

// retrieve scholarships answers
exports.retrieveAnswerValues = function (successCallBack) {
    successCallBack(answerData);
};
exports.logToChatbase = function (dialogflowResponse, sessionId, cookies) {
    if (cookies && cookies.indexOf('IUser=true') > -1) {
        chatbase.setPlatform('Internal');
    } else {
        chatbase.setPlatform('External');
    }
    // We've got a response from dialogflow.  Pass it onto chatbase analytics
    var intentName = dialogflowResponse.queryResult.intent.displayName,
        hopeText = '';

    //if we have FB replies, we need to get the options returned to the user. This variable will be used in checks further down
    var fbArray = dialogflowResponse.queryResult.fulfillmentMessages.filter(function (message) { 
        return message.platform === 'FACEBOOK'; 
    });

    //log response from Hope
    _.each(dialogflowResponse.queryResult.fulfillmentMessages, function (record) {
        //only write out the text from PLATFORM_UNSPECIFIED when there are no FB messages in the results. Otherwise we get dupes
        if (record.text && (record.platform === 'FACEBOOK' || (record.platform === 'PLATFORM_UNSPECIFIED' && fbArray.length === 0))) {
            hopeText += record.text.text + '|';    
        }
        else if (record.quickReplies && record.platform === 'FACEBOOK') {
            var replyArray =  record.quickReplies.quickReplies;
            hopeText += replyArray.join() + '|';     
        }   
    });

    //send agent msg to chatbase
    buildSendChatbaseMsg(hopeText, intentName, sessionId, false);

    //send user msg to chatbase
    buildSendChatbaseMsg(dialogflowResponse.queryResult.queryText, intentName, sessionId, true);

};

exports.queryDialogflow = function (req) {
    var projectId = req.body.projectId,
        sessionId = req.body.sessionId.toString(),
        languageCode = req.body.lang;

    // Instantiates a sessison client
    var sessionClient = new dialogflow.SessionsClient();

    // The path to identify the agent that owns the created intent.
    var sessionPath = sessionClient.sessionPath(projectId, sessionId);

    // Build the request payload that the dialgflow API expects.  Slightly different structure based on whether its an event or a query
    var request = {
        session: sessionPath,
        queryInput: {}
    };

    if (req.body.eventName) {
        request.queryInput = {
            event: {
                name: req.body.eventName,
                languageCode: languageCode
            }
        };
    } else if (req.body.queries) {
        request.queryInput = {
            text: {
                text: req.body.queries[0],
                languageCode: languageCode
            }
        };
    }

    return sessionClient.detectIntent(request);
};

exports.ClassifyUser = function (context) {
    var currentYear = new Date().getFullYear(),
        //classification variables
        enrollStat = context.EnrollmentStatus,
        gradYear = null,
        gradYearArray = [],
        //classification values
        LwrClass = 'UserClass-LowerClass',
        UprClass = 'UserClass-UpperClass',
        UprClsAluMix = 'UserClass-UpperClassAlumniMix',
        Alum = 'UserClass-Alumni',
        UnKwn = 'UserClass-Default',
        //Pre evaluations of expressions for reusage
        gradYearIsFalsy = true,
        enrollStatIsFalsy = (!enrollStat || (enrollStat === 'X' || enrollStat === 'Z' || enrollStat === 'N')),
        enrollStatIsFull = (enrollStat === 'F'),
        enrollStatIsHalf = (enrollStat === 'H' || enrollStat === 'L');

    //put all grad years in an array. Max grad year should be used
    if (context.Organizations) {
        for (var i = 0; i < context.Organizations.length; i++) {
            if (context.Organizations[i].ExpectedGraduationYear) {
                gradYearArray.push(context.Organizations[i].ExpectedGraduationYear);
            }
        }
        if (gradYearArray.length > 0) {
            gradYear = Math.max.apply(Math, gradYearArray);
        }
    }

    gradYearIsFalsy = (!gradYear || gradYear === 1900);

    //Default to Half time enrollment
    if (!gradYearIsFalsy && enrollStatIsFalsy) {
        enrollStat = 'H';
        enrollStatIsHalf = true;
    }

    /*** Lower Classmen classification - Start ***/
    if ((enrollStatIsFull && (gradYear >= (currentYear + 3))) ||
        (enrollStatIsHalf && (gradYear >= (currentYear + 6)))) {
        return LwrClass;
    }
    /*** Lower Classmen classification - End ***/

    /*** Upper Classmen classification - Start ***/
    if ((gradYearIsFalsy && !enrollStatIsFalsy) ||
        (enrollStatIsFull && (gradYear >= (currentYear) && gradYear <= (currentYear + 3))) ||
        (enrollStatIsHalf && (gradYear >= (currentYear) && gradYear <= (currentYear + 7)))) {
        return UprClass;
    }
    /*** Upper Classmen classification - End ***/

    /*** Alumni classification - Start ***/
    if ((enrollStatIsFull && !gradYearIsFalsy && (gradYear <= (currentYear - 4))) ||
        (enrollStatIsHalf && !gradYearIsFalsy && (gradYear <= (currentYear - 8)))) {
        return Alum;
    }

    //If we know that the graduation year is less than current year and
    // the enrollment status is null classify as Alumni
    if (((gradYear < currentYear) && enrollStatIsFalsy && !gradYearIsFalsy) ||
        ((gradYear <= currentYear - 10) && !gradYearIsFalsy && !enrollStatIsFalsy) ||
        ((enrollStat === 'G') || (enrollStat === 'W'))) {
        return Alum;
    }
    /*** Alumni classification - End ***/

    /*** Upper Classmen/Alumni Mix classification - Start ***/
    if ((enrollStatIsFull && (gradYear >= (currentYear - 7) && gradYear <= (currentYear - 1)) && !gradYearIsFalsy) ||
        (enrollStatIsHalf && (gradYear >= (currentYear - 11) && gradYear <= (currentYear - 1)) && !gradYearIsFalsy)) {
        return UprClsAluMix;
    }

    if (((gradYear >= currentYear - 10) && (gradYear <= currentYear) && enrollStatIsFalsy && !gradYearIsFalsy) ||
        (enrollStat === 'A')) {
        return UprClsAluMix;
    }
    /*** Upper Classmen/Alumni Mix classification - End ***/

    return UnKwn;
};
