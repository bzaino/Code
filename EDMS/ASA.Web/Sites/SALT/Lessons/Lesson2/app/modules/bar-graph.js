define([
  "lesson2/app",

  // Libs
  "backbone",

  // Views
  "lesson2/modules/bar-graph/views"

  // Plugins
  //,"lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views) {

  // Create a new module
  var BarGraph = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  BarGraph.Model = Backbone.Model.extend({
    
    // Default attributes for the bar graph
    //localStorage: new Store("barGraph-backbone"),

    defaults: {
      id: "barsGraphValues",
      balance: 0,
      expenses: 0,
      purchases: 0
    }

  });

  // Attach the Views sub-module into this module.
  BarGraph.Views = Views;

  // Required, return the module for AMD compliance
  return BarGraph;

});
