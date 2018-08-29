define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/step5/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var step5 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step5.Model = Backbone.Model.extend({

    localStorage: new Store("step5-backbone"),
    url: 'localstorage:',
    // Default attributes for the todo.
    defaults: {
      id: 'step5'
    }
  });

  // Attach the Views sub-module into this module.
  step5.Views = Views;

  // Required, return the module for AMD compliance
  return step5;

});
