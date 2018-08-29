var assert = require('assert');
//var SaltRoutes = require('./../routes.js');
var express = require('express');
var app = express();

/**** Start testing funtion ****/
describe('creating routes calls', function () {
    describe('#articleDetail', function () {
        it('creating mock uri call string and executing articleDetail call', function () {
            //var ArticleCall = SaltRoutes.articleTest();
            app.set('req', 'https://localhost/content/media/Article/choosing-a-career-may-be-easier-than-you-think/_/R-101-2303');
            app.get('req', function (req, res) {
                //assert.ok(SaltRoutes.articleDetail);
                //assert.ok(typeof SaltRoutes.articleTest === 'function');
            });
        });
    });
});
