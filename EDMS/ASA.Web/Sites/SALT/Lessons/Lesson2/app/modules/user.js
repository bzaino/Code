define([
  "lesson2/app",

  // Libs
  "backbone",

  "lesson2/plugins/backbone-asa-persistence"
],

function(app, Backbone) {

  // Create a new module
  var user = app.module();

  // Our basic **Steps** model has `content` and `done` attributes.
  user.Model = Backbone.Model.extend({

    url: app.serverUrl + '/Lesson2/User/',
    // Default attributes.
    defaults: {

      Lesson1Step: 0,
      Lesson2Step: 0,
      Lesson3Step: 0,
      
    },
    parse: function(resp,xhr)
    {
      if(resp != null) {
        resp.id = resp.UserId;
        return resp;
      }
    }
  });

  

  
  return user;

});
