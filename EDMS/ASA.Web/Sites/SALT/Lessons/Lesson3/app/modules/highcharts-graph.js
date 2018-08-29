define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/highcharts-graph/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var HighChartsGraph = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  HighChartsGraph.Model = Backbone.Model.extend({
    
    // Default attributes for the bar graph
    localStorage: new Store("highChartsGraph-backbone"),

    defaults: {
      id: "highChartsGraph",
      balance: 0,
      expenses: 0,
      purchases: 0
    }

  });

  // Attach the Views sub-module into this module.
  HighChartsGraph.Views = Views;

  // Required, return the module for AMD compliance
  return HighChartsGraph;

});
