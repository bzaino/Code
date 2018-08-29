define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/step3/views"

  // Plugins
  //,"lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views) {

  // Create a new module
  var step3 = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  step3.Model = Backbone.Model.extend({

    localStorage: new Store("step3-backbone"),
    url: 'localStorage:',
    // Default attributes.
    defaults: {
      name: '',
      number: 0,
      time: 0,
      id: 'step3'
    }

  });

  // Attach the Views sub-module into this module.
  step3.Views = Views;

  // Required, return the module for AMD compliance
  return step3;

});
