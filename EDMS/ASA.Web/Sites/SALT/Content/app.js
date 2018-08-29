var express = require('express'),
    app = express(),
    http = require('http'),
    listeningPort = process.env.PORT || 8000;

/** Pull in SALT custom Dust Helpers **/
require('./salt-dust-helpers');

var onError = function (res, msg) {
    console.log(msg);
    // redirect to generic error page
    if (res) {
        res.redirect('/errorPage.html');
        res.end();
    }
};

/** Configurations such as setting Dust as templating language and other middleware  **/
var config = require('./config')(app);

app.disable('x-powered-by');

/** Load Routes **/
var routes = require('./routes');

app.get(/^\/Content\/(media)\/(tool)\/(grad-degree-potential-salary-estimator)\/([-&\'\/\?\[\]_a-zA-Z0-9]*)/i, routes.JSIEstimator);
app.get(/^\/Content\/(media)\/(tool)\/(senior-education-finance-resource)\/([-&\'\/\?\[\]_a-zA-Z0-9]*)/i, routes.seniorEducationResource);
/*ENDECA CONTENT DETAIL PAGES */
app.get(/^\/Content\/(media)\/(article|form)\/([-&\'\/\?\[\]_a-zA-Z0-9]*)/i, routes.articleDetail);
app.get(/^\/Content\/(media)\/(lesson|infographic|comic|video|ebook|blog)\/([-&\'\/\?\[\]_a-zA-Z0-9]*)/i, routes.sharedDetailPage);
app.get(/^\/Content\/(media)\/(tool)\/([-&\'\/\?\[\]_a-zA-Z0-9]*)/i, routes.toolDetail);
app.get(/^\/calculators\/([-&\'\/\?\[\]_a-zA-Z0-9]*)/i, routes.calcXMLDetail);

/*ENDECA CONTENT SPECIAL EVENT PAGES*/
app.get(/^\/Landing\/([-&\'\/\?\[\]_a-zA-Z0-9]*)/i, routes.specialevent);

/*ENDECA CUSTOM LANDING*/
app.get(/^\/(courses)/i, routes.customlanding);

app.get('/home', routes.home);
app.get('/blog', routes.blog);
/* SINGLETON PAGES */
app.get(/^\/About\/(press)/i, routes.press);
app.get(/^\/(index\.html)/i, routes.home);
app.get(/^\/Content\/(search)/i, routes.search);
app.get(/^\/(manageprofile(\/(index\.html)?)?$)/i, routes.manageprofile);
app.get(/^\/(logon(\/(index\.html)?)?$)/i, routes.logon);
app.get(/^\/(register(\/(index\.html)?)?$)/i, routes.registration);
app.get(/^\/home\/NewPassword/i, routes.resetpassword);
app.get(/^\/(home(\/(about\.html)?)?$)/i, routes.root);
app.get(/^\/(home(\/(terms\.html)?)?$)/i, routes.root);
app.get(/^\/(home(\/(privacy\.html)?)?$)/i, routes.root);
app.get(/^\/(home(\/(contact\.html)?)?$)/i, routes.contact);
app.get(/^\/(home(\/(system_requirements\.html)?)?$)/i, routes.root);
app.get(/^\/(home(\/(glossary\.html)?)?$)/i, routes.glossary);
app.get(/^\/(schoollogo\.html)/i, routes.schoollogo); /*Used for SALTCourses Cobranding*/
app.get('/Navigator*', routes.repaymentNavigator);
app.get('/KnowWhatYouOwe/*', routes.knowWhatYouOwe);
app.get('/WhyUseSalt', routes.whyusesalt);
app.get(/^\/Scholarships\/answers/i, routes.retrieveAnswerValues);

app.post('/Dialogflow/intents', routes.detectIntent);

/* CATCH ALL ROUTE, IF NOT ONE OF THE ABOVE, SEND TO GENERIC ERROR PAGE */
app.get('*', function (req, res) {
    var msg = 'unrecognized endpoint: ';
    if (req) {
        msg = msg + req.originalUrl;
    }
    onError(res, msg);
});

app.listen(listeningPort);
//console.log('Listening on port ' + listeningPort + '\n\n');
