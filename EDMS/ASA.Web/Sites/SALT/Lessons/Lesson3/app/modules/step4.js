define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/step4/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var step4 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step4.Model = Backbone.Model.extend({

    localStorage: new Store("step4-backbone"),
    url: 'localstorage:',
    // Default attributes for the todo.
    defaults: {
      id: 'step4'
    },


    
  });

  // Attach the Views sub-module into this module.
  step4.Views = Views;

  // Required, return the module for AMD compliance
  return step4;

});
