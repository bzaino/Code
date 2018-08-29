define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/step2/views"

  // Plugins
  //,"lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views) {

  // Create a new module
  var step2 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step2.Model = Backbone.Model.extend({

    localStorage: new Store("step2-backbone"),
    url: 'localstorage:',
    // Default attributes.
    defaults: {
      testValue: '',
      hasImported: false,
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
