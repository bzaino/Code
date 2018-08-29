define([
  "lesson2/app",

  // Libs
  "backbone"
],

function(app, Backbone) {
  
  var Views = {};

 

  Views.Footer = Backbone.View.extend({
    template: "nav/footer",
    className: 'wrapper',
        
    initialize: function() {
      
    },
    afterRender: function(){
      if(this.model.attributes.noPrev){
        $('.left-button').hide();
      }
      
      if(this.model.attributes.showSum == false){
        $('#footer .left').hide();
        
      }
    },
    serialize: function() {
      return {
        amount: this.model.get("amount"),
        description: this.model.get("description"),
        subtitle: this.model.get("subtitle"),
        nextButton: this.model.get("nextButton"),
        next: this.model.get("next")
      };
    }
  });

  return Views;

});