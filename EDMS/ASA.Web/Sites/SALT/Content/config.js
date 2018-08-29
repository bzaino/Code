var cons = require('consolidate'),
    path = require('path'),
    express = require('express');

module.exports = function (app) {

    var template_engine = 'dust';

    app.configure(function () {
        app.engine('dust', cons.dust);
        app.set('template_engine', template_engine);
        app.set('views', path.join(__dirname, '../Assets/templates'));
        app.set('view engine', template_engine);
    });

    app.use(function (req, res, next) {
        //Set caching
        res.setHeader('Last-Modified', (new Date()).toUTCString());
        res.setHeader('Expires', 0);
        res.setHeader('cache-control', 'no-cache');

        //Pagination
        if (req.query.No) {
            if (req.query.No < 0) {
                //The pagination index is a negative number.  We dont accept this.  Change it to 0.
                req.query.No = 0;
            }
        }
        next();
    });
    app.use(express.bodyParser());
};
