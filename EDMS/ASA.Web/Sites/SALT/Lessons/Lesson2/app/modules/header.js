define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/header/views"

],

function(app, Backbone, Views) {

  // Create a new module
  var header = app.module();


  // Our basic **Steps** model has `content` and `done` attributes.
  header.Model = Backbone.Model.extend({
    // Default attributes for the todo.
    defaults: {
      currentStep: 0,
      stepsLength: 2,
      stepName: ''
    },

    // Remove this Todo from *localStorage* and delete its view.
    clear: function() {
      this.destroy();
    }

  });


  // Attach the Views sub-module into this module.
  header.Views = Views;

  // Required, return the module for AMD compliance
  return header;

});
