/*!
 * SALT JavaScript Library
 * =======================
*/
define([
    'underscore',
    'backbone'
], function (_, Backbone) {
    /**
     * Create an object that inherits from backbone events to serve as mediator for the application.
     */
    var SALT = _.extend({}, Backbone.Events);

    /**
     * Provide pub/sub vocabulary.
     */
    SALT.subscribe = SALT.sub = SALT.on;
    SALT.publish = SALT.pub = SALT.trigger;

    return SALT;
});
