define([
  // Libraries.
  "jquery",
  "lesson2/libs/lodash",
  "backbone",
  "salt",

  // Plugins.
  "lesson2/plugins/backbone.layoutmanager"
],

function($, _, Backbone, SALT) {
  /* Analytics */
  var wt = {
    _ready: $.Deferred(),
    _preloadCheck: $.Deferred(),

    defaults: {
      name: 'master-your-plastic'
    },

    trigger: function(eventName, object, force) {
      var self = this,
          preloadCheckPending = (this._preloadCheck.state() === 'pending'),
          additionalDetails = _.extend(this.defaults, {});

      var userInfo = {
        state: Backbone.Asa.User.state,
        siteMemberId: Backbone.Asa.User.siteMemberId
      };

      additionalDetails = $.extend(true, additionalDetails, userInfo);

      if (object) {
        additionalDetails = $.extend(true, additionalDetails, object);
      }

      $.when(force === true ? force : this._preloadCheck, this._ready).done(function() {

        SALT.global.WT.publish(eventName, {
          lesson: additionalDetails
        });
      }, this);
    }
  };

  wt._preloadCheck.name = 'Preload Data';

  // Lesson 1 Analytics Subscriptions
  require(['salt/analytics/webtrends'], function(WT) {
    WT.subscribe('salt/analytics/webtrends/maps/lessons.json').always(function() {
      wt._ready.resolve();
    });
  });

  // Provide a global location to place configuration settings and module
  // creation.
  var app = {
    // The root path to run the application.
    root: "/master-your-plastic/",  //QC-6289
    serverUrl: "/api/LessonsService",
    wt: wt
  };

  // Localize or create a new JavaScript Template object.
  var JST = window.JST = window.JST || {};

  // Configure LayoutManager with Backbone Boilerplate defaults.
  Backbone.LayoutManager.configure({
    // Allow LayoutManager to augment Backbone.View.prototype.
    manage: true,

    paths: {
      layout: "app/templates/layouts/",
      template: "app/templates/"
    },

    fetch: function(path) {
      // Initialize done for use in async-mode
      var done;

      // Concatenate the file extension.
      path = path + ".html";

      // If cached, use the compiled template.
      if (JST[path]) {
        return JST[path];
      } else {
        // Put fetch into `async-mode`.
        done = this.async();

        // Seek out the template asynchronously.
        return $.ajax({ url: path }).then(function(contents) {
          done(JST[path] = _.template(contents));
        });
      }
    }
  });

  // Mix Backbone.Events, modules, and layout management into the app object.
  return _.extend(app, {
    // Create a custom object with a nested Views object.
    module: function(additionalProps) {
      return _.extend({ Views: {} }, additionalProps);
    },

    // Helper for using layouts.
    useLayout: function(name, options){
      // If already using this Layout, then don't re-inject into the DOM.
      if (this.layout && this.layout.options.template === name) {
        return this.layout;
      }

      // If a layout already exists, remove it from the DOM.
      if (this.layout) {
        this.layout.remove();
      }

      // Create a new Layout with options.
      var layout = new Backbone.Layout(_.extend({
        template: name,
        className: "layout " + name,
        id: "layout"
      }, options));


      // Insert into the DOM.
      $("#main").empty().append(layout.el);

      // Render the layout.
      //layout.render();

      // Cache the refererence.
      this.layout = layout;

      // Return the reference, for chainability.
      return layout;
    }
  }, Backbone.Events);
});
