define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/compare/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var Compare = app.module();

  // Define Favorites Model
  Compare.Model = Backbone.Model.extend({

  });

  Compare.CompareList = Backbone.Collection.extend({
    model: Compare.Model,
    localStorage: new Store("Favorites-backbone"),

    nextOrder: function(){
      if (!this.length) {
        return 1;
      }
      return this.last().get("iteration") + 1;
    },

    newAttributes: function(value) {
      var iteration = this.nextOrder();
      return {
        valueID: value,
        iteration: iteration,
        id: 'Favorite' + iteration
      };
    }
  });

  // Attach the Views sub-module into this module.
  Compare.Views = Views;
  
  // Required, return the module for AMD compliance
  return Compare;

});
