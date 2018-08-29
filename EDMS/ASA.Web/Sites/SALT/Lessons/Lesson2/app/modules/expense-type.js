define([
  "lesson2/app",

  // Libs
  "backbone",

  "lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone) {

  // Create a new module
  var ExpenseType = app.module();

  
  ExpenseType.Model = Backbone.Model.extend({

    url: app.serverUrl + '/ExpenseType/',
    // Default attributes.
    defaults: {

      
    },
    // parse: function(resp,xhr)
    // {
      
    // }
  
  });

  

  
  return ExpenseType;

});
