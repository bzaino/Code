define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/graph-modifiers/views"

],

function(app, Backbone, Views) {

  // Create a new module
  var graphModifiers = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  graphModifiers.Model = Backbone.Model.extend({


  });


  // Attach the Views sub-module into this module.
  graphModifiers.Views = Views;

  // Required, return the module for AMD compliance
  return graphModifiers;

});
