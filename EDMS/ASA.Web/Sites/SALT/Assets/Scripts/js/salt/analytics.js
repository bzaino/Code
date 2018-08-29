/*!
 * SALT / Analytics Abstraction Layer
 */

define([
    'jquery',
    'salt',
    'salt/models/SiteMember',
    'underscore',
    'backbone',
    'dust',
    'backbone.wreqr'
], function ($, SALT, SiteMember, _, Backbone, dust) {

    var loggers = [],
        baseTemplateContext = {};
    // Add window.location properties to base context
    _.each(['hash', 'host', 'hostname', 'href', 'pathname', 'port', 'protocol', 'search'], function (prop) {
        if (!baseTemplateContext.location) {
            baseTemplateContext.location = {};
        }

        baseTemplateContext.location[prop] = window.location[prop];
    });
    // Add document properties to base context
    _.each(['URL', 'baseURI', 'compatMode', 'contentType', 'designMode', 'dir', 'documentURI', 'domain', 'inputEncoding', 'referrer', 'title'], function (prop) {
        if (!baseTemplateContext.document) {
            baseTemplateContext.document = {};
        }

        baseTemplateContext.document[prop] = document[prop];
    });
    /**
     * Recursively renders all string elements as Dust templates using message as template context.
     */
    function renderArray(array, message, callback) {
        var first, rest;

        if (array.length) {
            first = _.first(array);
            rest  = _.rest(array);

            renderConfig(first, message, function (first) {
                if (rest.length) {
                    renderArray(rest, message, function (rest) {
                        if (_.isFunction(callback)) {
                            callback([first].concat(rest));
                        }
                    });
                } else {
                    if (_.isFunction(callback)) {
                        callback([first]);
                    }
                }
            });
        } else {
            callback(array);
        }
    }

    /**
     * Recursively renders all string properties as Dust templates using message as template context.
     */
    function renderObject(object, message, callback) {
        var randomKey = _.first(_.keys(object)),
            random = _.clone(object[randomKey]),
            rest = {}, restKeys = [];

        for (var key in object) {
            if (key !== randomKey) {
                rest[key] = _.clone(object[key]);
                restKeys.push(key);
            }
        }

        renderConfig(random, message, function (random) {
            var renderedRandom = {};
            renderedRandom[randomKey] = random;

            if (restKeys.length) {
                renderObject(rest, message, function (rest) {
                    if (_.isFunction(callback)) {
                        callback(_.extend({}, renderedRandom, rest));
                    }
                });
            } else {
                if (_.isFunction(callback)) {
                    callback(renderedRandom);
                }
            }
        });
    }

    /**
     * Recursively renders all map config strings as Dust templates using message as template context.
     * Supported map types: bare strings, arrays, object literals
     */
    function renderConfig(config, message, callback) {
        if (_.isString(config)) {
            // Render strings
            dust.renderSource(config, message, function (error, string) {
                if (!error && _.isFunction(callback)) {
                    callback(string);
                }
            });
        } else if (_.isArray(config)) {
            // Render strings contained in arrays
            renderArray(config, message, function (array) {
                if (_.isFunction(callback)) {
                    callback(array);
                }
            });
        } else if (_.isObject(config) && !_.isFunction(config) && !_.isDate(config) && !_.isBoolean(config) && !_.isNumber(config) && !_.isRegExp(config) && !_.isArguments(config)) {
            // Render strings contained in object literals
            renderObject(config, message, function (object) {
                if (_.isFunction(callback)) {
                    callback(object);
                }
            });
        } else {
            // Everything else passes through as-is
            if (_.isFunction(callback)) {
                callback(config);
            }
        }
    }

    function registerSubscriptions(logger, subscriptions) {
        var subscribing = $.Deferred();

        function subSuccess() {
            subscribing.resolve();
        }

        function subFailure() {
            subscribing.reject();
        }

        if (_.isString(subscriptions)) {

            require(['json!' + subscriptions], function (subscriptionsJSON) {
                registerSubscriptions(logger, subscriptionsJSON).done(subSuccess).fail(subFailure);
            });
        } else if (_.isArray(subscriptions)) {
            _.each(subscriptions, function (subscription) {
                registerSubscriptions(logger, subscription).done(subSuccess).fail(subFailure);
            });
        } else if (_.isObject(subscriptions)) {
            $.when.apply($, _.map(subscriptions, function (config, topic) {
                return logger.subscribe(topic, config);
            })).done(subSuccess).fail(subFailure);
        } else {
            subscribing.reject();
        }

        return subscribing.promise();
    }

    var Analytics = new (Backbone.View.extend({
        /**
         * Starts logging on all application-level events.
         */
        initialize: function () {
            SALT.subscribe('all', function (topic, message) {
                this.log(topic, message);
            }, this);
        },

        /**
         * Logs a topic and message with all registered loggers.
         */
        log: function (topic, message) {
            _.each(loggers, function (logger) {
                logger.publish(topic, message);
            });
        },

        /**
         * Registers a new logger with the list of registered analytics loggers.
         * If the logger is already registered, the new logger replaces the old logger.
         *
         * All logger map strings are passed through the Dust renderer using the published message as context.
         */
        registerLogger: function (loggerConfig) {
            var newLogger,
                subscriptions   = loggerConfig.map,
                templateContext = $.extend(true, {}, baseTemplateContext, loggerConfig.context);

            loggerConfig.map     = undefined;
            loggerConfig.context = undefined;

            // Add tracker/track vocabulary alongside logger/log.
            if (loggerConfig.log) {
                loggerConfig.track = loggerConfig.log;
            } else {
                loggerConfig.log   = loggerConfig.track;
            }

            // Create the new logger instance as its own sandboxed event aggregator,
            // capable of managing its own set of event maps.
            newLogger = new (Backbone.Wreqr.EventAggregator.extend(loggerConfig))();
            newLogger.publish = newLogger.pub = newLogger.trigger;

            // Provide ability to add new subscriptions on-the-fly.
            // Format: [event name], [logger map config], [optional context]
            newLogger.subscribe = newLogger.sub = function (topic, config, context) {
                var subscribing;

                if (arguments.length === 1) {
                    return registerSubscriptions(newLogger, arguments[0]);
                } else {
                    context = context ? context : this;

                    subscribing = $.Deferred();

                    newLogger.on(topic, function (message) {
                        if (_.isFunction(newLogger.log)) {
                            renderConfig(config, $.extend(true, {}, templateContext, message, { siteMember: SiteMember.toJSON() }), function (renderedConfig) {
                                newLogger.log(renderedConfig);
                            });
                        }
                    }, context);

                    subscribing.resolve();
                    return subscribing.promise();
                }
            };

            // Remove the old logger, if it exists.
            loggers = _.reject(loggers, function (logger) {
                return (logger.id === newLogger.id);
            });

            // Register all configured subscriptions to the new logger.
            registerSubscriptions(newLogger, subscriptions);

            // Add the new logger.
            loggers.push(newLogger);

            // Call the new logger's initialize method, if it exists.
            if (_.isFunction(newLogger.initialize)) {
                newLogger.initialize();
            }

            return newLogger;
        }
    }))();

    return Analytics;
});
