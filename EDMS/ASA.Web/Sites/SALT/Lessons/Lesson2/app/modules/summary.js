define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/summary/views"

],

function(app, Backbone, Views) {

  // Create a new module
  var Summary = app.module();

  Summary.Model = Backbone.Model.extend({
    // Default attributes
    defaults: {
      
    }

  });


  // Attach the Views sub-module into this module.
  Summary.Views = Views;

  // Required, return the module for AMD compliance
  return Summary;

});
