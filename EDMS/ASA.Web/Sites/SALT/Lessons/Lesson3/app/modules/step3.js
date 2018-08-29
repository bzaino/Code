define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/step3/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var step3 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step3.Model = Backbone.Model.extend({

    localStorage: new Store("step3-backbone"),
    url: 'localstorage:',
    // Default attributes for the todo.
    defaults: {
      id: 'step3'
    },

    // Remove this Todo from *localStorage* and delete its view.
    clear: function() {
      this.destroy();
    }

    
  });

  // Attach the Views sub-module into this module.
  step3.Views = Views;

  // Required, return the module for AMD compliance
  return step3;

});
