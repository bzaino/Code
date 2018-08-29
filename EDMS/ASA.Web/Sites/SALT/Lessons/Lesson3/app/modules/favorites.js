define([
  "lesson3/app",

  // Libs
  "backbone",

  // Views
  "lesson3/modules/favorites/views",

  // Plugins
  "lesson3/plugins/backbone-asa-persistence"
],

function(app, Backbone, Views, LocalStorage) {

  // Create a new module
  var Favorites = app.module();

  // Define Favorites Model
  Favorites.Model = Backbone.Model.extend({
    url: app.serverUrl +'/Lesson3/Favorite/',
    updateForServer: function() {
      var updatedProps = {
        RepaymentName: this.get('valueID'),
        
      };
      this.set(updatedProps,{silent: true});
    }
  });

  Favorites.FavoritesList = Backbone.Collection.extend({
    model: Favorites.Model,
    localStorage: new Store("Favorites-backbone"),
    url: app.serverUrl+'/Lesson3/Favorite',
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
        iteration: iteration
      };
    },

    parse: function(resp) {
      if(resp != null){
        for(var i =0;i<resp.length;i++){
          //Need to increment since the number returned by next Order is based on the number of elements in the collection,
          // and at this point we havent added the new element
          resp[i].iteration = this.nextOrder() + i;
          resp[i].id = resp[i].FavoriteId;
          resp[i].valueID = resp[i].RepaymentName;
           
        }
      }
      
      return resp;
    }
  });

  // Attach the Views sub-module into this module.
  Favorites.Views = Views;
  
  // Required, return the module for AMD compliance
  return Favorites;

});
