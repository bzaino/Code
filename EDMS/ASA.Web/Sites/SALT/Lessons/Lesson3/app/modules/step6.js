define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/step6/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var step6 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step6.Model = Backbone.Model.extend({

    localStorage: new Store("step6-backbone"),
    url: 'localstorage:',
    // Default attributes for the todo.
    defaults: {
      id: 'step6'
    }
    
  });

  // Attach the Views sub-module into this module.
  step6.Views = Views;

  // Required, return the module for AMD compliance
  return step6;

});
