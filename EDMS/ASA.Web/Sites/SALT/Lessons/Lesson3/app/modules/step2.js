define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/step2/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var step2 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step2.Model = Backbone.Model.extend({

    localStorage: new Store("step2-backbone"),
    url: 'localstorage:',
    // Default attributes for the todo.
    defaults: {
      id: 'step2'
    },

    // Remove this Todo from *localStorage* and delete its view.
    clear: function() {
      this.destroy();
    }

    
  });

  // Attach the Views sub-module into this module.
  step2.Views = Views;

  // Required, return the module for AMD compliance
  return step2;

});
